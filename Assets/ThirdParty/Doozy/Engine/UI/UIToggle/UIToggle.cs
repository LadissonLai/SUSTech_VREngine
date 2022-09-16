// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Engine.Events;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Input;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if dUI_TextMeshPro
using TMPro;
#endif
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoPropertyWhenPossible
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system.
    ///     It contains all the logic needed for a toggle to work and behave in various ways in order to create complex UI interactions.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIToggle_AddComponentMenu_MenuName, MenuUtils.UIToggle_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Toggle))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UITOGGLE)]
    public class UIToggle : UIComponentBase<UIToggle>, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIToggle_MenuItem_ItemName, false, MenuUtils.UIToggle_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand)
        {
            var go = new GameObject(MenuUtils.UIToggle_GameObject_Name, typeof(RectTransform), typeof(Toggle), typeof(UIToggle));
            GameObjectUtility.SetParentAndAlign(go, GetParent(menuCommand.context as GameObject));
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            var uiToggle = go.GetComponent<UIToggle>();
            uiToggle.RectTransform.Center(true);
            uiToggle.RectTransform.sizeDelta = new Vector2(UIToggleSettings.DEFAULT_BUTTON_WIDTH, UIToggleSettings.DEFAULT_BUTTON_HEIGHT);

            GameObject background = DoozyUtils.CreateBackgroundImage(go);
            var backgroundRectTransform = background.GetComponent<RectTransform>();
            backgroundRectTransform.anchorMin = new Vector2(0, 1);
            backgroundRectTransform.anchorMax = new Vector2(0, 1);
            backgroundRectTransform.anchoredPosition = new Vector2(10, -10);
            backgroundRectTransform.sizeDelta = new Vector2(20, 20);
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = UnityResources.UISprite;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.fillCenter = true;

            var checkmark = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            GameObjectUtility.SetParentAndAlign(checkmark, background);
            var checkmarkRectTransform = checkmark.GetComponent<RectTransform>();
            checkmarkRectTransform.ResetLocalScaleToOne();
            checkmarkRectTransform.AnchorMinToZero();
            checkmarkRectTransform.AnchorMaxToOne();
            checkmarkRectTransform.anchoredPosition = new Vector2(0, 0);
            checkmarkRectTransform.SizeDeltaToZero();
            checkmarkRectTransform.CenterPivot();
            var checkmarkImage = checkmark.GetComponent<Image>();
            checkmarkImage.sprite = UnityResources.Checkmark;
            checkmarkImage.type = Image.Type.Simple;
            checkmarkImage.fillCenter = true;
            checkmarkImage.color = DoozyUtils.CheckmarkColor;

#if dUI_TextMeshPro
            CreateTextMeshProLabel(uiToggle);
#else
            CreateTextLabel(uiToggle);
#endif

            uiToggle.Toggle.interactable = true;
            uiToggle.Toggle.transition = Selectable.Transition.ColorTint;
            uiToggle.Toggle.targetGraphic = backgroundImage;
            uiToggle.Toggle.graphic = checkmarkImage;
            uiToggle.Toggle.isOn = true;

            Selection.activeObject = go; //select the newly created gameObject
        }


        /// <summary> (EDITOR ONLY) Creates a TextMeshPro label </summary>
        public static void CreateTextMeshProLabel(UIToggle toggle)
        {
#if dUI_TextMeshPro
            var label = new GameObject("Label", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, toggle.gameObject);
            var labelRectTransform = label.GetComponent<RectTransform>();
            labelRectTransform.FullScreen(true);

            var textMeshProLabel = label.AddComponent<TextMeshProUGUI>();
            textMeshProLabel.text = MenuUtils.UIToggle_GameObject_Name;
            textMeshProLabel.color = DoozyUtils.TextColor;
            textMeshProLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textMeshProLabel.alignment = TextAlignmentOptions.Left;
            labelRectTransform.anchoredPosition = new Vector2(10, 0);
            labelRectTransform.sizeDelta = new Vector2(-28, 0);

            toggle.TextMeshProLabel = textMeshProLabel;
            toggle.TargetLabel = TargetLabel.TextMeshPro;
#endif
        }

        /// <summary> (EDITOR ONLY) Creates a Text label </summary>
        public static void CreateTextLabel(UIToggle toggle)
        {
            var label = new GameObject("Label", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, toggle.gameObject);
            var labelRectTransform = label.GetComponent<RectTransform>();
            labelRectTransform.FullScreen(true);

            var textLabel = label.AddComponent<Text>();
            textLabel.text = MenuUtils.UIToggle_GameObject_Name;
            textLabel.color = DoozyUtils.TextColor;
            textLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textLabel.resizeTextForBestFit = false;
            textLabel.resizeTextMinSize = 12;
            textLabel.resizeTextMaxSize = 20;
            textLabel.alignment = TextAnchor.MiddleLeft;
            textLabel.alignByGeometry = true;
            textLabel.supportRichText = true;
            labelRectTransform.anchoredPosition = new Vector2(10, -0.5f);
            labelRectTransform.sizeDelta = new Vector2(-28, -3);

            toggle.TextLabel = textLabel;
            toggle.TargetLabel = TargetLabel.Text;
        }

