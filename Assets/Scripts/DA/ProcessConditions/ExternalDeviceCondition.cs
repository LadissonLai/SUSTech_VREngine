using Fxb.DA;

namespace Fxb.CMSVR
{
    public abstract class ExternalDeviceCondition : StateMatchProcessCondition
    {
        public override DAProcessTarget ProcessTarget => DAProcessTarget.Assemble | DAProcessTarget.Dismantle | DAProcessTarget.Fix;

    }
}
