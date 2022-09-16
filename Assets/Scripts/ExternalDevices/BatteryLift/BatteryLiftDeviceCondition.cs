using Framework;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class BatteryLiftDeviceCondition : ExternalDeviceCondition
    {
        public override float NumVal => (int)World.Get<DASceneState>().batteryLiftDeviceState;
 
        public override string GetCompareFaildMsg(CompareType faildCompareType)
        {
            Debug.Assert(faildCompareType == CompareType.Equal);

            switch (numParam)
            {
                case (int)(BatteryLiftDeviceState.AtLocation | BatteryLiftDeviceState.Lifted):
                    return "使用大电池举升装置后进行操作";
            }

            return null;
        }
    }
}
