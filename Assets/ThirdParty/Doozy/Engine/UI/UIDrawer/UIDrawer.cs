// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Orientation;
using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable VirtualMemberCallInConstructor

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system that works and behaves like a Navigation Drawer, that can be opened/closed from/to the Left/Right/Up/Down side of the screen.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIDrawer_AddComponentMenu_MenuName, MenuUtils.UIDrawer_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIDRAWER)]
    public class UIDrawer : UIComponentBase<UIDrawer>, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIDrawer_MenuItem_ItemName, false, MenuUtils.UIDrawer_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject(MenuUtils.UIDrawer_GameObject_Name, typeof(RectTransform), typeof(UIDrawer));
            GameObjectUtility.SetParentAndAlign(go, GetCanvasAsParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            var drawer = go.GetComponent<UIDrawer>();
            drawer.DrawerName = DefaultDrawerName;
            drawer.RectTransform.FullScreen(true);

            var overlay = new GameObject("Overlay", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Image));
            GameObjectUtility.SetParentAndAlign(overlay, go);
            overlay.GetComponent<Image>().color = DoozyUtils.OverlayColor;
            drawer.Overlay.RectTransform = overlay.GetComponent<RectTransform>();
            drawer.Overlay.RectTransform.localPosition = Vector3.zero;
            drawer.Overlay.FullScreen(true);
            drawer.Overlay.Init();

            var container = new GameObject("Container", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup));
            GameObjectUtility.SetParentAndAlign(container, go);
            drawer.Container.RectTransform = container.GetComponent<RectTransform>();
            drawer.Container.RectTransform.localPosition = Vector3.zero;
            drawer.Container.FullScreen(true);
            drawer.Container.Init();

            GameObject background = DoozyUtils.CreateBackgroundImage(container);
            background.transform.localPosition = Vector3.zero;

            var arrowContainer = new GameObject("ArrowContainer", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(arrowContainer, go);
            var arrowContainerRectTransform = arrowContainer.GetComponent<RectTransform>();
            arrowContainerRectTransform.FullScreen(true);
            drawer.Arrow.Container = arrowContainerRectTransform;
            arrowContainer.gameObject.SetActive(false);

            foreach (SimpleSwipe swipe in Enum.GetValues(typeof(SimpleSwipe)))
            {
                if (swipe == SimpleSwipe.None) continue;

                var root = new GameObject(swipe.ToString(), typeof(RectTransform));
                GameObjectUtility.SetParentAndAlign(root, arrowContainer);
                var rootRectTransform = root.GetComponent<RectTransform>();

                UIDrawerArrow.ResetArrowRootPosition(rootRectTransform, swipe);

                var closed = new GameObject("ClosedPosition", typeof(RectTransform));
                GameObjectUtility.SetParentAndAlign(closed, root);
                var closedRectTransform = closed.GetComponent<RectTransform>();
                UIDrawerArrow.ResetArrowClosedPosition(closedRectTransform, swipe);

                var opened = new GameObject("OpenedPosition", typeof(RectTransform));
                GameObjectUtility.SetParentAndAlign(opened, root);
                var openedRectTransform = opened.GetComponent<RectTransform>();
                UIDrawerArrow.ResetArrowOpenedPosition(openedRectTransform, swipe);

                switch (swipe)
                {
                    case SimpleSwipe.Left:
                        drawer.Arrow.Left.Root = rootRectTransform;
                        drawer.Arrow.Left.Closed = closedRectTransform;
                        drawer.Arrow.Left.Opened = openedRectTransform;
                        break;
                    case SimpleSwipe.Right:
                        drawer.Arrow.Right.Root = rootRectTransform;
                        drawer.Arrow.Right.Closed = closedRectTransform;
                        drawer.Arrow.Right.Opened = openedRectTransform;
                        break;
                    case SimpleSwipe.Up:
                        drawer.Arrow.Up.Root = rootRectTransform;
                        drawer.Arrow.Up.Closed = closedRectTransform;
                        drawer.Arrow.Up.Opened = openedRectTransform;
                        break;
                    case SimpleSwipe.Down:
                        drawer.Arrow.Down.Root = rootRectTransform;
                        drawer.Arrow.Down.Closed = closedRectTransform;
                        drawer.Arrow.Down.Opened = openedRectTransform;
                        break;
                }
            }

            var arrow = new GameObject("Arrow", typeof(RectTransform), typeof(UIDrawerArrowAnimator));
            GameObjectUtility.SetParentAndAlign(arrow, arrowContainer);
            arrow.GetComponent<RectTransform>().Center(true);
            arrow.transform.localPosition = Vector3.zero;
            var animator = arrow.GetComponent<UIDrawerArrowAnimator>();
            drawer.Arrow.Animator = animator;

            var rotator = new GameObject("Rotator", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(rotator, arrow);
            var rotatorRectTransform = rotator.GetComponent<RectTransform>();
            rotatorRectTransform.Center(true);
            rotatorRectTransform.sizeDelta = new Vector2(100, 100);
            rotatorRectTransform.localPosition = Vector3.zero;
            animator.Rotator = rotatorRectTransform;

            var leftBar = new GameObject("LeftBar", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(leftBar, rotator);
            var leftBarRectTransform = leftBar.GetComponent<RectTransform>();
            leftBarRectTransform.localScale = Vector3.one;
            leftBarRectTransform.anchorMin = new Vector2(0f, 0.5f);
            leftBarRectTransform.anchorMax = new Vector2(0f, 0.5f);
            leftBarRectTransform.pivot = new Vector2(1f, 0.5f);
            leftBarRectTransform.sizeDelta = new Vector2(30, 6);
            leftBarRectTransform.anchoredPosition = new Vector2(52, 0);
            leftBarRectTransform.localPosition = new Vector3(2, 0, 0);
            animator.LeftBar = leftBarRectTransform;

            var rightBar = new GameObject("RightBar", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(rightBar, rotator);
            var rightBarRectTransform = rightBar.GetComponent<RectTransform>();
            rightBarRectTransform.localScale = Vector3.one;
            rightBarRectTransform.anchorMin = new Vector2(1f, 0.5f);
            rightBarRectTransform.anchorMax = new Vector2(1f, 0.5f);
            rightBarRectTransform.pivot = new Vector2(0f, 0.5f);
            rightBarRectTransform.sizeDelta = new Vector2(30, 6);
            rightBarRectTransform.anchoredPosition = new Vector2(-52, 0);
            rightBarRectTransform.localPosition = new Vector3(-2, 0, 0);
            animator.RightBar = rightBarRectTransform;

            drawer.UpdateContainer();
            drawer.UpdateArrowContainer();

            drawer.Container.Canvas.enabled = true;
            drawer.Container.GraphicRaycaster.enabled = true;
            
            drawer.Overlay.Canvas.enabled = true;
            drawer.Overlay.GraphicRaycaster.enabled = true;

            Selection.activeObject = go; //select the newly created gameObject
        }
#endif

        #endregion

        #region Constants

        /// <summary> If the drawer has been dragged beyond this point from Cosed state towards Opened state, then Open it. Interval 0 to 1 (0 = 0% and 1 = 100%). Default value: 0.5f (50%) </summary>
        private const float AUTO_OPEN_IF_DRAGGED_OVER_VISIBILITY_PERCENT = 0.5f;

        /// <summary> If the drawer has been dragged beyond this point from Opened state towards Closed state, then Close it. Interval 0 to 1 (0 = 0% and 1 = 100%). Default value: 0.5f (50%) </summary>
        private const float AUTO_CLOSE_IF_DRAGGED_UNDER_VISIBILITY_PERCENT = 0.5f;

        /// <summary> How fast does the swipe speed has to be in order to determine an auto Open or Close of the drawer. This constant is used to determine the outcome of fast swipes on the screen </summary>
        private const float AUTO_OPEN_OR_CLOSE_TERMINAL_SWIPE_VELOCITY = 800f;

        #endregion

        #region Static Properties

        /// <summary> Returns TRUE if at least one UIDrawer is opened </summary>
        public static bool AnyDrawerOpened { get { return OpenedDrawer != null; } }

        /// <summary> Default UIDrawer drawer category name  </summary>
        public static string DefaultDrawerCategory { get { return NamesDatabase.GENERAL; } }

        /// <summary> Default UIDrawer drawer name </summary>
        public static string DefaultDrawerName { get { return NamesDatabase.UNNAMED; } }

        /// <summary> Returns the UIDrawer that is currently being dragged </summary>
        public static UIDrawer DraggedDrawer { get; private set; }

        /// <summary> Returns the currently opened UIDrawer. There can be only one UIDrawer opened at a time. </summary>
        public static UIDrawer OpenedDrawer { get; private set; }

        /// <summary> Action invoked whenever an UIDrawerBehavior is triggered </summary>
        public static Action<UIDrawer, UIDrawerBehaviorType> OnUIDrawerBehavior = delegate { };

        /// <summary> Direct reference to the TouchDetector </summary>
        private static TouchDetector Detector { get { return TouchDetector.Instance; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if this UIDrawer has a valid UIDrawerArrow Arrow and it is enabled </summary>
        public bool ArrowEnabled { get { return HasArrow && Arrow.Enabled; } }

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>() ?? gameObject.AddComponent<Canvas>();
                return m_canvas;
            }
        }

        /// <summary> Returns TRUE if this UIDrawerContainer Container is NOT visible (is NOT in view) </summary>
        public bool Closed { get { return Visibility == VisibilityState.NotVisible; } }

        /// <summary> Returns TRUE if this UIDrawer has a valid UIDrawerArrow Arrow </summary>
        public bool HasArrow { get { return Arrow != null && Arrow.Animator != null && Arrow.Container != null; } }

        /// <summary> Returns TRUE if this UIDrawer has a valid UIDrawerContainer Container </summary>
        public bool HasContainer { get { return Container != null && Container.RectTransform != null; } }

        /// <summary> Returns TRUE if this UIDrawer has a valid UIContainer Overlay </summary>
        public bool HasOverlay { get { return Overlay != null && Overlay.RectTransform != null; } }

        /// <summary> Returns the inverse of the current VisibilityProgress value (float between 1 and 0) (Closed - Opened) </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public float InverseVisibility { get { return 1 - VisibilityProgress; } }

        /// <summary> Returns TRUE if this UIDrawerContainer Container is going out of view </summary>
        public bool IsClosing { get { return Visibility == VisibilityState.Hiding; } }

        /// <summary> Returns true if this UIDrawerContainer Container is currently begin dragged </summary>
        public bool IsDragged { get; private set; }

        /// <summary> Returns TRUE if this UIDrawerContainer Container is becoming visible </summary>
        public bool IsOpening { get { return Visibility == VisibilityState.Showing; } }

        /// <summary> Returns TRUE if the UIDrawerContainer Container is visible (is in view) </summary>
        public bool Opened { get { return Visibility == VisibilityState.Visible; } }

        /// <summary> The current visibility state  (Visible, NotVisible, Hiding or Showing) of this UIDrawer UIDrawerContainer Container </summary>
        public VisibilityState Visibility { get { return m_visibility; } }

        /// <summary> The visibility value of this UIDrawerContainer Container (float between 0 and 1) (Closed - Opened) </summary>
        public float VisibilityProgress
        {
            get { return m_visibilityProgress; }
            private set
            {
                m_visibilityProgress = Mathf.Clamp01(value);
                UpdateOverlayAlpha(VisibilityProgress);
                UpdateContainerAlpha(VisibilityProgress);
                UpdateArrowActiveState();
                DebugOpenProgress();
                if (Progressor != null) Progressor.SetProgress(VisibilityProgress);
                OnProgressChanged.Invoke(VisibilityProgress);
                OnInverseProgressChanged.Invoke(InverseVisibility);
            }
        }

        #endregion

        #region Public Variables

        /// <summary> Settings for the animated arrow used to show the open and close directions for this UIDrawer </summary>
        public UIDrawerArrow Arrow;

        /// <summary> If TRUE, the 'Back' button event will be blocked by this UIDrawer is visible (default: TRUE) </summary>
        public bool BlockBackButton = true;
        
        /// <summary> Behavior when this UIDrawer gets hidden (goes off screen) </summary>
        public UIDrawerBehavior CloseBehavior;

        /// <summary> UIDrawer position when closed (this also affects what gesture shows/hides the drawer) </summary>
        public SimpleSwipe CloseDirection;

        /// <summary> UIDrawer hide animation speed </summary>
        public float CloseSpeed;

        /// <summary> UIDrawer container settings. This is the actual drawer that gets animated </summary>
        public UIDrawerContainer Container;

        /// <summary> Custom anchored position that this UIDrawer slides from/to when showing/hiding. You can use this in code to customize on the fly this position </summary>
        public Vector3 CustomStartAnchoredPosition;

        // ReSharper disable once NotAccessedField.Global
        /// <summary> Internal variable used by the custom inspector to allow you to type a custom name instead of selecting it from the database </summary>
        public bool CustomDrawerName;

        /// <summary> UIDrawer drawer name </summary>
        public string DrawerName;

        /// <summary>
        ///     Enables/Disables the gesture detectors of this UIDrawer.
        ///     If FALSE, this drawer will no longer react to gestures.
        ///     Useful if you plan on showing/hiding the drawer via a button or a script
        /// </summary>
        public bool DetectGestures;

        /// <summary> Behavior when this UIDrawer is being dragged </summary>
        public UIDrawerBehavior DragBehavior;

        /// <summary> If TRUE, the next 'Back' button event will hide (close) this UIDrawer (default: TRUE) </summary>
        public bool HideOnBackButton = true;
        
        /// <summary>
        ///     Callback executed when the drawer is animating (opening or closing) and its progress has been updated.
        ///     <para />
        ///     Passes the Progress (float between 0 - when the drawer is closed and 1 - when the drawer is opened)
        /// </summary>
        public ProgressEvent OnProgressChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the drawer is animating (opening or closing) and its progress has been updated.
        ///     <para />
        ///     Passes the InverseProgress (float between 1 - when the drawer is closed and 0 - when the drawer is opened)
        ///     <para />
        ///     InverseProgress = 1 - Progress
        /// </summary>
        public ProgressEvent OnInverseProgressChanged = new ProgressEvent();

        /// <summary> Behavior when the UIDrawer gets shown (becomes visible on screen) </summary>
        public UIDrawerBehavior OpenBehavior;

        /// <summary> UIDrawer show animation speed </summary>
        public float OpenSpeed;

        /// <summary> Settings for the Overlay UIContainer used to dim the screen when the this UIDrawer is shown </summary>
        public UIContainer Overlay;

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) in order to show the current open progress value of this drawer in a visual manner </summary>
        public Progressor Progressor;

        /// <summary> If TRUE, this UIDrawer will slide from and to a set custom position. Default is set to true. </summary>
        public bool UseCustomStartAnchoredPosition = true;

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUIDrawer; } }

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the Canvas attached to this GameObject </summary>
        private Canvas m_canvas;

        /// <summary> Internal variable that keeps track of this UIDrawer's Container visibility state (Visible, NotVisible, Hiding or Showing) </summary>
        private VisibilityState m_visibility = VisibilityState.Visible;

        /// <summary> Internal variable that keeps track of the open state percentage of this UIDrawer's Container (how much of the container is visible on screen). (0 - Closed and 1 - Open) </summary>
        private float m_visibilityProgress = 1f;

        /// <summary> Internal variable used to save the scaled canvas size </summary>
        private Vector2 m_scaledCanvas;

        /// <summary> Internal variable that is used to validate a drag. This variable is needed in order to filter out drags that happen on other UI components </summary>
        private bool m_availableForDrag;

        /// <summary> Internal variable used to save the start position from where the drag started </summary>
        private Vector2 m_dragStartPosition;

        #endregion

        #region Unity Methods

        private const string GIZMOS_TEXTURE_PATH = "Doozy/UI/UIDrawer/";
        private const bool GIZMOS_ALLOW_SCALING = true;
        private const string ARROW_ROOT = "ArrowRoot";
        private const string ARROW_LEFT = "ArrowLeft";
        private const string ARROW_RIGHT = "ArrowRight";
        private const string ARROW_UP = "ArrowUp";
        private const string ARROW_DOWN = "ArrowDown";

        private void OnDrawGizmosSelected()
        {
            if (!ArrowEnabled || CloseDirection == SimpleSwipe.None) return;

            switch (CloseDirection)
            {
                case SimpleSwipe.Left:
                    Gizmos.DrawIcon(Arrow.Left.Root.transform.position, GIZMOS_TEXTURE_PATH + ARROW_ROOT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Left.Closed.transform.position, GIZMOS_TEXTURE_PATH + ARROW_RIGHT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Left.Opened.transform.position, GIZMOS_TEXTURE_PATH + ARROW_LEFT, GIZMOS_ALLOW_SCALING);
                    break;
                case SimpleSwipe.Right:
                    Gizmos.DrawIcon(Arrow.Right.Root.transform.position, GIZMOS_TEXTURE_PATH + ARROW_ROOT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Right.Closed.transform.position, GIZMOS_TEXTURE_PATH + ARROW_LEFT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Right.Opened.transform.position, GIZMOS_TEXTURE_PATH + ARROW_RIGHT, GIZMOS_ALLOW_SCALING);
                    break;
                case SimpleSwipe.Up:
                    Gizmos.DrawIcon(Arrow.Up.Root.transform.position, GIZMOS_TEXTURE_PATH + ARROW_ROOT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Up.Closed.transform.position, GIZMOS_TEXTURE_PATH + ARROW_DOWN, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Up.Opened.transform.position, GIZMOS_TEXTURE_PATH + ARROW_UP, GIZMOS_ALLOW_SCALING);
                    break;
                case SimpleSwipe.Down:
                    Gizmos.DrawIcon(Arrow.Down.Root.transform.position, GIZMOS_TEXTURE_PATH + ARROW_ROOT, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Down.Closed.transform.position, GIZMOS_TEXTURE_PATH + ARROW_UP, GIZMOS_ALLOW_SCALING);
                    Gizmos.DrawIcon(Arrow.Down.Opened.transform.position, GIZMOS_TEXTURE_PATH + ARROW_DOWN, GIZMOS_ALLOW_SCALING);
                    break;
            }
        }

        protected override void Reset()
        {
            base.Reset();

            UIDrawerSettings.Instance.ResetComponent(this);

            OpenBehavior = new UIDrawerBehavior(UIDrawerBehaviorType.Open);
            CloseBehavior = new UIDrawerBehavior(UIDrawerBehaviorType.Close);
            DragBehavior = new UIDrawerBehavior(UIDrawerBehaviorType.Drag);

            if (Container == null) Container = new UIDrawerContainer();
            if (Overlay == null) Overlay = new UIContainer();
            if (Arrow == null) Arrow = new UIDrawerArrow();

            if (Arrow.Container != null) Arrow.Container.gameObject.SetActive(false);
        }

        public override void Awake()
        {
            base.Awake();

            m_canvas = Canvas;

            if (!HasContainer)
            {
                DDebug.Log(
                           "The '" + DrawerName + "' drawer does not have a container referenced. " +
                           "This is the main drawer component and should not be missing. " +
                           "Either reference it or delete this gameObject. " +
                           "For this session, this gameObject has been disabled. " +
                           "(HINT: You can create a new UIDrawer to see how the container should be referenced)",
                           gameObject);

                gameObject.SetActive(false);
                return;
            }

            StartPosition = UseCustomStartAnchoredPosition ? CustomStartAnchoredPosition : RectTransform.anchoredPosition3D;
            RectTransform.anchoredPosition3D = StartPosition;

            Container.DisableGraphicRaycaster = Container.DisableCanvas;
            Container.Init();
            Overlay.Init();

            if (!Container.DisableCanvas) Container.Canvas.enabled = true;
            if (!Container.DisableGraphicRaycaster) Container.GraphicRaycaster.enabled = true;

            UpdateContainerSize();
            InitContainerPositions();
            InitArrow();

            Close(true);

            m_availableForDrag = true;
            IsDragged = false;

            if (DoozySettings.Instance.UseOrientationDetector)
            {
                OrientationDetector.Instance.OnOrientationEvent.AddListener(OnOrientationChanged);
            }
        }


        public override void OnEnable()
        {
            base.OnEnable();
            Container.DisableGraphicRaycaster = Container.DisableCanvas;
            if (DoozySettings.Instance.UseOrientationDetector)
                OrientationDetector.Instance.OnOrientationEvent.AddListener(OnOrientationChanged);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (DoozySettings.Instance.UseOrientationDetector)
            {
                if (OrientationDetector.ApplicationIsQuitting) return;
                OrientationDetector.Instance.OnOrientationEvent.RemoveListener(OnOrientationChanged);
            }
        }

        private void Update()
        {
            if (IsOpening || IsClosing) UpdateShowProgress(); //if the drawer is opening or closing the alpha value needs to be updated
            if (Closed) m_availableForDrag = true;

            UpdateArrow();
            UpdateContainerVelocity();
            UpdateContainerAnimation();

            if (!DetectGestures) return;           //this UIDrawer's gesture detectors are disabled -> return
            if (!Detector.TouchInProgress) return; //No touch is active -> return

            if (Detector.CurrentTouchInfo.IsDragging) //is the TouchDetector dragging anything?
            {
                if (Detector.CurrentTouchInfo.DraggedObject != gameObject) return; //this is NOT the dragged object -> return

                UpdateContainerDraggedPosition();
                UpdateShowProgress(); //update its show progress value (how much of the drawer is shown on the screen)

                if (Detector.CurrentTouchInfo.Touch.phase != TouchPhase.Ended && Detector.CurrentTouchInfo.Touch.phase != TouchPhase.Canceled) return;
                DragBehavior.OnFinished.Invoke(gameObject);

                if (Opened) //is this drawer Opened?
                {
                    if (VisibilityProgress < AUTO_CLOSE_IF_DRAGGED_UNDER_VISIBILITY_PERCENT                                      //if the drawer was dragged a distance long enough -> close it
                        || Mathf.Abs(Detector.CurrentTouchInfo.Velocity.magnitude) > AUTO_OPEN_OR_CLOSE_TERMINAL_SWIPE_VELOCITY) //if the swipe speed (velocity) was greater than the set TERMINAL_SWIPE_VELOCITY -> close it
                        Close();
                    else //otherwise -> open the drawer back
                        Open();
                }
                else if (Closed) //is this drawer Closed?
                {
                    if (VisibilityProgress > AUTO_OPEN_IF_DRAGGED_OVER_VISIBILITY_PERCENT                                        //if the drawer was dragged a distance long enough -> open it
                        || Mathf.Abs(Detector.CurrentTouchInfo.Velocity.magnitude) > AUTO_OPEN_OR_CLOSE_TERMINAL_SWIPE_VELOCITY) //if the swipe speed (velocity) was greater than the set TERMINAL_SWIPE_VELOCITY -> open it
                        Open();
                    else //otherwise -> close the drawer back
                        Close();
                }

                DraggedDrawer = null; //clear the DraggedDrawer slot
                IsDragged = false;
                Detector.SetDraggedObject(null); //the touch ended (or was canceled) -> tell the TouchDetector that it is not dragging anything at the moment

                return;
            }

            if (UnityEventSystem.currentSelectedGameObject != null) //if the touch is over an interactable UI component -> return
                return;

            if (!m_availableForDrag) //if this drawer cannot be dragged -> return
                return;

            if (OpenedDrawer == this) //is this the opened drawer
            {
                //check the the used executed the correct swipe (in the expected direction) to start closing this drawer
                if (!Opened || TouchDetector.GetSimpleSwipe(Detector.CurrentTouchInfo.Direction) != CloseDirection) return;
                DraggedDrawer = this; //set this as the DraggedDrawer
                IsDragged = true;
                Detector.SetDraggedObject(gameObject);                                               //set this as the dragged object
                m_dragStartPosition = ScaledTouchPosition(Detector.CurrentTouchInfo.Touch.position); //save the start position of the drag
                DragBehavior.OnStart.Invoke(gameObject);
                NotifySystemOfTriggeredBehavior(UIDrawerBehaviorType.Drag);
            }
            else if (!AnyDrawerOpened) //if no drawer is opened
            {
                //make sure that this drawer is closed and listening for the correct swipe
                //check the the user executed the correct swipe (in the expected direction) to start opening this drawer
                if (!Closed || TouchDetector.GetSimpleSwipe(Detector.CurrentTouchInfo.Direction, true) != CloseDirection) return;
                DraggedDrawer = this; //set this as the DraggedDrawer
                IsDragged = true;
                Detector.SetDraggedObject(gameObject);                                               //set this as the dragged object in the TouchDetector
                m_dragStartPosition = ScaledTouchPosition(Detector.CurrentTouchInfo.Touch.position); //save the start position of the drag

                Container.Enable();
                if (HasOverlay) Overlay.Enable();

                DragBehavior.OnStart.Invoke(gameObject);
            }
        }

        public void OnDrag(PointerEventData eventData) { m_availableForDrag = true; }

        public void OnBeginDrag(PointerEventData eventData) { m_availableForDrag = true; }

        public void OnEndDrag(PointerEventData eventData) { m_availableForDrag = false; }

        private void OnRectTransformDimensionsChange() { OnOrientationChanged(DetectedOrientation.Unknown); }

        #endregion

        #region Public Methods

        /// <summary> Close the drawer </summary>
        /// <param name="instantAction"> Determines if the animation should happen instantly (in zero seconds) </param>
        public void Close(bool instantAction = false)
        {
            if (Closed && !IsDragged) return;
            if (DebugComponent) DDebug.Log("'" + DrawerName + "' - Closed " + (instantAction ? "in zero seconds!" : "with animation."), gameObject);
            InitiateClose();   //prepare the drawer to be closed
            if (instantAction) //is it an instant action (skip the animation) ?
            {
                FinalizeClose(); //finalize the drawer settings for the closed position
                return;
            }

            m_visibility = VisibilityState.Hiding; //drawer needs to be animated -> activate the animation in the Update method
            CloseBehavior.OnStart.Invoke(gameObject);
            NotifySystemOfTriggeredBehavior(UIDrawerBehaviorType.Close);
        }

        /// <summary> Disable gesture detection for this UIDrawer </summary>
        public void DisableGestureDetection() { DetectGestures = false; }

        /// <summary> Enable gesture detection for this UIDrawer </summary>
        public void EnableGestureDetection() { DetectGestures = true; }

        /// <summary> Send an UIDrawerMessage notifying the system that an UIDrawerBehavior has been triggered </summary>
        /// <param name="behaviorType"> The UIDrawerBehaviorType of the UIDrawerBehavior that has been triggered </param>
        public void NotifySystemOfTriggeredBehavior(UIDrawerBehaviorType behaviorType)
        {
            if (OnUIDrawerBehavior != null) OnUIDrawerBehavior.Invoke(this, behaviorType);
            Message.Send(new UIDrawerMessage(this, behaviorType));
        }

        /// <summary> Open the drawer </summary>
        /// <param name="instantAction"> Determines if the animation should happen instantly (in zero seconds) </param>
        public void Open(bool instantAction = false)
        {
            if (Opened && !IsDragged) return;
            if (DebugComponent) DDebug.Log("'" + DrawerName + "' - Opened " + (instantAction ? "in zero seconds!" : "with animation."), gameObject);
            if (AnyDrawerOpened && !Opened) OpenedDrawer.Close(true);
            InitiateOpen();    //prepare the drawer to be opened
            if (instantAction) //is it an instant action (skip the animation) ?
            {
                FinalizeOpen(); //finalize the drawer settings for the opened position
                return;
            }

            m_visibility = VisibilityState.Showing; //drawer needs to be animated -> activate the animation in the Update method
            OpenBehavior.OnStart.Invoke(gameObject);
            NotifySystemOfTriggeredBehavior(UIDrawerBehaviorType.Open);
        }

        /// <summary> Toggle the drawer's opened/closed state. If it's open it will close and vice versa </summary>
        /// <param name="instantAction">If set to <c>true</c> it will close or open the drawer in zero seconds.</param>
        public void Toggle(bool instantAction = false)
        {
            switch (Visibility)
            {
                case VisibilityState.Visible:
                case VisibilityState.Showing:
                    Close(instantAction);
                    break;
                case VisibilityState.NotVisible:
                case VisibilityState.Hiding:
                    Open(instantAction);
                    break;
            }
        }

        /// <summary> Toggle gesture detection for this UIDrawer </summary>
        public void ToggleGestureDetection() { DetectGestures = !DetectGestures; }

        /// <summary> Update the UIDrawerArrow Arrow by copying the UIDrawerContainer Container RectTransform properties to it </summary>
        public void UpdateArrowContainer() { Arrow.Container.Copy(Container.RectTransform); }

        /// <summary> Update the UIDrawerContainer Container to the set settings. This method is mostly used by the editor button to help setup and design the container </summary>
        public void UpdateContainer()
        {
            RectTransform.anchoredPosition3D = StartPosition;
            UpdateContainerSize(Container.Size, Container.PercentageOfScreen, Container.MinimumSize, Container.FixedSize);
        }

        /// <summary> Update the size of the UIDrawerContainer Container </summary>
        public void UpdateContainerSize() { UpdateContainerSize(Container.Size, Container.PercentageOfScreen, Container.MinimumSize, Container.FixedSize); }

        /// <summary> Update the size of the UIDrawerContainer Container to FixedSize, with the given percentageOfScreen and minimumSize </summary>
        /// <param name="fixedSize">Fixed Size of the container.</param>
        public void UpdateContainerSize(float fixedSize) { UpdateContainerSize(UIDrawerContainerSize.FixedSize, Container.PercentageOfScreen, Container.MinimumSize, fixedSize); }

        /// <summary> Update the size of the UIDrawerContainer Container to PercentageOfScreen, with the given percentageOfScreen and minimumSize </summary>
        /// <param name="percentageOfScreen">The percentage of screen.</param>
        /// <param name="minimumSize">The minimum size.</param>
        public void UpdateContainerSize(float percentageOfScreen, float minimumSize) { UpdateContainerSize(UIDrawerContainerSize.PercentageOfScreen, percentageOfScreen, minimumSize, Container.FixedSize); }

        /// <summary> Update the drawer's close direction </summary>
        /// <param name="hideDirection">This is the direction the drawer will slide to in order to close.</param>
        public void UpdateDrawerCloseDirection(SimpleSwipe hideDirection)
        {
            if (!AnyDrawerOpened) OpenedDrawer.Close(true);

            Open(true);
            CloseDirection = hideDirection;
            InitContainerPositions();
            InitArrow();
            Close(true);
        }

        #endregion

        #region Private Methods

        /// <summary> Initiates the open setup </summary>
        private void InitiateOpen()
        {
            OpenedDrawer = this;        //set this as the opened drawer
            m_availableForDrag = false; //set this drawer as not draggable (we need to validate the drag)
            Container.Enable();
            if (HasOverlay) Overlay.Enable();
        }

        /// <summary> Finalizes the open settings </summary>
        private void FinalizeOpen()
        {
            Container.RectTransform.anchoredPosition3D = Container.OpenedPosition; //snap to the opened position
            m_visibility = VisibilityState.Visible;                                //set the drawer state to Opened
            VisibilityProgress = 1;                                                //set the show progress to 1 (just in case)
            if (HasOverlay) Overlay.CanvasGroup.alpha = VisibilityProgress;        //set the overlay alpha to 1 because the drawer is opened and the overlay should be visible 
            OpenBehavior.OnFinished.Invoke(gameObject);
        }

        /// <summary> Initiates the close setup </summary>
        private void InitiateClose() { }

        /// <summary> Finalizes the close settings </summary>
        private void FinalizeClose()
        {
            OpenedDrawer = null;                                                   //clear the opened drawer slot (make way for another one)
            m_availableForDrag = true;                                             //set this drawer as draggable
            Container.RectTransform.anchoredPosition3D = Container.ClosedPosition; //snap to the closed position
            m_visibility = VisibilityState.NotVisible;                             //set the drawer state to Closed
            VisibilityProgress = 0;                                                //set the visibility to o (just in case)
            Container.Disable();

            if (HasOverlay) //check if the drawer has an overlay referenced
            {
                Overlay.CanvasGroup.alpha = VisibilityProgress; //set the overlay alpha to 0 because the drawer is closed and the overlay should not be visible
                Overlay.Disable();
            }

            CloseBehavior.OnFinished.Invoke(gameObject);
        }

        // ReSharper disable once UnusedMember.Local
        private void MoveToCustomStartPosition() { RectTransform.anchoredPosition3D = StartPosition; }

        private void OnOrientationChanged(DetectedOrientation detectedOrientation)
        {
            if (!HasContainer) return; //if there is no container referenced this UIDrawer needs to get disabled; this line prevents any errors in the console (a debug log is already provided in Awake)

            UpdateContainer();

            InitContainerPositions(); //orientation just changed -> the drawer size and positions need to be recalculated

            if (Opened)
                Container.RectTransform.anchoredPosition3D = Container.OpenedPosition;
            else if (Closed) Container.RectTransform.anchoredPosition3D = Container.ClosedPosition;

            UpdateArrowContainer();
        }

        /// <summary> Initializes the container's Opened and Closed positions </summary>
        private void InitContainerPositions()
        {
            Container.OpenedPosition = StartPosition;
            Container.ClosedPosition = GetContainerClosedPosition();
            Container.CurrentPosition = Container.RectTransform.anchoredPosition3D;
            Container.PreviousPosition = Container.CurrentPosition;
        }

        /// <summary> Updates the size of the UIDrawerContainer Container. Do not use this method unless you know what you are doing. Use the helper methods instead </summary>
        private void UpdateContainerSize(UIDrawerContainerSize size, float percentageOfScreen, float minimumSize, float fixedSize)
        {
            Container.Size = size;
            Container.PercentageOfScreen = percentageOfScreen;
            Container.MinimumSize = minimumSize;
            Container.FixedSize = fixedSize;

            Container.RectTransform.Copy(RectTransform);
            Container.RectTransform.localScale = Vector3.one;

            if (Container.Size == UIDrawerContainerSize.FullScreen) //do nothing as the copy method above made it full screen already
            {
                Container.RectTransform.anchorMin = new Vector2(0, 0);
                Container.RectTransform.anchorMax = new Vector2(1, 1);
                Container.RectTransform.pivot = new Vector2(0.5f, 0.5f);
                Container.RectTransform.sizeDelta = new Vector2(0, 0);
                Container.RectTransform.anchoredPosition = new Vector2(0, 0);
                return;
            }

            Container.PercentageOfScreen = Mathf.Clamp(Container.PercentageOfScreen, 0, 1); //clamp to sane values
            Container.MinimumSize = Mathf.Abs(Container.MinimumSize);                       //this should not be negative
            Container.FixedSize = Mathf.Abs(Container.FixedSize);                           //this should not be negative

            var rect = RectTransform.rect;
            m_scaledCanvas = new Vector2(rect.width, rect.height);
            Container.CalculatedSize = m_scaledCanvas;

            switch (CloseDirection)
            {
                case SimpleSwipe.Left:
                    if (Container.Size == UIDrawerContainerSize.PercentageOfScreen)
                    {
                        Container.MinimumSize = Mathf.Clamp(Container.MinimumSize, 0, m_scaledCanvas.x);
                        Container.CalculatedSize.x *= Container.PercentageOfScreen;
                        Container.CalculatedSize.x = Mathf.Clamp(Container.CalculatedSize.x, Container.MinimumSize, m_scaledCanvas.x);
                    }
                    else if (Container.Size == UIDrawerContainerSize.FixedSize)
                    {
                        Container.CalculatedSize.x = Mathf.Clamp(Container.FixedSize, 0, m_scaledCanvas.x);
                    }

                    Container.RectTransform.anchorMin = new Vector2(0, 0);
                    Container.RectTransform.anchorMax = new Vector2(0, 1);
                    Container.RectTransform.pivot = new Vector2(0, 0.5f);
                    Container.RectTransform.sizeDelta = new Vector2(Container.CalculatedSize.x, 0);
                    break;
                case SimpleSwipe.Right:
                    if (Container.Size == UIDrawerContainerSize.PercentageOfScreen)
                    {
                        Container.MinimumSize = Mathf.Clamp(Container.MinimumSize, 0, m_scaledCanvas.x);
                        Container.CalculatedSize.x *= Container.PercentageOfScreen;
                        Container.CalculatedSize.x = Mathf.Clamp(Container.CalculatedSize.x, Container.MinimumSize, m_scaledCanvas.x);
                    }
                    else if (Container.Size == UIDrawerContainerSize.FixedSize)
                    {
                        Container.CalculatedSize.x = Mathf.Clamp(Container.FixedSize, 0, m_scaledCanvas.x);
                    }

                    Container.RectTransform.anchorMin = new Vector2(1, 0);
                    Container.RectTransform.anchorMax = new Vector2(1, 1);
                    Container.RectTransform.pivot = new Vector2(1, 0.5f);
                    Container.RectTransform.sizeDelta = new Vector2(Container.CalculatedSize.x, 0);
                    break;
                case SimpleSwipe.Up:
                    if (Container.Size == UIDrawerContainerSize.PercentageOfScreen)
                    {
                        Container.MinimumSize = Mathf.Clamp(Container.MinimumSize, 0, m_scaledCanvas.y);
                        Container.CalculatedSize.y *= Container.PercentageOfScreen;
                        Container.CalculatedSize.y = Mathf.Clamp(Container.CalculatedSize.y, Container.MinimumSize, m_scaledCanvas.y);
                    }
                    else if (Container.Size == UIDrawerContainerSize.FixedSize)
                    {
                        Container.CalculatedSize.y = Mathf.Clamp(Container.FixedSize, 0, m_scaledCanvas.y);
                    }

                    Container.RectTransform.anchorMin = new Vector2(0, 1);
                    Container.RectTransform.anchorMax = new Vector2(1, 1);
                    Container.RectTransform.pivot = new Vector2(0.5f, 1);
                    Container.RectTransform.sizeDelta = new Vector2(0, Container.CalculatedSize.y);
                    break;
                case SimpleSwipe.Down:
                    if (Container.Size == UIDrawerContainerSize.PercentageOfScreen)
                    {
                        Container.MinimumSize = Mathf.Clamp(Container.MinimumSize, 0, m_scaledCanvas.y);
                        Container.CalculatedSize.y *= Container.PercentageOfScreen;
                        Container.CalculatedSize.y = Mathf.Clamp(Container.CalculatedSize.y, Container.MinimumSize, m_scaledCanvas.y);
                    }
                    else if (Container.Size == UIDrawerContainerSize.FixedSize)
                    {
                        Container.CalculatedSize.y = Mathf.Clamp(Container.FixedSize, 0, m_scaledCanvas.y);
                    }

                    Container.RectTransform.anchorMin = new Vector2(0, 0);
                    Container.RectTransform.anchorMax = new Vector2(1, 0);
                    Container.RectTransform.pivot = new Vector2(0.5f, 0);
                    Container.RectTransform.sizeDelta = new Vector2(0, Container.CalculatedSize.y);
                    break;
            }

            Container.RectTransform.anchoredPosition3D = Vector3.zero;
        }

        /// <summary>
        ///     Gets the container closed position.
        /// </summary>
        private Vector3 GetContainerClosedPosition()
        {
            Rect rect = Container.RectTransform.rect;
            float xOffset = rect.width;
            float yOffset = rect.height;

            switch (CloseDirection)
            {
                case SimpleSwipe.Left:  return new Vector3(Container.OpenedPosition.x - xOffset, Container.OpenedPosition.y, Container.OpenedPosition.z);
                case SimpleSwipe.Right: return new Vector3(Container.OpenedPosition.x + xOffset, Container.OpenedPosition.y, Container.OpenedPosition.z);
                case SimpleSwipe.Up:    return new Vector3(Container.OpenedPosition.x, Container.OpenedPosition.y + yOffset, Container.OpenedPosition.z);
                case SimpleSwipe.Down:  return new Vector3(Container.OpenedPosition.x, Container.OpenedPosition.y - yOffset, Container.OpenedPosition.z);
                default:                return Vector3.zero;
            }
        }

        private void UpdateContainerAnimation()
        {
            if (IsOpening) //if the drawer is opening -> execute the show tween
            {
                Container.RectTransform.anchoredPosition3D = Vector3.LerpUnclamped(Container.RectTransform.anchoredPosition3D, Container.OpenedPosition, Time.unscaledDeltaTime * OpenSpeed); //open tween
                UpdateShowProgress();
                if (VisibilityProgress >= 0.995) FinalizeOpen();
            }
            else if (IsClosing) //if the drawer is closing -> execute the hide tween
            {
                Container.RectTransform.anchoredPosition3D = Vector3.LerpUnclamped(Container.RectTransform.anchoredPosition3D, Container.ClosedPosition, Time.unscaledDeltaTime * CloseSpeed); //close tween
                UpdateShowProgress();
                if (VisibilityProgress <= 0.005) FinalizeClose();
            }
        }

        private void UpdateContainerVelocity()
        {
            if (Container.PreviousPosition != Container.CurrentPosition) UpdateShowProgress();
            Container.PreviousPosition = Container.CurrentPosition;                 //update the container's previous position; this is used to calculate the ContainerVelocity (its speed)
            Container.CurrentPosition = Container.RectTransform.anchoredPosition3D; //update the container's current position; this is used to calculate the ContainerVelocity (its speed)
        }

        private void UpdateContainerDraggedPosition()
        {
            switch (CloseDirection)
            {
                //does this drawer close to the Left or the Right of the screen?
                case SimpleSwipe.Left:
                case SimpleSwipe.Right:
                {
                    if (Opened) //is the drawer opened?
                    {
                        Vector3 anchoredPosition3D;
                        anchoredPosition3D = new Vector3(Container.OpenedPosition.x + ScaledPositionX(Detector.CurrentTouchInfo.Touch.position.x) - m_dragStartPosition.x, //calculate the X 'dragged' position
                                                         (anchoredPosition3D = Container.RectTransform.anchoredPosition3D).y,
                                                         anchoredPosition3D.z);
                        Container.RectTransform.anchoredPosition3D = anchoredPosition3D;
                    }
                    else if (Closed) //is the drawer closed?
                    {
                        Vector3 anchoredPosition3D;
                        anchoredPosition3D = new Vector3(Container.ClosedPosition.x + ScaledPositionX(Detector.CurrentTouchInfo.Touch.position.x) - m_dragStartPosition.x, //calculate the X 'dragged' position
                                                         (anchoredPosition3D = Container.RectTransform.anchoredPosition3D).y,
                                                         anchoredPosition3D.z);
                        Container.RectTransform.anchoredPosition3D = anchoredPosition3D;
                    }

                    if (CloseDirection == SimpleSwipe.Left) //does this drawer close to the Left -> clamp its position accordingly
                    {
                        Vector3 anchoredPosition3D;
                        anchoredPosition3D = new Vector3(Mathf.Clamp(Container.RectTransform.anchoredPosition3D.x, Container.ClosedPosition.x, Container.OpenedPosition.x), //clamp X position
                                                         (anchoredPosition3D = Container.RectTransform.anchoredPosition3D).y,
                                                         anchoredPosition3D.z);
                        Container.RectTransform.anchoredPosition3D = anchoredPosition3D;
                    }
                    else //this drawer closes to the Right -> clamp its position accordingly
                    {
                        Vector3 anchoredPosition3D;
                        anchoredPosition3D = new Vector3(Mathf.Clamp(Container.RectTransform.anchoredPosition3D.x, Container.OpenedPosition.x, Container.ClosedPosition.x), //clamp X position
                                                         (anchoredPosition3D = Container.RectTransform.anchoredPosition3D).y,
                                                         anchoredPosition3D.z);
                        Container.RectTransform.anchoredPosition3D = anchoredPosition3D;
                    }

                    break;
                }
                //does this drawer close Up or Down
                case SimpleSwipe.Up:
                case SimpleSwipe.Down:
                {
                    if (Opened) //is the drawer opened?
                        Container.RectTransform.anchoredPosition3D = new Vector3(Container.RectTransform.anchoredPosition3D.x,
                                                                                 Container.OpenedPosition.y + ScaledPositionY(Detector.CurrentTouchInfo.Touch.position.y) - m_dragStartPosition.y, //calculate the Y 'dragged' position
                                                                                 Container.RectTransform.anchoredPosition3D.z);
                    else if (Closed) //is the drawer closed?
                        Container.RectTransform.anchoredPosition3D = new Vector3(Container.RectTransform.anchoredPosition3D.x,
                                                                                 Container.ClosedPosition.y + ScaledPositionY(Detector.CurrentTouchInfo.Touch.position.y) - m_dragStartPosition.y, //calculate the Y 'dragged' position
                                                                                 Container.RectTransform.anchoredPosition3D.z);

                    if (CloseDirection == SimpleSwipe.Up) //does this drawer close Up -> clamp its position accordingly
                    {
                        Vector3 anchoredPosition3D = Container.RectTransform.anchoredPosition3D;
                        Container.RectTransform.anchoredPosition3D = new Vector3(anchoredPosition3D.x,
                                                                                 Mathf.Clamp(anchoredPosition3D.y, Container.OpenedPosition.y, Container.ClosedPosition.y), //clamp Y position
                                                                                 Container.RectTransform.anchoredPosition3D.z);
                    }
                    else //this drawer closes Down -> clamp its position accordingly
                    {
                        Vector3 anchoredPosition3D = Container.RectTransform.anchoredPosition3D;
                        Container.RectTransform.anchoredPosition3D = new Vector3(anchoredPosition3D.x,
                                                                                 Mathf.Clamp(anchoredPosition3D.y, Container.ClosedPosition.y, Container.OpenedPosition.y), //clamp Y position
                                                                                 Container.RectTransform.anchoredPosition3D.z);
                    }

                    break;
                }
            }
        }

        private void UpdateShowProgress()
        {
            switch (CloseDirection) //(currentX - minX) / (maxX - minX)
            {
                case SimpleSwipe.Left:
                    VisibilityProgress = (Container.RectTransform.anchoredPosition3D.x - Container.ClosedPosition.x) / (Container.OpenedPosition.x - Container.ClosedPosition.x);
                    break;
                case SimpleSwipe.Right:
                    VisibilityProgress = (Container.RectTransform.anchoredPosition3D.x - Container.ClosedPosition.x) / (Container.OpenedPosition.x - Container.ClosedPosition.x);
                    break;
                case SimpleSwipe.Up:
                    VisibilityProgress = (Container.RectTransform.anchoredPosition3D.y - Container.ClosedPosition.y) / (Container.OpenedPosition.y - Container.ClosedPosition.y);
                    break;
                case SimpleSwipe.Down:
                    VisibilityProgress = (Container.RectTransform.anchoredPosition3D.y - Container.ClosedPosition.y) / (Container.OpenedPosition.y - Container.ClosedPosition.y);
                    break;
            }
        }

        /// <summary> Initializes the drawer's arrow </summary>
        private void InitArrow()
        {
            if (Arrow == null) return;
            if (!Arrow.Enabled) return;
            UpdateArrowContainer();
            Arrow.Animator.gameObject.SetActive(true);
            Arrow.Animator.SetTargetDrawer(this);
        }

        /// <summary>
        ///     Updates the arrow's settings
        /// </summary>
        private void UpdateArrow()
        {
            //is the arrow enabled -> sync the arrowContainer to the drawer's container position
            if (!Arrow.Enabled) return;
            UpdateArrowContainer();
            Arrow.Animator.UpdateArrowColor(this);
            Arrow.Animator.UpdateArrow();

            if (DraggedDrawer == null && OpenedDrawer == null)
            {
                Arrow.Animator.UpdateRotatorPosition(1);
                return;
            }

            if (DraggedDrawer != null)
            {
                if (DraggedDrawer != this) Arrow.Animator.UpdateRotatorPosition(1 - DraggedDrawer.VisibilityProgress);
                else Arrow.Animator.UpdateRotatorPosition(1 - VisibilityProgress);

                return;
            }

            if (OpenedDrawer != null)
            {
                if (OpenedDrawer != this) Arrow.Animator.UpdateRotatorPosition(1 - OpenedDrawer.VisibilityProgress);
                else Arrow.Animator.UpdateRotatorPosition(1 - VisibilityProgress);
            }
        }

        private void UpdateOverlayAlpha(float value)
        {
            if (!HasOverlay) return;
            Overlay.CanvasGroup.alpha = value;
        }

        private void UpdateContainerAlpha(float value)
        {
            if (!HasContainer || !Container.FadeOut) return;
            Container.CanvasGroup.alpha = value; //-> fade out the entire container by updating the alpha value
        }

        private void UpdateArrowActiveState() { Arrow.Container.gameObject.SetActive(ArrowEnabled); }

        /// <summary> Returns the scaled X value, taking into account the changes made by the CanvasScaler on the rootCanvas </summary>
        private float ScaledPositionX(float x) { return x / Canvas.pixelRect.width * RectTransform.rect.width; }

        /// <summary> Returns the scaled Y value, taking into account the changes made by the CanvasScaler on the rootCanvas </summary>
        private float ScaledPositionY(float y) { return y / Canvas.pixelRect.height * RectTransform.rect.height; }

        /// <summary> Returns the adjusted touch position, taking into account the changes made by the CanvasScaler on the rootCanvas </summary>
        private Vector2 ScaledTouchPosition(Vector2 touchPosition) { return new Vector2(ScaledPositionX(touchPosition.x), ScaledPositionY(touchPosition.y)); }

        private void DebugOpenProgress()
        {
            if (DebugComponent) DDebug.Log("[" + name + "] OpenProgress: " + VisibilityProgress, this);
        }

        #endregion

        #region Static Methods

        /// <summary> Close the UIDrawer, with the given drawer name, that exists in the UIDrawer Database </summary>
        /// <param name="drawerName"> Drawer name to search for </param>
        /// <param name="debug"> Enable relevant debug messages to be printed to the console </param>
        public static void Close(string drawerName, bool debug = false)
        {
            UIDrawer drawer = Get(drawerName);
            if (drawer == null)
            {
                if (debug) DDebug.LogError("Unable to close the '" + drawerName + "' drawer because no such UIDrawer was found in the Database.");
                return;
            }

            if (AnyDrawerOpened && OpenedDrawer == drawer) OpenedDrawer = null;
            drawer.Close();
        }

        /// <summary> Returns TRUE if an UIDrawer, with the given drawer name, has been found in the UIDrawer Database </summary>
        /// <param name="drawerName"> Drawer name to search for </param>
        public static bool Contains(string drawerName)
        {
            foreach (UIDrawer drawer in Database)
                if (drawer.DrawerName.Equals(drawerName))
                    return true;

            return false;
        }

        /// <summary> Get a reference to the first UIDrawer, with the given drawer name, found in the UIDrawer Database. Returns null if an UIDrawer is not found </summary>
        /// <param name="drawerName"> Drawer name to search for </param>
        public static UIDrawer Get(string drawerName)
        {
            foreach (UIDrawer drawer in Database)
                if (drawer.DrawerName.Equals(drawerName))
                    return drawer;

            return null;
        }

        /// <summary> Open the UIDrawer, with the given drawer name, that exists in the UIDrawer Database </summary>
        /// <param name="drawerName"> Drawer name to search for </param>
        /// <param name="debug"> Enable relevant debug messages to be printed to the console </param>
        public static void Open(string drawerName, bool debug = false)
        {
            UIDrawer drawer = Get(drawerName);
            if (drawer == null)
            {
                if (debug) DDebug.LogError("Unable to open the '" + drawerName + "' drawer because no such UIDrawer was found in the Database.");
                return;
            }

            if (AnyDrawerOpened && OpenedDrawer != drawer) OpenedDrawer.Close(true);
            drawer.Open();
        }

        /// <summary> Toggle the open/close state of an UIDrawer, with the given drawer name, that exists in the UIDrawer Database. If the drawer is opened it will close and vice versa </summary>
        /// <param name="drawerName"> Drawer name to search for </param>
        /// <param name="debug"> Enable relevant debug messages to be printed to the console </param>
        public static void Toggle(string drawerName, bool debug = false)
        {
            UIDrawer drawer = Get(drawerName);
            if (drawer == null)
            {
                if (debug) DDebug.LogError("Unable to toggle the '" + drawerName + "' drawer because no such UIDrawer was found in the Database.");
                return;
            }

            drawer.Toggle();
        }

        #endregion
    }
}