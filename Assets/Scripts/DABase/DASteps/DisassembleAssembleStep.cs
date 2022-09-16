using Doozy.Engine;
using Framework;
using Fxb.CMSVR;
using Fxb.DA;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fxb.CMSVR
{
    /// <summary>
    /// 同时进行拆卸与安装
    /// 1.默认所有模块都进入拆卸模式，激活最外层依赖。
    /// 2.同时可以进行安装。
    /// 3.状态相关的更改放到对应拆装物体的控制中实现。
    /// </summary>
    public class DisassembleAssembleStep : DAAbstractStep
    {
        public override DAStepMode Mode => DAStepMode.ModelGroupAssemble;

        private List<AbstractDAObjCtr> preValidDAObjs;

        /// <summary>
        /// 可拆下的部件
        /// </summary>
        protected List<AbstractDAObjCtr> validDisassembleParts;
        
        /// <summary>
        /// 可拆下的螺丝卡扣
        /// </summary>
        protected List<AbstractDAObjCtr> validDisassembleSnapfits;

        /// <summary>
        /// 可安装的部件
        /// </summary>
        protected List<AbstractDAObjCtr> validAssembleParts;

        /// <summary>
        /// 可安装的螺丝卡扣
        /// </summary>
        protected List<AbstractDAObjCtr> validAssembleSnapfits;

        protected bool isDAObjsInvalid;
 
        protected override void Reset()
        {
            Message.RemoveListener<DAObjStateChangeMessage>(OnDAObjStateChangeMessage);

            cmsListPool.Despawn(validDisassembleParts);

            cmsListPool.Despawn(validDisassembleSnapfits);

            cmsListPool.Despawn(validAssembleParts);

            cmsListPool.Despawn(validAssembleSnapfits);

            base.Reset();
        }

        public override void Start(AbstractDAObjCtr root)
        {
            base.Start(root);
 
            Message.AddListener<DAObjStateChangeMessage>(OnDAObjStateChangeMessage);

            validDisassembleParts = cmsListPool.Spawn();

            validDisassembleSnapfits = cmsListPool.Spawn();

            validAssembleParts = cmsListPool.Spawn();

            validAssembleSnapfits = cmsListPool.Spawn();

            preValidDAObjs = cmsListPool.Spawn();

            isDAObjsInvalid = true;
        }

        protected override bool CheckFinished()
        {
            if (base.CheckFinished() && ValidDAObjsUpdatePredicate())
                UpdateValidDAObjs();

            //常驻
            return false;
        }

        protected override bool ValidDAObjsUpdatePredicate()
        {
            return isDAObjsInvalid;
        }

        protected override void DoProcessObj(AbstractDAObjCtr target)
        {
            throw new System.NotImplementedException();
        }

        private void OnDAObjStateChangeMessage(DAObjStateChangeMessage message)
        {
            var objCtr = message.objCtr;

            if (!validDAObjs.Contains(objCtr))
                return;

            isDAObjsInvalid = true;

            //通过先关闭再开启来刷新内部状态
            objCtr.SetActived(false);

            //Debug.Log("刷新拆装状态");
        }
 
        protected override void OnDAObjStateChanged(AbstractDAObjCtr objCtr)
        {
           //pass
        }

        protected void UpdateAssembleDAObjs()
        {
            validAssembleParts.Clear();

            validAssembleSnapfits.Clear();

            //暂不添加内部部件逻辑

            //添加依赖部件
            CmsDAUtils.FindDepandMatchObjs(root, CmsObjType.Parts, ~CmsObjState.Default, validAssembleParts, false, AttachConditionCheck);

            CmsDAUtils.RemovePartsByDepend(validAssembleParts);

            for (int i = validAssembleParts.Count - 1; i >= 0; i--)
            {
                var parts = validAssembleParts[i];

                var isVirtualParts = (parts is VRDaPartsCtr daParts) && daParts.IsVirtualParts;
 
                if (!isVirtualParts && parts.State == CmsObjState.Dismantled)
                {
                    //部件被拆下需要先装上部件，再进行螺丝等的安装
                    //如果interactObj为空（虚拟部件），直接将螺丝激活，螺丝安装完成后此部件自动安装

                    continue;
                }

                var tmpSnapfits = cmsListPool.Spawn();

                CmsDAUtils.FindDepandMatchObjs(parts, CmsObjType.SnapFit, ~CmsObjState.Default, tmpSnapfits, false);

                if (tmpSnapfits.Count > 0)
                {
                    validAssembleParts.Remove(parts);

                    foreach (var snapfit in tmpSnapfits)
                    {
                        validAssembleSnapfits.AddUnique(snapfit);
                    }
                }

                cmsListPool.Despawn(tmpSnapfits);
            }
        }

        protected void UpdateDisassembleDAObjs()
        {
            validDisassembleParts.Clear();

            validDisassembleSnapfits.Clear();

            //找到最外层未拆下的部件
            CmsDAUtils.FindDepandMatchObjs(root, CmsObjType.Parts, ~CmsObjState.Dismantled, validDisassembleParts, true, AttachConditionCheck);

            for (int i = validDisassembleParts.Count - 1; i >= 0; i--)
            {
                var part = validDisassembleParts[i];

                var tmpSnapfits = cmsListPool.Spawn();

                CmsDAUtils.FindDepandMatchObjs(part, CmsObjType.SnapFit, ~CmsObjState.Dismantled, tmpSnapfits, true);

                if (tmpSnapfits.Count > 0)
                {
                    //有依赖螺丝没拆下
                    validDisassembleParts.Remove(part);

                    validDisassembleSnapfits.AddRangeUnique(tmpSnapfits);
                }
                  
                cmsListPool.Despawn(tmpSnapfits);
            }
        }

        #region 旧逻辑

        /// <summary>
        /// 将未拆下的部件的螺丝激活，将可以被拆下的部件本身激活
        /// </summary>
        //protected void UpdateDisassembleDAObjs_backup()
        //{
        //    validDisassembleParts.Clear();

        //    validDisassembleSnapfits.Clear();

        //    //找到所有未拆下的部件->validDisassembleParts
        //    CmsDAUtils.FindAllDepandMatchObjs(root, CmsObjType.Parts, ~CmsObjState.Dismantled, validDisassembleParts, AttachConditionCheck);

        //    for (int i = validDisassembleParts.Count - 1; i >= 0; i--)
        //    {
        //        var part = validDisassembleParts[i];

        //        var tmpSnapfits = cmsListPool.Spawn();

        //        CmsDAUtils.FindDepandMatchObjs(part, CmsObjType.SnapFit, ~CmsObjState.Dismantled, tmpSnapfits, true);

        //        var hasDepandParts = CmsDAUtils.CheckDepandObjExist(part, CmsObjType.Parts, ~CmsObjState.Dismantled, out _);

        //        if (tmpSnapfits.Count > 0 || hasDepandParts)
        //        {
        //            //可拆除的部件有依赖的螺丝/卡扣
        //            validDisassembleParts.Remove(part);

        //            if (tmpSnapfits.Count > 0)
        //            {
        //                foreach (var snapfit in tmpSnapfits)
        //                {
        //                    validDisassembleSnapfits.AddUnique(snapfit);
        //                }
        //            }
        //        }

        //        cmsListPool.Despawn(tmpSnapfits);
        //    }
        //}
        #endregion
 
        /// <summary>
        /// 取消不合法的物体的激活状态
        /// </summary>
        protected void DeactiveInvalidDAObjs(List<AbstractDAObjCtr> checkList, List<AbstractDAObjCtr> newList)
        {
            if (checkList.Count == 0)
                return;

            foreach (var obj in checkList)
            {
                if (newList.Contains(obj))
                    continue;

                SetObjActive(obj, false);
            }
        }

        protected override void UpdateValidDAObjs()
        {
            //Debug.Log("UpdateValidDAObjs-----");

            isDAObjsInvalid = false;

            validDAObjs.Clear();

            UpdateDisassembleDAObjs();

            //UpdateDisassembleDAObjs_backup();

            UpdateAssembleDAObjs();

            validDAObjs.AddRangeUnique(validDisassembleSnapfits);

            validDAObjs.AddRangeUnique(validDisassembleParts);

            validDAObjs.AddRangeUnique(validAssembleSnapfits);

            validDAObjs.AddRangeUnique(validAssembleParts);

            DeactiveInvalidDAObjs(preValidDAObjs, validDAObjs);

            preValidDAObjs.Clear();

            preValidDAObjs.AddRange(validDAObjs);

            //拆装物体基本上会一直处于Active状态，拆下后默认就可以安装
            //有可能当前物体Active了但是DisplayMode处于Hide状态，需要其它条件来激活（比如抓取到了对应物体）
            SetObjActives(validDAObjs, true);
        }
    }
}
