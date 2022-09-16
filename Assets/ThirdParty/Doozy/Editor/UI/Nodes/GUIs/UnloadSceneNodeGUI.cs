// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.UI.Nodes;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(UnloadSceneNode))]
    public class UnloadSceneNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconUnloadSceneNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }
        
        private UnloadSceneNode TargetNode { get { return (UnloadSceneNode) Node; } }

        protected override void OnNodeGUI()
        {
            DrawNodeBody();
            DrawSocketsList(Node.InputSockets);
            DrawSocketsList(Node.OutputSockets);
            DrawActionDescription();
        }

        private void DrawActionDescription()
        {
            DynamicHeight += DGUI.Properties.Space(4);
            float x = DrawRect.x + 16;
            float lineHeight = DGUI.Properties.SingleLineHeight;

            var sceneNameOrBuildIndexModeRect = new Rect(x, DynamicHeight, DrawRect.width - 32, lineHeight);
            DynamicHeight += sceneNameOrBuildIndexModeRect.height;
            var continueAfterSceneUnloadedRect = new Rect(x, DynamicHeight, DrawRect.width - 32, lineHeight);
            DynamicHeight += continueAfterSceneUnloadedRect.height;
            
            DynamicHeight += DGUI.Properties.Space(4);

            if (ZoomedBeyondSocketDrawThreshold) return;
            
            Color textColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            string sceneNameOrBuildIndex;
            switch (TargetNode.GetSceneBy)
            {
                case GetSceneBy.Name:
                    sceneNameOrBuildIndex = UILabels.SceneName + ": " + TargetNode.SceneName;
                    break;
                case GetSceneBy.BuildIndex:
                    sceneNameOrBuildIndex = UILabels.SceneBuildIndex + ": " + TargetNode.SceneBuildIndex;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            GUI.Label(sceneNameOrBuildIndexModeRect, sceneNameOrBuildIndex, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), TargetNode.ErrorNoSceneName || TargetNode.ErrorBadBuildIndex ? Color.red : textColor));
            
            string waitForSceneToUnload = UILabels.WaitForSceneToUnload + ": " + (TargetNode.WaitForSceneToUnload ? UILabels.Yes : UILabels.No);

            GUI.Label(continueAfterSceneUnloadedRect, waitForSceneToUnload, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), textColor));
        }
    }
}