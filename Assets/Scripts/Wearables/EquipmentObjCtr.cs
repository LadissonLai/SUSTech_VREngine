using Doozy.Engine;
using Framework;
using Fxb.CMSVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    [DisallowMultipleComponent]
    public class EquipmentObjCtr : MonoBehaviour,IWearable
    {
        public EquipName equipName;

        public EquipName EquipName => equipName;

        public AdvancedInteractableObj interactObj;

        private void Awake()
        {
            World.current.Injecter.Regist(this,EquipName.ToString());
        }

        private void OnDestroy()
        {
            World.current.Injecter.UnRegist<EquipmentObjCtr>(EquipName.ToString());
        }

        public void Wear(bool isOn)
        {
            gameObject.SetActive(false);

            //先关闭再调用ForceStopInteracting，否则会延迟一帧处理stop interacting相关逻辑
            interactObj.ForceStopInteracting();

            Destroy(gameObject);

            Message.Send(new WearEquipmentMessage() { equipName = EquipName, isOn = isOn });
        }
    }
}
