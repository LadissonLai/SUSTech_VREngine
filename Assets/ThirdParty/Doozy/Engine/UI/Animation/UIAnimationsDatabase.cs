// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Doozy.Engine.UI.Animation
{
    /// <summary>
    ///     NamesDatabaseType of database that contains all of the UIAnimationDatabase references, of a given database type (AnimationType)
    /// </summary>
    [Serializable]
    public class UIAnimationsDatabase
    {
        #region Public Variables

        /// <summary> List of all the UIAnimationDatabase database names (preset categories) that this database contains </summary>
        public List<string> DatabaseNames;

        /// <summary> The animation type that determines what type of UIAnimationDatabase databases this database contains </summary>
        public AnimationType DatabaseType;

        /// <summary> List of references to UIAnimationDatabase assets </summary>
        public List<UIAnimationDatabase> Databases;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the UIAnimations.Database class, of the given type (AnimationType) </summary>
        /// <param name="animationType"> The animation type that determines what type of UIAnimationDatabase databases this database contains </param>
        public UIAnimationsDatabase(AnimationType animationType)
        {
            DatabaseType = animationType;
            Databases = new List<UIAnimationDatabase>();
            DatabaseNames = new List<string>();
        }

        #endregion

        #region Public Methods

        /// <summary> Adds a the given UIAnimationDatabase reference to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="database"> Target UIAnimationDatabase</param>
        public bool AddUIAnimationDatabase(UIAnimationDatabase database)
        {
            if (database == null || database.DataType != DatabaseType || Contains(database)) return false;
            Databases.Add(database);
            return true;
        }

        /// <summary> Returns TRUE if the database name has been found in the database </summary>
        /// <param name="databaseName"> Target database name to search for</param>
        public bool Contains(string databaseName) { return Get(databaseName) != null; }

        /// <summary> Returns TRUE if the UIAnimationDatabase has been found in the database </summary>
        /// <param name="database"> UIAnimationDatabase to search for </param>
        public bool Contains(UIAnimationDatabase database) { return database != null && Databases.Contains(database); }

        /// <summary> Iterates through the database to look for the database name. If found, returns a reference to the corresponding UIAnimationDatabase, else it returns null </summary>
        /// <param name="databaseName"> The database name to search for </param>
        public UIAnimationDatabase Get(string databaseName)
        {
            foreach (UIAnimationDatabase database in Databases)
                if (database.DatabaseName.Equals(databaseName))
                    return database;
            return null;
        }

        /// <summary>
        ///     Refreshes the entire database by removing null references, empty databases, adding the default presets database, renaming the preset databases file names to their database names,
        ///     sorting the database and updating the database names list
        /// </summary>
        public void Update()
        {
            AddUnreferencedPresets();
            RemoveEmptyDatabases();
            AddTheDefaultUIAnimationDatabase();
            RenameAssetFileNamesToReflectDatabaseNames();
            Sort();
            UpdateDatabaseNames();
            UpdateDatabases();
        }

        #endregion

        #region Private Methods

        /// <summary> [Editor Only] Adds the 'Uncategorized' database if it's missing </summary>
        private void AddTheDefaultUIAnimationDatabase()
        {
            if (Contains(UIAnimations.DEFAULT_DATABASE_NAME)) return;

#if UNITY_EDITOR
            var asset = AssetDatabase.LoadAssetAtPath<UIAnimationDatabase>(Path.Combine(Path.Combine(DoozyPath.UIANIMATIONS_RESOURCES_PATH, DatabaseType.ToString()), UIAnimations.DEFAULT_DATABASE_NAME + ".asset"));
            if (asset != null)
            {
                Databases.Add(asset);
                UpdateDatabaseNames();
                return;
            }
            
            UIAnimationDatabase database = AssetUtils.CreateAsset<UIAnimationDatabase>(Path.Combine(DoozyPath.UIANIMATIONS_RESOURCES_PATH, DatabaseType.ToString()), UIAnimations.DEFAULT_DATABASE_NAME);
#else
            UIAnimationDatabase database = ScriptableObject.CreateInstance<UIAnimationDatabase>();
#endif
            if(database == null) return;
            database.DatabaseName = UIAnimations.DEFAULT_DATABASE_NAME;
            database.name = database.DatabaseName;
            database.DataType = DatabaseType;
            database.RefreshDatabase(false);
            Databases.Add(database);
        }

        /// <summary> Iterates through all the referenced UIAnimationDatabase databases and populates them with any UIAnimationData </summary>
        private void AddUnreferencedPresets()
        {
#if UNITY_EDITOR
            bool saveAssets = false;
            if (Databases == null) Databases = new List<UIAnimationDatabase>();
            foreach (UIAnimationDatabase database in Databases)
            {
                Object[] objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(database)); //load all sub assets
                if (objects == null) return;                                                                //make sure they are not null
                List<UIAnimationData> foundData = objects.OfType<UIAnimationData>().ToList();               //create a list of all the found sub assets
                bool databaseUpdated = false;                                                               //mark true if any changes were performed on the database
                foreach (UIAnimationData data in foundData)
                {
                    if (database.Contains(data)) continue;
                    database.Database.Add(data);
                    databaseUpdated = true;
                }

                if (!databaseUpdated) continue;
                database.SetDirty(false);
                saveAssets = true;
            }

            if (saveAssets)
            {
                UpdateDatabaseNames();
                DoozyUtils.SaveAssets();
            }
#endif
        }

        /// <summary> Renames all the UIAnimationDatabase asset filename to their set database name </summary>
        private void RenameAssetFileNamesToReflectDatabaseNames()
        {
            foreach (UIAnimationDatabase database in Databases)
            {
                if (database == null || database.name.Equals(database.DatabaseName)) continue;
                database.name = database.DatabaseName;
                database.SetDirty(true);
            }
        }

        private void RemoveEmptyDatabases()
        {
            for (int i = Databases.Count - 1; i >= 0; i--)
            {
                UIAnimationDatabase database = Databases[i];
                
                if (database == null)
                {
                    Databases.RemoveAt(i);
                    continue;
                }
                
                if (database != null && database.Database.Count > 0) continue;
#if UNITY_EDITOR
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(database));
#endif
                Databases.RemoveAt(i);
            }
        }

        /// <summary> Sorts all the databases by the database name </summary>
        private void Sort() { Databases = Databases.OrderBy(database => database.DatabaseName).ToList(); }


        /// <summary> Updates the list of UIAnimationDatabase database names </summary>
        private void UpdateDatabaseNames()
        {
            if (DatabaseNames == null) DatabaseNames = new List<string>();
            DatabaseNames.Clear();
            foreach (UIAnimationDatabase database in Databases)
                DatabaseNames.Add(database.DatabaseName);
        }

        /// <summary> Executes a refresh for every referenced UIAnimationDatabase </summary>
        private void UpdateDatabases()
        {
            foreach (UIAnimationDatabase database in Databases)
                database.RefreshDatabase(false);
        }

        #endregion
    }
}