// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Extensions;
using Doozy.Engine.Layouts;
using Doozy.Engine.Orientation;
using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system.
    ///     It manages a container, that can contain any type of UI elements (buttons, toggles, images, texts...) and that can be animated in and out of view.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIView_AddComponentMenu_MenuName, MenuUtils.UIView_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIVIEW)]
    public class UIView : UIComponentBase<UIView>
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIView_MenuItem_ItemName, false, MenuUtils.UIView_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject(MenuUtils.UIView_GameObject_Name, typeof(RectTransform), typeof(UIView), typeof(Image));
            GameObjectUtility.SetParentAndAlign(go, GetCanvasAsParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            go.GetComponent<RectTransform>().FullScreen(true);
            go.GetComponent<Image>().color = DoozyUtils.BackgroundColor;
            Selection.activeObject = go; //select the newly created gameObject
        }
#endif

        #endregion

        #region Static Properties

        /// <summary> Default UIView view category name  </summary>
        public static string DefaultViewCategory { get { return NamesDatabase.GENERAL; } }

        /// <summary> Default UIView view name </summary>
        public static string DefaultViewName { get { return NamesDatabase.UNNAMED; } }

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        /// <summary> Action invoked whenever a UIViewBehavior is triggered </summary>
        public static Action<UIView, UIViewBehaviorType> OnUIViewAction = delegate { };

        // ReSharper disable once InconsistentNaming
        /// <summary> List of all the UIViews that are currently visible on screen </summary>
        public static readonly List<UIView> VisibleViews = new List<UIView>();

        /// <summary> Direct reference to the Orientation Detector Instance </summary>
        private static OrientationDetector OrientationDetector { get { return OrientationDetector.Instance; } }

        #endregion

        #region Properties        

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>();
                return m_canvas;
            }
        }

        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (m_canvasGroup == null) m_canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
                return m_canvasGroup;
            }
        }

        /// <summary>
        /// Returns the current start position (anchoredPosition3D) by checking the UseCustomStartAnchoredPosition value.
        /// <para/> Returns CustomStartAnchoredPosition, if UseCustomStartAnchoredPosition is TRUE.
        /// <para/> Returns StartPosition, if UseCustomStartAnchoredPosition is FALSE.
        /// </summary>
        public Vector3 CurrentStartPosition { get { return UseCustomStartAnchoredPosition ? CustomStartAnchoredPosition : StartPosition; } }

        /// <summary> Reference to the GraphicRaycaster component </summary>
        public GraphicRaycaster GraphicRaycaster
        {
            get
            {
                if (m_graphicRaycaster == null) m_graphicRaycaster = GetComponent<GraphicRaycaster>();
                return m_graphicRaycaster;
            }
        }

        /// <summary> Returns the inverse of the current VisibilityProgress value (float between 1 and 0) (NotVisible - Visible) </summary>
        public float InverseVisibility { get { return 1 - VisibilityProgress; } }

        /// <summary> Returns TRUE if this UIView is NOT visible (is NOT in view) </summary>
        public bool IsHidden { get { return Visibility == VisibilityState.NotVisible; } }

        /// <summary> Returns TRUE if this UIView is playing the Hide Animation to get out of view </summary>
        public bool IsHiding { get { return Visibility == VisibilityState.Hiding; } }

        /// <summary> Returns TRUE if this UIView is playing the Show Animation to become visible </summary>
        public bool IsShowing { get { return Visibility == VisibilityState.Showing; } }

        /// <summary> Returns TRUE if this UIView is visible (is in view) </summary>
        public bool IsVisible { get { return Visibility == VisibilityState.Visible; } }

        /// <summary> The current visibility state of this UIView (Visible, NotVisible, Hiding or Showing) </summary>
        public VisibilityState Visibility
        {
            get { return m_visibility; }
            set
            {
                m_visibility = value;
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (value)
                {
                    case VisibilityState.Visible:
                        VisibilityProgress = 1f;
                        break;
                    case VisibilityState.NotVisible:
                        VisibilityProgress = 0f;
                        break;
                }
            }
        }

        /// <summary> The visibility value of this UIView (float between 0 and 1) (NotVisible - Visible) </summary>
        public float VisibilityProgress
        {
            get { return m_visibilityProgress; }
            set
            {
                m_visibilityProgress = Mathf.Clamp01(value);

                switch (Visibility)
                {
                    case VisibilityState.Hiding:
                        if (HideProgressor != null) HideProgressor.SetProgress(InverseVisibility);
                        if (UpdateShowProgressorOnHide && ShowProgressor != null) ShowProgressor.SetProgress(VisibilityProgress);
                        break;
                    case VisibilityState.Showing:
                        if (ShowProgressor != null) ShowProgressor.SetProgress(VisibilityProgress);
                        if (UpdateHideProgressorOnShow && HideProgressor != null) HideProgressor.SetProgress(InverseVisibility);
                        break;
                    case VisibilityState.NotVisible:
                        if (HideProgressor != null) HideProgressor.SetProgress(InverseVisibility);
                        if (UpdateShowProgressorOnHide && ShowProgressor != null) ShowProgressor.SetProgress(VisibilityProgress);
                        break;
                    case VisibilityState.Visible:
                        if (ShowProgressor != null) ShowProgressor.SetProgress(VisibilityProgress);
                        if (UpdateHideProgressorOnShow && HideProgressor != null) HideProgressor.SetProgress(InverseVisibility);
                        break;
                }

                OnVisibilityChanged.Invoke(VisibilityProgress);
                OnInverseVisibilityChanged.Invoke(InverseVisibility);
            }
        }

        /// <summary> Returns TRUE if this UIView has at least one child UIView </summary> 
        private bool HasChildUIViews { get { return m_childUIViews != null && m_childUIViews.Length > 1; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUIView; } }

        #endregion

        #region Public Variables

        /// <summary> If TRUE, after this UIView gets shown, it will get automatically hidden after the AutoHideAfterShowDelay time interval has passed </summary>
        public bool AutoHideAfterShow;

        /// <summary> If AutoHideAfterShow is TRUE, this is the time interval after which this UIView will get automatically hidden </summary>
        public float AutoHideAfterShowDelay;

        /// <summary> If TRUE, after this UIView has been shown, the referenced SelectedButton GameObject will get automatically selected by EventSystem.current </summary>
        public bool AutoSelectButtonAfterShow;

        /// <summary> Determines the actions this UIView will automatically perform at Start (when it gets initialized) </summary>
        public UIViewStartBehavior BehaviorAtStart;

        /// <summary> A custom anchoredPosition3D that this UIView will have when it is shown. This is used to calculate the Show and Hide Move animations positions </summary>
        public Vector3 CustomStartAnchoredPosition;

        /// <summary> If TRUE, when this UIView is hidden, any button that is selected (EventSystem.current) will get deselected </summary>
        public bool DeselectAnyButtonSelectedOnHide;

        /// <summary> If TRUE, when this UIView is shown, any button that is selected (EventSystem.current) will get deselected </summary>
        public bool DeselectAnyButtonSelectedOnShow;

        /// <summary> If TRUE, when this UIView gets hidden, the Canvas component found on the same GameObject this UIView component is attached to, will be disabled </summary>
        public bool DisableCanvasWhenHidden;

        /// <summary> If TRUE, when this UIView gets hidden, the GameObject this UIView component is attached to, will be disabled </summary>
        public bool DisableGameObjectWhenHidden;

        /// <summary> If TRUE, when this UIView gets hidden, the GraphicRaycaster component found on the same GameObject this UIView component is attached to, will be disabled </summary>
        public bool DisableGraphicRaycasterWhenHidden;

        /// <summary> Behavior when this UIView gets hidden (goes off screen) </summary>
        public UIViewBehavior HideBehavior = new UIViewBehavior(AnimationType.Hide);

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) while hiding this view </summary>
        public Progressor HideProgressor;

        /// <summary> Loop animation started after this UIView gets shown and stopped before this UIView gets hidden </summary>
        public UIViewBehavior LoopBehavior = new UIViewBehavior(AnimationType.Loop);

        /// <summary>
        ///     Callback executed when the view is animating (showing or hiding) and the progress has been updated.
        ///     <para />
        ///     Passes the InverseVisibility (float between 1 - NotVisible and 0 - Visible)
        ///     <para />
        ///     InverseVisibility = 1 - Visibility
        /// </summary>
        public ProgressEvent OnInverseVisibilityChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the view is animating (showing or hiding) and the progress has been updated.
        ///     <para />
        ///     Passes the Visibility (float between 0 - NotVisible and 1 - Visible)
        /// </summary>
        public ProgressEvent OnVisibilityChanged = new ProgressEvent();

        /// <summary> Reference to the GameObject that should be selected after this UIView has been shown. Works only if AutoSelectButtonAfterShow is TRUE </summary>
        public GameObject SelectedButton;

        /// <summary> Behavior when this UIView gets shown (becomes visible on screen) </summary>
        public UIViewBehavior ShowBehavior = new UIViewBehavior(AnimationType.Show);

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) while showing this view </summary>
        public Progressor ShowProgressor;

        /// <summary> Determines on which orientation will this UIView be available for </summary>
        public TargetOrientation TargetOrientation;

        /// <summary> Should the HideProgressor get updated when this UIView is showing? </summary>
        public bool UpdateHideProgressorOnShow;

        /// <summary> Should the ShowProgressor get updated when this UIView is hiding? </summary>
        public bool UpdateShowProgressorOnHide;

        /// <summary> If TRUE, this UIView will 'snap' to the CustomStartAnchoredPosition in Awake (when the StartPosition is calculated) </summary>
        public bool UseCustomStartAnchoredPosition;

        /// <summary> The category this UIView's name belongs to </summary>
        public string ViewCategory;

        /// <summary> The name of this UIView </summary>
        public string ViewName;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the Canvas attached to this GameObject </summary>
        private Canvas m_canvas;

        /// <summary> Internal variable that holds a reference to the GraphicRaycaster attached to this GameObject </summary>
        private GraphicRaycaster m_graphicRaycaster;

        /// <summary> Internal variable that holds a reference to the CanvasGroup attached to this GameObject </summary>
        private CanvasGroup m_canvasGroup;

        /// <summary> Internal variable that keeps track of the visibility of this view (float between 0 and 1) (NotVisible - Visible) </summary>
        /// a
        private float m_visibilityProgress = 1f;

        /// <summary> Internal variable that keeps track of this UIView's visibility state (Visible, NotVisible, Hiding or Showing) </summary>
        private VisibilityState m_visibility = VisibilityState.Visible;

        /// <summary> Internal variable used to keep track of the coroutine used for when this UIView is shown </summary>
        private Coroutine m_showCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used for when this UIView is hidden </summary>
        private Coroutine m_hideCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used to auto hide this UIView after it was shown </summary>
        private Coroutine m_autoHideCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used to disable button clicks while this UIView is animating </summary>
        private Coroutine m_disableButtonClickCoroutine;

        /// <summary>
        ///     Internal array used to keep track of all the child UIButtons and mark them to update their start valued upon first interaction.
        ///     The update is done after this UIView has been shown
        /// </summary>
        private UIButton[] m_childUIButtons;

        /// <summary> Internal array used to keep tack of all the UIViews that are children of this one </summary>
        private UIView[] m_childUIViews;

        /// <summary> Internal variable used to suppress invoking events when the first Hide is executed </summary>
        private bool m_initialized;

        /// <summary> Internal variable used to keep a reference to the LayoutController that controls the RectTransform of this view (only if a LayoutGroup is used) </summary>
        private LayoutController m_layoutController;

        /// <summary> Internal variable used to keep track if this view has a LayoutController referenced (optimization needed to reduce the number of null checks) </summary>
        private bool m_hasLayoutController;

        /// <summary> Internal variable used to keep track if this view is directly under a LayoutGroup </summary>
        private bool m_controlledByLayoutGroup;

        #endregion

        #region Unity Methods

        protected override void Reset()
        {
            base.Reset();

            UIViewSettings.Instance.ResetComponent(this);

            ViewCategory = DefaultViewCategory;
            ViewName = DefaultViewName;

            SelectedButton = null;
            Visibility = VisibilityState.Visible;
        }

        public override void Awake()
        {
            if(UseCustomStartAnchoredPosition)
                MoveToCustomStartPosition();
            
            base.Awake();

            LoadPresets();
            m_initialized = false;

            Canvas.enabled = false;           //disable the canvas
            GraphicRaycaster.enabled = false; //disable the graphic raycaster
        }

        public override void Start()
        {
            base.Start();
            CheckForLayoutController();
            Initialize();
        }

        private void CheckForLayoutController()
        {
//            Debug.Log("CheckForLayoutController");
            if (!m_controlledByLayoutGroup)
            {
                m_controlledByLayoutGroup = GetComponentInParent<LayoutGroup>() != null;
                if (!m_controlledByLayoutGroup)
                {
                    m_hasLayoutController = false;
                    m_layoutController = null;
                }
                else
                {
                    m_layoutController = GetComponentInParent<LayoutController>();
                    m_hasLayoutController = m_layoutController != null;
                    if (!m_hasLayoutController) m_layoutController = transform.parent.gameObject.AddComponent<LayoutController>();
                }
            }
            else
            {
                m_hasLayoutController = m_layoutController != null;
                if (m_hasLayoutController) m_layoutController.Rebuild(true);
            }

            if (UseCustomStartAnchoredPosition && m_controlledByLayoutGroup)
                UseCustomStartAnchoredPosition = false;

            StartPosition = UseCustomStartAnchoredPosition ? CustomStartAnchoredPosition : RectTransform.anchoredPosition3D;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            m_childUIButtons = GetComponentsInChildren<UIButton>();
            m_childUIViews = GetComponentsInChildren<UIView>();
//            if (HasChildUIViews && DisableGameObjectWhenHidden) DisableGameObjectWhenHidden = false;
            if (Settings.UseOrientationDetector && OrientationDetector != null)
                OrientationDetector.OnOrientationEvent.AddListener(OnOrientationChange); //if using OrientationDetector subscribe to the onOrientationChange UnityEvent every time this UIView is enabled
        }


        public override void OnDisable()
        {
            base.OnDisable();

            StopHide();
            StopShow();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Loop);

            ResetToStartValues();

            if (Settings.UseOrientationDetector && OrientationDetector != null)
                OrientationDetector.OnOrientationEvent.RemoveListener(OnOrientationChange); //if using OrientationDetector unsubscribe from the OnOrientationChange UnityEvent every time this UIView is disabled
        }

        #endregion

        #region Public Methods

        /// <summary> Cancel an auto hide, if it was initiated </summary>
        public void CancelAutoHide()
        {
            if (m_autoHideCoroutine == null) return;
            StopCoroutine(m_autoHideCoroutine);
            m_autoHideCoroutine = null;
        }

        /// <summary> Hides this UIView </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void Hide(bool instantAction = false)
        {
            if (HideBehavior.InstantAnimation) instantAction = true;

            StopShow();

            if (!HideBehavior.Animation.Enabled && !instantAction)
            {
                DDebug.Log("You are trying to HIDE the (" + ViewCategory + ")(" + ViewName + ") UIView, but you did not enable any HIDE animations. Enable at least one HIDE animation in order to fix this issue.", this);
                return; //no HIDE animations have been enabled -> cannot hide this UIView -> stop here
            }

            if (Visibility == VisibilityState.Hiding) return;
            if (!IsVisible)
            {
                if (VisibleViews.Contains(this)) VisibleViews.Remove(this);
                return;
            }

            m_hideCoroutine = StartCoroutine(HideEnumerator(instantAction));
        }

        /// <summary> Hide this UIView after the set delay </summary>
        /// <param name="delay"> Time delay before this UIView is automatically hidden </param>
        public void Hide(float delay) { m_autoHideCoroutine = StartCoroutine(HideWithDelayEnumerator(delay)); }

        /// <summary> Hide this UIView instantly without triggering any of its actions </summary>
        public void InstantHide()
        {
            CheckForLayoutController();

            StopLoopAnimation();

            StopShow();
            StopHide();

            ResetToStartValues();

            Canvas.enabled = !DisableCanvasWhenHidden;                     //disable the canvas, if the option is enabled
            GraphicRaycaster.enabled = !DisableGraphicRaycasterWhenHidden; //disable the graphic raycaster, if the option is enabled
            gameObject.SetActive(!DisableGameObjectWhenHidden);            //disable the Source the UIView is attached to, if the option is enabled

            HideDeselectButton();

            Visibility = VisibilityState.NotVisible;
            if (VisibleViews.Contains(this)) VisibleViews.Remove(this);

            if (m_initialized) NotifySystemOfTriggeredBehavior(UIViewBehaviorType.Hide); //send the global events

            RemoveHiddenFromVisibleViews();

            if (!m_initialized) m_initialized = true;
        }

        /// <summary> Show this UIView instantly without triggering any of its actions </summary>
        public void InstantShow()
        {
            CheckForLayoutController();

            StopLoopAnimation();
            StopHide();
            StopShow();

            ResetToStartValues();

            Canvas.enabled = true;           //enable the canvas
            GraphicRaycaster.enabled = true; //enable the graphic raycaster
            gameObject.SetActive(true);      //set the active state to true (in case it has been disabled when hidden)

            Visibility = VisibilityState.Visible;
            if (!VisibleViews.Contains(this)) VisibleViews.Add(this);

            NotifySystemOfTriggeredBehavior(UIViewBehaviorType.Show); //send the global events

            if (AutoHideAfterShow) Hide(AutoHideAfterShowDelay);
            ShowSelectDeselectButton(); //select the selectedButton

            RemoveHiddenFromVisibleViews();

            if (HasChildUIViews) StartCoroutine(ShowViewNextFrame(ViewCategory, ViewName, true));
        }

        /// <summary> Sends an UIViewMessage notifying the system that an UIViewBehavior has been triggered </summary>
        /// <param name="behaviorType"> The UIViewBehaviorType of the UIViewBehavior that has been triggered </param>
        public void NotifySystemOfTriggeredBehavior(UIViewBehaviorType behaviorType)
        {
            if (OnUIViewAction != null) OnUIViewAction.Invoke(this, behaviorType);
            Message.Send(new UIViewMessage(this, behaviorType));
        }

        /// <inheritdoc />
        /// <summary> Resets this UIView's CanvasGroup.alpha to the StartAlpha value </summary>
        public override void ResetAlpha() { CanvasGroup.alpha = StartAlpha; }

        /// <inheritdoc />
        /// <summary> Resets this UIView's RectTransform.anchoredPosition3D to the CurrentStartPosition value </summary>
        public override void ResetPosition() { RectTransform.anchoredPosition3D = CurrentStartPosition; }

        /// <summary> Sets the 'visibility' of this UIView by showing or hiding it </summary>
        /// <param name="visible"> If TRUE it will show the UIView (if hidden). If FALSE it will hide the UIView (if visible) </param>
        public void SetVisibility(bool visible)
        {
            if (visible) Show();
            else Hide();
        }

        /// <summary> Sets the 'visibility' of this UIView by showing or hiding it </summary>
        /// <param name="visible"> If TRUE it will show the UIView (if hidden). If FALSE it will hide the UIView (if visible) </param>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void SetVisibility(bool visible, bool instantAction)
        {
            if (visible) Show(instantAction);
            else Hide(instantAction);
        }

        /// <summary> Shows this UIView </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void Show(bool instantAction = false)
        {
            if (ShowBehavior.InstantAnimation) instantAction = true;

            gameObject.SetActive(true); //set the active state to true (in case it has been disabled when hidden)

            StopHide();

            if (!ShowBehavior.Animation.Enabled && !instantAction)
            {
                DDebug.Log("You are trying to SHOW the (" + ViewCategory + ")(" + ViewName + ") UIView, but you did not enable any SHOW animations. Enable at least one SHOW animation in order to fix this issue.", this);
                return; //no SHOW animations have been enabled -> cannot show this UIView -> stop here
            }


            if (Visibility == VisibilityState.Showing) return;
            if (IsVisible)
            {
                if (!VisibleViews.Contains(this)) VisibleViews.Add(this);
                return; //this UIView is already visible -> stop here
            }

            m_showCoroutine = StartCoroutine(ShowEnumerator(instantAction));
            if (HasChildUIViews) StartCoroutine(ShowViewNextFrame(ViewCategory, ViewName, instantAction));
        }

        /// <summary> Start the loop animations set up on this UIView </summary>
        public void StartLoopAnimation()
        {
            if (!LoopBehavior.Animation.Enabled) return;
            UIAnimator.MoveLoop(RectTransform, LoopBehavior.Animation, CurrentStartPosition);
            UIAnimator.RotateLoop(RectTransform, LoopBehavior.Animation, StartRotation);
            UIAnimator.ScaleLoop(RectTransform, LoopBehavior.Animation);
            UIAnimator.FadeLoop(RectTransform, LoopBehavior.Animation);

            NotifySystemOfTriggeredBehavior(UIViewBehaviorType.Loop); //send the global events
        }

        /// <summary> Stop any running loop animations on this UIView </summary>
        public void StopLoopAnimation() { UIAnimator.StopAnimations(RectTransform, AnimationType.Loop); }

        /// <summary> Toggle this UIView by showing it if it's NOT visible or hiding it otherwise </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void Toggle(bool instantAction = false)
        {
            if (IsVisible)
                Hide(instantAction);
            else
                Show(instantAction);
        }

        #endregion

        #region Private Methods

        private void HideDeselectButton()
        {
            if (DeselectAnyButtonSelectedOnHide)              //check that the deselect any button on HIDE option is enabled
                UnityEventSystem.SetSelectedGameObject(null); //clear any selection
        }

        private void Initialize()
        {
            m_childUIButtons = GetComponentsInChildren<UIButton>();
//            RemoveNullChildUIButtons();
            UpdateChildUIButtonsStartValues();

            m_childUIViews = GetComponentsInChildren<UIView>();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (BehaviorAtStart)
            {
                case UIViewStartBehavior.DoNothing:
                    ShowSelectDeselectButton(); //select the auto-selected button
                    if (LoopBehavior.AutoStartLoopAnimation) StartLoopAnimation();
                    Canvas.enabled = true;           //enable the canvas
                    GraphicRaycaster.enabled = true; //enable the graphic raycaster
                    m_initialized = true;
                    break;
                case UIViewStartBehavior.Hide:
                    InstantHide();
                    break;
                case UIViewStartBehavior.PlayShowAnimation:
                    InstantHide();
                    if (Settings.UseOrientationDetector)
                    {
                        if (OrientationDetector.CurrentOrientation == DetectedOrientation.Unknown)
                            StartCoroutine(ExecuteGetOrientationEnumerator());
                        else
                            OnOrientationChange(OrientationDetector.CurrentOrientation);
                    }
                    else
                    {
                        Show();
                        if (HasChildUIViews) StartCoroutine(ShowViewNextFrame(ViewCategory, ViewName, false));
                    }

                    break;
            }
        }

        private void MoveToCustomStartPosition() { RectTransform.anchoredPosition3D = CurrentStartPosition; }

        private void LoadPresets()
        {
            if (ShowBehavior.LoadSelectedPresetAtRuntime) ShowBehavior.LoadPreset();
            if (HideBehavior.LoadSelectedPresetAtRuntime) HideBehavior.LoadPreset();
            if (LoopBehavior.LoadSelectedPresetAtRuntime) LoopBehavior.LoadPreset();
        }

        private void OnOrientationChange(DetectedOrientation newDeviceOrientation)
        {
            if (newDeviceOrientation == DetectedOrientation.Landscape && TargetOrientation == TargetOrientation.Portrait ||
                newDeviceOrientation == DetectedOrientation.Portrait && TargetOrientation == TargetOrientation.Landscape)
            {
                HideView(ViewCategory, ViewName, true); //hide all the views that have the same 'viewCategory' and 'viewName' instantly (in zero seconds)
                ShowView(ViewCategory, ViewName);       //show all the views that have the same 'viewCategory' and 'viewName'
            }
            else if (TargetOrientation == TargetOrientation.Any) //if this UIView targets both landscape and portrait orientations)
            {
                ShowView(ViewCategory, ViewName); //show all the views that have the same 'viewCategory' and 'viewName'
            }
            else //this UIView is not configured to work with the current orientation
            {
                Hide(true); //hide this UIView instantly (in zero seconds)
            }
        }

        private void ShowSelectDeselectButton()
        {
            if (AutoSelectButtonAfterShow && SelectedButton != null)    //check that the auto select option is enabled and that a selectedButton has been referenced
                UnityEventSystem.SetSelectedGameObject(SelectedButton); //select the referenced selectedButton
            else if (DeselectAnyButtonSelectedOnShow)                   //check that the deselect any button on SHOW option is enabled
                UnityEventSystem.SetSelectedGameObject(null);           //clear any selection
        }

        private void StopHide()
        {
            if (m_hideCoroutine == null) return;
            StopCoroutine(m_hideCoroutine);
            m_hideCoroutine = null;
            Visibility = VisibilityState.NotVisible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();
        }

        private void StopShow()
        {
            if (m_showCoroutine == null) return;
            StopCoroutine(m_showCoroutine);
            m_showCoroutine = null;
            Visibility = VisibilityState.Visible;
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();
        }

        // ReSharper disable once UnusedMember.Local
        private void RemoveNullChildUIButtons()
        {
            if (m_childUIButtons == null || m_childUIButtons.Length <= 0) return;     //check that the m_childUIButtons array is not null and that we have a least one entry
            bool foundNullEntry = m_childUIButtons.Any(uiButton => uiButton == null); //look for null entries
            if (!foundNullEntry) return;                                              //at least one null entries was found -> clean up the _childUIButtons array by removing all null entries
            m_childUIButtons = m_childUIButtons.Where(t => t != null).ToArray();      //set the new _childUIButtons as an array created from the temp list (that has no null entries)
        }

        private void UpdateChildUIButtonsStartValues()
        {
            if (m_childUIButtons == null) return; //check that the m_childUIButtons array is not null and that we have a least one entry
            foreach (UIButton button in m_childUIButtons)
                button.UpdateStartValues();
        }

        #endregion

        #region IEnumerators

        /// <summary> Show all the UIViews with the passed view category and view name in the NEXT FRAME </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name (found in the view category) </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        public static IEnumerator ShowViewNextFrame(string viewCategory, string viewName, bool instantAction = false)
        {
            yield return null; //skip a frame

            ShowView(viewCategory, viewName, instantAction);
        }

        /// <summary> Hide all the UIViews with the passed view category and view name int the NEXT FRAME</summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name (found in the view category) </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        public static IEnumerator HideViewNextFrame(string viewCategory, string viewName, bool instantAction = false)
        {
            yield return null; //skip a frame

            HideView(viewCategory, viewName, instantAction);
        }

        private IEnumerator ShowEnumerator(bool instantAction)
        {
//            yield return null; //skip a frame
            if (Settings.AutoDisableUIInteractions) DisableUIInteractions();

            UIAnimator.StopAnimations(RectTransform, ShowBehavior.Animation.AnimationType); //stop any SHOW animations

            if (LoopBehavior.Animation.Enabled) UIAnimator.StopAnimations(RectTransform, LoopBehavior.Animation.AnimationType);

            Canvas.enabled = true;           //enable the canvas
            GraphicRaycaster.enabled = true; //enable the graphic raycaster

            CheckForLayoutController();

            //MOVE
            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, ShowBehavior.Animation, CurrentStartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, ShowBehavior.Animation, CurrentStartPosition);
            if (!ShowBehavior.Animation.Move.Enabled || instantAction) ResetPosition();
            UIAnimator.Move(RectTransform, ShowBehavior.Animation, moveFrom, moveTo, instantAction); //initialize and play the SHOW Move tween

            //ROTATE
            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(ShowBehavior.Animation, StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(ShowBehavior.Animation, StartRotation);
            if (!ShowBehavior.Animation.Rotate.Enabled || instantAction) ResetRotation();
            UIAnimator.Rotate(RectTransform, ShowBehavior.Animation, rotateFrom, rotateTo, instantAction); //initialize and play the SHOW Rotate tween

            //SCALE
            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(ShowBehavior.Animation, StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(ShowBehavior.Animation, StartScale);
            if (!ShowBehavior.Animation.Scale.Enabled || instantAction) ResetScale();
            UIAnimator.Scale(RectTransform, ShowBehavior.Animation, scaleFrom, scaleTo, instantAction); //initialize and play the SHOW Scale tween

            //FADE
            float fadeFrom = UIAnimator.GetAnimationFadeFrom(ShowBehavior.Animation, StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(ShowBehavior.Animation, StartAlpha);
            if (!ShowBehavior.Animation.Fade.Enabled || instantAction) ResetAlpha();
            UIAnimator.Fade(RectTransform, ShowBehavior.Animation, fadeFrom, fadeTo, instantAction); //initialize and play the SHOW Fade tween

            Visibility = VisibilityState.Showing; //update the visibility state
            if (!VisibleViews.Contains(this)) VisibleViews.Add(this);

            if (instantAction) ShowBehavior.OnStart.Invoke(gameObject, false, false);
            NotifySystemOfTriggeredBehavior(UIViewBehaviorType.Show); //send the global events

            if (HideProgressor != null) HideProgressor.SetValue(0f);

            float startTime = Time.realtimeSinceStartup;
            if (!instantAction) //wait for the animation to finish
            {
//                yield return new WaitForSecondsRealtime(ShowBehavior.Animation.TotalDuration);

                float totalDuration = ShowBehavior.Animation.TotalDuration;
                float elapsedTime = startTime - Time.realtimeSinceStartup;
                float startDelay = ShowBehavior.Animation.StartDelay;
                bool invokedOnStart = false;
                while (elapsedTime <= totalDuration) //wait for seconds realtime (ignore Unity's Time.Timescale)
                {
                    elapsedTime = Time.realtimeSinceStartup - startTime;

                    if (!invokedOnStart && elapsedTime > startDelay)
                    {
                        ShowBehavior.OnStart.Invoke(gameObject);
                        invokedOnStart = true;
                    }

                    if (m_hasLayoutController) m_layoutController.Rebuild(true);

                    VisibilityProgress = elapsedTime / totalDuration;
                    yield return null;
                }
            }

            if (m_hasLayoutController) m_layoutController.Rebuild(true);

            ShowBehavior.OnFinished.Invoke(gameObject, !instantAction, !instantAction);

            Visibility = VisibilityState.Visible; //update the visibility state
            if (!VisibleViews.Contains(this)) VisibleViews.Add(this);
//            RemoveNullChildUIButtons();
//            UpdateChildUIButtonsStartValues();

            StartLoopAnimation(); //start playing the loop animation
            if (AutoHideAfterShow) Hide(AutoHideAfterShowDelay);
            ShowSelectDeselectButton(); //select the selectedButton
            m_showCoroutine = null;     //clear the coroutine reference
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();

            RemoveHiddenFromVisibleViews();
        }

        private IEnumerator HideEnumerator(bool instantAction)
        {
//            yield return null; //skip a frame
            if (Settings.AutoDisableUIInteractions) DisableUIInteractions();

            UIAnimator.StopAnimations(RectTransform, HideBehavior.Animation.AnimationType); //stop any HIDE animations

            if (LoopBehavior.Animation.Enabled) UIAnimator.StopAnimations(RectTransform, LoopBehavior.Animation.AnimationType);

            CheckForLayoutController();

            //MOVE
            if (m_controlledByLayoutGroup) UpdateStartPosition();
            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(RectTransform, HideBehavior.Animation, CurrentStartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(RectTransform, HideBehavior.Animation, CurrentStartPosition);
            if (!HideBehavior.Animation.Move.Enabled || instantAction) ResetPosition();
            UIAnimator.Move(RectTransform, HideBehavior.Animation, moveFrom, moveTo, instantAction); //initialize and play the HIDE Move tween

            //ROTATE
            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(HideBehavior.Animation, StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(HideBehavior.Animation, StartRotation);
            if (!HideBehavior.Animation.Rotate.Enabled || instantAction) ResetRotation();
            UIAnimator.Rotate(RectTransform, HideBehavior.Animation, rotateFrom, rotateTo, instantAction); //initialize and play the HIDE Rotate tween

            //SCALE
            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(HideBehavior.Animation, StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(HideBehavior.Animation, StartScale);
            if (!HideBehavior.Animation.Scale.Enabled || instantAction) ResetScale();
            UIAnimator.Scale(RectTransform, HideBehavior.Animation, scaleFrom, scaleTo, instantAction); //initialize and play the HIDE Scale tween

            //FADE
            float fadeFrom = UIAnimator.GetAnimationFadeFrom(HideBehavior.Animation, StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(HideBehavior.Animation, StartAlpha);
            if (!HideBehavior.Animation.Fade.Enabled || instantAction) ResetAlpha();
            UIAnimator.Fade(RectTransform, HideBehavior.Animation, fadeFrom, fadeTo, instantAction); //initialize and play the HIDE Fade tween

            HideDeselectButton();
            Visibility = VisibilityState.Hiding; //update the visibility state
            if (VisibleViews.Contains(this)) VisibleViews.Remove(this);
            if (m_initialized)
            {
                if (instantAction) HideBehavior.OnStart.Invoke(gameObject, false, false);
                NotifySystemOfTriggeredBehavior(UIViewBehaviorType.Hide); //send the global events
            }

            float startTime = Time.realtimeSinceStartup;
            if (!instantAction) //wait for the animation to finish
            {
//                    yield return new WaitForSecondsRealtime(HideBehavior.Animation.TotalDuration + UIViewSettings.DISABLE_WHEN_HIDDEN_TIME_BUFFER); //wait for seconds realtime (ignore Unity's Time.Timescale)

                float totalDuration = HideBehavior.Animation.TotalDuration;
                float elapsedTime = startTime - Time.realtimeSinceStartup;
                float startDelay = HideBehavior.Animation.StartDelay;
                bool invokedOnStart = false;
                while (elapsedTime <= totalDuration) //wait for seconds realtime (ignore Unity's Time.Timescale)
                {
                    elapsedTime = Time.realtimeSinceStartup - startTime;

                    if (!invokedOnStart && elapsedTime > startDelay)
                    {
                        HideBehavior.OnStart.Invoke(gameObject);
                        invokedOnStart = true;
                    }

                    if (m_hasLayoutController) m_layoutController.Rebuild(true);

                    VisibilityProgress = 1 - elapsedTime / totalDuration; //operation is reversed in hide than in show
                    yield return null;
                }

                yield return new WaitForSecondsRealtime(UIViewSettings.DISABLE_WHEN_HIDDEN_TIME_BUFFER); //wait for seconds realtime (ignore Unity's Time.Timescale)
            }

            if (m_hasLayoutController) m_layoutController.Rebuild(true);

            if (m_initialized)
            {
                HideBehavior.OnFinished.Invoke(gameObject, !instantAction, !instantAction);
            }

            Visibility = VisibilityState.NotVisible; //update the visibility state
            if (VisibleViews.Contains(this)) VisibleViews.Remove(this);

            Canvas.enabled = !DisableCanvasWhenHidden;                     //disable the canvas, if the option is enabled
            GraphicRaycaster.enabled = !DisableGraphicRaycasterWhenHidden; //disable the graphic raycaster, if the option is enabled
            gameObject.SetActive(!DisableGameObjectWhenHidden);            //disable the Source the UIView is attached to, if the option is enabled
            m_hideCoroutine = null;                                        //clear the coroutine reference
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();

            RemoveHiddenFromVisibleViews();

            if (!m_initialized) m_initialized = true;
        }

        private IEnumerator HideWithDelayEnumerator(float delay)
        {
            if (delay > 0) yield return new WaitForSecondsRealtime(delay);
            Hide();
            m_autoHideCoroutine = null;
        }

        private IEnumerator ExecuteGetOrientationEnumerator()
        {
            while (OrientationDetector.CurrentOrientation == DetectedOrientation.Unknown) //if the current orientation is unknown
            {
                OrientationDetector.CheckDeviceOrientation(); //check device orientation until we know if it's portrait or landscape
                yield return null;                            //skip to next frame
            }

            OnOrientationChange(OrientationDetector.CurrentOrientation);
        }

        #endregion

        #region Static Methods

        /// <summary> Returns a list of all the UIViews, registered in the UIView.Database, with the given view category and view name. If no UIView is found, it will return an empty list </summary>
        /// <param name="viewCategory"> UIView category to search for</param>
        /// <param name="viewName"> UIView name to search for (found in the view category) </param>
        /// <returns></returns>
        public static List<UIView> GetViews(string viewCategory, string viewName)
        {
            var views = new List<UIView>(); //create temp list
            foreach (UIView view in Database)
            {
                if (!view.ViewCategory.Equals(viewCategory)) continue;
                if (!view.ViewName.Equals(viewName)) continue;
                views.Add(view); //categories and names match -> add to list
            }

            return views; //return list
        }

        /// <summary> Hide all the UIViews with the passed view name and the default view category name 'General' </summary>
        /// <param name="viewName"> UIView name </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        public static void HideView(string viewName, bool instantAction = false) { HideView(DefaultViewCategory, viewName, instantAction); }

        /// <summary> Hide all the UIViews with the passed view category and view name </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name (found in the view category) </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        public static void HideView(string viewCategory, string viewName, bool instantAction = false)
        {
            //if (viewCategory.Equals(DefaultViewCategory) && viewName.Equals(DefaultViewName)) return;
            ExecuteHide(viewCategory, viewName, instantAction);
        }

        /// <summary> Hide all the UIViews with the passed view category </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        public static void HideViewCategory(string viewCategory, bool instantAction = false) { ExecuteHideCategory(viewCategory, instantAction); }

        /// <summary>
        /// Returns TRUE if at least one UIView, with the passed view category and view name, is visible.
        /// <para/> An UIView is considered visible if its IsVisible value is true and if it has been added to the VisibleViews list.
        /// </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name (found in the view category) </param>
        public static bool IsViewVisible(string viewCategory, string viewName)
        {
            foreach (UIView view in VisibleViews)
            {
                if (!view.ViewCategory.Equals(viewCategory)) continue;
                if (!view.ViewName.Equals(viewName)) continue;
                if (!view.IsVisible) continue;
                return true; //at least one UIView, with the passed category and name, is visible -> return TRUE
            }

            return false; //no UIView, with the passed category and name, is visible -> return FALSE
        }

        /// <summary> Show all the UIViews with the passed view name and the default view category name 'General' </summary>
        /// <param name="viewName"> UIView name </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        public static void ShowView(string viewName, bool instantAction = false) { ShowView(DefaultViewCategory, viewName, instantAction); }

        /// <summary> Show all the UIViews with the passed view category and view name </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name (found in the view category) </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        public static void ShowView(string viewCategory, string viewName, bool instantAction = false)
        {
            //if (viewCategory.Equals(DefaultViewCategory) && viewName.Equals(DefaultViewName)) return;
            ExecuteShow(viewCategory, viewName, instantAction);
        }

        /// <summary> Show all the UIViews with the passed view category </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        public static void ShowViewCategory(string viewCategory, bool instantAction = false) { ExecuteShowCategory(viewCategory, instantAction); }

        /// <summary> Execute the HIDE animation according to each UIView's specific settings </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        private static void ExecuteHide(string viewCategory, string viewName, bool instantAction = false)
        {
            bool foundNullEntry = false; //null check flag
            foreach (UIView view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                } //this null check flag has been added to fix the slim (impossible) chance that a null UIView reference exists in the registry;
                //this can happen if it has been destroyed/deleted (thus now it's null) and has not been unregistered (sanity check)

                if (!view.ViewCategory.Equals(viewCategory)) continue;
                if (!view.ViewName.Equals(viewName)) continue;
                if (!view.gameObject.activeInHierarchy) continue;
                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Hiding the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "NOT ") + "an instant action.");
                view.Hide(instantAction); //hide the UIView
            }

            if (foundNullEntry)                           //well... it happened... at least one null UIView reference was found in the UIView.Database
                RemoveAnyNullReferencesFromTheDatabase(); //remove any null references from the Database
        }

        /// <summary> Execute the HIDE animation for all the UIView's that belong to a given view category </summary>
        /// <param name="viewCategory"> The UIView category </param>
        /// <param name="instantAction"> Determines if the HIDE animation should happen instantly (in zero seconds) </param>
        private static void ExecuteHideCategory(string viewCategory, bool instantAction = false)
        {
            bool foundNullEntry = false; //null check flag
            foreach (UIView view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                } //this null check flag has been added to fix the slim (impossible) chance that a null UIView reference exists in the registry;
                //this can happen if it has been destroyed/deleted (thus now it's null) and has not been unregistered (sanity check)

                if (!view.ViewCategory.Equals(viewCategory)) continue;
                if (!view.gameObject.activeInHierarchy) continue;
                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Hiding the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "NOT ") + "an instant action.");
                view.Hide(instantAction); //hide the UIView
            }

            if (foundNullEntry)                           //well... it happened... at least one null UIView reference was found in the UIView.Database
                RemoveAnyNullReferencesFromTheDatabase(); //remove any null references from the Database
        }

        /// <summary> Execute the SHOW animation according to each UIView's specific settings </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="viewName"> UIView name </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        private static void ExecuteShow(string viewCategory, string viewName, bool instantAction)
        {
            bool foundNullEntry = false; //null check flag
            foreach (UIView view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                } //this null check flag has been added to fix the slim (impossible) chance that a null UIView reference exists in the registry;
                //this can happen if it has been destroyed/deleted (thus now it's null) and has not been unregistered (sanity check)


                if (!view.ViewCategory.Equals(viewCategory)) continue;
                if (!view.ViewName.Equals(viewName)) continue;
                view.gameObject.SetActive(true); //set the Source active state to true (in case it has been disabled when hidden)
                if (!view.gameObject.activeInHierarchy) continue;
                if (!Settings.UseOrientationDetector) //check if the system is using the OrientationDetector
                {
                    if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "NOT ") + "an instant action.");
                    view.Show(instantAction); //show the UIView
                    continue;                 //continue to the next Database entry
                }

                if (OrientationDetector.CurrentOrientation == DetectedOrientation.Unknown) //check that we do not have an Unknown orientation
                    OrientationDetector.CheckDeviceOrientation();                          //do an orientation check to update the current orientation

                switch (OrientationDetector.CurrentOrientation)
                {
                    case DetectedOrientation.Portrait:
                        switch (view.TargetOrientation)
                        {
                            case TargetOrientation.Portrait:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Portrait : ""));
                                view.Show(instantAction);
                                break;
                            case TargetOrientation.Landscape:
                                view.Visibility = VisibilityState.Visible;
                                view.Hide(true);
                                break;
                            case TargetOrientation.Any:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Any : ""));
                                view.Show(instantAction);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case DetectedOrientation.Landscape:
                        switch (view.TargetOrientation)
                        {
                            case TargetOrientation.Portrait:
                                view.Visibility = VisibilityState.Visible;
                                view.Hide(true);
                                break;
                            case TargetOrientation.Landscape:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Landscape : ""));
                                view.Show(instantAction);
                                break;
                            case TargetOrientation.Any:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + viewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Any : ""));
                                view.Show(instantAction);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case DetectedOrientation.Unknown:
                        if (DoozySettings.Instance.DebugUIView) DDebug.Log("Unknown Orientation");
                        //do nothing
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (foundNullEntry)                           //well... it happened... at least one null UIView reference was found in the UIView.Database
                RemoveAnyNullReferencesFromTheDatabase(); //remove any null references from the Database
        }

        /// <summary> Execute the SHOW animation for all the UIView's that belong to a given view category </summary>
        /// <param name="viewCategory"> UIView category </param>
        /// <param name="instantAction"> Determines if the SHOW animation should happen instantly (in zero seconds) </param>
        private static void ExecuteShowCategory(string viewCategory, bool instantAction = false)
        {
            bool foundNullEntry = false; //null check flag
            foreach (UIView view in Database)
            {
                if (view == null)
                {
                    foundNullEntry = true;
                    continue;
                } //this null check flag has been added to fix the slim (impossible) chance that a null UIView reference exists in the registry;
                //this can happen if it has been destroyed/deleted (thus now it's null) and has not been unregistered (sanity check)


                if (!view.ViewCategory.Equals(viewCategory)) continue;
                view.gameObject.SetActive(true); //set the Source active state to true (in case it has been disabled when hidden)
                if (!view.gameObject.activeInHierarchy) continue;
                if (!Settings.UseOrientationDetector) //check if the system is using the OrientationDetector
                {
                    if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "NOT ") + "an instant action.");
                    view.Show(instantAction); //show the UIView
                    continue;                 //continue to the next Database entry
                }

                if (OrientationDetector.CurrentOrientation == DetectedOrientation.Unknown) //check that we do not have an Unknown orientation
                    OrientationDetector.CheckDeviceOrientation();                          //do an orientation check to update the current orientation

                switch (OrientationDetector.CurrentOrientation)
                {
                    case DetectedOrientation.Portrait:
                        switch (view.TargetOrientation)
                        {
                            case TargetOrientation.Portrait:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Portrait : ""));
                                view.Show(instantAction);
                                break;
                            case TargetOrientation.Landscape:
                                view.Visibility = VisibilityState.Visible;
                                view.Hide(true);
                                break;
                            case TargetOrientation.Any:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Any : ""));
                                view.Show(instantAction);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case DetectedOrientation.Landscape:
                        switch (view.TargetOrientation)
                        {
                            case TargetOrientation.Portrait:
                                view.Visibility = VisibilityState.Visible;
                                view.Hide(true);
                                break;
                            case TargetOrientation.Landscape:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Landscape : ""));
                                view.Show(instantAction);
                                break;
                            case TargetOrientation.Any:
                                if (DoozySettings.Instance.DebugUIView) DDebug.Log("Showing the (" + viewCategory + ")(" + view.ViewName + ") UIView. This is " + (instantAction ? "" : "not ") + "an instant action." + (DoozySettings.Instance.UseOrientationDetector ? " TargetOrientation: " + TargetOrientation.Any : ""));
                                view.Show(instantAction);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        break;
                    case DetectedOrientation.Unknown:
                        if (DoozySettings.Instance.DebugUIView) DDebug.Log("Unknown Orientation");
                        //do nothing
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (foundNullEntry)                           //well... it happened... at least one null UIView reference was found in the UIView.Database
                RemoveAnyNullReferencesFromTheDatabase(); //remove any null references from the Database
        }

        /// <summary> Check the VisibleViews list and remove any null entries and any UIViews that are marked as being hidden </summary>
        private static void RemoveHiddenFromVisibleViews()
        {
            RemoveNullsFromVisibleViews();
            for (int i = VisibleViews.Count - 1; i >= 0; i--)
                if (VisibleViews[i].IsHidden)
                    VisibleViews.RemoveAt(i);
        }

        /// <summary> Check the VisibleViews list and removes any null entries </summary>
        private static void RemoveNullsFromVisibleViews()
        {
            for (int i = VisibleViews.Count - 1; i >= 0; i--)
                if (VisibleViews[i] == null)
                    VisibleViews.RemoveAt(i);
        }

        #endregion
    }
}