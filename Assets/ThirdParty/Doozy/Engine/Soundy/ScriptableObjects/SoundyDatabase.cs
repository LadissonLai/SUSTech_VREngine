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

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <inheritdoc />
    /// <summary>
    ///     NamesDatabaseType of database that contains references to SoundDatabase assets, used by Soundy to keep track and retrieve all the registered sounds
    /// </summary>
    [Serializable]
    public class SoundyDatabase : ScriptableObject
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Public Variables

        /// <summary> List of all the SoundDatabase names found in this database </summary>
        public List<string> DatabaseNames = new List<string>();

        /// <summary> List of references to all the SoundDatabase assets that make up this database </summary>
        public List<SoundDatabase> SoundDatabases = new List<SoundDatabase>();

        #endregion

        #region Public Methods

        /// <summary> Adds the SoundDatabase reference to this database. Returns TRUE if the operation was successful </summary>
        /// <param name="database"> SoundDatabase that will get added to the database </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool AddSoundDatabase(SoundDatabase database, bool saveAssets)
        {
            if (database == null) return false;
            if (SoundDatabases == null) SoundDatabases = new List<SoundDatabase>();
            SoundDatabases.Add(database);
            UpdateDatabaseNames(false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Returns TRUE if a SoundDatabase reference, with the given database name, has been found in the database </summary>
        /// <param name="databaseName"> The database name to search for </param>
        public bool Contains(string databaseName)
        {
            if (SoundDatabases == null)
            {
                SoundDatabases = new List<SoundDatabase>();
                return false;
            }

            bool result = false;
            bool foundNullReference = false;

            foreach (SoundDatabase database in SoundDatabases)
            {
                if (database == null)
                {
                    foundNullReference = true;
                    continue;
                }

                if (database.DatabaseName.Equals(databaseName))
                {
                    result = true;
                    break;
                }
            }

            if (foundNullReference)
                RemoveNullDatabases(false);

            return result;
        }

        /// <summary> Returns TRUE if the given database name exists and if it contains a SoundGroupData reference with the given sound name </summary>
        /// <param name="databaseName"> The database name to search for </param>
        /// <param name="soundName"> The sound name to search for </param>
        public bool Contains(string databaseName, string soundName) { return Contains(databaseName) && GetSoundDatabase(databaseName).Contains(soundName); }

        /// <summary> Creates a new SoundDatabase asset with the given database name and adds a reference to it to the database </summary>
        /// <param name="databaseName"> The name of the new SoundDatabase </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateSoundDatabase(string databaseName, bool showDialog = false, bool saveAssets = false) { return CreateSoundDatabase(DoozyPath.GetDataPath(DoozyPath.ComponentName.Soundy), databaseName, showDialog, saveAssets); }

        /// <summary> Creates a new SoundDatabase asset, at the given relative path, with the given database name and adds a reference to it to the database </summary>
        /// <param name="relativePath"> Path where to create the theme asset </param>
        /// <param name="databaseName"> The name of the new SoundDatabase </param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateSoundDatabase(string relativePath, string databaseName, bool showDialog = false, bool saveAssets = false)
        {
            databaseName = databaseName.Trim();

            if (string.IsNullOrEmpty(databaseName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewSoundDatabase, UILabels.EnterDatabaseName, UILabels.Ok);
#endif
                return false;
            }

            if (Contains(databaseName))
            {
#if UNITY_EDITOR
                if (showDialog) EditorUtility.DisplayDialog(UILabels.NewSoundDatabase, UILabels.DatabaseAlreadyExists, UILabels.Ok);
#endif
                return false;
            }

#if UNITY_EDITOR
            SoundDatabase soundDatabase = AssetUtils.CreateAsset<SoundDatabase>(relativePath, SoundyManager.GetSoundDatabaseFilename(databaseName.Replace(" ", string.Empty)));

#else
            SoundDatabase soundDatabase = ScriptableObject.CreateInstance<SoundDatabase>();
#endif
            soundDatabase.DatabaseName = databaseName;
            soundDatabase.Initialize(false);
            AddSoundDatabase(soundDatabase, false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Removes the given SoundDatabase reference from the database and also deletes its corresponding asset file. Returns TRUE if the operation was successful </summary>
        /// <param name="database"> SoundDatabase to delete </param>
        public bool DeleteDatabase(SoundDatabase database)
        {
            if (database == null) return false;

#if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog(UILabels.DeleteDatabase + " '" + database.DatabaseName + "'",
                                             UILabels.AreYouSureYouWantToDeleteDatabase +
                                             "\n\n" +
                                             UILabels.OperationCannotBeUndone,
                                             UILabels.Yes,
                                             UILabels.No))
                return false;

            SoundDatabases.Remove(database);
            AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(database));
            UpdateDatabaseNames(true);
#endif
            return true;
        }

        /// <summary>
        ///     Returns a SoundGroupData reference from the database with the given database name and that has the given sound name.
        ///     If the sound database does not exist, or no SoundGroupData with the given sound name is found, it returns null.
        /// </summary>
        /// <param name="databaseName"> The database name to search for </param>
        /// <param name="soundName"> The sound name to search for </param>
        public SoundGroupData GetAudioData(string databaseName, string soundName) { return !Contains(databaseName) ? null : GetSoundDatabase(databaseName).GetData(soundName); }

        /// <summary> Iterates through the database to look for the given database name to return the corresponding SoundDatabase reference. If the name does not exist, it will return null </summary>
        /// <param name="databaseName"> The database name to search for </param>
        public SoundDatabase GetSoundDatabase(string databaseName)
        {
            if (SoundDatabases == null)
            {
                SoundDatabases = new List<SoundDatabase>();
                return null;
            }

            foreach (SoundDatabase database in SoundDatabases)
                if (database.DatabaseName.Equals(databaseName))
                    return database;

            return null;
        }

        /// <summary> Adds the 'General' SoundDatabase if it does not exist and initializes it </summary>
        public void Initialize()
        {
            RemoveNullDatabases();

            if (Contains(SoundyManager.GENERAL)) return;

#if UNITY_EDITOR
            SearchForUnregisteredDatabases(false);
            if (Contains(SoundyManager.GENERAL)) return;

            SoundDatabase soundDatabase = AssetUtils.CreateAsset<SoundDatabase>(DoozyPath.GetDataPath(DoozyPath.ComponentName.Soundy), SoundyManager.GetSoundDatabaseFilename(SoundyManager.GENERAL));
#else
            SoundDatabase soundDatabase = ScriptableObject.CreateInstance<SoundDatabase>();
#endif
            AddSoundDatabase(soundDatabase, true);
            soundDatabase.DatabaseName = SoundyManager.GENERAL;
            soundDatabase.Initialize(true);
            UpdateDatabaseNames(true);
        }

        /// <summary> Removes any null references and initializes all the referenced SoundDatabase found in the database </summary>
        public void InitializeSoundDatabases()
        {
            if (SoundDatabases == null) return;

            //remove any null sound database reference
            bool foundNullReference = false;
            for (int i = SoundDatabases.Count - 1; i >= 0; i--)
            {
                SoundDatabase soundDatabase = SoundDatabases[i];
                if (soundDatabase == null)
                {
                    SoundDatabases.RemoveAt(i);
                    foundNullReference = true;
                    continue;
                }

                soundDatabase.Initialize(false);
            }

            //after removing any null references the database is still empty -> initialize it and add the 'General' sound database
            if (SoundDatabases.Count == 0)
            {
                Initialize();
                return;
            }


            //database is not empty, but at least one null sound database reference was removed -> mark the database as dirty
            if (foundNullReference) SetDirty(false);
        }

        /// <summary> Initializes the database and performs a refresh on all the referenced SoundDatabase assets </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshDatabase(bool performUndo = true, bool saveAssets = false)
        {
            if (performUndo) UndoRecord(UILabels.RefreshDatabase);

            Initialize();
            foreach (SoundDatabase soundDatabase in SoundDatabases)
                soundDatabase.RefreshDatabase(false, false);

            SetDirty(saveAssets);
        }

        /// <summary> Removes any null references from the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveNullDatabases(bool saveAssets = false)
        {
            bool needsSave = false;
            if (SoundDatabases == null)
            {
                SoundDatabases = new List<SoundDatabase>();
                SetDirty(false);
                needsSave = true;
            }

            bool removedDatabase = false;
            for (int i = SoundDatabases.Count - 1; i >= 0; i--)
            {
                if (SoundDatabases[i] != null) continue;
                SoundDatabases.RemoveAt(i);
                removedDatabase = true;
            }

            UpdateDatabaseNames();

            if (needsSave || removedDatabase) SetDirty(saveAssets);
        }

        /// <summary> Renames a SoundDatabase database name (including the asset filename). Returns TRUE if the operation was successful </summary>
        /// <param name="soundDatabase"> Target SoundDatabase </param>
        /// <param name="newDatabaseName"> The new database name for the target SoundDatabase </param>
        public bool RenameSoundDatabase(SoundDatabase soundDatabase, string newDatabaseName)
        {
            if (soundDatabase == null) return false;

            newDatabaseName = newDatabaseName.Trim();

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(newDatabaseName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameSoundDatabase + " '" + soundDatabase.DatabaseName + "'",
                                            UILabels.EnterDatabaseName,
                                            UILabels.Ok);

                return false;
            }

            if (Contains(newDatabaseName))
            {
                EditorUtility.DisplayDialog(UILabels.RenameSoundDatabase + " '" + soundDatabase.DatabaseName + "'",
                                            UILabels.NewSoundDatabase + ": '" + newDatabaseName + "" +
                                            "\n\n" +
                                            UILabels.AnotherEntryExists,
                                            UILabels.Ok);

                return false;
            }

            soundDatabase.DatabaseName = newDatabaseName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(soundDatabase), "SoundDatabase_" + newDatabaseName.Replace(" ", string.Empty));
            UpdateDatabaseNames(true);
