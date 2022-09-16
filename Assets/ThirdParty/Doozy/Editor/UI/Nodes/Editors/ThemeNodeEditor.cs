// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.Editors;
using Doozy.Editor.Themes;
using Doozy.Engine.Themes;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(ThemeNode))]
    public class ThemeNodeEditor : BaseNodeEditor
    {
        private ThemeNode TargetNode { get { return (ThemeNode) target; } }

        private static ThemesDatabase Database { get { return ThemesSettings.Database; } }
        private string[] ThemesNames;
        private string[] VariantsNames;
        private ThemeData m_theme;
        private int m_selectedThemeIndex;
        private int m_selectedVariantIndex;

        public override void OnInspectorGUI()
        {
            UpdateIds();
            UpdateLists();
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderThemeNode), MenuUtils.ThemeNode_Manual, MenuUtils.ThemeNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            int themeIndex = Database.GetThemeIndex(TargetNode.ThemeId);
            if (themeIndex != -1)
            {
                DrawThemePopup(Database, m_theme, ThemesNames, themeIndex, DGUI.Colors.ActionColorName, UpdateIds, UpdateLists);
                GUILayout.Space(DGUI.Properties.Space());
            }

            int variantIndex = m_theme.GetVariantIndex(TargetNode.VariantId);
            if (variantIndex != -1)
            {
                DrawVariantPopup(m_theme, VariantsNames, variantIndex, DGUI.Colors.ActionColorName, UpdateIds, UpdateLists);
            }
            

            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void UpdateIds()
        {
            if (!Database.Contains(TargetNode.ThemeId))
                TargetNode.ThemeId = Database.Themes[0].Id;
            m_theme = Database.GetThemeData(TargetNode.ThemeId);

            if (!m_theme.ContainsVariant(TargetNode.VariantId))
                TargetNode.VariantId = m_theme.Variants[0].Id;
        }

        private void UpdateLists()
        {
            ThemesNames = ThemesDatabase.GetThemesNames(Database);
            VariantsNames = ThemesDatabase.GetVariantNames(m_theme);
        }

        private void DrawThemePopup(ThemesDatabase database, ThemeData themeData, string[] themeNames, int themeIndex, ColorName componentColorName, Action updateIds, Action updateLists)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Line.Draw(false, componentColorName, true,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.TargetTheme, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                   GUILayout.Space(DGUI.Properties.Space());
                                   GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                   {
                                       GUILayout.Space(0);
                                       GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                                       EditorGUI.BeginChangeCheck();
                                       themeIndex = EditorGUILayout.Popup(GUIContent.none, themeIndex, themeNames);
                                       GUI.color = InitialGUIColor;
                                   }
                                   GUILayout.EndVertical();
                                   if (EditorGUI.EndChangeCheck())
                                   {
                                       DoozyUtils.UndoRecordObject(TargetNode, UILabels.UpdateValue);
                                       themeData = database.Themes[themeIndex];
                                       TargetNode.ThemeId = themeData.Id;
                                       updateIds.Invoke();
                                       updateLists.Invoke();
                                   }
                               });

                GUILayout.Space(DGUI.Properties.Space());

                ThemeTargetEditorUtils.DrawButtonTheme(themeData, componentColorName);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawVariantPopup(ThemeData themeData, string[] variantNames, int variantIndex, ColorName componentColorName, Action updateIds, Action updateLists)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Line.Draw(false, componentColorName, true,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.TargetVariant, Size.S, componentColorName, DGUI.Properties.SingleLineHeight);
                                   GUILayout.Space(DGUI.Properties.Space());
                                   GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                   {
                                       GUILayout.Space(0);
                                       GUI.color = DGUI.Colors.PropertyColor(componentColorName);
                                       EditorGUI.BeginChangeCheck();
                                       variantIndex = EditorGUILayout.Popup(GUIContent.none, variantIndex, variantNames);
                                       GUI.color = InitialGUIColor;
                                   }
                                   GUILayout.EndVertical();
                                   if (EditorGUI.EndChangeCheck())
                                   {
                                       DoozyUtils.UndoRecordObject(TargetNode, UILabels.UpdateValue);
                                       TargetNode.VariantId = themeData.Variants[variantIndex].Id;
                                       updateIds.Invoke();
                                       updateLists.Invoke();
                                   }
                               });
            }
            GUILayout.EndHorizontal();
        }
    }
}