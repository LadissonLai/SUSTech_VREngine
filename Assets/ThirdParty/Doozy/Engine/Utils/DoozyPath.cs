// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

// ReSharper disable InconsistentNaming
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Utils
{
    [Serializable]
    public class DoozyPath : ScriptableObject
    {
        private const string ASSETS_PATH = "Assets/";

        private const string DATA = "Data";
        private const string DATABASE = "Database";
        private const string DOOZY = "Doozy";
        private const string EDITOR = "Editor";
        private const string ENGINE = "Engine";
        private const string FONTS = "Fonts";
        private const string GUI = "GUI";
        private const string IMAGES = "Images";
        private const string INTERNAL = "Internal";
        private const string NODY = "Nody";
        private const string RESOURCES = "Resources";
        private const string SETTINGS = "Settings";
        private const string SKINS = "Skins";
        private const string SOUNDY = "Soundy";
        private const string THEMES = "Themes";
        private const string TEMPLATES = "Templates";
        private const string TOUCHY = "Touchy";
        private const string UI = "UI";
        private const string UIBUTTON = "UIButton";
        private const string UICANVAS = "UICanvas";
        private const string UIDRAWER = "UIDrawer";
        private const string UIPOPUP = "UIPopup";
        private const string UITOGGLE = "UITOGGLE";
        private const string UIVIEW = "UIView";
        private const string UTILS = "Utils";

        public const string UIANIMATIONS = "UIAnimations";
        private const string HIDE = "Hide";
        private const string LOOP = "Loop";
        private const string PUNCH = "Punch";
        private const string SHOW = "Show";
        private const string STATE = "State";

        public const string SOUNDY_DATABASE = SOUNDY + DATABASE;
        public const string THEMES_DATABASE = THEMES + DATABASE;
        public const string UIBUTTON_DATABASE = UIBUTTON + DATABASE;
        public const string UICANVAS_DATABASE = UICANVAS + DATABASE;
        public const string UIDRAWER_DATABASE = UIDRAWER + DATABASE;
        public const string UIPOPUP_DATABASE = UIPOPUP + DATABASE;
        public const string UIVIEW_DATABASE = UIVIEW + DATABASE;

        public static string DOOZY_PATH = BasePath;                          // -- Doozy/
        public static string EDITOR_PATH = Path.Combine(DOOZY_PATH, EDITOR); // -- Doozy/Editor/
        public static string ENGINE_PATH = Path.Combine(DOOZY_PATH, ENGINE); // -- Doozy/Engine/

        public static string EDITOR_FONTS_PATH = Path.Combine(EDITOR_PATH, FONTS);       // -- Doozy/Editor/Fonts/
        public static string EDITOR_GUI_PATH = Path.Combine(EDITOR_PATH, GUI);           // -- Doozy/Editor/GUI/
        public static string EDITOR_IMAGES_PATH = Path.Combine(EDITOR_PATH, IMAGES);     // -- Doozy/Editor/Images/
        public static string EDITOR_INTERNAL_PATH = Path.Combine(EDITOR_PATH, INTERNAL); // -- Doozy/Editor/Internal/
        public static string EDITOR_SETTINGS_PATH = Path.Combine(EDITOR_PATH, SETTINGS); // -- Doozy/Editor/Settings/
        public static string EDITOR_SKINS_PATH = Path.Combine(EDITOR_PATH, SKINS);       // -- Doozy/Editor/Skins/

        public static string EDITOR_NODY_PATH = Path.Combine(EDITOR_PATH, NODY);                   // -- Doozy/Editor/Nody/
        public static string EDITOR_NODY_IMAGES_PATH = Path.Combine(EDITOR_NODY_PATH, IMAGES);     // -- Doozy/Editor/Nody/Images/
        public static string EDITOR_NODY_SKINS_PATH = Path.Combine(EDITOR_NODY_PATH, SKINS);       // -- Doozy/Editor/Nody/Skins/
        public static string EDITOR_NODY_SETTINGS_PATH = Path.Combine(EDITOR_NODY_PATH, SETTINGS); // -- Doozy/Editor/Nody/Settings/
        public static string EDITOR_NODY_UTILS_PATH = Path.Combine(EDITOR_NODY_PATH, UTILS);       // -- Doozy/Editor/Nody/Utils/

        public static string ENGINE_NODY_PATH = Path.Combine(ENGINE_PATH, NODY);                     // -- Doozy/Engine/Nody/
        public static string ENGINE_NODY_RESOURCES_PATH = Path.Combine(ENGINE_NODY_PATH, RESOURCES); // -- Doozy/Engine/Nody/Resources/

        public static string ENGINE_SOUNDY_PATH = Path.Combine(ENGINE_PATH, SOUNDY);                     // -- Doozy/Engine/Soundy/
        public static string ENGINE_SOUNDY_RESOURCES_PATH = Path.Combine(ENGINE_SOUNDY_PATH, RESOURCES); // -- Doozy/Engine/Soundy/Resources/

        public static string ENGINE_TOUCHY_PATH = Path.Combine(ENGINE_PATH, TOUCHY);                     // -- Doozy/Engine/Touchy/
        public static string ENGINE_TOUCHY_RESOURCES_PATH = Path.Combine(ENGINE_TOUCHY_PATH, RESOURCES); // -- Doozy/Engine/Touchy/Resources/

        public static string ENGINE_THEMES_PATH = Path.Combine(ENGINE_PATH, THEMES);                     // -- Doozy/Engine/Themes/
        public static string ENGINE_THEMES_RESOURCES_PATH = Path.Combine(ENGINE_THEMES_PATH, RESOURCES); // -- Doozy/Engine/Themes/Resources/

        public static string ENGINE_RESOURCES_PATH = Path.Combine(ENGINE_PATH, RESOURCES);                             // -- Doozy/Engine/Resources/
        public static string ENGINE_RESOURCES_DATA_PATH = Path.Combine(ENGINE_RESOURCES_PATH, DATA);                   // -- Doozy/Engine/Resources/Data/
        public static string ENGINE_RESOURCES_DATA_SOUNDY_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, SOUNDY);     // -- Doozy/Engine/Resources/Data/Soundy/
        public static string ENGINE_RESOURCES_DATA_UIBUTTON_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, UIBUTTON); // -- Doozy/Engine/Resources/Data/UIButton/
        public static string ENGINE_RESOURCES_DATA_UICANVAS_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, UICANVAS); // -- Doozy/Engine/Resources/Data/UICanvas/
        public static string ENGINE_RESOURCES_DATA_UIDRAWER_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, UIDRAWER); // -- Doozy/Engine/Resources/Data/UIDrawer/
        public static string ENGINE_RESOURCES_DATA_UIPOPUP_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, UIPOPUP);   // -- Doozy/Engine/Resources/Data/UIPopup/
        public static string ENGINE_RESOURCES_DATA_UIVIEW_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, UIVIEW);     // -- Doozy/Engine/Resources/Data/UIView/
        public static string ENGINE_RESOURCES_DATA_THEMES_PATH = Path.Combine(ENGINE_RESOURCES_DATA_PATH, THEMES);     // -- Doozy/Engine/Resources/Data/Themes/

        public enum ComponentName
        {
            Soundy,
            Themes,
            UIButton,
            UICanvas,
            UIDrawer,
            UIPopup,
            UIView
        }

        /// <summary> Returns -- Doozy/Engine/Resources/Data/{componentName}/ </summary>
        /// <param name="componentName"></param>
        public static string GetDataPath(ComponentName componentName) { return Path.Combine(ENGINE_RESOURCES_DATA_PATH, componentName.ToString()); }

        public static string ENGINE_UI_PATH = Path.Combine(ENGINE_PATH, UI);                     // -- Doozy/Engine/UI/
        public static string ENGINE_UI_RESOURCES_PATH = Path.Combine(ENGINE_UI_PATH, RESOURCES); // -- Doozy/Engine/UI/Resources/

        public static string UIANIMATIONS_RESOURCES_PATH = Path.Combine(ENGINE_UI_RESOURCES_PATH, UIANIMATIONS);   // -- Doozy/Engine/UI/Resources/UIAnimations/
        public static string HIDE_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, HIDE);   // -- Doozy/Engine/UI/Resources/UIAnimations/Hide/
        public static string LOOP_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, LOOP);   // -- Doozy/Engine/UI/Resources/UIAnimations/Loop/
        public static string PUNCH_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, PUNCH); // -- Doozy/Engine/UI/Resources/UIAnimations/Punch/
        public static string SHOW_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, SHOW);   // -- Doozy/Engine/UI/Resources/UIAnimations/Show/
        public static string STATE_UIANIMATIONS_RESOURCES_PATH = Path.Combine(UIANIMATIONS_RESOURCES_PATH, STATE); // -- Doozy/Engine/UI/Resources/UIAnimations/State/

        public static string UIBUTTON_PATH = Path.Combine(ENGINE_UI_PATH, UIBUTTON);           // -- Doozy/Engine/UI/UIButton/
        public static string UIBUTTON_RESOURCES_PATH = Path.Combine(UIBUTTON_PATH, RESOURCES); // -- Doozy/Engine/UI/UIButton/Resources/

        public static string UICANVAS_PATH = Path.Combine(ENGINE_UI_PATH, UICANVAS);           // -- Doozy/Engine/UI/UICanvas/
        public static string UICANVAS_RESOURCES_PATH = Path.Combine(UICANVAS_PATH, RESOURCES); // -- Doozy/Engine/UI/UICanvas/Resources/

        public static string UIDRAWER_PATH = Path.Combine(ENGINE_UI_PATH, UIDRAWER);           // -- Doozy/Engine/UI/UIDrawer/
        public static string UIDRAWER_RESOURCES_PATH = Path.Combine(UIDRAWER_PATH, RESOURCES); // -- Doozy/Engine/UI/UIDrawer/Resources/

        public static string UIPOPUP_PATH = Path.Combine(ENGINE_UI_PATH, UIPOPUP);           // -- Doozy/Engine/UI/UIPopup/
        public static string UIPOPUP_RESOURCES_PATH = Path.Combine(UIPOPUP_PATH, RESOURCES); // -- Doozy/Engine/UI/UIPopup/Resources/

        public static string UIVIEW_PATH = Path.Combine(ENGINE_UI_PATH, UIVIEW);           // -- Doozy/Engine/UI/UIView/
        public static string UIVIEW_RESOURCES_PATH = Path.Combine(UIVIEW_PATH, RESOURCES); // -- Doozy/Engine/UI/UIView/Resources/

        public static string UITOGGLE_PATH = Path.Combine(ENGINE_UI_PATH, UITOGGLE);           // -- Doozy/Engine/UI/UIToggle/
        public static string UITOGGLE_RESOURCES_PATH = Path.Combine(UITOGGLE_PATH, RESOURCES); // -- Doozy/Engine/UI/UIToggle/Resources/

        public static string ENGINE_UTILS_PATH = Path.Combine(ENGINE_PATH, UTILS); // -- Doozy/Engine/Utils/

        private static string s_basePath;

        public static string BasePath
        {
            get
            {
#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(s_basePath)) return s_basePath;
                var obj = CreateInstance<DoozyPath>();
                UnityEditor.MonoScript s = UnityEditor.MonoScript.FromScriptableObject(obj);
                string assetPath = UnityEditor.AssetDatabase.GetAssetPath(s);
                DestroyImmediate(obj);
                var fileInfo = new FileInfo(assetPath);
                UnityEngine.Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
                UnityEngine.Debug.Assert(fileInfo.Directory.Parent != null, "fileInfo.Directory.Parent != null");
                DirectoryInfo baseDir = fileInfo.Directory.Parent.Parent;
                UnityEngine.Debug.Assert(baseDir != null, "baseDir != null");
                Assert.AreEqual(DOOZY, baseDir.Name);
                string baseDirPath = baseDir.ToString().Replace('\\', '/');
                int index = baseDirPath.LastIndexOf(ASSETS_PATH, StringComparison.Ordinal);
                Assert.IsTrue(index >= 0);
                baseDirPath = baseDirPath.Substring(index);
                s_basePath = baseDirPath;
                return s_basePath;
#else
                return "";
#endif
            }
        }

        public static void CreateMissingFolders(bool silentMode = false)
        {
#if UNITY_EDITOR
//            DoozyUtils.ClearProgressBar();

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_PATH, 0.1f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_PATH, RESOURCES); // -- Doozy/Engine/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_PATH, 0.11f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_PATH, DATA); // -- Doozy/Engine/Resources/Data/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_SOUNDY_PATH, 0.12f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_SOUNDY_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, SOUNDY); // -- Doozy/Engine/Resources/Data/Soundy/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_UIBUTTON_PATH, 0.13f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_UIBUTTON_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, UIBUTTON); // -- Doozy/Engine/Resources/Data/UIButton/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_UICANVAS_PATH, 0.14f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_UICANVAS_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, UICANVAS); // -- Doozy/Engine/Resources/Data/UICanvas/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_UIDRAWER_PATH, 0.15f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_UIDRAWER_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, UIDRAWER); // -- Doozy/Engine/Resources/Data/UIDrawer/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_UIPOPUP_PATH, 0.16f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_UIPOPUP_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, UIPOPUP); // -- Doozy/Engine/Resources/Data/UIPopup/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_UIVIEW_PATH, 0.17f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_UIVIEW_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, UIVIEW); // -- Doozy/Engine/Resources/Data/UIView

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_RESOURCES_DATA_THEMES_PATH, 0.18f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_RESOURCES_DATA_THEMES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_RESOURCES_DATA_PATH, THEMES); // -- Doozy/Engine/Resources/Data/Themes

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_NODY_RESOURCES_PATH, 0.2f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_NODY_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_NODY_PATH, RESOURCES); // -- Doozy/Engine/Nody/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_SOUNDY_RESOURCES_PATH, 0.25f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_SOUNDY_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_SOUNDY_PATH, RESOURCES); // -- Doozy/Engine/Soundy/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_TOUCHY_RESOURCES_PATH, 0.3f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_TOUCHY_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_TOUCHY_PATH, RESOURCES); // -- Doozy/Engine/Touchy/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_THEMES_RESOURCES_PATH, 0.35f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_THEMES_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_THEMES_PATH, RESOURCES); // -- Doozy/Engine/Themes/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", ENGINE_UI_RESOURCES_PATH, 0.4f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(ENGINE_UI_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_UI_PATH, RESOURCES); // -- Doozy/Engine/UI/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UIANIMATIONS_RESOURCES_PATH, 0.5f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(ENGINE_UI_RESOURCES_PATH, UIANIMATIONS); // -- Doozy/Engine/UI/Resources/UIAnimations/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", HIDE_UIANIMATIONS_RESOURCES_PATH, 0.51f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(HIDE_UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIANIMATIONS_RESOURCES_PATH, HIDE); // -- Doozy/Engine/UI/Resources/UIAnimations/Hide/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", LOOP_UIANIMATIONS_RESOURCES_PATH, 0.52f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(LOOP_UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIANIMATIONS_RESOURCES_PATH, LOOP); // -- Doozy/Engine/UI/Resources/UIAnimations/Loop/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", PUNCH_UIANIMATIONS_RESOURCES_PATH, 0.53f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(PUNCH_UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIANIMATIONS_RESOURCES_PATH, PUNCH); // -- Doozy/Engine/UI/Resources/UIAnimations/Punch/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", SHOW_UIANIMATIONS_RESOURCES_PATH, 0.54f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(SHOW_UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIANIMATIONS_RESOURCES_PATH, SHOW); // -- Doozy/Engine/UI/Resources/UIAnimations/Show/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", STATE_UIANIMATIONS_RESOURCES_PATH, 0.55f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(STATE_UIANIMATIONS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIANIMATIONS_RESOURCES_PATH, STATE); // -- Doozy/Engine/UI/Resources/UIAnimations/State/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UIBUTTON_RESOURCES_PATH, 0.61f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UIBUTTON_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIBUTTON_PATH, RESOURCES); // -- Doozy/Engine/UI/UIButton/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UICANVAS_RESOURCES_PATH, 0.62f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UICANVAS_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UICANVAS_PATH, RESOURCES); // -- Doozy/Engine/UI/UICanvas/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UIDRAWER_RESOURCES_PATH, 0.63f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UIDRAWER_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIDRAWER_PATH, RESOURCES); // -- Doozy/Engine/UI/UIDrawer/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UIPOPUP_RESOURCES_PATH, 0.64f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UIPOPUP_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIPOPUP_PATH, RESOURCES); // -- Doozy/Engine/UI/UIPopup/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UITOGGLE_RESOURCES_PATH, 0.65f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UITOGGLE_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UITOGGLE_PATH, RESOURCES); // -- Doozy/Engine/UI/UIToggle/Resources/

            if (!silentMode) DoozyUtils.DisplayProgressBar("Validate Folder", UIVIEW_RESOURCES_PATH, 0.66f);
            if (!UnityEditor.AssetDatabase.IsValidFolder(UIVIEW_RESOURCES_PATH)) UnityEditor.AssetDatabase.CreateFolder(UIVIEW_PATH, RESOURCES); // -- Doozy/Engine/UI/UIView/Resources/

            DoozyUtils.ClearProgressBar();
#endif
        }
    }
}