using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public interface ITaskModel
    {
        IReadOnlyList<TaskItemData> GetData();

        List<string> GetTaskIDs();

        List<string> GetRecordIDs(string taskID);

        bool CheckTaskCompleted(string taskID);

        bool CheckStepGroupCompleted(string groupID);

        bool CheckAllTaskCompleted();

        List<string> GetFirstUnCompletedTaskRecord();

        //void SetTaskID(IEnumerable<string> taskIDs);

        void Init();

        string GetStepGroupDescription(string groupID);

        bool IsSubmitAllTask { get; }

        void SubmitTask();

        float TotalScore { get; }

        /// <summary>
        /// 获取车辆详情数据
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        //DetailTopData GetTaskDetailTopData();

        string[] GetChildStepIDs(string groupID);

        /// <summary>
        /// 获取任务目标
        /// </summary>
        /// <param name="taskID"></param>
        /// <returns></returns>
        string GetTarget(string taskID);

        List<string> GetPropRelatedDAIDs(string taskID, bool ignoreSmallParts = false);

        void SkipTaskSteps(string[] stepIDs);
    }

}
