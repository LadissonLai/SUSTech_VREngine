// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Animation
{
    /// <inheritdoc />
    /// <summary>
    ///     NamesDatabaseType of database that contains references to UIAnimationData assets, used by the system to save and load animation presets
    /// </summary>
    [Serializable]
    public class UIAnimationDatabase : ScriptableObject
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels
        {
            get { return UILanguagePack.Instance; }
        }

        #endregion

        #region Public Variables

        /// <summary> List of all the UIAnimationData animation names that this database contains </summary>
        public List<string> AnimationNames = new List<string>();

        /// <summary> List of references to UIAnimationData assets </summary>
        public List<UIAnimationData> Database = new List<UIAnimationData>();

        /// <summary> The database name </summary>
        public string DatabaseName;

        /// <summary> The animation type that determines what type of animations this database contains </summary>
        public AnimationType DataType;

        #endregion

        #region Public Methods

        /// <summary> [Editor Only] Adds a new entry, a UIAnimationData, to the database. Returns TRUE if the operation was successful </summary>
        /// <param name="animation"> The UIAnimation that contains the animation settings to add to the database </param>
        /// <param name="animationName"> The animation (preset) name to add to the database </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Add(UIAnimation animation, string animationName, bool saveAssets = true)
        {
            animationName = animationName.Trim();
            if (Contains(animationName)) return false;
            UndoRecord(UILabels.CreateAnimation);
            var data = CreateInstance<UIAnimationData>();
            data.Category = DatabaseName;
            data.Name = animationName;
            data.name = data.Name;
            data.Animation = animation;
            Database.Add(data);
            AddObjectToAsset(data);
            data.SetDirty(false);
            RefreshDatabase(false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> [Editor Only] Adds the 'Default' preset name to the database if it's missing. Returns a reference to the UIAnimationData for the 'Default' preset </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public UIAnimationData AddDefaultData(bool saveAssets)
        {
            if (Contains(UIAnimations.DEFAULT_PRESET_NAME)) return Get(UIAnimations.DEFAULT_PRESET_NAME);
            var data = CreateInstance<UIAnimationData>();
            data.Category = DatabaseName;
            data.Name = UIAnimations.DEFAULT_PRESET_NAME;
            data.name = data.Name;
            data.Animation = new UIAnimation(DataType);
            AddObjectToAsset(data);
            data.SetDirty(false);
            SetDirty(saveAssets);

            return data;
        }

        /// <summary> Returns TRUE if the animation name has been found in the database </summary>
        /// <param name="animationName"> Target animation name to search for </param>
        public bool Contains(string animationName) { return Contains(Get(animationName)); }

        /// <summary> Returns TRUE if the UIAnimationData has been found in the database </summary>
        /// <param name="data"> UIAnimationData to search for </param>
        public bool Contains(UIAnimationData data) { return data != null && Database.Contains(data); }

        /// <summary> [Editor Only] Creates a new preset with the given preset name and UIAnimation settings </summary>
        /// <param name="newPresetName"> The animation name for the new preset </param>
        /// <param name="animation"> The animation settings for the new preset </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void CreatePreset(string newPresetName, UIAnimation animation, bool saveAssets = true) { Add(animation.Copy(), newPresetName, saveAssets); }

        /// <summary> [Editor Only] Iterates through the database to look for the animation name. If found, deletes the entry and the asset file and returns TRUE </summary>
        /// <param name="animationName"> The animation name to search for </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Delete(string animationName, bool saveAssets) { return Delete(Get(animationName), saveAssets); }

        /// <summary> [Editor Only] Iterates through the database to look for the UIAnimationData. If found, deletes the entry and the asset file and returns TRUE </summary>
        /// <param name="data"> UIAnimationData to search for </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public bool Delete(UIAnimationData data, bool saveAssets)
        {
            if (data == null) return false;
            if (!Database.Contains(data)) return false;
            Database.Remove(data);
            DestroyImmediate(data, true);
            RefreshDatabase(false);
            SetDirty(saveAssets);
            return true;
        }

        /// <summary> Iterates through the database to look for the animation name. If found, returns a reference to the corresponding UIAnimationData, else it returns null </summary>
        /// <param name="animationName"> The animation name to search for </param>
        public UIAnimationData Get(string animationName)
        {
            foreach (UIAnimationData data in Database)
                if (data.Name.Equals(animationName))
                    return data;
            return null;
        }

        /// <summary> Refreshes the entire database by removing nulls, renaming the preset file names to their animation names, sorting the database and updating the animation names list </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RefreshDatabase(bool saveAssets)
        {
            RemoveNullEntries(false);
            RenameAssetFileNamesToReflectAnimationNames();
            Sort(false);

            if (DatabaseName.Equals(UIAnimations.DEFAULT_DATABASE_NAME))
            {
                UIAnimationData defaultData = AddDefaultData(true);
                Database.Remove(defaultData);
                Database.Insert(0, defaultData);
            }

            UpdateAnimationNames(saveAssets);
        }

        /// <summary> [Editor Only] Removes any entries that do not have an UIAnimationData reference </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void RemoveNullEntries(bool saveAssets)
        {
            Database = Database.Where(data => data != null).ToList();
            SetDirty(saveAssets);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Sorts the entire database by animation name </summary>
        public void Sort(bool saveAssets)
        {
            Database = Database.OrderBy(data => data.Name).ToList();
            SetDirty(saveAssets);
        }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }

        /// <summary> Updates the list of animation names found in the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        private void UpdateAnimationNames(bool saveAssets)
        {
            if (AnimationNames == null) AnimationNames = new List<string>();
            AnimationNames.Clear();
            foreach (UIAnimationData data in Database)
                AnimationNames.Add(data.Name);

            SetDirty(saveAssets);
        }

        #endregion

        #region Private Methods

        /// <summary>[Editor Only] Adds an object to this asset </summary>
        /// <param name="objectToAdd"> Object that will get added under the asset</param>
        private void AddObjectToAsset(Object objectToAdd) { DoozyUtils.AddObjectToAsset(objectToAdd, this); }

        /// <summary> [Editor Only] Renames an UIAnimationData animation name (including the asset filename). Returns TRUE if the operation was successful </summary>
        /// <param name="oldAnimationName"> The previous animation name</param>
        /// <param name="newAnimationName"> The new animation name </param>
        private void Rename(string oldAnimationName, string newAnimationName)
        {
            UIAnimationData data = Get(oldAnimationName);
            if (data == null) return;
            newAnimationName = newAnimationName.Trim();
            if (Contains(newAnimationName)) return;
            DoozyUtils.UndoRecordObjects(new Object[] {data, this}, UILabels.Rename);
            data.Name = newAnimationName;
            data.name = data.Name;
            data.SetDirty(false);
            SetDirty(false);
            UpdateAnimationNames(true);
        }

        /// <summary>
        ///     [Editor Only] Renames all the UIAnimationData assets referenced in this database to their set animation names.
        ///     This aligns the filename to the animation name of each preset.
        /// </summary>
        private void RenameAssetFileNamesToReflectAnimationNames()
        {
            bool assetRenamed = false;
            foreach (UIAnimationData data in Database)
            {
                if (data == null || data.name.Equals(data.Name)) continue;
                data.name = data.Name;
                data.SetDirty(false);
                assetRenamed = true;
            }

            if (assetRenamed) SetDirty(true);
        }

        #endregion
    }
}