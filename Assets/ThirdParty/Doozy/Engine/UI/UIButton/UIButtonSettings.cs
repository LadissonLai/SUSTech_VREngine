// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UIButtonSettings : ScriptableObject
    {
        public const string FILE_NAME = "UIButtonSettings";
        private static string ResourcesPath { get { return DoozyPath.UIBUTTON_RESOURCES_PATH; } }

        private static UIButtonSettings s_instance;

        public static UIButtonSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UIButtonSettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        [SerializeField] private NamesDatabase database;

        public static NamesDatabase Database
        {
            get
            {
                if (Instance.database != null) return Instance.database;
                UpdateDatabase();
                return Instance.database;
            }
        }

        public static void UpdateDatabase()
        {
            Instance.database = NamesDatabase.GetDatabase(DoozyPath.UIBUTTON_DATABASE, NamesDatabase.GetPath(NamesDatabaseType.UIButton));
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.DatabaseType = NamesDatabaseType.UIButton;
            Instance.database.SearchForUnregisteredDatabases(false);
            Instance.database.RefreshDatabase(false, false);
            Instance.SetDirty(true);
#endif
        }

        public const SingleClickMode DEFAULT_SINGLE_CLICK_MODE = SingleClickMode.Instant;
        public const bool DEFAULT_ALLOW_MULTIPLE_CLICKS = true;
        public const bool DEFAULT_DESELECT_BUTTON_AFTER_CLICK = false;
        public const float BETWEEN_CLICKS_DISABLE_INTERVAL = 0.2f;
        public const float DEFAULT_BUTTON_HEIGHT = 30f;
        public const float DEFAULT_BUTTON_WIDTH = 160f;
        public const float DOUBLE_CLICK_REGISTER_INTERVAL = 0.2f;
        public const float LONG_CLICK_REGISTER_INTERVAL = 0.5f;
        public const string DEFAULT_RENAME_PREFIX = "Button - ";
        public const string DEFAULT_RENAME_SUFFIX = "";

        public InputMode InputMode = InputData.DEFAULT_INPUT_MODE;
        public KeyCode KeyCode = InputData.DEFAULT_ON_CLICK_KEY_CODE;
        public KeyCode KeyCodeAlt = InputData.DEFAULT_ON_CLICK_KEY_CODE_ALT;
        public SingleClickMode ClickMode = DEFAULT_SINGLE_CLICK_MODE;
        public bool AllowMultipleClicks = DEFAULT_ALLOW_MULTIPLE_CLICKS;
        public bool DeselectButtonAfterClick = DEFAULT_DESELECT_BUTTON_AFTER_CLICK;
        public bool EnableAlternateInputs = InputData.DEFAULT_ENABLE_ALTERNATE_INPUTS;
        public bool ShowNormalLoopAnimation = true;
        public bool ShowOnButtonDeselected = true;
        public bool ShowOnButtonSelected = true;
        public bool ShowOnClick = true;
        public bool ShowOnDoubleClick = true;
        public bool ShowOnLongClick = true;
        public bool ShowOnRightClick = true;
        public bool ShowOnPointerDown = true;
        public bool ShowOnPointerEnter = true;
        public bool ShowOnPointerExit = true;
        public bool ShowOnPointerUp = true;
        public bool ShowSelectedLoopAnimation = true;
        public float DisableButtonBetweenClicksInterval = BETWEEN_CLICKS_DISABLE_INTERVAL;
        public string RenamePrefix = DEFAULT_RENAME_PREFIX;
        public string RenameSuffix = DEFAULT_RENAME_SUFFIX;
        public string VirtualButtonName = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME;
        public string VirtualButtonNameAlt = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT;

        private void Reset()
        {
            AllowMultipleClicks = DEFAULT_ALLOW_MULTIPLE_CLICKS;
            ClickMode = DEFAULT_SINGLE_CLICK_MODE;
            DeselectButtonAfterClick = DEFAULT_DESELECT_BUTTON_AFTER_CLICK;
            DisableButtonBetweenClicksInterval = BETWEEN_CLICKS_DISABLE_INTERVAL;
            EnableAlternateInputs = InputData.DEFAULT_ENABLE_ALTERNATE_INPUTS;
            InputMode = InputData.DEFAULT_INPUT_MODE;
            KeyCode = InputData.DEFAULT_ON_CLICK_KEY_CODE;
            KeyCodeAlt = InputData.DEFAULT_ON_CLICK_KEY_CODE_ALT;
            RenamePrefix = DEFAULT_RENAME_PREFIX;
            RenameSuffix = DEFAULT_RENAME_SUFFIX;
            VirtualButtonName = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME;
            VirtualButtonNameAlt = InputData.DEFAULT_ON_CLICK_VIRTUAL_BUTTON_NAME_ALT;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UIButton button)
        {
            button.AllowMultipleClicks = AllowMultipleClicks;
            button.ClickMode = ClickMode;
            button.DeselectButtonAfterClick = DeselectButtonAfterClick;
            button.DisableButtonBetweenClicksInterval = DisableButtonBetweenClicksInterval;
            button.InputData = new InputData
                               {
                                   InputMode = InputMode,
                                   EnableAlternateInputs = EnableAlternateInputs,
                                   KeyCode = KeyCode,
                                   KeyCodeAlt = KeyCodeAlt,
                                   VirtualButtonName = VirtualButtonName,
                                   VirtualButtonNameAlt = VirtualButtonNameAlt
                               };
            button.NormalLoopAnimation = new UIButtonLoopAnimation(ButtonLoopAnimationType.Normal);
            button.OnClick = new UIButtonBehavior(UIButtonBehaviorType.OnClick, true);
            button.OnDeselected = new UIButtonBehavior(UIButtonBehaviorType.OnDeselected);
            button.OnDoubleClick = new UIButtonBehavior(UIButtonBehaviorType.OnDoubleClick);
            button.OnLongClick = new UIButtonBehavior(UIButtonBehaviorType.OnLongClick);
            button.OnRightClick = new UIButtonBehavior(UIButtonBehaviorType.OnRightClick);
            button.OnPointerDown = new UIButtonBehavior(UIButtonBehaviorType.OnPointerDown);
            button.OnPointerEnter = new UIButtonBehavior(UIButtonBehaviorType.OnPointerEnter);
            button.OnPointerExit = new UIButtonBehavior(UIButtonBehaviorType.OnPointerExit);
            button.OnPointerUp = new UIButtonBehavior(UIButtonBehaviorType.OnPointerUp);
            button.OnSelected = new UIButtonBehavior(UIButtonBehaviorType.OnSelected);
            button.SelectedLoopAnimation = new UIButtonLoopAnimation(ButtonLoopAnimationType.Selected);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}