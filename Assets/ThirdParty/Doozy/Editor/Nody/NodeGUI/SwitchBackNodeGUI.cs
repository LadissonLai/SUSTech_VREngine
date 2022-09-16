// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Nody.Nodes;
using UnityEngine;

namespace Doozy.Editor.Nody.NodeGUI
{
    [CustomNodeGUI(typeof(SwitchBackNode))]
    public class SwitchBackNodeGUI : BaseNodeGUI
    {
        private SwitchBackNode SwitchBackNode { get { return (SwitchBackNode) target; } }

        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconSwitchBackNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        private const float TITLE_ALPHA = 0.9f;
        private const float TITLE_ICON_SIZE = 14f;

        private static GUIStyle TargetIconStyle { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconFaBullseye); } }
        private static GUIStyle SourcesIconStyle { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconFaCompress); } }
        private static ColorName TitleIconColorName { get { return DGUI.Colors.DisabledTextColorName; } }
        private static float TitleVerticalSpacing { get { return DGUI.Properties.Space(6); } }
        private static Size TitleSize { get { return Doozy.Editor.Size.M; } }
        private static TextAlign TitleTextAlign { get { return TextAlign.Left; } }
        private static GUIStyle SourceNameStyle { get { return DGUI.Label.Style(Doozy.Editor.Size.S, TextAlign.Center); } }
        private static GUIContent TargetContent { get { return new GUIContent(UILabels.Target.ToUpperInvariant()); } }
        private static Vector2 TargetContentSize { get { return DGUI.Label.Style(TitleSize, TitleTextAlign).CalcSize(TargetContent); } }
        private static GUIContent SourcesContent { get { return new GUIContent(UILabels.Sources.ToUpperInvariant()); } }
        private static Vector2 SourcesContentSize { get { return DGUI.Label.Style(TitleSize, TitleTextAlign).CalcSize(SourcesContent); } }

        protected override void OnNodeGUI()
        {
            Color color = GUI.color;
            DrawNodeBody();

            float x = DrawRect.x + 6;
            float width = DrawRect.width - 12;


            DynamicHeight += TitleVerticalSpacing;

            //Draw Target
            var targetIconRect = new Rect(x + width / 2 - TITLE_ICON_SIZE / 2, DynamicHeight, TITLE_ICON_SIZE, TITLE_ICON_SIZE);
            DynamicHeight += targetIconRect.height;
            DynamicHeight += DGUI.Properties.Space();
            Vector2 targetContentSize = TargetContentSize;
            var targetTextRect = new Rect(x + width / 2 - targetContentSize.x / 2, DynamicHeight, targetContentSize.x, targetContentSize.y);
            DynamicHeight += targetTextRect.height;
            DynamicHeight += TitleVerticalSpacing;
            {
                GUI.color = GUI.color.WithAlpha(TITLE_ALPHA * color.a);
                DGUI.Icon.Draw(targetIconRect, TargetIconStyle, TitleIconColorName);
                DGUI.Label.Draw(targetTextRect, TargetContent, TitleSize, TitleTextAlign, TitleIconColorName);
                GUI.color = color;
            }
            DynamicHeight += DrawSocket(SwitchBackNode.TargetInputSocket).height;
            DynamicHeight += DrawSocket(SwitchBackNode.TargetOutputSocket).height;

            DynamicHeight += DGUI.Properties.Space(24);
            DynamicHeight += TitleVerticalSpacing;

            //Draw Sources
            var sourcesIconRect = new Rect(x + width / 2 - TITLE_ICON_SIZE / 2, DynamicHeight, TITLE_ICON_SIZE, TITLE_ICON_SIZE);
            DynamicHeight += sourcesIconRect.height;
            DynamicHeight += DGUI.Properties.Space();
            Vector2 sourcesContentSize = SourcesContentSize;
            var sourcesTextRect = new Rect(x + width / 2 - sourcesContentSize.x / 2, DynamicHeight, sourcesContentSize.x, sourcesContentSize.y);
            DynamicHeight += targetTextRect.height;
            DynamicHeight += TitleVerticalSpacing;
            {
                GUI.color = GUI.color.WithAlpha(TITLE_ALPHA * color.a);
                DGUI.Icon.Draw(sourcesIconRect, SourcesIconStyle, TitleIconColorName);
                DGUI.Label.Draw(sourcesTextRect, SourcesContent, TitleSize, TitleTextAlign, TitleIconColorName);
                GUI.color = color;
            }

            foreach (SwitchBackNode.SourceInfo source in SwitchBackNode.Sources)
            {
                var sourceNameContent = new GUIContent(source.SourceName);
                Vector2 sourceNameContentSize = SourceNameStyle.CalcSize(sourceNameContent);
                var sourceNameRect = new Rect(x + width / 2 - sourceNameContentSize.x / 2, DynamicHeight + (DGUI.Properties.SingleLineHeight - sourcesContentSize.y) / 2, sourceNameContentSize.x, sourceNameContentSize.y);
                

                if (!string.IsNullOrEmpty(SwitchBackNode.ReturnSourceOutputSocketId) && SwitchBackNode.ReturnSourceOutputSocketId == source.OutputSocketId)
                {
                    float chevronHeight = sourceNameRect.height;
                    float chevronWidth = sourceNameRect.height * 0.63f;
                    var leftChevronRect = new Rect(sourceNameRect.xMin - chevronWidth - DGUI.Properties.Space(2), sourceNameRect.y, chevronWidth, chevronHeight);
                    var rightChevronRect = new Rect(sourceNameRect.xMax + DGUI.Properties.Space(2), sourceNameRect.y, chevronWidth, chevronHeight);
                    DGUI.Icon.Draw(leftChevronRect, Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconFaChevronLeft), DGUI.Colors.ActionColorName);
                    DGUI.Icon.Draw(rightChevronRect, Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconFaChevronRight), DGUI.Colors.ActionColorName);
                    GUI.color = DGUI.Colors.IconColor(DGUI.Colors.ActionColorName);
                }
                
                GUI.Label(sourceNameRect, source.SourceName, SourceNameStyle);
                GUI.color = color;
                
                DynamicHeight += DGUI.Properties.SingleLineHeight;

                Socket inputSocket = Node.GetSocketFromId(source.InputSocketId);
                Socket outputSocket = Node.GetSocketFromId(source.OutputSocketId);
                if (inputSocket == null || outputSocket == null)
                {
                    SwitchBackNode.RegenerateSourcesSocketIds();
                    DynamicHeight += NodySettings.Instance.SocketHeight;
                    DynamicHeight += NodySettings.Instance.SocketHeight;
                }
                else
                {
                    DynamicHeight += DrawSocket(Node.GetSocketFromId(source.InputSocketId)).height;
                    DynamicHeight += DrawSocket(Node.GetSocketFromId(source.OutputSocketId)).height;
                }
                DynamicHeight += DGUI.Properties.Space(12);
            }

            DynamicHeight += DGUI.Properties.Space(2);

            GUI.color = color;
        }
    }
}