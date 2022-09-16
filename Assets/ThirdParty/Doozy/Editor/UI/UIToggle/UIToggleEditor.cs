// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Internal;
using Doozy.Editor.Soundy;
using Doozy.Editor.UI.Animation;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Input;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIToggle))]
    [CanEditMultipleObjects]
    public class UIToggleEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIToggleColorName; } }
        private UIToggle m_target;

        private UIToggle Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIToggle) target;
                return m_target;
            }
        }

        private static UIToggleSettings Settings { get { return UIToggleSettings.Instance; } }

        private static UIAnimations Animations { get { return UIAnimations.Instance; } }
        private UIAnimationsDatabase m_punchDatabase, m_stateDatabase;

        private readonly Dictionary<UIToggleBehavior, List<DGUI.IconGroup.Data>> m_behaviorAnimationIconsDatabase = new Dictionary<UIToggleBehavior, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIToggleBehavior, List<DGUI.IconGroup.Data>> m_buttonBehaviorIconsDatabase = new Dictionary<UIToggleBehavior, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIAction, List<DGUI.IconGroup.Data>> m_behaviorActionsIconsDatabase = new Dictionary<UIAction, List<DGUI.IconGroup.Data>>();

        private static string GetInfoMessageId(UIToggleBehaviorType type) { return type.ToString(); }

        private int OnValueChangedEventCount { get { return Target.OnValueChanged.GetPersistentEventCount(); } }

        private SerializedProperty
            m_allowMultipleClicks,
            m_disableButtonBetweenClicksInterval,
            m_deselectButtonAfterClick,
            m_onPointerEnter,
            m_onPointerExit,
            m_onClick,
            m_onSelected,
            m_onDeselected,
            m_inputData,
            m_toggleProgressor,
            m_onValueChanged;

        private AnimBool
            m_onPointerEnterExpanded,
            m_onPointerExitExpanded,
            m_onClickExpanded,
            m_onSelectedExpanded,
            m_onDeselectedExpanded,
            m_onValueChangedExpanded,
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded;

        private string GetRuntimePresetCategoryAndName(UIToggleBehavior behavior) { return string.Format(UILabels.RuntimePreset + ": {0} / {1}", behavior.PresetCategory, behavior.PresetName); }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_allowMultipleClicks = GetProperty(PropertyName.AllowMultipleClicks);
            m_disableButtonBetweenClicksInterval = GetProperty(PropertyName.DisableButtonBetweenClicksInterval);
            m_deselectButtonAfterClick = GetProperty(PropertyName.DeselectButtonAfterClick);
            m_onPointerEnter = GetProperty(PropertyName.OnPointerEnter);
            m_onPointerExit = GetProperty(PropertyName.OnPointerExit);
            m_onClick = GetProperty(PropertyName.OnClick);
            m_onSelected = GetProperty(PropertyName.OnSelected);
            m_onDeselected = GetProperty(PropertyName.OnDeselected);
            m_inputData = GetProperty(PropertyName.InputData);
            m_toggleProgressor = GetProperty(PropertyName.ToggleProgressor);
            m_onValueChanged = GetProperty(PropertyName.OnValueChanged);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_onPointerEnterExpanded = GetAnimBool(m_onPointerEnter.propertyPath, m_onPointerEnter.isExpanded);
            m_onPointerExitExpanded = GetAnimBool(m_onPointerExit.propertyPath, m_onPointerExit.isExpanded);
            m_onClickExpanded = GetAnimBool(m_onClick.propertyPath, m_onClick.isExpanded);
            m_onSelectedExpanded = GetAnimBool(m_onSelected.propertyPath, m_onSelected.isExpanded);
            m_onDeselectedExpanded = GetAnimBool(m_onDeselected.propertyPath, m_onDeselected.isExpanded);
            m_onValueChangedExpanded = GetAnimBool(m_onValueChanged.propertyPath, m_onValueChanged.isExpanded);

            m_soundDataExpanded = GetAnimBool("SOUND");
            m_effectExpanded = GetAnimBool("EFFECT");
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_target = (UIToggle) target;

            AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);

            m_punchDatabase = Animations.Get(AnimationType.Punch);
            m_stateDatabase = Animations.Get(AnimationType.State);

            AddInfoMessage(GetInfoMessageId(UIToggleBehaviorType.OnPointerEnter), new InfoMessage(DGUI.Icon.OnPointerEnter, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(GetInfoMessageId(UIToggleBehaviorType.OnPointerExit), new InfoMessage(DGUI.Icon.OnPointerExit, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(GetInfoMessageId(UIToggleBehaviorType.OnClick), new InfoMessage(DGUI.Icon.OnClick, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(GetInfoMessageId(UIToggleBehaviorType.OnSelected), new InfoMessage(DGUI.Icon.OnButtonSelect, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(GetInfoMessageId(UIToggleBehaviorType.OnDeselected), new InfoMessage(DGUI.Icon.OnButtonDeselect, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
            if (UIAnimatorUtils.PreviewIsPlaying) UIAnimatorUtils.StopButtonPreview(Target.RectTransform, Target.CanvasGroup);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIToggle), "UIToggle", MenuUtils.UIToggle_Manual, MenuUtils.UIToggle_YouTube);
            DrawDebugModeAndCreateParentAndCenterPivot();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawIsOn();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAllowMultipleClicks();
            GUILayout.Space(DGUI.Properties.Space());
            DrawDeselectButtonAfterClick();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawInputData();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawBehaviors();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawToggleProgressor();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawOnValueChanged();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebugModeAndCreateParentAndCenterPivot()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Doozy.DrawDebugMode(GetProperty(PropertyName.DebugMode), ColorName.Red);
                GUILayout.FlexibleSpace();
                DrawCreateParentAndCenterPivotButton();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawCreateParentAndCenterPivotButton()
        {
            if (!DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaExpand),
                                                    UILabels.CreateParentAndCenterPivot,
                                                    Size.S, TextAlign.Left,
                                                    DGUI.Colors.DisabledBackgroundColorName,
                                                    DGUI.Colors.DisabledTextColorName,
                                                    DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                    false)) return;

            var parent = new GameObject("Container - " + Target.name);
            Undo.RegisterCreatedObjectUndo(parent, "Create Parent and Center Pivot");
            var parentRectTransform = parent.AddComponent<RectTransform>();
            parent.transform.SetParent(Target.transform.parent, false);
            parentRectTransform.localRotation = Target.RectTransform.localRotation;
            parentRectTransform.localScale = Target.RectTransform.localScale;
            parentRectTransform.anchorMin = Target.RectTransform.anchorMin;
            parentRectTransform.anchorMax = Target.RectTransform.anchorMax;
            parentRectTransform.anchoredPosition = Target.RectTransform.anchoredPosition;
            parentRectTransform.sizeDelta = Target.RectTransform.sizeDelta;
            parentRectTransform.pivot = Target.RectTransform.pivot;
            Target.transform.SetParent(parent.transform);
            Target.RectTransform.anchorMin = Vector2.zero;
            Target.RectTransform.anchorMax = Vector2.one;
            Target.RectTransform.anchoredPosition = Vector2.zero;
            Target.RectTransform.sizeDelta = Vector2.zero;
            Target.RectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        private void DrawIsOn()
        {
            bool isOn = Target.IsOn;
            EditorGUI.BeginChangeCheck();
            isOn = DGUI.Toggle.Checkbox.Draw(isOn, "Is On", ComponentColorName, serializedObject.isEditingMultipleObjects, true, false);
            if (!EditorGUI.EndChangeCheck()) return;
            if (!serializedObject.isEditingMultipleObjects)
            {
                Undo.RecordObject(Target.Toggle, "Toggle " + (isOn ? "On" : "Off"));
                Target.IsOn = isOn;
                return;
            }

            var toggles = new Object[targets.Length];
            for (int i = 0; i < targets.Length; i++)
                toggles[i] = ((UIToggle) targets[i]).Toggle;

            Undo.RecordObjects(toggles, "Toggle " + (isOn ? "On" : "Off"));
            foreach (Object t in targets) ((UIToggle) t).IsOn = isOn;
        }

        private void DrawAllowMultipleClicks()
        {
            ColorName allowClickColor = DGUI.Colors.GetBackgroundColorName(m_allowMultipleClicks.boolValue, ComponentColorName);
            ColorName disableIntervalBackgroundColor = DGUI.Colors.GetBackgroundColorName(!m_allowMultipleClicks.boolValue, ComponentColorName);
            ColorName disableIntervalTextColor = DGUI.Colors.GetTextColorName(!m_allowMultipleClicks.boolValue, ComponentColorName);

            DGUI.Line.Draw(false,
                           () =>
                           {
                               DGUI.Toggle.Switch.Draw(m_allowMultipleClicks, UILabels.AllowMultipleClicks, allowClickColor, true, false);
                               GUILayout.Space(DGUI.Properties.Space());
                               bool enabledState = GUI.enabled;
                               GUI.enabled = !m_allowMultipleClicks.boolValue;
                               DGUI.Line.Draw(false, disableIntervalBackgroundColor,
                                              () =>
                                              {
                                                  GUILayout.Space(DGUI.Properties.Space(2));
                                                  float alpha = GUI.color.a;
                                                  GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(!m_allowMultipleClicks.boolValue));
                                                  DGUI.Label.Draw(UILabels.DisableButtonInterval, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                  DGUI.Property.Draw(m_disableButtonBetweenClicksInterval, disableIntervalBackgroundColor, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                                                  DGUI.Label.Draw(UILabels.Seconds, Size.S, disableIntervalTextColor, DGUI.Properties.SingleLineHeight);
                                                  GUILayout.Space(DGUI.Properties.Space(2));
                                                  GUI.color = GUI.color.WithAlpha(alpha);
                                              });
                               GUI.enabled = enabledState;
                           },
                           GUILayout.FlexibleSpace
                          );
        }

        private void DrawDeselectButtonAfterClick() { DGUI.Toggle.Switch.Draw(m_deselectButtonAfterClick, UILabels.DeselectButtonAfterClick, m_deselectButtonAfterClick.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, true, false); }

        private void DrawInputData()
        {
            SerializedProperty inputModeProperty = GetProperty(PropertyName.InputMode, m_inputData);
            var inputMode = (InputMode) inputModeProperty.enumValueIndex;
            SerializedProperty enableAlternateInputsProperty = GetProperty(PropertyName.EnableAlternateInputs, m_inputData);
            SerializedProperty keyCodeProperty = GetProperty(PropertyName.KeyCode, m_inputData);
            SerializedProperty keyCodeAltProperty = GetProperty(PropertyName.KeyCodeAlt, m_inputData);
            SerializedProperty virtualButtonNameProperty = GetProperty(PropertyName.VirtualButtonName, m_inputData);
            SerializedProperty virtualButtonNameAltProperty = GetProperty(PropertyName.VirtualButtonNameAlt, m_inputData);

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(inputMode != InputMode.None, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(inputMode != InputMode.None, ComponentColorName);

            AnimBool alternateExpanded = GetAnimBool(enableAlternateInputsProperty.propertyPath, inputMode != InputMode.None);
            alternateExpanded.target = inputMode != InputMode.None;

            DGUI.Line.Draw(false,
                           () =>
                           {
                               DGUI.Line.Draw(false, backgroundColorName,
                                              () =>
                                              {
                                                  GUILayout.Space(DGUI.Properties.Space(2));
                                                  DGUI.Label.Draw(UILabels.InputMode, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                                                  EditorGUI.BeginChangeCheck();
                                                  DGUI.Property.Draw(inputModeProperty, backgroundColorName, DGUI.Properties.SingleLineHeight);
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
                                                          DGUI.Property.Draw(keyCodeProperty, ComponentColorName, DGUI.Properties.SingleLineHeight);
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
                                                          DGUI.Property.Draw(virtualButtonNameProperty, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                          GUILayout.Space(DGUI.Properties.Space());
                                                      });
                                       break;
                               }
                           }
                          );

            if (DGUI.FadeOut.Begin(alternateExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Line.Draw(false,
                               () =>
                               {
                                   DGUI.Toggle.Checkbox.Draw(enableAlternateInputsProperty, UILabels.AlternateInput, ComponentColorName, true, false);
                               },
                               () =>
                               {
                                   bool enabled = GUI.enabled;
                                   GUI.enabled = enableAlternateInputsProperty.boolValue;

                                   backgroundColorName = !enableAlternateInputsProperty.boolValue ? DGUI.Colors.DisabledBackgroundColorName : ComponentColorName;
                                   textColorName = !enableAlternateInputsProperty.boolValue ? DGUI.Colors.DisabledTextColorName : ComponentColorName;

                                   switch (inputMode)
                                   {
                                       case InputMode.KeyCode:
                                           GUILayout.Space(DGUI.Properties.Space());
                                           DGUI.Line.Draw(false, backgroundColorName,
                                                          () =>
                                                          {
                                                              GUILayout.Space(DGUI.Properties.Space(2));
                                                              DGUI.Label.Draw(UILabels.AlternateKeyCode, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                                                              DGUI.Property.Draw(keyCodeAltProperty, backgroundColorName, DGUI.Properties.SingleLineHeight);
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
                                                              DGUI.Property.Draw(virtualButtonNameAltProperty, backgroundColorName, DGUI.Properties.SingleLineHeight);
                                                              GUILayout.Space(DGUI.Properties.Space());
                                                          });
                                           break;
                                   }

                                   GUI.enabled = enabled;
                               }
                              );
            }

            DGUI.FadeOut.End(alternateExpanded, false);
        }

        private void DrawBehaviors()
        {
            if (!Settings.ShowOnPointerEnter) Target.OnPointerEnter.Enabled = false;
            else DrawBehavior(UILabels.OnPointerEnter, Target.OnPointerEnter, m_onPointerEnter, DGUI.Icon.OnPointerEnter, m_onPointerEnterExpanded);
            if (!Settings.ShowOnPointerExit) Target.OnPointerExit.Enabled = false;
            else DrawBehavior(UILabels.OnPointerExit, Target.OnPointerExit, m_onPointerExit, DGUI.Icon.OnPointerExit, m_onPointerExitExpanded);

            if (Settings.ShowOnPointerEnter || Settings.ShowOnPointerExit) GUILayout.Space(DGUI.Properties.Space(4));

            if (!Settings.ShowOnClick) Target.OnClick.Enabled = false;
            else DrawBehavior(UILabels.OnClick, Target.OnClick, m_onClick, DGUI.Icon.OnClick, m_onClickExpanded);

            if (Settings.ShowOnClick) GUILayout.Space(DGUI.Properties.Space(4));

            if (!Settings.ShowOnButtonSelected) Target.OnSelected.Enabled = false;
            else DrawBehavior(UILabels.OnSelected, Target.OnSelected, m_onSelected, DGUI.Icon.OnButtonSelect, m_onSelectedExpanded);
            if (!Settings.ShowOnButtonDeselected) Target.OnDeselected.Enabled = false;
            else DrawBehavior(UILabels.OnDeselected, Target.OnDeselected, m_onDeselected, DGUI.Icon.OnButtonDeselect, m_onDeselectedExpanded);
        }

        private void DrawBehavior(string behaviorName, UIToggleBehavior toggleBehavior, SerializedProperty behaviorProperty, GUIStyle animationIcon, AnimBool behaviorExpanded)
        {
            SerializedProperty enabledProperty = GetProperty(PropertyName.Enabled, behaviorProperty);
            SerializedProperty triggerEventsAfterAnimationProperty = GetProperty(PropertyName.TriggerEventsAfterAnimation, behaviorProperty);
            SerializedProperty selectButtonProperty = GetProperty(PropertyName.SelectButton, behaviorProperty);
            SerializedProperty deselectButtonProperty = GetProperty(PropertyName.DeselectButton, behaviorProperty);
            SerializedProperty loadSelectedPresetAtRuntimeProperty = GetProperty(PropertyName.LoadSelectedPresetAtRuntime, behaviorProperty);
            SerializedProperty presetCategoryProperty = GetProperty(PropertyName.PresetCategory, behaviorProperty);
            SerializedProperty presetNameProperty = GetProperty(PropertyName.PresetName, behaviorProperty);
            SerializedProperty onToggleOnProperty = GetProperty(PropertyName.OnToggleOn, behaviorProperty);
            SerializedProperty onToggleOffProperty = GetProperty(PropertyName.OnToggleOff, behaviorProperty);
            SerializedProperty buttonAnimationTypeProperty = GetProperty(PropertyName.ButtonAnimationType, behaviorProperty);
            var buttonAnimationType = (ButtonAnimationType) buttonAnimationTypeProperty.enumValueIndex;

            AnimBool animationTypeExpanded = GetAnimBool(buttonAnimationTypeProperty.propertyPath, buttonAnimationTypeProperty.isExpanded);
            AnimBool onToggleOnExpanded = GetAnimBool(onToggleOnProperty.propertyPath, onToggleOnProperty.isExpanded);
            AnimBool onToggleOffExpanded = GetAnimBool(onToggleOffProperty.propertyPath, onToggleOffProperty.isExpanded);

            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(enabledProperty, ComponentColorName, NormalBarHeight);
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginVertical();
                {
                    bool enabledState = GUI.enabled;
                    GUI.enabled = enabledProperty.boolValue;
                    {
                        if (!enabledProperty.boolValue && behaviorExpanded.target) behaviorExpanded.target = false;
                        if (DGUI.Bar.Draw(behaviorName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, behaviorExpanded))
                        {
                            if (behaviorExpanded.target && !animationTypeExpanded.target)
                            {
                                animationTypeExpanded.target = true;
                                onToggleOnExpanded.value = false;
                                onToggleOffExpanded.value = false;
                            }

                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        if (enabledProperty.boolValue)
                        {
                            GUILayout.Space(-NormalBarHeight);
                            DrawButtonBehaviorIcons(toggleBehavior, behaviorExpanded);

                            InfoMessage presetInfoMessage = GetInfoMessage(toggleBehavior.BehaviorType.ToString());
                            presetInfoMessage.Message = GetRuntimePresetCategoryAndName(toggleBehavior);
                            GUILayout.Space(-DGUI.Properties.Space(2) * presetInfoMessage.Show.faded);
                            presetInfoMessage.DrawMessageOnly(toggleBehavior.LoadSelectedPresetAtRuntime);
                            GUILayout.Space(DGUI.Properties.Space(3) * presetInfoMessage.Show.faded);
                        }
                        else
                        {
                            GUILayout.Space(DGUI.Properties.Space());
                        }


                        if (DGUI.FadeOut.Begin(behaviorExpanded, false))
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);

                                bool addSpace = false;
                                switch (toggleBehavior.BehaviorType)
                                {
                                    case UIToggleBehaviorType.OnClick:
                                        DGUI.Toggle.Checkbox.Draw(triggerEventsAfterAnimationProperty, UILabels.TriggerEventsAfterAnimation, ComponentColorName, true, false);
                                        addSpace = true;
                                        break;
                                    case UIToggleBehaviorType.OnPointerEnter:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(selectButtonProperty, UILabels.SelectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawButtonBehaviorDisableInterval(behaviorProperty); }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIToggleBehaviorType.OnPointerExit:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(deselectButtonProperty, UILabels.DeselectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawButtonBehaviorDisableInterval(behaviorProperty); }
                                                      );
                                        addSpace = true;
                                        break;
                                }

                                if (addSpace) GUILayout.Space(DGUI.Properties.Space(4) * behaviorExpanded.faded);

                                switch (buttonAnimationType)
                                {
                                    case ButtonAnimationType.Punch:
                                        if (DGUI.FadeOut.Begin(animationTypeExpanded.faded, false))
                                        {
                                            if (UIAnimationUtils.DrawAnimationPreset(serializedObject,
                                                                                     loadSelectedPresetAtRuntimeProperty,
                                                                                     presetCategoryProperty,
                                                                                     presetNameProperty,
                                                                                     m_punchDatabase,
                                                                                     toggleBehavior.PunchAnimation,
                                                                                     ComponentColorName,
                                                                                     out toggleBehavior.PunchAnimation))
                                                if (serializedObject.isEditingMultipleObjects)
                                                    foreach (Object targetObject in serializedObject.targetObjects)
                                                    {
                                                        var targetButton = (UIToggle) targetObject;
                                                        switch (toggleBehavior.BehaviorType)
                                                        {
                                                            case UIToggleBehaviorType.OnClick:
                                                                targetButton.OnClick.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnClick.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnClick.PunchAnimation = toggleBehavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnPointerEnter:
                                                                targetButton.OnPointerEnter.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnPointerEnter.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnPointerEnter.PunchAnimation = toggleBehavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnPointerExit:
                                                                targetButton.OnPointerExit.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnPointerExit.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnPointerExit.PunchAnimation = toggleBehavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnSelected:
                                                                targetButton.OnSelected.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnSelected.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnSelected.PunchAnimation = toggleBehavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnDeselected:
                                                                targetButton.OnDeselected.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnDeselected.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnDeselected.PunchAnimation = toggleBehavior.PunchAnimation.Copy();
                                                                break;
                                                        }
                                                    }

                                            GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);
                                        }

                                        DGUI.FadeOut.End(animationTypeExpanded.faded, false);
                                        break;
                                    case ButtonAnimationType.State:
                                        if (DGUI.FadeOut.Begin(animationTypeExpanded.faded, false))
                                        {
                                            if (UIAnimationUtils.DrawAnimationPreset(serializedObject,
                                                                                     loadSelectedPresetAtRuntimeProperty,
                                                                                     presetCategoryProperty,
                                                                                     presetNameProperty,
                                                                                     m_stateDatabase,
                                                                                     toggleBehavior.StateAnimation,
                                                                                     ComponentColorName,
                                                                                     out toggleBehavior.StateAnimation))
                                                if (serializedObject.isEditingMultipleObjects)
                                                    foreach (Object targetObject in serializedObject.targetObjects)
                                                    {
                                                        var targetButton = (UIToggle) targetObject;
                                                        switch (toggleBehavior.BehaviorType)
                                                        {
                                                            case UIToggleBehaviorType.OnClick:
                                                                targetButton.OnClick.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnClick.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnClick.StateAnimation = toggleBehavior.StateAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnPointerEnter:
                                                                targetButton.OnPointerEnter.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnPointerEnter.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnPointerEnter.StateAnimation = toggleBehavior.StateAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnPointerExit:
                                                                targetButton.OnPointerExit.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnPointerExit.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnPointerExit.StateAnimation = toggleBehavior.StateAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnSelected:
                                                                targetButton.OnSelected.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnSelected.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnSelected.StateAnimation = toggleBehavior.StateAnimation.Copy();
                                                                break;
                                                            case UIToggleBehaviorType.OnDeselected:
                                                                targetButton.OnDeselected.PresetCategory = toggleBehavior.PresetCategory;
                                                                targetButton.OnDeselected.PresetName = toggleBehavior.PresetName;
                                                                targetButton.OnDeselected.StateAnimation = toggleBehavior.StateAnimation.Copy();
                                                                break;
                                                        }
                                                    }

                                            GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);
                                        }

                                        DGUI.FadeOut.End(animationTypeExpanded.faded, false);
                                        break;
                                    case ButtonAnimationType.Animator:

                                        break;
                                }


                                GUILayout.BeginHorizontal();
                                {
                                    if (DGUI.Doozy.DrawSectionButtonLeft(animationTypeExpanded.target,
                                                                         UILabels.Animation,
                                                                         DGUI.Icon.Animations,
                                                                         ComponentColorName,
                                                                         DGUI.Doozy.GetToggleBehaviorAnimationIcons(toggleBehavior,
                                                                                                                    m_behaviorAnimationIconsDatabase,
                                                                                                                    ComponentColorName)))
                                    {
                                        animationTypeExpanded.target = true;
                                        onToggleOnExpanded.value = false;
                                        onToggleOffExpanded.value = false;
                                        SoundyAudioPlayer.StopAllPlayers();
                                    }

                                    GUILayout.Space(DGUI.Properties.Space());

                                    if (DGUI.Doozy.DrawSectionButtonMiddle(onToggleOnExpanded.target,
                                                                           UILabels.ToggleON,
                                                                           DGUI.Icon.ToggleOn,
                                                                           DGUI.Colors.UIToggleOnColorName,
                                                                           DGUI.Doozy.GetActionsIcons(toggleBehavior.OnToggleOn,
                                                                                                      m_behaviorActionsIconsDatabase,
                                                                                                      DGUI.Colors.UIToggleOnColorName)))
                                    {
                                        animationTypeExpanded.value = false;
                                        onToggleOnExpanded.target = true;
                                        onToggleOffExpanded.value = false;
                                        SoundyAudioPlayer.StopAllPlayers();
                                    }

                                    GUILayout.Space(DGUI.Properties.Space());

                                    if (DGUI.Doozy.DrawSectionButtonRight(onToggleOffExpanded.target,
                                                                          UILabels.ToggleOFF,
                                                                          DGUI.Icon.ToggleOff,
                                                                          DGUI.Colors.UIToggleOffColorName,
                                                                          DGUI.Doozy.GetActionsIcons(toggleBehavior.OnToggleOff,
                                                                                                     m_behaviorActionsIconsDatabase,
                                                                                                     DGUI.Colors.UIToggleOffColorName)))
                                    {
                                        animationTypeExpanded.value = false;
                                        onToggleOnExpanded.value = false;
                                        onToggleOffExpanded.target = true;
                                        SoundyAudioPlayer.StopAllPlayers();
                                    }
                                }
                                GUILayout.EndHorizontal();

                                buttonAnimationTypeProperty.isExpanded = animationTypeExpanded.target;
                                onToggleOnProperty.isExpanded = onToggleOnExpanded.target;
                                onToggleOffProperty.isExpanded = onToggleOffExpanded.target;

                                GUILayout.Space(DGUI.Properties.Space(3) * behaviorExpanded.faded);

                                DrawButtonBehaviorAnimation(toggleBehavior, behaviorProperty);

                                if (DGUI.FadeOut.Begin(onToggleOnExpanded, false))
                                {
                                    DGUI.Doozy.DrawTitleWithIconAndBackground(DGUI.Icon.ToggleOn, UILabels.ToggleON, Size.L, NormalBarHeight, DGUI.Colors.UIToggleOnColorName, DGUI.Colors.UIToggleOnColorName);
                                    GUILayout.Space(DGUI.Properties.Space() * onToggleOnExpanded.faded);
                                    DrawBehaviorActions(toggleBehavior.OnToggleOn, onToggleOnProperty, onToggleOnExpanded, behaviorName + "." + PropertyName.OnToggleOn, DGUI.Colors.UIToggleOnColorName);
                                    GUILayout.Space(DGUI.Properties.Space(2) * onToggleOnExpanded.faded);
                                }

                                DGUI.FadeOut.End(onToggleOnExpanded, false);


                                if (DGUI.FadeOut.Begin(onToggleOffExpanded, false))
                                {
                                    DGUI.Doozy.DrawTitleWithIconAndBackground(DGUI.Icon.ToggleOff, UILabels.ToggleOFF, Size.L, NormalBarHeight, DGUI.Colors.UIToggleOffColorName, DGUI.Colors.UIToggleOffColorName);
                                    GUILayout.Space(DGUI.Properties.Space() * onToggleOffExpanded.faded);
                                    DrawBehaviorActions(toggleBehavior.OnToggleOff, onToggleOffProperty, onToggleOffExpanded, behaviorName + "." + PropertyName.OnToggleOff, DGUI.Colors.UIToggleOffColorName);
                                }

                                DGUI.FadeOut.End(onToggleOffExpanded, false);

                                GUILayout.Space(DGUI.Properties.Space(4) * behaviorExpanded.faded);
                            }
                            GUILayout.EndVertical();
                        }

                        DGUI.FadeOut.End(behaviorExpanded);
                    }
                    GUI.enabled = enabledState;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            behaviorProperty.isExpanded = behaviorExpanded.target;
        }

        private void DrawButtonBehaviorIcons(UIToggleBehavior behavior, AnimBool expanded)
        {
            if (DGUI.AlphaGroup.Begin(Mathf.Clamp(1 - expanded.faded, 0.2f, 1f)))
            {
                GUILayout.BeginHorizontal(GUILayout.Height(NormalBarHeight));
                {
                    GUILayout.FlexibleSpace();

                    if (!m_buttonBehaviorIconsDatabase.ContainsKey(behavior)) m_buttonBehaviorIconsDatabase.Add(behavior, new List<DGUI.IconGroup.Data>());

                    switch (behavior.ButtonAnimationType)
                    {
                        case ButtonAnimationType.Punch:
                            //GET PunchAnimation Icons
                            m_buttonBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorUIAnimationIcons(m_buttonBehaviorIconsDatabase[behavior],
                                                                                                             AnimationType.Punch,
                                                                                                             behavior.PunchAnimation.Move.Enabled,
                                                                                                             behavior.PunchAnimation.Rotate.Enabled,
                                                                                                             behavior.PunchAnimation.Scale.Enabled,
                                                                                                             false,
                                                                                                             ComponentColorName);
                            break;
                        case ButtonAnimationType.State:
                            //GET StateAnimation Icons
                            m_buttonBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorUIAnimationIcons(m_buttonBehaviorIconsDatabase[behavior],
                                                                                                             AnimationType.State,
                                                                                                             behavior.StateAnimation.Move.Enabled,
                                                                                                             behavior.StateAnimation.Rotate.Enabled,
                                                                                                             behavior.StateAnimation.Scale.Enabled,
                                                                                                             behavior.StateAnimation.Fade.Enabled,
                                                                                                             ComponentColorName);

                            break;
                        case ButtonAnimationType.Animator:
                            m_buttonBehaviorIconsDatabase[behavior].Clear();
                            if (behavior.HasAnimators)
                            {
                                m_buttonBehaviorIconsDatabase[behavior].Add(new DGUI.IconGroup.Data(behavior.AnimatorsCount.ToString(), ComponentColorName));
                                m_buttonBehaviorIconsDatabase[behavior].Add(new DGUI.IconGroup.Data(true, DGUI.IconGroup.IconSize, DGUI.Icon.Animator, DGUI.Icon.Animator, ComponentColorName, DGUI.Colors.DisabledIconColorName));
                            }

                            break;
                    }

                    //DRAW Animation Icons
                    if (m_buttonBehaviorIconsDatabase[behavior].Count > 0)
                        DGUI.IconGroup.Draw(m_buttonBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);

                    //GET Actions Icons
                    m_buttonBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorActionsIcons(m_buttonBehaviorIconsDatabase[behavior],
                                                                                                 behavior.HasSound,
                                                                                                 behavior.HasEffect,
                                                                                                 behavior.HasAnimatorEvents,
                                                                                                 behavior.HasGameEvents,
                                                                                                 behavior.HasUnityEvents,
                                                                                                 ComponentColorName);

                    //DRAW Actions Icons
                    if (m_buttonBehaviorIconsDatabase[behavior].Count > 0)
                    {
                        GUILayout.Space(DGUI.Properties.Space(4));
                        DGUI.IconGroup.Draw(m_buttonBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);
                    }

                    GUILayout.Space(DGUI.Properties.Space(3));
                }
                GUILayout.EndHorizontal();
            }

            DGUI.AlphaGroup.End();
        }

        private void DrawButtonBehaviorAnimation(UIToggleBehavior behavior, SerializedProperty behaviorProperty)
        {
            SerializedProperty buttonAnimationType = GetProperty(PropertyName.ButtonAnimationType, behaviorProperty);
            AnimBool animationTypeExpanded = GetAnimBool(buttonAnimationType.propertyPath);
            var selectedAnimationType = (ButtonAnimationType) buttonAnimationType.enumValueIndex;

            if (DGUI.FadeOut.Begin(animationTypeExpanded, false))
            {
                GUILayout.BeginVertical();
                {
                    DrawBehaviorAnimationTypeSelector(behavior, buttonAnimationType);

                    GUILayout.Space(DGUI.Properties.Space(2) * animationTypeExpanded.faded);

                    DGUI.Doozy.DrawPreviewAnimationButtons(serializedObject, Target, behavior, ComponentColorName);

                    GUILayout.Space(DGUI.Properties.Space(2) * animationTypeExpanded.faded);

                    switch (selectedAnimationType)
                    {
                        case ButtonAnimationType.Punch:
                            DrawBehaviorAnimationsProperty(GetProperty(PropertyName.PunchAnimation, behaviorProperty), selectedAnimationType);
                            break;
                        case ButtonAnimationType.State:
                            DrawBehaviorAnimationsProperty(GetProperty(PropertyName.StateAnimation, behaviorProperty), selectedAnimationType);
                            break;
                        case ButtonAnimationType.Animator:
                            DrawBehaviorAnimationsProperty(GetProperty(PropertyName.Animators, behaviorProperty), selectedAnimationType);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(animationTypeExpanded);
        }

        private void DrawBehaviorAnimationTypeSelector(UIToggleBehavior behavior, SerializedProperty buttonAnimationTypeProperty)
        {
            float iconSize = 20f;
            GUIStyle iconStyle = null;
            bool enabled = false;
            switch (behavior.ButtonAnimationType)
            {
                case ButtonAnimationType.Punch:
                    iconStyle = DGUI.Icon.PunchAnimation;
                    enabled = behavior.HasPunchAnimation;
                    break;
                case ButtonAnimationType.State:
                    iconStyle = DGUI.Icon.StateAnimation;
                    enabled = behavior.HasStateAnimation;

                    break;
                case ButtonAnimationType.Animator:
                    iconStyle = DGUI.Icon.Animator;
                    enabled = behavior.HasAnimators;
                    break;
            }

            float height = iconSize + DGUI.Properties.Space(3);

            ColorName colorName = enabled ? ComponentColorName : DGUI.Utility.IsProSkin ? ColorName.Gray : ColorName.White;
            ColorName iconColorName = enabled ? ComponentColorName : ColorName.Gray;

            DGUI.Line.Draw(false, colorName, height,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(4));
                               Color color = GUI.color;
                               GUI.color = DGUI.Colors.TextColor(iconColorName);
                               DGUI.Icon.Draw(iconStyle, iconSize, height);
                               GUI.color = color;
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Property.Draw(buttonAnimationTypeProperty, colorName, height);
                               GUILayout.Space(DGUI.Properties.Space());
                           });
        }

        private void DrawBehaviorAnimationsProperty(SerializedProperty property, ButtonAnimationType animationType)
        {
            if (animationType == ButtonAnimationType.Animator)
            {
                DGUI.List.Draw(property, ComponentColorName);
                GUILayout.Space(DGUI.Properties.Space(3));
                return;
            }

            SerializedProperty move = GetProperty(PropertyName.Move, property);
            SerializedProperty rotate = GetProperty(PropertyName.Rotate, property);
            SerializedProperty scale = GetProperty(PropertyName.Scale, property);
            AnimBool moveExpanded = GetAnimBool(move.propertyPath);
            AnimBool rotateExpanded = GetAnimBool(rotate.propertyPath);
            AnimBool scaleExpanded = GetAnimBool(scale.propertyPath);
            moveExpanded.target = GetProperty(PropertyName.Enabled, move).boolValue;
            rotateExpanded.target = GetProperty(PropertyName.Enabled, rotate).boolValue;
            scaleExpanded.target = GetProperty(PropertyName.Enabled, scale).boolValue;
            SerializedProperty fade = null;
            AnimBool fadeExpanded = null;

            const Size barSize = Size.M;

            GUILayout.BeginHorizontal();
            {
                if (DGUI.Bar.Draw(UILabels.Move,
                                  barSize,
                                  DGUI.Bar.Caret.CaretType.Move,
                                  moveExpanded.target ? DGUI.Colors.MoveColorName : DGUI.Colors.DisabledTextColorName,
                                  moveExpanded))
                    GetProperty(PropertyName.Enabled, move).boolValue = !GetProperty(PropertyName.Enabled, move).boolValue;

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Bar.Draw(UILabels.Rotate,
                                  barSize,
                                  DGUI.Bar.Caret.CaretType.Rotate,
                                  rotateExpanded.target ? DGUI.Colors.RotateColorName : DGUI.Colors.DisabledTextColorName,
                                  rotateExpanded))
                    GetProperty(PropertyName.Enabled, rotate).boolValue = !GetProperty(PropertyName.Enabled, rotate).boolValue;

                GUILayout.Space(DGUI.Properties.Space());

                if (DGUI.Bar.Draw(UILabels.Scale,
                                  barSize,
                                  DGUI.Bar.Caret.CaretType.Scale,
                                  scaleExpanded.target ? DGUI.Colors.ScaleColorName : DGUI.Colors.DisabledTextColorName,
                                  scaleExpanded))
                    GetProperty(PropertyName.Enabled, scale).boolValue = !GetProperty(PropertyName.Enabled, scale).boolValue;

                if (animationType == ButtonAnimationType.State)
                {
                    GUILayout.Space(DGUI.Properties.Space());

                    fade = GetProperty(PropertyName.Fade, property);
                    fadeExpanded = GetAnimBool(fade.propertyPath);
                    fadeExpanded.target = GetProperty(PropertyName.Enabled, fade).boolValue;
                    if (DGUI.Bar.Draw(UILabels.Fade, barSize, DGUI.Bar.Caret.CaretType.Fade, fadeExpanded.target ? DGUI.Colors.FadeColorName : DGUI.Colors.DisabledTextColorName, fadeExpanded))
                        GetProperty(PropertyName.Enabled, fade).boolValue = !GetProperty(PropertyName.Enabled, fade).boolValue;
                }
            }

            GUILayout.EndHorizontal();

            DGUI.Property.DrawWithFade(move, moveExpanded);
            DGUI.Property.DrawWithFade(rotate, rotateExpanded);
            DGUI.Property.DrawWithFade(scale, scaleExpanded);
            if (animationType == ButtonAnimationType.State)
                DGUI.Property.DrawWithFade(fade, fadeExpanded);
        }

        private void DrawBehaviorActions(UIAction actions, SerializedProperty actionsProperty, AnimBool expanded, string unityEventDisplayPath, ColorName colorName)
        {
            float alpha = GUI.color.a;
            if (DGUI.FadeOut.Begin(expanded, false))
            {
                SerializedProperty soundDataProperty = GetProperty(PropertyName.SoundData, actionsProperty);
                SerializedProperty effectProperty = GetProperty(PropertyName.Effect, actionsProperty);
                SerializedProperty animatorEventsProperty = GetProperty(PropertyName.AnimatorEvents, actionsProperty);
                SerializedProperty gameEventsProperty = GetProperty(PropertyName.GameEvents, actionsProperty);
                SerializedProperty unityEventProperty = GetProperty(PropertyName.Event, actionsProperty);

                if (!expanded.target)
                {
                    m_soundDataExpanded.target = false;
                    m_effectExpanded.target = false;
                    m_animatorEventsExpanded.target = false;
                    m_gameEventsExpanded.target = false;
                    m_unityEventsExpanded.target = false;
                }
                else
                {
                    if (!m_soundDataExpanded.target && !m_effectExpanded.target && !m_animatorEventsExpanded.target && !m_gameEventsExpanded.target && !m_unityEventsExpanded.target)
                        m_soundDataExpanded.target = true;
                }

                GUILayout.BeginHorizontal();
                {
                    if (DGUI.Doozy.DrawSubSectionButtonLeft(m_soundDataExpanded.target,
                                                            UILabels.Sound,
                                                            colorName,
                                                            DGUI.IconGroup.GetIcon(
                                                                                   actions.HasSound,
                                                                                   DGUI.IconGroup.IconSize,
                                                                                   DGUI.Icon.Sound, DGUI.Icon.Sound,
                                                                                   colorName, DGUI.Colors.DisabledIconColorName)))
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
                                                              colorName,
                                                              DGUI.IconGroup.GetIcon(
                                                                                     actions.HasEffect,
                                                                                     DGUI.IconGroup.IconSize,
                                                                                     DGUI.Icon.Effect, DGUI.Icon.Effect,
                                                                                     colorName, DGUI.Colors.DisabledIconColorName)))
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
                                                              colorName,
                                                              DGUI.IconGroup.GetIconWithCounter(
                                                                                                actions.HasAnimatorEvents,
                                                                                                actions.AnimatorEventsCount,
                                                                                                DGUI.IconGroup.IconSize,
                                                                                                DGUI.Icon.Animator, DGUI.Icon.Animator,
                                                                                                colorName, DGUI.Colors.DisabledIconColorName)))
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
                                                              colorName,
                                                              DGUI.IconGroup.GetIconWithCounter(
                                                                                                actions.HasGameEvents,
                                                                                                actions.GameEventsCount,
                                                                                                DGUI.IconGroup.IconSize,
                                                                                                DGUI.Icon.GameEvent, DGUI.Icon.GameEvent,
                                                                                                colorName, DGUI.Colors.DisabledIconColorName)))
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
                                                             colorName,
                                                             DGUI.IconGroup.GetIconWithCounter(
                                                                                               actions.HasUnityEvent,
                                                                                               actions.UnityEventListenerCount,
                                                                                               DGUI.IconGroup.IconSize,
                                                                                               DGUI.Icon.UnityEvent, DGUI.Icon.UnityEvent,
                                                                                               colorName, DGUI.Colors.DisabledIconColorName)))
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
                                            actions.Effect,
                                            GetProperty(PropertyName.ParticleSystem, effectProperty),
                                            GetProperty(PropertyName.Behavior, effectProperty),
                                            GetProperty(PropertyName.StopBehavior, effectProperty),
                                            GetProperty(PropertyName.AutoSort, effectProperty),
                                            GetProperty(PropertyName.SortingSteps, effectProperty),
                                            GetProperty(PropertyName.CustomSortingLayer, effectProperty),
                                            GetProperty(PropertyName.CustomSortingOrder, effectProperty),
                                            m_effectExpanded,
                                            colorName);

                //DRAW ANIMATOR EVENTS
                if (m_animatorEventsExpanded.target)
                    DGUI.List.DrawWithFade(animatorEventsProperty, m_animatorEventsExpanded, colorName, UILabels.ListIsEmpty);

                //DRAW GAME EVENTS
                if (m_gameEventsExpanded.target)
                    DGUI.List.DrawWithFade(gameEventsProperty, m_gameEventsExpanded, colorName, UILabels.ListIsEmpty, 1);

                //DRAW EVENTS (UnityEvent)
                if (m_unityEventsExpanded.target)
                    DGUI.Property.UnityEventWithFade(unityEventProperty, m_unityEventsExpanded, unityEventDisplayPath + ".Event", colorName, actions.UnityEventListenerCount);
            }

            DGUI.FadeOut.End(expanded, false, alpha);
        }

        private void DrawButtonBehaviorDisableInterval(SerializedProperty behavior)
        {
            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.DisableInterval, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               DGUI.Property.Draw(GetProperty(PropertyName.DisableInterval, behavior), ComponentColorName, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                               DGUI.Label.Draw(UILabels.Seconds, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawToggleProgressor() { DGUI.Property.Draw(m_toggleProgressor, UILabels.ToggleProgressor, ComponentColorName); }

        private void DrawOnValueChanged() { DrawUnityEvent(m_onValueChangedExpanded, m_onValueChanged, "OnValueChanged", OnValueChangedEventCount); }

        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }
    }
}