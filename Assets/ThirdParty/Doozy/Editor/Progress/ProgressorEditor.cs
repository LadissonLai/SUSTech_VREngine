// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using DG.DOTweenEditor;
using DG.Tweening;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using Doozy.Editor.Soundy;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(Progressor))]
    public class ProgressorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }
        private Progressor m_target;

        private Progressor Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (Progressor) target;
                return m_target;
            }
        }

        private const string SOME_PROGRESS_TARGETS_GET_UPDATED_ONLY_IN_PLAY_MODE = "PlayModeTarget";
        private bool m_showProgressTargetsSimulationMessage = false;
        
        private SerializedProperty
            m_progressTargets,
            m_onEnableResetValue,
            m_onDisableResetValue,
            m_customResetValue,
            m_animateValue,
            m_animationDuration,
            m_animationEase,
            m_animationIgnoresUnityTimescale,
            m_onValueChanged,
            m_onProgressChanged,
            m_onInverseProgressChanged,
            m_minValue,
            m_maxValue,
            m_wholeNumbers,
            m_currentValue;

        private AnimBool
            m_customResetValueExpanded,
            m_animateValueExpanded,
            m_onValueChangedExpanded,
            m_onProgressChangedExpanded,
            m_onInverseProgressChangedExpanded;

        private float m_previousProgress;

        private Tween m_simulatorTween;
        private float m_simulatorDuration = Progressor.DEFAULT_DURATION;
        private Ease m_simulatorEase = Progressor.DEFAULT_EASE;
        private float m_overrideCurrentValue;

        private bool ShowCustomResetValue
        {
            get
            {
                return (ResetValue) m_onEnableResetValue.enumValueIndex == ResetValue.ToCustomValue ||
                       (ResetValue) m_onDisableResetValue.enumValueIndex == ResetValue.ToCustomValue;
            }
        }

        private int OnValueChangedEventCount { get { return Target.OnValueChanged.GetPersistentEventCount(); } }
        private int OnProgressChangedEventCount { get { return Target.OnProgressChanged.GetPersistentEventCount(); } }
        private int OnInverseProgressChangedEventCount { get { return Target.OnInverseProgressChanged.GetPersistentEventCount(); } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_progressTargets = GetProperty(PropertyName.ProgressTargets);
            m_onEnableResetValue = GetProperty(PropertyName.OnEnableResetValue);
            m_onDisableResetValue = GetProperty(PropertyName.OnDisableResetValue);
            m_customResetValue = GetProperty(PropertyName.CustomResetValue);
            m_animateValue = GetProperty(PropertyName.AnimateValue);
            m_animationDuration = GetProperty(PropertyName.AnimationDuration);
            m_animationEase = GetProperty(PropertyName.AnimationEase);
            m_animationIgnoresUnityTimescale = GetProperty(PropertyName.AnimationIgnoresUnityTimescale);
            m_onValueChanged = GetProperty(PropertyName.OnValueChanged);
            m_onProgressChanged = GetProperty(PropertyName.OnProgressChanged);
            m_onInverseProgressChanged = GetProperty(PropertyName.OnInverseProgressChanged);
            m_minValue = GetProperty(PropertyName.m_minValue);
            m_maxValue = GetProperty(PropertyName.m_maxValue);
            m_wholeNumbers = GetProperty(PropertyName.m_wholeNumbers);
            m_currentValue = GetProperty(PropertyName.m_currentValue);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_customResetValueExpanded = GetAnimBool(m_customResetValue.propertyPath, ShowCustomResetValue);
            m_animateValueExpanded = GetAnimBool(m_animateValue.propertyPath, m_animateValue.boolValue);
            m_onValueChangedExpanded = GetAnimBool(m_onValueChanged.propertyPath, OnValueChangedEventCount > 0);
            m_onProgressChangedExpanded = GetAnimBool(m_onProgressChanged.propertyPath, OnProgressChangedEventCount > 0);
            m_onInverseProgressChangedExpanded = GetAnimBool(m_onInverseProgressChanged.propertyPath, OnInverseProgressChangedEventCount > 0);
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(SOME_PROGRESS_TARGETS_GET_UPDATED_ONLY_IN_PLAY_MODE, new InfoMessage(InfoMessage.MessageType.Info, UILabels.SomeProgressTargetsGetUpdatedOnlyInPlayMode, false, Repaint));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
            ResetSimulatorTween(true, false);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressor), MenuUtils.Progressor_Manual, MenuUtils.Progressor_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawProgressTargets();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawSimulator();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAnimateValue();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawWholeNumbers();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawResetValues();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawUnityEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();

            GetInfoMessage(SOME_PROGRESS_TARGETS_GET_UPDATED_ONLY_IN_PLAY_MODE).Draw(m_showProgressTargetsSimulationMessage, InspectorWidth);
            
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (m_previousProgress == Target.Progress) return;
            m_previousProgress = Target.Progress;
            Target.UpdateProgressTargets();
        }

        private void DrawProgressTargets()
        {
            DGUI.Doozy.DrawTitleWithIcon(Styles.GetStyle(Styles.StyleName.IconProgressTarget),
                                         UILabels.ProgressTargets, Size.L, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(3),
                                         ComponentColorName);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.List.Draw(m_progressTargets, ComponentColorName);
        }

        private void DrawSimulator()
        {
            float buttonHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(4);

            DrawProgressInfo();
            GUILayout.Space(DGUI.Properties.Space());
            DrawProgressBarAndCurrentValueSlider();
            GUILayout.Space(DGUI.Properties.Space());

            GUILayout.BeginHorizontal();
            {
                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaRepeat), UILabels.Simulate, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight, false))
                {
                    Simulate();
                }

                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Line.Draw(false, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.Duration, Size.M, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                                   GUI.color = DGUI.Colors.PropertyColor(ComponentColorName);
                                   m_simulatorDuration = EditorGUILayout.FloatField(m_simulatorDuration, GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                                   GUI.color = InitialGUIColor;
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.Ease, Size.M, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                                   GUI.color = DGUI.Colors.PropertyColor(ComponentColorName);
                                   m_simulatorEase = (Ease) EditorGUILayout.EnumPopup(m_simulatorEase, GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 3));
                                   GUI.color = InitialGUIColor;
                               });

                GUILayout.FlexibleSpace();

                if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaEdit), UILabels.SetValue, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight, false))
                {
                    SetValue(m_overrideCurrentValue);
                }

                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Line.Draw(false, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   GUI.color = DGUI.Colors.PropertyColor(ComponentColorName);
                                   m_overrideCurrentValue = EditorGUILayout.FloatField(m_overrideCurrentValue, GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                                   GUI.color = InitialGUIColor;
                               });
            }
            GUILayout.EndHorizontal();
        }

        private void DrawProgressBarAndCurrentValueSlider()
        {
            float barHeight = DGUI.Properties.Space(2);
            GUILayoutOption barHeightOption = GUILayout.Height(barHeight);
//            Color contentColor = DGUI.Utility.IsProSkin ? Color.white : Color.black;
            Color backgroundColor = DGUI.Utility.IsProSkin ? Color.black : Color.white;

            GUILayout.BeginVertical(barHeightOption);
            {
                GUI.color = backgroundColor.WithAlpha(0.2f);
                GUILayout.Label(GUIContent.none, DGUI.Properties.White, barHeightOption); //draw progress bar background
                GUILayout.Space(-barHeight);
                GUI.color = InitialGUIColor;
                GUI.color = DGUI.Colors.IconColor(ComponentColorName);
                float progressBarWidth = InspectorWidth * Target.Progress;
                GUILayout.Label(GUIContent.none, DGUI.Properties.White, barHeightOption, GUILayout.Width(progressBarWidth)); //draw progress bar (dynamic width)
                GUILayout.Space(-barHeight);
            }
            GUILayout.EndVertical();

            float currentValue = m_currentValue.floatValue;
            EditorGUI.BeginChangeCheck();
            currentValue = EditorGUILayout.Slider(currentValue, m_minValue.floatValue, m_maxValue.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                ResetSimulatorTween(true, false);
                SetValue(currentValue);
            }

            GUI.color = InitialGUIColor;
        }

        private void DrawProgressInfo()
        {
//            Color contentColor = DGUI.Utility.IsProSkin ? Color.white : Color.black;

            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginHorizontal(GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 2));
                {
                    DGUI.Property.Draw(m_minValue, UILabels.MinValue, ComponentColorName, ComponentColorName);
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                DGUI.Label.Draw(Math.Round(Target.Value, m_wholeNumbers.boolValue ? 0 : 3) + " (" + Mathf.Round(Target.Progress * 100) + "%)",
                                Size.M,
                                ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal(GUILayout.Width(DGUI.Properties.DefaultFieldWidth * 2));
                {
                    DGUI.Property.Draw(m_maxValue, UILabels.MaxValue, ComponentColorName, ComponentColorName);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAnimateValue()
        {
            m_animateValueExpanded.target = m_animateValue.boolValue;

            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_animateValue, UILabels.AnimateValue, ComponentColorName, true, false);
                if (DGUI.FadeOut.Begin(m_animateValueExpanded, false))
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(DGUI.Properties.Space());
                        GUILayout.BeginHorizontal(GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                        DGUI.Property.Draw(m_animationDuration, UILabels.Duration, ComponentColorName);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Property.Draw(m_animationEase, UILabels.Ease, ComponentColorName);
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Toggle.Switch.Draw(m_animationIgnoresUnityTimescale, UILabels.IgnoreTimescale, ComponentColorName, true, false);
                    }
                    GUILayout.EndHorizontal();
                }

                DGUI.FadeOut.End(m_animateValueExpanded, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawWholeNumbers() { DGUI.Toggle.Switch.Draw(m_wholeNumbers, UILabels.WholeNumbers, ComponentColorName, true, false); }

        private void DrawResetValues()
        {
            bool notDisabled = ResetValueEnabled(m_onEnableResetValue) ||
                               ResetValueEnabled(m_onDisableResetValue);

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(notDisabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(notDisabled, ComponentColorName);

            GUILayout.BeginHorizontal();
            {
                DGUI.Doozy.DrawTitleWithIcon(DGUI.Icon.Reset, UILabels.ResetValue, Size.M, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(), backgroundColorName);
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawResetValue(m_onEnableResetValue, "OnEnable");
                GUILayout.Space(DGUI.Properties.Space());
                DrawResetValue(m_onDisableResetValue, "OnDisable");
            }
            GUILayout.EndHorizontal();

            //CUSTOM RESET VALUE
            m_customResetValueExpanded.target = ShowCustomResetValue;
            if (DGUI.FadeOut.Begin(m_customResetValueExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(m_customResetValue, UILabels.CustomResetValue, backgroundColorName, textColorName);
            }

            DGUI.FadeOut.End(m_customResetValueExpanded, false);
        }

        private void DrawResetValue(SerializedProperty property, string propertyName)
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName((ResetValue) property.enumValueIndex != ResetValue.Disabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName((ResetValue) property.enumValueIndex != ResetValue.Disabled, ComponentColorName);
            DGUI.Property.Draw(property, propertyName, backgroundColorName, textColorName);
        }

        private void DrawUnityEvents()
        {
            DrawUnityEvent(m_onValueChangedExpanded, m_onValueChanged, "OnValueChanged", OnValueChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space());
            DrawUnityEvent(m_onProgressChangedExpanded, m_onProgressChanged, "OnProgressChanged", OnProgressChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space());
            DrawUnityEvent(m_onInverseProgressChangedExpanded, m_onInverseProgressChanged, "OnInverseProgressChanged", OnInverseProgressChangedEventCount);
        }

        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }

        private void ResetSimulatorTween(bool resetTweenTarget, bool resetProgress)
        {
            if (m_simulatorTween == null) return;
            m_simulatorTween.Kill(true);
            m_simulatorTween = null;
            if (resetProgress) Target.SetProgress(0, true);
            DOTweenEditorPreview.Stop(resetTweenTarget);
            Target.OnValueUpdated();
        }

        private void Simulate()
        {
            ResetSimulatorTween(false, true);
            m_simulatorTween = Target.GetAnimationTween(Target.MaxValue, m_simulatorDuration, m_simulatorEase, true)
                                     .OnComplete(() =>
                                     {
                                         ResetSimulatorTween(false, true);
                                         Target.OnValueUpdated();
                                     });
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(m_simulatorTween, false, false, true);
                DOTweenEditorPreview.Start();

                m_showProgressTargetsSimulationMessage = true;
            }
            else
            {
                m_simulatorTween.Play();
            }

            Target.OnValueUpdated();
        }

        private void SetValue(float value)
        {
            value = Target.ClampValueBetweenMinAndMax(value, Target.WholeNumbers);
            ResetSimulatorTween(false, false);
            m_simulatorTween = Target.GetAnimationTween(value,
                                                        Target.AnimateValue ? Target.AnimationDuration : 0,
                                                        Target.AnimateValue ? Target.AnimationEase : Progressor.DEFAULT_EASE,
                                                        Target.AnimateValue ? Target.AnimationIgnoresUnityTimescale : Progressor.DEFAULT_IGNORE_UNITY_TIMESCALE)
                                     .OnComplete(() =>
                                     {
                                         ResetSimulatorTween(false, false);
                                         Target.OnValueUpdated();
                                     });

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DOTweenEditorPreview.PrepareTweenForPreview(m_simulatorTween, false, false);
                DOTweenEditorPreview.Start();
//                if (Target.AnimateValue)
//                {
//                    DOTweenEditorPreview.PrepareTweenForPreview(m_simulatorTween, false, false);
//                    DOTweenEditorPreview.Start();
//                }
//                else
//                {
//                    Target.SetValue(value);
//                }
            }
            else
            {
                Target.SetValue(value);
            }

            Target.OnValueUpdated();
        }

        private static bool ResetValueEnabled(SerializedProperty property) { return (ResetValue) property.enumValueIndex != ResetValue.Disabled; }
    }
}