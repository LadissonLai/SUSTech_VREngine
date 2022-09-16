using System;

namespace Fxb.CMSVR
{
    public enum WorkMode
    {
        /// <summary>
        /// 未指定
        /// </summary>
        None,

        /// <summary>
        /// 训练模式
        /// </summary>
        Training,

        /// <summary>
        /// 考核模式
        /// </summary>
        Examination
    }

    public enum RecordStepType
    {
        None = 0,

        /// <summary>
        /// 拆下物体
        /// </summary>
        Dismantle = 1,

        /// <summary>
        /// 安装物体   (更换逻辑目前可以简单的认为是安装了一个未损坏的物品)
        /// </summary>
        Assemble = 2,

        /// <summary>
        /// 紧固物体
        /// </summary>
        Fix = 3,
         
        /// <summary>
        /// 举升车辆。 参数为举升的位置，目前举升到高处是2，低处是0,中间是1，2降到1是1.5
        /// </summary>
        LiftCar = 5,

        /// <summary>
        ///穿戴防护装备
        /// </summary>
        Equip = 6,

        /// <summary>
        /// 操作电池举升装置，参数为 BatteryLiftDeviceAction
        /// </summary>
        BatteryLift = 7,
    }

    public enum ErrorRecordType
    {
        None = 0,

        InvalidTools,

        InvalidSuit,

        InvalidHat,

        InvalidGlasses,

        InvalidShoes,

        InvalidCottonGloves,

        InvalidRubberGloves
    }

    //考虑改为EquipType/EquipCategory
    [Flags]
    public enum EquipName
    {
        None = 0,
        Suit = 1,
        Hat = 2,
        Glasses = 4,
        Shoes = 8,
        /// <summary>
        /// 棉耐磨手套
        /// </summary>
        CottonGloves = 16,
        /// <summary>
        /// 橡胶手套
        /// </summary>
        RubberGloves = 32
    }

    public enum EquipRegion
    {
        None = 0,
        Body,
        Head,
        Face,
        Foot,
        Hand,
    }

    public enum DaTaskMode
    {
        None,
        Training,
        Examination,
        GroupingMode
    }
    //public enum EquipStringType
    //{
    //    /// <summary>
    //    /// 检查方法
    //    /// </summary>
    //    CheckMethod,
    //    /// <summary>
    //    /// 作用介绍
    //    /// </summary>
    //    Introduction,
    //    /// <summary>
    //    /// 预制体名称
    //    /// </summary>
    //    PrefabName,
    //    ReadableName
    //}

    ///// <summary>
    ///// 装备无效的事件规模
    ///// </summary>
    //public enum EquipInvalidEventSize
    //{
    //    Max,
    //    Medium,
    //    Min
    //}
}