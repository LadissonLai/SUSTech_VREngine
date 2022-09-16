using Framework;
using Fxb.SpawnPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class TaskStepItemView : MonoBehaviour
    {
        TextMeshProUGUI title;

        IRecordModel recordModel;

        string curStepID;

        int curIndex;

        // Start is called before the first frame update
        void Start()
        {
            recordModel = World.Get<IRecordModel>();

            title = GetComponent<TextMeshProUGUI>();

            Refresh();
        }

        public void Init(string stepID, int index)
        {
            curStepID = stepID;

            curIndex = index;

            if (recordModel == null)
                return;

            Refresh();
        }

        void Refresh()
        {
            var item = recordModel.FindRecord(curStepID);

            title.text = $"{curIndex}.{item.Title}";
        }
    }

}