// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Settings;
using Doozy.Editor.Utils;
using Doozy.Engine.Soundy;
using Doozy.Engine.Themes;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    [InitializeOnLoad]
    public static class DoozyAssetsProcessor
    {
        static DoozyAssetsProcessor()
        {
//            Debug.Log("Initialize On Load");
            ExecuteProcessor();
        }

//        [UnityEditor.Callbacks.DidReloadScripts(100)]
//        private static void OnScriptsReloaded()
//        {
////            Debug.Log("Scripts Reloaded");
//            ExecuteProcessor();
//        }
        
        private static void ExecuteProcessor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, ExecuteProcessor);
                return;
            }
            ProcessorsSettings.ResetInstance();
            if (!ProcessorsSettings.Instance.RunDoozyAssetsProcessor) return;
            DelayedCall.Run(3f, Run);
        }

        public static void Run()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                DelayedCall.Run(2f, Run);
                return;
            }

            DoozyPath.CreateMissingFolders();

            CreateSettingsAssets(false);
            CreateDatabaseAssets(false);
            RegenerateDatabaseAssets(false);

            ProcessorsSettings.Instance.RunDoozyAssetsProcessor = false;
            ProcessorsSettings.Instance.SetDirty(false);

            DoozyUtils.DisplayProgressBar("Hold on...", "Saving Processor Settings...", 0.95f);
            DoozyUtils.SaveAssets();
            DoozyUtils.ClearProgressBar();

#if !dUI_MASTER
            if(AssetDatabase.IsValidFolder("Assets/DoozyInstaller"))
                AssetDatabase.MoveAssetToTrash("Assets/DoozyInstaller");
#endif
        }

        private static void CreateSettingsAssets(bool saveAssets = true)
        {
            DoozyUtils.ClearProgressBar();
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - DoozyWindowSettings", 0.1f);
            DoozyWindowSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - NodyWindowSettings", 0.2f);
            NodyWindowSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - SoundySettings", 0.3f);
            SoundySettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - ThemesSettings", 0.35f);
            ThemesSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - TouchySettings", 0.4f);
            TouchySettings.Instance.SetDirty(false);

            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIButtonSettings", 0.5f);
            UIButtonSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UICanvasSettings", 0.6f);
            UICanvasSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIDrawerSettings", 0.7f);
            UIDrawerSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIPopupSettings", 0.8f);
            UIPopupSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIToggleSettings", 0.85f);
            UIToggleSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIViewSettings", 0.9f);
            UIViewSettings.Instance.SetDirty(false);

            if (saveAssets)
            {
                DoozyUtils.DisplayProgressBar("Hold on...", "Saving Assets...", 0.95f);
                DoozyUtils.SaveAssets();
            }

            DoozyUtils.ClearProgressBar();
        }

        private static void CreateDatabaseAssets(bool saveAssets = true)
        {
            DoozyUtils.ClearProgressBar();
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIAnimations", 0.1f);
            UIAnimations.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - SoundySettings", 0.15f);
            SoundySettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - ThemesSettings", 0.2f);
            ThemesSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIButtonSettings", 0.3f);
            UIButtonSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UICanvasSettings", 0.5f);
            UICanvasSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIDrawerSettings", 0.7f);
            UIDrawerSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIPopupSettings", 0.8f);
            UIPopupSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Check Asset - UIViewSettings", 0.9f);
            UIViewSettings.Database.SetDirty(false);

            if (saveAssets)
            {
                DoozyUtils.DisplayProgressBar("Hold on...", "Saving Assets...", 0.95f);
                DoozyUtils.SaveAssets();
            }

            DoozyUtils.ClearProgressBar();
        }

        private static void RegenerateDatabaseAssets(bool saveAssets = true)
        {
            DoozyUtils.ClearProgressBar();

            //SOUNDY
            DoozyUtils.DisplayProgressBar("Hold on...", "Soundy - Search For Unregistered Databases", 0.1f);
            SoundySettings.Database.SearchForUnregisteredDatabases(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Soundy - Refresh", 0.15f);
            SoundySettings.Database.RefreshDatabase(false, true);

            //THEMES
            DoozyUtils.DisplayProgressBar("Hold on...", "Themes - Search For Unregistered Themes", 0.2f);
            ThemesSettings.Database.SearchForUnregisteredThemes(false);
            DoozyUtils.DisplayProgressBar("Hold on...", "Themes - Refresh", 0.25f);
            ThemesSettings.Database.RefreshDatabase(false, true);
            
            //UIAnimations
            DoozyUtils.DisplayProgressBar("Hold on...", "UIAnimations - Search For Unregistered Databases", 0.3f);
            UIAnimations.Instance.SearchForUnregisteredDatabases(true);

            //UIButtons
            DoozyUtils.DisplayProgressBar("Hold on...", "Buttons - Search For Unregistered Databases", 0.4f);
            UIButtonSettings.Database.SearchForUnregisteredDatabases(true);

            //UICanvases
            DoozyUtils.DisplayProgressBar("Hold on...", "Canvases - Search For Unregistered Databases", 0.6f);
            UICanvasSettings.Database.SearchForUnregisteredDatabases(true);

            //UIDrawers
            DoozyUtils.DisplayProgressBar("Hold on...", "Drawers - Search For Unregistered Databases", 0.7f);
            UIDrawerSettings.Database.SearchForUnregisteredDatabases(true);

            //UIViews
            DoozyUtils.DisplayProgressBar("Hold on...", "Views - Search For Unregistered Databases", 0.8f);
            UIViewSettings.Database.SearchForUnregisteredDatabases(true);

            //UIPopups
            DoozyUtils.DisplayProgressBar("Hold on...", "Popups - Search For Unregistered Databases", 0.9f);
            UIPopupSettings.Database.SearchForUnregisteredLinks(true);

            if (saveAssets)
            {
                DoozyUtils.DisplayProgressBar("Hold on...", "Saving Assets...", 0.95f);
                DoozyUtils.SaveAssets();
            }

            DoozyUtils.ClearProgressBar();
        }
    }
}