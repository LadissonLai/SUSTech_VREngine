// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UICanvas))]
    [CanEditMultipleObjects]
    public class UICanvasEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UICanvasColorName; } }
        private UICanvas m_target;

        private UICanvas Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UICanvas) target;
                return m_target;
            }
        }

        private static UICanvasSettings Settings { get { return UICanvasSettings.Instance; } }
        private static NamesDatabase Database { get { return UICanvasSettings.Database; } }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUICanvas), "UICanvas", MenuUtils.UICanvas_Manual, MenuUtils.UICanvas_YouTube);
            DrawDebugModeAndDontDestroyOnLoad();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawCanvasName();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Doozy.DrawRenameGameObjectAndOpenDatabaseButtons(Settings.RenamePrefix,
                                                                  GetProperty(PropertyName.CanvasName).stringValue,
                                                                  Settings.RenameSuffix,
                                                                  Target.gameObject,
                                                                  DoozyWindow.View.Canvases);
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebugModeAndDontDestroyOnLoad()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.DebugMode), UILabels.DebugMode, ColorName.Red, true, false);
                GUILayout.FlexibleSpace();
                DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.DontDestroyCanvasOnLoad), UILabels.DontDestroyGameObjectOnLoad, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCanvasName()
        {
            SerializedProperty canvasName = GetProperty(PropertyName.CanvasName);
            SerializedProperty customCanvasName = GetProperty(PropertyName.CustomCanvasName);
            float backgroundHeight = DGUI.Properties.SingleLineHeight * 1.9f;
            Color initialColor = GUI.color;
            DGUI.Line.Draw(true, ComponentColorName, backgroundHeight,
                           () =>
                           {
                               List<string> items = Database.GetNamesList(UICanvas.DefaultCanvasCategory);
                               int selectedIndex = 0;
                               if (!customCanvasName.boolValue)
                               {
                                   if (items.Contains(canvasName.stringValue))
                                       selectedIndex = items.IndexOf(canvasName.stringValue);
                                   else
                                       customCanvasName.boolValue = true;
                               }

                               GUILayout.BeginVertical();
                               {
                                   DGUI.Label.Draw(UILabels.CanvasName, Size.S, TextAlign.Left, ComponentColorName); //draw 'Canvas Name' label
                                   GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                   {
                                       GUI.color = DGUI.Colors.GetDColor(ComponentColorName).Light.WithAlpha(GUI.color.a);
                                       if (customCanvasName.boolValue)
                                       {
                                           EditorGUILayout.PropertyField(canvasName, GUIContent.none, true); //draw canvas name property field (text field)
                                       }
                                       else
                                       {
                                           EditorGUI.BeginChangeCheck();
                                           selectedIndex = EditorGUILayout.Popup(selectedIndex, items.ToArray()); //draw canvas name selector (dropdown field)
                                           if (EditorGUI.EndChangeCheck()) canvasName.stringValue = items[selectedIndex];
                                       }

                                       GUI.color = initialColor;
                                       EditorGUI.BeginChangeCheck();
                                       DGUI.Toggle.Checkbox.Draw(customCanvasName, UILabels.CustomName, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2)); //draw 'custom' toggle
                                       if (EditorGUI.EndChangeCheck())
                                           if (!customCanvasName.boolValue)
                                               if (!items.Contains(canvasName.stringValue))
                                               {
                                                   if (EditorUtility.DisplayDialog("New Canvas Name",
                                                                                   "Add the '" + canvasName.stringValue + "' canvas name to the database?",
                                                                                   "Yes",
                                                                                   "No"))
                                                   {
                                                       Database.GetNamesList(UICanvas.DefaultCanvasCategory, true).Add(canvasName.stringValue);
                                                       Database.SetDirty(true);
                                                   }
                                                   else
                                                   {
                                                       canvasName.stringValue = Database.GetNamesList(UICanvas.DefaultCanvasCategory)[0];
                                                   }
                                               }
                                   }
                                   GUILayout.EndHorizontal();
                               }
                               GUILayout.EndVertical();
                           });
        }
    }
}