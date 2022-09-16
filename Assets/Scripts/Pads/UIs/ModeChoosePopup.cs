using Doozy.Engine;
using Doozy.Engine.UI;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class ModeChoosePopup : MonoBehaviour
    {
        public TextMeshProUGUI title;

        public Button trainingBtn;

        public Button examinationBtn;

        public Button cancelBtn;

        string curTaskID;

        // Start is called before the first frame update
        void Start()
        {
            trainingBtn.onClick.AddListener(() => OnTaskChose(DaTaskMode.Training));

            examinationBtn.onClick.AddListener(() => OnTaskChose(DaTaskMode.Examination));

            cancelBtn.onClick.AddListener(() => World.Get<DASceneState>().taskID2Init = null);
        }

        public void UpdateData(string taskID)
        {
            title.text = $"当前任务：{World.Get<TaskCsvConfig>().FindRowDatas(taskID).Title}";

            curTaskID = taskID;
        }

        void OnTaskChose(DaTaskMode daTaskState)
        {
            World.Get<DASceneState>().taskMode = daTaskState;

            //重选
            if (World.Get<ITaskModel>() != null)
            {
                World.Get<IRecordModel>().ClearRecord();

                World.Get<DASceneState>().taskID2Init = curTaskID;

                Message.Send(new ReloadDaSceneMessage());

                return;
            }

            //直接选

            World.current.Injecter.Regist<ITaskModel>(
     new TaskModel(new string[] { curTaskID })).Init();

            UIView.ShowView(DoozyNamesDB.BTN_CATEGORY_PAD, DoozyNamesDB.VIEW_PAD_TASKDETAIL);

            Message.Send(new PrepareTaskMessage());
        }
    }

}