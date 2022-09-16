// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Themes
{
    /// <inheritdoc />
    /// <summary>
    ///     Global themes database that contains references to ThemeData assets, used by the theme management system to keep track and retrieve all the registered themes
    /// </summary>
    [Serializable]
    public class ThemesDatabase : ScriptableObject
    {
        #region Constants

        public const string GENERAL_THEME_NAME = "General";
        public const string THEME_ASSET_PREFIX = "Theme_";

        #endregion

        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Public Variables

        /// <summary> List of all the theme names found in this database </summary>
        public List<string> ThemesNames = new List<string>();

        /// <summary> List of references to all the ThemeData assets that make up this database </summary>
        public List<ThemeData> Themes = new List<ThemeData>();

        #endregion

        #region Public Methods

        /// <summary> Add the given ThemeData reference to this database. Returns TRUE if the operation was successful </summary>
        /// <param name="themeData"> ThemeData that will get added to the database </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool AddTheme(ThemeData themeData, bool saveAssets)
        {
            if (themeData == null) return false;
            if (Themes == null) Themes = new List<ThemeData>();
            Themes.Add(themeData);
            UpdateThemesNames(false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Returns TRUE if a ThemeData reference, with the given theme Guid, has been found in the database </summary>
        /// <param name="themeGuid"> The theme Guid to search for </param>
        public bool Contains(Guid themeGuid)
        {
            if (Themes == null)
            {
                Themes = new List<ThemeData>();
                return false;
            }

            bool result = false;
            bool foundNullReference = false;

            foreach (ThemeData theme in Themes)
            {
                if (theme == null)
                {
                    foundNullReference = true;
                    continue;
                }

                if (theme.Id.Equals(themeGuid))
                {
                    result = true;
                    break;
                }
            }

            if (foundNullReference)
                RemoveNullDatabases(false);

            return result;
        }

        /// <summary> Returns TRUE if a ThemeData reference, with the given theme name, has been found in the database </summary>
        /// <param name="themeName"> The theme name to search for </param>
        public bool Contains(string themeName)
        {
            if (Themes == null)
            {
                Themes = new List<ThemeData>();
                return false;
            }

            bool result = false;
            bool foundNullReference = false;

            foreach (ThemeData theme in Themes)
            {
                if (theme == null)
                {
                    foundNullReference = true;
                    continue;
                }

                if (theme.ThemeName.Equals(themeName))
                {
                    result = true;
                    break;
                }
            }

            if (foundNullReference)
                RemoveNullDatabases(false);

            return result;
        }

        /// <summary> Creates a new ThemeData asset with the given theme name and adds a reference to it to the database </summary>
        /// <param name="themeName"> The name of the new theme </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateTheme(string themeName, bool showDialog = false, bool saveAssets = false)
        {
            return CreateTheme(DoozyPath.GetDataPath(DoozyPath.ComponentName.Themes), themeName, showDialog, saveAssets);
        }

        /// <summary> Creates a new ThemeData asset, at the given relative path, with the given theme name and adds a reference to it to the database </summary>
        /// <param name="relativePath"> Path where to create the theme asset </param>
        /// <param name="themeName"> The name of the new theme </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateTheme(string relativePath, string themeName, bool showDialog = false, bool saveAssets = false)
        {
            themeName = themeName.Trim();

            if (string.IsNullOrEmpty(themeName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewTheme, UILabels.EnterThemeName, UILabels.Ok);
#endif
                return false;
            }

            if (Contains(themeName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewTheme, UILabels.ThemeNameAlreadyExists, UILabels.Ok);
#endif
                return false;
            }

#if UNITY_EDITOR
            ThemeData themeData = AssetUtils.CreateAsset<ThemeData>(relativePath, GetThemeDataFilename(themeName.Replace(" ", string.Empty)));

#else
            ThemeData themeData = ScriptableObject.CreateInstance<ThemeData>();
#endif
            if (themeData == null) return false;
            themeData.ThemeName = themeName;
            themeData.name = themeName;
            themeData.Init(false, false);
            themeData.SetDirty(false);
            AddTheme(themeData, false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Remove the given ThemeData reference from the database and also deletes its corresponding asset file. Returns TRUE if the operation was successful </summary>
        /// <param name="themeData"> ThemeData to delete </param>
        public bool DeleteThemeData(ThemeData themeData)
        {
            if (themeData == null) return false;

#if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog(UILabels.DeleteTheme + " '" + themeData.ThemeName + "'",
                                             UILabels.AreYouSureYouWantToDeleteTheme +
                                             "\n\n" +
                                             UILabels.OperationCannotBeUndone,
                                             UILabels.Yes,
                                             UILabels.No))
                return false;

            Themes.Remove(themeData);
            DataUtils.PlayerPrefsDeleteKey(themeData.Id.ToString());
            AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(themeData));
            UpdateThemesNames(true);
#endif
            return true;
        }

        /// <summary> Get the ThemeData reference by searching through the database to look for the theme with the given id. If a theme with the given id is not found, it will return null </summary>
        /// <param name="themeGuid"> The ThemeData Guid to search for </param>
        public ThemeData GetThemeData(Guid themeGuid)
        {
            if (Themes == null)
            {
                Themes = new List<ThemeData>();
                return null;
            }

            foreach (ThemeData themeData in Themes)
                if (themeData.Id.Equals(themeGuid))
                    return themeData;

            return null;
        }

        /// <summary> Get the ThemeData reference by searching through the database to look for the first theme with the given theme name. If a theme with the given name is not found, it will return null </summary>
        /// <param name="themeName"> The ThemeData name to search for </param>
        public ThemeData GetThemeData(string themeName)
        {
            if (Themes == null)
            {
                Themes = new List<ThemeData>();
                return null;
            }

            foreach (ThemeData themeData in Themes)
                if (themeData.ThemeName.Equals(themeName))
                    return themeData;

            return null;
        }

        /// <summary> Get the theme index by Guid </summary>
        /// <param name="id"> Guid to search for </param>
        public int GetThemeIndex(Guid id)
        {
            if (id == Guid.Empty) return -1;
            for (int i = 0; i < Themes.Count; i++)
            {
                if(!Themes[i].Id.Equals(id))continue;
                return i;
            }

            return -1;
        }

        /// <summary> Get the ThemeVariantData by Guid </summary>
        /// <param name="variantId"> Guid to search for </param>
        public ThemeVariantData GetVariant(Guid variantId)
        {
            if (variantId == Guid.Empty) return null;
            foreach (ThemeData theme in Themes)
            {
                if(theme == null) continue;
                foreach (ThemeVariantData variant in theme.Variants)
                {
                    if(variant == null) continue;
                    if (variant.Id.Equals(variantId))
                        return variant;
                }
            }

            return null;
        }

        
        /// <summary> Add the 'General' theme if it does not exist and initializes it </summary>
        public void Initialize()
        {
            RemoveNullDatabases();
            if (Contains(GENERAL_THEME_NAME)) return;
            CreateTheme(GENERAL_THEME_NAME, false, true);
        }

        /// <summary> Returns TRUE if the database contains at least one theme with the given name </summary>
        /// <param name="themeName"> Theme name to search for </param>
        public bool ContainsTheme(string themeName) { return Themes.Any(theme => theme.ThemeName.Equals(themeName)); }

        /// <summary> Remove any null references and initialize all the referenced ThemeData found in the database </summary>
        public void InitializeThemes()
        {
            if (Themes == null) return;

            RemoveDuplicates(false);
            bool removedNullEntries = RemoveNullDatabases();

            //after removing any null references the database is still empty -> initialize it and add the 'General' theme
            if (Themes.Count == 0 || !ContainsTheme(ThemeData.UNNAMED_THEME_NAME))
            {
                Initialize();
                return;
            }

            if (removedNullEntries) SetDirty(false);
        }

        /// <summary> Initialize the database and perform a refresh on all the referenced ThemeData assets </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshDatabase(bool performUndo = true, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.RefreshDatabase);

            Initialize();
            foreach (ThemeData theme in Themes)
                theme.RefreshThemeVariants(true, false, false);

            SetDirty(saveAssets);
        }

        /// <summary> Remove any duplicate entries found in the database </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveDuplicates(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.RemovedDuplicateEntries);
            Themes = Themes.Distinct().ToList();
            UpdateThemesNames(saveAssets);
        }

        /// <summary> Remove any null references from the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool RemoveNullDatabases(bool saveAssets = false)
        {
            bool needsSave = false;
            if (Themes == null)
            {
                Themes = new List<ThemeData>();
                SetDirty(false);
                needsSave = true;
            }

            bool removedDatabase = false;
            for (int i = Themes.Count - 1; i >= 0; i--)
            {
                if (Themes[i] != null) continue;
                Themes.RemoveAt(i);
                removedDatabase = true;
            }

            UpdateThemesNames();

            if (needsSave || removedDatabase) SetDirty(saveAssets);

            return removedDatabase;
        }

        /// <summary> Rename a ThemeData database name (including the asset filename). Returns TRUE if the operation was successful </summary>
        /// <param name="themeData"> Target ThemeData </param>
        /// <param name="newThemeName"> The new database name for the target ThemeData </param>
        public bool RenameThemeData(ThemeData themeData, string newThemeName)
        {
            if (themeData == null) return false;

            newThemeName = newThemeName.Trim();

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(newThemeName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameTheme + " '" + themeData.ThemeName + "'",
                                            UILabels.EnterDatabaseName,
                                            UILabels.Ok);

                return false;
            }

            if (Contains(newThemeName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameTheme + " '" + themeData.ThemeName + "'",
                                            UILabels.NewThemeName + ": '" + newThemeName + "" +
                                            "\n\n" +
                                            UILabels.AnotherEntryExists,
                                            UILabels.Ok);

                return false;
            }

            themeData.ThemeName = newThemeName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(themeData), GetThemeDataFilename(newThemeName.Replace(" ", string.Empty)));
            UpdateThemesNames(true);
