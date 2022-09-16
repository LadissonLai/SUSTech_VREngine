// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody;
using Doozy.Engine.Extensions;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(PortalNode))]
    public class PortalNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconPortalNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        private PortalNode TargetNode { get { return (PortalNode) Node; } }

        protected override void OnNodeGUI()
        {
            DrawNodeBody();

            if (TargetNode.SwitchBackMode)
            {
                if (TargetNode.InputSockets == null ||
                    TargetNode.InputSockets.Count == 0)
                {
                    TargetNode.AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
                    GraphEvent.ConstructGraph();
                    GraphEvent.RecalculateAllPoints();
                }

                DrawSocketsList(Node.InputSockets);
            }
            else
            {
                if (TargetNode.InputSockets != null
                    && TargetNode.InputSockets.Count > 0)
                {
                    if (TargetNode.FirstInputSocket != null &&
                        TargetNode.FirstInputSocket.IsConnected)
                        GraphEvent.DisconnectSocket(TargetNode.FirstInputSocket);

                    TargetNode.InputSockets.Clear();
                    GraphEvent.ConstructGraph();
                    GraphEvent.RecalculateAllPoints();
                }
            }

            DrawSocketsList(Node.OutputSockets);
            DrawActionDescription();
        }

        private GUIStyle m_actionIcon;
        private string m_title;
        private string m_description;

        private GUIStyle m_sourceIcon;
        private string m_sourceNodeName;

        private Styles.StyleName WaitForActionIconStyleName
        {
            get
            {
                switch (TargetNode.ListenFor)
                {
                    case PortalNode.ListenerType.GameEvent: return Styles.StyleName.IconGameEventListener;
                    case PortalNode.ListenerType.UIView:    return Styles.StyleName.IconUIViewListener;
                    case PortalNode.ListenerType.UIButton:  return Styles.StyleName.IconUIButtonListener;
                    case PortalNode.ListenerType.UIDrawer:  return Styles.StyleName.IconUIDrawerListener;
                    default:                                throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void DrawActionDescription()
        {
            DynamicHeight += DGUI.Properties.Space(4);
            float x = DrawRect.x + 16;
            float lineHeight = DGUI.Properties.SingleLineHeight;
            float iconLineHeight = lineHeight * 2;
            float iconSize = iconLineHeight * 0.6f;
            var iconRect = new Rect(x, DynamicHeight + (iconLineHeight - iconSize) / 2, iconSize, iconSize);
            float textX = iconRect.xMax + DGUI.Properties.Space(4);
            float textWidth = DrawRect.width - iconSize - DGUI.Properties.Space(4) - 32;
            var titleRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);
            DynamicHeight += titleRect.height;
            var descriptionRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);
            DynamicHeight += descriptionRect.height;
            DynamicHeight += DGUI.Properties.Space(4);

            var sourceIconRect = new Rect();
            var sourceTextRect = new Rect();

            bool switchBackModeEnabled = TargetNode.SwitchBackMode && TargetNode.HasSource;
            
            if (switchBackModeEnabled)
            {
                DynamicHeight -= DGUI.Properties.Space(2);
                sourceIconRect = new Rect(iconRect.x, DynamicHeight + (iconLineHeight - iconSize) / 2, iconSize, iconSize);
                DynamicHeight += (iconLineHeight - iconSize) / 2 + 2;
                sourceTextRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);
                DynamicHeight += sourceTextRect.height;
                DynamicHeight += DGUI.Properties.Space(4);
            }

            if (ZoomedBeyondSocketDrawThreshold) return;

            m_actionIcon = Styles.GetStyle(WaitForActionIconStyleName);
            m_title = UILabels.ListenFor + ": ";

            switch (TargetNode.ListenFor)
            {
                case PortalNode.ListenerType.GameEvent:
                    m_title += TargetNode.WaitForInfoTitle;
                    break;
                case PortalNode.ListenerType.UIButton:
                case PortalNode.ListenerType.UIView:
                case PortalNode.ListenerType.UIDrawer:
                    m_title = TargetNode.WaitForInfoTitle;
                    break;
            }


            m_description = TargetNode.WaitForInfoDescription;

            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            DGUI.Icon.Draw(iconRect, m_actionIcon, iconAndTextColor);
            GUI.Label(titleRect, m_title, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(descriptionRect, m_description, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Doozy.Editor.Size.M, TextAlign.Left), iconAndTextColor));

            if (!switchBackModeEnabled) return;
            m_sourceIcon = Styles.GetStyle(Styles.StyleName.IconFaAngleDoubleLeft);
            DGUI.Icon.Draw(sourceIconRect, m_sourceIcon, iconAndTextColor);
            GUI.Label(sourceTextRect, TargetNode.Source.Name, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Doozy.Editor.Size.M, TextAlign.Left), iconAndTextColor));
        }
    }
}