using Framework;
using Fxb.CPTTS;
using Fxb.SpawnPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class TaskRecordView : PadViewBase
    {
        public TextMeshProUGUI totalScore;

        public TextMeshProUGUI taskName;

        public Transform stepGroupItem;

        public RectTransform scrollContent;

        ITaskModel taskModel;

        string lastTask;

        List<TaskStepGroupItemView> groupItems;

        protected override void Start()
        {
            base.Start();

            SpawnPoolMgr.Inst.AddPool(stepGroupItem.name, stepGroupItem.gameObject);

            stepGroupItem.gameObject.SetActive(false);

            taskModel = World.Get<ITaskModel>();

            groupItems = new List<TaskStepGroupItemView>();

            Init();
        }

        protected override void OnStartShow()
        {
            base.OnStartShow();

            Init();
        }

        /// <summary>
        /// 目前默认只选择一个任务
        /// </summary>
        void Init()
        {
            if (taskModel == null)
                return;

            var curTask = taskModel.GetData()[0];

            if (lastTask == curTask.taskID)
                return;

            Clear();

            taskName.text = curTask.taskTitle;

            totalScore.text = taskModel.TotalScore.ToString();

            for (int i = 0; i < curTask.stepGroups.Count; i++)
            {
                InActiveSpawnObj spawnItem = (InActiveSpawnObj)SpawnPoolMgr.Inst.Spawn(
                    stepGroupItem.name, stepGroupItem.parent);

                var view = spawnItem.GetComponent<TaskStepGroupItemView>();

                view.Init(curTask.stepGroups[i], i);

                groupItems.Add(view);
            }

            lastTask = curTask.taskID;

            StartCoroutine(LayoutRefresh());  
        }

        IEnumerator LayoutRefresh()
        {
            yield return new WaitForSeconds(0.1f);

            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);

            yield return new WaitForSeconds(1f);

            LayoutRebuilder.MarkLayoutForRebuild(scrollContent);
        }

        void Clear()
        {
            foreach (var item in groupItems)
                item.Clear();

            groupItems.Clear();
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.A))
        //        LayoutRebuilder.MarkLayoutForRebuild(scrollContent);
        //}
    }

}