using Framework;
using Fxb.DA;

namespace Fxb.CMSVR
{
    public class CarLiftCondition : StateMatchProcessCondition
    {
        public DAProcessTarget processTarget;

        public override float NumVal => World.Get<CmsCarState>().liftLocation;

        public override DAProcessTarget ProcessTarget => processTarget;

        public override string GetCompareFaildMsg(CompareType faildCompareType)
        {
            switch (compareType)
            {
                case CompareType.Equal:
                    if (numParam == 0)
                        return "降下车辆到地面进行操作";
                    else if (numParam == 1)
                        return "举升车辆到中位进行操作";
                    else if (numParam == 2)
                        return "举升车辆到最高处进行操作";
                    break;
                case CompareType.Less:
                    return "降下车辆进行操作";
                case CompareType.More:
                    return "举升车辆进行操作";
                case CompareType.Less_Equal:
                    return "降下车辆进行操作";
                case CompareType.More_Equal:
                    return "举升车辆进行操作";
            }

            return "";
        }
    }
}
