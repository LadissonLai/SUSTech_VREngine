// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Settings;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void DrawViewAbout()
        {
            if (CurrentView != View.About) return;

            DrawDynamicViewVerticalSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconDoozyUILogoCompact), 128);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            DGUI.Colors.SetDisabledGUIColorAlpha();
            DGUI.Label.Draw(UILabels.Version + " " + DoozyUIVersion.Instance.Version, Size.M, DGUI.Colors.LightOrDarkColorName);
            DGUI.Colors.SetNormalGUIColorAlpha();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            DrawDynamicViewVerticalSpace(3);

            DrawAboutProduct(Styles.StyleName.IconDoozyUI, "DoozyUI", "UI Management System", DoozySettings.DOOZYUI_ABOUT, DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(2);
            DrawAboutProduct(Styles.StyleName.IconSoundy, "Soundy", "Sound Manager", DoozySettings.SOUNDY_ABOUT, DGUI.Colors.SoundyColorName);
            DrawDynamicViewVerticalSpace(2);
            DrawAboutProduct(Styles.StyleName.IconTouchy, "Touchy", "Touch Manager", DoozySettings.TOUCHY_ABOUT, DGUI.Colors.TouchyColorName);
            DrawDynamicViewVerticalSpace(2);
            DrawAboutProduct(Styles.StyleName.IconNody, "Nody", "Node Graph Engine", DoozySettings.NODY_ABOUT, DGUI.Colors.NodyColorName);

            DrawDynamicViewVerticalSpace(2);
        }

        private void DrawAboutProduct(Styles.StyleName iconStyleName, string productName, string subtitle, string about, ColorName colorName)
        {
            DGUI.WindowUtils.DrawIconTitle(iconStyleName, productName, subtitle, colorName);
            DrawDynamicViewVerticalSpace(0.5f);
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    GUI.color = DGUI.Colors.TextColor(colorName);
                    DGUI.Colors.SetDisabledGUIColorAlpha();
                    EditorGUILayout.LabelField(about, new GUIStyle(DGUI.Label.Style()) {wordWrap = true});
                    DGUI.Colors.SetNormalGUIColorAlpha();
                    GUI.color = InitialGUIColor;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
    }
}