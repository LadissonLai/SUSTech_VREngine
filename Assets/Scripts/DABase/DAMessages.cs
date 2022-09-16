using Doozy.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.DA
{
    /// <summary>
    /// 开始拆装模式消息
    /// </summary>
    public class StartDAModeMessage : Message
    {
        public IReadOnlyList<AbstractDAObjCtr> rootCtrs;

        public DAMode mode;
    }

    ///// <summary>
    ///// 拆装模式改变 通过 targetObjCtr 的 State 判断是否完成
    ///// </summary>
    //public class DAModeChangeMessage : Message
    //{
    //    //public AbstractDAObjCtr targetObjCtr;

    //    /// <summary>
    //    /// 上一个模式
    //    /// </summary>
    //    public DAMode preMode;

    //    /// <summary>
    //    /// 当前模式 为none表示退出模式
    //    /// </summary>
    //    public DAMode cutMode;
    //}

    /// <summary>
    /// 退出当前拆装步骤  
    /// </summary>
    public class CancelDAStepMessage : Message {}

    /// <summary>
    /// 安装/拆下/紧固等等处理完毕  
    /// TODO State逻辑已修改，通过 DAObjStateChangeMessage 能够达到此效果，待移除。
    /// </summary>
    public class DAObjProcessCompleteMessage : Message
    {
        public AbstractDAObjCtr objCtr;

        /// <summary>
        /// 目标状态
        /// </summary>
        public CmsObjState targetState;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool sucess;
    }

    /// <summary>
    /// 拆装物体的状态改变  
    /// 逻辑已更改，现在已无中间状态
    /// </summary>
    public class DAObjStateChangeMessage : Message
    {
        public AbstractDAObjCtr objCtr;

        public CmsObjState preState;
    }
     
    public class DAObjProcessingMessage : Message
    {
        public AbstractDAObjCtr target;
    }

    public class DAProcessObjMessage : Message
    {
        public AbstractDAObjCtr target;

        public IDAUsingTool usingTool;
    }

    public class DATipMessage : Message
    {
        public string tipInfo;

        public static void Send(string msgInfo, ref DATipMessage messageCache)
        {
            messageCache = messageCache ?? new DATipMessage();

            messageCache.tipInfo = msgInfo;

            messageCache.Send();
        }
    }
}