#endif

        #endregion

        #region Static Properties

        /// <summary> Action invoked whenever a UIToggleBehavior is triggered </summary>
        public static Action<UIToggle, UIToggleState, UIToggleBehaviorType> OnUIToggleAction = delegate { };

        #endregion

        #region Properties

        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (m_canvasGroup != null) return m_canvasGroup;
                m_canvasGroup = GetComponent<CanvasGroup>();
                if (m_canvasGroup != null) return m_canvasGroup;
                m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return m_canvasGroup;
            }
        }

        /// <summary>
        ///     TargetLabel.None -> Returns FALSE
        ///     <para />
        ///     TargetLabel.Text and TextLabel != null -> Returns TRUE
        ///     <para />
        ///     TargetLabel.TextMeshPro and TextMeshPro support is enabled and TextMeshProLabel != null -> Returns TRUE
        /// </summary>
        public bool HasLabel
        {
            get
            {
                switch (TargetLabel)
                {
                    case TargetLabel.None: return false;
                    case TargetLabel.Text: return TextLabel != null;
                    case TargetLabel.TextMeshPro:
#if dUI_TextMeshPro
                        return TextMeshProLabel != null;
#else
                        return false;
#endif

                    default: return false;
                }
            }
        }

        /// <summary> Returns TRUE if the button's Toggle component is interactable. This also toggles this toggle's interactability </summary>
        public bool Interactable { get { return Toggle.interactable; } set { Toggle.interactable = value; } }

        /// <summary> Returns TRUE if the Toggle is on and FALSE otherwise. This also toggles this Toggle's on/off state </summary>
        public bool IsOn { get { return Toggle.isOn; } set { Toggle.isOn = value; } }

        /// <summary> Returns TRUE if this UIToggle is selected, by checking the EventSystem.current.currentSelectedGameObject </summary>
        public bool IsSelected { get { return UnityEventSystem != null && UnityEventSystem.currentSelectedGameObject == gameObject; } }

        /// <summary> Reference to the Toggle component </summary>
        public Toggle Toggle
        {
            get
            {
                if (m_toggle != null) return m_toggle;
                m_toggle = GetComponent<Toggle>();
                return m_toggle;
            }
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary> If TRUE, the start values get updated when the next interaction happens </summary>
        public bool UpdateStartValuesRequired { get { return m_updateStartValuesRequired; } set { m_updateStartValuesRequired = value; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUIToggle; } }

        #endregion

        #region Public Variables

        /// <summary> If TRUE, this toggle can be spam clicked and it won't get disabled. If FALSE, after each click, this toggle will get disabled for DisableButtonBetweenClicksInterval value </summary>
        public bool AllowMultipleClicks;

        /// <summary> If AllowMultipleClicks is FALSE, then this interval determines for how long this toggle gets disabled after each click </summary>
        public float DisableButtonBetweenClicksInterval;

        /// <summary> If TRUE, after this toggle has been clicked it will get automatically deselected </summary>
        public bool DeselectButtonAfterClick;

        /// <summary> Allows the toggle to react to keys or virtual buttons (only when selected) </summary>
        public InputData InputData = new InputData();

        /// <summary> Behavior when the pointer enters (hovers in) over the toggle's area </summary>
        public UIToggleBehavior OnPointerEnter;

        /// <summary> Behavior when the pointer exits (hovers out) the toggle's area (happens only after OnPointerEnter) </summary>
        public UIToggleBehavior OnPointerExit;

        /// <summary> Behavior when the pointer performs a click over the toggle </summary>
        public UIToggleBehavior OnClick;

        /// <summary> Behavior when the toggle gets selected </summary>
        public UIToggleBehavior OnSelected;

        /// <summary> Behavior when the toggle gets deselected </summary>
        public UIToggleBehavior OnDeselected;

        /// <summary> Callback executed when the value of the toggle has changed </summary>
        public BoolEvent OnValueChanged = new BoolEvent();

        /// <summary> Determines what type of label this toggle has </summary>
        public TargetLabel TargetLabel;

        /// <summary> Reference to the Text component used as a label for this toggle </summary>
        public Text TextLabel;

#if dUI_TextMeshPro
        /// <summary> Reference to the TextMeshProUGUI component used as a label for this toggle. Available only if TextMeshPro support is enabled </summary>
        public TextMeshProUGUI TextMeshProLabel;
#endif

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) when this UIToggle has been toggled </summary>
        public Progressor ToggleProgressor;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the CanvasGroup attached to this GameObject </summary>
        private CanvasGroup m_canvasGroup;

        /// <summary> Internal variable that holds a reference to the coroutine that disables the button after click </summary>
        private Coroutine m_disableButtonCoroutine;

