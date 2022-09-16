// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Dropdown
        {
            public static void Draw(SerializedProperty property, GUIContent content, DColor dColor, bool drawBackground = true)
            {
                float lineHeight = Properties.SingleLineHeight;
                float backgroundHeight = Properties.SingleLineHeight + Properties.Space(2);
                GUILayoutOption heightOption = GUILayout.Height(backgroundHeight);
                GUILayout.BeginVertical(heightOption);
                {
                    if (drawBackground) Background.Draw(dColor, GUILayout.Height(backgroundHeight));
                    GUILayout.Space(drawBackground ? -backgroundHeight : -Properties.Space());
                    GUILayout.BeginHorizontal(heightOption);
                    {
                        GUILayout.Space(Properties.Space(2));
                        if (content != GUIContent.none) Label.Draw(content, Size.S, dColor, backgroundHeight);
                        Color color = GUI.color;
                        GUI.color = Colors.BackgroundColor(dColor);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(property, GUIContent.none, true, GUILayout.Height(lineHeight));
                        if (EditorGUI.EndChangeCheck()) Properties.ResetKeyboardFocus();
                        GUI.color = color;
                        GUILayout.Space(Properties.Space());
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(drawBackground ? Properties.Space() : Properties.Space(2));
                }
                GUILayout.EndVertical();
            }

            public static void Draw(SerializedProperty property, bool drawBackground = true) { Draw(property, GUIContent.none, Colors.BaseDColor(), drawBackground); }
            public static void Draw(SerializedProperty property, DColor dColor, bool drawBackground = true) { Draw(property, GUIContent.none, dColor, drawBackground); }
            public static void Draw(SerializedProperty property, ColorName colorName, bool drawBackground = true) { Draw(property, GUIContent.none, Colors.GetDColor(colorName), drawBackground); }
            public static void Draw(SerializedProperty property, GUIContent content, bool drawBackground = true) { Draw(property, content, Colors.BaseDColor(), drawBackground); }
            public static void Draw(SerializedProperty property, GUIContent content, ColorName colorName, bool drawBackground = true) { Draw(property, content, Colors.GetDColor(colorName), drawBackground); }
            public static void Draw(SerializedProperty property, string text, bool drawBackground = true) { Draw(property, new GUIContent(text), drawBackground); }
            public static void Draw(SerializedProperty property, string text, ColorName colorName, bool drawBackground = true) { Draw(property, new GUIContent(text), Colors.GetDColor(colorName), drawBackground); }
            public static void Draw(SerializedProperty property, string text, DColor dColor, bool drawBackground = true) { Draw(property, new GUIContent(text), dColor, drawBackground); }
        }
    }
}