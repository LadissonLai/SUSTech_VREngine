using Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 获取当前指引的记录项内容
    /// 根据指定的记录项进行指引。 
    /// 指引的内容包括：左上角长段文字   场景内的物体高亮（可交互物体）
    /// </summary>
    public sealed class DATaskGuide : MonoBehaviour
    {
        private ITaskModel TaskModel => World.Get<ITaskModel>();

        private IRecordModel RecordModel => World.Get<IRecordModel>();

        private DASceneState DASceneState => World.Get<DASceneState>();

        private GuideStepFactory stepFactory;

        private AbstractGuideStep cutGuideStep;
        
        private List<string> taskRecords;

        /// <summary>
        /// 目前任务刷新需要刷新场景
        /// </summary>
        bool isTaskInited;

        public string Current
        {
            get;
            private set;
        }

        public bool IsStepCompleted => cutGuideStep == null || cutGuideStep.IsCompleted;

        public bool MoveNext()
        {
            if (DASceneState.isTaskPreparing || DASceneState.taskMode == DaTaskMode.Examination)
            {
                return false;
            }

            if (!isTaskInited && taskRecords.Count == 0)
            {
                var allTasks = TaskModel.GetTaskIDs();

                foreach (var task in allTasks)
                {
                    taskRecords.AddRange(TaskModel.GetRecordIDs(task));
                }

                isTaskInited = true;

                return true;
            }

            string cutRecord = null;

            if (TaskModel.IsSubmitAllTask && taskRecords.Count > 0)
                taskRecords.Clear();

            if (taskRecords.Count > 0)
            {
                var stepComplete = (cutGuideStep == null || cutGuideStep.IsCompleted);

                var recordCompleted = RecordModel.CheckRecordCompleted(taskRecords[0]);

                //指引完成条件：当前指引步骤完成并且当前记录项也完成
                if (stepComplete && recordCompleted)
                {
                    taskRecords.RemoveAt(0);
                }
            }

            if (taskRecords.Count > 0)
                cutRecord = taskRecords[0];

            if (Current != cutRecord)
            {
                Current = cutRecord;

                return true;
            }

            return false;
        }

        private void UpdateGuideByRecord(string recordID)
        {
            stepFactory = stepFactory ?? new GuideStepFactory();

            if (cutGuideStep != null)
            {
                stepFactory.ClearGuideStep(cutGuideStep);

                cutGuideStep = null;
            }

            if (recordID != null)
            {
                DASceneState.isGuiding = true;

                cutGuideStep = stepFactory.CreateGuideStep(recordID);
            }
            else
            {
                DASceneState.isGuiding = false;
            }
        }

        private void Update()
        {
            if (cutGuideStep != null)
            {
                cutGuideStep.Tick();
            }

            if (MoveNext())
            {
                UpdateGuideByRecord(Current);
            }
        }

        private void Awake()
        {
            //targetInteractObjs = new List<FocusOnlyInteractableObj>();

            stepFactory = new GuideStepFactory();

            taskRecords = new List<string>();

            //spTipMessage = new GuideSpecialTipMessage();
        }
        
        private void OnDestroy()
        {
            stepFactory.Destroy();

            if (DASceneState != null)
                DASceneState.isGuiding = false;
        }
    }
}