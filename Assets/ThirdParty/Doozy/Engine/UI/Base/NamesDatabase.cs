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

// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Base
{
    /// <inheritdoc />
    /// <summary>
    ///     Database model used by the UIButton, UICanvas, UIDrawer and UIView components.
    ///     It stores and manages all the categories/names for each component type.
    /// </summary>
    [Serializable]
    public class NamesDatabase : ScriptableObject
    {
        #region Constants

        public const string BACK = "Back";
        public const string CUSTOM = "Custom";
        public const string DOWN = "Down";
        public const string GENERAL = "General";
        public const string LEFT = "Left";
        public const string MASTER_CANVAS = "MasterCanvas";
        public const string RIGHT = "Right";
        public const string UNNAMED = "Unnamed";
        public const string UP = "Up";

        #endregion

        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if the Database is empty </summary>
        public bool IsEmpty { get { return Categories.Count == 0; } }

        #endregion

        #region Public Variables

        /// <summary> Defines which type of component this database is for </summary>
        public NamesDatabaseType DatabaseType = NamesDatabaseType.UIView;

        /// <summary> List of all the category names found in this database </summary>
        public List<string> CategoryNames = new List<string>();

        /// <summary> List of names for each category </summary>
        public List<ListOfNames> Categories = new List<ListOfNames>();

        #endregion

        #region Public Methods

        /// <summary> Adds a new list of names to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="category"> The list of names </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Add(ListOfNames category, bool performUndo, bool saveAssets)
        {
            if (category == null) return false;
            if (Categories == null) Categories = new List<ListOfNames>();
            if (performUndo) UndoRecord(UILabels.AddItem);
            Categories.Add(category);
            category.DatabaseType = DatabaseType;
            UpdateListOfCategoryNames();
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Adds the default categories to the database by taking into account the database type. Returns TRUE if the operation was successful </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddDefaultCategories(bool saveAssets)
        {
            if (!Contains(GENERAL))
            {
//                CreateCategory(GENERAL, new List<string>(), false, false);
#if UNITY_EDITOR
                var listOfNames = AssetUtils.CreateAsset<ListOfNames>(GetPath(DatabaseType), GetDatabaseFileName(DatabaseType, GENERAL));
#else
                ListOfNames listOfNames = ScriptableObject.CreateInstance<ListOfNames>();
#endif
                listOfNames.CategoryName = GENERAL;
                listOfNames.DatabaseType = DatabaseType;
                Categories.Add(listOfNames);
                UpdateListOfCategoryNames();
                SetDirty(saveAssets);
            }

            ListOfNames general = GetCategory(GENERAL);

            switch (DatabaseType)
            {
                case NamesDatabaseType.UIButton:
                    if (!general.Contains(UNNAMED)) general.AddName(UNNAMED, false);
                    if (!general.Contains(BACK)) general.AddName(BACK, false);
                    break;

                case NamesDatabaseType.UICanvas:
                    if (!general.Contains(MASTER_CANVAS)) general.AddName(MASTER_CANVAS, false);
                    break;

                case NamesDatabaseType.UIView:
                    if (!general.Contains(UNNAMED)) general.AddName(UNNAMED, false);
                    break;

                case NamesDatabaseType.UIDrawer:
                    if (!general.Contains(UNNAMED)) general.AddName(UNNAMED, false);
                    if (!general.Contains(LEFT)) general.AddName(LEFT, false);
                    if (!general.Contains(RIGHT)) general.AddName(RIGHT, false);
                    if (!general.Contains(UP)) general.AddName(UP, false);
                    if (!general.Contains(DOWN)) general.AddName(DOWN, false);
                    break;
            }
        }

        /// <summary> Returns TRUE if a ListOfName reference, with the given category name, has been found in the database </summary>
        /// <param name="categoryName"> The category name to search for </param>
        public bool Contains(string categoryName)
        {
            categoryName = categoryName.Trim();
            if (categoryName.Equals(CUSTOM)) return true;
            if (Categories != null) return Categories.Where(listOfNames => listOfNames != null).Any(listOfNames => listOfNames.CategoryName.Equals(categoryName));
            Categories = new List<ListOfNames>();
            return false;

        }

        /// <summary> Creates a new ListOfNames asset with the given category name and adds a reference to it to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="categoryName"> The category name of the new ListOfNames </param>
        /// <param name="names"> Names to be added to the list </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateCategory(string categoryName, List<string> names, bool showDialog = false, bool saveAssets = false) { return CreateCategory(GetPath(DatabaseType), categoryName, names, showDialog, saveAssets); }

        /// <summary> Creates a new ListOfNames asset, at the given relative path, with the given category name and adds a reference to it to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="relativePath"> Path where to create the theme asset </param>
        /// <param name="categoryName"> The category name of the new ListOfNames </param>
        /// <param name="names"> Names to be added to the list </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateCategory(string relativePath, string categoryName, List<string> names, bool showDialog = false, bool saveAssets = false)
        {
            categoryName = categoryName.Trim();
            if (string.IsNullOrEmpty(categoryName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewCategory, UILabels.EnterCategoryName, UILabels.Ok);
#endif
                return false;
            }

            if (Contains(categoryName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewCategory, UILabels.AnotherEntryExists, UILabels.Ok);
#endif
                return false;
            }

#if UNITY_EDITOR
            var listOfNames = AssetUtils.CreateAsset<ListOfNames>(relativePath, GetDatabaseFileName(DatabaseType, categoryName));
#else
            ListOfNames listOfNames = ScriptableObject.CreateInstance<ListOfNames>();
#endif

            listOfNames.CategoryName = categoryName;
            if (names == null) names = new List<string>();
            listOfNames.AddNames(names, true, false);
            listOfNames.DatabaseType = DatabaseType;
            listOfNames.SetDirty(false);
            Categories.Add(listOfNames);
            UpdateListOfCategoryNames();
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Removes the given ListOfNames reference from the database and also deletes its corresponding asset file. Returns TRUE if the operation was successful </summary>
        /// <param name="category"> Target category to be deleted </param>
        public bool DeleteCategory(ListOfNames category)
        {
            if (category == null) return false;

#if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog(UILabels.Delete + " '" + category.CategoryName + "'",
                                             UILabels.AreYouSureYouWantToDeleteDatabase +
                                             "\n\n" +
                                             UILabels.OperationCannotBeUndone,
                                             UILabels.Yes,
                                             UILabels.No))
                return false;

            Categories.Remove(category);
            DoozyUtils.MoveAssetToTrash(AssetDatabase.GetAssetPath(category));
            UpdateListOfCategoryNames();
            SetDirty(false);
#endif
            return true;
        }

        /// <summary> Returns a reference to the ListOfName for the passed category name. Returns null if the category name was not found </summary>
        /// <param name="categoryName"> Category name to search for </param>
        public ListOfNames GetCategory(string categoryName)
        {
            foreach (ListOfNames listOfNames in Categories)
                if (listOfNames.CategoryName.Equals(categoryName))
                    return listOfNames;

            return null;
        }

        /// <summary> Returns a copy of the list of items (names) found in the given category name. If the category does not exist, it will return null </summary>
        /// <param name="categoryName"> Category name to search for </param>
        /// <param name="getDirectReference"> Should a direct reference to the items (names) list be returned? (instead of a copy) </param>
        public List<string> GetNamesList(string categoryName, bool getDirectReference = false)
        {
            foreach (ListOfNames category in Categories)
                if (category.CategoryName.Equals(categoryName))
                    return getDirectReference ? category.Names : new List<string>(category.Names);
            return null;
        }

        /// <summary> Refreshes the entire database by adding the default categories, removing empty and duplicate entries, sorting the database and updating the category names list </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshDatabase(bool performUndo, bool saveAssets)
        {
            string title = UILabels.Database + ": " + DatabaseType; // ProgressBar Title
            string info = UILabels.RefreshDatabase;                 // ProgressBar Info

            DoozyUtils.DisplayProgressBar(title, info, 0.1f);
            if (performUndo) UndoRecord(UILabels.RefreshDatabase);
            RemoveUnreferencedData(false);
            DoozyUtils.DisplayProgressBar(title, info, 0.2f);
            RemoveEmptyNames(false);
            DoozyUtils.DisplayProgressBar(title, info, 0.3f);
            RemoveDuplicateNamesFromCategories(false);
            DoozyUtils.DisplayProgressBar(title, info, 0.4f);
            AddDefaultCategories(false);
            DoozyUtils.DisplayProgressBar(title, info, 0.6f);
            Sort(false, false);
            DoozyUtils.DisplayProgressBar(title, info, 0.8f);
            UpdateListOfCategoryNames();
            DoozyUtils.DisplayProgressBar(title, info, 1f);
            SetDirty(saveAssets);
            DoozyUtils.ClearProgressBar();
        }

        /// <summary> Removes the category with the passed category name (if it exists) </summary>
        /// <param name="categoryName"> Category name to search for </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveCategory(string categoryName, bool showDialog, bool saveAssets)
        {
            if (!Contains(categoryName)) return;
#if UNITY_EDITOR
            if (showDialog)
                if (!EditorUtility.DisplayDialog(UILabels.RemoveCategory + " '" + categoryName + "'",
                                                 UILabels.AreYouSureYouWantToRemoveCategory + "\n\n" + UILabels.OperationCannotBeUndone,
                                                 UILabels.Yes,
                                                 UILabels.No))
                    return;
#endif

            bool needSave = false;
            for (int i = Categories.Count - 1; i >= 0; i--)
            {
                ListOfNames listOfNames = Categories[i];
                if (listOfNames.CategoryName != categoryName) continue;
                if (DeleteCategory(listOfNames))
                    needSave = true;
            }

            if (needSave) SetDirty(saveAssets);
        }

        /// <summary> Renames a category to a new category name. Returns TRUE if the operation was successful </summary>
        /// <param name="oldCategoryName"> The category on which the rename action is being performed </param>
        /// <param name="newCategoryName"> New category name </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Rename(string oldCategoryName, string newCategoryName, bool performUndo = true, bool saveAssets = false)
        {
            if (!CategoryNames.Contains(oldCategoryName)) return false;

#if UNITY_EDITOR
            newCategoryName = newCategoryName.Trim();
            if (string.IsNullOrEmpty(newCategoryName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameCategory + " '" + oldCategoryName + "'",
                                            UILabels.EnterCategoryName,
                                            UILabels.Ok);
                return false;
            }

            if (Contains(newCategoryName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameCategory + " '" + oldCategoryName + "'",
                                            UILabels.NewCategory + ": '" + newCategoryName + "" +
                                            "\n\n" +
                                            UILabels.AnotherEntryExists,
                                            UILabels.Ok);

                return false;
            }

            if (performUndo) UndoRecord(UILabels.RenameCategory);
#endif

            foreach (ListOfNames listOfNames in Categories)
            {
                if (!listOfNames.CategoryName.Equals(oldCategoryName)) continue;
                listOfNames.Rename(newCategoryName, GetDatabaseFileName(DatabaseType, newCategoryName), false);
                break;
            }

            UpdateListOfCategoryNames();
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Removes any duplicate entries found in the database </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveDuplicateNamesFromCategories(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.RemovedDuplicateEntries);
            foreach (ListOfNames listOfNames in Categories)
                listOfNames.RemoveDuplicateNames();
        }

        /// <summary> Removes any null references from the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveNullDatabases(bool saveAssets = false)
        {
            bool needsSave = false;
            if (Categories == null)
            {
                Categories = new List<ListOfNames>();
                SetDirty(false);
                needsSave = true;
            }

            bool removedDatabase = false;
            for (int i = Categories.Count - 1; i >= 0; i--)
            {
                if (Categories[i] != null) continue;
                Categories.RemoveAt(i);
                removedDatabase = true;
            }

            UpdateListOfCategoryNames();

            if (needsSave || removedDatabase) SetDirty(saveAssets);
        }

        /// <summary> Removes any entries that have no name </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveEmptyNames(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.RemoveEmptyEntries);
            foreach (ListOfNames listOfNames in Categories)
                listOfNames.RemoveEmptyNames();
        }

        /// <summary> Removes any unreferenced ListOfNames from the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveUnreferencedData(bool saveAssets = false)
        {
#if UNITY_EDITOR
            if (Categories == null) Categories = new List<ListOfNames>();
            bool needsSave = false;
            for (int i = Categories.Count - 1; i >= 0; i--)
                if (Categories[i] == null)
                {
                    Categories.RemoveAt(i);
                    needsSave = true;
                }

            if (!needsSave) return;
            UpdateListOfCategoryNames();
            SetDirty(saveAssets);
#endif
        }

        /// <summary> Resets the database to the default values </summary>
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

            for (int i = Categories.Count - 1; i >= 0; i--)
            {
                ListOfNames category = Categories[i];
                Categories.Remove(category);
                DoozyUtils.MoveAssetToTrash(AssetDatabase.GetAssetPath(category));
            }

            UpdateListOfCategoryNames();
            AddDefaultCategories(true);
            DDebug.Log(UILabels.DatabaseHasBeenReset);
#endif
            return true;
        }

        /// <summary> [Editor Only] Performs a deep search through the project for any unregistered ListOfNames asset files and adds them to the database. The search is done only in all the Resources folders. </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SearchForUnregisteredDatabases(bool saveAssets)
        {
            DoozyUtils.DisplayProgressBar(UILabels.SearchForDatabases, UILabels.Search, 0.1f);

            bool foundUnregisteredDatabase = false;
            ListOfNames[] array = Resources.LoadAll<ListOfNames>("");
            if (array == null || array.Length == 0)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            if (Categories == null) Categories = new List<ListOfNames>();
            for (int i = 0; i < array.Length; i++)
            {
                DoozyUtils.DisplayProgressBar(UILabels.SearchForDatabases, UILabels.Search, 0.1f + 0.7f * (i + 1) / array.Length);
                ListOfNames foundList = array[i];
                if (foundList.DatabaseType != DatabaseType) continue;
                if (Categories.Contains(foundList)) continue;
                Categories.Add(foundList);
                foundUnregisteredDatabase = true;
            }

            if (!foundUnregisteredDatabase)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            DoozyUtils.DisplayProgressBar(UILabels.SearchForDatabases, UILabels.Search, 0.9f);
            UpdateListOfCategoryNames();
            SetDirty(saveAssets);
            DoozyUtils.ClearProgressBar();
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Sorts the entire database </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Sort(bool performUndo, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.SortDatabase);
            Categories = Categories.OrderBy(listOfNames => listOfNames.CategoryName).ToList(); //sort database by category name
            foreach (ListOfNames listOfNames in Categories) listOfNames.Names.Sort();          //sort items in each category
            SetDirty(saveAssets);
        }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Updates the list of category name as defined in the database </summary>
        public void UpdateListOfCategoryNames()
        {
            CategoryNames.Clear();
            foreach (ListOfNames listOfNames in Categories.Where(listOfNames => listOfNames != null).Where(listOfNames => !CategoryNames.Contains(listOfNames.CategoryName)))
            {
                CategoryNames.Add(listOfNames.CategoryName);
            }

            CategoryNames.Insert(0, CUSTOM);
        }

        #endregion

        #region Static Methods

        /// <summary> Returns TRUE if an item can be deleted from the database. This is used to make sure some values cannot be deleted from the database according to the DatabaseType (NamesDatabaseType) </summary>
        /// <param name="database"> The database reference that contains the item </param>
        /// <param name="itemName"> The item's name </param>
        public static bool CanDeleteItem(NamesDatabase database, string itemName)
        {
            switch (database.DatabaseType)
            {
                case NamesDatabaseType.UIButton:
                    if (itemName.Equals(UNNAMED)) return false;
                    if (itemName.Equals(BACK)) return false;
                    break;
                case NamesDatabaseType.UICanvas:
                    if (itemName.Equals(MASTER_CANVAS)) return false;
                    break;
                case NamesDatabaseType.UIView:
                    if (itemName.Equals(UNNAMED)) return false;
                    break;
                case NamesDatabaseType.UIDrawer:
                    if (itemName.Equals(UNNAMED)) return false;
                    if (itemName.Equals(LEFT)) return false;
                    if (itemName.Equals(RIGHT)) return false;
                    if (itemName.Equals(UP)) return false;
                    if (itemName.Equals(DOWN)) return false;
                    break;
            }

            return true;
        }

        /// <summary> Returns a NamesDatabase with the given fileName from the target resourcesPath. If the database is not found, a new one will get created instead. </summary>
        /// <param name="fileName"> Database filename </param>
        /// <param name="resourcesPath"> Database relative file path to the Resources folder </param>
        public static NamesDatabase GetDatabase(string fileName, string resourcesPath)
        {
            //
            return AssetUtils.GetScriptableObject<NamesDatabase>("_" + fileName, resourcesPath, false, false);
        }

        /// <summary> Returns the default resources path for the given database type </summary>
        /// <param name="databaseType"> Database type </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetPath(NamesDatabaseType databaseType) { return DoozyPath.GetDataPath(GetComponentName(databaseType)); }

        /// <summary> Returns the ComponentName for the passed NamesDatabaseType </summary>
        /// <param name="databaseType"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static DoozyPath.ComponentName GetComponentName(NamesDatabaseType databaseType)
        {
            switch (databaseType)
            {
                case NamesDatabaseType.UIButton: return DoozyPath.ComponentName.UIButton;
                case NamesDatabaseType.UICanvas: return DoozyPath.ComponentName.UICanvas;
                case NamesDatabaseType.UIView:   return DoozyPath.ComponentName.UIView;
                case NamesDatabaseType.UIDrawer: return DoozyPath.ComponentName.UIDrawer;
                default:                         throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Returns the file name that will be used when creating a new ListOfNames asset file </summary>
        /// <param name="databaseType"> Database type that will use this list of name </param>
        /// <param name="categoryName"> Category name that this list of names will have </param>
        private static string GetDatabaseFileName(NamesDatabaseType databaseType, string categoryName) { return databaseType + "_" + categoryName.Replace(" ", string.Empty); }

        #endregion
    }
}