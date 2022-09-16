// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Soundy;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Soundy
{
    [CustomEditor(typeof(SoundyController))]
    public class SoundyControllerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.SoundyControllerColorName; } }
        private SoundyController Target { get { return (SoundyController) target; } }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSoundyController));
            DrawSoundySettings();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawSoundySettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconSoundy),
                                                   UILabels.SoundySettings,
                                                   Size.M, TextAlign.Left,
                                                   ComponentColorName, ComponentColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.Soundy);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}