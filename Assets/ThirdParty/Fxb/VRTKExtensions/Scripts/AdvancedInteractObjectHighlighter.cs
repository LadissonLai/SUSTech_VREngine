
using System;
using UnityEngine;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 增加focus交互高亮
    /// </summary>
    public class AdvancedInteractObjectHighlighter : VRTK_InteractObjectHighlighter
    {
        [Tooltip("The colour to highlight the object on the use interaction.")]
        public Color focusHighlight = Color.clear;

        public Color tipHighlight = Color.clear;

        protected override bool SetupListeners(bool throwError)
        {
            base.SetupListeners(throwError);

            if(objectToMonitor != null && objectToMonitor is AdvancedInteractableObj obj)
            {
                if (obj.isTiped)
                    OnObjTiped(obj);

                obj.InteractableObjectFocusEnter += OnObjFocusEnter;
                obj.InteractableObjectFocusExit += OnObjFocusExit;
                obj.InteractableObjectTiped += OnObjTiped;
                obj.InteractableObjectUnTip += OnObjUnTip;

                return true;
            }

            return false;
        }

        protected override void TearDownListeners()
        {
            if(objectToMonitor != null && objectToMonitor is AdvancedInteractableObj obj)
            {
                obj.InteractableObjectFocusEnter -= OnObjFocusEnter;
                obj.InteractableObjectFocusExit -= OnObjFocusExit;
                obj.InteractableObjectTiped -= OnObjTiped;
                obj.InteractableObjectUnTip -= OnObjUnTip;
            }

            base.TearDownListeners();
        }
 
        private void OnObjUnTip(AdvancedInteractableObj interactObj)
        {
            if(interactObj.IsTouched() || interactObj.IsNearTouched() || interactObj.isInFocused)
                return;

            Unhighlight();

            OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, interactObj.gameObject));
        }

        private void OnObjTiped(AdvancedInteractableObj interactObj)
        {
            if(baseHighlighter != null && baseHighlighter.active)
                return;

            Highlight(tipHighlight);

            //InteractionType 不好扩展
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, interactObj.gameObject));
        }

        private void OnObjFocusEnter(object sender, InteractableObjectEventArgs e)
        {
            Highlight(focusHighlight);

            //InteractionType 不好扩展
            OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, e.interactingObject));
        }

        protected override void TouchUnHighlightObject(object sender, InteractableObjectEventArgs e)
        {
            var interactableObject = sender as AdvancedInteractableObj;

            var color = Color.clear;
             
            if (interactableObject.IsNearTouched())
                color = nearTouchHighlight;
            else if (interactableObject.isTiped)
                color = tipHighlight;

            if (color != Color.clear)
            {
                Highlight(color);

                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, e.interactingObject));
            }
            else
            {
                Unhighlight();

                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, e.interactingObject));
            }
        }

        private void OnObjFocusExit(object sender, InteractableObjectEventArgs e)
        {
            var interactObj = sender as AdvancedInteractableObj;

            var color = Color.clear;

            if (interactObj.IsTouched())
                color = touchHighlight;
            else if (interactObj.IsNearTouched())
                color = nearTouchHighlight;
            else if (interactObj.isTiped)
                color = tipHighlight;

            if(color != Color.clear)
            {
                Highlight(color);

                OnInteractObjectHighlighterHighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, e.interactingObject));
            }
            else
            {
                Unhighlight();

                OnInteractObjectHighlighterUnhighlighted(SetEventArgs(VRTK_InteractableObject.InteractionType.None, e.interactingObject));
            }
        }
    }
}
