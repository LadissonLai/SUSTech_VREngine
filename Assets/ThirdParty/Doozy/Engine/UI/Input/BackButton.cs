// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Input
{
    /// <inheritdoc />
    /// <summary>
    ///     Listens for any 'Back' button related events and executes the 'Back' actions if they are enabled.
    /// </summary>
    [AddComponentMenu(MenuUtils.BackButton_AddComponentMenu_MenuName, MenuUtils.BackButton_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.BACK_BUTTON)]
    public class BackButton : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.BackButton_MenuItem_ItemName, false, MenuUtils.BackButton_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected BackButton() { }

        private static BackButton s_instance;

        /// <summary> Returns a reference to the BackButton in the Scene. If one does not exist, it gets created. </summary>
        public static BackButton Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (s_applicationIsQuitting) return null;
                s_instance = FindObjectOfType<BackButton>();
                // ReSharper disable once RedundantArgumentDefaultValue
                if (s_instance == null) DontDestroyOnLoad(AddToScene(false).gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Constants

        public const bool DEFAULT_ENABLE_ALTERNATE_INPUTS = false;
        public const float BACK_BUTTON_DETECTION_DISABLE_INTERVAL = 0.2f;
        public const InputMode DEFAULT_INPUT_MODE = InputMode.VirtualButton;
        public const KeyCode DEFAULT_BACK_BUTTON_KEY_CODE = KeyCode.Escape;
        public const KeyCode DEFAULT_BACK_BUTTON_KEY_CODE_ALT = KeyCode.Backspace;
        public const string DEFAULT_BACK_BUTTON_VIRTUAL_BUTTON_NAME = "Cancel";
        public const string DEFAULT_BACK_BUTTON_VIRTUAL_BUTTON_NAME_ALT = "Cancel";
        public const string NAME = "Back";

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool s_applicationIsQuitting;

        /// <summary> Internal variable that keeps track if this class has been initialized </summary>
        private static bool s_initialized;

        #endregion

        #region Properties

        /// <summary> Returns TRUE if the 'Back' button is disabled </summary>
        public bool BackButtonDisabled
        {
            get
            {
                if (m_backButtonDisableLevel < 0) m_backButtonDisableLevel = 0;

                return m_backButtonDisableLevel != 0;
            }
        }

        /// <summary>
        ///     Returns TRUE if the duration since the last time the 'Back' button was executed is greater or equal to the the BACK_BUTTON_DETECTION_DISABLE_INTERVAL.
        ///     <para />
        ///     This is needed and used not to allow for the 'Back' button execution to happen too fast (in a very short interval).
        ///     <para />
        ///     This blocks the user from spamming the 'Back' button and triggering the detection of an infinite loop (a fail safe) in the node graph.
        /// </summary>
        public bool CanExecuteBackButton { get { return Time.realtimeSinceStartup - m_lastBackButtonPressTime >= BACK_BUTTON_DETECTION_DISABLE_INTERVAL; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugBackButton; } }

        #endregion

        #region Public Variables

        /// <summary> 'Back' button input settings </summary>
        public InputData BackButtonInputData;

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        #endregion

        #region Private Variables

        /// <summary>
        ///     Internal variable used to keep track if the 'Back' button is disabled or not.
        ///     <para>This is an additive bool so if == 0 --> false (the 'Back' button is NOT disabled) and if > 0 --> true (the 'Back' button is disabled).</para>
        /// </summary>
        private int m_backButtonDisableLevel;

        /// <summary> Internal variable used to keep track when the 'Back' button was executed the last time </summary>
        private double m_lastBackButtonPressTime;

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            s_applicationIsQuitting = false;
            s_initialized = false;
        }
#endif
        
        private void Reset() { BackButtonInputData = GetBackButtonInputData(); }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two '" + GetType().Name + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);

            if (BackButtonInputData == null) BackButtonInputData = GetBackButtonInputData();
            m_lastBackButtonPressTime = Time.realtimeSinceStartup;

            s_initialized = true;
        }

        private void Update()
        {
            if (!CanExecuteBackButton) return;
            if (BackButtonDisabled) return;
            if (BackButtonInputData.InputMode == InputMode.None) return;
            bool triggerBackButton = false;
            switch (BackButtonInputData.InputMode)
            {
                case InputMode.KeyCode:
                    if (UnityEngine.Input.GetKeyDown(BackButtonInputData.KeyCode) ||
                        BackButtonInputData.EnableAlternateInputs && UnityEngine.Input.GetKeyDown(BackButtonInputData.KeyCodeAlt))
                    {
                        if (DebugComponent) DDebug.Log("Back button detected via KeyCode: " + BackButtonInputData.KeyCode, Instance);
                        triggerBackButton = true;
                    }

                    break;
                case InputMode.VirtualButton:
                    if (UnityEngine.Input.GetButtonDown(BackButtonInputData.VirtualButtonName) ||
                        BackButtonInputData.EnableAlternateInputs && UnityEngine.Input.GetButtonDown(BackButtonInputData.VirtualButtonNameAlt))
                    {
                        if (DebugComponent) DDebug.Log("Back button detected via Virtual Button: " + BackButtonInputData.VirtualButtonName, Instance);
                        triggerBackButton = true;
                    }

                    break;
            }

            if (!triggerBackButton) return;
            Execute();
        }

        private void OnApplicationQuit() { s_applicationIsQuitting = true; }

        #endregion

        #region Public Methods

        /// <summary> Fire the 'Back' button event (if it can be executed and is enabled) </summary>
        public void Execute()
        {
            if (!DoozySettings.Instance.UseBackButton) return;
            if (!CanExecuteBackButton) return;
            if (BackButtonDisabled) return;

            if (UIPopup.AnyPopupVisible)
            {
                UIPopup lastShownPopup = UIPopup.LastShownPopup;
                if (lastShownPopup.HideOnBackButton)
                    lastShownPopup.Hide();

                if (lastShownPopup.BlockBackButton)
                    return;
            }

            if (UIDrawer.AnyDrawerOpened)
            {
                UIDrawer openedDrawer = UIDrawer.OpenedDrawer;
                if (openedDrawer.HideOnBackButton)
                    openedDrawer.Close();

                if (openedDrawer.BlockBackButton)
                    return;
            }

            Message.Send(new UIButtonMessage(NAME, UIButtonBehaviorType.OnClick));
            m_lastBackButtonPressTime = Time.realtimeSinceStartup;
        }

        #endregion

        #region Static Methods

        /// <summary> Adds BackButton to scene and returns a reference to it </summary>
        public static BackButton AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<BackButton>(MenuUtils.BackButton_GameObject_Name, true, selectGameObjectAfterCreation); }

        /// <summary> Disable the 'Back' button functionality </summary>
        public static void Disable()
        {
            Instance.m_backButtonDisableLevel++; //if == 0 --> false (back button is not disabled) if > 0 --> true (back button is disabled)
        }

        /// <summary> Enable the 'Back' button functionality </summary>
        public static void Enable()
        {
            Instance.m_backButtonDisableLevel--; //if == 0 --> false (back button is not disabled) if > 0 --> true (back button is disabled)
            if (Instance.m_backButtonDisableLevel < 0) Instance.m_backButtonDisableLevel = 0;
        }

        /// <summary> Enable the 'Back' button functionality by resetting the additive bool to zero. backButtonDisableLevel = 0. Use this ONLY for special cases when something wrong happens and the back button is stuck in disabled mode </summary>
        public static void EnableByForce() { Instance.m_backButtonDisableLevel = 0; }

        /// <summary> Initialize the BackButton Instance </summary>
        public static void Init()
        {
            if (s_initialized || s_instance != null) return;
            s_instance = Instance;
        }

        private static InputData GetBackButtonInputData()
        {
            return new InputData
                   {
                       InputMode = DEFAULT_INPUT_MODE,
                       KeyCode = DEFAULT_BACK_BUTTON_KEY_CODE,
                       KeyCodeAlt = DEFAULT_BACK_BUTTON_KEY_CODE_ALT,
                       EnableAlternateInputs = DEFAULT_ENABLE_ALTERNATE_INPUTS,
                       VirtualButtonName = DEFAULT_BACK_BUTTON_VIRTUAL_BUTTON_NAME,
                       VirtualButtonNameAlt = DEFAULT_BACK_BUTTON_VIRTUAL_BUTTON_NAME_ALT
                   };
        }

        #endregion
    }
}