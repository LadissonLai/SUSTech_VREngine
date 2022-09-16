// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Bar
        {
            public const float CARET_SIZE_RATIO = 0.9f;
            public static float Height(Size size) { return Sizes.BarHeight(size); }

            public static GUIStyle BarStyle(Size size, bool enabled)
            {
                switch (size)
                {
                    case Size.S:  return Styles.GetStyle(enabled ? Styles.StyleName.BarSEnabled : Styles.StyleName.BarSDisabled);
                    case Size.M:  return Styles.GetStyle(enabled ? Styles.StyleName.BarMEnabled : Styles.StyleName.BarMDisabled);
                    case Size.L:  return Styles.GetStyle(enabled ? Styles.StyleName.BarLEnabled : Styles.StyleName.BarLDisabled);
                    case Size.XL: return Styles.GetStyle(enabled ? Styles.StyleName.BarXLEnabled : Styles.StyleName.BarXLDisabled);
                    default:      throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static GUIStyle LabelStyle(Size size, bool enabled)
            {
                switch (size)
                {
                    case Size.S:  return Styles.GetStyle(enabled ? Styles.StyleName.BarSEnabledLabel : Styles.StyleName.BarSDisabledLabel);
                    case Size.M:  return Styles.GetStyle(enabled ? Styles.StyleName.BarMEnabledLabel : Styles.StyleName.BarMDisabledLabel);
                    case Size.L:  return Styles.GetStyle(enabled ? Styles.StyleName.BarLEnabledLabel : Styles.StyleName.BarLDisabledLabel);
                    case Size.XL: return Styles.GetStyle(enabled ? Styles.StyleName.BarXLEnabledLabel : Styles.StyleName.BarXLDisabledLabel);
                    default:      throw new ArgumentOutOfRangeException("size", size, null);
                }
            }

            public static bool Draw(string text, Size size, Caret.CaretType caretType, ColorName barColorName, AnimBool expanded)
            {
                Color initialColor = GUI.color;
                bool result = false;

                GUIStyle barStyle = BarStyle(size, expanded.target);
                var labelStyle = new GUIStyle(LabelStyle(size, expanded.target)) {normal = {textColor = Colors.TextColor(barColorName)}};

                float barHeight = barStyle.fixedHeight;
                float caretSize = barHeight * CARET_SIZE_RATIO;

                GUILayout.BeginVertical(GUILayout.Height(barHeight));
                {
                    GUI.color = Colors.BarColor(barColorName, expanded.target);
                    if (GUILayout.Button(GUIContent.none, barStyle, GUILayout.ExpandWidth(true)))
                    {
                        expanded.target = !expanded.target;
                        Properties.ResetKeyboardFocus();
                        Event.current.Use();
                        result = true;
                    }

                    GUI.color = initialColor;

                    GUILayout.Space(-barHeight);
                    GUILayout.Label(new GUIContent(text), labelStyle, GUILayout.ExpandWidth(true), GUILayout.Height(barHeight));

                    GUILayout.Space(-barHeight);
                    Caret.Draw(expanded, barColorName, caretType, caretSize);
                }
                GUILayout.EndVertical();
                return result;
            }

            public static void Draw(Rect rect, string text, Size size, ColorName barColorName, AnimBool expanded)
            {
                if (GUI.Button(rect, new GUIContent(text), BarStyle(size, expanded.target)))
                {
                    expanded.target = !expanded.target;
                    Properties.ResetKeyboardFocus();
                    Event.current.Use();
                }
            }

            public static class Caret
            {
                public enum CaretType
                {
                    Caret,
                    Move,
                    Rotate,
                    Scale,
                    Fade
                }

                private static Texture[] s_carets, s_moveCarets, s_rotateCarets, s_scaleCarets, s_fadeCarets;

                private static Texture[] Carets { get { return s_carets ?? (s_carets = GetCarets(CaretType.Caret)); } }
                private static Texture[] MoveCarets { get { return s_moveCarets ?? (s_moveCarets = GetCarets(CaretType.Move)); } }
                private static Texture[] RotateCarets { get { return s_rotateCarets ?? (s_rotateCarets = GetCarets(CaretType.Rotate)); } }
                private static Texture[] ScaleCarets { get { return s_scaleCarets ?? (s_scaleCarets = GetCarets(CaretType.Scale)); } }
                private static Texture[] FadeCarets { get { return s_fadeCarets ?? (s_fadeCarets = GetCarets(CaretType.Fade)); } }


                private static Texture[] GetCarets(CaretType caretType)
                {
                    string textureName = "Caret" + (caretType != CaretType.Caret ? caretType.ToString() : "");
                    return AnimatedTexture.GetAnimatedTextureArray(textureName, DoozyPath.EDITOR_IMAGES_PATH, false);
                }

                public static void Draw(AnimBool expanded, ColorName colorName, CaretType caretType, float size)
                {
                    switch (caretType)
                    {
                        case CaretType.Caret:
                            AnimatedTexture.Draw(expanded, Carets, size, colorName);
                            break;
                        case CaretType.Move:
                            AnimatedTexture.Draw(expanded, MoveCarets, size, colorName);
                            break;
                        case CaretType.Rotate:
                            AnimatedTexture.Draw(expanded, RotateCarets, size, colorName);
                            break;
                        case CaretType.Scale:
                            AnimatedTexture.Draw(expanded, ScaleCarets, size, colorName);
                            break;
                        case CaretType.Fade:
                            AnimatedTexture.Draw(expanded, FadeCarets, size, colorName);
                            break;
                    }
                }
            }
        }
    }
}