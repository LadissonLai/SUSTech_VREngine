using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class TaskGroupingModeItem : MonoBehaviour
    {
        public TextMeshProUGUI title;

        public Toggle toggle;

        public event Action<bool,string> OnToggleChangeValue;

        public GameObject progressMark;

       // [SerializeField]
       private string id;
    
        void Start()
        {
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool arg0)
        {
            var state = arg0;

            OnToggleChangeValue?.Invoke(state, id);
        }

        public void Refresh(string taskID,TaskCsvConfig taskCfg)
        {
            taskCfg = taskCfg ?? World.Get<TaskCsvConfig>();

            var data = taskCfg.FindRowDatas(taskID);

            title.text = data.Title;

            id = taskID;
        }

    }

}