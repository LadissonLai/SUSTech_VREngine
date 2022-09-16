// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UIToggleSettings : ScriptableObject
    {
        public const string FILE_NAME = "UIToggleSettings";
        private static string ResourcesPath { get { return DoozyPath.UITOGGLE_RESOURCES_PATH; } }

        private static UIToggleSettings s_instance;

        public static UIToggleSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UIToggleSettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        public const bool ALLOW_MULTIPLE_CLICKS_DEFAULT_VALUE = true;
        public const bool DESELECT_BUTTON_AFTER_CLICK_DEFAULT_VALUE = false;
        public const float BETWEEN_CLICKS_DISABLE_INTERVAL_DEFAULT_VALUE = 0.2f;
        public const float DEFAULT_BUTTON_HEIGHT = 20f;
        public const float DEFAULT_BUTTON_WIDTH = 160f;

        public InputMode InputMode = InputData.DEFAULT_INPUT_MODE;
        public KeyCode KeyCode = InputData.DEFAULT_ON_CLICK_KEY_CODE;
        public KeyCode KeyCodeAlt = InputData.DEFAULT_ON_CLICK_KEY_CODE_ALT;
        public bool AllowMultipleClicks = ALLOW_MULTIPLE_CLICKS_DEFAULT_VALUE;
        public bool DeselectButtonAfterClick = DESELECT_BUTTON_AFTER_CLICK_DEFAULT_VALUE;
        public bool EnableAlternateInputs = InputData.DEFAULT_ENABLE_ALTERNATE_INPUTS;
        public bool ShowOnButtonDeselected = true;
        public bool ShowOnButtonSelected = true;
        public bool ShowOnClick = true;
        public bool ShowOnPointerEnter = true;
        public bool ShowOnPointerExit = true;
        public float DisableButtonBetweenClicksInterval = BETWEEN_CLICKS_DISABLE_INTERVAL_DEFAULT_VALUE;
        public string VirtualButtonName = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME;
        public string VirtualButtonNameAlt = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT;

        private void Reset()
        {
            AllowMultipleClicks = ALLOW_MULTIPLE_CLICKS_DEFAULT_VALUE;
            DeselectButtonAfterClick = DESELECT_BUTTON_AFTER_CLICK_DEFAULT_VALUE;
            DisableButtonBetweenClicksInterval = BETWEEN_CLICKS_DISABLE_INTERVAL_DEFAULT_VALUE;
            EnableAlternateInputs = InputData.DEFAULT_ENABLE_ALTERNATE_INPUTS;
            InputMode = InputData.DEFAULT_INPUT_MODE;
            KeyCode = InputData.DEFAULT_ON_CLICK_KEY_CODE;
            KeyCodeAlt = InputData.DEFAULT_ON_CLICK_KEY_CODE_ALT;
            VirtualButtonName = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME;
            VirtualButtonNameAlt = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UIToggle toggle)
        {
            toggle.AllowMultipleClicks = AllowMultipleClicks;
            toggle.DeselectButtonAfterClick = DeselectButtonAfterClick;
            toggle.DisableButtonBetweenClicksInterval = DisableButtonBetweenClicksInterval;
            toggle.InputData = new InputData
                               {
                                   InputMode = InputMode,
                                   EnableAlternateInputs = EnableAlternateInputs,
                                   KeyCode = KeyCode,
                                   KeyCodeAlt = KeyCodeAlt,
                                   VirtualButtonName = VirtualButtonName,
                                   VirtualButtonNameAlt = VirtualButtonNameAlt
                               };
            toggle.OnClick = new UIToggleBehavior(UIToggleBehaviorType.OnClick, true);
            toggle.OnDeselected = new UIToggleBehavior(UIToggleBehaviorType.OnDeselected);
            toggle.OnPointerEnter = new UIToggleBehavior(UIToggleBehaviorType.OnPointerEnter);
            toggle.OnPointerExit = new UIToggleBehavior(UIToggleBehaviorType.OnPointerExit);
            toggle.OnSelected = new UIToggleBehavior(UIToggleBehaviorType.OnSelected);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}