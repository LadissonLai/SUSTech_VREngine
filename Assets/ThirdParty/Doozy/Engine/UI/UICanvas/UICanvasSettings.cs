// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI.Settings
{
    [Serializable]
    public class UICanvasSettings : ScriptableObject
    {
        public const string FILE_NAME = "UICanvasSettings";
        private static string ResourcesPath { get { return DoozyPath.UICANVAS_RESOURCES_PATH; } }

        private static UICanvasSettings s_instance;

        public static UICanvasSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<UICanvasSettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        // ReSharper disable once InconsistentNaming
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
            Instance.database = NamesDatabase.GetDatabase(DoozyPath.UICANVAS_DATABASE, NamesDatabase.GetPath(NamesDatabaseType.UICanvas));
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.DatabaseType = NamesDatabaseType.UICanvas;
            Instance.database.SearchForUnregisteredDatabases(false);
            Instance.database.RefreshDatabase(false, false);
            Instance.SetDirty(true);
#endif
        }

        public const bool DONT_DESTROY_CANVAS_ON_LOAD_DEFAULT_VALUE = true;
        public const string RENAME_PREFIX_DEFAULT_VALUE = "Canvas - ";
        public const string RENAME_SUFFIX_DEFAULT_VALUE = "";

        public bool DontDestroyCanvasOnLoad = DONT_DESTROY_CANVAS_ON_LOAD_DEFAULT_VALUE;
        public string RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
        public string RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;

        private void Reset()
        {
            DontDestroyCanvasOnLoad = DONT_DESTROY_CANVAS_ON_LOAD_DEFAULT_VALUE;
            RenamePrefix = RENAME_PREFIX_DEFAULT_VALUE;
            RenameSuffix = RENAME_SUFFIX_DEFAULT_VALUE;
        }

        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(UICanvas canvas) { canvas.DontDestroyCanvasOnLoad = DontDestroyCanvasOnLoad; }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}