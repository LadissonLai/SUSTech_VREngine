using Framework;
using System.Collections;
using System.Collections.Generic;
using System;
using Doozy.Engine;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class ErrorCache : Dictionary<string, List<ErrorRecordType>>
    {
        public ErrorCache(string param, ErrorRecordType errorRecordType)
        {
            Add(param, new List<ErrorRecordType>() { errorRecordType });
        }
    }

    public class ErrorData
    {
        public float scoreDetucting;

        public string allError;

        public ErrorData(float score, string error)
        {
            scoreDetucting = score;

            allError = error;
        }
    }

    public class RecordModel : IRecordModel
    {
        /// <summary>
        /// 记录已经完成的参数
        /// </summary>
        Dictionary<RecordStepType, HashSet<string>> paramCaches = new Dictionary<RecordStepType, HashSet<string>>();

        List<string> completedRecords = new List<string>();

        RecordCsvConfig cfg = World.Get<RecordCsvConfig>();

        RecordErrorCsvConfig errorCfg = World.Get<RecordErrorCsvConfig>();

        /// <summary>
        /// 错误的记录
        /// </summary>
        Dictionary<RecordStepType, ErrorCache> recordErrorCaches = new Dictionary<RecordStepType, ErrorCache>();
 
        Dictionary<string, ErrorData> errorRecords = new Dictionary<string, ErrorData>();

        /// <summary>
        /// 已经分割过的全部参数 recordID  /  参数ID
        /// </summary>
        Dictionary<string, string[]> paramSplitCaches = new Dictionary<string, string[]>();

        RefreshRecordItemStateMessage recordItemStateMessage = new RefreshRecordItemStateMessage();

        public void PrintRecordList()
        {
            foreach (string id in completedRecords)
                DebugEx.Log($"completedRecord：{id}");

            foreach (var cache in paramCaches)
            {
                foreach (string param in cache.Value)
                    DebugEx.Log($"uncompletedParams:{param}-");
            }
        }

        /// <summary>
        /// 记录多个参数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns>是否全部成功</returns>
        public virtual bool Record(RecordStepType type, params string[] args)
        {
            bool recordFlag = true;

            foreach (string param in args)
            {
                if (!Record(type, param))
                    recordFlag = false;
            }
            return recordFlag;
        }

        /// <summary>
        /// 目前重复记录 返回false
        /// </summary>
        /// <param name="type"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool Record(RecordStepType type, string arg)
        {
            if (paramCaches.ContainsKey(type))
            {
                if (!paramCaches[type].AddUnique(arg))
                {
                    //重复记录属于正常逻辑，不需要log输出。
                    //DebugEx.Log($" 记录 {arg} 已完成，不再记录 ");

                    return false;
                }
            }
            else
            {
                paramCaches.Add(type, new HashSet<string>() { arg });
            }

            Message.Send(recordItemStateMessage);

            return true;
        }
 
        public bool CheckRecordCompleted(string recordID)
        {
            if (completedRecords.Contains(recordID))
                return true;

            var result = FindRecord(recordID);

            Debug.Assert(result != null, $"recordID未找到:{recordID}");

            var type = result.Type;

            //不存在此RecordStepType  （已经完成该类型全部记录）
            if (!paramCaches.ContainsKey(type))
                return false;

            //todo 尝试优化不同RecordStepType 相同allParam 的workingParams 
            if (!paramSplitCaches.TryGetValue(recordID, out string[] allParam))
                allParam = result.Params.Split(',');

            foreach (string param in allParam)
            {
                if (!paramCaches[type].Contains(param))
                {
                    if (!paramSplitCaches.ContainsKey(recordID))
                        paramSplitCaches.Add(recordID, allParam);

                    return false;
                }
            }

            //paramCaches 去除 allParam    为空 清空

            foreach (string param in allParam)
            {
                paramCaches[type].Remove(param);
            }

            if (paramCaches[type].Count == 0)
                paramCaches.Remove(type);

            paramSplitCaches.Remove(recordID);

            completedRecords.Add(recordID);

            return true;
        }

        /// <summary>
        /// 记录每条子步骤的工具对错
        /// </summary>
        /// <param name="type"></param>
        /// <param name="daObjID"></param>
        public bool RecordError(RecordStepType type, string daObjID, ErrorRecordType errorType)
        {
            if (recordErrorCaches.ContainsKey(type))
            {
                if (recordErrorCaches[type].ContainsKey(daObjID))
                    return recordErrorCaches[type][daObjID].AddUnique(errorType);
                else
                    recordErrorCaches[type].Add(daObjID, new List<ErrorRecordType>() { errorType });
            }
            else
                recordErrorCaches.Add(type, new ErrorCache(daObjID, errorType));

            return true;
        }

        public void ClearRecord()
        {
            paramCaches.Clear();

            completedRecords.Clear();

            recordErrorCaches.Clear();

            errorRecords.Clear();
        }
         
        public void SetRecordCompleted(string recordID)
        {
            completedRecords.AddUnique(recordID);
        }

        public float GetRecordErrorScoreDeducting(string recordID)
        {
            PickupErrorRecords(recordID);

            if (errorRecords.TryGetValue(recordID, out ErrorData data))
                return data.scoreDetucting;

            return 0;
        }

        public string GetRecordAllErrors(string recordID)
        {
            PickupErrorRecords(recordID);

            if (errorRecords.TryGetValue(recordID, out ErrorData data))
                return data.allError;

            return null;
        }

        /// <summary>
        /// 收集错误记录信息
        /// </summary>
        /// <param name="recordID"></param>
        /// <returns></returns>
        bool PickupErrorRecords(string recordID)
        {
            if (errorRecords.ContainsKey(recordID))
                return true;

            var result = FindRecord(recordID);

            var type = result.Type;

            //不存在此RecordStepType  （已经完成该类型全部记录）
            if (!recordErrorCaches.ContainsKey(type))
                return false;

            if (!paramSplitCaches.TryGetValue(recordID, out string[] allParam))
                allParam = result.Params.Split(',');

            foreach (string param in allParam)
            {
                if (recordErrorCaches[type].ContainsKey(param))
                {
                    float score = 0;

                    string error = "";

                    foreach (var item in recordErrorCaches[type][param])
                    {
                        var data = errorCfg.FindRowDatas(((int)item).ToString());
                       
                        if (data == null)
                        {
                            DebugEx.Log($"PickupErrorRecords item:{((int)item)} param :{param} type:{type} ");
                            continue;
                        }

                        score += data.Score;
                        string title = data.Title;

                        if (string.IsNullOrWhiteSpace(title))
                            continue;

                        error = $"{error}、{title}";
                    }

                    if (!string.IsNullOrWhiteSpace(error))
                        error = error.Substring(1);

                    errorRecords.Add(recordID, new ErrorData(score, error));


                    //Clear
                    paramSplitCaches.Remove(recordID);

                    foreach (string para in allParam)
                        recordErrorCaches[type].Remove(para);

                    if (recordErrorCaches[type].Count == 0)
                        recordErrorCaches.Remove(type);

                    return true;
                }
            }

            return false;
        }

        public RecordCsvConfig.Item FindRecord(string id)
        {
            return cfg.FindRowDatas(id);
        }
    }


    public class DARecordErrorMessage : Message
    {
        public ErrorRecordType errorType;

        public RecordStepType recordStep;

        public string param;
    }


    public class RefreshRecordItemStateMessage : Message { }
}
