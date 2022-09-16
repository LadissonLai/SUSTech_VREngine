using Framework;
using VRTKExtensions;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 举升机引导
    /// </summary>
    public class LiftCarGuideStep : AbstractGuideStep
    {
        private AdvancedInteractableObj liftUpButtonObj;

        private AdvancedInteractableObj liftDownButtonObj;

        private float targetLiftLocation;

        public override void Setup(string tipInfo, RecordStepType type, string singleParam = null, string[] mutiPrams = null)
        {
            base.Setup(tipInfo, type, singleParam, mutiPrams);

            DebugEx.AssertIsTrue(singleParam != null && mutiPrams == null);

            targetLiftLocation = float.Parse(singleParam);

            //举升机记录参数下降时为小数
            if (!int.TryParse(singleParam.ToString(), out int result))
                targetLiftLocation -= 0.5f;

            var carLiftCtr = UnityEngine.Object.FindObjectOfType<CarLiftCtr>();

            DebugEx.AssertNotNull(carLiftCtr, "未找到举升机脚本CarLiftCtr");

            liftUpButtonObj = carLiftCtr.up_InteractableObj;

            liftDownButtonObj = carLiftCtr.down_InteractableObj;
        }

        public override bool IsCompleted
        {
            get
            {
                return targetLiftLocation == World.Get<CmsCarState>().liftLocation;
            }
        }

        public override void Clear()
        {
            liftUpButtonObj.UnTipInteractableObj();
            liftDownButtonObj.UnTipInteractableObj();

            base.Clear();
        }

        protected override void UpdateTipObjs()
        {
            var curLiftLocation = World.Get<CmsCarState>().liftLocation;

            if (curLiftLocation < targetLiftLocation)
            {
                liftUpButtonObj.TipInteractableObj();
                liftDownButtonObj.UnTipInteractableObj();
            }
            else if (curLiftLocation > targetLiftLocation)
            {
                liftDownButtonObj.TipInteractableObj();
                liftUpButtonObj.UnTipInteractableObj();
            }
            else
            {
                liftDownButtonObj.UnTipInteractableObj();
                liftUpButtonObj.UnTipInteractableObj();
            }
        }
    }
}
