using Framework;
using Fxb.DA;
using System;
using System.Collections;
using UnityEngine;

namespace Fxb.CMSVR
{
    public class VRDAScrew : VRDAClaspCtr
    {
        public event Action<VRDAScrew, IDAUsingTool> OnScrewWillPlaced;

        private void Reset()
        {
            //螺丝一般都会使用工具进行操作，默认layer修改，忽略与手部的碰撞
            gameObject.ApplyLayer(LayerConst.IgnoreHandTouch);

            autoDisappear = true;
        }
         
        public override CmsObjState State
        {
            get => base.State;

            protected set => base.State = value;
        }

        public override void DoAssemble(IDAUsingTool usingTool = null)
        {
            if (autoDisappear && DisplayMode != CmsDisplayMode.Default)
            {
                if (!CheckProcessCondition(DAProcessTarget.Place))
                {
                    return;
                }

                var daCloneObj = usingTool as DACloneObjCtr;

                if (daCloneObj == null || daCloneObj.PropID != PropID)
                    return;

                if (OnScrewWillPlaced != null)
                    OnScrewWillPlaced(this, usingTool);
                else
                    World.current.StartCoroutine(PlaceScrewBeforeAssemble(usingTool));

                return;
            }

            base.DoAssemble(usingTool);
        }
 
        /// <summary>
        /// 先放置螺丝再进行安装
        /// </summary>
        /// <param name="targetPackProID">选择一个螺丝需要把同一批次的其它螺丝也显示出来，外部需要指定传入的pack pro id</param>
        /// <returns></returns>
        public IEnumerator PlaceScrewBeforeAssemble(IDAUsingTool usingTool)
        {
            //Debug.Log("PlaceScrewBeforeAssemble---- " + name);

            IsProcessing = true;

            if (transRecorder != null)
            {
                //回到动画播放后的位置
                transRecorder.Back();
            }
 
            yield return AppearWithAnim(usingTool);

            //螺丝都需要先放置后通过工具去安装
            State = CmsObjState.Placed;

            IsProcessing = false;
        }
    }
}
