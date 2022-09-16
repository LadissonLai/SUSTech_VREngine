// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

#if dUI_TextMeshPro
using TMPro;
#endif

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        #region Constants

        private const int COLOR_TAB_INDEX = 0;
        private const int SPRITE_TAB_INDEX = 1;
        private const int TEXTURE_TAB_INDEX = 2;
        private const int FONT_TAB_INDEX = 3;
        private const int FONT_ASSET_TAB_INDEX = 4;

        #endregion

        #region View Variables

        private static ThemesDatabase ThemesDatabase { get { return ThemesSettings.Database; } }

        private ThemeData m_currentThemeData;

        private ThemeData CurrentThemeData
        {
            get
            {
                if (m_currentThemeData != null) return m_currentThemeData;
                m_currentThemeData = ThemesDatabase.GetThemeData(m_currentThemeId);
                return m_currentThemeData;
            }
            set { m_currentThemeData = value; }
        }

        private Guid m_currentThemeId;

        public Guid CurrentThemeId
        {
            get { return m_currentThemeId; }
            set
            {
                m_currentThemeId = value;
                m_currentThemeData = ThemesDatabase.GetThemeData(m_currentThemeId);
                m_currentThemeData.Init(false, false);
                EditorPrefs.SetString(DoozyWindowSettings.Instance.EditorPrefsKeyWindowCurrentTheme, m_currentThemeId.ToString());
            }
        }

        private int m_selectedThemeTab = 0;

        private DGUI.Toolbar.ToolbarButton m_themeColorTab, m_themeImageTab, m_themeSoundTab, m_themeFontTab, m_themeFontAssetTab;

        private DGUI.Toolbar.ToolbarButton ThemeColorTab
        {
            get
            {
                if (m_themeColorTab != null) return m_themeColorTab;
                m_themeColorTab = new DGUI.Toolbar.ToolbarButton(UILabels.Color, Styles.GetStyle(Styles.StyleName.IconFaPalette), DGUI.Colors.DisabledIconColorName, DGUI.Colors.ThemesColorName);
                return m_themeColorTab;
            }
        }

        private DGUI.Toolbar.ToolbarButton ThemeSpriteTab
        {
            get
            {
                if (m_themeImageTab != null) return m_themeImageTab;
                m_themeImageTab = new DGUI.Toolbar.ToolbarButton(UILabels.Sprite, Styles.GetStyle(Styles.StyleName.IconSprite), DGUI.Colors.DisabledIconColorName, DGUI.Colors.ThemesColorName);
                return m_themeImageTab;
            }
        }

        private DGUI.Toolbar.ToolbarButton ThemeTextureTab
        {
            get
            {
                if (m_themeSoundTab != null) return m_themeSoundTab;
                m_themeSoundTab = new DGUI.Toolbar.ToolbarButton(UILabels.Texture, Styles.GetStyle(Styles.StyleName.IconTexture), DGUI.Colors.DisabledIconColorName, DGUI.Colors.ThemesColorName);
                return m_themeSoundTab;
            }
        }

        private DGUI.Toolbar.ToolbarButton ThemeFontTab
        {
            get
            {
                if (m_themeFontTab != null) return m_themeFontTab;
                m_themeFontTab = new DGUI.Toolbar.ToolbarButton(UILabels.Font, Styles.GetStyle(Styles.StyleName.IconFont), DGUI.Colors.DisabledIconColorName, DGUI.Colors.ThemesColorName);
                return m_themeFontTab;
            }
        }

        private DGUI.Toolbar.ToolbarButton ThemeFontAssetTab
        {
            get
            {
                if (m_themeFontAssetTab != null) return m_themeFontAssetTab;
                m_themeFontAssetTab = new DGUI.Toolbar.ToolbarButton(UILabels.FontAsset, Styles.GetStyle(Styles.StyleName.IconFontAsset), DGUI.Colors.DisabledIconColorName, DGUI.Colors.ThemesColorName);
                return m_themeFontAssetTab;
            }
        }

        private GUIStyle m_themeNameStyle;

        public GUIStyle ThemeNameStyle
        {
            get
            {
                if (m_themeNameStyle != null) return m_themeNameStyle;
                m_themeNameStyle = new GUIStyle(Styles.GetStyle(Styles.StyleName.LabelXLCenter))
                                   {
                                       fontSize = 18
                                   };
                return m_themeNameStyle;
            }
        }

        private float m_themeHeaderHeight = 96f;
        private float m_themeVariantButtonHeight = 24f;
        private float m_themePropertyColumnWidth = 160f;
        private float m_themeVariantColumnWidth = 160f;
        private float ThemeVariantImageSize { get { return m_themeVariantColumnWidth * 0.6f; } }
        private Vector2 m_themesVariantsScrollPosition;

        private Vector2 m_themeVariantScrollPosition;

        private Rect ThemesHeaderAreaRect
        {
            get
            {
                Rect viewWithMenusRect = ViewWithMenusRect;
                return new Rect(viewWithMenusRect.x,
                                viewWithMenusRect.y,
                                viewWithMenusRect.width,
                                m_themeHeaderHeight);
            }
        }

        private Rect ThemesPropertiesAreaRect
        {
            get
            {
                Rect viewWithMenusRect = ViewWithMenusRect;
                return new Rect(viewWithMenusRect.x,
                                viewWithMenusRect.y + m_themeHeaderHeight,
                                m_themePropertyColumnWidth,
                                viewWithMenusRect.height - m_themeHeaderHeight);
            }
        }

        private Rect ThemesVariantsAreaRect
        {
            get
            {
                Rect themesPropertiesAreaRect = ThemesPropertiesAreaRect;
                return new Rect(themesPropertiesAreaRect.xMax,
                                themesPropertiesAreaRect.y,
                                ViewWithMenusRect.width - m_themePropertyColumnWidth,
                                themesPropertiesAreaRect.height);
            }
        }

        private bool m_refreshThemes;

        #endregion

        private void InitViewThemes()
        {
            ThemesDatabase.InitializeThemes();

            bool renamedThemeData = false;
            foreach (ThemeData themeData in ThemesDatabase.Themes)
            {
                if (themeData == null) continue;
                if (themeData.name == ThemesDatabase.GetThemeDataFilename(themeData.ThemeName)) continue;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(themeData), ThemesDatabase.GetThemeDataFilename(themeData.ThemeName));
                themeData.SetDirty(false);
                renamedThemeData = true;
            }

            if (EditorPrefs.HasKey(DoozyWindowSettings.Instance.EditorPrefsKeyWindowCurrentTheme))
            {
                m_currentThemeId = new Guid(EditorPrefs.GetString(DoozyWindowSettings.Instance.EditorPrefsKeyWindowCurrentTheme));
                if (!ThemesDatabase.Contains(m_currentThemeId))
                    CurrentThemeId = ThemesDatabase.GetThemeData(ThemesDatabase.GENERAL_THEME_NAME).Id;
            }
            else
            {
                ThemeData generalTheme = ThemesDatabase.GetThemeData(ThemesDatabase.GENERAL_THEME_NAME);
                CurrentThemeId = generalTheme.Id;
            }

            if (renamedThemeData) Save();
        }

        private void DrawViewThemes()
        {
            if (CurrentView != View.Themes) return;

            DrawViewThemesTopMenu();
            DrawViewThemesLeftMenu();
            DrawViewThemesView();

            if (!m_refreshThemes && EditMode.target)
                m_refreshThemes = true;

            if (m_refreshThemes && !EditMode.target)
            {
                m_refreshThemes = false;
                InitViewThemes();
            }
        }

        private void DrawViewThemesTopMenu()
        {
            BeginDrawViewTopMenuArea();
            {
                GUILayout.Space((m_viewTopMenuHeight - BarHeight) / 2 - m_viewTopMenuHeight * (1 - CurrentViewExpanded.faded));

                GUILayout.BeginHorizontal();
                {
                    DrawDefaultViewHorizontalSpacing();
                    {
                        GUILayout.Space((m_viewTopMenuHeight - BarHeight) / 3);

                        //New Database
                        GUI.enabled = !NewDatabase.target && !EditMode.target;
                        {
                            if (ButtonNew(NewDatabase.target, UILabels.NewTheme))
                            {
                                NewDatabase.target = !NewDatabase.target;
                                if (NewDatabase.target)
                                {
                                    DoozyUtils.SaveAssets();
                                    m_newDatabaseName = string.Empty;
                                }
                            }
                        }
                        GUI.enabled = true;

                        DrawDefaultViewHorizontalSpacing();

                        if (NewDatabase.target)
                        {
                            GUILayout.Space(DGUI.Properties.Space());
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Space(DGUI.Properties.Space(2));

                                GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)));
                                {
                                    GUI.color = DGUI.Colors.GetDColor(ColorName.Green).Normal.WithAlpha(GUI.color.a);
                                    DGUI.Properties.SetNextControlName("m_newDatabaseName");
                                    m_newDatabaseName = EditorGUILayout.TextField(GUIContent.none, m_newDatabaseName, GUILayout.Width(NewCategoryNameTextFieldWidth));
                                    DGUI.Properties.FocusTextInControl("m_newDatabaseName");
                                    GUI.color = InitialGUIColor;

                                    if (DGUI.KeyMapper.DetectKeyUp(Event.current, KeyCode.Return) ||
                                        DGUI.KeyMapper.DetectKeyUp(Event.current, KeyCode.KeypadEnter) ||
                                        DGUI.Button.IconButton.Ok()) //OK - New Database
                                    {
                                        DGUI.Properties.ResetKeyboardFocus();
                                        NewDatabase.target = false;
                                        if (ThemesDatabase.CreateTheme(m_newDatabaseName, true, true))
                                        {
                                            CurrentThemeId = ThemesDatabase.GetThemeData(m_newDatabaseName).Id;
                                            ThemesDatabase.Sort(false);
                                            ThemesDatabase.UpdateThemesNames(true);
                                        }
                                    }

                                    if (DGUI.Button.IconButton.Cancel()) //CANCEL - New Database
                                    {
                                        DGUI.Properties.ResetKeyboardFocus();
                                        NewDatabase.target = false;
                                    }

                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();
                            }
                            GUILayout.EndVertical();
                        }
                        else
                        {
                            bool enabled = GUI.enabled;
                            GUI.enabled = !EditMode.target;
                            {
                                if (ButtonSortDatabase()) ThemesDatabase.Sort(true, true); //Sort Database
                                DrawDefaultViewHorizontalSpacing();
                                if (ButtonRefreshDatabase()) ThemesDatabase.RefreshDatabase(false, true); //Refresh Database
                                DrawDefaultViewHorizontalSpacing();
                                if (ButtonSaveDatabase()) ThemesDatabase.SetDirty(true); //Save Database
                                DrawDefaultViewHorizontalSpacing();
                                if (ButtonSearchFor(UILabels.SearchForThemes)) ThemesDatabase.SearchForUnregisteredThemes(true); //Search for themes
                                GUILayout.FlexibleSpace();
                                DrawDefaultViewHorizontalSpacing();
                                if (ButtonAutoSave(ThemesSettings.Instance.AutoSave))
                                {
                                    if (DoozyUtils.DisplayDialog(UILabels.AutoSave, 
                                                                 (!ThemesSettings.Instance.AutoSave 
                                                                      ? UILabels.ThemesEnableAutoSave
                                                                      : UILabels.ThemesDisableAutoSave),
                                                                  UILabels.Ok,
                                                                  UILabels.Cancel))
                                    {
                                        ThemesSettings.Instance.UndoRecord(UILabels.SetValue);
                                        ThemesSettings.Instance.AutoSave = !ThemesSettings.Instance.AutoSave;
                                        ThemesSettings.Instance.SetDirty(true);
                                        DoozyUtils.DisplayDialog(ThemesSettings.Instance.AutoSave
                                                                     ? UILabels.AutoSaveEnabled
                                                                     : UILabels.AutoSaveDisabled,
                                                                 ThemesSettings.Instance.AutoSave
                                                                     ? UILabels.ThemesAutoSaveEnabled
                                                                     : UILabels.ThemesAutoSaveDisabled,
                                                                 UILabels.Ok);
                                    }
                                }

                                DrawDefaultViewHorizontalSpacing();
                                if (ButtonResetDatabase()) ThemesDatabase.ResetDatabase(); //Reset Database
                            }
                            GUI.enabled = enabled;
                            DrawDefaultViewHorizontalSpacing();
                            if (ButtonEditMode(EditMode.target)) EditMode.target = !EditMode.target; //Edit Mode
                        }
                    }

                    GUILayout.Space((m_viewTopMenuHeight - BarHeight) / 3);
                }
                GUILayout.EndHorizontal();
            }
            EndDrawViewTopMenuArea();
        }

        private void DrawViewThemesLeftMenu()
        {
            ThemeData deleteTheme = null;

            BeginDrawViewLeftMenuArea();
            {
                ColorName viewColorName = CurrentViewColorName;
                GUI.enabled = !NewDatabase.target;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Properties.Space(2));
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(DGUI.Properties.Space(8) * CurrentViewExpanded.faded);
                        foreach (ThemeData theme in ThemesDatabase.Themes)
                        {
                            if (theme == null) continue;
                            bool isSelected = GUI.enabled && CurrentThemeId == theme.Id;
                            string themeName = theme.ThemeName;
                            int maxLength = 14 + (int) (3 * (1 - EditMode.faded));
                            if (themeName.Length > maxLength) themeName = themeName.Substring(0, maxLength - 1) + "...";
                            GUILayout.BeginHorizontal();
                            {
                                if (EditMode.faded > 0.01f)
                                {
                                    GUILayout.Space(DGUI.Properties.Space(1 - EditMode.faded));

                                    bool enabled = GUI.enabled;
                                    GUI.enabled = !theme.ThemeName.Equals(ThemesDatabase.GENERAL_THEME_NAME);
                                    if (DGUI.Button.IconButton.Minus(BarHeight)) deleteTheme = theme;
                                    GUI.enabled = enabled;

                                    GUILayout.Space(DGUI.Properties.Space(2 * (1 - EditMode.faded)));
                                }

                                if (DGUI.Button.Draw(themeName, Size.M, TextAlign.Left,
                                                     isSelected ? viewColorName : DGUI.Colors.DisabledBackgroundColorName,
                                                     isSelected ? viewColorName : DGUI.Colors.DisabledTextColorName,
                                                     GUI.enabled && CurrentThemeId == theme.Id, BarHeight))
                                    CurrentThemeId = theme.Id;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(DGUI.Properties.Space(2) * CurrentViewExpanded.faded);
                        }

                        GUILayout.Space(DGUI.Properties.Space(2) * CurrentViewExpanded.faded);
                    }
                    GUILayout.EndVertical();
                    GUILayout.Space(DGUI.Properties.Space(2));
                }
                GUILayout.EndHorizontal();
                GUI.enabled = true;
            }
            EndDrawViewLeftMenuArea();

            if (deleteTheme == null) return;
            if (ThemesDatabase.DeleteThemeData(deleteTheme))
                InitViewThemes();
        }

        private void DrawViewThemesView()
        {
            ThemeData themeData = CurrentThemeData;
            if (themeData == null) return;

            DrawViewThemesHeader(themeData);
            DrawViewThemesProperties(themeData);
            DrawViewThemesVariants(themeData);
        }

        private void DrawViewThemesHeader(ThemeData themeData)
        {
            GUILayout.BeginArea(ThemesHeaderAreaRect);
            {
                #region Theme Name

                GUILayout.Space(DGUI.Properties.Space(8) * CurrentViewExpanded.faded);
                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    if (EditMode.faded > 0.01f)
                    {
                        string themeName = themeData.ThemeName;
                        GUI.enabled = !themeData.IsGeneralTheme;
                        {
                            EditorGUI.BeginChangeCheck();
                            themeName = EditorGUILayout.TextField(themeName);
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (!ThemesDatabase.Contains(themeName) && !ThemesDatabase.Contains(themeName.Trim()))
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    themeData.ThemeName = themeName;
                                    themeData.SetDirty(false);
                                }
                            }
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        DGUI.Label.Draw(themeData.ThemeName, ThemeNameStyle, DGUI.Colors.DisabledTextColorName);
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                if (EditMode.faded > 0.01f) GUILayout.Space(3f);
                GUILayout.Space(DGUI.Properties.Space(6) * CurrentViewExpanded.faded);

                #endregion

                #region Tabs - Color, Image, Sound...

                GUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    var toolbarButtons = new List<DGUI.Toolbar.ToolbarButton>
                                         {
                                             ThemeColorTab,
                                             ThemeSpriteTab,
                                             ThemeTextureTab,
                                             ThemeFontTab
                                         };

#if dUI_TextMeshPro
                    toolbarButtons.Add(ThemeFontAssetTab);
#endif

                    m_selectedThemeTab = DGUI.Toolbar.Draw(m_selectedThemeTab,
                                                           toolbarButtons,
                                                           DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(8));
                }
                GUILayout.EndHorizontal();

                if (EditMode.faded > 0.01f) GUILayout.Space(-4f);
                GUILayout.Space(DGUI.Properties.Space(8) * CurrentViewExpanded.faded);

                #endregion
            }
            GUILayout.EndArea();
        }

        #region Properties Area

        private void DrawViewThemesProperties(ThemeData themeData)
        {
            GUILayout.BeginArea(ThemesPropertiesAreaRect);
            GUILayout.BeginScrollView(new Vector2(0, m_themesVariantsScrollPosition.y), false, false, GUIStyle.none, GUIStyle.none);
            {
                GUILayout.Space(m_themeVariantButtonHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawViewThemesViewProperties(themeData, m_selectedThemeTab);
                GUILayout.Space(DGUI.Properties.SingleLineHeight * 2 + GUI.skin.horizontalScrollbar.fixedHeight);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawAddPropertyButton(string buttonName, Action addPropertyAction)
        {
            if (DGUI.FadeOut.Begin(EditMode, false))
            {
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaPlus),
                                                           buttonName, Size.M, TextAlign.Left,
                                                           DGUI.Colors.DisabledBackgroundColorName,
                                                           CurrentViewColorName,
                                                           DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                           false))
                    {
                        addPropertyAction.Invoke();
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
            }

            DGUI.FadeOut.End(EditMode);
        }

        private float GetThemePropertyRowHeight(int selectedThemeTabIndex)
        {
            switch (selectedThemeTabIndex)
            {
                case COLOR_TAB_INDEX:   return DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
                case SPRITE_TAB_INDEX:  return ThemeVariantImageSize;
                case TEXTURE_TAB_INDEX: return ThemeVariantImageSize;
                case FONT_TAB_INDEX:    return DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
#if dUI_TextMeshPro
                case FONT_ASSET_TAB_INDEX: return DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
#endif
                default: return DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
            }
        }

        private void DrawViewThemesViewProperties(ThemeData themeData, int selectedThemeTabIndex)
        {
#if !dUI_TextMeshPro
            if (selectedThemeTabIndex == FONT_ASSET_TAB_INDEX)
            {
                selectedThemeTabIndex = COLOR_TAB_INDEX;
                return;
            }
#endif

            Guid deleteGuid = Guid.Empty;
            List<LabelId> propertyList = new List<LabelId>();
            string addPropertyButtonName = "+";
            Action addPropertyButtonAction = () => { };

            switch (selectedThemeTabIndex)
            {
                case COLOR_TAB_INDEX:
                    propertyList = themeData.ColorLabels;
                    addPropertyButtonName = UILabels.NewColor;
                    addPropertyButtonAction = () => { themeData.AddColorProperty(true); };
                    break;
                case SPRITE_TAB_INDEX:
                    propertyList = themeData.SpriteLabels;
                    addPropertyButtonName = UILabels.NewSprite;
                    addPropertyButtonAction = () => { themeData.AddSpriteProperty(true); };
                    break;
                case TEXTURE_TAB_INDEX:
                    propertyList = themeData.TextureLabels;
                    addPropertyButtonName = UILabels.NewTexture;
                    addPropertyButtonAction = () => { themeData.AddTextureProperty(true); };
                    break;
                case FONT_TAB_INDEX:
                    propertyList = themeData.FontLabels;
                    addPropertyButtonName = UILabels.NewFont;
                    addPropertyButtonAction = () => { themeData.AddFontProperty(true); };
                    break;
#if dUI_TextMeshPro
                case FONT_ASSET_TAB_INDEX:
                    propertyList = themeData.FontAssetLabels;
                    addPropertyButtonName = UILabels.NewFontAsset;
                    addPropertyButtonAction = () => { themeData.AddFontAssetProperty(true); };
                    break;
#endif
            }


            GUILayout.Space(DGUI.Properties.Space(2));
            DrawHorizontalDivider();
            GUILayout.Space(DGUI.Properties.Space());

            for (int propertyIndex = 0; propertyIndex < propertyList.Count; propertyIndex++)
            {
                GUILayout.BeginVertical(GUILayout.Height(GetThemePropertyRowHeight(selectedThemeTabIndex)));
                {
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal(GUILayout.Width(m_themePropertyColumnWidth));
                    {
                        if (EditMode.faded > 0.01f)
                        {
                            GUILayout.Space(DGUI.Properties.Space(1 - EditMode.faded));

                            if (DGUI.Button.IconButton.Minus())
                                deleteGuid = propertyList[propertyIndex].Id;

                            GUILayout.Space(DGUI.Properties.Space(2 * (1 - EditMode.faded)));

                            EditorGUI.BeginChangeCheck();
                            string label = EditorGUILayout.TextField(propertyList[propertyIndex].Label,
                                                                     GUILayout.Width(m_themePropertyColumnWidth - DGUI.Properties.Space(4) - DGUI.Sizes.ICON_BUTTON_SIZE));
                            if (EditorGUI.EndChangeCheck())
                            {
                                themeData.UndoRecord(UILabels.UpdateValue);
                                propertyList[propertyIndex] = new LabelId(propertyList[propertyIndex].Id, label);
                                themeData.SetDirty(false);
                            }
                        }
                        else
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themePropertyColumnWidth));
                            {
                                GUILayout.Space(DGUI.Properties.Space(2));
                                DGUI.Label.Draw(propertyList[propertyIndex].Label, Size.L, TextAlign.Left, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.Space(DGUI.Properties.Space());
                    DrawHorizontalDivider();
                    GUILayout.Space(DGUI.Properties.Space());
                }
                GUILayout.EndVertical();
            }

            DrawAddPropertyButton(addPropertyButtonName, addPropertyButtonAction);

            if (deleteGuid == Guid.Empty) return;
            switch (selectedThemeTabIndex)
            {
                case COLOR_TAB_INDEX:
                    themeData.RemoveColorProperty(deleteGuid, true, false);
                    break;
                case SPRITE_TAB_INDEX:
                    themeData.RemoveSpriteProperty(deleteGuid, true, false);
                    break;
                case TEXTURE_TAB_INDEX:
                    themeData.RemoveTextureProperty(deleteGuid, true, false);
                    break;
                case FONT_TAB_INDEX:
                    themeData.RemoveFontProperty(deleteGuid, true, false);
                    break;
#if dUI_TextMeshPro
                case FONT_ASSET_TAB_INDEX:
                    themeData.RemoveFontAssetProperty(deleteGuid, true, false);
                    break;
#endif
            }
        }

        #endregion

        #region Variants Area

        private void DrawViewThemesVariants(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;

            GUILayout.BeginArea(ThemesVariantsAreaRect);
            m_themesVariantsScrollPosition = GUILayout.BeginScrollView(m_themesVariantsScrollPosition);
            {
                switch (m_selectedThemeTab)
                {
                    case COLOR_TAB_INDEX:
                        deleteVariant = DrawViewThemesViewVariantsColor(themeData);
                        break;
                    case SPRITE_TAB_INDEX:
                        deleteVariant = DrawViewThemesViewVariantsSprite(themeData);
                        break;
                    case TEXTURE_TAB_INDEX:
                        deleteVariant = DrawViewThemesViewVariantsTexture(themeData);
                        break;
                    case FONT_TAB_INDEX:
                        deleteVariant = DrawViewThemesViewVariantsFont(themeData);
                        break;
#if dUI_TextMeshPro
                    case FONT_ASSET_TAB_INDEX:
                        deleteVariant = DrawViewThemesViewVariantsFontAsset(themeData);
                        break;
#endif
                }

                GUILayout.Space(DGUI.Properties.SingleLineHeight * 2);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if (deleteVariant != null) themeData.RemoveVariant(deleteVariant, true, true, true);
        }

        private ThemeVariantData DrawThemeVariantButton(ThemeData themeData, ThemeVariantData variant)
        {
            ThemeVariantData deleteVariant = null;

            if (EditMode.faded > 0.01f)
            {
                string variantName = variant.VariantName;
                GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                {
                    GUILayout.Space(DGUI.Properties.Space(1 - EditMode.faded));

                    if (DGUI.Button.IconButton.Minus()) deleteVariant = variant;

                    GUILayout.Space(DGUI.Properties.Space(2 * (1 - EditMode.faded)));

                    EditorGUI.BeginChangeCheck();
                    variantName = EditorGUILayout.TextField(variantName);
                }
                GUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    themeData.UndoRecord(UILabels.UpdateValue);
                    variant.VariantName = variantName;
                    themeData.SetDirty(false);
                }
            }
            else
            {
                bool activeVariant = themeData.ActiveVariant.Id.Equals(variant.Id);
                if (DGUI.Button.Draw(variant.VariantName,
                                     activeVariant ? CurrentViewColorName : DGUI.Colors.DisabledBackgroundColorName,
                                     activeVariant ? CurrentViewColorName : DGUI.Colors.DisabledTextColorName,
                                     activeVariant,
                                     m_themeVariantButtonHeight,
                                     m_themeVariantColumnWidth))
                {
                    themeData.ActivateVariant(variant);
                    themeData.SetDirty(false);
                    UpdateAllThemeTargets(themeData);
                }
            }


            if (EditMode.faded > 0.01f) GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawHorizontalDivider();
            GUILayout.Space(DGUI.Properties.Space());

            return deleteVariant;
        }

        private void DrawThemeAddVariantButton(ThemeData themeData)
        {
            if (DGUI.FadeOut.Begin(EditMode, false))
            {
                GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaPlus),
                                                           UILabels.NewThemeVariant, Size.M, TextAlign.Left,
                                                           DGUI.Colors.DisabledBackgroundColorName,
                                                           CurrentViewColorName,
                                                           m_themeVariantButtonHeight,
                                                           false))
                    {
                        themeData.AddVariant(true);
                    }

                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }

            DGUI.FadeOut.End(EditMode, false);
        }

        private ThemeVariantData DrawViewThemesViewVariantsColor(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;

            GUILayout.BeginHorizontal();
            {
                foreach (ThemeVariantData variant in themeData.Variants.Where(variant => variant != null))
                {
                    GUILayout.BeginVertical(GUILayout.Width(m_themeVariantColumnWidth));
                    {
                        deleteVariant = DrawThemeVariantButton(themeData, variant);
                        if (deleteVariant != null)
                        {
                            GUILayout.EndVertical();
                            break;
                        }

                        for (int propertyIndex = 0; propertyIndex < themeData.ColorLabels.Count; propertyIndex++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                Color color = EditorGUILayout.ColorField(GUIContent.none, variant.Colors[propertyIndex].Color, GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    variant.Colors[propertyIndex] = new ColorId(variant.Colors[propertyIndex].Id, color);
                                    themeData.SetDirty(false);
                                    UpdateAllThemeTargets(themeData);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(DGUI.Properties.Space());
                            DrawHorizontalDivider();
                            GUILayout.Space(DGUI.Properties.Space());
                        }
                    }
                    GUILayout.EndVertical();
                }

                DrawThemeAddVariantButton(themeData);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            return deleteVariant;
        }

        private ThemeVariantData DrawViewThemesViewVariantsSprite(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;
            float rowHeight = GetThemePropertyRowHeight(m_selectedThemeTab) - DGUI.Properties.Space(5);

            GUILayout.BeginHorizontal();
            {
                foreach (ThemeVariantData variant in themeData.Variants.Where(variant => variant != null))
                {
                    GUILayout.BeginVertical(GUILayout.Width(m_themeVariantColumnWidth),
                                            GUILayout.Height(rowHeight));
                    {
                        deleteVariant = DrawThemeVariantButton(themeData, variant);
                        if (deleteVariant != null)
                        {
                            GUILayout.EndVertical();
                            break;
                        }

                        for (int propertyIndex = 0; propertyIndex < themeData.SpriteLabels.Count; propertyIndex++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                var sprite = (Sprite) EditorGUILayout.ObjectField(GUIContent.none, variant.Sprites[propertyIndex].Sprite, typeof(Sprite), false,
                                                                                  GUILayout.Width(rowHeight),
                                                                                  GUILayout.Height(rowHeight));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    variant.Sprites[propertyIndex] = new SpriteId(variant.Sprites[propertyIndex].Id, sprite);
                                    themeData.SetDirty(false);
                                    UpdateAllThemeTargets(themeData);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.FlexibleSpace();
                            GUILayout.Space(DGUI.Properties.Space());
                            DrawHorizontalDivider();
                            GUILayout.Space(DGUI.Properties.Space());
                        }
                    }
                    GUILayout.EndVertical();
                }

                DrawThemeAddVariantButton(themeData);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            return deleteVariant;
        }

        private ThemeVariantData DrawViewThemesViewVariantsTexture(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;
            float rowHeight = GetThemePropertyRowHeight(m_selectedThemeTab) - DGUI.Properties.Space(5);

            GUILayout.BeginHorizontal();
            {
                foreach (ThemeVariantData variant in themeData.Variants.Where(variant => variant != null))
                {
                    GUILayout.BeginVertical(GUILayout.Width(m_themeVariantColumnWidth),
                                            GUILayout.Height(rowHeight));
                    {
                        deleteVariant = DrawThemeVariantButton(themeData, variant);
                        if (deleteVariant != null)
                        {
                            GUILayout.EndVertical();
                            break;
                        }

                        for (int propertyIndex = 0; propertyIndex < themeData.TextureLabels.Count; propertyIndex++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                var texture = (Texture) EditorGUILayout.ObjectField(GUIContent.none, variant.Textures[propertyIndex].Texture, typeof(Texture), false,
                                                                                    GUILayout.Width(rowHeight),
                                                                                    GUILayout.Height(rowHeight));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    variant.Textures[propertyIndex] = new TextureId(variant.Textures[propertyIndex].Id, texture);
                                    themeData.SetDirty(false);
                                    UpdateAllThemeTargets(themeData);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.FlexibleSpace();
                            GUILayout.Space(DGUI.Properties.Space());
                            DrawHorizontalDivider();
                            GUILayout.Space(DGUI.Properties.Space());
                        }
                    }
                    GUILayout.EndVertical();
                }

                DrawThemeAddVariantButton(themeData);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            return deleteVariant;
        }

        private ThemeVariantData DrawViewThemesViewVariantsFont(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;

            GUILayout.BeginHorizontal();
            {
                foreach (ThemeVariantData variant in themeData.Variants.Where(variant => variant != null))
                {
                    GUILayout.BeginVertical(GUILayout.Width(m_themeVariantColumnWidth));
                    {
                        deleteVariant = DrawThemeVariantButton(themeData, variant);
                        if (deleteVariant != null)
                        {
                            GUILayout.EndVertical();
                            break;
                        }

                        for (int propertyIndex = 0; propertyIndex < themeData.FontLabels.Count; propertyIndex++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                            {
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                var font = (Font) EditorGUILayout.ObjectField(GUIContent.none, variant.Fonts[propertyIndex].Font, typeof(Font), false, GUILayout.Width(m_themeVariantColumnWidth));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    variant.Fonts[propertyIndex] = new FontId(variant.Fonts[propertyIndex].Id, font);
                                    themeData.SetDirty(false);
                                    UpdateAllThemeTargets(themeData);
                                }

                                GUILayout.FlexibleSpace();
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(DGUI.Properties.Space());
                            DrawHorizontalDivider();
                            GUILayout.Space(DGUI.Properties.Space());
                        }
                    }
                    GUILayout.EndVertical();
                }

                DrawThemeAddVariantButton(themeData);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            return deleteVariant;
        }

        private ThemeVariantData DrawViewThemesViewVariantsFontAsset(ThemeData themeData)
        {
            ThemeVariantData deleteVariant = null;

            GUILayout.BeginHorizontal();
            {
                foreach (ThemeVariantData variant in themeData.Variants.Where(variant => variant != null))
                {
                    GUILayout.BeginVertical(GUILayout.Width(m_themeVariantColumnWidth));
                    {
                        deleteVariant = DrawThemeVariantButton(themeData, variant);
                        if (deleteVariant != null)
                        {
                            GUILayout.EndVertical();
                            break;
                        }

                        for (int propertyIndex = 0; propertyIndex < themeData.FontAssetLabels.Count; propertyIndex++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Width(m_themeVariantColumnWidth));
                            {
#if dUI_TextMeshPro
                                GUILayout.FlexibleSpace();
                                EditorGUI.BeginChangeCheck();
                                var fontAsset = (TMP_FontAsset) EditorGUILayout.ObjectField(GUIContent.none, variant.FontAssets[propertyIndex].FontAsset, typeof(TMP_FontAsset), false, GUILayout.Width(m_themeVariantColumnWidth));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    themeData.UndoRecord(UILabels.UpdateValue);
                                    variant.FontAssets[propertyIndex] = new FontAssetId(variant.FontAssets[propertyIndex].Id, fontAsset);
                                    themeData.SetDirty(false);
                                    UpdateAllThemeTargets(themeData);
                                }

                                GUILayout.FlexibleSpace();
#endif
                            }
                            GUILayout.EndHorizontal();

                            GUILayout.Space(DGUI.Properties.Space());
                            DrawHorizontalDivider();
                            GUILayout.Space(DGUI.Properties.Space());
                        }
                    }
                    GUILayout.EndVertical();
                }

                DrawThemeAddVariantButton(themeData);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            return deleteVariant;
        }

        #endregion

        private void DrawHorizontalDivider()
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.3f);
            GUILayout.BeginHorizontal();
            {
                DGUI.Divider.Draw(DGUI.Divider.Type.One);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUI.color = InitialGUIColor;
        }

        private static void UpdateAllThemeTargets(ThemeData themeData)
        {
            if (EditorApplication.isPlaying)
            {
                ThemeManager.UpdateTargets(themeData);
                return;
            }

            ThemeTarget[] themeTargets = FindObjectsOfType<ThemeTarget>();
            if (themeTargets == null || themeTargets.Length == 0) return;
            foreach (ThemeTarget target in themeTargets)
            {
                if (!target.ThemeId.Equals(themeData.Id)) continue;
                target.UpdateTarget(themeData);
            }
        }
    }
}