using Framework;
using Fxb.SpawnPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class TaskStepGroupItemView : MonoBehaviour
    {
        [Tooltip("头部圆圈标志的遮罩")]
        public Image headMarkMask;

        public TextMeshProUGUI title;

        public TextMeshProUGUI score;

        public Transform stepItem;

        public Transform errorItem;

        ITaskModel taskModel;

        IRecordModel recordModel;

        List<TaskStepItemView> stepItems;

        List<TaskErrorItemView> errorItems;

        StepGroupData curData;

        int curIndex;

        // Start is called before the first frame update
        void Start()
        {
            SpawnPoolMgr.Inst.AddPool(stepItem.name, stepItem.gameObject);

            SpawnPoolMgr.Inst.AddPool(errorItem.name, errorItem.gameObject);

            stepItem.gameObject.SetActive(false);

            errorItem.gameObject.SetActive(false);

            taskModel = World.Get<ITaskModel>();

            recordModel = World.Get<IRecordModel>();

            stepItems = new List<TaskStepItemView>();

            errorItems = new List<TaskErrorItemView>();

            Refresh();
        }

        public void Init(StepGroupData stepGroup, int index)
        {
            curData = stepGroup;

            curIndex = index;

            if (taskModel == null)
                return;

            Refresh();
        }

        void Refresh()
        {
            if (curData == null)
                return;

            title.text = taskModel.GetStepGroupDescription(curData.id);

            score.text = $"({curData.score}分)";

            if (curIndex == 0)
                headMarkMask.enabled = true;
            else
                headMarkMask.enabled = false;

            var steps = taskModel.GetChildStepIDs(curData.id);

            InitSteps(steps);

            InitErrors(steps);
        }

        void InitSteps(string[] steps)
        {
            for (int i = 0; i < steps.Length; i++)
            {
                InActiveSpawnObj spawnItem = (InActiveSpawnObj)SpawnPoolMgr.Inst.Spawn(
                    stepItem.name, stepItem.parent);

                var view = spawnItem.GetComponent<TaskStepItemView>();

                view.Init(steps[i], i + 1);

                stepItems.Add(view);
            }
        }

        void InitErrors(string[] steps)
        {
            //临时计算每个记录的分数
            float stepScore = (float)Math.Round(curData.score / steps.Length, 1);

            for (int i = 0; i < steps.Length; i++)
            {
                if (recordModel.GetRecordErrorScoreDeducting(steps[i]) == 0 &&
                    recordModel.CheckRecordCompleted(steps[i]))
                    continue;

                InActiveSpawnObj spawnItem = (InActiveSpawnObj)SpawnPoolMgr.Inst.Spawn(
                    errorItem.name, errorItem.parent);

                var view = spawnItem.GetComponent<TaskErrorItemView>();

                view.Init(steps[i], i + 1, stepScore);

                errorItems.Add(view);
            }
        }

        public void Clear()
        {
            foreach (var item in stepItems)
                SpawnPoolMgr.Inst.Despawn(item.transform);

            foreach (var item in errorItems)
                SpawnPoolMgr.Inst.Despawn(item.transform);

            stepItems.Clear();

            errorItems.Clear();

            SpawnPoolMgr.Inst.Despawn(transform);
        }
    }

}