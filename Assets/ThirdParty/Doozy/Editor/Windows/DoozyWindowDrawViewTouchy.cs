// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Touchy;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Windows
{
    public partial class DoozyWindow
    {
        private void DrawViewTouchy()
        {
            if (CurrentView != View.Touchy) return;

            DGUI.WindowUtils.DrawIconTitle(Styles.StyleName.IconFaCog, UILabels.Settings, UILabels.TouchySettings, CurrentViewColorName);
            DrawDynamicViewVerticalSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Bar.Height(Size.XL) + DGUI.Properties.Space(4));
                GUILayout.BeginVertical();
                {
                    DrawTouchySwipeLength();
                    DrawDynamicViewVerticalSpace(0.5f);
                    DrawTouchyLongTapDuration();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
            
            DrawDynamicViewVerticalSpace(2);
        }

        private void DrawTouchySwipeLength()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.SwipeLength, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float swipeLength = TouchySettings.Instance.SwipeLength;
                EditorGUI.BeginChangeCheck();
                swipeLength = EditorGUILayout.Slider(swipeLength, TouchySettings.SWIPE_LENGTH_MIN, TouchySettings.SWIPE_LENGTH_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    TouchySettings.Instance.SwipeLength = (float) Math.Round(swipeLength, 1);
                    TouchySettings.Instance.SetDirty(false);
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
                    TouchySettings.Instance.SwipeLength = TouchySettings.SWIPE_LENGTH_DEFAULT_VALUE;
                    TouchySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.SwipeLengthDescription);
        }

        private void DrawTouchyLongTapDuration()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.WindowUtils.DrawSettingLabel(UILabels.LongTapDuration, CurrentViewColorName, NormalRowHeight);
                GUILayout.Space(DGUI.Properties.Space(2));
                float duration = TouchySettings.Instance.LongTapDuration;
                EditorGUI.BeginChangeCheck();
                duration = EditorGUILayout.Slider(duration, TouchySettings.LONG_TAP_DURATION_MIN, TouchySettings.LONG_TAP_DURATION_MAX);
                if (EditorGUI.EndChangeCheck())
                {
                    TouchySettings.Instance.LongTapDuration = (float) Math.Round(duration, 1);
                    TouchySettings.Instance.SetDirty(false);
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
                    TouchySettings.Instance.LongTapDuration = TouchySettings.LONG_TAP_DURATION_DEFAULT_VALUE;
                    TouchySettings.Instance.SetDirty(false);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.WindowUtils.DrawSettingDescription(UILabels.LongTapDurationDescription);
        }
    }
}