using Framework;
using Fxb.Localization;
using System;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 提示触发器
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class InteractObjTooltipTriggerBase : MonoBehaviour
    {
        public bool isToolTipActived { get; protected set; }

        [Tooltip("是否需要翻译")]
        public bool isLocalizeTarget;

        [System.NonSerialized]
        public string overrideTipMsg;

        public string TipMsg
        {
            get
            {
                if (!string.IsNullOrEmpty(overrideTipMsg))
                    return overrideTipMsg;

                var msg = InteractableObject != null ? InteractableObject.readableName : null;

                if (msg != null && isLocalizeTarget)
                {
                    msg = LocalizeMgr.Inst.GetLocalizedStr(msg);
                }

                return msg;
            }
        }

        public virtual void RefTipMsg()
        {

        }

        public bool enableTooltipOnFocus = true;

        public bool enableTooltipOnTouch;

        private AdvancedInteractableObj interactableObject;

        public AdvancedInteractableObj InteractableObject
        {
            get
            {
                if (interactableObject == null)
                    interactableObject = GetComponent<AdvancedInteractableObj>();

                return interactableObject;
            }
        }

        protected virtual void OnDestroy()
        {
            if (isToolTipActived && !World.current.IsAppQuitting)
                HideTooltip();

            if (InteractableObject == null)
                return;

            InteractableObject.InteractableObjectFocusEnter -= OnObjFocusEnter;

            InteractableObject.InteractableObjectFocusExit -= OnObjFocusExit;

            InteractableObject.InteractableObjectTouched -= OnObjTouched;

            InteractableObject.InteractableObjectUntouched -= OnObjUnTouched;

            InteractableObject.InteractableObjectGrabbed -= OnObjGrabbed;
        }

        private void Start()
        {
            InteractableObject.InteractableObjectGrabbed += OnObjGrabbed;

            if (enableTooltipOnFocus)
            {
                InteractableObject.InteractableObjectFocusEnter += OnObjFocusEnter;

                interactableObject.InteractableObjectFocusExit += OnObjFocusExit;
            }

            if (enableTooltipOnTouch)
            {
                InteractableObject.InteractableObjectTouched += OnObjTouched;

                InteractableObject.InteractableObjectUntouched += OnObjUnTouched;
            }
        }
 
        protected virtual void OnObjGrabbed(object sender, InteractableObjectEventArgs e)
        {
            HideTooltip();
        }

        protected virtual void OnObjUnTouched(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log("OnObjUnTouched----------" + name);

            HideTooltip();
        }

        protected virtual void OnObjTouched(object sender, InteractableObjectEventArgs e)
        {
            //Debug.Log($"OnObjTouched----------  name:{name}  {e.interactingObject}");

            var touchHandHasGrabed = false;

            if (e.interactingObject == VRTKHelper.LeftHand)
                touchHandHasGrabed = VRTKHelper.LeftGrab.GetGrabbedObject() != null;
            else if (e.interactingObject == VRTKHelper.RightHand)
                touchHandHasGrabed = VRTKHelper.RightGrab.GetGrabbedObject() != null;

            if (touchHandHasGrabed)
                return;

            if (!string.IsNullOrEmpty(TipMsg))
                ShowTooltip();
        }

        protected virtual void OnObjFocusExit(object sender, InteractableObjectEventArgs e)
        {
            HideTooltip();
        }

        protected virtual void OnObjFocusEnter(object sender, InteractableObjectEventArgs e)
        {
            if(!string.IsNullOrEmpty(TipMsg))
                ShowTooltip();
        }

        public virtual void ShowTooltip()
        {
            isToolTipActived = true;
        }

        public virtual void HideTooltip()
        {
            isToolTipActived = false;
        }
    }
}

