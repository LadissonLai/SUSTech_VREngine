// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(WaitNode))]
    public class WaitNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconWaitNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        private WaitNode TargetNode { get { return (WaitNode) Node; } }

        private GUIStyle m_actionIcon;
        private string m_title;
        private string m_description;

        private Styles.StyleName WaitForActionIconStyleName
        {
            get
            {
                switch (TargetNode.WaitFor)
                {
                    case WaitNode.WaitType.Time:              return Styles.StyleName.IconTime;
                    case WaitNode.WaitType.GameEvent:         return Styles.StyleName.IconGameEventListener;
                    case WaitNode.WaitType.SceneLoad:         return Styles.StyleName.NodeIconLoadSceneNode;
                    case WaitNode.WaitType.SceneUnload:       return Styles.StyleName.NodeIconUnloadSceneNode;
                    case WaitNode.WaitType.ActiveSceneChange: return Styles.StyleName.NodeIconActivateLoadedScenesNode;
                    case WaitNode.WaitType.UIView:            return Styles.StyleName.IconUIViewListener;
                    case WaitNode.WaitType.UIButton:          return Styles.StyleName.IconUIButtonListener;
                    case WaitNode.WaitType.UIDrawer:          return Styles.StyleName.IconUIDrawerListener;
                    default:                                  throw new ArgumentOutOfRangeException();
                }
            }
        }

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

            if (ZoomedBeyondSocketDrawThreshold) return;

            m_actionIcon = Styles.GetStyle(WaitForActionIconStyleName);

            m_title = UILabels.WaitFor + ": ";

            switch (TargetNode.WaitFor)
            {
                case WaitNode.WaitType.Time:
                    m_title += (TargetNode.WaitFor == WaitNode.WaitType.Time && EditorApplication.isPlayingOrWillChangePlaymode
                                    ? "[" + TargetNode.CurrentDuration + "] " + UILabels.Seconds
                                    : TargetNode.WaitForInfoTitle);
                    break;
                case WaitNode.WaitType.GameEvent:   break;
                case WaitNode.WaitType.SceneLoad:   break;
                case WaitNode.WaitType.SceneUnload: break;
                case WaitNode.WaitType.ActiveSceneChange:
                    m_title += TargetNode.WaitForInfoTitle;
                    break;
                case WaitNode.WaitType.UIView:
                case WaitNode.WaitType.UIButton:
                case WaitNode.WaitType.UIDrawer:
                    m_title = TargetNode.WaitForInfoTitle;
                    break;
            }

            m_description = TargetNode.WaitForInfoDescription;

            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            DGUI.Icon.Draw(iconRect, m_actionIcon, iconAndTextColor);
            GUI.Label(titleRect, m_title, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));

            switch (TargetNode.WaitFor)
            {
                case WaitNode.WaitType.GameEvent:
                    if (TargetNode.ErrorNoGameEvent)
                        iconAndTextColor = Color.red;
                    break;
                case WaitNode.WaitType.SceneLoad:
                case WaitNode.WaitType.SceneUnload:
                case WaitNode.WaitType.ActiveSceneChange:
                    switch (TargetNode.GetSceneBy)
                    {
                        case GetSceneBy.Name:
                            if (TargetNode.ErrorNoSceneName)
                                iconAndTextColor = Color.red;
                            break;
                        case GetSceneBy.BuildIndex:
                            if (TargetNode.ErrorBadBuildIndex)
                                iconAndTextColor = Color.red;
                            break;
                    }

                    break;
                case WaitNode.WaitType.UIView:
                    if (TargetNode.UIViewTriggerAction == UIViewBehaviorType.Unknown)
                        iconAndTextColor = Color.red;
                    break;
            }

            if (TargetNode.WaitFor != WaitNode.WaitType.Time)
                GUI.Label(descriptionRect, m_description, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            else
                DrawProgressBar(descriptionRect, TargetNode.TimerProgress, DGUI.Properties.Space(), true);
        }
    }
}