using System.Collections;
using System.Collections.Generic;
using Fxb.DA;
using UnityEngine;
using Framework;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 模块组  
    /// 逻辑修改：1.类似之前的无依赖时自动拆下的逻辑，模块组本身不需要拆装，根据依赖的部件是否拆下来决定自身状态
    /// </summary>
    public class VRDaModelGroupCtr : VRDaPartsCtr
    {
        public override CmsObjType Type => CmsObjType.ModelGroup;

        public override bool AutoDefault => false;

        protected override Material PlaceHolderMat
        {
            get
            {
                return Resources.Load<Material>("Mats/Clear");
            }
        }

        //待测试 应该已弃用
        //[Tooltip("是否使用内部部件作为placeholder作为占位的展示 目前依赖 autoDisassemblyIfNoDepends 的开启")]
        //private bool placeholderWithInternalParts = false;

        //[Tooltip("是否指定place holder的占位parts")]
        //public List<AbstractDAObjCtr> customPlaceHolderParts;

        /// <summary>
        /// 所有子部件，缓存起来作为判断是否所有部件都安装完成的依据
        /// </summary>
        private List<AbstractDAObjCtr> allDependParts = new List<AbstractDAObjCtr>();

        protected override void Awake()
        {
            base.Awake();

            DebugEx.AssertIsTrue(DependSnapFits == null || DependSnapFits.Count > 0, "模块本身不能依赖螺丝/卡扣");
            DebugEx.AssertIsTrue(interactObj == null, "模块本身不能拥有交互功能");

            if(autoDisappear)
            {
                Debug.LogWarning("模块组不开启autoDisappear属性");

                autoDisappear = false;
            }
        }

        protected override void SetupDependObjs()
        {
            base.SetupDependObjs();
 
            SetupDependParts(this);
        }

        /// <summary>
        /// 所有依赖的部件交互都指向模块自身。
        /// 暂时不考虑内部部件的情况。
        /// </summary>
        /// <param name="objCtr"></param>
        private void SetupDependParts(AbstractDAObjCtr objCtr)
        {
            if (objCtr.DependParts == null)
                return;

            foreach (var parts in objCtr.DependParts)
            {
                if (parts.attachTo != null && parts.attachTo != this)
                    continue;

                parts.attachTo = this;

                allDependParts.AddUnique(parts);

                parts.OnStateChanged += Parts_OnStateChanged;

                SetupDependParts(parts);
            }
        }

        private void Parts_OnStateChanged(AbstractDAObjCtr part)
        {
            if(part.State == CmsObjState.Default)
            {
                //所有部件安装成功则模块切换成default状态
                if(allDependParts.TrueForAll(CmsDAUtils.ObjsDefaultPredicate))
                {
                    State = CmsObjState.Default;
                }
            }
            else
            {
                //部件状态改变，模块不再是default状态
                if (State == CmsObjState.Default)
                {
                    State = CmsObjState.Fixed;
                }

                if(part.State == CmsObjState.Dismantled)
                {
                    if(allDependParts.TrueForAll(CmsDAUtils.ObjsDismantledPredicate))
                    {
                        //所有部件被拆下
                        State = CmsObjState.Dismantled;
                    }
                }

                //Debug.Log($"Parts_OnStateChanged  part:{part}  state:{part.State}");
            }
        }
    }
}

