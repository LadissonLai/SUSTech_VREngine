using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.DA
{
    public enum DAMode
    {
        None,
        Disassembly,
        Assembly,

        /// <summary>
        /// 安装和拆卸同时进行
        /// </summary>
        DisassemblyAssembly
    }

    [Flags]
    public enum DAProcessTarget
    {
        None = 0,
        Dismantle = 1,
        Assemble = 2,
        Fix = 4,
        Place = 8
    }
    
    [Flags]
    public enum CmsObjState : byte
    {
        None = 0,

        /// <summary>
        /// 完好
        /// </summary>
        Default = 1,

        /// <summary>
        /// 被拆下
        /// </summary>
        Dismantled = 2,
 
        /// <summary>
        /// 安装上，未固定。（有卡扣螺丝等未安装）
        /// </summary>
        Assembled = 4,

        /// <summary>
        /// 固定上但不完整
        /// parts与snapfit 固定上之后就自动切换到default状态。 
        /// modelgroup在所有部件安装完毕后才会切换到default状态。
        /// </summary>
        Fixed = 8,

        WaitForPickup = 16,

        /// <summary>
        /// 新增状态 当一个物体被放置后处于此状态，允许继续安装
        /// </summary>
        Placed = 32,

        Dismantable = Default | Fixed | Assembled | WaitForPickup,

        Installable = Dismantled | Placed,

        All = 255
    }

    [Flags]
    public enum CmsDisplayMode : byte
    {
        Default = 1,

        Hide = 2,

        PlaceHolder = 4
    }

    [Flags]
    public enum CmsObjType : byte
    {
        ModelGroup = 1,

        /// <summary>
        /// 部件
        /// </summary>
        Parts = 2,

        /// <summary>
        /// 螺丝,或者卡扣之类的 
        /// </summary>
        SnapFit = 4,
    }

    public enum DAStepMode
    {
        /// <summary>
        /// 部件拆除步骤
        /// </summary>
        PartsDisassemble,

        /// <summary>
        /// 模块拆除步骤
        /// </summary>
        ModelGroupDisassemble,

        /// <summary>
        /// 部件安装步骤
        /// </summary>
        PartsAssemble,

        /// <summary>
        /// 模块安装步骤
        /// </summary>
        ModelGroupAssemble
    }

    public partial class DAState
    {
        public bool isRunning;

        /// <summary>
        /// 正在处理中的物体  
        /// </summary>
        public HashSet<AbstractDAObjCtr> processingObjs = new HashSet<AbstractDAObjCtr>();

        public List<AbstractDAObjCtr> validProcessParts;

        public List<AbstractDAObjCtr> validProcessSnapFits;

        /// <summary>
        /// 在本次引导中相关联的拆装物体。 包括当前引导步骤中完成与未完成的物体，未完成的物体会高亮提示。
        /// </summary>
        public List<AbstractDAObjCtr> tipsInGuiding;

        /// <summary>
        /// 指引中允许的拆装内容
        /// </summary>
        public DAProcessTarget guidingProcessTarget;
    }
}
