// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;
#if dUI_TextMeshPro
using TMPro;
#endif
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable ConvertToAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system that works and behaves like a pop-up window. It appears superimposed over everything and without the user explicitly triggering it.
    ///     <para/> The UIPopup also works as a modal window that can force the user to interact with it before they can go back to using the parent application.
    ///     <para/> UIPopups are sometimes called heavy windows or modal dialogs because they often display a dialog box that communicates information to the user and prompts them for a response.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIPopup_AddComponentMenu_MenuName, MenuUtils.UIPopup_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIPOPUP)]
    public class UIPopup : UIComponentBase<UIPopup>
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIPopup_MenuItem_ItemName, false, MenuUtils.UIPopup_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject(MenuUtils.UIPopup_GameObject_Name, typeof(RectTransform), typeof(UIPopup));
            GameObjectUtility.SetParentAndAlign(go, GetCanvasAsParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            var popup = go.GetComponent<UIPopup>();
            popup.RectTransform.FullScreen(true);

            var overlay = new GameObject("Overlay", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Image));
            GameObjectUtility.SetParentAndAlign(overlay, go);
            overlay.GetComponent<Image>().color = DoozyUtils.OverlayColor;
            popup.Overlay.RectTransform = overlay.GetComponent<RectTransform>();
            popup.Overlay.RectTransform.localPosition = Vector3.zero;
            popup.Overlay.FullScreen(true);
            popup.Overlay.Init();

            var container = new GameObject("Container", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup), typeof(Image));
            GameObjectUtility.SetParentAndAlign(container, go);
            container.GetComponent<Image>().color = DoozyUtils.BackgroundColor;
            popup.Container.RectTransform = container.GetComponent<RectTransform>();
            popup.Container.RectTransform.localPosition = Vector3.zero;
            popup.Container.FullScreen(true);
            popup.Container.RectTransform.Center(true);
            popup.Container.RectTransform.sizeDelta = new Vector2(256, 192);
            popup.Container.Init();
            popup.Container.Canvas.enabled = true;
            popup.Container.GraphicRaycaster.enabled = true;

            var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            popup.Data.Images.Add(icon.GetComponent<Image>());
            GameObjectUtility.SetParentAndAlign(icon, container);
            var iconRectTransform = icon.GetComponent<RectTransform>();
            iconRectTransform.localPosition = new Vector3(0, 136, 0);
            iconRectTransform.localScale = Vector3.one;
            iconRectTransform.anchorMin = new Vector2(0.5f, 1);
            iconRectTransform.anchorMax = new Vector2(0.5f, 1);
            iconRectTransform.anchoredPosition = new Vector2(0, 40);
            iconRectTransform.sizeDelta = new Vector2(80, 80);
            iconRectTransform.pivot = new Vector2(0.5f, 1);
            icon.GetComponent<Image>().color = Color.black;


#if dUI_TextMeshPro
            GameObject title = CreateTextMeshProLabel("Title", "TITLE", container);
            title.GetComponent<TextMeshProUGUI>().fontSize = 18;
#else
            GameObject title = CreateTextLabel("Title", "TITLE", container);
            title.GetComponent<Text>().fontSize = 18;
#endif
            popup.Data.Labels.Add(title);
            var titleRectTransform = title.GetComponent<RectTransform>();
            titleRectTransform.localPosition = new Vector3(0, 32, 0);
            titleRectTransform.localScale = Vector3.one;
            titleRectTransform.anchorMin = new Vector2(0, 1);
            titleRectTransform.anchorMax = new Vector2(1, 1);
            titleRectTransform.anchoredPosition = new Vector2(0, -64);
            titleRectTransform.sizeDelta = new Vector2(-16, 40);
            titleRectTransform.pivot = new Vector2(0.5f, 0.5f);

#if dUI_TextMeshPro
            GameObject message = CreateTextMeshProLabel("Message", "message", container);
#else
            GameObject message = CreateTextLabel("Message", "message", container);
