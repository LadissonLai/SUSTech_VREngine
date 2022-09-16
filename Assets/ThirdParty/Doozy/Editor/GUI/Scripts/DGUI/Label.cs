// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Label
        {
            public const Size DEFAULT_LABEL_FONT_SIZE = Size.M;
            public const TextAlign DEFAULT_TEXT_ALIGN = TextAlign.Left;

            public static GUIStyle Style(Size size, TextAlign textAlign, ColorName textColorName) { return Colors.ColorTextOfGUIStyle(Style(size, textAlign), Colors.TextColor(textColorName)); }
            public static GUIStyle Style(Size size, TextAlign textAlign, DColor textDColor) { return Colors.ColorTextOfGUIStyle(Style(size, textAlign), Colors.TextColor(textDColor)); }
            public static GUIStyle Style(Size size, ColorName textColorName) { return Colors.ColorTextOfGUIStyle(Style(size), Colors.TextColor(textColorName)); }
            public static GUIStyle Style(Size size, DColor textDColor) { return Colors.ColorTextOfGUIStyle(Style(size), Colors.TextColor(textDColor)); }
            public static GUIStyle Style(Size size = DEFAULT_LABEL_FONT_SIZE) { return Style(size, DEFAULT_TEXT_ALIGN); }

            public static GUIStyle Style(Size size, TextAlign textAlign)
            {
                switch (size)
                {
                    case Size.S:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.LabelSLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.LabelSCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.LabelSRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }

                    case Size.M:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.LabelMLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.LabelMCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.LabelMRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }
                    case Size.L:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.LabelLLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.LabelLCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.LabelLRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }

                    case Size.XL:
                        switch (textAlign)
                        {
                            case TextAlign.Left:   return Styles.GetStyle(Styles.StyleName.LabelXLLeft);
                            case TextAlign.Center: return Styles.GetStyle(Styles.StyleName.LabelXLCenter);
                            case TextAlign.Right:  return Styles.GetStyle(Styles.StyleName.LabelXLRight);
                            default:               throw new ArgumentOutOfRangeException("textAlign", textAlign, null);
                        }

                    default: throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static Rect Draw(Rect position, string text, Size size, TextAlign textAlign, ColorName textColorName) { return Draw(position, text, Style(size, textAlign, textColorName)); }
            public static Rect Draw(Rect position, string text, Size size, TextAlign textAlign, DColor textDColor) { return Draw(position, text, Style(size, textAlign, textDColor)); }
            public static Rect Draw(Rect position, string text, Size size, TextAlign textAlign) { return Draw(position, text, Style(size, textAlign)); }
            public static Rect Draw(Rect position, string text, Size size, ColorName textColorName) { return Draw(position, text, Style(size, textColorName)); }
            public static Rect Draw(Rect position, string text, Size size, DColor textDColor) { return Draw(position, text, Style(size, textDColor)); }
            public static Rect Draw(Rect position, string text, Size size) { return Draw(position, text, Style(size)); }
            public static Rect Draw(Rect position, string text, GUIStyle style, ColorName textColorName) { return Draw(position, text, Colors.ColorTextOfGUIStyle(style, textColorName)); }
            public static Rect Draw(Rect position, string text, GUIStyle style, DColor textDColor) { return Draw(position, text, Colors.ColorTextOfGUIStyle(style, textDColor)); }
            public static Rect Draw(Rect position, string text, GUIStyle style) { return Draw(position, new GUIContent(text), style); }
            public static Rect Draw(Rect position, GUIContent content, Size size, TextAlign textAlign, ColorName textColorName) { return Draw(position, content, Style(size, textAlign, textColorName)); }
            public static Rect Draw(Rect position, GUIContent content, Size size, TextAlign textAlign, DColor textDColor) { return Draw(position, content, Style(size, textAlign, textDColor)); }
            public static Rect Draw(Rect position, GUIContent content, Size size, TextAlign textAlign) { return Draw(position, content, Style(size, textAlign)); }
            public static Rect Draw(Rect position, GUIContent content, Size size, ColorName textColorName) { return Draw(position, content, Style(size, textColorName)); }
            public static Rect Draw(Rect position, GUIContent content, Size size, DColor textDColor) { return Draw(position, content, Style(size, textDColor)); }
            public static Rect Draw(Rect position, GUIContent content, Size size) { return Draw(position, content, Style(size)); }
            public static Rect Draw(Rect position, GUIContent content, GUIStyle style, ColorName textColorName) { return Draw(position, content, Colors.ColorTextOfGUIStyle(style, textColorName)); }
            public static Rect Draw(Rect position, GUIContent content, GUIStyle style, DColor textDColor) { return Draw(position, content, Colors.ColorTextOfGUIStyle(style, textDColor)); }

            public static Rect Draw(Rect position, GUIContent content, GUIStyle style)
            {
                var rect = new Rect(position.x, position.y, style.CalcSize(content).x, position.height);
                GUI.Label(rect, content, style);
                return rect;
            }

            public static void Draw(string text, Size textSize, TextAlign textAlign, ColorName textColorName, float rowHeight) { Draw(text, Style(textSize, textAlign, textColorName), rowHeight); }
            public static void Draw(string text, Size textSize, TextAlign textAlign, DColor textDColor, float rowHeight) { Draw(text, Style(textSize, textAlign, textDColor), rowHeight); }
            public static void Draw(string text, Size textSize, TextAlign textAlign, float rowHeight) { Draw(text, Style(textSize, textAlign), rowHeight); }
            public static void Draw(string text, Size textSize, ColorName textColorName, float rowHeight) { Draw(text, Style(textSize, textColorName), rowHeight); }
            public static void Draw(string text, Size textSize, DColor textDColor, float rowHeight) { Draw(text, Style(textSize, textDColor), rowHeight); }
            public static void Draw(string text, Size textSize, float rowHeight) { Draw(text, Style(textSize), rowHeight); }
            public static void Draw(string text, GUIStyle labelStyle, ColorName textColorName, float rowHeight) { Draw(text, Colors.ColorTextOfGUIStyle(labelStyle, textColorName), rowHeight); }
            public static void Draw(string text, GUIStyle labelStyle, DColor textDColor, float rowHeight) { Draw(text, Colors.ColorTextOfGUIStyle(labelStyle, textDColor), rowHeight); }
            public static void Draw(string text, GUIStyle labelStyle, float rowHeight) { Draw(new GUIContent(text), labelStyle, rowHeight); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign, ColorName textColorName, float rowHeight) { Draw(content, Style(textSize, textAlign, textColorName), rowHeight); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign, DColor textDColor, float rowHeight) { Draw(content, Style(textSize, textAlign, textDColor), rowHeight); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign, float rowHeight) { Draw(content, Style(textSize, textAlign), rowHeight); }
            public static void Draw(GUIContent content, Size textSize, ColorName textColorName, float rowHeight) { Draw(content, Style(textSize, textColorName), rowHeight); }
            public static void Draw(GUIContent content, Size textSize, DColor textDColor, float rowHeight) { Draw(content, Style(textSize, textDColor), rowHeight); }
            public static void Draw(GUIContent content, Size textSize, float rowHeight) { Draw(content, Style(textSize), rowHeight); }
            public static void Draw(GUIContent content, GUIStyle labelStyle, ColorName textColorName, float rowHeight) { Draw(content, Colors.ColorTextOfGUIStyle(labelStyle, textColorName), rowHeight); }
            public static void Draw(GUIContent content, GUIStyle labelStyle, DColor textDColor, float rowHeight) { Draw(content, Colors.ColorTextOfGUIStyle(labelStyle, textDColor), rowHeight); }

            public static void Draw(GUIContent content, GUIStyle labelStyle, float rowHeight)
            {
                Vector2 contentSize = labelStyle.CalcSize(content);
                GUILayout.BeginVertical(GUILayout.Width(contentSize.x), GUILayout.Height(rowHeight));
                {
                    GUILayout.Space((rowHeight - contentSize.y) / 2);
                    GUILayout.Label(content, labelStyle, GUILayout.Width(contentSize.x));
                }
                GUILayout.EndVertical();
            }

            public static void Draw(string text, Size textSize, TextAlign textAlign, ColorName textColorName) { Draw(text, Style(textSize, textAlign, textColorName)); }
            public static void Draw(string text, Size textSize, TextAlign textAlign, DColor textDColor) { Draw(text, Style(textSize, textAlign, textDColor)); }
            public static void Draw(string text, Size textSize, TextAlign textAlign) { Draw(text, Style(textSize, textAlign)); }
            public static void Draw(string text, Size textSize, ColorName textColorName) { Draw(text, Style(textSize, textColorName)); }
            public static void Draw(string text, Size textSize, DColor textDColor) { Draw(text, Style(textSize, textDColor)); }
            public static void Draw(string text, Size textSize) { Draw(text, Style(textSize)); }
            public static void Draw(string text, GUIStyle labelStyle, ColorName textColorName) { Draw(text, Colors.ColorTextOfGUIStyle(labelStyle, textColorName)); }
            public static void Draw(string text, GUIStyle labelStyle, DColor textDColor) { Draw(text, Colors.ColorTextOfGUIStyle(labelStyle, textDColor)); }
            public static void Draw(string text, GUIStyle labelStyle) { Draw(new GUIContent(text), labelStyle); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign, ColorName textColorName) { Draw(content, Style(textSize, textAlign, textColorName)); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign, DColor textDColor) { Draw(content, Style(textSize, textAlign, textDColor)); }
            public static void Draw(GUIContent content, Size textSize, TextAlign textAlign) { Draw(content, Style(textSize, textAlign)); }
            public static void Draw(GUIContent content, Size textSize, ColorName textColorName) { Draw(content, Style(textSize, textColorName)); }
            public static void Draw(GUIContent content, Size textSize, DColor textDColor) { Draw(content, Style(textSize, textDColor)); }
            public static void Draw(GUIContent content, Size textSize) { Draw(content, Style(textSize)); }
            public static void Draw(GUIContent content, GUIStyle labelStyle, ColorName textColorName) { Draw(content, Colors.ColorTextOfGUIStyle(labelStyle, textColorName)); }
            public static void Draw(GUIContent content, GUIStyle labelStyle, DColor textDColor) { Draw(content, Colors.ColorTextOfGUIStyle(labelStyle, textDColor)); }
            public static void Draw(GUIContent content, GUIStyle labelStyle) { GUILayout.Label(content, labelStyle, GUILayout.Width(labelStyle.CalcSize(content).x)); }
        }
    }
}