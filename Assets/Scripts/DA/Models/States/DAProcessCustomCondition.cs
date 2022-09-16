using Framework;
using Fxb.DA;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 自定义的拆装条件
    /// </summary>
    [System.Serializable]
    public class DAProcessCustomCondition
    {
        public enum CheckType
        {
            None,
            liftLocation
        }

        public enum CompareType
        {
            Less,
            Equal,
            More,
            Less_Equal,
            More_Equal
        }

        public DAProcessTarget processTarget;

        public CheckType checkType;

        public float numParam;

        [Tooltip("将目标值与numParam进行比较的方式")]
        public CompareType compareType;

        public bool Check(DAObjCtr daObjCtr, out string errorMsg)
        {
            errorMsg = null;

            switch (checkType)
            {
                case CheckType.liftLocation:
                    return CarLiftDAChecker.Check(daObjCtr, this, out errorMsg);
                default:
                    break;
            }

            return true;
        }
    }


    public static class CarLiftDAChecker
    {
        public static bool Check(DAObjCtr daObjCtr, DAProcessCustomCondition condition, out string errorMsg)
        {
            errorMsg = null;

            var liftLocation = World.Get<CmsCarState>().liftLocation;

            switch (condition.compareType)
            {
                case DAProcessCustomCondition.CompareType.Less:
                    errorMsg = condition.numParam > liftLocation ? null : "降下车辆进行操作";
                    break;
                case DAProcessCustomCondition.CompareType.Less_Equal:
                    errorMsg = condition.numParam >= liftLocation ? null : "降下车辆进行操作";
                    break;
                case DAProcessCustomCondition.CompareType.More:
                    errorMsg = condition.numParam < liftLocation ? null : "举升车辆进行操作";
                    break;
                case DAProcessCustomCondition.CompareType.More_Equal:
                    errorMsg = condition.numParam <= liftLocation ? null : "举升车辆进行操作";
                    break;
                case DAProcessCustomCondition.CompareType.Equal:
                    Debug.Assert(condition.numParam == 0 || condition.numParam == 1 || condition.numParam == 2);

                    if (Mathf.Abs(condition.numParam - liftLocation) > 0.001f)
                    {
                        if (condition.numParam == 0)
                            errorMsg = "降下车辆到地面进行操作";
                        else if (condition.numParam == 1)
                            errorMsg = "举升车辆到中位进行操作";
                        else if (condition.numParam == 2)
                            errorMsg = "举升车辆到最高处进行操作";
                        else
                            Debug.LogError("不支持0、1、2以外的举升位置相等判断");
                    }
                    break;
            }

            return errorMsg == null;
        }
    }
}

