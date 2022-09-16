// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Input
{
    /// <summary>
    /// Data container for individual component input settings. It is mostly used by the BackButton, the UIButton and the UIToggle components.
    /// </summary>
    [Serializable]
    public class InputData
    {
        #region Constants 

        public const bool DEFAULT_ENABLE_ALTERNATE_INPUTS = true;
        public const InputMode DEFAULT_INPUT_MODE = InputMode.VirtualButton;
        public const KeyCode DEFAULT_ON_CLICK_KEY_CODE = KeyCode.Return;
        public const KeyCode DEFAULT_ON_CLICK_KEY_CODE_ALT = KeyCode.Space;
        public const string DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME = "Submit";
        public const string DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT = "Jump";

        #endregion

        #region Public Variables

        /// <summary>
        ///     The enables the check for alternate inputs for the target component.
        ///     <para>If the InputMode is set to None, this option does nothing.</para>
        ///     <para>If the InputMode is set to KeyCode, this option enables the setting for an alternate button (KeyCode) to register a click/touch.</para>
        ///     <para>If the InputMode is set to VirtualButton, this option enabled the setting for an alternate virtual button (button name set in the InputManager) to register a click/touch.</para>
        /// </summary>
        public bool EnableAlternateInputs;

        /// <summary> The controller input mode. If set to InputMode.None, the target component will react only to mouse clicks and touches </summary>
        public InputMode InputMode;

        /// <summary> The on click key code </summary>
        public KeyCode KeyCode;

        /// <summary> The on click key code alternate </summary>
        public KeyCode KeyCodeAlt;

        /// <summary> The on click virtual button name (set int the InputManager) </summary>
        public string VirtualButtonName;

        /// <summary> The on click virtual button name alternate (set in the InputManager) </summary>
        public string VirtualButtonNameAlt;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        public InputData() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            InputMode = InputMode.VirtualButton;
            EnableAlternateInputs = false;
            KeyCode = DEFAULT_ON_CLICK_KEY_CODE;
            KeyCodeAlt = DEFAULT_ON_CLICK_KEY_CODE_ALT;
            VirtualButtonName = DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME;
            VirtualButtonNameAlt = DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT;
        }

        #endregion
    }
}