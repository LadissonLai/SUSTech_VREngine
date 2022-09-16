// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Settings;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private AnimBool m_anyDebugExpanded;
        private const View DEFAULT_VIEW = View.General;
        private string[] m_viewNames;

        private AnimBool CurrentViewExpanded { get { return GetAnimBool(CurrentView); } }
        private ColorName CurrentViewColorName { get { return GetViewColorName(CurrentView); } }

        public enum View
        {
            General,

            Buttons,
            Views,
            Canvases,
            Drawers,
            Popups,

            Nody,
            Soundy,
            Touchy,

            Animations,
            Templates,
            Themes,

            Settings,
            Debug,
            Keys,

            Help,
            About
        }

        private static ColorName GetViewColorName(View view)
        {
            switch (view)
            {
                case View.General: return DGUI.Colors.GeneralColorName;

                case View.Buttons:    return DGUI.Colors.UIButtonColorName;
                case View.Views:      return DGUI.Colors.UIViewColorName;
                case View.Canvases:   return DGUI.Colors.UICanvasColorName;
                case View.Drawers:    return DGUI.Colors.UIDrawerColorName;
                case View.Popups:     return DGUI.Colors.UIPopupColorName;
                case View.Nody:       return DGUI.Colors.NodyColorName;
                case View.Soundy:     return DGUI.Colors.SoundyColorName;
                case View.Touchy:     return DGUI.Colors.TouchyColorName;
                case View.Animations: return DGUI.Colors.AnimationsColorName;
                case View.Templates:  return DGUI.Colors.UITemplateColorName;
                case View.Themes:     return DGUI.Colors.ThemesColorName;
                case View.Settings:   return DGUI.Colors.SettingsColorName;
                case View.Debug:      return DGUI.Colors.DebugColorName;
                case View.Keys:       return DGUI.Colors.KeysColorName;
                case View.Help:       return DGUI.Colors.HelpColorName;
                case View.About:      return DGUI.Colors.AboutColorName;
                default:              throw new ArgumentOutOfRangeException("view", view, null);
            }
        }

        /// <summary>
        ///     Current View
        /// </summary>
        private View m_view = DEFAULT_VIEW;

        private View CurrentView
        {
            get { return m_view; }
            set
            {
                m_view = value;
                EditorPrefs.SetInt(DoozyWindowSettings.Instance.EditorPrefsKeyWindowCurrentView, (int) m_view);
            }
        }


        private float ViewContentLeftHorizontalPadding { get { return DoozyWindowSettings.Instance.WindowViewContentHorizontalPadding + m_anyDebugExpanded.faded * 8; } }
        static float ViewContentRightHorizontalPadding { get { return DoozyWindowSettings.Instance.WindowViewContentHorizontalPadding; } }

        private void DrawViews()
        {
            //update the AnimBool show variables
            foreach (View view in Enum.GetValues(typeof(View)))
                GetAnimBool(view.ToString()).target = CurrentView == view;

            DrawViewHeader();


            //draw the currently selected view
            if (CurrentView != View.Themes)
                m_viewScrollPosition = GUILayout.BeginScrollView(m_viewScrollPosition);
            {
                DrawView(DrawViewGeneral, GetAnimBool(View.General), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewViews, GetAnimBool(View.Views), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewButtons, GetAnimBool(View.Buttons), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewCanvases, GetAnimBool(View.Canvases), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewDrawers, GetAnimBool(View.Drawers), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewPopups, GetAnimBool(View.Popups), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewNody, GetAnimBool(View.Nody), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewSoundy, GetAnimBool(View.Soundy), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewTouchy, GetAnimBool(View.Touchy), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewAnimations, GetAnimBool(View.Animations), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewTemplates, GetAnimBool(View.Templates), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewSettings, GetAnimBool(View.Settings), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewDebug, GetAnimBool(View.Debug), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewKeys, GetAnimBool(View.Keys), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewHelp, GetAnimBool(View.Help), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
                DrawView(DrawViewAbout, GetAnimBool(View.About), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);
            }
            if (CurrentView != View.Themes)
                GUILayout.EndScrollView();

            DrawView(DrawViewThemes, GetAnimBool(View.Themes), ViewContentLeftHorizontalPadding, ViewContentRightHorizontalPadding);

            ListenForButtons();
        }

        private void ListenForButtons()
        {
            Event current = Event.current;
            if (EditMode.target && DGUI.KeyMapper.DetectKeyUp(current, KeyCode.Escape)) //ESC - exit Edit Mode 
            {
                EditMode.target = false;
                DGUI.Properties.ResetKeyboardFocus();
            }

            if (NewDatabase.target && DGUI.KeyMapper.DetectKeyUp(current, KeyCode.Escape)) //ESC - exit New Database Mode 
            {
                NewDatabase.target = false;
                DGUI.Properties.ResetKeyboardFocus();
            }
        }

        private AnimBool GetAnimBool(View view) { return GetAnimBool(view.ToString()); }

        private void SetView(View view)
        {
            ResetCreateNewCategory();
            ResetRenameCategory();
            ResetSearchDatabase();

            ResetCreateNewSoundDatabase();
            ResetRenameSoundDatabase();
            ResetSearchSoundyDatabase();

            if (CurrentView == View.Soundy) StopAllSounds();

            switch (view)
            {
                case View.General:
                    break;
                case View.Buttons:
                    InitViewButtons();
                    break;
                case View.Views:
                    InitViewButtons();
                    InitViewViews();
                    break;
                case View.Canvases:
                    InitViewCanvases();
                    break;
                case View.Drawers:
                    InitViewDrawers();
                    break;
                case View.Popups:
                    InitViewPopups();
                    break;
                case View.Nody:
                    break;
                case View.Soundy:
                    InitViewSoundy();
                    break;
                case View.Touchy:
                    break;
                case View.Animations:
                    break;
                case View.Templates:
                    break;
                case View.Themes:
                    m_themesVariantsScrollPosition = Vector2.zero;
                    InitViewThemes();
                    break;
                case View.Settings:
                    break;
                case View.Debug:
                    break;
                case View.Keys:
                    break;
                case View.Help:
                    break;
                case View.About:
                    break;
            }

            m_viewScrollPosition = Vector2.zero;
            m_viewLeftMenuScrollPosition = Vector2.zero;

            CurrentView = view;

            EditMode.value = false;
            NewDatabase.value = false;
        }

        private void DrawViewHeader()
        {
            GUIStyle headerStyle;
            switch (CurrentView)
            {
                case View.General:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderGeneral);
                    break;
                case View.Buttons:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderButtons);
                    break;
                case View.Views:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderViews);
                    break;
                case View.Canvases:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderCanvases);
                    break;
                case View.Drawers:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderDrawers);
                    break;
                case View.Popups:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderPopups);
                    break;
                case View.Nody:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderNody);
                    break;
                case View.Soundy:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderSoundy);
                    break;
                case View.Touchy:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderTouchy);
                    break;
                case View.Animations:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderAnimations);
                    break;
                case View.Templates:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderTemplates);
                    break;
                case View.Themes:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderThemes);
                    break;
                case View.Debug:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderDebug);
                    break;
                case View.Settings:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderSettings);
                    break;
                case View.Keys:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderKeys);
                    break;
                case View.Help:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderHelp);
                    break;
                case View.About:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderAbout);
                    break;
                default:
                    headerStyle = Styles.GetStyle(Styles.StyleName.DoozyWindowViewHeaderGeneral);
                    break;
            }

            GUILayout.Label(GUIContent.none, headerStyle, GUILayout.ExpandWidth(true));
            DrawDynamicViewVerticalSpace();
        }

        private float DynamicViewVerticalSpace(float multiplier = 1) { return DGUI.Properties.Space(8) * multiplier * CurrentViewExpanded.faded; }
        private void DrawDynamicViewVerticalSpace(float multiplier = 1) { GUILayout.Space(DynamicViewVerticalSpace(multiplier)); }
        private void DrawViewSearchHorizontalPadding() { GUILayout.Space(DGUI.Properties.Space(7)); }
        private void DrawViewHorizontalPadding() { GUILayout.Space(DGUI.Properties.Space(2)); }

        private void DrawSearchIconAndText()
        {
            DGUI.Icon.Draw(SearchIconStyle, SearchRowHeight * 0.8f, SearchRowHeight, CurrentViewColorName); //draw search icon
            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Label.Draw(UILabels.Search, Size.M, CurrentViewColorName, SearchRowHeight); //draw search label
        }
    }
}