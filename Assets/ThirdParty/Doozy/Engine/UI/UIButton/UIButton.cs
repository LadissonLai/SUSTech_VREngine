// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
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
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Core component in the DoozyUI system.
    ///     It contains all the logic needed for a button to work and behave in various ways in order to create complex UI interactions.
    /// </summary>
    [AddComponentMenu(MenuUtils.UIButton_AddComponentMenu_MenuName, MenuUtils.UIButton_AddComponentMenu_Order)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.UIBUTTON)]
    public class UIButton : UIComponentBase<UIButton>, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.UIButton_MenuItem_ItemName, false, MenuUtils.UIButton_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { CreateUIButton(GetParent(menuCommand.context as GameObject)); }

        /// <summary> (EDITOR ONLY) Creates a UIButton and returns a reference to it </summary>
        public static UIButton CreateUIButton(GameObject parent)
        {
            var go = new GameObject(MenuUtils.UIButton_GameObject_Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(UIButton));
            GameObjectUtility.SetParentAndAlign(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            var uiButton = go.GetComponent<UIButton>();
            uiButton.RectTransform.Center(true);
            uiButton.RectTransform.sizeDelta = new Vector2(UIButtonSettings.DEFAULT_BUTTON_WIDTH, UIButtonSettings.DEFAULT_BUTTON_HEIGHT);
            uiButton.Button.targetGraphic = go.GetComponent<Image>();

#if dUI_TextMeshPro
            CreateTextMeshProLabel(uiButton);
#else
            CreateTextLabel(uiButton);
#endif

            Selection.activeObject = go; //select the newly created gameObject

            return uiButton;
        }

        /// <summary> [Editor Only] Creates a TextMeshPro label </summary>
        public static void CreateTextMeshProLabel(UIButton button)
        {
#if dUI_TextMeshPro
            var label = new GameObject("Label", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, button.gameObject);

            var textMeshProLabel = label.AddComponent<TextMeshProUGUI>();
            textMeshProLabel.text = MenuUtils.UIButton_GameObject_Name;
            textMeshProLabel.color = DoozyUtils.TextColor;
            textMeshProLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textMeshProLabel.alignment = TextAlignmentOptions.Center;

            label.GetComponent<RectTransform>().FullScreen(true);

            button.TextMeshProLabel = textMeshProLabel;
            button.TargetLabel = TargetLabel.TextMeshPro;
#endif
        }

        /// <summary> [Editor Only] Creates a Text label </summary>
        public static void CreateTextLabel(UIButton button)
        {
            var label = new GameObject("Label", typeof(RectTransform));
            GameObjectUtility.SetParentAndAlign(label, button.gameObject);

            var textLabel = label.AddComponent<Text>();
            textLabel.text = MenuUtils.UIButton_GameObject_Name;
            textLabel.color = DoozyUtils.TextColor;
            textLabel.fontSize = DoozyUtils.TEXT_FONT_SIZE;
            textLabel.resizeTextForBestFit = false;
            textLabel.resizeTextMinSize = 12;
            textLabel.resizeTextMaxSize = 20;
            textLabel.alignment = TextAnchor.MiddleCenter;
            textLabel.alignByGeometry = true;
            textLabel.supportRichText = true;

            label.GetComponent<RectTransform>().FullScreen(true);

            button.TextLabel = textLabel;
            button.TargetLabel = TargetLabel.Text;
        }
#endif

        #endregion

        #region Static Properties

        /// <summary> Default UIButton button name for a 'Back' button </summary>
        public static string BackButtonName { get { return BackButton.NAME; } }

        /// <summary> Default UIButton custom button category name </summary>
        public static string CustomButtonCategory { get { return NamesDatabase.CUSTOM; } }

        /// <summary> Default UIButton button category name  </summary>
        public static string DefaultButtonCategory { get { return NamesDatabase.GENERAL; } }

        /// <summary> Default UIButton button name </summary>
        public static string DefaultButtonName { get { return NamesDatabase.UNNAMED; } }

        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        /// <summary> Action invoked whenever a UIButtonBehavior is triggered </summary>
        public static Action<UIButton, UIButtonBehaviorType> OnUIButtonAction = delegate { };

        #endregion

        #region Properties

        /// <summary> Reference to the Button component </summary>
        public Button Button
        {
            get
            {
                if (m_button != null) return m_button;
                m_button = GetComponent<Button>();
                return m_button;
            }
        }

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

        /// <summary> Returns TRUE if the Button component is interactable. This also toggles the interactability of this UIButton </summary>
        public bool Interactable { get { return Button.interactable; } set { Button.interactable = value; } }

        /// <summary> Returns TRUE if this UIButton has the 'Back' button name </summary>
        public bool IsBackButton { get { return ButtonName.Equals(BackButtonName); } }

        /// <summary> Returns TRUE if this UIButton is selected, by checking the EventSystem.current.currentSelectedGameObject </summary>
        public bool IsSelected { get { return UnityEventSystem != null && UnityEventSystem.currentSelectedGameObject == gameObject; } }

        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        /// <summary> If TRUE, the start values get updated when the next interaction happens </summary>
        public bool UpdateStartValuesRequired { get { return m_updateStartValuesRequired; } set { m_updateStartValuesRequired = value; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugUIButton; } }

        #endregion

        #region Public Variables

        /// <summary> If TRUE, this button can be spam clicked and it won't get disabled. If FALSE, after each click, this button will get disabled for DisableButtonBetweenClicksInterval value </summary>
        public bool AllowMultipleClicks;

        /// <summary> UIButton category name </summary>
        public string ButtonCategory;

        /// <summary> UIButton button name </summary>
        public string ButtonName;

        /// <summary> [OnClick Only] Determines if on click is triggered instantly or after it checks if it's a double click or not. Depending on your use case, you might need the Instant or Delayed mode. Default is set to Instant </summary>
        public SingleClickMode ClickMode;

        /// <summary> If TRUE, after this button has been clicked, double clicked or long clicked, it will get automatically deselected </summary>
        public bool DeselectButtonAfterClick;

        /// <summary> If AllowMultipleClicks is FALSE, then this interval determines for how long this button gets disabled after each click, double click or long click </summary>
        public float DisableButtonBetweenClicksInterval;

        /// <summary> [OnDoubleClick Only] Time interval used to register a double click. This is the time interval calculated between two sequential clicks to determine if either a double click, or two separate clicks occurred </summary>
        public float DoubleClickRegisterInterval;

        /// <summary> Allows the button to react to keys or virtual buttons (only when selected) </summary>
        public InputData InputData = new InputData();

        /// <summary> [OnLongClick Only] Time interval used to register a long click. This is the time interval a button has to be pressed down to be considered a long click </summary>
        public float LongClickRegisterInterval;

        /// <summary> Behavior when the pointer enters (hovers in) over the button's area </summary>
        public UIButtonBehavior OnPointerEnter = new UIButtonBehavior(UIButtonBehaviorType.OnPointerEnter);

        /// <summary> Behavior when the pointer exits (hovers out) the button's area (happens only after OnPointerEnter) </summary>
        public UIButtonBehavior OnPointerExit = new UIButtonBehavior(UIButtonBehaviorType.OnPointerExit);

        /// <summary> Behavior when the pointer is down over the button </summary>
        public UIButtonBehavior OnPointerDown = new UIButtonBehavior(UIButtonBehaviorType.OnPointerDown);

        /// <summary> Behavior when the pointer is up over the button (happens only after OnPointerDown) </summary>
        public UIButtonBehavior OnPointerUp = new UIButtonBehavior(UIButtonBehaviorType.OnPointerUp);

        /// <summary> Behavior when the pointer performs a left mouse button click over the button </summary>
        public UIButtonBehavior OnClick = new UIButtonBehavior(UIButtonBehaviorType.OnClick);

        /// <summary> Behavior when the pointer performs a left mouse button double click over the button </summary>
        public UIButtonBehavior OnDoubleClick = new UIButtonBehavior(UIButtonBehaviorType.OnDoubleClick);

        /// <summary> Behavior when the pointer performs a left mouse button long click over the button </summary>
        public UIButtonBehavior OnLongClick = new UIButtonBehavior(UIButtonBehaviorType.OnLongClick);

        /// <summary> Behavior when the pointer performs a right mouse button click over the button </summary>
        public UIButtonBehavior OnRightClick = new UIButtonBehavior(UIButtonBehaviorType.OnRightClick);

        /// <summary> Behavior when the button gets selected </summary>
        public UIButtonBehavior OnSelected = new UIButtonBehavior(UIButtonBehaviorType.OnSelected);

        /// <summary> Behavior when the button gets deselected </summary>
        public UIButtonBehavior OnDeselected = new UIButtonBehavior(UIButtonBehaviorType.OnDeselected);

        /// <summary> Loop animation triggered when the button is not selected (in idle mode) </summary>
        public UIButtonLoopAnimation NormalLoopAnimation = new UIButtonLoopAnimation(ButtonLoopAnimationType.Normal);

        /// <summary> Loop animation triggered when the button gets selected (happens only after OnSelected UIButtonBehavior has been triggered) </summary>
        public UIButtonLoopAnimation SelectedLoopAnimation = new UIButtonLoopAnimation(ButtonLoopAnimationType.Selected);

        /// <summary> Determines what type of label this button has </summary>
        public TargetLabel TargetLabel;

        /// <summary> Reference to the Text component used as a label for this button </summary>
        public Text TextLabel;

#if dUI_TextMeshPro
        /// <summary> Reference to the TextMeshProUGUI component used as a label for this button. Available only if TextMeshPro support is enabled </summary>
        public TextMeshProUGUI TextMeshProLabel;
#endif

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the Button component </summary>
        private Button m_button;

        /// <summary> Internal variable that holds a reference to the CanvasGroup attached to this GameObject </summary>
        private CanvasGroup m_canvasGroup;

        /// <summary> (only for Behavior.OnDoubleClick) Internal variable that is marked as true after one click occured </summary>
        private bool m_clickedOnce;

        /// <summary> Internal variable that holds a reference to the coroutine that disables the button after click </summary>
        private Coroutine m_disableButtonCoroutine;

        /// <summary> (only for Behavior.OnDoubleClick) Internal variable used to calculate the time interval between two sequential clicks </summary>
        private float m_doubleClickTimeoutCounter;

        /// <summary> (only for Behavior.OnLongClick) Internal variable that is marked as true after the system determined that a long click occured </summary>
        private bool m_executedLongClick;

        /// <summary> (only for Behavior.OnLongClick) Internal variable used to store a reference to the Coroutine that determines if a long click occured or not </summary>
        private Coroutine m_longClickRegisterCoroutine;

        /// <summary> (only for Behavior.OnLongClick) Internal variable used to calculate how long was the button pressed </summary>
        private float m_longClickTimeoutCounter;

        /// <summary> Internal variable used to update the start values when the first interaction happens </summary>
        private bool m_updateStartValuesRequired;

        #endregion

        #region Unity Methods

        protected override void Reset()
        {
            base.Reset();

            UIButtonSettings.Instance.ResetComponent(this);

            ButtonCategory = DefaultButtonCategory;
            ButtonName = DefaultButtonName;

            //Disable Button
            m_disableButtonCoroutine = null;

            //Double Click
            DoubleClickRegisterInterval = UIButtonSettings.DOUBLE_CLICK_REGISTER_INTERVAL;
            m_clickedOnce = false;
            m_doubleClickTimeoutCounter = 0;

            //Long Click
            LongClickRegisterInterval = UIButtonSettings.LONG_CLICK_REGISTER_INTERVAL;
            m_longClickTimeoutCounter = 0;
            m_executedLongClick = false;
            m_longClickRegisterCoroutine = null;
        }

        public override void Awake()
        {
            base.Awake();
            LoadPresets();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (IsSelected) StartSelectedLoopAnimation();
            else StartNormalLoopAnimation();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            UIAnimator.StopAnimations(RectTransform, AnimationType.Punch);
            UIAnimator.StopAnimations(RectTransform, AnimationType.State);

            StopSelectedLoopAnimation();
            StopNormalLoopAnimation();

            ResetToStartValues();

            ReadyAllBehaviors();
            
            if (m_disableButtonCoroutine == null) return;
            StopCoroutine(m_disableButtonCoroutine);
            m_disableButtonCoroutine = null;
            EnableButton();
        }
        
        private void Update()
        {
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

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) { TriggerButtonBehavior(OnPointerEnter); }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) { TriggerButtonBehavior(OnPointerExit); }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) { TriggerButtonBehavior(OnPointerDown); }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) { TriggerButtonBehavior(OnPointerUp); }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    TriggerButtonBehavior(OnClick);
                    break;
                case PointerEventData.InputButton.Right:
                    TriggerButtonBehavior(OnRightClick);
                    break;
                case PointerEventData.InputButton.Middle: break;
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject) return;
            StopNormalLoopAnimation();
            if (!OnSelected.Enabled)
            {
                StartSelectedLoopAnimation();
                return;
            }

            TriggerButtonBehavior(OnSelected);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject != gameObject) return;
            StopSelectedLoopAnimation();
            if (!OnDeselected.Enabled)
            {
                StartNormalLoopAnimation();
                return;
            }

            TriggerButtonBehavior(OnDeselected);
        }

        #endregion

        #region Public Methods

        /// <summary> Deselect this button from the EventSystem (if selected) </summary>
        public void DeselectButton()
        {
            if (!IsSelected) return;
            UnityEventSystem.SetSelectedGameObject(null);
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary> Deselect this button from the EventSystem (if selected), after a set delay </summary>
        /// <param name="delay"> Time to wait before deselecting this button from the EventSystem </param>
        public void DeselectButton(float delay) { Coroutiner.Start(DeselectButtonEnumerator(delay)); }

        /// <summary> Sets Interactable property to FALSE </summary>
        public void DisableButton() { Interactable = false; }

        /// <summary> Disable the button for a set time duration </summary>
        /// <param name="duration"> How long will the button get disabled for </param>
        public void DisableButton(float duration)
        {
            if (!Interactable) return;
            DisableButton();
            m_disableButtonCoroutine = StartCoroutine(DisableButtonEnumerator(duration));
        }

        /// <summary> Sets Interactable to TRUE </summary>
        public void EnableButton() { Interactable = true; }

        /// <summary> Execute OnPointerEnter actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerEnter(bool debug = false)
        {
            if (!OnPointerEnter.Enabled)
            {
                StopNormalLoopAnimation();
                StopSelectedLoopAnimation();
                return;
            }

            PrintBehaviorDebugMessage(OnPointerEnter, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerEnter));
            if (OnPointerEnter.DisableInterval > 0) StartCoroutine(DisableButtonBehaviorEnumerator(OnPointerEnter));
            PrintBehaviorDebugMessage(OnPointerEnter, "executed", debug);
        }

        /// <summary> Execute OnPointerExit actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerExit(bool debug = false)
        {
            if (!OnPointerExit.Enabled) return;
            PrintBehaviorDebugMessage(OnPointerExit, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerExit));
            if (OnPointerExit.DisableInterval > 0) StartCoroutine(DisableButtonBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnPointerDown actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerDown(bool debug = false)
        {
            ResetLongClick(debug);
            if (OnLongClick.Enabled && Interactable) RegisterLongClick();
            if (!OnPointerDown.Enabled) return;
            PrintBehaviorDebugMessage(OnPointerDown, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerDown));
            PrintBehaviorDebugMessage(OnPointerDown, "executed", debug);
        }

        /// <summary> Execute OnPointerUp actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecutePointerUp(bool debug = false)
        {
            UnregisterLongClick();
            if (!OnPointerUp.Enabled) return;
            PrintBehaviorDebugMessage(OnPointerUp, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerUp));
            PrintBehaviorDebugMessage(OnPointerUp, "executed", debug);
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
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(OnClick));
                    PrintBehaviorDebugMessage(OnClick, "executed", debug);
                }

                if (!AllowMultipleClicks) DisableButton(DisableButtonBetweenClicksInterval);
            }

            if (Interactable || !OnPointerExit.Enabled || !OnPointerExit.Ready) return;
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnDoubleClick actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteDoubleClick(bool debug = false)
        {
            if (OnDoubleClick.Enabled)
            {
                PrintBehaviorDebugMessage(OnDoubleClick, "initiated", debug);
                if (Interactable)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(OnDoubleClick));
                    PrintBehaviorDebugMessage(OnDoubleClick, "executed", debug);
                }

                if (!AllowMultipleClicks) DisableButton(DisableButtonBetweenClicksInterval);
            }

            if (Interactable || !OnPointerExit.Enabled || !OnPointerExit.Ready) return;
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnLongClick actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteLongClick(bool debug = false)
        {
            if (OnLongClick.Enabled)
            {
                PrintBehaviorDebugMessage(OnLongClick, "initiated", debug);
                if (Interactable)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(OnLongClick));
                    PrintBehaviorDebugMessage(OnLongClick, "executed", debug);
                }

                if (!AllowMultipleClicks) DisableButton(DisableButtonBetweenClicksInterval);
            }

            if (Interactable || !OnPointerExit.Enabled || !OnPointerExit.Ready) return;
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnRightClick actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteRightClick(bool debug = false)
        {
            if (OnRightClick.Enabled)
            {
                PrintBehaviorDebugMessage(OnRightClick, "initiated", debug);
                if (Interactable)
                {
                    StartCoroutine(ExecuteButtonBehaviorEnumerator(OnRightClick));
                    PrintBehaviorDebugMessage(OnClick, "OnRightClick", debug);
                }

                if (!AllowMultipleClicks) DisableButton(DisableButtonBetweenClicksInterval);
            }

            if (Interactable || !OnPointerExit.Enabled || !OnPointerExit.Ready) return;
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnPointerExit));
            PrintBehaviorDebugMessage(OnPointerExit, "executed", debug);
        }

        /// <summary> Execute OnDeselected actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteOnButtonDeselected(bool debug = false)
        {
            if (!OnDeselected.Enabled) return;
            PrintBehaviorDebugMessage(OnDeselected, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnDeselected));
            if (OnDeselected.DisableInterval > 0) StartCoroutine(DisableButtonBehaviorEnumerator(OnDeselected));
            PrintBehaviorDebugMessage(OnDeselected, "executed", debug);
        }

        /// <summary> Execute OnSelected actions, if enabled </summary>
        /// <param name="debug"> Enables relevant debug messages to be printed to the console </param>
        public void ExecuteOnButtonSelected(bool debug = false)
        {
            if (!OnSelected.Enabled) return;
            PrintBehaviorDebugMessage(OnSelected, "initiated", debug);
            StartCoroutine(ExecuteButtonBehaviorEnumerator(OnSelected));
            if (OnSelected.DisableInterval > 0) StartCoroutine(DisableButtonBehaviorEnumerator(OnSelected));
            PrintBehaviorDebugMessage(OnSelected, "executed", debug);
        }

        /// <summary> Load all the preset animations, that are set to be loaded at runtime, for all the enabled behaviors </summary>
        public void LoadPresets()
        {
            if (OnPointerEnter.Enabled && OnPointerEnter.LoadSelectedPresetAtRuntime) OnPointerEnter.LoadPreset();
            if (OnPointerExit.Enabled && OnPointerExit.LoadSelectedPresetAtRuntime) OnPointerExit.LoadPreset();
            if (OnPointerDown.Enabled && OnPointerDown.LoadSelectedPresetAtRuntime) OnPointerDown.LoadPreset();
            if (OnPointerUp.Enabled && OnPointerUp.LoadSelectedPresetAtRuntime) OnPointerUp.LoadPreset();

            if (OnClick.Enabled && OnClick.LoadSelectedPresetAtRuntime) OnClick.LoadPreset();
            if (OnDoubleClick.Enabled && OnDoubleClick.LoadSelectedPresetAtRuntime) OnDoubleClick.LoadPreset();
            if (OnLongClick.Enabled && OnLongClick.LoadSelectedPresetAtRuntime) OnLongClick.LoadPreset();

            if (OnSelected.Enabled && OnSelected.LoadSelectedPresetAtRuntime) OnSelected.LoadPreset();
            if (OnDeselected.Enabled && OnDeselected.LoadSelectedPresetAtRuntime) OnDeselected.LoadPreset();

            if (NormalLoopAnimation.Enabled && NormalLoopAnimation.LoadSelectedPresetAtRuntime) NormalLoopAnimation.LoadPreset();
            if (SelectedLoopAnimation.Enabled && SelectedLoopAnimation.LoadSelectedPresetAtRuntime) SelectedLoopAnimation.LoadPreset();
        }

        /// <summary> Sends an UIButtonMessage notifying the system that an UIButtonBehavior has been triggered </summary>
        /// <param name="behaviorType"> The UIButtonBehaviorType of the UIButtonBehavior that has been triggered </param>
        public void NotifySystemOfTriggeredBehavior(UIButtonBehaviorType behaviorType)
        {
            if (OnUIButtonAction != null) OnUIButtonAction.Invoke(this, behaviorType);
            Message.Send(new UIButtonMessage(this, behaviorType));
        }

        /// <summary> Selects this button in the EventSystem </summary>
        public void SelectButton() { UnityEventSystem.SetSelectedGameObject(gameObject); }

        /// <summary> If this UIButton has a label referenced, its text will get updated to the given text value </summary>
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

        /// <summary> Starts playing the normal loop animation </summary>
        public void StartNormalLoopAnimation()
        {
            if (NormalLoopAnimation == null) return;
            if (!NormalLoopAnimation.Enabled) return;
            ResetToStartValues();
            NormalLoopAnimation.Start(RectTransform, StartPosition, StartRotation);
        }

        /// <summary> Start playing the selected loop animation </summary>
        public void StartSelectedLoopAnimation()
        {
            if (SelectedLoopAnimation == null) return;
            if (!SelectedLoopAnimation.Enabled) return;
            ResetToStartValues();
            SelectedLoopAnimation.Start(RectTransform, StartPosition, StartRotation);
        }

        /// <summary> Stops playing the normal loop animation </summary>
        public void StopNormalLoopAnimation()
        {
            if (NormalLoopAnimation == null) return;
            if (!NormalLoopAnimation.IsPlaying) return;
            NormalLoopAnimation.Stop(RectTransform);
            ResetToStartValues();
        }

        /// <summary> Stop playing the selected loop animation </summary>
        public void StopSelectedLoopAnimation()
        {
            if (SelectedLoopAnimation == null) return;
            if (!SelectedLoopAnimation.IsPlaying) return;
            SelectedLoopAnimation.Stop(RectTransform);
            ResetToStartValues();
        }

        #endregion

        #region Private Methods

        private void PrintBehaviorDebugMessage(UIButtonBehavior behavior, string action, bool debug = false)
        {
            if (DebugComponent || debug) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - " + action + ".", this);
        }

        private void TriggerButtonBehavior(UIButtonBehavior behavior, bool debug = false)
        {
            switch (behavior.BehaviorType)
            {
                case UIButtonBehaviorType.OnClick:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    InitiateClick();
                    break;
                case UIButtonBehaviorType.OnPointerEnter:
                    StopNormalLoopAnimation();
                    StopSelectedLoopAnimation();
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerEnter();
                    break;
                case UIButtonBehaviorType.OnPointerExit:
                    if (IsSelected) StartSelectedLoopAnimation();
                    else StartNormalLoopAnimation();
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerExit();
                    break;
                case UIButtonBehaviorType.OnPointerDown:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerDown();
                    break;
                case UIButtonBehaviorType.OnPointerUp:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecutePointerUp();
                    break;
                case UIButtonBehaviorType.OnRightClick:
                    if (!Interactable || UIInteractionsDisabled) return;
                    if (!behavior.Ready) return;
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecuteRightClick();
                    break;
                case UIButtonBehaviorType.OnSelected:
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecuteOnButtonSelected();
                    break;
                case UIButtonBehaviorType.OnDeselected:
                    if (behavior.Enabled) PrintBehaviorDebugMessage(behavior, "triggered", debug);
                    ExecuteOnButtonDeselected();
                    break;
            }
        }

        private void InitiateClick(bool debug = false)
        {
            if (m_executedLongClick)
            {
                ResetLongClick(debug);
                return;
            }

            StartCoroutine(RunOnClickEnumerator(debug));
        }

        private void ReadyAllBehaviors()
        {
            OnPointerEnter.Ready = true;
            OnPointerExit.Ready = true;
            OnPointerUp.Ready = true;
            OnPointerDown.Ready = true;
            OnClick.Ready = true;
            OnDoubleClick.Ready = true;
            OnLongClick.Ready = true;
            OnRightClick.Ready = true;
            OnSelected.Ready = true;
            OnDeselected.Ready = true;
        }
        
        private void RegisterLongClick(bool debug = false)
        {
            if (OnLongClick.Enabled) PrintBehaviorDebugMessage(OnLongClick, "registered", debug);
            if (m_executedLongClick) return;
            ResetLongClick(debug);
            m_longClickRegisterCoroutine = StartCoroutine(RunOnLongClickEnumerator(debug));
        }

        private void UnregisterLongClick(bool debug = false)
        {
            if (OnLongClick.Enabled) PrintBehaviorDebugMessage(OnLongClick, "unregistered", debug);
            if (m_executedLongClick) return;
            ResetLongClick(debug);
        }

        private void ResetLongClick(bool debug = false)
        {
            if (OnLongClick.Enabled) PrintBehaviorDebugMessage(OnLongClick, "reset", debug);
            m_executedLongClick = false;
            m_longClickTimeoutCounter = 0;
            if (m_longClickRegisterCoroutine == null) return;
            StopCoroutine(m_longClickRegisterCoroutine);
            m_longClickRegisterCoroutine = null;
        }

        // ReSharper disable once UnusedMember.Local
        private bool BehaviorEnabled(UIButtonBehaviorType behaviorType)
        {
            switch (behaviorType)
            {
                case UIButtonBehaviorType.OnClick:        return OnClick.Enabled;
                case UIButtonBehaviorType.OnDoubleClick:  return OnDoubleClick.Enabled;
                case UIButtonBehaviorType.OnLongClick:    return OnLongClick.Enabled;
                case UIButtonBehaviorType.OnRightClick:   return OnRightClick.Enabled;
                case UIButtonBehaviorType.OnPointerEnter: return OnPointerEnter.Enabled;
                case UIButtonBehaviorType.OnPointerExit:  return OnPointerExit.Enabled;
                case UIButtonBehaviorType.OnPointerDown:  return OnPointerDown.Enabled;
                case UIButtonBehaviorType.OnPointerUp:    return OnPointerUp.Enabled;
                case UIButtonBehaviorType.OnSelected:     return OnSelected.Enabled;
                case UIButtonBehaviorType.OnDeselected:   return OnDeselected.Enabled;
                default: throw new ArgumentOutOfRangeException("behaviorType", behaviorType, null);
            }
        }

        #endregion

        #region IEnumerators

        private IEnumerator DeselectButtonEnumerator(float delay)
        {
            if (Settings.IgnoreUnityTimescale)                  //check if the UI ignores Unity's Time.Timescale or not
                yield return new WaitForSecondsRealtime(delay); //wait for seconds realtime (ignore Unity's Time.Timescale)
            else
                yield return new WaitForSeconds(delay); //wait for seconds (respect Unity's Time.Timescale)
            DeselectButton();
        }

        private IEnumerator ExecuteButtonBehaviorEnumerator(UIButtonBehavior behavior)
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
                case UIButtonBehaviorType.OnClick:
                case UIButtonBehaviorType.OnDoubleClick:
                case UIButtonBehaviorType.OnLongClick:
                case UIButtonBehaviorType.OnRightClick:
                case UIButtonBehaviorType.OnPointerEnter:
                case UIButtonBehaviorType.OnPointerExit:
                case UIButtonBehaviorType.OnPointerDown:
                case UIButtonBehaviorType.OnPointerUp:
                    if (!Interactable || UIInteractionsDisabled) yield break;
                    break;
            }

            StopNormalLoopAnimation();
            StopSelectedLoopAnimation();
            behavior.PlayAnimation(this);
            behavior.OnTrigger.ExecuteEffect(behavior.OnTrigger.GetCanvas(gameObject));
            behavior.OnTrigger.InvokeAnimatorEvents();

            if (behavior.TriggerEventsAfterAnimation)
                yield return new WaitForSecondsRealtime(behavior.GetAnimationTotalDuration()); //wait for seconds realtime (ignore Unity's Time.Timescale)

            behavior.OnTrigger.SendGameEvents(gameObject);

            behavior.OnTrigger.InvokeAction(gameObject);
            behavior.OnTrigger.InvokeUnityEvent();

            if (IsBackButton &&
                (behavior.BehaviorType == UIButtonBehaviorType.OnClick ||
                 behavior.BehaviorType == UIButtonBehaviorType.OnDoubleClick ||
                 behavior.BehaviorType == UIButtonBehaviorType.OnLongClick))
            {
                BackButton.Instance.Execute();
            }
            else
            {
                NotifySystemOfTriggeredBehavior(behavior.BehaviorType);
            }


            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (behavior.BehaviorType)
            {
                case UIButtonBehaviorType.OnSelected:
                    StartSelectedLoopAnimation();
                    break;
                case UIButtonBehaviorType.OnDeselected:
                    StartNormalLoopAnimation();
                    break;
                case UIButtonBehaviorType.OnClick:
                case UIButtonBehaviorType.OnDoubleClick:
                case UIButtonBehaviorType.OnLongClick:
                case UIButtonBehaviorType.OnRightClick:
                    if (DeselectButtonAfterClick)
                    {
                        if (DebugComponent) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - Deselect Button.", this);
                        DeselectButton();
                    }

                    break;
                case UIButtonBehaviorType.OnPointerEnter:
                    if (behavior.SelectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - Select Button.", this);
                        SelectButton();
                    }

                    break;
                case UIButtonBehaviorType.OnPointerExit:
                    if (behavior.DeselectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - Deselect Button.", this);
                        DeselectButton();
                    }

                    if (IsSelected) StartSelectedLoopAnimation();
                    else StartNormalLoopAnimation();

                    break;
                case UIButtonBehaviorType.OnPointerDown:
                    if (behavior.SelectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - Select Button.", this);
                        SelectButton();
                    }

                    break;
                case UIButtonBehaviorType.OnPointerUp:
                    if (behavior.DeselectButton)
                    {
                        if (DebugComponent) DDebug.Log("(" + ButtonName + ") UIButton - " + behavior.BehaviorType + " - Deselect Button.", this);
                        DeselectButton();
                    }

                    break;
            }
        }

        private IEnumerator DisableButtonEnumerator(float duration)
        {
            DisableButton();
            if (Settings.IgnoreUnityTimescale)                     //check if the UI ignores Unity's Time.Timescale or not
                yield return new WaitForSecondsRealtime(duration); //wait for seconds realtime (ignore Unity's Time.Timescale)
            else
                yield return new WaitForSeconds(duration); //wait for seconds (respect Unity's Time.Timescale)
            EnableButton();
            m_disableButtonCoroutine = null;
        }

        private IEnumerator DisableButtonBehaviorEnumerator(UIButtonBehavior behavior)
        {
            behavior.Ready = false;
            if (Settings.IgnoreUnityTimescale)                                     //check if the UI ignores Unity's Time.Timescale or not
                yield return new WaitForSecondsRealtime(behavior.DisableInterval); //wait for seconds realtime (ignore Unity's Time.Timescale)
            else
                yield return new WaitForSeconds(behavior.DisableInterval); //wait for seconds (respect Unity's Time.Timescale)
            behavior.Ready = true;
        }

        private IEnumerator RunOnClickEnumerator(bool debug = false)
        {
            if (ClickMode == SingleClickMode.Instant) ExecuteClick(debug);

            if (!m_clickedOnce && m_doubleClickTimeoutCounter < DoubleClickRegisterInterval)
            {
                m_clickedOnce = true;
            }
            else
            {
                m_clickedOnce = false;
                yield break; //button is pressed twice -> don't allow the second function call to fully execute
            }

            yield return new WaitForEndOfFrame();

            while (m_doubleClickTimeoutCounter < DoubleClickRegisterInterval)
            {
                if (!m_clickedOnce)
                {
                    ExecuteDoubleClick(debug);
                    m_doubleClickTimeoutCounter = 0f;
                    m_clickedOnce = false;
                    yield break;
                }

                if (Settings.IgnoreUnityTimescale)                         //check if the UI ignores Unity's Time.Timescale or not
                    m_doubleClickTimeoutCounter += Time.unscaledDeltaTime; //increment counter by change in time between frames (ignore Unity's Time.Timescale)
                else
                    m_doubleClickTimeoutCounter += Time.deltaTime; //increment counter by change in time between frames (respect Unity's Time.Timescale)

                yield return null; //wait for the next frame
            }

            if (ClickMode == SingleClickMode.Delayed) ExecuteClick(debug);

            m_doubleClickTimeoutCounter = 0f;
            m_clickedOnce = false;
        }

        private IEnumerator RunOnLongClickEnumerator(bool debug = false)
        {
            while (m_longClickTimeoutCounter < LongClickRegisterInterval)
            {
                if (Settings.IgnoreUnityTimescale)                       //check if the UI ignores Unity's Time.Timescale or not
                    m_longClickTimeoutCounter += Time.unscaledDeltaTime; //increment counter by change in time between frames (ignore Unity's Time.Timescale)
                else
                    m_longClickTimeoutCounter += Time.deltaTime; //increment counter by change in time between frames (respect Unity's Time.Timescale)
                yield return null;
            }

            ExecuteLongClick(debug);
            m_executedLongClick = true;
        }

        #endregion
        
        #region Static Methods
        
        /// <summary>
        /// Returns a list of all the UIButton, registered in the UIButton.Database, with the given button category and button name.
        /// <para/> If no UIButton is found, it will return an empty list.
        /// </summary>
        /// <param name="buttonCategory"> UIButton category to search for</param>
        /// <param name="buttonName"> UIButton name to search for (found in the button category) </param>
        public static List<UIButton> GetButtons(string buttonCategory, string buttonName)
        {
            var views = new List<UIButton>(); //create temp list
            foreach (UIButton button in Database)
            {
                if (!button.ButtonCategory.Equals(buttonCategory)) continue;
                if (!button.ButtonName.Equals(buttonName)) continue;
                views.Add(button); //categories and names match -> add to list
            }

            return views; //return list
        }
        
        #endregion
    }
}