using Doozy.Engine;
using Doozy.Engine.UI;
using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VRTK;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    public class DAHandViewCtr : MonoBehaviour
    {
        public TextMeshProUGUI tipTxt;

        [Header("显示面板的参数")]

        public float maxShowDistance = 0.2f;

        [Tooltip("水平，垂直")]
        public Vector2 maxShowAngle = new Vector2(30, 30);

        Vector2 maxShowCosAngle;

        //bool taskPreparing = true;

        //float preShowTime = 5f;

        //float timer;

        const string preTips = "操作平板电脑，选择任务";

        // Start is called before the first frame update
        void Start()
        {
            Message.AddListener<GuideTipMessage>(OnRefreshTips);

            maxShowCosAngle = new Vector2(Mathf.Cos(maxShowAngle.x * Mathf.Deg2Rad), Mathf.Cos(maxShowAngle.y * Mathf.Deg2Rad));

            tipTxt.text = preTips;
        }

        private void OnDestroy()
        {
            Message.RemoveListener<GuideTipMessage>(OnRefreshTips);
        }

        void Update()
        {
            if (World.Get<DASceneState>().taskMode == DaTaskMode.Examination)
            {
                UIView.HideView(DoozyNamesDB.VIEW_GENERAL_HANDVIEW);

                return;
            }

            if (CheckViewState())
                UIView.ShowView(DoozyNamesDB.VIEW_GENERAL_HANDVIEW);
            else
                UIView.HideView(DoozyNamesDB.VIEW_GENERAL_HANDVIEW);
        }

        bool CheckViewState()
        {
            var viewCamera = VRTKHelper.HeadSetCamera;

            if (!viewCamera)
                return false;

            //任务开始前一直显示
            if (World.Get<DASceneState>().isTaskPreparing)
                return true;

            //预先出现一段时间
            //if (taskPreparing && !World.Get<DASceneState>().isTaskPreparing)
            //{
            //    timer += Time.deltaTime;

            //    if (timer > preShowTime)
            //    {
            //        taskPreparing = false;

            //        timer = 0;
            //    }

            //    return true;
            //}

            //小于水平角
            float result = Vector3.Dot(tipTxt.transform.right, viewCamera.transform.right);

            //Debug.Log($"result2right:{result}");

            if (result < maxShowCosAngle.x)
                return false;

            //小于垂直角
            result = Vector3.Dot(tipTxt.transform.up, viewCamera.transform.up);

            //Debug.Log($"result2Up: {result}");

            if (result < maxShowCosAngle.y)
                return false;

            //小于相机距离
            var dis = Vector3.Distance(viewCamera.transform.position, transform.position);

            if (dis > maxShowDistance)
                return false;

            return true;
        }

        void OnRefreshTips(GuideTipMessage msg)
        {
            tipTxt.text = msg.tip;
        }
    }


    public class GuideTipMessage : Message
    {
        public string tip;
    }
}