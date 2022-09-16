using Doozy.Engine;

namespace Fxb.CMSVR
{
    public class CarLiftLocationChangedMessages : Message {}

    public class CarStateChangedMessage : Message
    {
        public enum StateType
        {
            PowerState,
            LockState,
            SwitchState
        }

        public StateType stateType;

        public int intNewState;
    }
}
