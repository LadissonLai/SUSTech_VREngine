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
    [CustomNodeGUI(typeof(UIDrawerNode))]
    public class UIDrawerNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconUIDrawerNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }
        
        private UIDrawerNode TargetNode { get { return (UIDrawerNode) Node; } }

        private Rect
            m_actionRect,
            m_drawerNameRect;

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
            float textX = x;
            float lineWidth = DrawRect.width - 32;
            float lineHeight = DGUI.Properties.SingleLineHeight;

            m_actionRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
            DynamicHeight += m_actionRect.height;

            m_drawerNameRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
            DynamicHeight += m_drawerNameRect.height;

            DynamicHeight += DGUI.Properties.Space(4);

            if (ZoomedBeyondSocketDrawThreshold) return;


            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            GUI.Label(m_actionRect, TargetNode.Action.ToString(), DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.M, TextAlign.Center), iconAndTextColor));
            GUI.Label(m_drawerNameRect, TargetNode.DrawerName, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Center), iconAndTextColor));
        }
    }
}