#pragma warning disable 0414
        /// <summary> Internal variable used to detect when a a toggle change has happened </summary>
        private bool m_previousValue;
#pragma warning restore 0414

        /// <summary> Internal variable that holds a reference to the Toggle component </summary>
        private Toggle m_toggle;

        /// <summary> Internal variable used to update the start values when the first interaction happens </summary>
        private bool m_updateStartValuesRequired;

        /// <summary> Internal variable used to keep track if this component has been initialized </summary>
        private bool m_initialized;

        #endregion

        #region Unity Methods

        protected override void Reset()
        {
            base.Reset();

            UIToggleSettings.Instance.ResetComponent(this);

            //Disable Button
            m_disableButtonCoroutine = null;
        }

        public override void Awake()
        {
            m_initialized = false;
            base.Awake();
            LoadPresets();
            m_previousValue = !IsOn;
            Toggle.onValueChanged.AddListener(ToggleOnValueChanged);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (m_initialized) return;
            if (!m_updateStartValuesRequired) //on the first interaction update the start values so that the reset method works as intended 
            {
                UpdateStartValues();
                m_updateStartValuesRequired = true;
            }

            OnClick.Invoke(this, false, false, false, true, true, true);
        }

        public override void Start()
        {
            base.Start();
            m_initialized = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Punch);
            UIAnimator.StopAnimations(RectTransform, AnimationType.State);

            ResetToStartValues();

            OnPointerEnter.Ready = true;
            OnPointerExit.Ready = true;
            OnClick.Ready = true;
            OnSelected.Ready = true;
            OnDeselected.Ready = true;

            if (m_disableButtonCoroutine == null) return;
            StopCoroutine(m_disableButtonCoroutine);
            m_disableButtonCoroutine = null;
            EnableToggle();

            m_previousValue = !IsOn;
        }

        private void Update()
        {
//            if (m_initialized && m_previousValue != IsOn)
//            {
//                m_previousValue = IsOn;
//                OnValueChanged.Invoke(IsOn);
//                Debug.Log("On value changed");
//                if (ToggleProgressor != null) ToggleProgressor.SetProgress(IsOn ? 1 : 0);
//            }

            if (InputData.InputMode == InputMode.None) return;

            if (!IsSelected) return;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (InputData.InputMode)
            {
                case InputMode.KeyCode:
                    if (UnityEngine.Input.GetKeyDown(InputData.KeyCode) ||
                        InputData.EnableAlternateInputs && UnityEngine.Input.GetKeyDown(InputData.KeyCodeAlt))
                        ExecuteClick();

                    break;
                case InputMode.VirtualButton:
                    if (UnityEngine.Input.GetButtonDown(InputData.VirtualButtonName) ||
                        InputData.EnableAlternateInputs && UnityEngine.Input.GetButtonDown(InputData.VirtualButtonNameAlt))
                        ExecuteClick();

                    break;
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) { TriggerToggleBehavior(OnPointerEnter); }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) { TriggerToggleBehavior(OnPointerExit); }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) { TriggerToggleBehavior(OnClick); }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject) return;
            if (!OnSelected.Enabled) return;

            TriggerToggleBehavior(OnSelected);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject) return;
            if (!OnDeselected.Enabled) return;

            TriggerToggleBehavior(OnDeselected);
        }

        #endregion

        #region Public Methods

        /// <summary> Deselect this toggle from the EventSystem (if selected) </summary>
        public void DeselectToggle()
        {
            if (!IsSelected) return;
            UnityEventSystem.SetSelectedGameObject(null);
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary> Deselect this toggle from the EventSystem (if selected), after a set delay </summary>
        /// <param name="delay"> Time to wait before deselecting this toggle from the EventSystem </param>
        public void DeselectToggle(float delay) { Coroutiner.Start(DeselectToggleEnumerator(delay)); }

        /// <summary> Set Interactable to FALSE </summary>
        public void DisableToggle() { Interactable = false; }

        /// <summary> Disable the toggle for a set time duration </summary>
        /// <param name="duration"> How long will the toggle get disabled for </param>
        public void DisableToggle(float duration)
        {
            if (!Interactable) return;
            DisableToggle();
            m_disableButtonCoroutine = StartCoroutine(DisableToggleEnumerator(duration));
        }

        /// <summary> Set Interactable to TRUE </summary>
        public void EnableToggle() { Interactable = true; }

        /// <summary> Execute OnPointerEnter actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerEnter(bool debug = false)
        {
            if (!OnPointerEnter.Enabled) return;
            PrintBehaviorDebugMessage(OnPointerEnter, "initiated", debug);
            StartCoroutine(ExecuteToggleBehaviorEnumerator(OnPointerEnter));
            if (OnPointerEnter.DisableInterval > 0) StartCoroutine(DisableToggleBehaviorEnumerator(OnPointerEnter));
            PrintBehaviorDebugMessage(OnPointerEnter, "executed", debug);
        }

        /// <summary> Execute OnPointerExit actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerExit(bool debug = false)
        {
            if (!OnPointerExit.Enabled) return;
            PrintBehaviorDebugMessage(OnPointerExit, "initiated", debug);
            StartCoroutine(ExecuteToggleBehaviorEnumerator(OnPointerExit));
            if (OnPointerExit.DisableInterval > 0) StartCoroutine(DisableToggleBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnClick actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteClick(bool debug = false)
        {
            if (OnClick.Enabled)
            {
                PrintBehaviorDebugMessage(OnClick, "initiated", debug);
                if (Interactable)
                {
                    StartCoroutine(ExecuteToggleBehaviorEnumerator(OnClick));
                    PrintBehaviorDebugMessage(OnClick, "executed", debug);
                }

                if (!AllowMultipleClicks) DisableToggle(DisableButtonBetweenClicksInterval);
            }

            if (Interactable || !OnPointerExit.Enabled || !OnPointerExit.Ready) return;
            StartCoroutine(ExecuteToggleBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnDeselected actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteOnButtonDeselected(bool debug = false)
        {
            if (!OnDeselected.Enabled) return;
            PrintBehaviorDebugMessage(OnDeselected, "initiated", debug);
            StartCoroutine(ExecuteToggleBehaviorEnumerator(OnDeselected));
            if (OnDeselected.DisableInterval > 0) StartCoroutine(DisableToggleBehaviorEnumerator(OnDeselected));
            PrintBehaviorDebugMessage(OnDeselected, "executed", debug);
        }

        /// <summary> Execute OnSelected actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteOnButtonSelected(bool debug = false)
        {
            if (!OnSelected.Enabled) return;
            PrintBehaviorDebugMessage(OnSelected, "initiated", debug);
            StartCoroutine(ExecuteToggleBehaviorEnumerator(OnSelected));
            if (OnSelected.DisableInterval > 0) StartCoroutine(DisableToggleBehaviorEnumerator(OnSelected));
            PrintBehaviorDebugMessage(OnSelected, "executed", debug);
        }

        /// <summary> Load all the preset animations, that are set to be loaded at runtime, for all the enabled behaviors </summary>
        public void LoadPresets()
        {
            if (OnPointerEnter.Enabled && OnPointerEnter.LoadSelectedPresetAtRuntime) OnPointerEnter.LoadPreset();
            if (OnPointerExit.Enabled && OnPointerExit.LoadSelectedPresetAtRuntime) OnPointerExit.LoadPreset();

            if (OnClick.Enabled && OnClick.LoadSelectedPresetAtRuntime) OnClick.LoadPreset();

            if (OnSelected.Enabled && OnSelected.LoadSelectedPresetAtRuntime) OnSelected.LoadPreset();
            if (OnDeselected.Enabled && OnDeselected.LoadSelectedPresetAtRuntime) OnDeselected.LoadPreset();
        }

        /// <summary> Send an UIToggleMessage notifying the system that an UIToggleBehavior has been triggered </summary>
        /// <param name="toggleState"> The toggle state the UIToggle that has been triggered </param>
        /// <param name="behaviorType"> The UIToggleBehaviorType of the UIToggleBehavior that has been triggered  </param>
        public void NotifySystemOfTriggeredBehavior(UIToggleState toggleState, UIToggleBehaviorType behaviorType)
        {
            if (OnUIToggleAction != null) OnUIToggleAction.Invoke(this, toggleState, behaviorType);
            Message.Send(new UIToggleMessage(this, toggleState, behaviorType));
        }

        /// <summary> Select this toggle in the EventSystem </summary>
        public void SelectToggle() { UnityEventSystem.SetSelectedGameObject(gameObject); }

        /// <summary> If this UIToggle has a label referenced, its text will get updated to the given text value </summary>
        /// <param name="text"> The new text value for the referenced label (if there is one) </param>
        public void SetLabelText(string text)
        {
            if (!HasLabel) return;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (TargetLabel)
            {
                case TargetLabel.Text:
                    TextLabel.text = text;
                    break;
                case TargetLabel.TextMeshPro:
#if dUI_TextMeshPro
                    TextMeshProLabel.text = text;
#endif
                    break;
            }
        }

        /// <summary> Toggle the toggle off. IsOn = false </summary>
        public void ToggleOff() { IsOn = false; }

        /// <summary> Toggle the toggle on. IsOn = true </summary>
        public void ToggleOn() { IsOn = true; }

        #endregion

        #region Private Methods

        private void PrintBehaviorDebugMessage(UIToggleBehavior behavior, string action, bool debug = false)
        {
            if (DebugComponent || debug) DDebug.Log("(" + name + ") - [" + (IsOn ? UIToggleState.On : UIToggleState.Off) + "]" + behavior.BehaviorType + " - " + action + ".", this);
        }

        private void ToggleOnValueChanged(bool value)
        {
            m_previousValue = IsOn;
            OnValueChanged.Invoke(IsOn);
            if (ToggleProgressor != null) ToggleProgressor.SetProgress(IsOn ? 1 : 0);
        }

        private void TriggerToggleBehavior(UIToggleBehavior behavior, bool debug = false)
        {
            switch (behavior.BehaviorType)
            {
                case UIToggleBehaviorType.OnClick:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered - ", debug);
                    ExecuteClick(debug);
                    break;
                case UIToggleBehaviorType.OnPointerEnter:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerEnter(debug);
                    break;
                case UIToggleBehaviorType.OnPointerExit:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerExit(debug);
                    break;
                case UIToggleBehaviorType.OnSelected:
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecuteOnButtonSelected(debug);
                    break;
                case UIToggleBehaviorType.OnDeselected:
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecuteOnButtonDeselected(debug);
                    break;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private bool BehaviorEnabled(UIToggleBehaviorType behaviorType)
        {
            switch (behaviorType)
            {
                case UIToggleBehaviorType.OnClick:        return OnClick.Enabled;
                case UIToggleBehaviorType.OnPointerEnter: return OnPointerEnter.Enabled;
                case UIToggleBehaviorType.OnPointerExit:  return OnPointerExit.Enabled;
                case UIToggleBehaviorType.OnSelected:     return OnSelected.Enabled;
                case UIToggleBehaviorType.OnDeselected:   return OnDeselected.Enabled;
                default:                                  throw new ArgumentOutOfRangeException("behaviorType", behaviorType, null);
            }
        }

        #endregion

        #region IEnumerators

        private IEnumerator DeselectToggleEnumerator(float delay)
        {
            yield return new WaitForSecondsRealtime(delay); //wait for seconds realtime (ignore Unity's Time.Timescale)
            DeselectToggle();
        }

        private IEnumerator ExecuteToggleBehaviorEnumerator(UIToggleBehavior behavior)
        {
            if (!behavior.Enabled) yield break;

            if (!m_updateStartValuesRequired) //on the first interaction update the start values so that the reset method works as intended 
            {
                UpdateStartValues();
                m_updateStartValuesRequired = true;
            }

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (behavior.BehaviorType)
            {
                case UIToggleBehaviorType.OnClick:
                case UIToggleBehaviorType.OnPointerEnter:
                case UIToggleBehaviorType.OnPointerExit:
                    if (!Interactable || UIInteractionsDisabled) yield break;
                    break;
            }

            behavior.Invoke(this);

            NotifySystemOfTriggeredBehavior(IsOn ? UIToggleState.On : UIToggleState.Off, behavior.BehaviorType);

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (behavior.BehaviorType)
            {
                case UIToggleBehaviorType.OnClick:
                    if (DeselectButtonAfterClick)
                    {
                        if (DebugComponent) DDebug.Log("(" + name + ") - [" + (IsOn ? UIToggleState.On : UIToggleState.Off) + "]" + behavior.BehaviorType + " - Deselect Button.", this);
                        DeselectToggle();
                    }

                    break;
                case UIToggleBehaviorType.OnPointerEnter:
                    if (behavior.SelectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + name + ") - [" + (IsOn ? UIToggleState.On : UIToggleState.Off) + "]" + behavior.BehaviorType + " - Select Button.", this);
                        SelectToggle();
                    }

                    break;
                case UIToggleBehaviorType.OnPointerExit:
                    if (behavior.DeselectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + name + ") - [" + (IsOn ? UIToggleState.On : UIToggleState.Off) + "]" + behavior.BehaviorType + " - Deselect Button.", this);
                        DeselectToggle();
                    }

                    break;
            }
        }

        private IEnumerator DisableToggleEnumerator(float duration)
        {
            DisableToggle();
            yield return new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
            EnableToggle();
            m_disableButtonCoroutine = null;
        }

        private IEnumerator DisableToggleBehaviorEnumerator(UIToggleBehavior behavior)
        {
            behavior.Ready = false;
            yield return new WaitForSecondsRealtime(behavior.DisableInterval); //wait for seconds realtime (ignore Unity's Time.Timescale)
            behavior.Ready = true;
        }

        #endregion
    }
}