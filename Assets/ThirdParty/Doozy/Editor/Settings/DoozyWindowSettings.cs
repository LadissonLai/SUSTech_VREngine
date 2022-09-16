// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Settings
{
    public class DoozyWindowSettings :  ScriptableObject
    {
        private static string AssetPath { get { return Path.Combine(DoozyPath.EDITOR_SETTINGS_PATH, "DoozyWindowSettings.asset"); } }
        
        private static DoozyWindowSettings s_instance;
        public static DoozyWindowSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<DoozyWindowSettings>(AssetPath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<DoozyWindowSettings>();
                AssetDatabase.CreateAsset(s_instance, AssetPath);
                DoozyUtils.SetDirty(s_instance, false);
                return s_instance;
            }
        }
        
        public bool DynamicToolbarExpanded = true;
        public float DynamicToolbarButtonHeight = 48f;
        public float DynamicToolbarButtonIconSize = 16f;
        public float DynamicToolbarButtonSpacing = 2f;
        public float DynamicToolbarButtonWidth = 64f;
        public float DynamicToolbarPadding = 4f;
        public float ToolbarAnimationSpeed = 2f;
        public float ToolbarButtonHeight = 32f;
        public float ToolbarCollapsedWidth = 32f;
        public float ToolbarExpandCollapseButtonHeight = 24f;
        public float ToolbarExpandedWidth = 128f;
        public float ToolbarHeaderHeight = 64f;
        public float ToolbarShadowWidth = 4f;
        public float WindowViewContentHorizontalPadding = 8f;
        public string EditorPrefsKeyWindowCurrentView = "Doozy.Window.CurrentView";
        public string EditorPrefsKeyWindowToolbarState = "Doozy.Window.ToolbarState";
        public string EditorPrefsKeyWindowCurrentTheme = "Doozy.Window.CurrentTheme";
        public string WindowTitle = "DoozyUI";

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}