// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

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
    [CustomNodeGUI(typeof(RandomNode))]
    public class RandomNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconRandomNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }
        
        private RandomNode TargetNode { get { return (RandomNode) Node; } }

        private Rect
            m_areaRect,
            m_weightIconRect,
            m_progressBarRect;

        private GUIStyle m_socketIcon;

        private GUIStyle SocketIcon
        {
            get
            {
                if (m_socketIcon != null) return m_socketIcon;
                m_socketIcon = Styles.GetStyle(Styles.StyleName.IconFaPercentage);
                return m_socketIcon;
            }
        }


        protected override void OnNodeGUI()
        {
            if (TargetNode.MaxChance <= 0) TargetNode.UpdateMaxChance();
            TargetNode.UpdateConnectedOutputSockets();

            DrawNodeBody();
            DrawSocketsList(Node.InputSockets);
            DrawSocketsList(Node.OutputSockets);
            DrawAddSocketButton(SocketDirection.Output, ConnectionMode.Override, typeof(WeightedConnection));
        }

        protected override Rect DrawSocket(Socket socket)
        {
            Rect socketRect = base.DrawSocket(socket);

            if (ZoomedBeyondSocketDrawThreshold) return socketRect;
            if (socket.IsInput) return socketRect;

            WeightedConnection socketValue = WeightedConnection.GetValue(socket);

            m_areaRect = new Rect(socket.GetX() + 24, socket.GetY(), socket.GetWidth() - 48, socket.GetHeight());

            float socketOpacity = socket.IsConnected ? NodySettings.Instance.SocketConnectedOpacity : NodySettings.Instance.SocketNotConnectedOpacity;
            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(socketOpacity);

            float chance = socket.IsConnected && TargetNode.MaxChance > 0
                               ? (float) socketValue.Weight / TargetNode.MaxChance
                               : 0f;

            string weightLabel = socketValue.Weight.ToString();
            string chanceLabel = Mathf.Round(chance * 100f) + "%";

            if (socket.IsConnected)
            {
                m_progressBarRect = new Rect(m_areaRect.x + kSocketIconSize + DGUI.Properties.Space(4), m_areaRect.yMax - DGUI.Properties.Space(2), m_areaRect.width - kSocketIconSize, DGUI.Properties.Space());
                DrawProgressBar(m_progressBarRect, chance, m_progressBarRect.height, false);
            }

            m_weightIconRect = new Rect(0, socket.GetHeight() / 2 - kSocketIconSize / 2, kSocketIconSize, kSocketIconSize);

            GUILayout.BeginArea(m_areaRect);
            {
                GUILayout.BeginHorizontal();
                {
                    DGUI.Icon.Draw(m_weightIconRect, SocketIcon, iconAndTextColor);
                    GUILayout.Space(DGUI.Properties.Space(2) + kSocketIconSize + DGUI.Properties.Space(2));
                    DGUI.Label.Draw(chanceLabel, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.M, TextAlign.Left), iconAndTextColor), socket.GetHeight());
                    GUILayout.FlexibleSpace();
                    DGUI.Label.Draw(UILabels.Weight + ": " + weightLabel, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor.WithAlpha(0.6f)), socket.GetHeight());
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            return socket.GetRect();
        }
    }
}