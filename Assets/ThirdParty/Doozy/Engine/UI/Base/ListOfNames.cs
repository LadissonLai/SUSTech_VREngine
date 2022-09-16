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

namespace Doozy.Engine.UI.Base
{
    /// <inheritdoc />
    /// <summary> Contains a list of names that belong to a category </summary>
    [Serializable]
    public class ListOfNames : ScriptableObject
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Public Variables

        /// <summary> Category name </summary>
        public string CategoryName;

        /// <summary> Defines which type of component this database is for </summary>
        public NamesDatabaseType DatabaseType = NamesDatabaseType.UIView;

        /// <summary> List of names </summary>
        public List<string> Names = new List<string>();

        #endregion

        #region Public Methods

        /// <summary> Adds a new entry to the names list </summary>
        /// <param name="value"> Name to add to the list </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddName(string value, bool performUndo, bool saveAssets = false)
        {
            value = value.Trim();
            if (string.IsNullOrEmpty(value)) return;
            if (Names == null) Names = new List<string>();
            if (Names.Contains(value)) return;
            if (performUndo) UndoRecord(UILabels.AddItem);
            Names.Add(value);
            SetDirty(saveAssets);
        }

        /// <summary> Adds a list of entries to the names list </summary>
        /// <param name="names"> List of names </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void AddNames(List<string> names, bool performUndo, bool saveAssets = false)
        {
            if (names == null) return;
            if (Names == null) Names = new List<string>();
            if (performUndo) UndoRecord(UILabels.AddItem);
            foreach (string s in names)
            {
                if (Names.Contains(s)) continue;
                Names.Add(s);
            }

            SetDirty(saveAssets);
        }

        /// <summary> Clears the manes list </summary>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Clear(bool performUndo, bool saveAssets = false)
        {
            if (Names == null) Names = new List<string>();
            if (performUndo) UndoRecord(UILabels.AddItem);
            Names.Clear();
            SetDirty(saveAssets);
        }

        /// <summary> Returns TRUE if the value was found in the names list </summary>
        /// <param name="value"> Name to search for </param>
        public bool Contains(string value)
        {
            if (Names == null) Names = new List<string>();
            return Names.Contains(value);
        }

        /// <summary> Removes any duplicate entries from the names list </summary>
        public void RemoveDuplicateNames()
        {
            if (Names == null) Names = new List<string>();
            int count = Names.Count;
            Names = Names.Distinct().ToList();
            if(count != Names.Count) SetDirty(false);
        }
        
        /// <summary> Removes any empty entries from the names list </summary>
        public void RemoveEmptyNames()
        {
            if (Names == null) Names = new List<string>();
            bool setDirty = false;
            for (int i = Names.Count - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(Names[i].Trim())) continue;
                Names.RemoveAt(i);
                setDirty = true;
            }
            if(setDirty) SetDirty(false);
        }

        /// <summary> Removes an entry from the names list </summary>
        /// <param name="value"> Name to remove from the list </param>
        /// <param name="performUndo"> Record changes? </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveName(string value, bool performUndo, bool saveAssets = false)
        {
            value = value.Trim();
            if (string.IsNullOrEmpty(value)) return;
            if (Names == null) Names = new List<string>();
            if (!Names.Contains(value)) return;
            if (performUndo) UndoRecord(UILabels.AddItem);
            Names.Remove(value);
            SetDirty(saveAssets);
        }

        /// <summary> [Editor Only] Renames the category name and the asset name </summary>
        /// <param name="newCategoryName"> New category name </param>
        /// <param name="newAssetName"> New asset name </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void Rename(string newCategoryName, string newAssetName, bool saveAssets)
        {
#if UNITY_EDITOR
            newCategoryName = newCategoryName.Trim();
            if (string.IsNullOrEmpty(newCategoryName)) return;
            if (string.IsNullOrEmpty(newAssetName)) return;
            CategoryName = newCategoryName;
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), newAssetName);
            SetDirty(saveAssets);
#endif
        }


        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        #endregion
    }
}