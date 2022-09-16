// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    public class ProcessorsSettings : ScriptableObject
    {
        private static string AssetPath { get { return Path.Combine(DoozyPath.EDITOR_INTERNAL_PATH, "ProcessorsSettings.asset"); } }

        private static ProcessorsSettings s_instance;

        public static ProcessorsSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<ProcessorsSettings>(AssetPath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<ProcessorsSettings>();
                AssetDatabase.CreateAsset(s_instance, AssetPath);
                DoozyUtils.SetDirty(s_instance, false);
                return s_instance;
            }
        }
        
        public static void ResetInstance() { s_instance = null; }

        public bool RunFoldersProcessor = true;
        public bool RunDoozyAssetsProcessor = true;

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}