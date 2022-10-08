using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using Fxb.CPTTS;
using TMPro;
using UnityEngine.UI;
using Doozy.Engine;
using Fxb.DA;
using Doozy.Engine.UI;

namespace Fxb.CMSVR
{
    public class TaskDetailView : PadViewBase
    {
        public Transform taskTemplate;

        public TextMeshProUGUI title;

        public TextMeshProUGUI description;

        public string descriptionPrex = "<b>Task Description</b>：";

        public TextMeshProUGUI completedAmountTxt;

        public Button submitBtn;

        //public Button stepBtn;

        ITaskModel taskModel;

        string lastTask;

        List<TaskDetailItemView> detailItems = new List<TaskDetailItemView>();

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            Message.AddListener<RefreshRecordItemStateMessage>(OnRefreshRecordItemState);

            submitBtn.onClick.AddListener(SubmitTask);
            //stepBtn.onClick.AddListener(ShowStep);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<RefreshRecordItemStateMessage>(OnRefreshRecordItemState);
        }

        /// <summary>
        /// 实时刷新UI状态
        /// </summary>
        /// <param name="msg"></param>
        private void OnRefreshRecordItemState(RefreshRecordItemStateMessage msg)
        {
            foreach (var item in detailItems)
                item.RefreshStep();

            UpdateCompletedAmount();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();

            taskModel = taskModel ?? World.Get<ITaskModel>();

            Init();
        }

        /// <summary>
        /// 目前只显示第一个任务的详情
        /// </summary>
        void Init()
        {
            //每次重置列表位置
            (description.transform.parent.parent as RectTransform).anchoredPosition = Vector2.zero;

            var data = taskModel.GetData()[0];

            if (data.taskID == lastTask)
            {
                foreach (var item in detailItems)
                    item.RefreshStep();

                UpdateCompletedAmount();

                if(taskModel.IsSubmitAllTask)
                    submitBtn.gameObject.SetActive(false);

                return;
            }

            title.text = data.taskTitle;

            description.text = $"{descriptionPrex}{data.taskDescription}";

            taskTemplate.gameObject.SetActive(true);

            int index;

            for (index = 0; index < data.stepGroups.Count; index++)
            {
                if (detailItems.Count <= index)
                    detailItems.Add(Instantiate(taskTemplate, taskTemplate.parent).
                        GetComponent<TaskDetailItemView>());

                detailItems[index].InitStep(index + 1, data.stepGroups[index].id);
            }

            while (detailItems.Count > index)
            {
                Destroy(detailItems[index].gameObject);

                detailItems.RemoveAt(index);
            }

            taskTemplate.gameObject.SetActive(false);

            submitBtn.gameObject.SetActive(true);

            UpdateCompletedAmount(data.stepGroups.Count);

            lastTask = data.taskID;
        }

        /// <summary>
        /// 默认初始化时存在已经完成的步骤
        /// </summary>
        /// <param name="newStepGroupsAmnt"></param>
        void UpdateCompletedAmount(int newStepGroupsAmnt = 0)
        {
            if (newStepGroupsAmnt != 0)
                completedAmountTxt.text = $"{0}/{newStepGroupsAmnt}";

            var data = taskModel.GetData()[0];

            int amount = 0;

            foreach (var item in data.stepGroups)
            {
                if (taskModel.CheckStepGroupCompleted(item.id))
                    amount += 1;
            }

            completedAmountTxt.text = $"{amount}/{data.stepGroups.Count}";
        }

        void SubmitTask()
        {
            string hint = taskModel.CheckAllTaskCompleted() ? "Are you sure to submit?" : "There is a task that has not been completed, submit it?";

            var alert = UIPopupManager.ShowPopup(DoozyNamesDB.POPUP_NAME_YESORNO, false, false)
                .GetComponent<YesOrNoPopup>();

            alert.UpdateMsg(new YesOrNoPopup.Data()
            {
                title = null,
                msg = hint,
                enterBtnText = "submit",
                cancelBtnText = "think again"
            });

            alert.OnEntrerBtnClick += DoSubmitTask;
        }

        void DoSubmitTask(int param)
        {
            taskModel.SubmitTask();

            submitBtn.gameObject.SetActive(false);

            UIView.ShowView(DoozyNamesDB.VIEW_CATEGORY_PAD, DoozyNamesDB.VIEW_PAD_RECORD); 
        }
        void ShowStep()
        {
            Message.Send(new ShowStepMessage());
        }
    }
}