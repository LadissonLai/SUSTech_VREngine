using Framework;
using System;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 加入focus相关交互.
    /// 加入tip相关表现
    /// </summary>
    public class AdvancedInteractableObj : VRTK_InteractableObject, IFocusAble
    {
        [SerializeField]
        private AdvancedInteractableObj overrideParentObj;

        public AdvancedInteractableObj OverrideParentObj
        {
            get => overrideParentObj;

            set
            {
                if (overrideParentObj == value)
                    return;

                overrideParentObj = value;

                parentObjInvalid = true;
            }
        }
 
        private AdvancedInteractableObj parentInteractObj;

        private bool parentObjInvalid = true;
         
        /// <summary>
        /// 人类可读的物体名字
        /// </summary>
        public string readableName;

        public bool isTiped {get; protected set;}

        public bool isInFocused {get; protected set;}

        /// <summary>
        /// 是否允许交互。 暂时只实现了focus相关交互
        /// </summary>
        /// <value></value>
        public bool isInteractable {get; private set;} = true;

        public bool visible {get; protected set;} = true;

        [Tooltip("是否允许事件冒泡 ")]
        public bool enableBubbling = true;

        [Tooltip("射线focus set是否触发自动抓取（允许抓取的情况下）")]
        public bool autoGrabOnFocusSetIfEnable = true;

        public event InteractableObjectEventHandler InteractableObjectFocusEnter;

        public event InteractableObjectEventHandler InteractableObjectFocusExit;

        public event InteractableObjectEventHandler InteractableObjectFocusSet;

        public event Action<AdvancedInteractableObj> InteractableObjectTiped;

        public event Action<AdvancedInteractableObj> InteractableObjectUnTip;

        /// <summary>
        /// param:当前物体,当前是否允许放置
        /// </summary>
        public event Action<AdvancedInteractableObj, bool> InteractableObjectTryDrop;
        
        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);

            var grab = e.interactingObject.GetComponent<VRTK_InteractGrab>();

            grab.GrabButtonPressed += Grab_GrabButtonPressed;
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);
 
            var grab = e.interactingObject.GetComponent<VRTK_InteractGrab>();

            grab.GrabButtonPressed -= Grab_GrabButtonPressed;
        }

        private void Grab_GrabButtonPressed(object sender, ControllerInteractionEventArgs e)
        {
            InteractableObjectTryDrop?.Invoke(this, IsDroppable());
        }
 
        protected override bool IsIdle()
        {
            if(base.IsIdle())
            {
                return !isInFocused;
            }

            return false;
        }

        protected virtual AdvancedInteractableObj FindParentObjs()
        {
            if (overrideParentObj != null)
                return overrideParentObj;

            return transform.FindComponentInParents<AdvancedInteractableObj>(10);
        }

        protected virtual void OnTransformParentChanged() {
            //Debug.Log("OnTransformParentChanged:" + transform.parent);

            parentObjInvalid = true;
        }
        
        protected override void OnDisable() {
            OnInteractableObjectFocusExit(default);
             
            base.OnDisable();
             
        }

        protected override void Awake() {
            base.Awake();
        }

        protected virtual void OnDestroy() {
            
        }
    
        /// <summary>
        /// 待调整 把获取父层物体和获取自身分开
        /// </summary>
        /// <returns></returns>
        public AdvancedInteractableObj GetValidInteractAbleObj()
        {
            if(enableBubbling)
            {
                if (parentObjInvalid)
                {
                    parentInteractObj = FindParentObjs();
 
                    parentObjInvalid = false;
                }

                if (parentInteractObj != null)
                {
                    var validParentObj = parentInteractObj.GetValidInteractAbleObj();

                    if (validParentObj != null)
                        return validParentObj;
                }
            }
 
            return isInteractable ? this : null;
        }

        public virtual void OnInteractableObjectFocusEnter(InteractableObjectEventArgs e)
        { 
            if(!isInteractable)
                return;

            if(isInFocused)
                return;

            isInFocused = true;
 
            InteractableObjectFocusEnter?.Invoke(this, e);

            ToggleEnableState(true);
        }

        public virtual void OnInteractableObjectFocusExit(InteractableObjectEventArgs e)
        {
            if(!isInFocused)
                return;
 
            isInFocused = false;

            InteractableObjectFocusExit?.Invoke(this, e);
        }

        public virtual void OnInteractableObjectFocusSet(InteractableObjectEventArgs e)
        {
            if(!isInteractable)
                return;
            
            DoFocusSet(e);
        }

        protected virtual void DoFocusSet(InteractableObjectEventArgs e)
        {
            InteractableObjectFocusSet?.Invoke(this, e);
        }

        /// <summary>
        /// 设置物体为提示状态
        /// </summary>
        public virtual void TipInteractableObj()
        {
            if(isTiped)
                return;

            isTiped = true;

            InteractableObjectTiped?.Invoke(this);
        }

        /// <summary>
        /// 取消提示状态
        /// </summary>
        public virtual void UnTipInteractableObj()
        {
            if(!isTiped)
                return;

            isTiped = false;

            InteractableObjectUnTip?.Invoke(this);
        }

        public virtual void SetVisible(bool val)
        {
            
        }

        /// <summary>
        /// 打开或者关闭物体交互
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="withCollision">是否打开或关闭碰撞体（碰撞与射线）</param>
        /// <param name="withChildObjs">是否包括子物体</param>
        public virtual void SetInteractAble(bool isEnable, bool withCollision = false)
        {
            if(isInteractable == isEnable)
                return;

            isInteractable = isEnable;

            if(!isInteractable && isInFocused)
            {
                isInFocused = false;

                InteractableObjectFocusExit?.Invoke(this, new InteractableObjectEventArgs());
            }

            if(withCollision)
            {
                this.SetChildCollisionEnable(isInteractable, true);
            }
        }
    }
}

