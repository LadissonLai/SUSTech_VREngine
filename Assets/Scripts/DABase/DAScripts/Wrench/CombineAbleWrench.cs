using UnityEngine;
using Framework;
using VRTK.GrabAttachMechanics;

namespace Fxb.DA
{
    public abstract class CombineAbleWrench : MonoBehaviour
    {
        protected Transform screwConnect;

        protected string toolID;

        [SerializeField]
        protected WrenchInfo wrenchInfo;

        /// <summary>
        /// 位移根节点
        /// </summary>
        public Transform translateRoot;

        /// <summary>
        /// 手柄根节点
        /// </summary>
        public Transform handleRoot;

        /// <summary>
        /// 套筒根节点，存放接杆与套筒
        /// </summary>
        public Transform sleeveRoot;

        /// <summary>
        /// 手柄
        /// </summary>
        public Transform handle;

        /// <summary>
        /// 接杆
        /// </summary>
        public Transform extension;

        /// <summary>
        /// 套筒
        /// </summary>
        public Transform sleeve;

        /// <summary>
        /// 螺丝的接口  从套筒身上找
        /// </summary>
        public Transform ScrewConnect
        {
            get
            {
                if(screwConnect == null && sleeve != null)
                {
                    screwConnect = sleeve.Find("Connect");
                }

                return screwConnect;
            }
        }

        public virtual WrenchInfo WrenchInfo => wrenchInfo;

        public virtual string ToolID
        {
            get
            {
                if (string.IsNullOrEmpty(toolID))
                    toolID = $"{wrenchInfo.banshou}_{wrenchInfo.jiegan}_{wrenchInfo.taotong}";

                return toolID;
            }
        }

        public abstract Transform GenWrenchParts(string partID);

        protected virtual void Start()
        {

        }

        public virtual void CombineWrench(WrenchInfo info)
        {
            wrenchInfo = info;

            handle = GenWrenchParts(info.banshou);

            if(!string.IsNullOrEmpty(info.jiegan))
                extension = GenWrenchParts(info.jiegan);

            sleeve = GenWrenchParts(info.taotong);

            CombineWrench(handle, extension, sleeve);
        }

        protected virtual void CombineWrench(Transform handle, Transform extension, Transform sleeve)
        {
            handle.SetParent(handleRoot);

            var grabAttach = GetComponent<VRTK_BaseGrabAttach>();
 
            grabAttach.leftSnapHandle = handle.Find("LeftGrabPos");

            grabAttach.rightSnapHandle = handle.Find("RightGrabPos");

            if (extension != null)
            {
                extension.SetParent(sleeveRoot);

                sleeve.SetParent(extension.Find("Connect"));
            }
            else
            {
                sleeve.SetParent(sleeveRoot);
            }

            handle.ResetLocalMatrix();

            extension.ResetLocalMatrix();

            sleeve.ResetLocalMatrix();
        }
    }
}
