using Framework;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fxb.CMSVR
{
    public class BatteryLiftGuideStep : AbstractGuideStep
    {
        private BatteryLiftDeviceState CutBatteryLiftState => World.Get<DASceneState>().batteryLiftDeviceState;

        private BatteryLiftDeviceAction targetAction;

        private BatteryLiftDeviceCtr batteryLiftDevice;

        public override bool IsCompleted
        {
            get
            {
                switch (targetAction)
                {
                    case BatteryLiftDeviceAction.Place:
                        return CutBatteryLiftState.HasFlag(BatteryLiftDeviceState.AtLocation);
                    case BatteryLiftDeviceAction.Lift:
                        return CutBatteryLiftState.HasFlag(BatteryLiftDeviceState.Lifted);
                    case BatteryLiftDeviceAction.Dorp:
                        return !CutBatteryLiftState.HasFlag(BatteryLiftDeviceState.Lifted);
                    case BatteryLiftDeviceAction.Back:
                        return !CutBatteryLiftState.HasFlag(BatteryLiftDeviceState.AtLocation);
                }

                return false;
            }
        }

        protected override void UpdateTipObjs()
        {
            switch (targetAction)
            {
                case BatteryLiftDeviceAction.Place:
                    batteryLiftDevice.TipInteract(false, false ,true);
                    break;
                case BatteryLiftDeviceAction.Lift:
                    batteryLiftDevice.TipInteract(true);
                    break;
                case BatteryLiftDeviceAction.Dorp:
                    batteryLiftDevice.TipInteract(false, true);
                    break;
                case BatteryLiftDeviceAction.Back:
                    batteryLiftDevice.TipInteract(false, false, true);
                    break;
            }
        }

        public override void Setup(string tipInfo, RecordStepType type, string singleParam = null, string[] mutiPrams = null)
        {
            base.Setup(tipInfo, type, singleParam, mutiPrams);

            //只支持一个状态
            Debug.Assert(!string.IsNullOrEmpty(singleParam));

            int.TryParse(singleParam, out var result);

            targetAction = (BatteryLiftDeviceAction)result;

            Debug.Assert(targetAction != BatteryLiftDeviceAction.None);

            if (batteryLiftDevice == null)
                batteryLiftDevice = UnityEngine.Object.FindObjectOfType<BatteryLiftDeviceCtr>();

            Debug.Assert(batteryLiftDevice != null);
        }

        public override void Clear()
        {
            base.Clear();

            batteryLiftDevice.UntipInteracts();
        }
    }
}


