using Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class GuideStepFactory
    {
        private Dictionary<Type, AbstractGuideStep> stepPool;

        public void ClearGuideStep(AbstractGuideStep guideStep)
        {
            guideStep.Clear();
        }

        public AbstractGuideStep CreateGuideStep(string recordID)
        {
            stepPool = stepPool ?? new Dictionary<Type, AbstractGuideStep>();

            var recordCsvConfig = World.Get<RecordCsvConfig>();

            var rd = recordCsvConfig.FindRowDatas(recordID);

            Debug.Assert(rd != null, $"记录数据为空 id:{recordID}");
             
            var stepType = rd.Type;

            var title = rd.Title;

            var paramStr = rd.Params;

            string[] paramArr = null;

            if (paramStr.IndexOf(',') > -1)
                paramArr = paramStr.Split(',');

            AbstractGuideStep guideStep = null;

            Type subStepType = null;

            switch (stepType)
            {
                case RecordStepType.Dismantle:
                case RecordStepType.Assemble:
                case RecordStepType.Fix:
                    subStepType = typeof(DAGuideStep);
                    break;
                case RecordStepType.LiftCar:
                    subStepType = typeof(LiftCarGuideStep);
                    break;
                case RecordStepType.Equip:
                    subStepType = typeof(EquipGuideStep);
                    break;
                case RecordStepType.BatteryLift:
                    subStepType = typeof(BatteryLiftGuideStep);
                    break;
                default:
                    Debug.LogError("RecordStepType 不合法:" + stepType);
                    return null;
            }
 
            if (stepPool.ContainsKey(subStepType))
            {
                guideStep = stepPool[subStepType];
            }
            else
            {
                guideStep = Activator.CreateInstance(subStepType) as AbstractGuideStep;

                stepPool.Add(subStepType, guideStep);
            }

            guideStep.Setup(title, stepType, paramArr == null ? paramStr : null, paramArr);

            return guideStep;
        }

        public void Destroy()
        {
            if (stepPool != null)
            {
                foreach (var item in stepPool)
                    item.Value.Destroy();
            }
        }
    }
}
