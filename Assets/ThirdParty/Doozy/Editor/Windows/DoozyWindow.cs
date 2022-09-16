// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using Doozy.Editor.Internal;
using Doozy.Editor.Settings;
using Doozy.Engine.Extensions;
using Doozy.Engine.Settings;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow : BaseEditorWindow
    {
        protected override bool UseCustomRepaintInterval { get { return true; } }
        private bool m_needsSave;
        private float VerticalIndicatorBarWidth { get { return WindowSettings.ToolbarButtonHeight; } }

        public override void OnEnable()
        {
            base.OnEnable();
            InitWindow();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllSounds();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (!m_needsSave) return;
            Save();
            DoozyUtils.ClearProgressBar();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            Event current = Event.current;
            DetectKeys(current);
            ResetKeyboardFocus(current);
            DrawViewsAreas();
            ContextualRepaint(); //perform a Repaint if any relevant event happened
        }

        private void DrawViewsAreas()
        {
            //View Area
            GUILayout.BeginArea(FullViewRect);
            {
                EditorGUI.DrawRect(new Rect(0, 0, FullViewWidth, position.height), EditorGUIUtility.isProSkin ? EditorColors.Instance.UnityDark.Dark : EditorColors.Instance.UnityLight.Light);
                EditorGUI.DrawRect(new Rect(0, 0, FullViewWidth, position.height), Color.black.WithAlpha(0.1f));
                DrawViews();
            }
            GUILayout.EndArea();

            //Vertical Indicator Bar Area
            GUILayout.BeginArea(FullViewRect);

            GUILayout.Space(WindowSettings.ToolbarHeaderHeight +
                            WindowSettings.ToolbarExpandCollapseButtonHeight +
                            WindowSettings.ToolbarButtonHeight * 10 +
                            LeftToolbarVerticalSectionSpacing * 3.5f + 2);

            m_anyDebugExpanded.target = AnyDebugActive;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(-VerticalIndicatorBarWidth * (1 - m_anyDebugExpanded.faded));
                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(-4);
                        DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.BackgroundRound), VerticalIndicatorBarWidth * m_anyDebugExpanded.faded, VerticalIndicatorBarWidth, Color.black);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(-WindowSettings.ToolbarButtonHeight);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(VerticalIndicatorBarWidth * m_anyDebugExpanded.faded * 0.2f - 2);
                        DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconDebug), VerticalIndicatorBarWidth * m_anyDebugExpanded.faded * 0.6f, VerticalIndicatorBarWidth, DGUI.Colors.GetDColor(DGUI.Colors.DebugColorName).Normal);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

            //Toolbar Shadow Area
            GUILayout.BeginArea(MainToolbarShadowRect);
            GUI.color = Color.black.WithAlpha(EditorGUIUtility.isProSkin ? 0.4f : 0.3f);
            GUILayout.Label(GUIContent.none, Styles.GetStyle(Styles.StyleName.WhiteGradientLeftToRight), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            GUI.color = InitialGUIColor;
            GUILayout.EndArea();

            //Toolbar Area
            GUILayout.BeginArea(MainToolbarRect);
            EditorGUI.DrawRect(MainToolbarRect, EditorGUIUtility.isProSkin ? EditorColors.Instance.UnityDark.Dark : EditorColors.Instance.UnityLight.Light);
            DrawLeftToolbar();
            GUILayout.EndArea();
        }

        private void InitWindow()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) FoldersProcessor.Run(true); // added to fix issues with SVNs deleting empty folders

            //window setup
            titleContent = new GUIContent(DoozyWindowSettings.Instance.WindowTitle);
            wantsMouseMove = true;
            minSize = new Vector2((MainToolbarAnimBool.target
                                       ? DoozyWindowSettings.Instance.ToolbarExpandedWidth
                                       : DoozyWindowSettings.Instance.ToolbarCollapsedWidth) +
                                  320,
                                  DoozyWindowSettings.Instance.ToolbarHeaderHeight +
                                  DoozyWindowSettings.Instance.ToolbarExpandCollapseButtonHeight +
                                  DoozyWindowSettings.Instance.ToolbarButtonHeight * 13 +
                                  LeftToolbarVerticalSectionSpacing * 5);

            m_view = (View) EditorPrefs.GetInt(DoozyWindowSettings.Instance.EditorPrefsKeyWindowCurrentView, (int) DEFAULT_VIEW);

            MainToolbarAnimBool.value = DoozyWindowSettings.Instance.DynamicToolbarExpanded;
            m_anyDebugExpanded = new AnimBool(AnyDebugActive, Repaint) {speed = 3};
            SetView(CurrentView);
        }

        private void Save()
        {
            DoozySettings.Instance.AssetDatabaseSaveAssetsNeeded = true;
            DoozySettings.Instance.SaveAndRefreshAssetDatabase();
        }
    }
}