#endif
            return true;
        }

        /// <summary> [Editor Only] Performs a deep search through the project for any unregistered SoundDatabase asset files and adds them to the database. The search is done only in all the Resources folders. </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SearchForUnregisteredDatabases(bool saveAssets)
        {
            bool foundUnregisteredDatabase = false;
            SoundDatabase[] array = Resources.LoadAll<SoundDatabase>("");
            if (array == null || array.Length == 0) return;
            if (SoundDatabases == null) SoundDatabases = new List<SoundDatabase>();
            foreach (SoundDatabase foundDatabase in array)
            {
                if (SoundDatabases.Contains(foundDatabase)) continue;
                AddSoundDatabase(foundDatabase, false);
                foundUnregisteredDatabase = true;
            }

            if (!foundUnregisteredDatabase) return;
            UpdateDatabaseNames();
            SetDirty(saveAssets);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Updates the list of SoundDatabase names found in the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void UpdateDatabaseNames(bool saveAssets = false)
        {
            if (DatabaseNames == null) DatabaseNames = new List<string>();
            if (SoundDatabases == null) SoundDatabases = new List<SoundDatabase>();
            DatabaseNames.Clear();
            bool foundNullDatabaseReference = false;
            foreach (SoundDatabase database in SoundDatabases)
            {
                if (database == null)
                {
                    foundNullDatabaseReference = true;
                    continue;
                }

                DatabaseNames.Add(database.DatabaseName);
            }

            DatabaseNames.Sort();
            if (foundNullDatabaseReference)
            {
                SoundDatabases = SoundDatabases.Where(soundDatabase => soundDatabase != null).ToList();
                SetDirty(false);
            }

            SetDirty(saveAssets);
        }

        #endregion
    }
}