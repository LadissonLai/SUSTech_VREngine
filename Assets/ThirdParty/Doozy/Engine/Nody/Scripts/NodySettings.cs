// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Nody
{
    public class NodySettings : ScriptableObject
    {
        public const string FILE_NAME = "NodySettings";
        private static string ResourcesPath { get { return DoozyPath.ENGINE_NODY_RESOURCES_PATH; } }

        private static NodySettings s_instance;

        public static NodySettings Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = AssetUtils.GetScriptableObject<NodySettings>(FILE_NAME, ResourcesPath, false, false);
                return s_instance;
            }
        }

        #region Values

        [Header("Opacity Values")] public float NormalOpacity = 0.8f;
        public float ActiveOpacity = 0.6f;
        public float HoverOpacity = 1.0f;

        [Header("Node GUI")] 
        public float FooterHeight = 10f;
        public float MaxNodeWidth = 300f;
        public float MinNodeWidth = 100f;
        public float NodeAccentLineHeight = 2f;
        public float NodeAddSocketButtonSize = 12f;
        public float NodeBodyOpacity = 0.7f;
        public float NodeDeleteButtonSize = 20f;
        public float NodeGlowOpacity = 0.2f;
        public float NodeHeaderHeight = 32f;
        public float NodeHeaderIconSize = 20f;
        public float PingColorChangeSpeed = 0.6f;

        [Header("Socket GUI")] 
        public float SocketConnectedOpacity = 1f;
        public float SocketCurveModifierMaxValue = 1f;
        public float SocketCurveModifierMinValue = -1f;
        public float SocketDividerHeight = 1f;
        public float SocketDividerOpacity = 0.3f;
        public float SocketHeight = 24f;
        public float SocketNotConnectedOpacity = 0.5f;
        public float SocketVerticalSpacing;

        [Header("Connection GUI")] 
        public float ConnectionPointHeight = 16f;
        public float ConnectionPointOffsetFromLeftMargin = -2f;
        public float ConnectionPointOffsetFromRightMargin = 2f;
        public float ConnectionPointWidth = 16f;

        [Header("Curve Settings")] 
        public float CurvePointsMultiplier = 3f;
        public float CurveStrengthModifier = 0.48f;
        public float CurveWidth = 3f;
        public float DefaultCurveModifier;
        public float MaxCurveModifier = 0.5f;
        public float MinCurveModifier = -0.5f;

        [Header("Graph Tabs")]
        public float GraphTabDividerWidth = 1f;
        public float GraphTabElementSpacing = 4f;
        public float GraphTabMaximumWidth = 200f;
        public float GraphTabMinimumWidth = 40;
        public float GraphTabsAreaHeight = 40f;
        public float GraphTabsBackgroundOpacity = 0.8f;

        [Header("Repaint Intervals")] 
        public double RepaintIntervalDuringPlayMode = 0.4f;
        public double RepaintIntervalWhileIdle = 0.6f;

        [Header("Editor Prefs Keys")]
        public string EditorPrefsKeyWindowToolbar = "Doozy.Nody.WindowToolbar";
        public string EditorPrefsKeyDotAnimationSpeed = "Doozy.Nody.DotAnimationSpeed";

        [Header("Default Node Sizes")] 
        public float DefaultNodeHeight = 216f;
        public float DefaultNodeWidth = 216f;
        public float EnterNodeWidth = 120f;
        public float ExitNodeWidth = 120f;
        public float StartNodeWidth = 120f;
        public float SubGraphNodeWidth = 216f;
        public float SwitchBackNodeWidth = 216f;

        [Header("Misc")] 
        public HideFlags DefaultHideFlagsForNodes = HideFlags.HideInHierarchy; //HideFlags.None;

        #endregion

        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }

        /// <summary> Records any changes done on the object after this function </summary>
        /// <param name="undoMessage"> The title of the action to appear in the undo history (i.e. visible in the undo menu) </param>
        public void UndoRecord(string undoMessage) { DoozyUtils.UndoRecordObject(this, undoMessage); }
    }
}