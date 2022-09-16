// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Windows;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void DrawViewNody()
        {
            if (CurrentView != View.Nody) return;

            DrawOpenNodyWindowButton();
            DrawDynamicViewVerticalSpace(2);
            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaCog, UILabels.Settings, UILabels.NodySettings, CurrentViewColorName);
            DrawDynamicViewVerticalSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    DrawNodyClearRecentGraphs();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawNodyGraphZoom();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawNodyGraphDotAnimationSpeed();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawNodyShowNodeNotes();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            
            DrawDynamicViewVerticalSpace(2);
        }

        private static void DrawOpenNodyWindowButton()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconNody),
                                                       UILabels.OpenNody,
                                                       Size.XL, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space(2),
                                                       false))
                    NodyWindow.Open();
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawNodyClearRecentGraphs()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.ClearRecentOpenedGraphsList, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(4));
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaBroom),
                                                       UILabels.Clear,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    NodyWindowSettings.Instance.RecentlyOpenedGraphs.Clear();
                    NodyWindowSettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.ClearRecentOpenedGraphsListDescription);
        }

        private void DrawNodyGraphZoom()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.DefaultZoom, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float zoom = NodyWindowSettings.Instance.DefaultZoom;
                EditorGUI.BeginChangeCheck();
                zoom = EditorGUILayout.Slider(zoom, NodyWindowSettings.ZOOM_MIN, NodyWindowSettings.ZOOM_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    NodyWindowSettings.Instance.DefaultZoom = (float) Math.Round(zoom, 1);
                    NodyWindowSettings.Instance.SetDirty(false);
                }

                GUILayout.Space(DGUI.Properties.Space(2));
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                       UILabels.Reset,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    NodyWindowSettings.Instance.DefaultZoom = NodyWindowSettings.ZOOM_DEFAULT_VALUE;
                    NodyWindowSettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.DefaultZoomDescription);
        }

        private void DrawNodyGraphDotAnimationSpeed()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.DotAnimationSpeed, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float dotAnimationSpeed = NodyWindowSettings.Instance.DefaultDotAnimationSpeed;
                EditorGUI.BeginChangeCheck();
                dotAnimationSpeed = EditorGUILayout.Slider(dotAnimationSpeed, NodyWindowSettings.DOT_ANIMATION_SPEED_MIN, NodyWindowSettings.DOT_ANIMATION_SPEED_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    NodyWindowSettings.Instance.DefaultDotAnimationSpeed = (float) Math.Round(dotAnimationSpeed, 1);
                    NodyWindowSettings.Instance.CurrentDotAnimationSpeed = NodyWindowSettings.Instance.DefaultDotAnimationSpeed;
                    NodyWindowSettings.Instance.SetDirty(false);
                }

                GUILayout.Space(DGUI.Properties.Space(2));
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconReset),
                                                       UILabels.Reset,
                                                       Size.M, TextAlign.Left,
                                                       DGUI.Colors.DisabledBackgroundColorName,
                                                       DGUI.Colors.DisabledTextColorName,
                                                       NormalRowHeight,
                                                       false))
                {
                    NodyWindowSettings.Instance.DefaultDotAnimationSpeed = NodyWindowSettings.DOT_ANIMATION_SPEED_DEFAULT_VALUE;
                    NodyWindowSettings.Instance.CurrentDotAnimationSpeed = NodyWindowSettings.Instance.DefaultDotAnimationSpeed;
                    NodyWindowSettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.DefaultDotAnimationSpeedDescription);
        }
        
        private void DrawNodyShowNodeNotes()
        {
            GUILayout.BeginHorizontal();
            {
                bool value = NodyWindowSettings.Instance.ShowNodeNotes;
                EditorGUI.BeginChangeCheck();
                value = DGUI.Toggle.Switch.Draw(value, CurrentViewColorName);
                if (EditorGUI.EndChangeCheck())
                {
                    NodyWindowSettings.Instance.UndoRecord(UILabels.ShowNodeNotes);
                    NodyWindowSettings.Instance.ShowNodeNotes = value;
                    NodyWindowSettings.Instance.SetDirty(false);
                }
                GUILayout.Space(DGUI.Properties.Space(4));
                DGUI.WindowUtils.DrawSettingLabel(UILabels.ShowNodeNotes, DGUI.Colors.GetTextColorName(value, CurrentViewColorName), NormalRowHeight);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.ShowNodeNotesDescription);
        }
    }
}