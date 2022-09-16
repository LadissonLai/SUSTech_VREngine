// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Doozy.Engine.Soundy
{
    [Serializable]
    public class SoundySettings : ScriptableObject
    {
        public const string FILE_NAME = "SoundySettings";
        private static string ResourcesPath { get { return DoozyPath.ENGINE_SOUNDY_RESOURCES_PATH; } }

        private static SoundySettings s_instance;

        public static SoundySettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<SoundySettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        [SerializeField] private SoundyDatabase database;

        public static SoundyDatabase Database
        {
            get
            {
                if (Instance.database != null) return Instance.database;
                UpdateDatabase();
                return Instance.database;
            }
        }

        public static void UpdateDatabase()
        {
            Instance.database = AssetUtils.GetScriptableObject<SoundyDatabase>("_" + DoozyPath.SOUNDY_DATABASE, DoozyPath.GetDataPath(DoozyPath.ComponentName.Soundy), false, false);
#if UNITY_EDITOR
            if (Instance.database == null) return;
            Instance.database.Initialize();
            Instance.database.SearchForUnregisteredDatabases(false);
            Instance.SetDirty(true);
#endif
        }

        public const bool AUTO_KILL_IDLE_CONTROLLERS_DEFAULT_VALUE = true;
        public const float CONTROLLER_IDLE_KILL_DURATION_DEFAULT_VALUE = 20f;
        public const float CONTROLLER_IDLE_KILL_DURATION_MIN = 0f;
        public const float CONTROLLER_IDLE_KILL_DURATION_MAX = 300f;
        public const float IDLE_CHECK_INTERVAL_DEFAULT_VALUE = 5f;
        public const float IDLE_CHECK_INTERVAL_MIN = 0.1f;
        public const float IDLE_CHECK_INTERVAL_MAX = 60f;
        public const int MINIMUM_NUMBER_OF_CONTROLLERS_DEFAULT_VALUE = 3;
        public const int MINIMUM_NUMBER_OF_CONTROLLERS_MIN = 0;
        public const int MINIMUM_NUMBER_OF_CONTROLLERS_MAX = 20;

        public bool AutoKillIdleControllers = AUTO_KILL_IDLE_CONTROLLERS_DEFAULT_VALUE;
        public float ControllerIdleKillDuration = CONTROLLER_IDLE_KILL_DURATION_DEFAULT_VALUE;
        public float IdleCheckInterval = IDLE_CHECK_INTERVAL_DEFAULT_VALUE;
        public int MinimumNumberOfControllers = MINIMUM_NUMBER_OF_CONTROLLERS_DEFAULT_VALUE;

        private void Reset()
        {
            AutoKillIdleControllers = AUTO_KILL_IDLE_CONTROLLERS_DEFAULT_VALUE;
            ControllerIdleKillDuration = CONTROLLER_IDLE_KILL_DURATION_DEFAULT_VALUE;
            IdleCheckInterval = IDLE_CHECK_INTERVAL_DEFAULT_VALUE;
            MinimumNumberOfControllers = MINIMUM_NUMBER_OF_CONTROLLERS_DEFAULT_VALUE;
        }
        
        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }

        public void ResetComponent(SoundyPooler pooler)
        {
            
        }
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}