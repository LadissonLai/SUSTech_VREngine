// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class WindowUtils
        {
            public static void DrawIconTitle(Styles.StyleName iconStyleName, string mainTitle, string subTitle, ColorName colorName)
            {
                GUILayout.BeginHorizontal();
                {
                    Icon.Draw(Styles.GetStyle(iconStyleName), Bar.Height(Size.XL), Bar.Height(Size.XL), colorName);
                    GUILayout.Space(Properties.Space(4));
                    GUILayout.BeginVertical(GUILayout.Height(Bar.Height(Size.XL)));
                    {
                        GUILayout.Space(-Properties.Space(2));
                        Label.Draw(mainTitle, Size.XL, colorName, Bar.Height(Size.M));
                        GUILayout.Space(Properties.Space());
                        Divider.Draw(Divider.Type.One, colorName);
                        GUI.color = GUI.color.WithAlpha(0.8f);
                        Label.Draw(subTitle, Size.S, colorName, Bar.Height(Size.M));
                        GUI.color = GUI.color.WithAlpha(1f);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

            public static void DrawSettingLabel(string label, ColorName colorName, float rowHeight) { DGUI.Label.Draw(label, Size.M, colorName, rowHeight); }

            public static void DrawSettingDescription(string description)
            {
                GUI.color = GUI.color.WithAlpha(Utility.IsProSkin ? 0.6f : 0.8f);
                var textStyle = new GUIStyle(Label.Style(Size.S, TextAlign.Left, Colors.DisabledTextColorName));Label.Style(Size.S, TextAlign.Left, Colors.DisabledTextColorName);
                textStyle.wordWrap = true;
                textStyle.padding = new RectOffset(0, 0, 0, 8);
                EditorGUILayout.LabelField(description, textStyle);
                Colors.SetNormalGUIColorAlpha();
            }
        }
    }
}