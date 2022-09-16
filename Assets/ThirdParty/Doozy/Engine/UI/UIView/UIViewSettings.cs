// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UIViewSettings : ScriptableObject
    {
        public const string FILE_NAME = "UIViewSettings";
        private static string ResourcesPath { get { return DoozyPath.UIVIEW_RESOURCES_PATH; } }

        private static UIViewSettings s_instance;

        public static UIViewSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UIViewSettings>(FILE_NAME, ResourcesPath, false, false);
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
            Instance.database = NamesDatabase.GetDatabase(DoozyPath.UIVIEW_DATABASE, NamesDatabase.GetPath(NamesDatabaseType.UIView));
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.DatabaseType = NamesDatabaseType.UIView;
            Instance.database.SearchForUnregisteredDatabases(false);
            Instance.database.RefreshDatabase(false, false);
            Instance.SetDirty(true);
#endif
        }

        public const TargetOrientation TARGET_ORIENTATION_DEFAULT_VALUE = TargetOrientation.Any;
        public const UIViewStartBehavior BEHAVIOUR_AT_START_DEFAULT_VALUE = UIViewStartBehavior.DoNothing;
        public const bool DEFAULT_AUTO_HIDE_AFTER_SHOW = false;
        public const bool DEFAULT_AUTO_SELECT_BUTTON_AFTER_SHOW = false;
        public const bool DESELECT_ANY_BUTTON_SELECTED_ON_HIDE_DEFAULT_VALUE = false;
        public const bool DESELECT_ANY_BUTTON_SELECTED_ON_SHOW_DEFAULT_VALUE = false;
        public const bool DISABLE_CANVAS_WHEN_HIDDEN_DEFAULT_VALUE = true;
        public const bool DISABLE_GAME_OBJECT_WHEN_HIDDEN_DEFAULT_VALUE = true;
        public const bool DISABLE_GRAPHIC_RAYCASTER_WHEN_HIDDEN_DEFAULT_VALUE = true;
        public const bool USE_CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE = true;
        public const float DEFAULT_AUTO_HIDE_AFTER_SHOW_DELAY = 3f;
        public const float DISABLE_WHEN_HIDDEN_TIME_BUFFER = 0.05f; //after an UIView has been hidden, the system will wait for an additional time buffer before it sets the Source's active state to false. This is a failsafe measure and fixes a small bug on iOS 
        public const string RENAME_PREFIX_DEFAULT_VALUE = "View - ";
        public const string RENAME_SUFFIX_DEFAULT_VALUE = "";
        public static Vector3 CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE = Vector3.zero;

        public TargetOrientation TargetOrientation;
        public UIViewStartBehavior BehaviorAtStart;
        public Vector3 CustomStartAnchoredPosition;
        public bool DeselectAnyButtonSelectedOnHide;
        public bool DeselectAnyButtonSelectedOnShow;
        public bool DisableCanvasWhenHidden;
        public bool DisableGameObjectWhenHidden;
        public bool DisableGraphicRaycasterWhenHidden;
        public bool UseCustomStartAnchoredPosition;
        public string RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
        public string RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;

        private void Reset()
        {
            BehaviorAtStart = BEHAVIOUR_AT_START_DEFAULT_VALUE;
            CustomStartAnchoredPosition = CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
            DeselectAnyButtonSelectedOnHide = DESELECT_ANY_BUTTON_SELECTED_ON_HIDE_DEFAULT_VALUE;
            DeselectAnyButtonSelectedOnShow = DESELECT_ANY_BUTTON_SELECTED_ON_SHOW_DEFAULT_VALUE;
            DisableCanvasWhenHidden = DISABLE_CANVAS_WHEN_HIDDEN_DEFAULT_VALUE;
            DisableGameObjectWhenHidden = DISABLE_GAME_OBJECT_WHEN_HIDDEN_DEFAULT_VALUE;
            DisableGraphicRaycasterWhenHidden = DISABLE_GRAPHIC_RAYCASTER_WHEN_HIDDEN_DEFAULT_VALUE;
            RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
            RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;
            TargetOrientation = TARGET_ORIENTATION_DEFAULT_VALUE;
            UseCustomStartAnchoredPosition = USE_CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UIView view)
        {
            view.AutoHideAfterShow = DEFAULT_AUTO_HIDE_AFTER_SHOW;
            view.AutoHideAfterShowDelay = DEFAULT_AUTO_HIDE_AFTER_SHOW_DELAY;
            view.AutoSelectButtonAfterShow = DEFAULT_AUTO_SELECT_BUTTON_AFTER_SHOW;
            view.BehaviorAtStart = BehaviorAtStart;
            view.CustomStartAnchoredPosition = CustomStartAnchoredPosition;
            view.DeselectAnyButtonSelectedOnHide = DeselectAnyButtonSelectedOnHide;
            view.DeselectAnyButtonSelectedOnShow = DeselectAnyButtonSelectedOnShow;
            view.DisableCanvasWhenHidden = DisableCanvasWhenHidden;
            view.DisableGameObjectWhenHidden = DisableGameObjectWhenHidden;
            view.DisableGraphicRaycasterWhenHidden = DisableGraphicRaycasterWhenHidden;
            view.HideBehavior = new UIViewBehavior(AnimationType.Hide);
            view.LoopBehavior = new UIViewBehavior(AnimationType.Loop);
            view.ShowBehavior = new UIViewBehavior(AnimationType.Show);
            view.TargetOrientation = TargetOrientation;
            view.UseCustomStartAnchoredPosition = UseCustomStartAnchoredPosition;
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}