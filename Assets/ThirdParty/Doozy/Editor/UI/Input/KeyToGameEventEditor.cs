// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.UI.Input
{
    [CustomEditor(typeof(KeyToGameEvent))]
    public class KeyToGameEventEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.KeyToGameEventColorName; } }
        private KeyToGameEvent m_target;

        public KeyToGameEvent Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (KeyToGameEvent) target;
                return m_target;
            }
        }

        private SerializedProperty
            m_inputData,
            m_enableAlternateInputs,
            m_inputMode,
            m_keyCode,
            m_keyCodeAlt,
            m_virtualButtonName,
            m_virtualButtonNameAlt,
            m_gameEvent;

        private AnimBool
            m_alternateExpanded;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();
            m_inputData = GetProperty(PropertyName.InputData);
            m_enableAlternateInputs = GetProperty(PropertyName.EnableAlternateInputs, m_inputData);
            m_inputMode = GetProperty(PropertyName.InputMode, m_inputData);
            m_keyCode = GetProperty(PropertyName.KeyCode, m_inputData);
            m_keyCodeAlt = GetProperty(PropertyName.KeyCodeAlt, m_inputData);
            m_virtualButtonName = GetProperty(PropertyName.VirtualButtonName, m_inputData);
            m_virtualButtonNameAlt = GetProperty(PropertyName.VirtualButtonNameAlt, m_inputData);
            m_gameEvent = GetProperty(PropertyName.GameEvent);
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_alternateExpanded = GetAnimBool(m_enableAlternateInputs.propertyPath, (InputMode) m_inputMode.enumValueIndex != InputMode.None);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderKeyToGameEvent), MenuUtils.KeyToGameEvent_Manual, MenuUtils.KeyToGameEvent_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawInputData();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawGameEvent();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInputData()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconFaKeyboard),
                                                      UILabels.Key,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);
            GUILayout.Space(DGUI.Properties.Space());

            var inputMode = (InputMode) m_inputMode.enumValueIndex;
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(inputMode != InputMode.None, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(inputMode != InputMode.None, ComponentColorName);


            m_alternateExpanded.target = inputMode != InputMode.None;

            DGUI.Line.Draw(false,
                           () =>
                           {
                               DGUI.Line.Draw(false, backgroundColorName,
                                              () =>
                                              {
                                                  GUILayout.Space(DGUI.Properties.Space(2));
                                                  DGUI.Label.Draw(UILabels.InputMode, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                                                  EditorGUI.BeginChangeCheck();
                                                  DGUI.Property.Draw(m_inputMode, backgroundColorName, DGUI.Properties.SingleLineHeight);
                                                  if (EditorGUI.EndChangeCheck()) DGUI.Properties.ResetKeyboardFocus();
                                              });
                           },
                           () =>
                           {
                               switch (inputMode)
                               {
                                   case InputMode.KeyCode:
                                       GUILayout.Space(DGUI.Properties.Space());
                                       DGUI.Line.Draw(false, ComponentColorName,
                                                      () =>
                                                      {
                                                          GUILayout.Space(DGUI.Properties.Space(2));
                                                          DGUI.Label.Draw(UILabels.KeyCode, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                          DGUI.Property.Draw(m_keyCode, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                          GUILayout.Space(DGUI.Properties.Space());
                                                      });
                                       break;
                                   case InputMode.VirtualButton:
                                       GUILayout.Space(DGUI.Properties.Space());
                                       DGUI.Line.Draw(false, ComponentColorName,
                                                      () =>
                                                      {
                                                          GUILayout.Space(DGUI.Properties.Space(2));
                                                          DGUI.Label.Draw(UILabels.VirtualButton, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                          DGUI.Property.Draw(m_virtualButtonName, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                          GUILayout.Space(DGUI.Properties.Space());
                                                      });
                                       break;
                               }
                           }
                          );

            if (DGUI.FadeOut.Begin(m_alternateExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Line.Draw(false,
                               () =>
                               {
                                   DGUI.Toggle.Checkbox.Draw(m_enableAlternateInputs, UILabels.AlternateInput, ComponentColorName, true, false);
                               },
                               () =>
                               {
                                   bool enabled = GUI.enabled;
                                   GUI.enabled = m_enableAlternateInputs.boolValue;

                                   backgroundColorName = !m_enableAlternateInputs.boolValue ? DGUI.Colors.DisabledBackgroundColorName : ComponentColorName;
                                   textColorName = !m_enableAlternateInputs.boolValue ? DGUI.Colors.DisabledTextColorName : ComponentColorName;

                                   switch (inputMode)
                                   {
                                       case InputMode.KeyCode:
                                           GUILayout.Space(DGUI.Properties.Space());
                                           DGUI.Line.Draw(false, backgroundColorName,
                                                          () =>
                                                          {
                                                              GUILayout.Space(DGUI.Properties.Space(2));
                                                              DGUI.Label.Draw(UILabels.AlternateKeyCode, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                                                              DGUI.Property.Draw(m_keyCodeAlt, backgroundColorName, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.Space(DGUI.Properties.Space());
                                                          });
                                           break;
                                       case InputMode.VirtualButton:
                                           GUILayout.Space(DGUI.Properties.Space());
                                           DGUI.Line.Draw(false, backgroundColorName,
                                                          () =>
                                                          {
                                                              GUILayout.Space(DGUI.Properties.Space(2));
                                                              DGUI.Label.Draw(UILabels.AlternateVirtualButton, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                                                              DGUI.Property.Draw(m_virtualButtonNameAlt, backgroundColorName, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.Space(DGUI.Properties.Space());
                                                          });
                                           break;
                                   }

                                   GUI.enabled = enabled;
                               }
                              );
            }

            DGUI.FadeOut.End(m_alternateExpanded, false);
        }

        private void DrawGameEvent()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconGameEvent),
                                                      UILabels.GameEvent,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);
            
            GUILayout.Space(DGUI.Properties.Space());
            
            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.GameEvent, Size.M, TextAlign.Left, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space());
                               DGUI.Property.Draw(m_gameEvent, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }
    }
}