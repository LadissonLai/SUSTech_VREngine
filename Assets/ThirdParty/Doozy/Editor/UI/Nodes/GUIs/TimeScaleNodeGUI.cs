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
    [CustomNodeGUI(typeof(TimeScaleNode))]
    public class TimeScaleNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconTimeScaleNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }
        
        private TimeScaleNode TargetNode { get { return (TimeScaleNode) Node; } }

        private Rect
            m_timeScaleRect,
            m_targetTimeScaleRect,
            m_animateValueRect,
            m_progressBarRect,
            m_animationDurationRect,
            m_waitForAnimationToFinishRect;

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

            m_timeScaleRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
            DynamicHeight += m_timeScaleRect.height;

            m_targetTimeScaleRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
            DynamicHeight += m_targetTimeScaleRect.height;

            m_animateValueRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
            DynamicHeight += m_animateValueRect.height;

            if (TargetNode.AnimateValue)
            {
                m_progressBarRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight / 2);
                DynamicHeight += m_progressBarRect.height;

                m_animationDurationRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
                DynamicHeight += m_animationDurationRect.height;

                m_waitForAnimationToFinishRect = new Rect(textX, DynamicHeight, lineWidth, lineHeight);
                DynamicHeight += m_waitForAnimationToFinishRect.height;
            }

            DynamicHeight += DGUI.Properties.Space(4);

            if (ZoomedBeyondSocketDrawThreshold) return;


            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            GUI.Label(m_timeScaleRect, UILabels.CurrentTimeScale + ": " + Time.timeScale, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(m_targetTimeScaleRect, UILabels.TargetTimeScale + ": " + TargetNode.TargetValue, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(m_animateValueRect, UILabels.AnimateValue + ": " + (TargetNode.AnimateValue ? UILabels.Yes : UILabels.No), DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));

            if (!TargetNode.AnimateValue) return;
            DrawProgressBar(m_progressBarRect, TargetNode.TimerProgress, DGUI.Properties.Space(), true);
            GUI.Label(m_animationDurationRect, UILabels.Duration + ": " + TargetNode.AnimationDuration + " " + UILabels.Seconds, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(m_waitForAnimationToFinishRect, UILabels.WaitForAnimationToFinish + ": " + (TargetNode.WaitForAnimationToFinish ? UILabels.Yes : UILabels.No), DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
        }
    }
}