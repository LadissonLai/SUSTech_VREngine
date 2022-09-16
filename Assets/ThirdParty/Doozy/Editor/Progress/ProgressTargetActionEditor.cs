// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Editor.Soundy;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressTargetAction))]
    [CanEditMultipleObjects]
    public class ProgressTargetActionEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }
        private ProgressTargetAction m_target;

        private ProgressTargetAction Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (ProgressTargetAction) target;
                return m_target;
            }
        }

        private SerializedProperty
            m_actions,
            m_compareMethod,
            m_disableTriggerAfterActivation,
            m_resetAfterDelay,
            m_resetDelay,
            m_resetOnDisable,
            m_resetOnEnable,
            m_triggerValue,
            m_triggerMinValue,
            m_triggerMaxValue,
            m_targetVariable,
            m_tolerance,
            m_useUnscaledTime;

        private AnimBool
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded;


        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_actions = GetProperty(PropertyName.Actions);
            m_compareMethod = GetProperty(PropertyName.CompareMethod);
            m_disableTriggerAfterActivation = GetProperty(PropertyName.DisableTriggerAfterActivation);
            m_resetAfterDelay = GetProperty(PropertyName.ResetAfterDelay);
            m_resetDelay = GetProperty(PropertyName.ResetDelay);
            m_resetOnDisable = GetProperty(PropertyName.ResetOnDisable);
            m_resetOnEnable = GetProperty(PropertyName.ResetOnEnable);
            m_triggerValue = GetProperty(PropertyName.TriggerValue);
            m_triggerMinValue = GetProperty(PropertyName.TriggerMinValue);
            m_triggerMaxValue = GetProperty(PropertyName.TriggerMaxValue);
            m_targetVariable = GetProperty(PropertyName.TargetVariable);
            m_tolerance = GetProperty(PropertyName.Tolerance);
            m_useUnscaledTime = GetProperty(PropertyName.UseUnscaledTime);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_soundDataExpanded = GetAnimBool("SOUND");
            m_effectExpanded = GetAnimBool("EFFECT");
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTargetAction), MenuUtils.ProgressTargetAction_Manual, MenuUtils.ProgressTargetAction_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawTargetVariableTargetValueAndTolerance();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawDisableTriggerAfterActivation();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawResetOptions();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawResetDelay();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawActions();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTargetVariableTargetValueAndTolerance()
        {
            float backgroundHeight = DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space();
            DGUI.Background.Draw(ComponentColorName, GUILayout.Height(backgroundHeight));
            GUILayout.Space(-backgroundHeight);
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(2));
                DGUI.Label.Draw(UILabels.TriggerActions, Size.S, ComponentColorName);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(2));
                DGUI.Label.Draw(UILabels.When, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                GUILayout.Space(DGUI.Properties.Space());
                EditorGUI.BeginChangeCheck();
                DGUI.Property.Draw(m_targetVariable, ComponentColorName, 100, DGUI.Properties.SingleLineHeight);
                if (EditorGUI.EndChangeCheck()) DGUI.Properties.ResetKeyboardFocus();
                DGUI.Label.Draw(UILabels.Is, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);


                var compareMethod = (CompareType) m_compareMethod.enumValueIndex;
                float compareMethodWidth;
                switch (compareMethod)
                {
                    case CompareType.Between:
                        compareMethodWidth = 60f;
                        break;
                    case CompareType.NotBetween:
                        compareMethodWidth = 85f;
                        break;
                    case CompareType.EqualTo:
                    case CompareType.NotEqualTo:
                    case CompareType.GreaterThan:
                    case CompareType.LessThan:
                        compareMethodWidth = 80f;
                        break;
                    case CompareType.GreaterThanOrEqualTo:
                    case CompareType.LessThanOrEqualTo:
                        compareMethodWidth = 150f;
                        break;
                    default:
                        compareMethodWidth = 150f;
                        break;
                }

                DGUI.Property.Draw(m_compareMethod, ComponentColorName, compareMethodWidth, DGUI.Properties.SingleLineHeight);
                switch (compareMethod)
                {
                    case CompareType.Between:
                    case CompareType.NotBetween:
                        DGUI.Label.Draw(UILabels.Min, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(m_triggerMinValue, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Label.Draw(UILabels.Max, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(m_triggerMaxValue, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        break;
                    case CompareType.EqualTo:
                    case CompareType.NotEqualTo:
                        GUILayout.Space(DGUI.Properties.Space(2));
                        DGUI.Label.Draw(UILabels.Value, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(m_triggerValue, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        GUILayout.Space(DGUI.Properties.Space(2));
                        DGUI.Label.Draw(UILabels.Tolerance, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(m_tolerance, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        GUILayout.Space(DGUI.Properties.Space());
                        break;
                    case CompareType.GreaterThan:
                    case CompareType.LessThan:
                    case CompareType.GreaterThanOrEqualTo:
                    case CompareType.LessThanOrEqualTo:
                        GUILayout.Space(DGUI.Properties.Space(2));
                        DGUI.Label.Draw(UILabels.Value, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(m_triggerValue, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        GUILayout.Space(DGUI.Properties.Space());
                        break;
                }

                GUILayout.Space(DGUI.Properties.Space());
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDisableTriggerAfterActivation() { DGUI.Toggle.Switch.Draw(m_disableTriggerAfterActivation, UILabels.DisableTriggerAfterActivation, ComponentColorName, true, true); }

        private void DrawResetOptions()
        {
            bool enabled = m_resetOnEnable.boolValue || m_resetOnDisable.boolValue || m_resetAfterDelay.boolValue;

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(enabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(enabled, ComponentColorName);

            float backgroundHeight = DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space();
            DGUI.Background.Draw(backgroundColorName, GUILayout.Height(backgroundHeight));
            GUILayout.Space(-backgroundHeight);
            GUILayout.Space(DGUI.Properties.Space());
            float alpha = GUI.color.a;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(2));
                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(enabled));
                DGUI.Label.Draw(UILabels.ResetTrigger, Size.S, textColorName);
                GUI.color = GUI.color.WithAlpha(alpha);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_resetOnEnable, UILabels.OnEnable, m_resetOnEnable.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_resetOnDisable, UILabels.OnDisable, m_resetOnDisable.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_resetAfterDelay, UILabels.AfterDelay, m_resetAfterDelay.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawResetDelay()
        {
            bool enabled = GUI.enabled;
            GUI.enabled = m_resetAfterDelay.boolValue;
            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.ResetDelay, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               EditorGUI.BeginChangeCheck();
                               DGUI.Property.Draw(m_resetDelay, ComponentColorName, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                               if (EditorGUI.EndChangeCheck()) DGUI.Properties.ResetKeyboardFocus();
                               DGUI.Label.Draw(UILabels.Seconds, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space(4));
                               DGUI.Toggle.Checkbox.Draw(m_useUnscaledTime, UILabels.UseUnscaledTime, ComponentColorName, false, false);
                               GUILayout.FlexibleSpace();
                           });

            GUI.enabled = enabled;
        }

        private void DrawActions()
        {
            ColorName currentColor = Target.IsActive ? ComponentColorName : ColorName.Red;
            
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconAction),
                                                      UILabels.Actions,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      currentColor,
                                                      currentColor);

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
                                                        currentColor,
                                                        DGUI.IconGroup.GetIcon(
                                                                               Target.Actions.HasSound,
                                                                               DGUI.IconGroup.IconSize,
                                                                               DGUI.Icon.Sound, DGUI.Icon.Sound,
                                                                               currentColor, DGUI.Colors.DisabledIconColorName)))
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
                                                          currentColor,
                                                          DGUI.IconGroup.GetIcon(
                                                                                 Target.Actions.HasEffect,
                                                                                 DGUI.IconGroup.IconSize,
                                                                                 DGUI.Icon.Effect, DGUI.Icon.Effect,
                                                                                 currentColor, DGUI.Colors.DisabledIconColorName)))
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
                                                          currentColor,
                                                          DGUI.IconGroup.GetIconWithCounter(
                                                                                            Target.Actions.HasAnimatorEvents,
                                                                                            Target.Actions.AnimatorEventsCount,
                                                                                            DGUI.IconGroup.IconSize,
                                                                                            DGUI.Icon.Animator, DGUI.Icon.Animator,
                                                                                            currentColor, DGUI.Colors.DisabledIconColorName)))
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
                                                          currentColor,
                                                          DGUI.IconGroup.GetIconWithCounter(
                                                                                            Target.Actions.HasGameEvents,
                                                                                            Target.Actions.GameEventsCount,
                                                                                            DGUI.IconGroup.IconSize,
                                                                                            DGUI.Icon.GameEvent, DGUI.Icon.GameEvent,
                                                                                            currentColor, DGUI.Colors.DisabledIconColorName)))
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
                                                         currentColor,
                                                         DGUI.IconGroup.GetIconWithCounter(
                                                                                           Target.Actions.HasUnityEvent,
                                                                                           Target.Actions.UnityEventListenerCount,
                                                                                           DGUI.IconGroup.IconSize,
                                                                                           DGUI.Icon.UnityEvent, DGUI.Icon.UnityEvent,
                                                                                           currentColor, DGUI.Colors.DisabledIconColorName)))
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
                                        currentColor);

            //DRAW ANIMATOR EVENTS
            if (m_animatorEventsExpanded.target)
                DGUI.List.DrawWithFade(animatorEventsProperty, m_animatorEventsExpanded, currentColor, UILabels.ListIsEmpty);

            //DRAW GAME EVENTS
            if (m_gameEventsExpanded.target)
                DGUI.List.DrawWithFade(gameEventsProperty, m_gameEventsExpanded, currentColor, UILabels.ListIsEmpty, 1);

            //DRAW EVENTS (UnityEvent)
            if (m_unityEventsExpanded.target)
                DGUI.Property.UnityEventWithFade(unityEventProperty, m_unityEventsExpanded, "Actions.Event", currentColor, Target.Actions.UnityEventListenerCount);
        }
    }
}