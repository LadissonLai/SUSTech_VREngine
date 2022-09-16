// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine;
using Doozy.Engine.Utils;
using Doozy.Engine.UI.Settings;
using UnityEngine;

namespace Doozy.Engine.Touchy
{
    [Serializable]
    public class TouchySettings : ScriptableObject
    {
        public const string FILE_NAME = "TouchySettings";
        private static string ResourcesPath { get { return DoozyPath.ENGINE_TOUCHY_RESOURCES_PATH; } }

        private static TouchySettings s_instance;

        public static TouchySettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<TouchySettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        public const float LONG_TAP_DURATION_DEFAULT_VALUE = 0.4f;
        public const float LONG_TAP_DURATION_MAX = 1f;
        public const float LONG_TAP_DURATION_MIN = 0.2f;
        public const float SWIPE_LENGTH_DEFAULT_VALUE = 2f;
        public const float SWIPE_LENGTH_MAX = 200f;
        public const float SWIPE_LENGTH_MIN = 0.1f;

        /// <summary> Time duration, for a light touch on a touch-sensitive screen, to be considered a long tap (long press) </summary>
        public float LongTapDuration = LONG_TAP_DURATION_DEFAULT_VALUE;
        
        /// <summary> Minimum travel distance, for a sliding a finger or a stylus pen across a touch-sensitive screen, to be considered a swipe </summary>
        [Range(SWIPE_LENGTH_MIN, SWIPE_LENGTH_MAX)]
        public float SwipeLength = SWIPE_LENGTH_DEFAULT_VALUE;

        private void Reset()
        {
            SwipeLength = SWIPE_LENGTH_DEFAULT_VALUE;
            LongTapDuration = LONG_TAP_DURATION_DEFAULT_VALUE;
        }
        
        public void Reset(bool saveAssets)
        {
            Reset();
            SetDirty(saveAssets);
        }
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}