// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine;
using UnityEditor;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Doozy.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(MColor))]
    public class MColorDrawer : BaseDrawer
    {
        private void Init(SerializedProperty property)
        {
            if (Initialized.ContainsKey(property.propertyPath) && Initialized[property.propertyPath]) return;

            Elements.Add(Properties.Add("Name", property));
            Elements.Add(Properties.Add("M50", property));
            Elements.Add(Properties.Add("M100", property));
            Elements.Add(Properties.Add("M200", property));
            Elements.Add(Properties.Add("M300", property));
            Elements.Add(Properties.Add("M400", property));
            Elements.Add(Properties.Add("M500", property));
            Elements.Add(Properties.Add("M600", property));
            Elements.Add(Properties.Add("M700", property));
            Elements.Add(Properties.Add("M800", property));
            Elements.Add(Properties.Add("M900", property));
            Elements.Add(Properties.Add("A100", property));
            Elements.Add(Properties.Add("A200", property));
            Elements.Add(Properties.Add("A400", property));
            Elements.Add(Properties.Add("A700", property));

            if (!Initialized.ContainsKey(property.propertyPath))
                Initialized.Add(property.propertyPath, true);
            else
                Initialized[property.propertyPath] = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Init(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Draw(position, property);

                // set indent back to what it was
                EditorGUI.indentLevel = indent;

                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        private void Draw(Rect position, SerializedProperty property)
        {
            SerializedProperty Name = Properties.Get("Name", property);
            SerializedProperty M50 = Properties.Get("M50", property);
            SerializedProperty M100 = Properties.Get("M100", property);
            SerializedProperty M200 = Properties.Get("M200", property);
            SerializedProperty M300 = Properties.Get("M300", property);
            SerializedProperty M400 = Properties.Get("M400", property);
            SerializedProperty M500 = Properties.Get("M500", property);
            SerializedProperty M600 = Properties.Get("M600", property);
            SerializedProperty M700 = Properties.Get("M700", property);
            SerializedProperty M800 = Properties.Get("M800", property);
            SerializedProperty M900 = Properties.Get("M900", property);
            SerializedProperty A100 = Properties.Get("A100", property);
            SerializedProperty A200 = Properties.Get("A200", property);
            SerializedProperty A400 = Properties.Get("A400", property);
            SerializedProperty A700 = Properties.Get("A700", property);

            float nameLabelWidth = 80;
            float totalWidth = position.width - nameLabelWidth - DGUI.Properties.Space();
            float colorWidth = totalWidth / 14;

            float x = position.x;
            float y = position.y;
            float height = DGUI.Properties.SingleLineHeight;
            GUI.Label(new Rect(x, y, nameLabelWidth, height), Name.stringValue, new GUIStyle(DGUI.Label.Style(Size.S)) {alignment = TextAnchor.MiddleRight});
            x += nameLabelWidth + DGUI.Properties.Space();
            DrawColorField(M50, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M100, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M200, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M300, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M400, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M500, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M600, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M700, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M800, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(M900, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(A100, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(A200, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(A400, x, y, colorWidth, height);
            x += colorWidth;
            DrawColorField(A700, x, y, colorWidth, height);
            x += colorWidth;
        }

        private static void DrawColorField(SerializedProperty serializedProperty, float x, float y, float colorWidth, float height) { serializedProperty.colorValue = EditorGUI.ColorField(new Rect(x, y, colorWidth, height), GUIContent.none, serializedProperty.colorValue, false, false, false); }
    }
}