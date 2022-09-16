// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class IconGroup
        {
            public static float IconSize { get { return Properties.SingleLineHeight - Properties.Space(2); } }
            public static float IconSpacing { get { return Properties.Space(2); } }
            public static Data IconSpacingData = new Data(IconSpacing);

            public static void Draw(List<Data> items, float lineHeight, bool drawBackground = true)
            {
                Color initialColor = GUI.color;

                float width = items.Sum(icon => icon.Width) + Properties.Space(4);
                float height = lineHeight + Properties.Space(2);


                GUILayout.BeginHorizontal(GUILayout.Height(height));
                {
                    if (drawBackground)
                    {
                        GUI.color = EditorGUIUtility.isProSkin
                            ? Colors.GetDColor(ColorName.Gray).Normal.WithAlpha(GUI.color.a * 0.5f)
                            : Colors.GetDColor(ColorName.Gray).Light.WithAlpha(GUI.color.a * 0.5f);

                        GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.BackgroundRound), GUILayout.Width(width), GUILayout.Height(height));
                        GUILayout.Space(-width);
                        GUI.color = initialColor;
                        GUILayout.Space(Properties.Space(2));
                    }


                    foreach (Data item in items)
                        switch (item.DataType)
                        {
                            case Data.Type.EmptySpace:
                                item.Draw();
                                continue;
                            case Data.Type.Text:
                                GUILayout.BeginVertical(GUILayout.Height(height));
                                GUILayout.FlexibleSpace();
                                item.Draw();
                                GUILayout.Space(1);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndVertical();
                                continue;
                            case Data.Type.Icon:
                                GUILayout.BeginVertical(GUILayout.Height(height));
                                GUILayout.Space((height - item.Height) / 2);
                                item.Draw();
                                GUILayout.EndVertical();
                                continue;
                        }

                    if (drawBackground)
                        GUILayout.Space(Properties.Space(2));
                }
                GUILayout.EndHorizontal();

                GUI.color = initialColor;
            }

            public static List<Data> GetIcon(bool enabled, float iconSize, GUIStyle enabledIcon, GUIStyle disabledIcon, ColorName enabledIconColorName, ColorName disabledIconColorName) { return new List<Data> {new Data(enabled, iconSize, enabledIcon, disabledIcon, enabledIconColorName, disabledIconColorName)}; }

            public static List<Data> GetIconWithCounter(bool enabled, int counter, float iconSize, GUIStyle enabledIcon, GUIStyle disabledIcon, ColorName enabledIconColorName, ColorName disabledIconColorName)
            {
                var icons = new List<Data>();
                if (counter > 0) icons.Add(new Data(counter.ToString(), enabledIconColorName));
                icons.Add(new Data(enabled, iconSize, enabledIcon, disabledIcon, enabledIconColorName, disabledIconColorName));
                return icons;
            }

            [Serializable]
            public class Data
            {
                public enum Type
                {
                    EmptySpace,
                    Text,
                    Icon
                }

                private readonly Type m_dataType;
                private readonly bool m_enabled;
                private readonly float m_spacePixels;
                private readonly Vector2 m_iconSize;
                private readonly Vector2 m_textSize;
                private readonly string m_text;
                private readonly GUIStyle m_enabledStyle;
                private readonly GUIStyle m_disabledStyle;
                private readonly ColorName m_enabledColorName;
                private readonly ColorName m_disabledColorName;

                public Type DataType { get { return m_dataType; } }

                public float Width
                {
                    get
                    {
                        switch (m_dataType)
                        {
                            case Type.EmptySpace: return m_spacePixels;
                            case Type.Text:       return m_textSize.x;
                            case Type.Icon:       return m_iconSize.x;
                            default:              throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                public float Height
                {
                    get
                    {
                        switch (m_dataType)
                        {
                            case Type.EmptySpace: return 0;
                            case Type.Text:       return m_textSize.y;
                            case Type.Icon:       return m_iconSize.y;
                            default:              throw new ArgumentOutOfRangeException();
                        }
                    }
                }

                public Data(float spacePixels)
                {
                    m_dataType = Type.EmptySpace;
                    m_spacePixels = spacePixels;
                    m_enabled = true;
                }

                public Data(string text, ColorName enabledColorName)
                {
                    m_dataType = Type.Text;
                    m_text = text;
                    m_enabled = true;
                    m_enabledColorName = enabledColorName;
                    m_textSize = Label.Style(Size.S).CalcSize(new GUIContent(text));
                }

                public Data(bool enabled, string text, ColorName enabledColorName, ColorName disabledColorName)
                {
                    m_dataType = Type.Text;
                    m_enabled = enabled;
                    m_text = text;
                    m_enabledColorName = enabledColorName;
                    m_disabledColorName = disabledColorName;
                    m_textSize = Label.Style(Size.S).CalcSize(new GUIContent(text));
                }

                public Data(float iconSize, GUIStyle enabledStyle, ColorName enabledColorName)
                {
                    m_dataType = Type.Icon;
                    m_iconSize = Vector2.one * iconSize;
                    m_enabled = true;
                    m_enabledStyle = enabledStyle;
                    m_enabledColorName = enabledColorName;
                }

                public Data(bool enabled, float iconSize, GUIStyle enabledStyle, GUIStyle disabledStyle, ColorName enabledColorName, ColorName disabledColorName)
                {
                    m_dataType = Type.Icon;
                    m_enabled = enabled;
                    m_iconSize = Vector2.one * iconSize;
                    m_enabledStyle = enabledStyle;
                    m_disabledStyle = disabledStyle;
                    m_enabledColorName = enabledColorName;
                    m_disabledColorName = disabledColorName;
                }

                public void Draw()
                {
                    Color initialColor = GUI.color;
                    Color iconColor = m_enabled
                        ? Colors.IconColor(m_enabledColorName)
                        : Colors.GetDColor(m_disabledColorName).Normal.WithAlpha(0.4f); //ToDo: make color template with these settings

                    GUILayout.BeginHorizontal();
                    {
                        switch (m_dataType)
                        {
                            case Type.EmptySpace:
                                GUILayout.Space(m_spacePixels);
                                break;
                            case Type.Text:
                                GUILayout.Label(m_text, new GUIStyle {normal = {textColor = iconColor}, fontSize = (int) m_iconSize.y}, GUILayout.Width(m_textSize.x));
                                GUILayout.Space(Properties.Space());
                                break;
                            case Type.Icon:
                                GUI.color = iconColor;
                                GUILayout.Label(GUIContent.none, m_enabled ? m_enabledStyle : m_disabledStyle, GUILayout.Width(m_iconSize.x), GUILayout.Height(m_iconSize.y));
                                GUI.color = initialColor;
                                break;
                            default: throw new ArgumentOutOfRangeException();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}