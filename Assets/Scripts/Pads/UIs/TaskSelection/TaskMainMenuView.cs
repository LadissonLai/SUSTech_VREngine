using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fxb.CPTTS;
using UnityEngine.UI;
using UnityEngine.U2D;
using Doozy.Engine.UI;
using Framework;

namespace Fxb.CMSVR
{
    public class TaskMainMenuView : PadViewBase
    {
        public List<TaskData> taskIcons;

        public Transform taskTemplate;

        public SpriteAtlas spriteAtlas;

        public UIView childView;

        List<TaskCategory> taskCategories;

        TaskCsvConfig taskCfg;

        protected override void Awake()
        {
            base.Awake();

            //TestRegistInfo();
        }

        protected override void Start()
        {
            base.Start();

            taskCfg = World.Get<TaskCsvConfig>();

            GetTaskCategory();

            InitTaskItem();
        }

        //测试
        void TestRegistInfo()
        {
            World.current.Injecter.Regist<DASceneState>();

            SpawnPool.SpawnPoolMgr.Inst.Init();

            World.current.Injecter.Regist(CsvConfig.CsvSerializer.Serialize<TaskCsvConfig, TaskCsvConfig.Item>
                (Resources.Load<TextAsset>(PathConfig.CONFIG_TASK).text, 1));

            World.current.Injecter.Regist(CsvConfig.CsvSerializer.Serialize<TaskStepGroupCsvConfig, TaskStepGroupCsvConfig.Item>
    (Resources.Load<TextAsset>(PathConfig.CONFIG_TASKSTEP).text, 1));

            World.current.Injecter.Regist(CsvConfig.CsvSerializer.Serialize<RecordCsvConfig, RecordCsvConfig.Item>
(Resources.Load<TextAsset>(PathConfig.CONFIG_RECORD).text, 1));

            World.current.Injecter.Regist<IRecordModel>(new RecordModel());

            World.current.Injecter.Regist(CsvConfig.CsvSerializer.Serialize<SoftwareCsvConfig, SoftwareCsvConfig.Item>
(Resources.Load<TextAsset>(PathConfig.CONFIG_SOFTWARE).text, 1));
        }

        void InitTaskItem()
        {
            foreach (var item in taskCategories)
            {
                var mainTask = Instantiate(taskTemplate, taskTemplate.parent).GetComponent<Image>();

                var t = taskIcons.Find((data) => data.sysName == item.sysName);

                mainTask.sprite = spriteAtlas.GetSprite(taskIcons.Find(
                    (data) => data.sysName == item.sysName).iconPath);

                mainTask.GetComponent<Button>().onClick.AddListener(() =>
                {
                    childView.Show();

                    childView.GetComponent<TaskView>().Refresh(item.sysName, item.taskIDs, spriteAtlas);
                });
            }

            taskTemplate.gameObject.SetActive(false);
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
    }


    [System.Serializable]
    public struct TaskData
    {
        public string sysName;

        public string iconPath;
    }


    public struct TaskCategory
    {
        public string sysName;

        public List<string> taskIDs;
    }
}