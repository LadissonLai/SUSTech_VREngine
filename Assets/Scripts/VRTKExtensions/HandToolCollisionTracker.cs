using System;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using VRTK;

namespace VRTKExtensions
{
    public class HandToolCollisionTracker : MonoBehaviour
    {
        private bool invalidTouchObjState;

        private bool invalidUsingObjState;

        private bool triggerIsColliding;

        protected VRTK_InteractTouch interactTouch;

        protected VRTK_InteractUse interactUse;

        protected AdvancedInteractableObj toolInteractObj;
         
        protected AdvancedInteractableObj touchedObjByTool;

        /// <summary>
        /// OnUse,OnUnuse事件的回调顺序在清理usingObj之前，如果在事件中执行stopUsing会有问题。 缓存起来在下帧调用。
        /// </summary>
        protected List<AdvancedInteractableObj> preUsingObjs;

        private AdvancedInteractableObj objUsingCache;

        public VRTK_PolicyList policyList;

        public Predicate<AdvancedInteractableObj> CustomPredicate;

        public AdvancedInteractableObj UsingObjByTool
        {
            get => objUsingCache;

            protected set
            {
                if (value == objUsingCache)
                    return;

                preUsingObjs = preUsingObjs ?? new List<AdvancedInteractableObj>();

                //尝试删除被新使用的物体
                preUsingObjs.Remove(value);

                if (objUsingCache != null)
                    preUsingObjs.AddUnique(objUsingCache);
                 
                objUsingCache = value;

                invalidUsingObjState = true;
            }
        }

        public AdvancedInteractableObj TouchedObjByTool
        {
            get => touchedObjByTool;

            protected set
            {
                if (value == touchedObjByTool)
                    return;

                var preTouchedObjByTool = touchedObjByTool;

                if (preTouchedObjByTool != null)
                    preTouchedObjByTool.StopTouching(interactTouch);

                touchedObjByTool = value;

                invalidTouchObjState = true;
            }
        }

        protected virtual void Awake()
        {
            if (policyList == null)
                policyList = GetComponent<VRTK_PolicyList>();

            toolInteractObj = GetComponentInParent<AdvancedInteractableObj>();

            toolInteractObj.InteractableObjectGrabbed += InteractObj_InteractableObjectGrabbed;

            toolInteractObj.InteractableObjectUngrabbed += InteractObj_InteractableObjectUngrabbed;

            toolInteractObj.InteractableObjectUsed += ToolInteractObj_InteractableObjectUsed;

            toolInteractObj.InteractableObjectUnused += ToolInteractObj_InteractableObjectUnused;

            if (!toolInteractObj.IsGrabbed())
            {
                enabled = false;
            }
        }

        protected virtual void OnEnable()
        {
            interactTouch = toolInteractObj.GetGrabbingObject().GetComponent<VRTK_InteractTouch>();

            interactUse = interactTouch.GetComponent<VRTK_InteractUse>();
        }

        protected virtual void OnDisable()
        {
            TouchedObjByTool = null;

            UsingObjByTool = null;

            LateUpdate();
        }
 
        private void ToolInteractObj_InteractableObjectUsed(object sender, InteractableObjectEventArgs e)
        {
            //工具使用的时候同时触发对应物体的使用
            //Debug.Log("ToolInteractObj_InteractableObjectUsed:" + sender + "|" + e.interactingObject);

            if (TouchedObjByTool != null)
            {
                UsingObjByTool = TouchedObjByTool;
            }
        }

        private void ToolInteractObj_InteractableObjectUnused(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log("ToolInteractObj_InteractableObjectUnused:" + sender + "|" + e.interactingObject);

            UsingObjByTool = null;
        }

        private void InteractObj_InteractableObjectUngrabbed(object sender, InteractableObjectEventArgs e)
        {
            enabled = false;
        }

        private void InteractObj_InteractableObjectGrabbed(object sender, InteractableObjectEventArgs e)
        {
            enabled = true;
        }

        protected void OnTriggerEnter(Collider collider)
        {
            //Debug.Log("OnTriggerEnter---:" + collider);
            var targetInteract = collider.GetComponentInParent<AdvancedInteractableObj>();

            if (targetInteract == null || targetInteract == TouchedObjByTool)
                return;

            if (!CheckObjTouchAble(targetInteract))
                return;

            TouchedObjByTool = targetInteract;
        }

        private void OnTriggerStay(Collider other)
        {
            if (TouchedObjByTool != null && other.transform.IsChildOf(TouchedObjByTool.transform))
            {
                triggerIsColliding = true;

                return;
            }

            if(TouchedObjByTool == null)
            {
                OnTriggerEnter(other);
            }
        }

        protected void FixedUpdate()
        {
            if(!triggerIsColliding)
            {
                //没有触发trigger
                TouchedObjByTool = null;
            }

            triggerIsColliding = false;
        }

        protected void LateUpdate()
        {
            if (invalidTouchObjState)
            {
                invalidTouchObjState = false;
                 
                if(TouchedObjByTool != null && !TouchedObjByTool.IsTouched())
                    TouchedObjByTool.StartTouching(interactTouch);
            }

            if(invalidUsingObjState)
            {
                invalidUsingObjState = false;

                if(preUsingObjs != null && preUsingObjs.Count > 0)
                {
                    foreach (var obj in preUsingObjs)
                    {
                        obj.StopUsing(interactUse);
                    }

                    preUsingObjs.Clear();
                }

                if (UsingObjByTool != null && interactUse != null && !UsingObjByTool.IsUsing(interactUse.gameObject))
                    UsingObjByTool.StartUsing(interactUse);

            }
        }
 
        protected virtual bool CheckObjTouchAble(AdvancedInteractableObj obj)
        {
            if (!enabled)
                return false;

            if (!obj.isUsable || obj.IsUsing())
                return false;

            if(policyList != null && policyList.Find(obj.gameObject))
            {
                return false;
            }

            if (CustomPredicate != null && !CustomPredicate(obj))
                return false;

            return true;
        }
    }
}

