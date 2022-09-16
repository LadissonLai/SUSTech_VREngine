// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(DColor), true)]
    public class DColorDrawer : PropertyDrawer
    {
        private const float NAME_LABEL_WIDTH = 120;
        private SerializedProperty m_colorName, m_light, m_normal, m_dark;
        private float m_propertyHeight;


        private static GUIStyle LabelStyle { get { return DGUI.Label.Style(); } }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                //get sub properties
                GetProperties(property);

                //calculate rects
                var rect = new Rect(position.x, position.y, position.width, DGUI.Properties.SingleLineHeight);
                var colorNameRect = new Rect(rect.x, rect.y, NAME_LABEL_WIDTH, rect.height);
                float colorFieldWidth = (rect.width - NAME_LABEL_WIDTH - DGUI.Properties.StandardVerticalSpacing * 3) / 3;
                var lightColorRect = new Rect(colorNameRect.x + NAME_LABEL_WIDTH + DGUI.Properties.StandardVerticalSpacing, rect.y, colorFieldWidth, rect.height);
                var normalColorRect = new Rect(lightColorRect.x + colorFieldWidth + DGUI.Properties.StandardVerticalSpacing, rect.y, colorFieldWidth, rect.height);
                var darkColorRect = new Rect(normalColorRect.x + colorFieldWidth + DGUI.Properties.StandardVerticalSpacing, rect.y, colorFieldWidth, rect.height);

                //draw
                GUI.color = new Color(m_normal.colorValue.r, m_normal.colorValue.g, m_normal.colorValue.b, 0.5f);
                GUI.Label(colorNameRect, GUIContent.none, DGUI.Properties.White);
                GUI.color = Color.white;
                EditorGUI.PropertyField(colorNameRect, m_colorName, GUIContent.none, true);
                EditorGUI.PropertyField(lightColorRect, m_light, GUIContent.none, true);
                EditorGUI.PropertyField(normalColorRect, m_normal, GUIContent.none, true);
                EditorGUI.PropertyField(darkColorRect, m_dark, GUIContent.none, true);
                GUI.Label(lightColorRect, "LIGHT", new GUIStyle(LabelStyle) {alignment = TextAnchor.UpperLeft, padding = new RectOffset(2, 0, 2, 0), fontSize = LabelStyle.fontSize - 2, normal = {textColor = m_dark.colorValue}});
                GUI.Label(normalColorRect, "NORMAL", new GUIStyle(LabelStyle) {alignment = TextAnchor.UpperLeft, padding = new RectOffset(2, 0, 2, 0), fontSize = LabelStyle.fontSize - 2, normal = {textColor = m_dark.colorValue}});
                GUI.Label(darkColorRect, "DARK", new GUIStyle(LabelStyle) {alignment = TextAnchor.UpperLeft, padding = new RectOffset(2, 0, 2, 0), fontSize = LabelStyle.fontSize - 2, normal = {textColor = m_light.colorValue}});

                // set indent back to what it was
                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }

        private void GetProperties(SerializedProperty property)
        {
            if (m_colorName == null) m_colorName = property.FindPropertyRelative("ColorName");
            if (m_light == null) m_light = property.FindPropertyRelative("Light");
            if (m_normal == null) m_normal = property.FindPropertyRelative("Normal");
            if (m_dark == null) m_dark = property.FindPropertyRelative("Dark");
        }
    }
}