using Doozy.Engine;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fxb.CMSVR
{
    public sealed class TaskModel : ITaskModel
    {
        /// <summary>
        /// 所有的任务数据
        /// </summary>
        List<TaskItemData> taskItemDatas = new List<TaskItemData>();

        TaskCsvConfig cfgs = World.Get<TaskCsvConfig>();

        TaskStepGroupCsvConfig stepCfgs = World.Get<TaskStepGroupCsvConfig>();

        IRecordModel recordModel = World.Get<IRecordModel>();

        List<string> completedStepGroupIDs = new List<string>();

        Dictionary<string, string[]> stepGroupCache = new Dictionary<string, string[]>();

        const string EQUIPSTEPID = "1001";

        const string LOWPOWERSTEPID = "32";

        const string HIGHPOWERSTEPID = "41";

        bool isSubmitAllTask;

        public bool IsSubmitAllTask { get { return isSubmitAllTask; } }

        float totalScore;

        public float TotalScore { get { return totalScore; } }

        List<StepGroupData> InitStepGroup(float taskScore, string groupIDs, TaskStepGroupCsvConfig cfg, IRecordModel recordDatas)
        {
            List<StepGroupData> stepGroups = new List<StepGroupData>();

            string[] ids = groupIDs.Split(',');

            float totalWeight = 0;

            float weightScore;

            foreach (string id in ids)
            {
                var row = cfg.FindRowDatas(id);

                if(row==null)
                {
                    DebugEx.Error($"StepGroup:{id} not found in TaskStepGroupCsvConfig");

                    continue;
                }    

                StepGroupData groupData = new StepGroupData()
                {
                    id = id,
                };

                totalWeight += row.Weight;

                //暂时存储权重
                groupData.score = row.Weight;

                stepGroups.Add(groupData);
            }

            //计算分数
            weightScore = taskScore / totalWeight;

            foreach (var item in stepGroups)
            {
                item.score *= weightScore;

                item.score = (float)Math.Round(item.score, 1);
            }

            return stepGroups;
        }

        void Clear()
        {
            taskItemDatas.Clear();

            stepGroupCache.Clear();
        }

        List<string> GetAllStepIDs()
        {
            List<string> stepIDs = new List<string>();

            foreach (var item in taskItemDatas)
            {
                for (int i = 0; i < item.stepGroups.Count; i++)
                    stepIDs.AddUnique(item.stepGroups[i].id);
            }

            return stepIDs;
        }

        bool TryAddPowerOn(List<string> idCaches)
        {
            if (idCaches.Count != taskItemDatas.Count)
                return false;

            bool lowPower = false;

            bool highPower = false;

            //lowPowerStepID 居首 highPowerStepID 居中 末尾
            foreach (var item in idCaches)
            {
                if (!lowPower)
                    lowPower = item.Contains($"{LOWPOWERSTEPID},");

                if (!highPower)
                {
                    highPower = item.Contains($",{HIGHPOWERSTEPID},");

                    if (!highPower)
                        highPower = item.Substring(item.LastIndexOf(',') + 1) == HIGHPOWERSTEPID;
                }

                if (lowPower && highPower)
                    break;
            }

            if (highPower)
                idCaches[idCaches.Count - 1] = $"{idCaches[idCaches.Count - 1]},1002,42,43";

            if (lowPower)
                idCaches[idCaches.Count - 1] = $"{idCaches[idCaches.Count - 1]},38";

            //低压或者高压都表示添加成功
            if (lowPower || highPower)
                return true;
            else
                return false;
        }

        #region  public 

        public IReadOnlyList<TaskItemData> GetData()
        {
            return taskItemDatas;
        }

        public TaskModel(IEnumerable<string> taskIDs)
        {
            //已经设置过任务ID
            if (taskItemDatas.Count != 0)
                return;

            foreach (string task in taskIDs)
            {
                TaskItemData taskItemData = new TaskItemData()
                {
                    taskID = task
                };

                taskItemDatas.Add(taskItemData);
            }
        }

        /// <summary>
        /// 进入DAScene后初始化
        /// </summary>
        public void Init()
        {
            if (cfgs == null)
                cfgs = World.Get<TaskCsvConfig>();

            if (stepCfgs == null)
                stepCfgs = World.Get<TaskStepGroupCsvConfig>();

            if (recordModel == null)
                recordModel = World.Get<IRecordModel>();

            float totalWeight = 0;

            float weightScore;

            List<string> stepIDCaches = new List<string>();

            foreach (TaskItemData itemData in taskItemDatas)
            {
                var row = cfgs.FindRowDatas(itemData.taskID);

                //无用log关闭
                //DebugEx.Log(itemData.taskID);

                itemData.taskTitle = row.Title;

                itemData.taskDescription = row.Description;

                totalWeight += row.Weight;

                //暂时存储权重
                itemData.score = row.Weight;

                string stepids = row.StepGroupID;

                ////添加在第一步
                //if (isAddSafetyEquip)
                //{
                //    stepids = $"{EQUIPSTEPID},{stepids}";

                //    isAddSafetyEquip = false;
                //}

                //暂时存储步骤组id
                stepIDCaches.Add(stepids);
            }

            //添加低压高压上电步骤
            //bool flag = TryAddPowerOn(stepIDCaches);

            //DebugEx.Log($"添加上电步骤{flag}");

            //计算分数
            weightScore = 100 / totalWeight;

            for (int i = 0; i < taskItemDatas.Count; i++)
            {
                taskItemDatas[i].score *= weightScore;

                taskItemDatas[i].score = (float)Math.Round(taskItemDatas[i].score, 1);

                taskItemDatas[i].stepGroups = InitStepGroup(taskItemDatas[i].score, stepIDCaches[i], stepCfgs, recordModel);
            }

            totalScore = 100;

            isSubmitAllTask = false;
        }

        public List<string> GetRecordIDs(string taskID)
        {
            TaskItemData itemData = taskItemDatas.Find((item) => item.taskID == taskID);

            List<string> ids = new List<string>();

            foreach (var stepGroup in itemData.stepGroups)
            {
                string[] childStepIDs = GetChildStepIDs(stepGroup.id);

                ids.AddRange(childStepIDs);
            }

            return ids;
        }

        public List<string> GetTaskIDs()
        {
            List<string> ids = new List<string>();

            foreach (var task in taskItemDatas)
            {
                ids.Add(task.taskID);
            }

            return ids;
        }

        public bool CheckTaskCompleted(string taskID)
        {
            TaskItemData itemData = taskItemDatas.Find((item) => item.taskID == taskID);

            if (!itemData.isCompleted)
            {
                foreach (var stepGroup in itemData.stepGroups)
                {
                    if (!CheckStepGroupCompleted(stepGroup.id))
                        return itemData.isCompleted;
                }

                itemData.isCompleted = true;
            }

            return itemData.isCompleted;
        }

        public bool CheckAllTaskCompleted()
        {
            foreach (TaskItemData taskItemData in taskItemDatas)
            {
                if (!CheckTaskCompleted(taskItemData.taskID))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 全部完成返回空
        /// </summary>
        /// <returns></returns>
        public List<string> GetFirstUnCompletedTaskRecord()
        {
            foreach (var item in taskItemDatas)
            {
                if (!CheckTaskCompleted(item.taskID))
                    return GetRecordIDs(item.taskID);
            }

            return null;
        }

        public bool CheckStepGroupCompleted(string groupID)
        {
            if (groupID == null)
                return false;

            if (completedStepGroupIDs.Contains(groupID))
                return true;

            string[] childs = GetChildStepIDs(groupID);

            foreach (string id in childs)
            {
                if (!recordModel.CheckRecordCompleted(id))
                    return false;
            }

            completedStepGroupIDs.Add(groupID);

            return true;
        }

        public string GetStepGroupDescription(string groupID)
        {
            return stepCfgs.FindRowDatas(groupID).Description;
        }

        public string[] GetChildStepIDs(string groupID)
        {
            var result = stepCfgs.FindRowDatas(groupID).StepIDs;

            if (result == null)
            {
                DebugEx.Error($"{groupID} not found in TaskStepGroupCsvConfig");

                return null;
            }

            //减少分割字符串
            if (result.Length == 1)
                return new string[] { result };

            if (!stepGroupCache.ContainsKey(groupID))
                stepGroupCache.Add(groupID, result.Split(','));

            return stepGroupCache[groupID];
        }

        //public DetailTopData GetTaskDetailTopData()
        //{
        //    var row = World.Get<SoftwareCsvConfig>().GetRowDatas(0);

        //    DetailTopData detailTopData = new DetailTopData()
        //    {
        //        carName = row[(int)SoftwareCsvConfig.Key.Channel],

        //        miles = row[(int)SoftwareCsvConfig.Key.Miles],

        //        color = row[(int)SoftwareCsvConfig.Key.Color],

        //        dateProduction = row[(int)SoftwareCsvConfig.Key.ProductDate],

        //        framework = row[(int)SoftwareCsvConfig.Key.Framework],

        //        icon = row[(int)SoftwareCsvConfig.Key.Icon]
        //    };

        //    return detailTopData;
        //}

        public void SubmitTask()
        {
            isSubmitAllTask = true;

            foreach (var item in taskItemDatas)
            {
                for (int i = 0; i < item.stepGroups.Count; i++)
                {
                    //统计未做的记录

                    StepGroupData stepGroupData = item.stepGroups[i];

                    if (!CheckStepGroupCompleted(stepGroupData.id))
                    {
                        stepGroupData.dockScore = stepGroupData.score;

                        totalScore -= stepGroupData.dockScore;

                        //totalScore = (float)Math.Round(totalScore, 1);

                        continue;
                    }


                    //统计错误工具的记录
                    string[] recordIDs = GetChildStepIDs(stepGroupData.id);

                    for (int j = 0; j < recordIDs.Length; j++)
                    {
                        var score = recordModel.GetRecordErrorScoreDeducting(recordIDs[j]);

                        stepGroupData.dockScore += score;
                    }

                    totalScore -= stepGroupData.dockScore;
                }
            }

            totalScore = (float)Math.Round(totalScore, 1);


            //修正分数
            if (totalScore < 0)
                totalScore = 0;
            else //全部步骤都没完成 0分
            {
                foreach (var item in taskItemDatas)
                {
                    for (int i = 0; i < item.stepGroups.Count; i++)
                    {
                        if (CheckStepGroupCompleted(item.stepGroups[i].id))
                            return;
                    }
                }

                totalScore = 0;
            }
        }

        public string GetTarget(string taskID)
        {
            return cfgs.FindRowDatas(taskID).Targets;
        }

        //public List<string> GetPropRelatedDAIDs(string taskID, bool ignoreSmallParts = false)
        //{
        //    var record = GetRecordIDs(taskID);

        //    var daCfg = World.Get<DACsvConfig>();

        //    //全部ID
        //    List<string> daIDs = new List<string>();

        //    for (int i = 0; i < record.Count; i++)
        //    {
        //        var param = recordModel.GetRecordParams(record[i]);

        //        if (string.IsNullOrEmpty(param))
        //        {
        //            DebugEx.Error($"{record[i]}没有param");

        //            continue;
        //        }

        //        //保留背包ID
        //        var ids = param.Split(',');

        //        for (int j = 0; j < ids.Length; j++)
        //        {
        //            var propID = daCfg.Find((int)DACsvConfig.Key.PropID, ids[j]);

        //            if (string.IsNullOrEmpty(propID))
        //                continue;

        //            if (ignoreSmallParts && daCfg.Find((int)DACsvConfig.Key.SmallParts, ids[j]) == "1")
        //                continue;

        //            daIDs.Add(ids[j]);
        //        }
        //    }

        //    return daIDs;
        //}

        public List<string> GetPropRelatedDAIDs(string taskID, bool ignoreSmallParts = false)
        {
            string id = cfgs.FindRowDatas(taskID).ObfuscatedID;

            if (string.IsNullOrEmpty(id))
            {
                DebugEx.Log($"{taskID} 没有混淆ID");

                return new List<string>();
            }


            return id.Split(',').ToList();
        }

        public void SkipTaskSteps(string[] stepIDs)
        {
            var ids = stepIDs.ToList();

            foreach (var item in taskItemDatas)
            {
                for (int i = 0; i < item.stepGroups.Count; i++)
                {
                    if (stepIDs.Contains(item.stepGroups[i].id))
                    {
                        var recordIDs = GetChildStepIDs(item.stepGroups[i].id);

                        for (int j = 0; j < recordIDs.Length; j++)
                        {
                            recordModel.SetRecordCompleted(recordIDs[j]);
                        }

                        ids.Remove(item.stepGroups[i].id);

                        if (ids.Count == 0)
                            return;
                    }
                }
            }
        }

        #endregion
    }


    public class TaskItemData
    {
        public string taskID;

        public string taskTitle;

        public string taskDescription;

        public List<StepGroupData> stepGroups;

        /// <summary>
        /// 根据权重计算得到的分数
        /// </summary>
        public float score;

        public bool isCompleted;

        public bool isExpanded;
    }


    public class StepGroupData
    {
        public string id;

        /// <summary>
        /// 根据权重计算得到的分数
        /// </summary>
        public float score;

        /// <summary>
        /// 扣除的分数
        /// </summary>
        public float dockScore;

        public bool isExpanded;
    }
}
