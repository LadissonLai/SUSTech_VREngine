// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private const float GENERAL_COLUMN_MIN_WIDTH = 256;
        private const float GENERAL_COLUMN_SPACING = 24;
        private const float PLUGIN_BOX_HEIGHT = 48;
        private const float PLUGIN_BOX_ICON_SIZE = 32;
        private const float PLUGIN_BOX_PADDING = 8;
        private const float PLUGIN_BOX_WIDTH = 256;

        private const float OPTION_BOX_HEIGHT = 48;
        private const float OPTION_BOX_ICON_SIZE = 32;
        private const float OPTION_BOX_PADDING = 8;
        private const float OPTION_BOX_WIDTH = 256;

        private static bool s_needsToUpdateScriptingDefineSymbols;

        private float GeneralColumnWidth { get { return (FullViewWidth - ViewContentLeftHorizontalPadding * 2 - ViewContentRightHorizontalPadding * 2 - DynamicViewVerticalSpace(2)) / 2; } }

        private void DrawViewGeneral()
        {
            if (CurrentView != View.General) return;

            //Refresh & Language
            DrawGeneralRefreshAndLanguageOptions();

            DrawDynamicViewVerticalSpace();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                //column 1 - Functionality
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DrawGeneralFunctionality();
                }
                GUILayout.EndVertical();

                DrawDynamicViewVerticalSpace(2);

                //column 2 - Integrations
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.MaxWidth(GeneralColumnWidth));
                {
                    DrawGeneralIntegrations();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            DrawDynamicViewVerticalSpace(2);

            //News
            DrawGeneralNews();

            DrawDynamicViewVerticalSpace(2);
        }

        #region Refresh and Language

        private void DrawGeneralRefreshAndLanguageOptions()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            {
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaMagic), UILabels.Refresh, Size.S, TextAlign.Left, DGUI.Colors.DarkOrLightColorName, DGUI.Colors.LightOrDarkColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                {
                    DoozyAssetsProcessor.Run();
                    DGUI.Properties.ExitGUI();
                }
                GUILayout.FlexibleSpace();
                GUI.enabled = false;
                {
                    DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconFaLanguage), 28, DGUI.Properties.SingleLineHeight, DGUI.Colors.LightOrDarkColorName);
                    Engine.Language currentLanguage = LanguagePack.CurrentLanguage;
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginVertical();
                    GUILayout.Space(0);
                    currentLanguage = (Engine.Language) EditorGUILayout.EnumPopup(currentLanguage, GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 3));
                    GUILayout.EndVertical();
                    if (EditorGUI.EndChangeCheck())
                        if (currentLanguage != Engine.Language.Unknown)
                            LanguagePack.CurrentLanguage = currentLanguage;
                }
                GUI.enabled = true;
            }
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Functionality

        private void DrawGeneralFunctionality()
        {
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaAtomAlt, UILabels.Functionality, UILabels.FunctionalityDescription, DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(0.5f);
            DrawGeneralOptionBoxOrientationDetector();
            DrawDynamicViewVerticalSpace(0.25f);
            DrawUseBackButton();
            DrawDynamicViewVerticalSpace(0.25f);
            DrawAutoDisableUIInteractionsButton();
        }

        private void DrawGeneralOptionBoxOrientationDetector()
        {
            bool enabled = Settings.UseOrientationDetector;
            EditorGUI.BeginChangeCheck();
            enabled = DrawGeneralOptionBox("Orientation Detector", UILabels.OrientationDetectorDescription, enabled, Styles.GetStyle(Styles.StyleName.IconOrientationDetector), DGUI.Colors.OrientationDetectorColorName);
            if (EditorGUI.EndChangeCheck()) Settings.UseOrientationDetector = enabled;
        }

        private void DrawUseBackButton()
        {
            bool enabled = Settings.UseBackButton;
            EditorGUI.BeginChangeCheck();
            enabled = DrawGeneralOptionBox("'Back' Button", UILabels.UseBackButtonDescription, enabled, Styles.GetStyle(Styles.StyleName.IconBackButton), DGUI.Colors.BackButtonColorName);
            if (EditorGUI.EndChangeCheck()) Settings.UseBackButton = enabled;
        }

        private void DrawAutoDisableUIInteractionsButton()
        {
            bool enabled = Settings.AutoDisableUIInteractions;
            EditorGUI.BeginChangeCheck();
            enabled = DrawGeneralOptionBox("Auto Disable UI Interactions", UILabels.AutoDisableUIInteractionsDescription, enabled, Styles.GetStyle(Styles.StyleName.IconDisableButton), ColorName.Orange);

            if (EditorGUI.EndChangeCheck()) Settings.AutoDisableUIInteractions = enabled;
        }


        private bool DrawGeneralOptionBox(string optionName, string optionDescription, bool enabled, GUIStyle optionIcon, ColorName colorName)
        {
            bool value = enabled;

            GUILayout.BeginVertical(DGUI.Background.Style(CornerType.Round), GUILayout.MinHeight(OPTION_BOX_HEIGHT));
            {
                GUILayout.Space(OPTION_BOX_PADDING);
                GUI.enabled = !EditorApplication.isCompiling;
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(OPTION_BOX_PADDING);
                        DGUI.Icon.Draw(optionIcon, OPTION_BOX_ICON_SIZE, DGUI.Colors.GetTextColorName(enabled, colorName));
                        GUILayout.Space(OPTION_BOX_PADDING);
                        GUILayout.BeginVertical();
                        {
                            DGUI.Label.Draw(optionName, Size.XL, DGUI.Colors.GetTextColorName(enabled, colorName));
                            GUILayout.Space(2);
                            GUI.enabled = !EditorApplication.isCompiling;
                            DGUI.WindowUtils.DrawSettingDescription(optionDescription);
                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(OPTION_BOX_PADDING);
                        GUILayout.FlexibleSpace();
                        if (DGUI.Button.Draw(enabled ? UILabels.Enabled : UILabels.Disabled, Size.M, enabled ? ColorName.Green : DGUI.Colors.DarkOrLightColorName, enabled ? ColorName.Green : DGUI.Colors.LightOrDarkColorName, enabled, OPTION_BOX_ICON_SIZE, 80))
                        {
                            DoozySettings.Instance.UndoRecord(enabled ? UILabels.DisableFunctionality : UILabels.EnableFunctionality);
                            DoozySettings.Instance.SetDirty(false);
                            m_needsSave = true;
                            value = !value;
                        }

                        GUILayout.Space(OPTION_BOX_PADDING);
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }
            GUILayout.EndVertical();

            return value;
        }

        #endregion

        #region Integrations

        private void DrawGeneralIntegrations()
        {
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaChartNetwork, UILabels.Integrations, UILabels.ToggleSupportForThirdPartyPlugins, DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(0.5f);
            DrawGeneralPluginBoxPlaymaker();
            DrawDynamicViewVerticalSpace(0.25f);
            DrawGeneralPluginBoxMasterAudio();
            DrawDynamicViewVerticalSpace(0.25f);
            DrawGeneralPluginBoxTextMeshPro();

            if (!s_needsToUpdateScriptingDefineSymbols) return;
            s_needsToUpdateScriptingDefineSymbols = false;
            DefineSymbolsProcessor.UpdateScriptingDefineSymbols();
        }

        private void DrawGeneralPluginBoxMasterAudio()
        {
            bool usePlugin = Settings.UseMasterAudio;
            EditorGUI.BeginChangeCheck();
            usePlugin = DrawGeneralPluginBox("MasterAudio", Settings.MasterAudioDetected, usePlugin, DefineSymbolsProcessor.DEFINE_MASTER_AUDIO, Styles.GetStyle(Styles.StyleName.IconMasterAudio));
            if (EditorGUI.EndChangeCheck()) Settings.UseMasterAudio = usePlugin;
        }

        private void DrawGeneralPluginBoxPlaymaker()
        {
            bool usePlugin = Settings.UsePlaymaker;
            EditorGUI.BeginChangeCheck();
            usePlugin = DrawGeneralPluginBox("Playmaker", Settings.PlaymakerDetected, usePlugin, DefineSymbolsProcessor.DEFINE_PLAYMAKER, Styles.GetStyle(Styles.StyleName.IconPlaymaker));
            if (EditorGUI.EndChangeCheck()) Settings.UsePlaymaker = usePlugin;
        }

        private void DrawGeneralPluginBoxTextMeshPro()
        {
            bool usePlugin = Settings.UseTextMeshPro;
            EditorGUI.BeginChangeCheck();
            usePlugin = DrawGeneralPluginBox("TextMeshPro", Settings.TextMeshProDetected, usePlugin, DefineSymbolsProcessor.DEFINE_TEXT_MESH_PRO, Styles.GetStyle(Styles.StyleName.IconTextMeshPro));
            if (EditorGUI.EndChangeCheck()) Settings.UseTextMeshPro = usePlugin;
        }

        private bool DrawGeneralPluginBox(string pluginName, bool hasPlugin, bool usePlugin, string pluginDefineSymbol, GUIStyle pluginIcon)
        {
            bool value = usePlugin;

            GUILayout.BeginVertical(DGUI.Background.Style(CornerType.Round), GUILayout.Height(PLUGIN_BOX_HEIGHT));
            {
                GUILayout.Space(PLUGIN_BOX_PADDING);
                GUI.enabled = !EditorApplication.isCompiling;
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(PLUGIN_BOX_PADDING);
                        DGUI.Icon.Draw(pluginIcon, PLUGIN_BOX_ICON_SIZE, Color.white);
                        GUILayout.Space(PLUGIN_BOX_PADDING);
                        GUILayout.BeginVertical();
                        {
                            DGUI.Label.Draw(pluginName, Size.XL, DGUI.Colors.DisabledTextColorName);
                            GUILayout.Space(2);
                            GUI.enabled = !EditorApplication.isCompiling && hasPlugin;
                            DGUI.Label.Draw(EditorApplication.isCompiling
                                                ? UILabels.Compiling + "..."
                                                : hasPlugin
                                                    ? UILabels.Installed
                                                    : UILabels.NotInstalled,
                                            Size.M,
                                            DGUI.Colors.DisabledTextColorName);
                        }
                        GUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                        if (DGUI.Button.Draw(usePlugin ? UILabels.Enabled : UILabels.Disabled, Size.M, usePlugin ? ColorName.Green : DGUI.Colors.DarkOrLightColorName, usePlugin ? ColorName.Green : DGUI.Colors.LightOrDarkColorName, usePlugin, PLUGIN_BOX_ICON_SIZE, 80))
                            if (DoozyUtils.DisplayDialog((usePlugin ? UILabels.Disable : UILabels.Enable) + " " + pluginName,
                                                         UILabels.ScriptingDefineSymbol + ": " + pluginDefineSymbol +
                                                         "\n\n" +
                                                         (usePlugin ? UILabels.RemoveSymbolFromAllBuildTargetGroups : UILabels.AddSymbolToAllBuildTargetGroups),
                                                         UILabels.Yes,
                                                         UILabels.No))
                            {
                                DoozySettings.Instance.UndoRecord(usePlugin ? UILabels.DisablePlugin : UILabels.EnablePlugin);
                                DoozySettings.Instance.SetDirty(false);
                                m_needsSave = true;
                                s_needsToUpdateScriptingDefineSymbols = true;
                                value = !value;
                            }

                        GUILayout.Space(PLUGIN_BOX_PADDING);
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.enabled = true;
            }
            GUILayout.EndVertical();

            return value;
        }

        #endregion

        #region News

        private GUIStyle m_newsTitleStyle, m_newsContentStyle;

        private GUIStyle NewsTitleStyle
        {
            get
            {
                if (m_newsTitleStyle != null) return m_newsTitleStyle;
                m_newsTitleStyle = new GUIStyle(DGUI.Label.Style(Size.L, ColorName.LightBlue));
                return m_newsTitleStyle;
            }
        }

        private GUIStyle NewsContentStyle
        {
            get
            {
                if (m_newsContentStyle != null) return m_newsContentStyle;
                m_newsContentStyle = new GUIStyle(DGUI.Label.Style(Size.S, DGUI.Colors.DisabledTextColorName)) {wordWrap = true};
                return m_newsContentStyle;
            }
        }

        private void DrawGeneralNews()
        {
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaNewspaper, UILabels.News, UILabels.NoteworthyInformation, DGUI.Colors.LightOrDarkColorName);
            DrawDynamicViewVerticalSpace(0.5f);

            GUILayout.BeginHorizontal(GUILayout.Width(FullViewWidth - ViewContentLeftHorizontalPadding * 2 - ViewContentRightHorizontalPadding * 2));
            {
                GUILayout.Space(40 * CurrentViewExpanded.faded);
                GUILayout.BeginVertical();
                {
                    DGUI.Label.Draw("Welcome to Version 3!",
                                    NewsTitleStyle);
                    GUILayout.Space(4 * CurrentViewExpanded.faded);
                    GUILayout.Label(
                                    "In a world full of options, we would like to take a moment and say thanks for choosing DoozyUI as your UI management solution. " +
                                    "We’ve put a lot of thought, work and love into creating this product and we really hope you’ll enjoy using it." +
                                    "\n\n" +
                                    "We've been developing this system since 2015, when it was first released on the Unity Asset Store, and we have a lot of plans for it, going forward. " +
                                    "The UITemplates system is still in the works (in beta), because its current workflow is not as clean and easy as we would like it to be. DoozyUI version 3.1 will be the version we release it in." +
                                    "\n\n" +
                                    "A new line of products will be released gradually in the next few months that will include packs of animated icons, animated progress bars (indicators), animated buttons, animated transitions, animated titles and animated lower thirds. " +
                                    "Our goal is to give you a lot of ready-to-use UI elements that can be customized and added to your scene with a simple drag and drop." +
                                    "\n\n" +
                                    "Once again, thanks for choosing us, if you’ve got any feedback, we’re just an email away.",
                                    NewsContentStyle
                                   );
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}