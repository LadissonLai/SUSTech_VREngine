// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class WindowHeader
        {
            private static GUIStyle s_titleStyle;

            public static GUIStyle TitleStyle
            {
                get
                {
                    if (s_titleStyle != null) return s_titleStyle;
                    s_titleStyle = new GUIStyle
                    {
                        normal = {textColor = Color.white},
                        font = Fonts.SansationBold,
                        fontSize = 28,
                        alignment = TextAnchor.MiddleLeft,
                        stretchWidth = true
                    };
                    return s_titleStyle;
                }
            }

            private static GUIStyle s_subtitleStyle;

            public static GUIStyle SubtitleStyle
            {
                get
                {
                    if (s_subtitleStyle != null) return s_subtitleStyle;
                    s_subtitleStyle = new GUIStyle(TitleStyle)
                    {
                        fontSize = 14,
                        font = Fonts.SansationLight
                    };
                    return s_subtitleStyle;
                }
            }


            public static void Draw(string title, string subtitle, GUIStyle icon, ColorName colorName)
            {
                Color initialColor = GUI.color;

                float height = WindowIcon.Height;

                GUILayout.BeginVertical();
                {
                    GUI.color = Colors.GetDColor(colorName).Dark.WithAlpha(GUI.color.a * 0.1f);
                    GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.WhiteGradientTopToBottom), GUILayout.ExpandWidth(true), GUILayout.Height(height));
                    GUILayout.Space(-height);
                    GUI.color = initialColor;
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(Properties.Space(4));
                        Icon.Draw(icon, height * 0.8f, height, colorName);
                        GUILayout.Space(Properties.Space(4));
                        GUILayout.BeginVertical(GUILayout.Height(height));
                        {
                            GUILayout.FlexibleSpace();
                            Label.Draw(title.ToUpper(), TitleStyle, colorName);
                            Label.Draw(subtitle, SubtitleStyle, colorName);
                            GUILayout.Space(Properties.Space());
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                        WindowIcon.Draw(icon, colorName);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                GUI.color = initialColor;
            }

            private static class WindowIcon
            {
                public static float Width { get { return Styles.GetStyle(Styles.StyleName.WindowIconBackground).fixedWidth; } }
                public static float Height { get { return Styles.GetStyle(Styles.StyleName.WindowIconBackground).fixedHeight; } }

                public static void Draw(GUIStyle icon, ColorName colorNameOld)
                {
                    Color initialColor = GUI.color;
                    float iconSize = Height * 0.35f;
                    GUILayout.BeginHorizontal(GUILayout.Width(Width), GUILayout.Height(Height));
                    {
                        GUI.color = Colors.GetDColor(colorNameOld).Normal.WithAlpha(GUI.color.a);
                        GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.WindowIconBackground));
                        GUI.color = initialColor;
                        GUILayout.Space(-iconSize - Height * 0.1f);
                        GUILayout.BeginVertical(GUILayout.Width(iconSize), GUILayout.Height(iconSize));
                        {
                            GUILayout.Space(Height * 0.1f);
                            GUI.color = Colors.GetDColor(colorNameOld).Normal.WithAlpha(GUI.color.a);
                            GUILayout.Label(GUIContent.none, icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize));
                            GUI.color = initialColor;
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}