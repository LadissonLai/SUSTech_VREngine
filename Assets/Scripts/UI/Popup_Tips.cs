using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fxb.CMSVR
{
    public class Popup_Tips : MonoBehaviour
    {
        public static void Show(string txt, string popupName = null, bool addToQueue = false)
        {
            popupName = popupName ?? DoozyNamesDB.POPUP_NAME_TIPS;
 
            var tips = UIPopupManager.ShowPopup(popupName, addToQueue, false);

            var tipsPopup = tips.GetComponent<Popup_Tips>();

            tipsPopup.UpdateMsg(new Popup_Tips.Data
            {
                tipText = txt,
                tipsPos = new Vector3(0.0f, 0.0f, 0.0f)
            });
        }

        // Start is called before the first frame update
        [SerializeField]
        private UIPopup popup;

        public Transform tips_Tran;

        public Text text;

        private void Start()
        {
            //popup.Container.CanvasGroup.interactable = false;
        }

        public struct Data
        {
            public string tipText;

            public Vector3 tipsPos;
        }

        //设置初始默认位置在屏幕中心点的上方 new Vector3(0.0f, 100.0f, 0.0f)
        public void UpdateMsg(Data data)
        {
            text.text = data.tipText;
            tips_Tran.localPosition = data.tipsPos + new Vector3(0.0f, 100.0f, 0.0f);
            popup.Hide(1.0f);

        }

        public void UpdateMsg(string msg)
        {
            text.text = msg;
            popup.Hide(1.0f);
        }
    }
}
