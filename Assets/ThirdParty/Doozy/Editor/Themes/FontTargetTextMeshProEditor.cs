// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Themes
{
    [CustomEditor(typeof(FontTargetTextMeshPro))]
    [CanEditMultipleObjects]
    public class FontTargetTextMeshProEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ThemesColorName; } }

        private const string SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED = "SupportForTextMeshProNotEnabled";
        
        private FontTargetTextMeshPro m_target;

        private FontTargetTextMeshPro Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (FontTargetTextMeshPro) target;
                return m_target;
            }
        }

        private static ThemesDatabase Database { get { return ThemesSettings.Database; } }
#if dUI_TextMeshPro
        private string[] ThemesNames;
        private ThemeData m_theme;
        
        private string[] VariantsNames;
        
        private SerializedProperty
            m_textMeshPro;

        private LabelId m_Property;
        private int m_selectedThemeIndex;
        private int m_selectedVariantIndex;
        private int m_selectedPropertyIndex;

        private bool HasReference { get { return m_textMeshPro.objectReferenceValue != null; } }
#endif
        
         protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();
            
#if dUI_TextMeshPro
            m_textMeshPro = GetProperty(PropertyName.TextMeshPro);
#endif
        }

         protected override void OnEnable()
         {
             base.OnEnable();

             AddInfoMessage(SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED, new InfoMessage(InfoMessage.MessageType.Warning, UILabels.SupportForTextMeshProNotEnabled, false, Repaint));
         }
         
        public override void OnInspectorGUI()
        {
            UpdateIds();
            UpdateLists();
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderFontTargetTextMeshPro), MenuUtils.FontTargetTextMeshPro_Manual, MenuUtils.FontTargetTextMeshPro_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));
            
#if dUI_TextMeshPro
            DGUI.Property.Draw(m_textMeshPro, UILabels.TextMeshPro, HasReference ? ComponentColorName : ColorName.Red);
            GUILayout.Space(DGUI.Properties.Space(4));
            int themeIndex = Database.GetThemeIndex(Target.ThemeId);
            if (themeIndex != -1)
            {
                ThemeTargetEditorUtils.DrawThemePopup(Database, m_theme, ThemesNames, themeIndex, ComponentColorName, serializedObject, targets, Target, InitialGUIColor, UpdateIds, UpdateLists);
                GUILayout.Space(DGUI.Properties.Space());
                ThemeTargetEditorUtils.DrawActiveVariant(m_theme, ComponentColorName);
            }

            GUILayout.Space(DGUI.Properties.Space(2));
            int propertyIndex = m_theme.GetFontAssetPropertyIndex(Target.PropertyId);
            if (Target.PropertyId == Guid.Empty || propertyIndex == -1) ThemeTargetEditorUtils.DrawLabelNoPropertyFound();
            else ThemeTargetEditorUtils.DrawFontAssetProperties(m_theme, propertyIndex, serializedObject, targets, Target, ComponentColorName, InitialGUIColor);
#else
            GetInfoMessage(SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED).Draw(true, InspectorWidth);
#endif      
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void UpdateIds()
        {
#if dUI_TextMeshPro
            if (!Database.Contains(Target.ThemeId))
                Target.ThemeId = Database.Themes[0].Id;
            m_theme = Database.GetThemeData(Target.ThemeId);

            if (!m_theme.ContainsFontAssetProperty(Target.PropertyId))
                Target.PropertyId = m_theme.FontAssetLabels.Count > 0
                                        ? m_theme.FontAssetLabels[0].Id
                                        : Guid.Empty;
#endif   
        }

        private void UpdateLists()
        {
#if dUI_TextMeshPro
            ThemesNames = ThemesDatabase.GetThemesNames(Database);
#endif   
        }
    }
}