// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Soundy;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.UI.Input
{
    [CustomEditor(typeof(KeyToAction))]
    public class KeyToActionEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.KeyToActionColorName; } }
        private KeyToAction m_target;

        private KeyToAction Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (KeyToAction) target;
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
            m_actions;

        private AnimBool
            m_alternateExpanded,
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded;

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
            m_actions = GetProperty(PropertyName.Actions);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_alternateExpanded = GetAnimBool(m_enableAlternateInputs.propertyPath, (InputMode) m_inputMode.enumValueIndex != InputMode.None);
            m_soundDataExpanded = GetAnimBool("SOUND");
            m_effectExpanded = GetAnimBool("EFFECT");
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable() { base.OnEnable(); }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderKeyToAction), MenuUtils.KeyToAction_Manual, MenuUtils.KeyToAction_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawInputData();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawActions();
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

        private void DrawActions()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconAction),
                                                      UILabels.Actions,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);

            GUILayout.Space(DGUI.Properties.Space());

            SerializedProperty soundDataProperty = GetProperty(PropertyName.SoundData, m_actions);
            SerializedProperty effectProperty = GetProperty(PropertyName.Effect, m_actions);
            SerializedProperty animatorEventsProperty = GetProperty(PropertyName.AnimatorEvents, m_actions);
            SerializedProperty gameEventsProperty = GetProperty(PropertyName.GameEvents, m_actions);
            SerializedProperty unityEventProperty = GetProperty(PropertyName.Event, m_actions);


            if (!m_soundDataExpanded.target && !m_effectExpanded.target && !m_animatorEventsExpanded.target && !m_gameEventsExpanded.target && !m_unityEventsExpanded.target)
                m_soundDataExpanded.target = true;

            GUILayout.BeginHorizontal();
            {
                if (DGUI.Doozy.DrawSubSectionButtonLeft(m_soundDataExpanded.target,
                                                        UILabels.Sound,
                                                        ComponentColorName,
                                                        DGUI.IconGroup.GetIcon(
                                                                               Target.Actions.HasSound,
                                                                               DGUI.IconGroup.IconSize,
                                                                               DGUI.Icon.Sound, DGUI.Icon.Sound,
                                                                               ComponentColorName, DGUI.Colors.DisabledIconColorName)))
                {
                    m_soundDataExpanded.target = true;
                    m_effectExpanded.value = false;
                    m_animatorEventsExpanded.value = false;
                    m_gameEventsExpanded.value = false;
                    m_unityEventsExpanded.value = false;
                }

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Doozy.DrawSubSectionButtonMiddle(m_effectExpanded.target,
                                                          UILabels.Effect,
                                                          ComponentColorName,
                                                          DGUI.IconGroup.GetIcon(
                                                                                 Target.Actions.HasEffect,
                                                                                 DGUI.IconGroup.IconSize,
                                                                                 DGUI.Icon.Effect, DGUI.Icon.Effect,
                                                                                 ComponentColorName, DGUI.Colors.DisabledIconColorName)))
                {
                    m_soundDataExpanded.value = false;
                    m_effectExpanded.target = true;
                    m_animatorEventsExpanded.value = false;
                    m_gameEventsExpanded.value = false;
                    m_unityEventsExpanded.value = false;
                    SoundyAudioPlayer.StopAllPlayers();
                }

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Doozy.DrawSubSectionButtonMiddle(m_animatorEventsExpanded.target,
                                                          UILabels.Animators,
                                                          ComponentColorName,
                                                          DGUI.IconGroup.GetIconWithCounter(
                                                                                            Target.Actions.HasAnimatorEvents,
                                                                                            Target.Actions.AnimatorEventsCount,
                                                                                            DGUI.IconGroup.IconSize,
                                                                                            DGUI.Icon.Animator, DGUI.Icon.Animator,
                                                                                            ComponentColorName, DGUI.Colors.DisabledIconColorName)))
                {
                    m_soundDataExpanded.value = false;
                    m_effectExpanded.value = false;
                    m_animatorEventsExpanded.target = true;
                    m_gameEventsExpanded.value = false;
                    m_unityEventsExpanded.value = false;
                    SoundyAudioPlayer.StopAllPlayers();
                }

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Doozy.DrawSubSectionButtonMiddle(m_gameEventsExpanded.target,
                                                          UILabels.GameEvents,
                                                          ComponentColorName,
                                                          DGUI.IconGroup.GetIconWithCounter(
                                                                                            Target.Actions.HasGameEvents,
                                                                                            Target.Actions.GameEventsCount,
                                                                                            DGUI.IconGroup.IconSize,
                                                                                            DGUI.Icon.GameEvent, DGUI.Icon.GameEvent,
                                                                                            ComponentColorName, DGUI.Colors.DisabledIconColorName)))
                {
                    m_soundDataExpanded.value = false;
                    m_effectExpanded.value = false;
                    m_animatorEventsExpanded.value = false;
                    m_gameEventsExpanded.target = true;
                    m_unityEventsExpanded.value = false;
                    SoundyAudioPlayer.StopAllPlayers();
                }

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Doozy.DrawSubSectionButtonRight(m_unityEventsExpanded.target,
                                                         UILabels.UnityEvents,
                                                         ComponentColorName,
                                                         DGUI.IconGroup.GetIconWithCounter(
                                                                                           Target.Actions.HasUnityEvent,
                                                                                           Target.Actions.UnityEventListenerCount,
                                                                                           DGUI.IconGroup.IconSize,
                                                                                           DGUI.Icon.UnityEvent, DGUI.Icon.UnityEvent,
                                                                                           ComponentColorName, DGUI.Colors.DisabledIconColorName)))
                {
                    m_soundDataExpanded.value = false;
                    m_effectExpanded.value = false;
                    m_animatorEventsExpanded.value = false;
                    m_gameEventsExpanded.value = false;
                    m_unityEventsExpanded.target = true;
                    SoundyAudioPlayer.StopAllPlayers();
                }
            }
            GUILayout.EndHorizontal();

            //ADD EXTRA SPACE if needed
            if (m_animatorEventsExpanded.target ||
                m_effectExpanded.target ||
                m_gameEventsExpanded.target)
                GUILayout.Space(DGUI.Properties.Space());

            //DRAW SOUND
            if (m_soundDataExpanded.target)
                DGUI.Property.DrawWithFade(soundDataProperty, m_soundDataExpanded);

            //DRAW EFFECT
            if (m_effectExpanded.target)
                DGUI.Doozy.DrawUIEffect(Target.gameObject,
                                        Target.Actions.Effect,
                                        GetProperty(PropertyName.ParticleSystem, effectProperty),
                                        GetProperty(PropertyName.Behavior, effectProperty),
                                        GetProperty(PropertyName.StopBehavior, effectProperty),
                                        GetProperty(PropertyName.AutoSort, effectProperty),
                                        GetProperty(PropertyName.SortingSteps, effectProperty),
                                        GetProperty(PropertyName.CustomSortingLayer, effectProperty),
                                        GetProperty(PropertyName.CustomSortingOrder, effectProperty),
                                        m_effectExpanded,
                                        ComponentColorName);

            //DRAW ANIMATOR EVENTS
            if (m_animatorEventsExpanded.target)
                DGUI.List.DrawWithFade(animatorEventsProperty, m_animatorEventsExpanded, ComponentColorName, UILabels.ListIsEmpty);

            //DRAW GAME EVENTS
            if (m_gameEventsExpanded.target)
                DGUI.List.DrawWithFade(gameEventsProperty, m_gameEventsExpanded, ComponentColorName, UILabels.ListIsEmpty, 1);

            //DRAW EVENTS (UnityEvent)
            if (m_unityEventsExpanded.target)
                DGUI.Property.UnityEventWithFade(unityEventProperty, m_unityEventsExpanded, "Actions.Event", ComponentColorName, Target.Actions.UnityEventListenerCount);
        }
    }
}