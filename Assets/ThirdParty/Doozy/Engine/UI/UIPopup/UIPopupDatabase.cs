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
// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Database model used by the UIPopupManager to store references of pairs of UIPopup prefab and popup name.
    /// </summary>
    [Serializable]
    public class UIPopupDatabase : ScriptableObject
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if the Database is empty </summary>
        public bool IsEmpty { get { return Popups.Count == 0; } }

        #endregion

        #region Public Variables

        /// <summary> List of all the popup names found in this database (these are NOT the prefab names) </summary>
        public List<string> PopupNames = new List<string>();

        /// <summary> Database of pairs of UIPopup prefab and popup name </summary>
        public List<UIPopupLink> Popups = new List<UIPopupLink>();

        #endregion

        #region Public Methods

        /// <summary> Adds a new entry to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="popupLink"> Popup link asset </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Add(UIPopupLink popupLink, bool performUndo, bool saveAssets)
        {
            if (popupLink == null) return false;
            if (Popups == null) Popups = new List<UIPopupLink>();
            if (performUndo) UndoRecord(UILabels.AddItem);
            Popups.Add(popupLink);
            UpdateListOfPopupNames();
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Returns TRUE if the popup name has been defined in the database </summary>
        /// <param name="popupName"> Popup name to search for </param>
        public bool Contains(string popupName)
        {
            popupName = popupName.Trim();
            foreach (UIPopupLink links in Popups)
                if (links.PopupName.Equals(popupName))
                    return true;
            return false;
        }

        /// <summary> Returns TRUE if the prefab has a reference in the database </summary>
        /// <param name="prefab"> Prefab to search for </param>
        public bool Contains(UIPopup prefab)
        {
            if (prefab == null) return false;
            foreach (UIPopupLink reference in Popups)
                if (reference.Prefab == prefab.gameObject)
                    return true;
            return false;
        }

        /// <summary> Creates a new UIPopupLink asset with the given settings and adds a reference to it to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="popupName"> Popup name used to retrieve the prefab </param>
        /// <param name="prefab"> Prefab reference </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool CreateUIPopupLink(string popupName, GameObject prefab, bool performUndo, bool saveAssets)
        {
            popupName = popupName.Trim();
            if (string.IsNullOrEmpty(popupName))
            {
                DDebug.Log(UILabels.NewPopup + ": " + UILabels.CannotAddEmptyEntry + " " + UILabels.PleaseEnterNewName);

#if UNITY_EDITOR
                EditorUtility.DisplayDialog(UILabels.NewPopup,
                                            UILabels.CannotAddEmptyEntry + "\n\n" + UILabels.PleaseEnterNewName,
                                            UILabels.Ok);
#endif
                return false;
            }

            if (Contains(popupName))
            {
                DDebug.Log(UILabels.NewPopup + ": " + UILabels.AnotherEntryExists + " " + UILabels.PleaseEnterNewName);

#if UNITY_EDITOR
                EditorUtility.DisplayDialog(UILabels.NewPopup,
                                            UILabels.AnotherEntryExists + "\n\n" + UILabels.PleaseEnterNewName,
                                            UILabels.Ok);
#endif

                return false;
            }

#if UNITY_EDITOR
            UIPopupLink link = AssetUtils.CreateAsset<UIPopupLink>(DoozyPath.ENGINE_RESOURCES_DATA_UIPOPUP_PATH, "UIPopupLink_" + popupName);
#else
            UIPopupLink link = ScriptableObject.CreateInstance<UIPopupLink>();
#endif

            link.PopupName = popupName;
            link.Prefab = prefab;
            Add(link, false, saveAssets);
            return true;
        }

        /// <summary> Deletes a reference from the database. Returns TRUE if the operation was successful </summary>
        /// <param name="reference"> Target link to be deleted </param>
        public bool DeletePopupLink(UIPopupLink reference)
        {
            if (reference == null || !Popups.Contains(reference)) return false; //sanity check
#if UNITY_EDITOR
            string popupName = reference.PopupName;
            if (!EditorUtility.DisplayDialog(UILabels.DeletePopupReference + " '" + popupName + "'",
                                             UILabels.AreYouSureYouWantToDeletePopupReference +
                                             "\n\n" +
                                             UILabels.OperationCannotBeUndone,
                                             UILabels.Yes,
                                             UILabels.No))
                return false;

            Popups.Remove(reference);
            DoozyUtils.MoveAssetToTrash(AssetDatabase.GetAssetPath(reference));
            UpdateListOfPopupNames();
            SetDirty(false);
#endif
            return true;
        }

        /// <summary>
        ///     Returns the prefab reference associated with the given popup name.
        ///     <para />
        ///     Returns null if the popup name has not been defined in the database.
        /// </summary>
        /// <param name="popupName"> Popup name to search for </param>
        public GameObject GetPrefab(string popupName)
        {
            foreach (UIPopupLink reference in Popups)
                if (reference.PopupName.Equals(popupName))
                    return reference.Prefab;
            return null;
        }

        /// <summary>
        ///     Returns the popup name defined for the given prefab.
        ///     <para />
        ///     Returns null if the prefab has not been referenced in the database.
        /// </summary>
        /// <param name="prefab"> Prefab to search for (null will get ignored and return NULL)</param>
        public string GetPopupName(UIPopup prefab)
        {
            if (prefab == null) return null;
            foreach (UIPopupLink reference in Popups)
                if (reference.Prefab == prefab.gameObject)
                    return reference.PopupName;
            return null;
        }

        /// <summary>
        /// Iterates through the database and returns the index where the popup name is found.
        /// Returns -1 if the popup name was not found.
        /// </summary>
        /// <param name="popupName"> Popup name to search for (null or empty will get ignored and return -1)</param>
        public int IndexOf(string popupName)
        {
            if (!Contains(popupName)) return -1;
            for (int i = 0; i < Popups.Count; i++)
                if (Popups[i].PopupName.Equals(popupName))
                    return i;

            return -1;
        }

        /// <summary>
        /// Iterates through the database and returns the index where the prefab reference is found.
        /// Returns -1 if the prefab reference was not found.
        /// </summary>
        /// <param name="prefab"> Prefab to search for (null will get ignored and return -1) </param>
        public int IndexOf(UIPopup prefab)
        {
            if (!Contains(prefab)) return -1;
            for (int i = 0; i < Popups.Count; i++)
                if (Popups[i].Prefab == prefab.gameObject)
                    return i;

            return -1;
        }

        /// <summary>
        /// Refreshes the entire database by adding removing empty and null entries,
        /// sorting the database and updating the category names list.
        /// </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshDatabase(bool performUndo, bool saveAssets)
        {
            string title = UILabels.Database + ": UIPopup"; // ProgressBar Title
            string info = UILabels.RefreshDatabase;         // ProgressBar Info

            DoozyUtils.DisplayProgressBar(title, info, 0.1f);
            if (performUndo) UndoRecord(UILabels.RefreshDatabase);
            DoozyUtils.DisplayProgressBar(title, info, 0.4f);
            RemoveUnreferencedData(false);
            DoozyUtils.DisplayProgressBar(title, info, 0.8f);
            Sort(false, false);
            UpdateListOfPopupNames();
            DoozyUtils.DisplayProgressBar(title, info, 1f);
            SetDirty(saveAssets);
            DoozyUtils.ClearProgressBar();
        }

        /// <summary> Removes the link with the passed popup name (if it exists) </summary>
        /// <param name="popupName"> Popup name to search for (null or empty will get ignored and return FALSE)</param>
        /// <param name="showDialog"> Should a display dialog be shown before executing the action </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveLink(string popupName, bool showDialog, bool saveAssets)
        {
            if (!Contains(popupName)) return;
#if UNITY_EDITOR
            if (showDialog)
            {
                if (!EditorUtility.DisplayDialog(UILabels.DeletePopupReference + " '" + popupName + "'",
                                                 UILabels.AreYouSureYouWantToDeletePopupReference + "\n\n" + UILabels.OperationCannotBeUndone,
                                                 UILabels.Yes,
                                                 UILabels.No))
                    return;
            }
#endif

            bool needSave = false;
            for (int i = Popups.Count - 1; i >= 0; i--)
            {
                UIPopupLink link = Popups[i];
                if (link.PopupName != popupName) continue;
                if (DeletePopupLink(link))
                    needSave = true;
            }

            if (needSave) SetDirty(saveAssets);
        }

        /// <summary> Removes any unreferenced UIPopupLinks from the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveUnreferencedData(bool saveAssets = false)
        {
#if UNITY_EDITOR
            if (Popups == null) Popups = new List<UIPopupLink>();
            bool needsSave = false;
            for (int i = Popups.Count - 1; i >= 0; i--)
                if (Popups[i] == null)
                {
                    Popups.RemoveAt(i);
                    needsSave = true;
                }

            if (!needsSave) return;
            UpdateListOfPopupNames();
            SetDirty(saveAssets);
#endif
        }

        /// <summary> Resets the database to the default values </summary>
        public bool ResetDatabase()
        {
#if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog(UILabels.ResetDatabase,
                                             UILabels.AreYouSureYouWantToResetDatabase +
                                             "\n\n" +
                                             UILabels.OperationCannotBeUndone,
                                             UILabels.Yes,
                                             UILabels.No))
                return false;

            for (int i = Popups.Count - 1; i >= 0; i--)
            {
                UIPopupLink link = Popups[i];
                Popups.Remove(link);
                DoozyUtils.MoveAssetToTrash(AssetDatabase.GetAssetPath(link));
            }

            UpdateListOfPopupNames();
            SetDirty(true);
            DDebug.Log(UILabels.DatabaseHasBeenReset);
