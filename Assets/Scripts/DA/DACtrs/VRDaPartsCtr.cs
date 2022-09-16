using Framework;
using UnityEngine;
using Fxb.DA;

namespace Fxb.CMSVR
{
    public class VRDaPartsCtr : DAObjCtr
    {
        public override CmsObjType Type => CmsObjType.Parts;

        /// <summary>
        /// 是否虚拟部件。 默认无交互即为虚拟部件
        /// </summary>
        public virtual bool IsVirtualParts => interactObj == null;

        protected override void SetupDependObjs()
        {
            base.SetupDependObjs();

            if (DependSnapFits == null)
                return;
             
            foreach (var snapfit in DependSnapFits)
            {
                if (snapfit.attachTo != null && snapfit.attachTo != this)
                    continue;
                 
                snapfit.attachTo = this;

                snapfit.OnStateChanged += Snapfit_OnStateChanged;

                if(snapfit is VRDAScrew screw)
                    screw.OnScrewWillPlaced += Screw_OnScrewWillPlaced;
            }
        }

        private void Screw_OnScrewWillPlaced(VRDAScrew screw, IDAUsingTool usingTool)
        {
            //优先使用手部抓取的物体，再判断桌上的物体是否足够           
            var cloneObj = usingTool as DACloneObjCtr;
 
            var targetScrew = screw;

            var cloneObjAmount = cloneObj.GetAmount();

            var objPropID = cloneObj.PropID;

            World.current.StartCoroutine(targetScrew.PlaceScrewBeforeAssemble(cloneObj));

            cloneObjAmount--;

            foreach (var snapFit in DependSnapFits)
            {
                targetScrew = snapFit as VRDAScrew;

                if (targetScrew == screw)
                    continue;

                if (!targetScrew.autoDisappear || targetScrew.State != CmsObjState.Dismantled)
                    continue;

                if (targetScrew.PropID != objPropID)
                    continue;

                if (cloneObjAmount == 0)
                {
                    //尝试去获取桌上的物体
                    if (World.Get<DASceneState>().cloneObjsInTable.TryGetValue(objPropID, out var cloneObjIdOnTable))
                    {
                        cloneObj = World.Get<DACloneObjCtr>(cloneObjIdOnTable);

                        cloneObjAmount = cloneObj.GetAmount();
                    }
                    else
                    {
                        cloneObj = null;
                    }
                }

                if (cloneObjAmount == 0)
                    break;
                 
                World.current.StartCoroutine(targetScrew.PlaceScrewBeforeAssemble(cloneObj));

                cloneObjAmount--;
            }
        }

        private void Snapfit_OnStateChanged(AbstractDAObjCtr snapfit)
        {
            if(snapfit.State == CmsObjState.Dismantled || snapfit.State == CmsObjState.WaitForPickup)
            {
                //螺丝卡扣被查下，部件解锁
                DoUnFix();

                if (IsVirtualParts && !DependSnapFits.Exists(CmsDAUtils.ObjsUnDismantledPredicate))
                {
                    //虚拟部件  螺丝/卡扣全被拆下后，自身也自动拆下
                    DoDisassemble(null);
                }
 
                return;
            }

            if(snapfit.State == CmsObjState.Placed || snapfit.State == CmsObjState.Assembled)
            {
                if(State == CmsObjState.Dismantled && interactObj == null)
                {
                    State = CmsObjState.Assembled;
                }

                return;
            }
            
            if(snapfit.State == CmsObjState.Default)
            {
                //所有螺丝卡扣都被安装成功，部件恢复到锁定状态
                if(!DependSnapFits.Exists(CmsDAUtils.ObjsUnDefaultPredicate))
                {
                    DoFix();
                }
            }
        }
    }
}

 