#endif
            popup.Data.Labels.Add(message);
            var messageRectTransform = message.GetComponent<RectTransform>();
            messageRectTransform.localPosition = new Vector3(0, -34, 0);
            messageRectTransform.localScale = Vector3.one;
            messageRectTransform.anchorMin = new Vector2(0, 0);
            messageRectTransform.anchorMax = new Vector2(1, 1);
            messageRectTransform.anchoredPosition = new Vector2(0, -34);
            messageRectTransform.sizeDelta = new Vector2(-40, -116);
            messageRectTransform.pivot = new Vector2(0.5f, 0.5f);

            UIButton button = UIButton.CreateUIButton(container);
            button.ButtonCategory = UIButton.CustomButtonCategory;
            button.ButtonName = "Ok";
            button.name = UIButtonSettings.Instance.RenamePrefix + button.ButtonName + UIButtonSettings.Instance.RenameSuffix;
            button.SetLabelText(button.ButtonName);
            button.RectTransform.localPosition = new Vector3(0, -111, 0);
            button.RectTransform.localScale = Vector3.one;
            button.RectTransform.anchorMin = new Vector2(0.5f, 0);
            button.RectTransform.anchorMax = new Vector2(0.5f, 0);
            button.RectTransform.anchoredPosition = new Vector2(0, -15);
            button.RectTransform.sizeDelta = new Vector2(60, 30);
            button.RectTransform.pivot = new Vector2(0.5f, 0);
            popup.Data.Buttons.Add(button);

            Selection.activeObject = go; //select the newly created gameObject
        }

        /// <summary> (EDITOR ONLY) Creates a TextMeshPro label </summary>
        public static GameObject CreateTextMeshProLabel(string labelName, string labelText, GameObject parent)
        {
#if dUI_TextMeshPro
            var label = new GameObject(labelName, typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, parent);

            var textMeshProLabel = label.AddComponent<TextMeshProUGUI>();
            textMeshProLabel.text = labelText;
            textMeshProLabel.color = DoozyUtils.TextColor;
            textMeshProLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textMeshProLabel.alignment = TextAlignmentOptions.Center;

            label.GetComponent<RectTransform>().FullScreen(true);
            return label;
#else
            return null;
#endif
        }

        /// <summary> (EDITOR ONLY) Creates a Text label </summary>
        public static GameObject CreateTextLabel(string labelName, string labelText, GameObject parent)
        {
            var label = new GameObject(labelName, typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, parent);

            var textLabel = label.AddComponent<Text>();
            textLabel.text = labelText;
            textLabel.color = DoozyUtils.TextColor;
            textLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textLabel.resizeTextForBestFit = false;
            textLabel.resizeTextMinSize = 12;
            textLabel.resizeTextMaxSize = 20;
            textLabel.alignment = TextAnchor.MiddleCenter;
            textLabel.alignByGeometry = true;
            textLabel.supportRichText = true;

            label.GetComponent<RectTransform>().FullScreen(true);

            return label;
        }
