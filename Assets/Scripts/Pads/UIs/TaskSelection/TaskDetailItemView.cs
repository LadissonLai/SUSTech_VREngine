using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class TaskDetailItemView : MonoBehaviour
    {
        public Image detailMark;

        public TextMeshProUGUI info;

        public Color completedColor;

        ITaskModel taskModel;

        string curStepGroupID;

        Color defaultColor;

        // Start is called before the first frame update
        void Awake()
        {
            defaultColor = info.color;
        }

        public void InitStep(int index, string stepGroupID)
        {
            taskModel = taskModel ?? World.Get<ITaskModel>();

            info.text = $"{index}.{taskModel.GetStepGroupDescription(stepGroupID)}";

            info.color = defaultColor;

            detailMark.enabled = false;

            curStepGroupID = stepGroupID;

            RefreshStep();
        }

        public void RefreshStep()
        {
            if (taskModel == null)
                return;

            if (detailMark.enabled)
                return;

            if (taskModel.CheckStepGroupCompleted(curStepGroupID))
            {
                info.color = completedColor;

                detailMark.enabled = true;
            }
        }
    }

}