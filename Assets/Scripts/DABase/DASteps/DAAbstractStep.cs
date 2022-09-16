using Doozy.Engine;
using Framework;
using Framework.Tools;
using System.Collections.Generic;
using UnityEngine;
using VRTKExtensions;

namespace Fxb.DA
{
    public abstract class DAAbstractStep
    {
        public static ObjectPool<List<AbstractDAObjCtr>> cmsListPool;

        public bool IsRunning { get; protected set; }

        /// <summary>
        /// 不是很好用   待修改
        /// </summary>
        public abstract DAStepMode Mode { get; }

        public AbstractDAObjCtr root;

        public DAAbstractStep subStep;

        public bool needExit;

        protected static List<InteractFocus> CachedInteractFocus => World.Get<List<InteractFocus>>();

        protected DAState DAState { get => World.Get<DAState>(); }

        protected DATipMessage tipMessageCache;

        /// <summary>
        /// 当前被激活的所有物体
        /// </summary>
        protected List<AbstractDAObjCtr> validDAObjs;

        protected virtual bool CheckFinished()
        {
            if (DAState.processingObjs.Count > 0)
                return false;

            if (subStep != null || needExit)
                return false;

            return true;
        }

        /// <summary>
        /// 刷新当前需要被激活的拆装物体
        /// </summary>
        protected abstract void UpdateValidDAObjs();

        /// <summary>
        /// 需要刷新拆装物体激活状态的条件
        /// </summary>
        /// <returns></returns>
        protected abstract bool ValidDAObjsUpdatePredicate();

        protected virtual void SetObjActive(AbstractDAObjCtr daObj, bool actived, bool updateForceInteract = false)
        {
            if (string.IsNullOrEmpty(DAConfig.forceInteractLayer))
                updateForceInteract = false;

            if(updateForceInteract)
            {
                var applyLayer = actived ? LayerMask.NameToLayer(DAConfig.forceInteractLayer) : 0;

                daObj.gameObject.ApplyLayer(applyLayer);
            }
 
            daObj.SetActived(actived);
        }

        protected virtual void SetObjActives(IList<AbstractDAObjCtr> daObjs, bool actived, bool updateForceInteract = false)
        {
            if (daObjs == null)
                return;

            if (string.IsNullOrEmpty(DAConfig.forceInteractLayer))
                updateForceInteract = false;

            foreach (var obj in daObjs)
            {
                SetObjActive(obj, actived, updateForceInteract);
            }

            if (updateForceInteract)
            {
                foreach (var interactFocus in CachedInteractFocus)
                {
                    if (interactFocus != null)
                    {
                        if (actived)
                            interactFocus.SetRaycastIgnoreCustom(~LayerMask.GetMask(DAConfig.forceInteractLayer));
                        else
                            interactFocus.ResetRaycastIgnore();
                    }
                }
            }
        }
 
        public DAAbstractStep(){}

        public virtual void Start(AbstractDAObjCtr root)
        {
            Debug.Assert(root != null);

            IsRunning = true;

            this.root = root;

            DebugEx.AssertIsTrue(validDAObjs == null);

            validDAObjs = cmsListPool.Spawn();

            if (Mode == DAStepMode.ModelGroupAssemble || Mode == DAStepMode.ModelGroupDisassemble)
                DAState.validProcessParts = validDAObjs;
            else
                DAState.validProcessSnapFits = validDAObjs; 

            Message.AddListener<DAObjStateChangeMessage>(OnDAObjStateChangeMessage);
        }

        protected virtual void Reset()
        {
            Debug.Log("step reset:" + this);

            IsRunning = false;

            root = null;

            needExit = false;

            subStep = null;

            if (DAState.validProcessParts == validDAObjs)
                DAState.validProcessParts = null;
            else if (DAState.validProcessSnapFits == validDAObjs)
                DAState.validProcessSnapFits = null;

            cmsListPool.Despawn(validDAObjs);

            validDAObjs = null;

            Message.RemoveListener<DAObjStateChangeMessage>(OnDAObjStateChangeMessage);
        }

        private void OnDAObjStateChangeMessage(DAObjStateChangeMessage obj)
        {
            OnDAObjStateChanged(obj.objCtr);
        }

        /// <summary>
        /// 直接退出
        /// </summary>
        public virtual void Exit()
        {
            if (subStep != null)
                subStep.Exit();

            needExit = true;
        }

        /// <summary>
        /// 取消当前步骤
        /// </summary>
        /// <returns></returns>
        public virtual void Cancel()
        {
            if (subStep != null)
            {
                subStep.Cancel();

                //当前步骤的目标一样  一起退出
                if (subStep.root == root)
                    Exit();
            }
            else
            {
                Exit();
            }
        }

        /// <summary>
        /// 外部Step统一调用 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool TryProcessObj(AbstractDAObjCtr target)
        {
            if (subStep != null)
            {
                subStep.TryProcessObj(target);

                return false;
            }

            DoProcessObj(target);

            return true;
        }

        protected abstract void DoProcessObj(AbstractDAObjCtr target);

        protected virtual void OnDAObjStateChanged(AbstractDAObjCtr objCtr)
        {

        }

        protected virtual void OnStepComplete()
        {

        }

        public virtual bool MoveNext()
        {
            if (subStep != null)
            {
                if (subStep.MoveNext())
                {
                    subStep = null;
                }

                return false;
            }

            var hasFinished = CheckFinished();

            if (hasFinished)
                OnStepComplete();

            if (needExit || hasFinished)
            {
                Reset();

                return true;
            }

            return false;
        }

        //一下为新增内容   待确定

        /// <summary>
        /// attach to 到其它模块只能通过对应模块开启
        /// </summary>
        /// <param name="objCtr"></param>
        /// <returns></returns>
        protected bool AttachConditionCheck(AbstractDAObjCtr objCtr)
        {
            if (objCtr.attachTo == null || objCtr.attachTo.Type != CmsObjType.ModelGroup)
                return true;

            if (objCtr.attachTo.Type == CmsObjType.Parts)
                return AttachConditionCheck(objCtr.attachTo);

            return objCtr.attachTo == root;
        }
    }
}
