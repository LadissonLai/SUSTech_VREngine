using System;
using UnityEngine;

namespace Fxb.DA
{
    [Serializable]
    public struct WrenchUseCondition
    {
        [Tooltip("扳手id")]
        public string banshouID;

        [Tooltip("紧固时的扭力扳手id")]
        public string fixBanshouID;

        [Tooltip("接杆id")]
        public string jieganID;

        [Tooltip("套筒id")]
        public string taotongID;

        [Tooltip("是否需要接杆 需要时才会检查jieganID")]
        public bool needJiegan;

        [Header("紧固时需要的扭力范围")]
        public int minFixTorsionRange;
        public int maxFixTorsionRange;
    }

    [Serializable]
    public struct WrenchInfo
    {
        /// <summary>
        /// 扳手
        /// </summary>
        public string banshou;

        /// <summary>
        /// 接杆
        /// </summary>
        public string jiegan;

        /// <summary>
        /// 套筒
        /// </summary>
        public string taotong;

        /// <summary>
        /// 棘轮扳手默认-1  扭力扳手默认0
        /// </summary>
        public int torsion;
         
    }


    /// <summary>
    /// 半厘米递增
    /// </summary>
    public enum ScrewOutLevel
    {
        NoMove = 0,
        HalfCM = 1,
        OneCM = 2,
        OneHalfCM = 3,
        TwoCM = 4
    }
}
