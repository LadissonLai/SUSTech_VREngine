// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UIDrawerSettings : ScriptableObject
    {
        public const string FILE_NAME = "UIDrawerSettings";
        private static string ResourcesPath { get { return DoozyPath.UIDRAWER_RESOURCES_PATH; } }

        private static UIDrawerSettings s_instance;
        public static UIDrawerSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UIDrawerSettings>(FILE_NAME, ResourcesPath, false, false);
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
            Instance.database = NamesDatabase.GetDatabase(DoozyPath.UIDRAWER_DATABASE, NamesDatabase.GetPath(NamesDatabaseType.UIDrawer));
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.DatabaseType = NamesDatabaseType.UIDrawer;
            Instance.database.SearchForUnregisteredDatabases(false);
            Instance.database.RefreshDatabase(false, false);
            Instance.SetDirty(true);
#endif
        }
        
        public const bool BLOCK_BACK_BUTTON_DEFAULT_VALUE = true;
        public const bool DETECT_GESTURES_DEFAULT_VALUE = true;
        public const bool HIDE_ON_BACK_BUTTON_DEFAULT_VALUE = true;
        public const bool USE_CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE = true;
        public const float CLOSE_SPEED_DEFAULT_VALUE = 10f;
        public const float OPEN_SPEED_DEFAULT_VALUE = 10f;
        public const SimpleSwipe CLOSE_DIRECTION_DEFAULT_VALUE = SimpleSwipe.Left;
        public const string RENAME_PREFIX_DEFAULT_VALUE = "Drawer - ";
        public const string RENAME_SUFFIX_DEFAULT_VALUE = "";
        public static Vector3 CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE = Vector3.zero;
        
        public SimpleSwipe CloseDirection = CLOSE_DIRECTION_DEFAULT_VALUE;
        public Vector3 CustomStartAnchoredPosition = CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
        public bool BlockBackButton = BLOCK_BACK_BUTTON_DEFAULT_VALUE;
        public bool HideOnBackButton = HIDE_ON_BACK_BUTTON_DEFAULT_VALUE;
        public bool DetectGestures = DETECT_GESTURES_DEFAULT_VALUE;
        public bool UseCustomStartAnchoredPosition = USE_CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
        public float CloseSpeed = CLOSE_SPEED_DEFAULT_VALUE;
        public float OpenSpeed = OPEN_SPEED_DEFAULT_VALUE;
        public string RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
        public string RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;
        
        private void Reset()
        {
            CloseDirection = CLOSE_DIRECTION_DEFAULT_VALUE;
            CloseSpeed = CLOSE_SPEED_DEFAULT_VALUE;
            CustomStartAnchoredPosition = CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
            BlockBackButton = BLOCK_BACK_BUTTON_DEFAULT_VALUE;
            HideOnBackButton = HIDE_ON_BACK_BUTTON_DEFAULT_VALUE;
            DetectGestures = DETECT_GESTURES_DEFAULT_VALUE;
            OpenSpeed = OPEN_SPEED_DEFAULT_VALUE;
            RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
            RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;
            UseCustomStartAnchoredPosition = USE_CUSTOM_START_ANCHORED_POSITION_DEFAULT_VALUE;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UIDrawer drawer)
        {
            drawer.CloseDirection = CloseDirection;
            drawer.CloseSpeed = CloseSpeed;
            drawer.CustomStartAnchoredPosition = CustomStartAnchoredPosition;
            drawer.BlockBackButton = BlockBackButton;
            drawer.HideOnBackButton = HideOnBackButton;
            drawer.DetectGestures = DetectGestures;
            drawer.OpenSpeed = OpenSpeed;
            drawer.UseCustomStartAnchoredPosition = UseCustomStartAnchoredPosition;
        }
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}