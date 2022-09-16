// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.NodeGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.Themes;
using Doozy.Engine.UI.Nodes;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomNodeGUI(typeof(ThemeNode))]
    public class ThemeNodeGUI : BaseNodeGUI
    {
        private static GUIStyle s_iconStyle;
        private static GUIStyle IconStyle { get { return s_iconStyle ?? (s_iconStyle = Styles.GetStyle(Styles.StyleName.NodeIconThemeNode)); } }
        protected override GUIStyle GetIconStyle() { return IconStyle; }

        private ThemeNode TargetNode { get { return (ThemeNode) Node; } }

        private readonly GUIStyle m_actionIcon = Styles.GetStyle(Styles.StyleName.IconThemeManager);
        private string m_targetThemeName;
        private string m_targetVariantName;

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
            var themeNameRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);
            DynamicHeight += themeNameRect.height;
            var variantNameRect = new Rect(textX, DynamicHeight, textWidth, lineHeight);
            DynamicHeight += variantNameRect.height;
            DynamicHeight += DGUI.Properties.Space(4);

            if (ZoomedBeyondSocketDrawThreshold) return;

            m_targetThemeName = "---";
            m_targetVariantName = "---";
            
            ThemeData theme = ThemesSettings.Database.GetThemeData(TargetNode.ThemeId);
            if (theme != null)
            {
                m_targetThemeName = theme.ThemeName;
                ThemeVariantData variant = theme.GetVariant(TargetNode.VariantId);
                if (variant != null)
                    m_targetVariantName = variant.VariantName;
            }

            Color iconAndTextColor = (DGUI.Utility.IsProSkin ? Color.white.Darker() : Color.black.Lighter()).WithAlpha(0.6f);
            DGUI.Icon.Draw(iconRect, m_actionIcon, iconAndTextColor);
            GUI.Label(themeNameRect, m_targetThemeName, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.S, TextAlign.Left), iconAndTextColor));
            GUI.Label(variantNameRect, m_targetVariantName, DGUI.Colors.ColorTextOfGUIStyle(DGUI.Label.Style(Editor.Size.M, TextAlign.Left), iconAndTextColor));
        }
    }
}