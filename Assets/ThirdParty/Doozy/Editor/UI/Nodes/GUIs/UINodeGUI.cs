// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.UI.Nodes;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    // ReSharper disable once UnusedMember.Global
    [CustomNodeGUI(typeof(UINode))]
    public class UINodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconUINode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        private UINode TargetNode { get { return (UINode) Node; } }

        private Rect
            m_areaRect,
            m_iconRect,
            m_progressBarRect;

        protected override void OnNodeGUI()
        {
            DrawNodeBody();
            DrawSocketsList(Node.InputSockets);
            DrawSocketsList(Node.OutputSockets);
            DrawAddSocketButton(SocketDirection.Output, ConnectionMode.Override, typeof(UIConnection));
        }

        protected override Rect DrawSocket(Socket socket)
        {
            Rect socketRect = base.DrawSocket(socket);

            if (ZoomedBeyondSocketDrawThreshold) return socketRect;
            if (socket.IsInput) return socketRect;

            UIConnection socketValue = UIConnection.GetValue(socket);

            m_areaRect = new Rect(socket.GetX() + 24, socket.GetY(), socket.GetWidth() - 48, socket.GetHeight());

            GUIStyle triggerIcon;
            switch (socketValue.Trigger)
            {
                case UIConnectionTrigger.ButtonClick:
                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonClick);
                    break;
                case UIConnectionTrigger.ButtonDoubleClick:
                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonDoubleClick);
                    break;
                case UIConnectionTrigger.ButtonLongClick:
                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonLongClick);
                    break;
                case UIConnectionTrigger.GameEvent:
                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconGameEventListener);
                    break;
                case UIConnectionTrigger.TimeDelay:
                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconTime);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            float socketOpacity = socket.IsConnected ? NodySettings.Instance.SocketConnectedOpacity : NodySettings.Instance.SocketNotConnectedOpacity;
            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(socketOpacity);

            string label;
            switch (socketValue.Trigger)
            {
                case UIConnectionTrigger.ButtonClick:
                case UIConnectionTrigger.ButtonDoubleClick:
                case UIConnectionTrigger.ButtonLongClick:
                    label = socketValue.ButtonName;
                    break;
                case UIConnectionTrigger.GameEvent:
                    label = socketValue.GameEvent;
                    break;
                case UIConnectionTrigger.TimeDelay:
                    label = socketValue.TimeDelay + " " + UILabels.SecondsDelay;
                    if (socket.IsConnected)
                    {
                        m_progressBarRect = new Rect(m_areaRect.x + kSocketIconSize + DGUI.Properties.Space(4), m_areaRect.yMax - DGUI.Properties.Space(2), m_areaRect.width - kSocketIconSize, DGUI.Properties.Space());
                        DrawProgressBar(m_progressBarRect, TargetNode.TimerProgress, m_progressBarRect.height, false);
                    }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            GUILayout.BeginArea(m_areaRect);
            {
                GUILayout.BeginHorizontal();
                {
                    m_iconRect = new Rect(0, socket.GetHeight() / 2 - kSocketIconSize / 2, kSocketIconSize, kSocketIconSize);
                    DGUI.Icon.Draw(m_iconRect, triggerIcon, iconAndTextColor);
                    GUILayout.Space(DGUI.Properties.Space(2) + kSocketIconSize + DGUI.Properties.Space(2));
                    DGUI.Label.Draw(label, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.M, TextAlign.Left), iconAndTextColor), socket.GetHeight());
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            return socket.GetRect();
        }
    }
}