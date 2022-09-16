using Doozy.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fxb.CMSVR
{
    public class BatteryLiftDeviceStateChangeMessage : Message
    {
        public BatteryLiftDeviceState newState;

        public BatteryLiftDeviceAction byAction;

        public static void Send(BatteryLiftDeviceState newState, BatteryLiftDeviceAction byAction, ref BatteryLiftDeviceStateChangeMessage messageCache)
        {
            messageCache = messageCache ?? new BatteryLiftDeviceStateChangeMessage();

            messageCache.newState = newState;

            messageCache.byAction = byAction;

            Send(messageCache);
        }
    }

    //public class BatteryLiftDeviceActionMessage : Message
    //{
    //    public BatteryLiftDeviceAction action;

    //    public static void Send(BatteryLiftDeviceAction action, ref BatteryLiftDeviceActionMessage messageCache)
    //    {
    //        messageCache = messageCache ?? new BatteryLiftDeviceActionMessage();

    //        messageCache.action = action;

    //        Send(messageCache);
    //    }
    //}
}
