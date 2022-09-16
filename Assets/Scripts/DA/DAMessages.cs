using Doozy.Engine;
using UnityEngine;
using VRTK;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 克隆物体支持多数量合并 单独提出抓取消息
    /// </summary>
    public class CloneObjPickupMessage : Message
    {
        public DACloneObjCtr target;
    }
     
    public class PartsTableDropErrorMessage : Message
    {
        public string tipInfo;   
    }

    /// <summary>
    /// 零件桌放置物体改变消息
    /// </summary>
    public class PartsTableDropObjChangeMessage : Message
    {
        public string propId;

        /// <summary>
        /// 为空表示被拿起
        /// </summary>
        public DACloneObjCtr objOnTable;

        public void Send(string inPropID, DACloneObjCtr inObjOnTable)
        {
            propId = inPropID;

            objOnTable = inObjOnTable;

            Send(this);
        }
    }

    /// <summary>
    /// 控制器抓取/放下物体消息
    /// </summary>
    public class ControllerGrabInteractObjMessage : Message
    {
        public void FromGrabEvts(object sender, ObjectInteractEventArgs e)
        {
            controllerRef = e.controllerReference;

            grab = sender as VRTK_InteractGrab;

            interactObj = e.target;
        }

        public VRTK_InteractGrab grab;

        public GameObject interactObj;

        public VRTK_ControllerReference controllerRef;

        /// <summary>
        /// true:抓取   false:放下
        /// </summary>
        public bool isGrab;
    }

    public class ReloadDaSceneMessage : Message { }

    /// <summary>
    /// 显示步骤信息
    /// </summary>
    public class ShowStepMessage : Message { }
    /// <summary>
    /// 显示记录消息
    /// </summary>
    public class ShowRecordMessage : Message { }
}


