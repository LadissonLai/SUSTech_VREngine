// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Settings
{
    public class NodyWindowSettings : ScriptableObject
    {
        private static string AssetPath { get { return Path.Combine(DoozyPath.EDITOR_NODY_SETTINGS_PATH, "NodyWindowSettings.asset"); } }

        private static NodyWindowSettings s_instance;

        public static NodyWindowSettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetDatabase.LoadAssetAtPath<NodyWindowSettings>(AssetPath);
                if (s_instance != null) return s_instance;
                s_instance = CreateInstance<NodyWindowSettings>();
                AssetDatabase.CreateAsset(s_instance, AssetPath);
                DoozyUtils.SetDirty(s_instance, false);
                return s_instance;
            }
        }

        private static readonly AnimationCurve DotAnimationCurveDefaultValue = new AnimationCurve(new Keyframe(0.0f, 0.0f, 1.5f, 1.5f), new Keyframe(1.0f, 1.0f, 0.2f, 0.2f));
        private static readonly Vector2 PanOffsetDefaultValue = Vector2.zero;

        public const float DOT_ANIMATION_SPEED_DEFAULT_VALUE = 0.6f;
        public const float DOT_ANIMATION_SPEED_MAX = 2f;
        public const float DOT_ANIMATION_SPEED_MIN = 0.2f;
        public const float RECENT_GRAPHS_AREA_WIDTH = 200f;
        public const bool SHOW_NODE_NOTES_DEFAULT_VALUE = true;
        public const float SNAP_TO_GRID_SIZE = 12f;
        public const float TOOLBAR_OPACITY = 0.95f;
        public const float ZOOM_DEFAULT_VALUE = 1f;
        public const float ZOOM_DRAW_THRESHOLD_FOR_CONNECTIONS = 0.2f;
        public const float ZOOM_DRAW_THRESHOLD_FOR_CONNECTION_POINTS = 0.4f;
        public const float ZOOM_DRAW_THRESHOLD_FOR_SOCKETS = 0.4f;
        public const float ZOOM_MAX = 2f;
        public const float ZOOM_MIN = 0.1f;
        public const float ZOOM_OVERVIEW = 0.6f;
        public const float ZOOM_STEP = 0.1f;

        public AnimationCurve DotAnimationCurve = DotAnimationCurveDefaultValue;
        public Graph CurrentGraph;
        public List<Graph> RecentlyOpenedGraphs = new List<Graph>();
        public List<Node> CopiedNodes = new List<Node>();
        public List<Node> DeletedNodes = new List<Node>();
        public List<Node> SelectedNodes = new List<Node>();
        public List<NodyWindow.GraphTab> GraphTabs = new List<NodyWindow.GraphTab>();
        public NodyWindow.View CurrentView = NodyWindow.View.General;
        public Vector2 CurrentPanOffset = PanOffsetDefaultValue;
        public Vector2 DefaultPanOffset = PanOffsetDefaultValue;
        public bool IsDirty;
        public bool SaveCurrentGraphWithControlS = true;
        public float CurrentDotAnimationSpeed = DOT_ANIMATION_SPEED_DEFAULT_VALUE;
        public float CurrentZoom = ZOOM_DEFAULT_VALUE;
        public float DefaultDotAnimationSpeed = DOT_ANIMATION_SPEED_DEFAULT_VALUE;
        public float DefaultZoom = ZOOM_DEFAULT_VALUE;
        public bool ShowNodeNotes = SHOW_NODE_NOTES_DEFAULT_VALUE;

        public bool CanPasteNodes { get { return CopiedNodes.Count > 0; } }

        public void Initialize(bool saveAssets = false)
        {
            bool isDirty = false;

            if (CurrentZoom <= 0)
            {
                CurrentZoom = ZOOM_DEFAULT_VALUE;
                isDirty = true;
            }

            if (CurrentDotAnimationSpeed <= 0)
            {
                CurrentDotAnimationSpeed = DOT_ANIMATION_SPEED_DEFAULT_VALUE;
                isDirty = true;
            }

            if (DotAnimationCurve == null)
            {
                DotAnimationCurve = DotAnimationCurveDefaultValue;
                isDirty = true;
            }

            if (RecentlyOpenedGraphs == null)
            {
                RecentlyOpenedGraphs = new List<Graph>();
                isDirty = true;
            }

            if (GraphTabs == null)
            {
                GraphTabs = new List<NodyWindow.GraphTab>();
                isDirty = true;
            }

            if (SelectedNodes == null)
            {
                SelectedNodes = new List<Node>();
                isDirty = true;
            }

            if (DeletedNodes == null)
            {
                DeletedNodes = new List<Node>();
                isDirty = true;
            }

            if (CopiedNodes == null)
            {
                CopiedNodes = new List<Node>();
                isDirty = true;
            }

            if (isDirty) SetDirty(saveAssets);
        }

        public void ClearRecentlyOpenedGraphs(bool saveAssets = false)
        {
            RecentlyOpenedGraphs = new List<Graph>();
            SetDirty(saveAssets);
        }

        public void AddGraphToRecentlyOpenedGraphs(Graph graph)
        {
            if (graph == null) return; //check that that graph is not null 

            if (RecentlyOpenedGraphs == null)
                RecentlyOpenedGraphs = new List<Graph>(); //make sure we have a valid list

            for (int i = RecentlyOpenedGraphs.Count - 1; i >= 0; i--) //remove any null references from the recent list (the dev might have deleted the assets)
                if (RecentlyOpenedGraphs[i] == null)
                    RecentlyOpenedGraphs.RemoveAt(i);

            if (RecentlyOpenedGraphs.Contains(graph)) //check if the graph that is being opened has been opened before and remove it from the recent list
                RecentlyOpenedGraphs.Remove(graph);

            RecentlyOpenedGraphs.Insert(0, graph); //insert the graph that is being opened to the top of the list

            SetDirty(false);
        }

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}