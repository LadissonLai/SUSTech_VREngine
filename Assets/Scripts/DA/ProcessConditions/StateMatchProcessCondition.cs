using System;
using Framework;
using Fxb.DA;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 匹配目标状态条件
    /// </summary>
    public abstract class StateMatchProcessCondition : MonoBehaviour, IDAProcessAssertAble
    {
        public enum CompareType
        {
            Equal,
            Less,
            More,
            Less_Equal,
            More_Equal
        }
         
        [Tooltip("待比较值")]
        public float numParam;

        /// <summary>
        /// 当前比较的值
        /// </summary>
        public abstract float NumVal { get; }

        [Tooltip("将目标值与numParam进行比较的方式")]
        public CompareType compareType;
 
        public abstract DAProcessTarget ProcessTarget { get; }

        public abstract string GetCompareFaildMsg(CompareType faildCompareType);

        public virtual bool Check(DAObjCtr daObjCtr, out string errorMsg)
        {
            errorMsg = null;

            var compareWith = NumVal;
             
            switch (compareType)
            {
                case CompareType.Less:
                    errorMsg = numParam > compareWith ? null : GetCompareFaildMsg(CompareType.Less);
                    break;
                case CompareType.Less_Equal:
                    errorMsg = numParam >= compareWith ? null : GetCompareFaildMsg(CompareType.Less_Equal);
                    break;
                case CompareType.More:
                    errorMsg = numParam < compareWith ? null : GetCompareFaildMsg(CompareType.More);
                    break;
                case CompareType.More_Equal:
                    errorMsg = numParam <= compareWith ? null : GetCompareFaildMsg(CompareType.More_Equal);
                    break;
                case CompareType.Equal:

                    if (Mathf.Abs(numParam - compareWith) > 0.001f)
                    {
                        errorMsg = GetCompareFaildMsg(CompareType.Equal);
                    }
                    break;
            }

            return errorMsg == null;
        }
    }
}
