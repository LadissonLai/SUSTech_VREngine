// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Property
        {
            public static void Draw(Rect position, SerializedProperty property, Color color)
            {
                Color initialColor = GUI.color;
                GUI.color = color;
                Draw(position, property);
                GUI.color = initialColor;
            }

            public static void Draw(Rect position, SerializedProperty property, ColorName colorName) { Draw(position, property, Colors.PropertyColor(colorName)); }
            public static void Draw(Rect position, SerializedProperty property, DColor dColor) { Draw(position, property, Colors.PropertyColor(dColor)); }
            public static void Draw(Rect position, SerializedProperty property) { EditorGUI.PropertyField(position, property, GUIContent.none, true); }


            public static void Draw(SerializedProperty property, params GUILayoutOption[] options) { EditorGUILayout.PropertyField(property, GUIContent.none, true, options); }

            public static void Draw(SerializedProperty property, float rowHeight)
            {
                GUILayout.BeginVertical(GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - Properties.SingleLineHeight) / 2);
                Draw(property, GUILayout.Height(Properties.SingleLineHeight), GUILayout.ExpandWidth(true));
                GUILayout.EndVertical();
            }

            public static void Draw(SerializedProperty property, DColor dColor, params GUILayoutOption[] options)
            {
                Color initialColor = GUI.color;
                GUI.color = Colors.PropertyColor(dColor);
                Draw(property, options);
                GUI.color = initialColor;
            }

            public static void Draw(SerializedProperty property, DColor dColor, float rowHeight)
            {
                GUILayout.BeginVertical(GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - Properties.SingleLineHeight) / 2);
                Draw(property, dColor, GUILayout.Height(Properties.SingleLineHeight), GUILayout.ExpandWidth(true));
                GUILayout.EndVertical();
            }

            public static void Draw(SerializedProperty property, DColor dColor, float fieldWidth, float rowHeight)
            {
                GUILayout.BeginVertical(GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - Properties.SingleLineHeight) / 2);
                Draw(property, dColor, GUILayout.Height(Properties.SingleLineHeight), GUILayout.Width(fieldWidth));
                GUILayout.EndVertical();
            }

            public static void Draw(SerializedProperty property, ColorName colorName, params GUILayoutOption[] options) { Draw(property, Colors.GetDColor(colorName), options); }
            public static void Draw(SerializedProperty property, ColorName colorName, float rowHeight) { Draw(property, Colors.GetDColor(colorName), rowHeight); }
            public static void Draw(SerializedProperty property, ColorName colorName, float fieldWidth, float rowHeight) { Draw(property, Colors.GetDColor(colorName), fieldWidth, rowHeight); }

            public static void Draw(SerializedProperty property, string label, ColorName propertyColorName, bool hasErrors = false) { Draw(property, label, propertyColorName, propertyColorName, hasErrors); }

            public static void Draw(SerializedProperty property, string label, ColorName propertyColorName, float fieldWidth, bool hasErrors = false)
            {
                Draw(property, label, propertyColorName, propertyColorName, fieldWidth, hasErrors);
            }
            
            public static void Draw(SerializedProperty property, string label, ColorName propertyColorName, ColorName textColorName, bool hasErrors = false)
            {
                Line.Draw(false, propertyColorName,
                          () =>
                          {
                              if (!string.IsNullOrEmpty(label))
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(label, Size.S, TextAlign.Left, hasErrors ? ColorName.Red : textColorName, Properties.SingleLineHeight);
                              }

                              Draw(property, hasErrors ? ColorName.Red : propertyColorName, Properties.SingleLineHeight);
                          });
            }
            
            public static void Draw(SerializedProperty property, string label, ColorName propertyColorName, ColorName textColorName, float fieldWidth, bool hasErrors = false)
            {
                Line.Draw(false, propertyColorName,
                          () =>
                          {
                              if (!string.IsNullOrEmpty(label))
                              {
                                  GUILayout.Space(Properties.Space(2));
                                  Label.Draw(label, Size.S, TextAlign.Left, hasErrors ? ColorName.Red : textColorName, Properties.SingleLineHeight);
                              }

                              Draw(property, hasErrors ? ColorName.Red : propertyColorName, fieldWidth, Properties.SingleLineHeight);
                          });
            }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options) { DrawWithFade(property, expanded.faded, indentContent, addExtraSpaceWhenExpanded, options); }

            public static void DrawWithFade(SerializedProperty property, float faded, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, indentContent)) Draw(property, options);
                FadeOut.End(faded, addExtraSpaceWhenExpanded, alpha);
            }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, DColor dColor, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options) { DrawWithFade(property, expanded.faded, dColor, indentContent, addExtraSpaceWhenExpanded, options); }

            public static void DrawWithFade(SerializedProperty property, float faded, DColor dColor, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, indentContent)) Draw(property, dColor, options);
                FadeOut.End(faded, addExtraSpaceWhenExpanded, alpha);
            }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, float rowHeight, DColor dColor, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, expanded.faded, rowHeight, dColor, indentContent, addExtraSpaceWhenExpanded); }

            public static void DrawWithFade(SerializedProperty property, float faded, float rowHeight, DColor dColor, bool indentContent = false, bool addExtraSpaceWhenExpanded = false)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, indentContent)) Draw(property, dColor, rowHeight);
                FadeOut.End(faded, addExtraSpaceWhenExpanded, alpha);
            }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, float rowHeight, DColor dColor, float propertyWidth, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, expanded.faded, rowHeight, dColor, propertyWidth, indentContent, addExtraSpaceWhenExpanded); }

            public static void DrawWithFade(SerializedProperty property, float faded, float rowHeight, DColor dColor, float propertyWidth, bool indentContent = false, bool addExtraSpaceWhenExpanded = false)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, indentContent)) Draw(property, dColor, propertyWidth, rowHeight);
                FadeOut.End(faded, addExtraSpaceWhenExpanded, alpha);
            }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, ColorName colorName, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options) { DrawWithFade(property, expanded.faded, Colors.GetDColor(colorName), indentContent, addExtraSpaceWhenExpanded, options); }
            public static void DrawWithFade(SerializedProperty property, float faded, ColorName colorName, bool indentContent = false, bool addExtraSpaceWhenExpanded = false, params GUILayoutOption[] options) { DrawWithFade(property, faded, Colors.GetDColor(colorName), indentContent, addExtraSpaceWhenExpanded, options); }
            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, float rowHeight, ColorName colorName, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, expanded.faded, rowHeight, Colors.GetDColor(colorName), indentContent, addExtraSpaceWhenExpanded); }
            public static void DrawWithFade(SerializedProperty property, float faded, float rowHeight, ColorName colorName, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, faded, rowHeight, Colors.GetDColor(colorName), indentContent, addExtraSpaceWhenExpanded); }
            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, float rowHeight, ColorName colorName, float propertyWidth, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, expanded.faded, rowHeight, Colors.GetDColor(colorName), propertyWidth, indentContent, addExtraSpaceWhenExpanded); }
            public static void DrawWithFade(SerializedProperty property, float faded, float rowHeight, ColorName colorName, float propertyWidth, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { DrawWithFade(property, faded, rowHeight, Colors.GetDColor(colorName), propertyWidth, indentContent, addExtraSpaceWhenExpanded); }

            public static void UnityEvent(SerializedProperty property, string label, ColorName colorName, int eventsCount) { UnityEvent(property, label, Colors.GetDColor(colorName), eventsCount); }

            public static void UnityEvent(SerializedProperty property, string label, DColor dColor, int eventsCount)
            {
                Color initialColor = GUI.color;
                GUI.color = Colors.PropertyColor(eventsCount > 0 ? dColor : Colors.DisabledBackgroundDColor);
                EditorGUILayout.PropertyField(property, new GUIContent(label), true);
                GUI.color = initialColor;
            }

            public static void UnityEventWithFade(SerializedProperty property, AnimBool expanded, string label, ColorName colorName, int eventsCount, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { UnityEventWithFade(property, expanded.faded, label, Colors.GetDColor(colorName), eventsCount, indentContent, addExtraSpaceWhenExpanded); }

            public static void UnityEventWithFade(SerializedProperty property, float faded, string label, ColorName colorName, int eventsCount, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { UnityEventWithFade(property, faded, label, Colors.GetDColor(colorName), eventsCount, indentContent, addExtraSpaceWhenExpanded); }

            public static void UnityEventWithFade(SerializedProperty property, AnimBool expanded, string label, DColor dColor, int eventsCount, bool indentContent = false, bool addExtraSpaceWhenExpanded = false) { UnityEventWithFade(property, expanded.faded, label, dColor, eventsCount, indentContent, addExtraSpaceWhenExpanded); }

            public static void UnityEventWithFade(SerializedProperty property, float faded, string label, DColor dColor, int eventsCount, bool indentContent = false, bool addExtraSpaceWhenExpanded = false)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, indentContent)) UnityEvent(property, label, dColor, eventsCount);
                FadeOut.End(faded, addExtraSpaceWhenExpanded, alpha);
            }
        }
    }
}