// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Nodes;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(BackButtonNode))]
    public class BackButtonNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconBackButtonNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }
        
        private BackButtonNode TargetNode { get { return (BackButtonNode) Node; } }

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
            float textX = x;
            var actionTitleRect = new Rect(textX, DynamicHeight, DrawRect.width, lineHeight);
            DynamicHeight += actionTitleRect.height;
            var actionNameRect = new Rect(textX, DynamicHeight, DrawRect.width, lineHeight);
            DynamicHeight += actionNameRect.height;
            DynamicHeight += DGUI.Properties.Space(4);

            if (ZoomedBeyondSocketDrawThreshold) return;

            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            GUI.Label(actionTitleRect, UILabels.BackButton, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(actionNameRect, TargetNode.BackButtonAction.ToString(), DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.M, TextAlign.Left), iconAndTextColor));
        }
    }
}