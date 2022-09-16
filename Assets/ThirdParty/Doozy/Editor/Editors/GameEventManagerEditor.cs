// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    [CustomEditor(typeof(GameEventManager))]
    public class GameEventManagerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.GameEventManagerColorName; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderGameEventManager), MenuUtils.GameEventManager_Manual, MenuUtils.GameEventManager_YouTube);
            DrawGeneralSettings();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawGeneralSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconDoozy),
                                                   UILabels.GeneralSettings,
                                                   Size.M, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.General);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

}