#endif
            return true;
        }
        
        /// <summary> Reset the database to the default values </summary>
        public bool ResetDatabase()
        {
#if UNITY_EDITOR
            if (!DoozyUtils.DisplayDialog(UILabels.ResetDatabase,
                                          UILabels.AreYouSureYouWantToResetDatabase +
                                          "\n\n" +
                                          UILabels.OperationCannotBeUndone,
                                          UILabels.Yes,
                                          UILabels.No))
                return false;

            for (int i = Themes.Count - 1; i >= 0; i--)
            {
                ThemeData themeData = Themes[i];
                Themes.Remove(themeData);
                DataUtils.PlayerPrefsDeleteKey(themeData.Id.ToString());
                DoozyUtils.MoveAssetToTrash(AssetDatabase.GetAssetPath(themeData));
            }

            Initialize();
            DDebug.Log(UILabels.DatabaseHasBeenReset);
#endif
            return true;
        }

        /// <summary> [Editor Only] Perform a deep search through the project for any unregistered ThemeData asset files and adds them to the database. The search is done only in all the Resources folders. </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SearchForUnregisteredThemes(bool saveAssets)
        {
            DoozyUtils.DisplayProgressBar(UILabels.SearchForThemes, UILabels.Search, 0.1f);

            bool foundUnregisteredThemeData = false;
            ThemeData[] array = Resources.LoadAll<ThemeData>("");
            if (array == null || array.Length == 0)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            if (Themes == null) Themes = new List<ThemeData>();
            for (int i = 0; i < array.Length; i++)
            {
                ThemeData foundThemeData = array[i];
                DoozyUtils.DisplayProgressBar(UILabels.SearchForThemes, UILabels.Search, 0.1f + 0.1f + 0.7f * (i + 1) / array.Length);
                if (Themes.Contains(foundThemeData)) continue;
                AddTheme(foundThemeData, false);
                foundUnregisteredThemeData = true;
            }
            
            if (!foundUnregisteredThemeData)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            DoozyUtils.DisplayProgressBar(UILabels.SearchForThemes, UILabels.Search, 0.9f);
            UpdateThemesNames();
            SetDirty(saveAssets);
            DoozyUtils.ClearProgressBar();
        }

        /// <summary> [Editor Only] Mark target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Sort the entire database </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Sort(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.SortDatabase);
            Themes = Themes.OrderBy(data => data.ThemeName).ToList(); //sort database by theme name
            foreach (ThemeData theme in Themes)
                theme.Sort(false, false);
            UpdateThemesNames(saveAssets);
        }

        /// <summary> [Editor Only] Record any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Update the list of ThemeData names found in the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void UpdateThemesNames(bool saveAssets = false)
        {
            ThemesNames.Clear();
            bool foundNullThemeDataReference = false;
            foreach (ThemeData theme in Themes)
            {
                if (theme == null)
                {
                    foundNullThemeDataReference = true;
                    continue;
                }

                ThemesNames.Add(theme.ThemeName);
            }

            ThemesNames.Sort();
            if (foundNullThemeDataReference)
            {
                Themes = Themes.Where(themeData => themeData != null).ToList();
                SetDirty(false);
            }

            SetDirty(saveAssets);
        }

        #endregion

        #region Static Methods

        /// <summary> Get a string array of all the theme names found in the database </summary>
        /// <param name="database"> Target database </param>
        public static string[] GetThemesNames(ThemesDatabase database)
        {
            var array = new string[database.Themes.Count];
            for (int i = 0; i < database.Themes.Count; i++)
                array[i] = database.Themes[i].ThemeName;
            return array;
        }

        /// <summary> Get a string array of all the variant names found in a theme </summary>
        /// <param name="themeData"> Target theme </param>
        public static string[] GetVariantNames(ThemeData themeData)
        {
            var array = new string[themeData.Variants.Count];
            for (int i = 0; i < themeData.Variants.Count; i++)
                array[i] = themeData.Variants[i].VariantName;
            return array;
        }

        /// <summary> Returns a proper formatted filename for a given theme name </summary>
        /// <param name="themeName"> Database name </param>
        public static string GetThemeDataFilename(string themeName) { return THEME_ASSET_PREFIX + themeName.Trim(); }

        #endregion
    }
}