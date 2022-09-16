using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class EquipGuideStep : AbstractGuideStep
    {
        public override bool IsCompleted => equipments == null || equipments.Count == 0;

        public List<EquipmentObjCtr> equipments;

        public override void Setup(string tipInfo, RecordStepType type, string singleParam = null, string[] mutiPrams = null)
        {
            base.Setup(tipInfo, type, singleParam, mutiPrams);

            equipments = new List<EquipmentObjCtr>();

            foreach (var param in mutiPrams)
            {
                //EquipName
                if(Enum.TryParse<EquipName>(param, out var res))
                {
                    equipments.Add(World.Get<EquipmentObjCtr>(res.ToString()));
                }
                else
                {
                    Debug.LogError("装备类型错误:" + param);
                }
            }
        }

        protected override void UpdateTipObjs()
        {
            if (equipments == null || equipments.Count == 0)
                return;

            if(equipments[0] == null)
            {
                equipments.RemoveAt(0);
            }
            else
            {
                equipments[0].interactObj.TipInteractableObj();
            }
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}
