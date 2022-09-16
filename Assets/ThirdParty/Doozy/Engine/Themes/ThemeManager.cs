// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Themes
{
    /// <summary> Manager that handles the theme management at runtime </summary>
    [AddComponentMenu(MenuUtils.ThemeManager_AddComponentMenu_MenuName, MenuUtils.ThemeManager_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.THEME_MANAGER)]
    public class ThemeManager : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [UnityEditor.MenuItem(MenuUtils.ThemeManager_MenuItem_ItemName, false, MenuUtils.ThemeManager_MenuItem_Priority)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected ThemeManager() { }

        private static ThemeManager s_instance;

        /// <summary> Returns a reference to the ThemeManager in the Scene. If one does not exist, it gets created </summary>
        public static ThemeManager Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<ThemeManager>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        public static bool ApplicationIsQuitting { get; private set; }

        /// <summary> Internal variable that keeps track if this class has been initialized </summary>
        private static bool s_initialized;

        /// <summary> Returns TRUE if the system should automatically save and load, at runtime, the last selected active theme variants </summary>
        public static bool AutoSave { get { return ThemesSettings.Instance.AutoSave; } }
        
        /// <summary> Reference to the global themes database </summary>
        public static ThemesDatabase Database { get { return ThemesSettings.Database; } }

        /// <summary> Internal database used to register all the theme targets to their respective selected theme </summary>
        public static readonly Dictionary<Guid, List<ThemeTarget>> ThemeTargets = new Dictionary<Guid, List<ThemeTarget>>();

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            ApplicationIsQuitting = false;
            s_initialized = false;
            ThemeTargets.Clear();
        }
#endif
        
        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two " + typeof(ThemeManager) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
            s_initialized = true;
        }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Public Methods

        /// <summary> Get the theme with the given theme id. If a theme with the given id is not found in the database, it will return null </summary>
        /// <param name="themeId"> Theme Guid to search for </param>
        public ThemeData GetTheme(Guid themeId) { return themeId == Guid.Empty ? null : Database.GetThemeData(themeId); }
        
        /// <summary> Get the first theme with the given name. If a theme with the given name is not found in the database, it will return null </summary>
        /// <param name="themeName"> Theme name to search for </param>
        public ThemeData GetTheme(string themeName) { return string.IsNullOrEmpty(themeName) ? null : Database.GetThemeData(themeName); }

        /// <summary> Get the theme variant with the given variant. If a theme variant with the given id is not found in the database, it will return null </summary>
        /// <param name="variantId"> Variant Guid to search for </param>
        public ThemeVariantData GetVariant(Guid variantId) { return variantId == Guid.Empty ? null : Database.GetVariant(variantId); }

        /// <summary> Get the theme variant with the given variant id. If a theme variant with the given variant id is not found in the target theme, it will return null </summary>
        /// <param name="themeId"> Theme Guid to search for (the theme where the variant can be found) </param>
        /// <param name="variantId"> Variant Guid to search for (in the theme with the given id) </param>
        public ThemeVariantData GetVariant(Guid themeId, Guid variantId)
        {
            if (themeId == Guid.Empty || variantId == Guid.Empty) return null;
            ThemeData themeData = Database.GetThemeData(themeId);
            return themeData == null ? null : themeData.GetVariant(variantId);
        }

        /// <summary> Get the theme variant with the given variant name. If a theme variant with the given variant name is not found in the target theme, it will return null </summary>
        /// <param name="themeId"> Theme Guid to search for (the theme where the variant can be found) </param>
        /// <param name="variantName"> Variant name to search for (in the theme with the given id) </param>
        public ThemeVariantData GetVariant(Guid themeId, string variantName)
        {
            if (themeId == Guid.Empty || string.IsNullOrEmpty(variantName)) return null;
            ThemeData themeData = Database.GetThemeData(themeId);
            return themeData == null ? null : themeData.GetVariant(variantName);
        }

        /// <summary> Get the theme variant with the given variant id. If a theme variant with the given variant id is not found in the target theme, it will return null </summary>
        /// <param name="themeName"> Theme name to search for (the theme where the variant can be found) </param>
        /// <param name="variantId"> Variant Guid to search for (in the first theme with the given name) </param>
        public ThemeVariantData GetVariant(string themeName, Guid variantId)
        {
            if (string.IsNullOrEmpty(themeName) || variantId == Guid.Empty) return null;
            ThemeData themeData = Database.GetThemeData(themeName);
            return themeData == null ? null : themeData.GetVariant(variantId);
        }

        /// <summary> Get the theme variant with the given variant name. If a theme variant with the given variant name is not found in the target theme, it will return null </summary>
        /// <param name="themeName"> Theme name to search for (the theme where the variant can be found) </param>
        /// <param name="variantName"> Variant name to search for (in the theme with the given id) </param>
        public ThemeVariantData GetVariant(string themeName, string variantName)
        {
            if (string.IsNullOrEmpty(themeName) || string.IsNullOrEmpty(variantName)) return null;
            ThemeData themeData = Database.GetThemeData(themeName);
            return themeData == null ? null : themeData.GetVariant(variantName);
        }

        #endregion

        #region Static Methods

        /// <summary> Activate the variant by id found in the theme with the given id </summary>
        /// <param name="themeId"> Theme Guid to search for (the theme where the variant can be found) </param>
        /// <param name="variantId"> Variant Guid to search for (in the theme with the given id) </param>
        public static void ActivateVariant(Guid themeId, Guid variantId)
        {
            if (!s_initialized) Init();
            ThemeData theme = Instance.GetTheme(themeId);
            if (theme == null) return;
            theme.ActivateVariant(variantId);
            UpdateTargets(theme);

            if (!AutoSave) return;
            SaveActiveVariant(theme);
            DataUtils.PlayerPrefsSave();
        }

        /// <summary> Activate the variant by name found in the theme with the given id </summary>
        /// <param name="themeId"> Theme Guid to search for (the theme where the variant can be found) </param>
        /// <param name="variantName"> Variant name to search for (in the theme with the given id) </param>
        public static void ActivateVariant(Guid themeId, string variantName)
        {
            if (!s_initialized) Init();
            ThemeData theme = Instance.GetTheme(themeId);
            if (theme == null) return;
            theme.ActivateVariant(variantName);
            UpdateTargets(theme);

            if (!AutoSave) return;
            SaveActiveVariant(theme);
            DataUtils.PlayerPrefsSave();
        }

        /// <summary> Activate the variant by id in the theme with the given name </summary>
        /// <param name="themeName"> Theme name to search for (the theme where the variant can be found) </param>
        /// <param name="variantId"> Variant Guid to search for (in the first theme with the given theme name) </param>
        public static void ActivateVariant(string themeName, Guid variantId)
        {
            if (!s_initialized) Init();
            ThemeData theme = Instance.GetTheme(themeName);
            if (theme == null) return;
            theme.ActivateVariant(variantId);
            UpdateTargets(theme);

            if (!AutoSave) return;
            SaveActiveVariant(theme);
            DataUtils.PlayerPrefsSave();
        }

        /// <summary> Activate the variant by name in the theme with the given name </summary>
        /// <param name="themeName"> Theme name to search for (the theme where the variant can be found) </param>
        /// <param name="variantName"> Variant name to search for (in the theme with the given name) </param>
        public static void ActivateVariant(string themeName, string variantName)
        {
            if (!s_initialized) Init();
            ThemeData theme = Instance.GetTheme(themeName);
            if (theme == null) return;
            theme.ActivateVariant(variantName);
            UpdateTargets(theme);

            if (!AutoSave) return;
            SaveActiveVariant(theme);
            DataUtils.PlayerPrefsSave();
        }

        /// <summary> Activate the variant by id. Performs a search through all the registered themes (the entire database) </summary>
        /// <param name="variantId"> Variant Guid to search for in the database </param>
        public static void ActivateVariant(Guid variantId)
        {
            if (!s_initialized) Init();
            foreach (ThemeData theme in Database.Themes.Where(theme => theme == null))
            {
                foreach (ThemeVariantData variant in theme.Variants.Where(variant => variant == null))
                {
                    if (!variant.Id.Equals(variantId)) continue;
                    theme.ActivateVariant(variant);
                    if (AutoSave) SaveActiveVariant(theme);
                    UpdateTargets(theme);
                    break;
                }
            }

            if (AutoSave) DataUtils.PlayerPrefsSave();
        }

        /// <summary> Initialize the ThemeManager Instance </summary>
        public static void Init()
        {
            if (s_initialized || s_instance != null) return;
            s_instance = Instance;

            if (AutoSave)
                foreach (ThemeData theme in Database.Themes)
                    LoadActiveVariant(theme);


            s_initialized = true;
        }
        
        /// <summary> Loads the active variant of the given theme from the PlayerPrefs </summary>
        /// <param name="theme"> Target theme </param>
        public static void LoadActiveVariant(ThemeData theme)
        {
            if (theme == null) return;
            if (DataUtils.PlayerPrefsHasKey(theme.Id.ToString()))
                theme.ActivateVariant(new Guid(DataUtils.PlayerPrefsGetString(theme.Id.ToString())));
            else
                SaveActiveVariant(theme);
        }

        /// <summary> Register a theme target to the ThemeManager. If the given theme target does not have a valid ThemeId, it will not get registered </summary>
        /// <param name="target"> Theme target </param>
        public static void RegisterTarget(ThemeTarget target)
        {
            if (!s_initialized) Init();
            if (target == null) return;
            if (target.ThemeId == Guid.Empty) return;
            if (!Database.Contains(target.ThemeId)) return;
            if (!ThemeTargets.ContainsKey(target.ThemeId)) ThemeTargets.Add(target.ThemeId, new List<ThemeTarget>());
            if (ThemeTargets[target.ThemeId].Contains(target)) return;
            ThemeTargets[target.ThemeId].Add(target);
            target.UpdateTarget(Database.GetThemeData(target.ThemeId));
        }

        /// <summary> Saves the active variant of the given theme to the PlayerPrefs </summary>
        /// <param name="theme"> Target theme </param>
        public static void SaveActiveVariant(ThemeData theme)
        {
            if (theme == null) return;
            if (theme.ActiveVariant == null) return;
            DataUtils.PlayerPrefsSetString(theme.Id.ToString(), theme.ActiveVariant.Id.ToString());
        }
        
        /// <summary> Unregister a theme target from the ThemeManager </summary>
        /// <param name="target"> Theme target </param>
        public static void UnregisterTarget(ThemeTarget target)
        {
            if (!s_initialized) Init();
            if (target == null) return;
            if (target.ThemeId == Guid.Empty) return;
            if (!Database.Contains(target.ThemeId)) return;
            if (!ThemeTargets.ContainsKey(target.ThemeId)) return;
            if (!ThemeTargets[target.ThemeId].Contains(target)) return;
            ThemeTargets[target.ThemeId].Remove(target);
        }

        /// <summary> Update all the theme targets registered in the ThemeManager </summary>
        public static void UpdateTargets()
        {
            if (!s_initialized) Init();
            foreach (Guid themeId in ThemeTargets.Keys)
            {
                ThemeData theme = Database.GetThemeData(themeId);
                if (theme == null) continue;
                foreach (ThemeTarget themeTarget in ThemeTargets[themeId])
                    themeTarget.UpdateTarget(theme);
            }
        }

        /// <summary> Update all the theme targets registered in the ThemeManager, for the given theme </summary>
        /// <param name="themeData"> Theme reference </param>
        public static void UpdateTargets(ThemeData themeData)
        {
            if (!s_initialized) Init();
            if (themeData == null) return;
            foreach (Guid themeId in ThemeTargets.Keys)
            {
                if (themeId != themeData.Id) continue;
                foreach (ThemeTarget themeTarget in ThemeTargets[themeId])
                    themeTarget.UpdateTarget(themeData);
                break;
            }
        }

        /// <summary> Update all the theme targets registered in the ThemeManager, for the given theme id </summary>
        /// <param name="themeId"> Theme Guid </param>
        public static void UpdateTargets(Guid themeId)
        {
            if (!s_initialized) Init();
            if (themeId == Guid.Empty) return;
            foreach (Guid guid in ThemeTargets.Keys)
            {
                if (guid != themeId) continue;
                ThemeData theme = Database.GetThemeData(themeId);
                if (theme == null) break;
                foreach (ThemeTarget themeTarget in ThemeTargets[themeId])
                    themeTarget.UpdateTarget(theme);
                break;
            }
        }

        /// <summary> Add ThemeManager to scene and get a reference to it </summary>
        private static ThemeManager AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<ThemeManager>(MenuUtils.ThemeManager_GameObject_Name, true, selectGameObjectAfterCreation); }
        
        #endregion
    }
}