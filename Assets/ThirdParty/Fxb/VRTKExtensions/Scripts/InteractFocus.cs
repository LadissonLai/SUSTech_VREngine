using Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRTK;

namespace VRTKExtensions
{
    /// <summary>
    /// 焦点交互
    /// </summary>
    [RequireComponent(typeof(VRTK_DestinationMarker))]
    public class InteractFocus : MonoBehaviour
    {
        protected static readonly string LAYER_NAME_UI = "UI";

        protected static readonly string VRTK_CANVAS_DRAGGABLE_PANEL = "VRTK_UICANVAS_DRAGGABLE_PANEL";
         
        public VRTK_DestinationMarker focusMarker;

        public VRTK_CustomRaycast markerCustomRaycast;

        public VRTK_UIPointer uiPointer;

        protected LayerMask layerUI;

        protected LayerMask defaultRaycastIgnore;

        protected LayerMask markerCustomLayerIgoreCache;

        protected IFocusAble currentObjInFocus;

        protected bool focusEnable = true;

        protected float lastUIElementEnteredTimeFlag;

        protected virtual void Awake()
        {
            layerUI = LayerMask.GetMask(LAYER_NAME_UI);

            if (focusMarker == null)
                focusMarker = GetComponent<VRTK_DestinationMarker>();

            Debug.Assert(markerCustomRaycast != null);

            Debug.Assert((markerCustomRaycast.layersToIgnore & layerUI) != 0, "默认需要忽略ui层");

            //记录默认的射线忽略层
            markerCustomLayerIgoreCache = defaultRaycastIgnore = markerCustomRaycast.layersToIgnore;

            AddMarkerEvents();
        }

        private void OnDestroy()
        {
            RemoveMarkerEvents();
        }

        private void AddMarkerEvents()
        {
            if (focusMarker == null)
                return;

            focusMarker.DestinationMarkerEnter += FocusMarker_DestinationMarkerEnter;

            focusMarker.DestinationMarkerExit += FocusMarker_DestinationMarkerExit;

            focusMarker.DestinationMarkerSet += FocusMarker_DestinationMarkerSet;

            focusMarker.DestinationMarkerHover += FocusMarker_DestinationMarkerHover;
        }

        private void RemoveMarkerEvents()
        {
            if (focusMarker == null)
                return;

            focusMarker.DestinationMarkerEnter -= FocusMarker_DestinationMarkerEnter;

            focusMarker.DestinationMarkerExit -= FocusMarker_DestinationMarkerExit;

            focusMarker.DestinationMarkerSet -= FocusMarker_DestinationMarkerSet;

            focusMarker.DestinationMarkerHover -= FocusMarker_DestinationMarkerHover;
        }

        public void SetRaycastIgnoreCustom(LayerMask customIgnore)
        {
            if (markerCustomLayerIgoreCache.value == customIgnore.value)
                return;

            markerCustomLayerIgoreCache = customIgnore;
        }

        public void ResetRaycastIgnore()
        {
            markerCustomLayerIgoreCache = defaultRaycastIgnore;
        }

        public void SetFocusEnable(bool enable)
        {
            if (enable == focusEnable)
                return;

            focusEnable = enable;
        }

        protected virtual void Update()
        {
            if (IsUIElementEntered())
            {
                //ui被选中时只允许ui层被检测
                markerCustomRaycast.layersToIgnore = ~layerUI;

                lastUIElementEnteredTimeFlag = Time.realtimeSinceStartup;
            }
            else
            {
                if ((Time.realtimeSinceStartup - lastUIElementEnteredTimeFlag) < 0.1f)
                    return;

                //无ui元素被选中
                if (focusEnable)
                    markerCustomRaycast.layersToIgnore = markerCustomLayerIgoreCache;
                else
                    markerCustomRaycast.layersToIgnore.value = Physics.AllLayers;
            }
        }
 
        /// <summary>
        /// 不同的canvas叠加交互切换时可能会有1帧检测到无ui被交互
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsUIElementEntered()
        {
            if (uiPointer == null)
            {
                return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            }

            if (!uiPointer.PointerActive())
                return false;

            var pointerEnter = uiPointer.pointerEventData.pointerEnter;

            if (pointerEnter == null)
                return false;

            if (pointerEnter.name == VRTK_CANVAS_DRAGGABLE_PANEL)
                return false;

            return true;
        }

        #region focusable evts

        protected virtual void FocusMarker_DestinationMarkerHover(object sender, DestinationMarkerEventArgs e)
        {

        }

        protected virtual void FocusMarker_DestinationMarkerEnter(object sender, DestinationMarkerEventArgs e)
        {
            var focusAbleObj = e.target.GetComponentInParent<IFocusAble>();

            if (focusAbleObj == null)
                return;

            if (currentObjInFocus == focusAbleObj)
                return;

            if (focusAbleObj is AdvancedInteractableObj interactObj)
            {
                interactObj = interactObj.GetValidInteractAbleObj();

                if (interactObj != null)
                    focusAbleObj = interactObj;
            }

            currentObjInFocus = focusAbleObj;

            focusAbleObj.OnInteractableObjectFocusEnter(new InteractableObjectEventArgs() { interactingObject = gameObject });
        }

        protected virtual void FocusMarker_DestinationMarkerSet(object sender, DestinationMarkerEventArgs e)
        {
            if (currentObjInFocus == null)
                return;

            currentObjInFocus.OnInteractableObjectFocusSet(new InteractableObjectEventArgs() { interactingObject = gameObject });
        }

        protected virtual void FocusMarker_DestinationMarkerExit(object sender, DestinationMarkerEventArgs e)
        {
            if (e.target == null)
                return;

            if (currentObjInFocus == null)
                return;

            currentObjInFocus.OnInteractableObjectFocusExit(new InteractableObjectEventArgs() { interactingObject = gameObject });
            currentObjInFocus = null;
        }

        #endregion

    }
}
