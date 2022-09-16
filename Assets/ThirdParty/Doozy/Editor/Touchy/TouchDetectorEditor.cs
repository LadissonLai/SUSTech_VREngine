// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Touchy;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Touchy
{
    [CustomEditor(typeof(TouchDetector))]
    public class TouchDetectorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.TouchDetectorColorName; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderTouchDetector), MenuUtils.TouchDetector_Manual, MenuUtils.TouchDetector_YouTube);
            DrawTouchySettings();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTouchySettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconTouchy),
                                                   UILabels.TouchySettings,
                                                   Size.M, TextAlign.Left,
                                                   ComponentColorName, ComponentColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.Touchy);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}