using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// layer强类型  没有考虑直接赋值int值，防止layer setting中修改内容后没有同步
    /// </summary>
    public static class LayerConst
    {
        public readonly static int Default = LayerMask.NameToLayer(nameof(Default));

        public readonly static int UI = LayerMask.NameToLayer(nameof(UI));

        public readonly static int Floor = LayerMask.NameToLayer(nameof(Floor));

        public readonly static int VRHand = LayerMask.NameToLayer(nameof(VRHand));

        /// <summary>
        /// 手持工具使用的物体
        /// </summary>
        public readonly static int IgnoreHandTouch = LayerMask.NameToLayer(nameof(IgnoreHandTouch));
    }

    public static class TagConst
    {
        public readonly static string WrenchParts = "WrenchParts";

        public readonly static string WrenchPartsPlaceHolder = "WrenchPartsPlaceHolder";
    }
}

