// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Animation
{
    /// <inheritdoc />
    /// <summary>
    ///     Central animations database that knows about all of the UIAnimationDatabase databases.
    ///     Each database type (Show, Hide, Loop, Punch, State) has its own UIAnimations.Database container, where all UIAnimationDatabase of a given type are referenced.
    /// </summary>
    [Serializable]
    public class UIAnimations : ScriptableObject
    {
        #region Constants

        private const string FILE_NAME = "_UIAnimations";
        public const string DEFAULT_DATABASE_NAME = "Uncategorized";
        public const string DEFAULT_PRESET_NAME = "Default";

        #endregion

        #region Static Properties

        private static UIAnimations s_instance;

        /// <summary> Returns a reference to the UIAnimations asset </summary>
        public static UIAnimations Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = Resources.Load<UIAnimations>(Path.Combine(DoozyPath.UIANIMATIONS, FILE_NAME));
                if (s_instance != null)
                {
                    s_instance.Initialize();
                    return s_instance;
                }
#if UNITY_EDITOR
                s_instance = AssetUtils.CreateAsset<UIAnimations>(DoozyPath.UIANIMATIONS_RESOURCES_PATH, FILE_NAME);
#endif
                if (s_instance != null)
                {
                    s_instance.SearchForUnregisteredDatabases(true);
                    s_instance.Initialize();
                }
                return s_instance;
            }
        }

        #endregion

        #region Public Variables

        /// <summary> Show animations databases </summary>
        public UIAnimationsDatabase Show;

        /// <summary> Hide animations databases </summary>
        public UIAnimationsDatabase Hide;

        /// <summary> Loop animations databases </summary>
        public UIAnimationsDatabase Loop;

        /// <summary> Punch animations databases </summary>
        public UIAnimationsDatabase Punch;

        /// <summary> State animations databases </summary>
        public UIAnimationsDatabase State;

        #endregion

        #region Public Methods

        /// <summary> Creates a new UIAnimationDatabase of the given type and preset category name. Returns a reference to the newly created UIAnimationDatabase </summary>
        /// <param name="databaseType"> Database animations type </param>
        /// <param name="newPresetCategory"> Database (preset category) name </param>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public UIAnimationDatabase CreateDatabase(AnimationType databaseType, string newPresetCategory, bool saveAssets = false)
        {
#if UNITY_EDITOR
            var database = AssetUtils.CreateAsset<UIAnimationDatabase>(Path.Combine(DoozyPath.UIANIMATIONS_RESOURCES_PATH, databaseType.ToString()), newPresetCategory, ".asset", false, false);
#else
                var database = CreateInstance<UIAnimationDatabase>();
#endif
            database.DatabaseName = newPresetCategory;
            database.name = database.DatabaseName;
            database.DataType = databaseType;
            database.RefreshDatabase(false);
            Get(databaseType).AddUIAnimationDatabase(database);
            SetDirty(saveAssets);
            return database;
        }

        /// <summary> Returns the global database for the selected database type (AnimationType) </summary>
        /// <param name="databaseType"> Database animations type </param>
        public UIAnimationsDatabase Get(AnimationType databaseType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (databaseType)
            {
                case AnimationType.Show:  return Show;
                case AnimationType.Hide:  return Hide;
                case AnimationType.Loop:  return Loop;
                case AnimationType.Punch: return Punch;
                case AnimationType.State: return State;
                default:                  return null;
            }
        }

        /// <summary>
        ///     Iterates through all the databases of the given database type (AnimationType) to find the one that has the given database name, then looks for the animation name.
        ///     If found, returns a reference to the corresponding UIAnimationData, else it returns null.
        /// </summary>
        /// <param name="databaseType"> The type of animations contained in the target database </param>
        /// <param name="databaseName"> The database name to search for </param>
        /// <param name="animationName"> The animation name to search for </param>
        public UIAnimationData Get(AnimationType databaseType, string databaseName, string animationName)
        {
            UIAnimationDatabase category = Get(databaseType, databaseName);
            return category.Get(animationName);
        }

        /// <summary>
        ///     Iterates through all the databases of the given database type (AnimationType) to find the one that has the given database name.
        ///     If found, returns a reference to the corresponding UIAnimationDatabase, else it returns null.
        /// </summary>
        /// <param name="databaseType"> The type of animations contained in the target database </param>
        /// <param name="databaseName"> The database name to search for </param>
        public UIAnimationDatabase Get(AnimationType databaseType, string databaseName)
        {
            UIAnimationsDatabase database = Get(databaseType);
            return database.Get(databaseName);
        }

        /// <summary>
        ///     Performs an initial check to make sure that all the UIAnimation.Database references are not null (if they are, it generates the missing ones).
        ///     After the references have been validated, the databases are updated.
        /// </summary>
        public void Initialize()
        {
            bool needsSave = false;

            if (Show == null)
            {
                Show = new UIAnimationsDatabase(AnimationType.Show);
                needsSave = true;
            }

            if (Show.DatabaseType != AnimationType.Show)
            {
                Show.DatabaseType = AnimationType.Show;
                needsSave = true;
            }


            if (Hide == null)
            {
                Hide = new UIAnimationsDatabase(AnimationType.Hide);
                needsSave = true;
            }

            if (Hide.DatabaseType != AnimationType.Hide)
            {
                Hide.DatabaseType = AnimationType.Hide;
                needsSave = true;
            }


            if (Loop == null)
            {
                Loop = new UIAnimationsDatabase(AnimationType.Loop);
                needsSave = true;
            }

            if (Loop.DatabaseType != AnimationType.Loop)
            {
                Loop.DatabaseType = AnimationType.Loop;
                needsSave = true;
            }


            if (Punch == null)
            {
                Punch = new UIAnimationsDatabase(AnimationType.Punch);
                needsSave = true;
            }

            if (Punch.DatabaseType != AnimationType.Punch)
            {
                Punch.DatabaseType = AnimationType.Punch;
                needsSave = true;
            }

            if (State == null)
            {
                State = new UIAnimationsDatabase(AnimationType.State);
                needsSave = true;
            }

            if (State.DatabaseType != AnimationType.State)
            {
                State.DatabaseType = AnimationType.State;
                needsSave = true;
            }


            Show.Update();
            Hide.Update();
            Loop.Update();
            Punch.Update();
            State.Update();

            if (needsSave) SetDirty(true);
        }

        /// <summary> [Editor Only] Performs a deep search through the project for any unregistered UIAnimationDatabase asset files and adds them to the corresponding Database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SearchForUnregisteredDatabases(bool saveAssets)
        {
            DoozyUtils.DisplayProgressBar("UIAnimations", "Initializing", 0.1f);
            Initialize();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Searching for databases", 0.3f);

            bool foundUnregisteredDatabase = false;
            UIAnimationDatabase[] array = Resources.LoadAll<UIAnimationDatabase>("");
            if (array == null || array.Length == 0)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            DoozyUtils.DisplayProgressBar("UIAnimations", "Updating databases", 0.5f);

            foreach (UIAnimationDatabase foundDatabase in array)
            {
                UIAnimationsDatabase database = Get(foundDatabase.DataType);
                if (database == null) continue;
                if (database.Contains(foundDatabase)) continue;
                database.AddUIAnimationDatabase(foundDatabase);
                foundUnregisteredDatabase = true;
            }

            if (!foundUnregisteredDatabase)
            {
                DoozyUtils.ClearProgressBar();
                return;
            }

            DoozyUtils.DisplayProgressBar("UIAnimations", "Update Show", 0.6f);
            Show.Update();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Update Hide", 0.6f);
            Hide.Update();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Update Loop", 0.7f);
            Loop.Update();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Update Punch", 0.8f);
            Punch.Update();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Update State", 0.9f);
            State.Update();
            DoozyUtils.DisplayProgressBar("UIAnimations", "Saving database", 1f);
            SetDirty(saveAssets);
            DoozyUtils.ClearProgressBar();
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Iterates through all the UIAnimationDatabase databases of the given database type (AnimationType) to find the one that has the given database name (preset category), then looks for the animation name (preset name).
        ///     If found, returns a deep copy of the corresponding UIAnimation, else it returns null.
        /// </summary>
        /// <param name="animationType"> The type of animations contained in the target database</param>
        /// <param name="presetCategory"> The database name to search for </param>
        /// <param name="presetName"> The animation name to search for </param>
        public static UIAnimation LoadPreset(AnimationType animationType, string presetCategory, string presetName)
        {
            UIAnimationData data = Instance.Get(animationType, presetCategory, presetName);
            return data == null ? null : data.Animation.Copy();
        }

        #endregion

        #region Classes

        #endregion
    }
}