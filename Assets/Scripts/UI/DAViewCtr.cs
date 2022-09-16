using Doozy.Engine;
using Fxb.DA;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class DAViewCtr : MonoBehaviour
    {
        private void Awake()
        {
            Message.AddListener<DAToolErrorMessage>(OnWrenchError);

            Message.AddListener<DATipMessage>(OnTipShow);

            Message.AddListener<PartsTableDropErrorMessage>(OnPartsTableDropErrorMessage);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<DAToolErrorMessage>(OnWrenchError);

            Message.RemoveListener<DATipMessage>(OnTipShow);

            Message.RemoveListener<PartsTableDropErrorMessage>(OnPartsTableDropErrorMessage);
        }
         
        private void OnWrenchError(DAToolErrorMessage message)
        {
            Popup_Tips.Show(message.tipInfo);
        }

        void OnTipShow(DATipMessage msg)
        {
            if (string.IsNullOrEmpty(msg.tipInfo))
                return;

            Popup_Tips.Show(msg.tipInfo);
        }

        private void OnPartsTableDropErrorMessage(PartsTableDropErrorMessage msg)
        {
            Popup_Tips.Show(msg.tipInfo);
        }
    }
}