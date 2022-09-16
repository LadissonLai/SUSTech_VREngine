using Fxb.CPTTS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using UnityEngine.U2D;
using Doozy.Engine.UI;
using Fxb.SpawnPool;
using TMPro;
using Doozy.Engine;
using System;

namespace Fxb.CMSVR
{
    public class TaskGroupingModeView : PadViewBase
    {
        public Transform taskTemplate;

        public RectTransform scrollContent;

        TaskCsvConfig taskCfg;

        private List<string> ids;

        public Button enterBtn;

        private ITaskModel TaskModel => World.Get<ITaskModel>();

        private DASceneState SceneState => World.Get<DASceneState>();

        protected override void Start()
        {
            base.Start();

            taskCfg = taskCfg ?? World.Get<TaskCsvConfig>();

            ids = new List<string>();

            enterBtn.onClick.AddListener(OnEnterClick);
            enterBtn.gameObject.SetActive(false);

            InitItem();
        }

        private void OnEnterClick()
        {
            if (ids.Count > 0)
            {
                World.Get<DASceneState>().taskMode = DaTaskMode.GroupingMode;

                World.Get<IRecordModel>().ClearRecord();

                if (World.Get<DASceneState>().taskIDGroupingModeInit != null)
                    World.Get<DASceneState>().taskIDGroupingModeInit.Clear();

                ids.Sort();
                World.Get<DASceneState>().taskIDGroupingModeInit = ids;
#if UNITY_EDITOR
                foreach (var item in SceneState.taskIDGroupingModeInit)
                {
                    DebugEx.Log($"taskIDGroupingMode:{item}");
                }
#endif
                Message.Send(new ReloadDaSceneMessage());
            }
        }

        public void InitItem()
        {
            var row = taskCfg.RowCount;

            taskTemplate.gameObject.SetActive(true);

            for (var index = 0; index < row; index++)
            {
                var taskGroupingModeItem = Instantiate(taskTemplate, taskTemplate.parent).GetComponent<TaskGroupingModeItem>();

                var taskRw = taskCfg.GetRowDatas(index);

                taskGroupingModeItem.Refresh(taskRw.ID, taskCfg);

                taskGroupingModeItem.OnToggleChangeValue += TaskGroupingModeItem_OnToggleChangeValue;

                if (SceneState.taskMode == DaTaskMode.GroupingMode)
                {
                    if (TaskModel == null)
                        taskGroupingModeItem.progressMark.SetActive(false);
                    else
                        taskGroupingModeItem.progressMark.SetActive(TaskModel.GetTaskIDs().Contains(taskRw.ID));
                }
            }

            taskTemplate.gameObject.SetActive(false);

            scrollContent.anchoredPosition = Vector2.zero;

            StartCoroutine(RebuildLayout());
        }

        private void TaskGroupingModeItem_OnToggleChangeValue(bool arg1, string arg2)
        {
            var state = arg1;

            var cacheId = arg2;

            if (state)
            {
                if (!ids.Contains(cacheId))
                    ids.Add(cacheId);
            }
            else
            {
                if (ids.Contains(cacheId))
                    ids.Remove(cacheId);
            }

            enterBtn.gameObject.SetActive(ids.Count > 0);
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
        }
    }

}