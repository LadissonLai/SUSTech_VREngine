// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Toggle
        {
            private static Color Color(bool enabled, ColorName colorName) { return Color(enabled, Colors.GetDColor(colorName)); }
            private static Color Color(bool enabled, DColor dColor) { return enabled ? dColor.Normal : UnityEngine.Color.white; }

            public static bool Draw(Rect rect, SerializedProperty property, GUIStyle toggleStyle, ColorName colorName) { return Draw(rect, property, toggleStyle, Colors.GetDColor(colorName)); }

            public static bool Draw(Rect rect, SerializedProperty property, GUIStyle toggleStyle, DColor dColor)
            {
                Color initialColor = GUI.color;
                GUI.color = Color(property.boolValue, dColor);
                bool value = property.boolValue;
                EditorGUI.BeginChangeCheck();
                value = GUI.Toggle(rect, value, GUIContent.none, toggleStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    property.boolValue = value;
                    Properties.ResetKeyboardFocus();
                }

                GUI.color = initialColor;
                return value;
            }

            public static bool Draw(bool enabled, GUIStyle toggleStyle, ColorName colorName) { return Draw(enabled, toggleStyle, Colors.GetDColor(colorName)); }

            public static bool Draw(bool enabled, GUIStyle toggleStyle, DColor dColor)
            {
                DColor backgroundColor = enabled ? dColor : Colors.DisabledBackgroundDColor;

                Color initialColor = GUI.color;
                GUI.color = Color(enabled, backgroundColor);
                bool result = enabled;
                EditorGUI.BeginChangeCheck();
                result = GUILayout.Toggle(result, GUIContent.none, toggleStyle);
                GUI.color = initialColor;
                if (!EditorGUI.EndChangeCheck()) return result;
                Properties.ResetKeyboardFocus();
                return result;
            }

            public static bool Draw(bool enabled, GUIStyle toggleStyle, ColorName colorName, float rowHeight) { return Draw(enabled, toggleStyle, Colors.GetDColor(colorName), rowHeight); }

            public static bool Draw(bool enabled, GUIStyle toggleStyle, DColor dColor, float rowHeight)
            {
                bool result;
                GUILayout.BeginVertical(GUILayout.Width(toggleStyle.fixedHeight), GUILayout.Height(rowHeight));
                {
                    GUILayout.Space((rowHeight - toggleStyle.fixedHeight) / 2);
                    result = Draw(enabled, toggleStyle, dColor);
                }
                GUILayout.EndVertical();
                return result;
            }

            public static bool Draw(bool enabled, string label, GUIStyle toggleStyle, ColorName colorName, bool drawBackground, bool expandWidth, float rowHeight) { return Draw(enabled, label, toggleStyle, Colors.GetDColor(colorName), drawBackground, expandWidth, rowHeight); }

            public static bool Draw(bool enabled, string label, GUIStyle toggleStyle, DColor dColor, bool drawBackground, bool expandWidth, float rowHeight)
            {
                DColor backgroundColor = enabled ? dColor : Colors.DisabledBackgroundDColor;
                DColor textColorColor = enabled ? dColor : Colors.DisabledTextDColor;

                bool result;
                if (rowHeight < 0) rowHeight = Properties.SingleLineHeight;
                float backgroundHeight = rowHeight + (drawBackground ? Properties.Space(2) : 0);
                var content = new GUIContent(label);
                GUIStyle labelStyle = Label.Style(Size.S);
                Vector2 labelSize = labelStyle.CalcSize(content);
                float totalWidth = Properties.Space(drawBackground ? 2 : 1) + toggleStyle.fixedWidth + Properties.Space() + labelSize.x + Properties.Space(drawBackground ? 3 : 1);
                GUILayout.BeginVertical(GUILayout.Height(backgroundHeight), expandWidth ? GUILayout.ExpandWidth(true) : GUILayout.Width(totalWidth));
                {
                    if (drawBackground)
                    {
                        Background.Draw(backgroundColor, backgroundHeight); //draw background
                        GUILayout.Space(-backgroundHeight + Properties.Space());
                    }

                    GUILayout.BeginHorizontal(GUILayout.Height(rowHeight), expandWidth ? GUILayout.ExpandWidth(true) : GUILayout.Width(totalWidth));
                    {
                        GUILayout.Space(Properties.Space(drawBackground ? 2 : 1));
                        result = Draw(enabled, toggleStyle, backgroundColor, rowHeight); //draw toggle
                        GUILayout.Space(Properties.Space());
                        float initialAlpha = GUI.color.a;
                        GUI.color = GUI.color.WithAlpha(initialAlpha * Properties.TextIconAlphaValue(enabled));
                        Label.Draw(content, labelStyle, textColorColor, rowHeight); //draw label
                        GUI.color = GUI.color.WithAlpha(initialAlpha);
                        GUILayout.Space(Properties.Space(drawBackground ? 3 : 1));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                return result;
            }

            public static bool Draw(SerializedProperty property, GUIStyle toggleStyle, ColorName colorName) { return Draw(property, toggleStyle, Colors.GetDColor(colorName)); }

            public static bool Draw(SerializedProperty property, GUIStyle toggleStyle, DColor dColor)
            {
                bool result = property.boolValue;
                EditorGUI.BeginChangeCheck();
                result = Draw(result, toggleStyle, dColor);
                if (EditorGUI.EndChangeCheck()) property.boolValue = result;
                return result;
            }

            public static bool Draw(SerializedProperty property, GUIStyle toggleStyle, ColorName colorName, float rowHeight) { return Draw(property, toggleStyle, Colors.GetDColor(colorName), rowHeight); }

            public static bool Draw(SerializedProperty property, GUIStyle toggleStyle, DColor dColor, float rowHeight)
            {
                DColor backgroundColor = property.boolValue ? dColor : Colors.DisabledBackgroundDColor;

                bool result;
                GUILayout.BeginVertical(GUILayout.Width(toggleStyle.fixedHeight), GUILayout.Height(rowHeight));
                {
                    GUILayout.Space((rowHeight - toggleStyle.fixedHeight) / 2);
                    result = Draw(property, toggleStyle, backgroundColor);
                }
                GUILayout.EndVertical();
                return result;
            }

            public static bool Draw(SerializedProperty property, string label, GUIStyle toggleStyle, ColorName colorName, bool drawBackground, bool expandWidth, float rowHeight) { return Draw(property, label, toggleStyle, Colors.GetDColor(colorName), drawBackground, expandWidth, rowHeight); }

            public static bool Draw(SerializedProperty property, string label, GUIStyle toggleStyle, DColor dColor, bool drawBackground, bool expandWidth, float rowHeight = -1)
            {
                DColor backgroundColor = property.boolValue ? dColor : Colors.DisabledBackgroundDColor;
                DColor textColorColor = property.boolValue ? dColor : Colors.DisabledTextDColor;

                bool result;
                if (rowHeight < 0) rowHeight = Properties.SingleLineHeight;
                float backgroundHeight = rowHeight + (drawBackground ? Properties.Space(2) : 0);
                var content = new GUIContent(label);
                GUIStyle labelStyle = Label.Style(Size.S);
                Vector2 labelSize = labelStyle.CalcSize(content);
                float totalWidth = Properties.Space(drawBackground ? 2 : 1) + toggleStyle.fixedWidth + Properties.Space() + labelSize.x + Properties.Space(drawBackground ? 3 : 1);
                GUILayout.BeginVertical(GUILayout.Height(backgroundHeight), expandWidth ? GUILayout.ExpandWidth(true) : GUILayout.Width(totalWidth));
                {
                    if (drawBackground)
                    {
                        Background.Draw(backgroundColor, backgroundHeight); //draw background
                        GUILayout.Space(-backgroundHeight + Properties.Space());
                    }

                    GUILayout.BeginHorizontal(GUILayout.Height(rowHeight), expandWidth ? GUILayout.ExpandWidth(true) : GUILayout.Width(totalWidth));
                    {
                        GUILayout.Space(Properties.Space(drawBackground ? 2 : 1));
                        result = Draw(property, toggleStyle, backgroundColor, rowHeight); //draw toggle
                        GUILayout.Space(Properties.Space());
                        float initialAlpha = GUI.color.a;
                        GUI.color = GUI.color.WithAlpha(initialAlpha * Properties.TextIconAlphaValue(property.boolValue));
                        Label.Draw(content, labelStyle, textColorColor, rowHeight); //draw label
                        GUI.color = GUI.color.WithAlpha(initialAlpha);
                        GUILayout.Space(Properties.Space(drawBackground ? 3 : 1));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                return result;
            }

            public static class Switch
            {
                public static float Width { get { return Sizes.SWITCH_WIDTH; } }
                public static float Height { get { return Sizes.SWITCH_HEIGHT; } }

                public static GUIStyle Style(ToggleState toggleState)
                {
                    switch (toggleState)
                    {
                        case ToggleState.Disabled:    return Styles.GetStyle(Styles.StyleName.SwitchDisabled);
                        case ToggleState.Enabled:     return Styles.GetStyle(Styles.StyleName.SwitchEnabled);
                        case ToggleState.MultiValues: return Styles.GetStyle(Styles.StyleName.SwitchMultiValues);
                        default:                      throw new ArgumentOutOfRangeException("toggleState", toggleState, null);
                    }
                }

                public static GUIStyle Style(bool enabled, bool hasMultipleDifferentValues = false)
                {
                    return hasMultipleDifferentValues
                        ? Style(ToggleState.MultiValues)
                        : enabled
                            ? Style(ToggleState.Enabled)
                            : Style(ToggleState.Disabled);
                }

                public static GUIStyle Style(SerializedProperty boolProperty) { return Style(boolProperty.boolValue, boolProperty.hasMultipleDifferentValues); }

                public static bool Draw(Rect rect, SerializedProperty property, DColor dColor) { return Toggle.Draw(rect, property, Style(property), dColor); }
                public static bool Draw(Rect rect, SerializedProperty property, ColorName colorName) { return Toggle.Draw(rect, property, Style(property), colorName); }

                public static bool Draw(bool enabled, DColor dColor, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), dColor); }
                public static bool Draw(bool enabled, ColorName colorName, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), colorName); }
                public static bool Draw(bool enabled, DColor dColor, float rowHeight, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), dColor, rowHeight); }
                public static bool Draw(bool enabled, ColorName colorName, float rowHeight, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), colorName, rowHeight); }
                public static bool Draw(bool enabled, string label, DColor dColor, bool multiObjectEditing, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(enabled, label, Style(enabled, multiObjectEditing), dColor, drawBackground, expandWidth, rowHeight); }
                public static bool Draw(bool enabled, string label, ColorName colorName, bool multiObjectEditing, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(enabled, label, Style(enabled, multiObjectEditing), colorName, drawBackground, expandWidth, rowHeight); }

                public static bool Draw(SerializedProperty property, DColor dColor) { return Toggle.Draw(property, Style(property), dColor); }
                public static bool Draw(SerializedProperty property, ColorName colorName) { return Toggle.Draw(property, Style(property), colorName); }
                public static bool Draw(SerializedProperty property, DColor dColor, float rowHeight) { return Toggle.Draw(property, Style(property), dColor, rowHeight); }
                public static bool Draw(SerializedProperty property, ColorName colorName, float rowHeight) { return Toggle.Draw(property, Style(property), colorName, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, DColor dColor, float rowHeight) { return Toggle.Draw(property, label, Style(property), dColor, false, false, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, ColorName colorName, float rowHeight) { return Toggle.Draw(property, label, Style(property), colorName, false, false, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, DColor dColor, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(property, label, Style(property), dColor, drawBackground, expandWidth, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, ColorName colorName, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(property, label, Style(property), colorName, drawBackground, expandWidth, rowHeight); }
            }

            public static class Checkbox
            {
                public static float Width { get { return Sizes.CHECKBOX_WIDTH; } }
                public static float Height { get { return Sizes.CHECKBOX_HEIGHT; } }

                public static GUIStyle Style(ToggleState toggleState)
                {
                    switch (toggleState)
                    {
                        case ToggleState.Disabled:    return Styles.GetStyle(Styles.StyleName.CheckBoxDisabled);
                        case ToggleState.Enabled:     return Styles.GetStyle(Styles.StyleName.CheckBoxEnabled);
                        case ToggleState.MultiValues: return Styles.GetStyle(Styles.StyleName.CheckBoxMultiValues);
                        default:                      throw new ArgumentOutOfRangeException("toggleState", toggleState, null);
                    }
                }

                public static GUIStyle Style(bool enabled, bool hasMultipleDifferentValues)
                {
                    return hasMultipleDifferentValues
                        ? Style(ToggleState.MultiValues)
                        : enabled
                            ? Style(ToggleState.Enabled)
                            : Style(ToggleState.Disabled);
                }

                public static GUIStyle Style(SerializedProperty boolProperty) { return Style(boolProperty.boolValue, boolProperty.hasMultipleDifferentValues); }

                public static bool Draw(Rect rect, SerializedProperty property, DColor dColor) { return Toggle.Draw(rect, property, Style(property), dColor); }
                public static bool Draw(Rect rect, SerializedProperty property, ColorName colorName) { return Toggle.Draw(rect, property, Style(property), colorName); }

                public static bool Draw(bool enabled, DColor dColor, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), dColor); }
                public static bool Draw(bool enabled, ColorName colorName, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), colorName); }
                public static bool Draw(bool enabled, DColor dColor, float rowHeight, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), dColor, rowHeight); }
                public static bool Draw(bool enabled, ColorName colorName, float rowHeight, bool multiObjectEditing = false) { return Toggle.Draw(enabled, Style(enabled, multiObjectEditing), colorName, rowHeight); }
                public static bool Draw(bool enabled, string label, DColor dColor, bool multiObjectEditing, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(enabled, label, Style(enabled, multiObjectEditing), dColor, drawBackground, expandWidth, rowHeight); }
                public static bool Draw(bool enabled, string label, ColorName colorName, bool multiObjectEditing, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(enabled, label, Style(enabled, multiObjectEditing), colorName, drawBackground, expandWidth, rowHeight); }

                public static bool Draw(SerializedProperty property, DColor dColor) { return Toggle.Draw(property, Style(property), dColor); }
                public static bool Draw(SerializedProperty property, ColorName colorName) { return Toggle.Draw(property, Style(property), colorName); }
                public static bool Draw(SerializedProperty property, DColor dColor, float rowHeight) { return Toggle.Draw(property, Style(property), dColor, rowHeight); }
                public static bool Draw(SerializedProperty property, ColorName colorName, float rowHeight) { return Toggle.Draw(property, Style(property), colorName, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, DColor dColor, float rowHeight) { return Toggle.Draw(property, label, Style(property), dColor, false, false, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, ColorName colorName, float rowHeight) { return Toggle.Draw(property, label, Style(property), colorName, false, false, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, DColor dColor, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(property, label, Style(property), dColor, drawBackground, expandWidth, rowHeight); }
                public static bool Draw(SerializedProperty property, string label, ColorName colorName, bool drawBackground, bool expandWidth, float rowHeight = -1) { return Toggle.Draw(property, label, Style(property), colorName, drawBackground, expandWidth, rowHeight); }
            }
        }
    }
}