#endif
            return true;
        }

        /// <summary> [Editor Only] Performs a deep search through the project for any unregistered UIPopupLink asset files and adds them to the database. The search is done only in all the Resources folders. </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SearchForUnregisteredLinks(bool saveAssets)
        {
            string title = UILabels.Database + ": UIPopup"; // ProgressBar Title
            string info = UILabels.Search;                  // ProgressBar Info

            DoozyUtils.DisplayProgressBar(title, info, 0.1f);
            bool foundUnregisteredLink = false;
            UIPopupLink[] array = Resources.LoadAll<UIPopupLink>("");
            if (array == null || array.Length == 0)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            if (Popups == null) Popups = new List<UIPopupLink>();
            for (int i = 0; i < array.Length; i++)
            {
                DoozyUtils.DisplayProgressBar(title, info, 0.1f + 0.8f * (i + 1) / array.Length);
                UIPopupLink link = array[i];
                if (Popups.Contains(link)) continue;
                Popups.Add(link);
                foundUnregisteredLink = true;
            }

            DoozyUtils.DisplayProgressBar(title, info, 1f);
            if (!foundUnregisteredLink)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            UpdateListOfPopupNames();
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
            Popups = Popups.OrderBy(reference => reference.PopupName).ToList();
            SetDirty(saveAssets);
//            DDebug.Log(UILabels.DatabaseSorted);
        }

        /// <summary> [Editor Only] Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Updates the list of all the popup names as defined in the database </summary>
        public void UpdateListOfPopupNames()
        {
            PopupNames.Clear();
            foreach (UIPopupLink reference in Popups)
                PopupNames.Add(reference.PopupName);
            SetDirty(false);
        }

        #endregion
    }
}