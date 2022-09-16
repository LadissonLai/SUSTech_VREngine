// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Themes
{
    [CustomEditor(typeof(ThemeManager))]
    public class ThemeManagerEditor  : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ThemesColorName; } }
        private ThemeManager m_target;

        private ThemeManager Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (ThemeManager) target;
                return m_target;
            }
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderThemeManager), MenuUtils.ThemeManager_Manual, MenuUtils.ThemeManager_YouTube);
            DrawThemesButton();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawThemesButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconThemeManager),
                                                   UILabels.Themes,
                                                   Size.M, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.Themes);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}