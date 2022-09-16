using System;

namespace Fxb.DA
{
    public static partial class DAConfig
    {
        public static string forceInteractLayer = "RaycastOnly";

        public static Type modelGroupAssembleStepType = null;

        public static Type modelGroupDisassembleStepType = null;

        public static Type PartsAssembleStepType = null;

        public static Type PartsDisassembleStepType = null;

        #region 测试
        /// <summary>
        /// 是否忽略扳手条件检查 忽略时可以使用任意扳手进行操作。用作测试
        /// </summary>
        public static bool ignoreWrenchConditionCheck = true;

        public static bool skipToolAnimation = false;

        #endregion

    }
}
