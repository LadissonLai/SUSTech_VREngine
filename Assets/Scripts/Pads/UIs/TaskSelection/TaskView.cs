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

namespace Fxb.CMSVR
{
    public class TaskView : PadViewBase
    {
        public Transform taskTemplate;

        public TextMeshProUGUI title;

        public RectTransform scrollContent;

        public SpriteAtlas spriteAtlas;

        List<TaskItemView> taskItems = new List<TaskItemView>();


        TaskCsvConfig taskCfg;


        List<TaskCategory> taskCategories;

        string curTaskID;

        protected override void OnStartShow()
        {
            base.OnStartShow();
        }

        protected override void Start()
        {
            base.Start();

            taskCfg = World.Get<TaskCsvConfig>();

            GetTaskCategory();

            InitTaskItem();
        }

        void InitTaskItem()
        {
            foreach (var item in taskCategories)
            {
                Refresh(item.sysName, item.taskIDs, spriteAtlas);       
            }
        }

        void GetTaskCategory()
        {
            taskCategories = new List<TaskCategory>();

            var datas = taskCfg.DataArray;

            foreach (var item in datas)
            {
                var sysName = item.System;

                var category = taskCategories.Find((task) => task.sysName == sysName);

                if (string.IsNullOrEmpty(category.sysName))
                {
                    category.sysName = sysName;

                    category.taskIDs = new List<string>();

                    taskCategories.Add(category);
                }

                category.taskIDs.Add(item.ID);
            }
        }



        public void Refresh(string sysName, List<string> taskIDs, SpriteAtlas spriteAtlas)
        {
            title.text = sysName;

            taskTemplate.gameObject.SetActive(true);

            int index;

            for (index = 0; index < taskIDs.Count; index++)
            {
                if (taskItems.Count > index)
                    taskItems[index].Refresh(taskIDs[index]);
                else
                {
                    var task = Instantiate(taskTemplate, taskTemplate.parent).GetComponent<TaskItemView>();

                    task.Refresh(taskIDs[index], spriteAtlas);

                    task.OnTitleBtnClicked += OnTaskItemViewClicked;

                    taskItems.Add(task);
                }
            }

            while (taskItems.Count > index)
            {
                Destroy(taskItems[index].gameObject);

                taskItems.RemoveAt(index);
            }

            taskTemplate.gameObject.SetActive(false);

            scrollContent.anchoredPosition = Vector2.zero;

            StartCoroutine(RebuildLayout());
        }

        IEnumerator RebuildLayout()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
        }

        void OnTaskItemViewClicked(string taskID)
        {
            curTaskID = taskID;

            if (TryRechooseTask())
                return;

            TryChooseTaskMode();
        }

        void TryChooseTaskMode()
        {
            var alert = UIPopupManager.ShowPopup(DoozyNamesDB.POPUP_NAME_MODECHOOSE, false, false).
                GetComponent<ModeChoosePopup>();

            alert.UpdateData(curTaskID);
        }

        bool TryRechooseTask()
        {
            var taskModel = World.Get<ITaskModel>();

            if (taskModel == null || taskModel.IsSubmitAllTask)
                return false;

            var alert = UIPopupManager.ShowPopup(DoozyNamesDB.POPUP_NAME_YESORNO, false, false)
.GetComponent<YesOrNoPopup>();

            alert.UpdateMsg(new YesOrNoPopup.Data()
            {
                title = null,
                msg = $"Select the new one will interrupt the task，continue?",
                enterBtnText = "Yes",
                cancelBtnText = "No"
            });

            alert.OnEntrerBtnClick += param => TryChooseTaskMode();

            return true;
        }
    }

}