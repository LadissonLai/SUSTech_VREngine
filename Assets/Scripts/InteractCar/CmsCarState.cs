using System.Collections.Generic;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 车辆的各种状态 期望能够通用，允许有部分冗余数据来兼容电车与油车
    /// </summary>
    public class CmsCarState
    {
        public enum PowerState
        {
            Off,
            On,
            IG,
            Charge
        }

        public enum LockState
        {
            Undefined = 0,
            /// <summary>
            /// 前舱盖锁
            /// </summary>
            Bonnet
        }

        public enum SwitchState
        {
            //通用的一些开关
            Undefined = 0,

            /// <summary>
            /// 左前车门
            /// </summary>
            LFDoor,

            /// <summary>
            /// 右前车门
            /// </summary>
            RFDoor,

            /// <summary>
            /// 前舱盖
            /// </summary>
            Bonnet,

            /// <summary>
            /// 交流充电口盖
            /// </summary>
            ACChargeCover,

            /// <summary>
            /// 直流充电口盖
            /// </summary>
            DCChargeCover,
        }

        /// <summary>
        /// 举升位置 默认0 ，中位1，高位2
        /// </summary>
        public float liftLocation;
        
        /// <summary>
        /// 向上举升
        /// </summary>
        public bool liftUp;

        /// <summary>
        /// 启动状态
        /// </summary>
        public PowerState powerState;

        /// <summary>
        /// 已解锁的内容  默认全都锁定
        /// </summary>
        public HashSet<LockState> unLockerStates = new HashSet<LockState>();

        /// <summary>
        /// 开启的开关   默认全都关闭
        /// </summary>
        public HashSet<SwitchState> switchStates = new HashSet<SwitchState>();
    } 
}