// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private static Texture[] s_doozyToolbarHeader;
        private static Texture[] DoozyToolbarHeader { get { return s_doozyToolbarHeader ?? (s_doozyToolbarHeader = DGUI.AnimatedTexture.GetAnimatedTextureArray("DoozyToolbarHeader", DoozyPath.EDITOR_IMAGES_PATH)); } }

        private static float LeftToolbarVerticalSectionSpacing { get { return DGUI.Properties.Space(8); } }

        private void DrawLeftToolbar()
        {
            GUILayout.BeginVertical(GUILayout.Height(position.height));
            {
                DGUI.AnimatedTexture.Draw(MainToolbarAnimBool, DoozyToolbarHeader, DoozyWindowSettings.Instance.ToolbarExpandedWidth, DoozyWindowSettings.Instance.ToolbarHeaderHeight); //Doozy Toolbar Header

                DrawToolbarExpandCollapseButton();
                GUILayout.Space(LeftToolbarVerticalSectionSpacing / 2);
                DrawToolbarButton(View.General, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonGeneral), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonGeneralSelected));
                GUILayout.Space(LeftToolbarVerticalSectionSpacing);
                DrawToolbarButton(View.Canvases, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonCanvases), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonCanvasesSelected));
                DrawToolbarButton(View.Views, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonViews), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonViewsSelected));
                DrawToolbarButton(View.Buttons, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonButtons), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonButtonsSelected));
                DrawToolbarButton(View.Drawers, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonDrawers), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonDrawersSelected));
                DrawToolbarButton(View.Popups, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonPopups), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonPopupsSelected));
                GUILayout.Space(LeftToolbarVerticalSectionSpacing);
                DrawToolbarButton(View.Soundy, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonSoundy), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonSoundySelected));
                DrawToolbarButton(View.Touchy, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonTouchy), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonTouchySelected));
                DrawToolbarButton(View.Nody, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonNody), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonNodySelected));
                GUILayout.Space(DGUI.Properties.Space(8));
//                DrawToolbarButton(View.Animations, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonAnimations), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonAnimationsSelected));
//                DrawToolbarButton(View.Templates, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonTemplates), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonTemplatesSelected));
                DrawToolbarButton(View.Themes, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonThemes), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonThemesSelected));
                GUILayout.Space(LeftToolbarVerticalSectionSpacing);
                DrawToolbarButton(View.Settings, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonSettings), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonSettingsSelected));
                DrawToolbarButton(View.Debug, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonDebug), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonDebugSelected));
//                DrawToolbarButton(View.Keys, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonKeys), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonKeysSelected));
                GUILayout.Space(LeftToolbarVerticalSectionSpacing);
                DrawToolbarButton(View.Help, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonHelp), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonHelpSelected));
                DrawToolbarButton(View.About, Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonAbout), Styles.GetStyle(Styles.StyleName.DoozyWindowToolbarButtonAboutSelected));

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        private void DrawToolbarSaveButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(LeftToolbarVerticalSectionSpacing);
            if (DGUI.Button.Draw(UILabels.Save, ColorName.LightGreen, ColorName.LightGreen, m_needsSave, DoozyWindowSettings.Instance.ToolbarButtonHeight))
                Save();
            GUILayout.Space(LeftToolbarVerticalSectionSpacing);
            GUILayout.EndHorizontal();
        }

        private void DrawToolbarButton(View view, GUIStyle normalStyle, GUIStyle selectedStyle, Action callback = null)
        {
            if (!DGUI.Button.DrawToolbarButton(view.ToString(),
                                               normalStyle,
                                               selectedStyle,
                                               CurrentView == view,
                                               CurrentView == view
                                                   ? GetViewColorName(view)
                                                   : DGUI.Utility.IsProSkin
                                                       ? ColorName.UnityLight
                                                       : ColorName.Gray,
                                               GUILayout.Height(DoozyWindowSettings.Instance.ToolbarButtonHeight))) return;
            SetView(view);
            if (callback != null) callback.Invoke();
        }

        private static void DrawToolbarButton(string text, GUIStyle normalStyle, GUIStyle selectedStyle, ColorName colorName, bool selected, Action callback)
        {
            if (!DGUI.Button.DrawToolbarButton(text, normalStyle, selectedStyle, selected, colorName, GUILayout.Height(DoozyWindowSettings.Instance.ToolbarButtonHeight))) return;
            if (callback != null) callback.Invoke();
        }

        private void DrawToolbarExpandCollapseButton(ColorName colorName = ColorName.White)
        {
            Color initialColor = GUI.color;
            GUI.color = DGUI.Colors.GetDColor(colorName).Normal.WithAlpha(GUI.color.a);
            GUILayout.BeginHorizontal(GUILayout.Width(MainToolbarWidth));
            GUILayout.Space(6 * (1 - MainToolbarAnimBool.faded));
            bool result = GUILayout.Button(GUIContent.none,
                                           Styles.GetStyle(MainToolbarAnimBool.target
                                                               ? Styles.StyleName.ArrowButtonToLeft
                                                               : Styles.StyleName.ArrowButtonToRight),
                                           GUILayout.Height(DoozyWindowSettings.Instance.ToolbarExpandCollapseButtonHeight));
            GUILayout.Space(2 * MainToolbarAnimBool.faded);
            GUILayout.EndHorizontal();
            GUI.color = initialColor;

            MainToolbarAnimBool.target = DoozyWindowSettings.Instance.DynamicToolbarExpanded;
            if (!result) return;
            ToggleToolbarMenuExpandOrCollapse();
        }

        private void ToggleToolbarMenuExpandOrCollapse()
        {
            DoozyWindowSettings.Instance.DynamicToolbarExpanded = !MainToolbarAnimBool.target;
            DoozyWindowSettings.Instance.SetDirty(false);
        }
    }
}