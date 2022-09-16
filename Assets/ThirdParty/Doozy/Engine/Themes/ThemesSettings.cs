// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Engine.Themes
{
    [Serializable]
    public class ThemesSettings : ScriptableObject
    {
        public const string FILE_NAME = "ThemesSettings";
        private static string ResourcesPath { get { return DoozyPath.ENGINE_THEMES_RESOURCES_PATH; } }

        private static ThemesSettings s_instance;
        public static ThemesSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<ThemesSettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        [SerializeField] private ThemesDatabase database;

        public static ThemesDatabase Database
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
            Instance.database = AssetUtils.GetScriptableObject<ThemesDatabase>("_" + DoozyPath.THEMES_DATABASE, DoozyPath.GetDataPath(DoozyPath.ComponentName.Themes), false, false);

#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.Initialize();
            Instance.database.SearchForUnregisteredThemes(false);
            Instance.SetDirty(true);
#endif
        }

        public const bool DEFAULT_AUTO_SAVE = true;
        
        public bool AutoSave = DEFAULT_AUTO_SAVE;

        private void Reset()
        {
            AutoSave = DEFAULT_AUTO_SAVE;
        }
        
        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}