#endif

        #endregion

        #region Constants

        public const string DEFAULT_POPUP_CANVAS_NAME = "PopupCanvas";
        public const int DEFAULT_POPUP_CANVAS_OVERLAY_SORT_ORDER = 10000;

        #endregion

        #region Static Properties

        /// <summary> Returns TRUE if an UIPopup is currently visible </summary>
        public static bool AnyPopupVisible
        {
            get
            {
                RemoveNullsFromVisiblePopups();
                return VisiblePopups.Count > 0;
            }
        }

        /// <summary> Default UIPopup popup name </summary>
        public static string DefaultPopupName { get { return NamesDatabase.UNNAMED; } }

        /// <summary> Default UIPopup target canvas name </summary>
        public static string DefaultTargetCanvasName { get { return UICanvas.MasterCanvasName; } }

        /// <summary> Returns last entry in the VisiblePopups list. If the list is empty, it returns null </summary>
        public static UIPopup LastShownPopup { get { return VisiblePopups.Count > 0 ? VisiblePopups[VisiblePopups.Count - 1] : null; } }

        // ReSharper disable once InconsistentNaming
        /// <summary> Action invoked whenever an UIPopupBehavior is triggered </summary>
        public static Action<UIPopup, AnimationType> OnUIPopupAction = delegate { };

        // ReSharper disable once InconsistentNaming
        /// <summary> List of all the popups that are currently visible on screen </summary>
        public static readonly List<UIPopup> VisiblePopups = new List<UIPopup>();

        /// <summary> Direct reference to the TouchDetector </summary>
        private static TouchDetector Detector { get { return TouchDetector.Instance; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if the UIPopupManager has marked this UIPopup as being in the PopupQueue </summary>
        public bool AddedToQueue { get { return m_addedToQueue; } set { m_addedToQueue = value; } }

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas
        {
            get
            {
                if (m_canvas == null) m_canvas = GetComponent<Canvas>();
                return m_canvas;
            }
        }

        /// <summary> Returns TRUE if either HideOnClickAnywhere, or HasOverlay and HideOnClickOverlay, or HasContainer and HideOnClickContainer are true </summary>
        public bool DetectsTouch
        {
            get
            {
                return HideOnClickAnywhere || HasOverlay &&
                       HideOnClickOverlay || HasContainer &&
                       HideOnClickContainer;
            }
        }

        /// <summary> Reference to the GraphicRaycaster component </summary>
        public GraphicRaycaster GraphicRaycaster
        {
            get
            {
                if (m_graphicRaycaster == null) m_graphicRaycaster = GetComponent<GraphicRaycaster>();
                return m_graphicRaycaster;
            }
        }

        /// <summary> Returns TRUE if this UIPopup has a valid UIContainer Container </summary>
        public bool HasContainer { get { return Container != null && Container.RectTransform != null; } }

        /// <summary> Returns TRUE if this UIPopup has a valid UIContainer Overlay </summary>
        public bool HasOverlay { get { return Overlay != null && Overlay.RectTransform != null; } }

        /// <summary> Returns the inverse of the current Visibility value (float between 1 and 0) (NotVisible - Visible) </summary>
        public float InverseVisibility { get { return 1 - VisibilityProgress; } }

        /// <summary> Returns TRUE if this UIPopup is NOT visible (is NOT in view) </summary>
        public bool IsHidden { get { return Visibility == VisibilityState.NotVisible; } }

        /// <summary> Returns TRUE if this UIPopup is playing the Hide Animation to get out of view </summary>
        public bool IsHiding { get { return Visibility == VisibilityState.Hiding; } }

        /// <summary> Returns TRUE if this UIPopup is playing the Show Animation to become visible </summary>
        public bool IsShowing { get { return Visibility == VisibilityState.Showing; } }

        /// <summary> Returns TRUE if this UIPopup is visible (is in view) </summary>
        public bool IsVisible { get { return Visibility == VisibilityState.Visible; } }

        /// <summary> Set by the UIPopupManager when this UIPopup (clone) is created (from the prefab). Used for identification purposes </summary>
        public string PopupName { get; private set; }

        /// <summary> The current visibility state of this UIPopup (Visible, NotVisible, Hiding or Showing) </summary>
        public VisibilityState Visibility
        {
            get { return m_visibilityState; }
            set
            {
                m_visibilityState = value;
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

        /// <summary> The visibility value of this UIPopup (float between 0 and 1) (NotVisible - Visible) </summary>
        public float VisibilityProgress
        {
            get { return m_visibilityProgress; }
            set
            {
                m_visibilityProgress = Mathf.Clamp01(value);
                UpdateOverlayAlpha(VisibilityProgress);
                // ReSharper disable once SwitchStatementMissingSomeCases
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

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUIPopup; } }

        #endregion

        #region Public Variables

        /// <summary>
        ///     Should this popup be added to the PopupQueue?
        ///     <para />
        ///     By adding this popup to the PopupQueue means that if another UIPopup is currently visible, then this UIPopup will appear only after that one has been closed.
        /// </summary>
        public bool AddToPopupQueue;

        /// <summary> If TRUE, after this UIPopup gets shown, it will get automatically hidden after the AutoHideAfterShowDelay time interval has passed </summary>
        public bool AutoHideAfterShow;

        /// <summary> If AutoHideAfterShow is TRUE, this is the time interval after which this UIPopup will get automatically hidden </summary>
        public float AutoHideAfterShowDelay;

        /// <summary> If TRUE, after this UIPopup has been shown, the referenced SelectedButton GameObject will get automatically selected by EventSystem.current </summary>
        public bool AutoSelectButtonAfterShow;

        /// <summary> If TRUE, after this UIPopup has been hidden, the previously selected GameObject will get automatically selected by EventSystem.current </summary>
        public bool AutoSelectPreviouslySelectedButtonAfterHide = true;

        /// <summary> If TRUE, the 'Back' button event will be blocked by this UIDrawer is visible (default: TRUE) </summary>
        public bool BlockBackButton = true;

        /// <summary> The implicit target UICanvas where this UIPopup will be shown </summary>
        public string CanvasName = DefaultTargetCanvasName;

        /// <summary> UIDrawer container settings. This is the actual popup that gets animated </summary>
        public UIContainer Container;

        /// <summary> Internal variable used by the custom inspector to allow you to type a custom canvas name instead of selecting it from the database </summary>
        public bool CustomCanvasName;

        /// <summary> References to elements that can be customized (Labels, Images and Buttons) </summary>
        public UIPopupContentReferences Data;

        /// <summary> Should this UIPopup get automatically destroyed after being hidden (default: TRUE) </summary>
        public bool DestroyAfterHide;

        /// <summary> Determines if this UIPopup should be displayed on the PopupCanvas or on a specific target canvas </summary>
        public PopupDisplayOn DisplayTarget;

        /// <summary> Behavior when this UIPopup gets hidden (goes off screen) </summary>
        public UIPopupBehavior HideBehavior = new UIPopupBehavior(AnimationType.Hide);

        /// <summary> If TRUE, clicking any referenced UIButton will hide (close) this UIPopup (default: FALSE) </summary>
        public bool HideOnAnyButton = false;

        /// <summary> If TRUE, the next 'Back' button event will hide (close) this UIPopup (default: TRUE) </summary>
        public bool HideOnBackButton = true;

        /// <summary> If TRUE, the next click (anywhere on screen) will hide (close) this UIPopup (default: FALSE) </summary>
        public bool HideOnClickAnywhere = false;

        /// <summary> If TRUE, the next click (on the Container) will hide (close) this UIPopup (default: TRUE) </summary>
        public bool HideOnClickContainer = true;

        /// <summary> If TRUE, the next click (on the Overlay) will hide (close) this UIPopup (default: TRUE) </summary>
        public bool HideOnClickOverlay = true;

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) while hiding this UIPopup </summary>
        public Progressor HideProgressor;

        /// <summary>
        ///     Callback executed when the popup is animating (showing or hiding) and the progress has been updated.
        ///     <para />
        ///     Passes the InverseVisibility (float between 1 - NotVisible and 0 - Visible)
        ///     <para />
        ///     InverseVisibility = 1 - Visibility
        /// </summary>
        public ProgressEvent OnInverseVisibilityChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the popup is animating (showing or hiding) and the progress has been updated.
        ///     <para />
        ///     Passes the Visibility (float between 0 - NotVisible and 1 - Visible)
        /// </summary>
        public ProgressEvent OnVisibilityChanged = new ProgressEvent();

        /// <summary>
        /// Settings for the Overlay UIContainer used to dim the screen when the this UIPopup is shown.
        /// <para/> This is the popup's background that gets faded in/out.
        /// <para/> Its effect is to tint the screen by being drawn (sorted) behind the UIContainer Container
        /// </summary>
        public UIContainer Overlay;

        /// <summary> Reference to the GameObject that should be selected after this UIPopup has been shown. Works only if AutoSelectButtonAfterShow is TRUE </summary>
        public GameObject SelectedButton;

        /// <summary> Behavior when this UIPopup gets shown (becomes visible on screen) </summary>
        public UIPopupBehavior ShowBehavior = new UIPopupBehavior(AnimationType.Show);

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) while showing this UIPopup </summary>
        public Progressor ShowProgressor;

        /// <summary> Should the HideProgressor get updated when this UIPopup is showing? </summary>
        public bool UpdateHideProgressorOnShow;

        /// <summary> Should the ShowProgressor get updated when this UIPopup is hiding? </summary>
        public bool UpdateShowProgressorOnHide;

        /// <summary> Should this UIPopup use the Overlay </summary>
        public bool UseOverlay = true;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the Canvas attached to this GameObject </summary>
        private Canvas m_canvas;

        /// <summary> Internal variable that holds a reference to the GraphicRaycaster attached to this GameObject </summary>
        private GraphicRaycaster m_graphicRaycaster;

        /// <summary>
        ///     Internal variable used to keep track of what button was selected by the EventSystem.current, before this popup was shown.
        ///     <para />
        ///     This is needed in order to restore the selection after this popup has been hidden.
        /// </summary>
        private GameObject m_previousSelectedButton;

        /// <summary> Internal variable that keeps track of the visibility of this UIPopup (float between 0 and 1) (NotVisible - Visible) </summary>
        /// a
        private float m_visibilityProgress = 1f;

        /// <summary> Internal variable that keeps track of this UIPopup's visibility state (Visible, NotVisible, Hiding or Showing) </summary>
        private VisibilityState m_visibilityState = VisibilityState.Visible;

        /// <summary> Internal variable that keeps track if this UIPopup has been added to the PopupQueue by the UIPopupManager </summary>
        private bool m_addedToQueue;

        /// <summary> Internal variable used to keep track of the coroutine used for when this UIPopup is shown </summary>
        private Coroutine m_showCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used for when this UIPopup is hidden </summary>
        private Coroutine m_hideCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used to auto hide this UIPopup after it was shown </summary>
        private Coroutine m_autoHideCoroutine;

        /// <summary> Internal variable used to keep track of the coroutine used to disable button clicks while this UIPopup is animating </summary>
        private Coroutine m_disableButtonClickCoroutine;

        /// <summary>
        ///     Internal array used to keep track of all the child UIButtons and mark them to update their start valued upon first interaction.
        ///     The update is done after this UIPopup has been shown
        /// </summary>
        private UIButton[] m_childUIButtons;

        /// <summary>
        ///     Internal variable used to keep track if this UIPopup has been hidden (after being instantiated).
        ///     It is needed because when an UIPopup hides, it notifies the UIPopupManager to remove it from the PopupQueue and, when it's hidden for the first time, it should not send that notification.
        /// </summary>
        private bool m_initialized;

        #endregion

        #region Unity Methods

        protected override void Reset()
        {
            base.Reset();
            UIPopupSettings.Instance.ResetComponent(this);
        }

        public override void Awake()
        {
            m_initialized = false;
            RectTransform.FullScreen(true);
            base.Awake();
            LoadPresets();
            Container.Init();
            Overlay.Init();
            Initialize();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            StopHide();
            StopShow();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Hide);
            UIAnimator.StopAnimations(RectTransform, AnimationType.Show);

            Container.ResetToStartValues();
            if (Overlay.Enabled) Overlay.ResetToStartValues();
            ResetToStartValues();
        }

        private void Update()
        {
            if (!DetectsTouch) return;             //this UIPopup's touch detectors are disabled -> return
            if (!Detector.TouchInProgress) return; //No touch is active -> return

            if (HideOnClickAnywhere)
            {
                Hide();
                return;
            }

            if (HasOverlay &&
                HideOnClickOverlay &&
                Detector.CurrentTouchInfo.GameObject == Overlay.RectTransform.gameObject)
            {
                Hide();
                return;
            }

            if (HasContainer &&
                HideOnClickContainer &&
                Detector.CurrentTouchInfo.GameObject == Container.RectTransform.gameObject)
                Hide();
        }

        #endregion

        #region Public Methods

        /// <summary> Cancel an auto hide, if it was initiated </summary>
        public void CancelAutoHide()
        {
            if (m_autoHideCoroutine == null) return;
            StopCoroutine(m_autoHideCoroutine);
            m_autoHideCoroutine = null;
            if (DebugComponent) DDebug.Log("Cancel Auto Hide", this);
        }

        /// <summary> Returns the TargetCanvas designated for this UIPopup </summary>
        public UICanvas GetTargetCanvas() { return GetTargetCanvas(DisplayTarget, CanvasName); }

        /// <summary> Hide this UIPopup after the set delay </summary>
        /// <param name="delay"> Time delay before this UIPopup is automatically hidden </param>
        public void Hide(float delay)
        {
            if (DebugComponent) DDebug.Log("Hide with a " + delay + " seconds delay.", this);
            m_autoHideCoroutine = StartCoroutine(HideWithDelayEnumerator(delay));
        }

        /// <summary> Hide this UIPopup </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void Hide(bool instantAction = false)
        {
            if (HideBehavior.InstantAnimation) instantAction = true;

            StopShow();

            if (!HideBehavior.Animation.Enabled && !instantAction)
            {
                DDebug.Log("You are trying to HIDE the (" + name + ") UIPopup, but you did not enable any HIDE animations. Enable at least one HIDE animation in order to fix this issue.", this);
                return; //no HIDE animations have been enabled -> cannot hide this UIPopup -> stop here
            }

            if (Visibility == VisibilityState.Hiding) return;
            if (!IsVisible)
            {
                if (VisiblePopups.Contains(this)) VisiblePopups.Remove(this);
                return;
            }

            if (DebugComponent) DDebug.Log("Hide", this);
            m_hideCoroutine = StartCoroutine(HideEnumerator(instantAction));
        }

        /// <summary> Hide this UIPopup instantly without triggering any of its actions </summary>
        public void InstantHide()
        {
            StopShow();
            StopHide();

            Container.ResetToStartValues();
            if (Overlay.Enabled) Overlay.ResetToStartValues();
            ResetToStartValues();

            Container.Disable(); //disable the gameobject, canvas and graphic raycaster - if their disable options are set to true
            Overlay.Disable();   //disable the gameobject, canvas and graphic raycaster - if their disable options are set to true

            Visibility = VisibilityState.NotVisible; //update the visibility state
            if (VisiblePopups.Contains(this)) VisiblePopups.Remove(this);

            RemoveHiddenFromVisiblePopups();

            if (!m_initialized) m_initialized = true;
        }

        /// <summary> Reset this UIPopup's DisplayTarget to the PopupCanvas (the default canvas used for all UIPopups) </summary>
        /// <param name="reparentImmediately"> Should this UIPopup also get re-parented to the PopupCanvas? </param>
        public void ResetTargetCanvasToPopupCanvas(bool reparentImmediately = true)
        {
            DisplayTarget = PopupDisplayOn.PopupCanvas;
            if (reparentImmediately) ReparentToPopupCanvas();
        }

        /// <summary> Show this UIPopup </summary>
        /// <param name="instantAction"> Should the animation happen instantly? (zero seconds) </param>
        public void Show(bool instantAction = false)
        {
            if (ShowBehavior.InstantAnimation) instantAction = true;

            ReparentToTargetCanvas();

            StopHide();

            if (!ShowBehavior.Animation.Enabled && !instantAction)
            {
                DDebug.Log("You are trying to SHOW the (" + name + ") UIPopup, but you did not enable any SHOW animations. Enable at least one SHOW animation in order to fix this issue.", this);
                return; //no SHOW animations have been enabled -> cannot show this UIPopup -> stop here
            }

            if (Visibility == VisibilityState.Showing) return;
            if (IsVisible)
            {
                if (!VisiblePopups.Contains(this)) VisiblePopups.Add(this);
                return; //this UIPopup is already visible -> stop here
            }

            if (DebugComponent) DDebug.Log("Show", this);
            m_showCoroutine = StartCoroutine(ShowEnumerator(instantAction));
        }

        /// <summary> Send an UIPopupMessage notifying the system that an UIPopupBehavior has been triggered </summary>
        /// <param name="animationType"> The AnimationType of the UIPopupBehavior that has been triggered </param>
        public void NotifySystemOfTriggeredBehavior(AnimationType animationType)
        {
            if (OnUIPopupAction != null) OnUIPopupAction.Invoke(this, animationType);
            Message.Send(new UIPopupMessage(this, animationType));
        }

        /// <summary> Set the PopupName property. Used by the UIPopupManager to identify this UIPopup </summary>
        /// <param name="popupName"> The popup name designation for this UIPopup </param>
        public void SetPopupName(string popupName)
        {
            PopupName = popupName;
            if (DebugComponent) DDebug.Log("Set PopupName: " + popupName, this);
        }

        /// <summary> Set a new target canvas name and also re-parents this UIPopup under it (if the option is enabled) </summary>
        /// <param name="canvasName"> The new target canvas name what will be used by this UIPopup when shown </param>
        /// <param name="reparentImmediately"> Should this UIPopup also get re-parented to the new target canvas? </param>
        public void SetTargetCanvasName(string canvasName, bool reparentImmediately = true)
        {
            DisplayTarget = PopupDisplayOn.TargetCanvas;
            CanvasName = canvasName;
            if (reparentImmediately) ReparentToTargetCanvas();
            if (DebugComponent) DDebug.Log("Set Target Canvas Name: " + canvasName, this);
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            SetPopupName(name + GetInstanceID());

            m_childUIButtons = GetComponentsInChildren<UIButton>();
            UpdateChildUIButtonsStartValues();

            InstantHide();
//            Hide(true);

            if (DebugComponent) DDebug.Log("Initialize", this);
        }

        private void LoadPresets()
        {
            if (ShowBehavior.LoadSelectedPresetAtRuntime) ShowBehavior.LoadPreset();
            if (HideBehavior.LoadSelectedPresetAtRuntime) HideBehavior.LoadPreset();

            if (DebugComponent) DDebug.Log("Load Presets", this);
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

        private void UpdateChildUIButtonsStartValues()
        {
            if (m_childUIButtons == null) return; //check that the m_childUIButtons array is not null and that we have a least one entry
            foreach (UIButton button in m_childUIButtons)
                button.UpdateStartValues();
        }

        private void UpdateOverlayAlpha(float value)
        {
            if (!HasOverlay || !Overlay.Enabled) return;
            Overlay.CanvasGroup.alpha = value;
        }

        private void ReparentToTargetCanvas()
        {
            transform.SetParent(GetTargetCanvas().transform);
            RectTransform.FullScreen(true);
        }

        private void ReparentToPopupCanvas()
        {
            transform.SetParent(GetPopupOverlayCanvas().transform);
            RectTransform.FullScreen(true);
        }

        #endregion

        #region IEnumerators

        // ReSharper disable once UnusedMember.Local
        private IEnumerator TriggerShowInNextFrame(bool instantAction)
        {
            yield return null; //skip a frame
            yield return null; //skip a frame

            Show(instantAction);
        }

        private IEnumerator ShowEnumerator(bool instantAction)
        {
            RectTransform.FullScreen(true);
            Overlay.FullScreen(true);

            yield return null; //skip a frame
            if (Settings.AutoDisableUIInteractions) DisableUIInteractions();

            UIAnimator.StopAnimations(Container.RectTransform, ShowBehavior.Animation.AnimationType); //stop any SHOW animations
            Container.Enable();                                                                       //enable the gameobject, canvas and graphic raycaster
            Overlay.Enable();

            //MOVE
            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(Container.RectTransform, ShowBehavior.Animation, Container.StartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(Container.RectTransform, ShowBehavior.Animation, Container.StartPosition);
            if (!ShowBehavior.Animation.Move.Enabled || instantAction) Container.ResetPosition();
            UIAnimator.Move(Container.RectTransform, ShowBehavior.Animation, moveFrom, moveTo, instantAction); //initialize and play the SHOW Move tween

            //ROTATE
            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(ShowBehavior.Animation, Container.StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(ShowBehavior.Animation, Container.StartRotation);
            if (!ShowBehavior.Animation.Rotate.Enabled || instantAction) Container.ResetRotation();
            UIAnimator.Rotate(Container.RectTransform, ShowBehavior.Animation, rotateFrom, rotateTo, instantAction); //initialize and play the SHOW Rotate tween

            //SCALE
            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(ShowBehavior.Animation, Container.StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(ShowBehavior.Animation, Container.StartScale);
            if (!ShowBehavior.Animation.Scale.Enabled || instantAction) Container.ResetScale();
            UIAnimator.Scale(Container.RectTransform, ShowBehavior.Animation, scaleFrom, scaleTo, instantAction); //initialize and play the SHOW Scale tween

            //FADE
            float fadeFrom = UIAnimator.GetAnimationFadeFrom(ShowBehavior.Animation, Container.StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(ShowBehavior.Animation, Container.StartAlpha);
            if (!ShowBehavior.Animation.Fade.Enabled || instantAction) Container.ResetAlpha();
            UIAnimator.Fade(Container.RectTransform, ShowBehavior.Animation, fadeFrom, fadeTo, instantAction); //initialize and play the SHOW Fade tween

            Visibility = VisibilityState.Showing; //update the visibility state
            if (!VisiblePopups.Contains(this)) VisiblePopups.Add(this);

            if (instantAction) ShowBehavior.OnStart.Invoke(gameObject, false, false);
            NotifySystemOfTriggeredBehavior(AnimationType.Show); //send the global events

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

                    VisibilityProgress = elapsedTime / totalDuration;
                    yield return null;
                }
            }

            ShowBehavior.OnFinished.Invoke(gameObject, !instantAction, !instantAction);

            Visibility = VisibilityState.Visible; //update the visibility state
            if (!VisiblePopups.Contains(this)) VisiblePopups.Add(this);

            if (AutoHideAfterShow) Hide(AutoHideAfterShowDelay);
            StartCoroutine(ExecuteShowSelectDeselectButtonEnumerator()); //select the selectedButton
            m_showCoroutine = null;                                      //clear the coroutine reference
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();

            if (HideOnAnyButton)
            {
                if (Data.HasButtons)
                {
                    foreach (UIButton button in Data.Buttons)
                    {
                        if (button == null) continue;
                        button.Button.onClick.RemoveAllListeners();
                        button.Button.onClick.AddListener(() => { Hide(); });
                    }
                }
            }

            RemoveHiddenFromVisiblePopups();
        }

        private IEnumerator HideEnumerator(bool instantAction)
        {
            RectTransform.FullScreen(true);
            Overlay.FullScreen(true);

//            yield return null; //skip a frame
            if (Settings.AutoDisableUIInteractions) DisableUIInteractions();

            UIAnimator.StopAnimations(Container.RectTransform, HideBehavior.Animation.AnimationType); //stop any HIDE animations

            //MOVE
            Vector3 moveFrom = UIAnimator.GetAnimationMoveFrom(Container.RectTransform, HideBehavior.Animation, Container.StartPosition);
            Vector3 moveTo = UIAnimator.GetAnimationMoveTo(Container.RectTransform, HideBehavior.Animation, Container.StartPosition);
            if (!HideBehavior.Animation.Move.Enabled || instantAction) Container.ResetPosition();
            UIAnimator.Move(Container.RectTransform, HideBehavior.Animation, moveFrom, moveTo, instantAction); //initialize and play the HIDE Move tween

            //ROTATE
            Vector3 rotateFrom = UIAnimator.GetAnimationRotateFrom(HideBehavior.Animation, Container.StartRotation);
            Vector3 rotateTo = UIAnimator.GetAnimationRotateTo(HideBehavior.Animation, Container.StartRotation);
            if (!HideBehavior.Animation.Rotate.Enabled || instantAction) Container.ResetRotation();
            UIAnimator.Rotate(Container.RectTransform, HideBehavior.Animation, rotateFrom, rotateTo, instantAction); //initialize and play the HIDE Rotate tween

            //SCALE
            Vector3 scaleFrom = UIAnimator.GetAnimationScaleFrom(HideBehavior.Animation, Container.StartScale);
            Vector3 scaleTo = UIAnimator.GetAnimationScaleTo(HideBehavior.Animation, Container.StartScale);
            if (!HideBehavior.Animation.Scale.Enabled || instantAction) Container.ResetScale();
            UIAnimator.Scale(Container.RectTransform, HideBehavior.Animation, scaleFrom, scaleTo, instantAction); //initialize and play the HIDE Scale tween

            //FADE
            float fadeFrom = UIAnimator.GetAnimationFadeFrom(HideBehavior.Animation, Container.StartAlpha);
            float fadeTo = UIAnimator.GetAnimationFadeTo(HideBehavior.Animation, Container.StartAlpha);
            if (!HideBehavior.Animation.Fade.Enabled || instantAction) Container.ResetAlpha();
            UIAnimator.Fade(Container.RectTransform, HideBehavior.Animation, fadeFrom, fadeTo, instantAction); //initialize and play the HIDE Fade tween

            StartCoroutine(ExecuteHideDeselectButtonEnumerator());
            Visibility = VisibilityState.Hiding; //update the visibility state

            if (m_initialized)
            {
                if (instantAction) HideBehavior.OnStart.Invoke(gameObject, false, false);
                NotifySystemOfTriggeredBehavior(AnimationType.Hide); //send the global events
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

                    VisibilityProgress = 1 - elapsedTime / totalDuration; //operation is reversed in hide than in show
                    yield return null;
                }

                yield return new WaitForSecondsRealtime(UIPopupSettings.DISABLE_WHEN_HIDDEN_TIME_BUFFER); //wait for seconds realtime (ignore Unity's Time.Timescale)
            }

            if (m_initialized)
            {
                HideBehavior.OnFinished.Invoke(gameObject, !instantAction, !instantAction);
            }

            Visibility = VisibilityState.NotVisible; //update the visibility state
            if (VisiblePopups.Contains(this)) VisiblePopups.Remove(this);

            Container.Disable(); //disable the gameobject, canvas and graphic raycaster - if their disable options are set to true
            Overlay.Disable();

            m_hideCoroutine = null; //clear the coroutine reference
            if (Settings.AutoDisableUIInteractions) EnableUIInteractions();

            RemoveHiddenFromVisiblePopups();

            if (!m_initialized)
            {
                m_initialized = true;
                yield break;
            }

            UIPopupManager.RemoveFromQueue(this);
            if (!DestroyAfterHide) yield break;
            yield return null;
            Destroy(gameObject);
        }

        private IEnumerator HideWithDelayEnumerator(float delay)
        {
            if (delay > 0) yield return new WaitForSecondsRealtime(delay);
            Hide();
            m_autoHideCoroutine = null;
        }

        private IEnumerator ExecuteShowSelectDeselectButtonEnumerator()
        {
            yield return null;                                                     //skip a frame
            m_previousSelectedButton = UnityEventSystem.currentSelectedGameObject; //save the currently selected button (so that the selection can be restored when the popup is hidden)

            if (AutoSelectButtonAfterShow && SelectedButton != null)    //check that the auto select option is enabled and that a selectedButton has been referenced
                UnityEventSystem.SetSelectedGameObject(SelectedButton); //select the referenced selectedButton
        }

        private IEnumerator ExecuteHideDeselectButtonEnumerator()
        {
            if (!AutoSelectPreviouslySelectedButtonAfterHide) yield break;
            yield return null; //skip a frame
            if (m_previousSelectedButton == null) yield break;
            UnityEventSystem.SetSelectedGameObject(m_previousSelectedButton); //select the previously selected button (before the popup was shown)
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Looks in the UIPopupManager PopupsDatabase for an UIPopup prefab linked to the given popup name.
        /// If found, it instantiates a clone of it and returns a reference to it. Otherwise it returns null
        /// </summary>
        /// <param name="popupName"> Popup name to search for </param>
        public static UIPopup GetPopup(string popupName) { return UIPopupManager.GetPopup(popupName); }

        /// <summary> Returns the UICanvas named 'PopupCanvas' and sets its render mode to ScreenSpaceOverlay </summary>
        public static UICanvas GetPopupOverlayCanvas()
        {
            UICanvas canvas = UICanvas.GetUICanvas(DEFAULT_POPUP_CANVAS_NAME, true);
            canvas.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.Canvas.sortingOrder = DEFAULT_POPUP_CANVAS_OVERLAY_SORT_ORDER;
            return canvas;
        }

        /// <summary> Returns either the UICanvas named 'PopupCanvas' (if PopupDisplayOn.PopupCanvas) or another UICanvas with the target canvas name </summary>
        /// <param name="popupDisplayOn"> Where should the UIPopup be shown </param>
        /// <param name="targetCanvasName"> Target canvas name (used if PopupDisplayOn.TargetCanvas) </param>
        public static UICanvas GetTargetCanvas(PopupDisplayOn popupDisplayOn, string targetCanvasName) { return popupDisplayOn == PopupDisplayOn.PopupCanvas ? GetPopupOverlayCanvas() : UICanvas.GetUICanvas(targetCanvasName); }

        /// <summary> Hides any UIPopup that has the given popup name </summary>
        /// <param name="popupName"> Popup name to search for </param>
        /// <param name="instantAction"> Determines if the animation should happen instantly (in zero seconds) </param>
        /// <returns></returns>
        public static bool HidePopup(string popupName, bool instantAction = false)
        {
            bool result = false;
            if (string.IsNullOrEmpty(popupName)) return false;
            if (!AnyPopupVisible) return false;

            for (int i = VisiblePopups.Count -1; i >= 0; i--)
            {
                var popup = VisiblePopups[i];

                if (popup.PopupName.Equals(popupName))
                {
                    popup.Hide(instantAction);
                    result = true; //a popup with the given popupName was found and hidden
                }
            }

            //ÐÞ¸´µü´úÊ±ÐÞ¸ÄÈÝÆ÷bug
            //foreach (UIPopup popup in VisiblePopups)
            //    if (popup.PopupName.Equals(popupName))
            //    {
            //        popup.Hide(instantAction);
            //        result = true; //a popup with the given popupName was found and hidden
            //    }

            if (DoozySettings.Instance.DebugUIPopup) DDebug.Log("Hide PopupName: " + popupName);

            return result;
        }

        /// <summary> Removes any UIPopups set as hidden from the VisiblePopups list </summary>
        private static void RemoveHiddenFromVisiblePopups()
        {
            RemoveNullsFromVisiblePopups();
            for (int i = VisiblePopups.Count - 1; i >= 0; i--)
                if (VisiblePopups[i].IsHidden)
                    VisiblePopups.RemoveAt(i);
        }

        /// <summary> Removes any null entries from the VisiblePopups list </summary>
        private static void RemoveNullsFromVisiblePopups()
        {
            for (int i = VisiblePopups.Count - 1; i >= 0; i--)
                if (VisiblePopups[i] == null)
                    VisiblePopups.RemoveAt(i);
        }

        #endregion
    }
}