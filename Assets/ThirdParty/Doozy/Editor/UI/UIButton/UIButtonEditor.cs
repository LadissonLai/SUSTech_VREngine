// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Animation;
using Doozy.Editor.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Input;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using Doozy.Editor.Soundy;
using Doozy.Engine.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using PropertyName = Doozy.Editor.PropertyName;
using Styles = Doozy.Editor.Styles;

#if dUI_TextMeshPro
using TMPro;
#endif

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIButton))]
    [CanEditMultipleObjects]
    public class UIButtonEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIButtonColorName; } }
        private UIButton m_target;

        private UIButton Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIButton) target;
                return m_target;
            }
        }

        private static UIButtonSettings Settings { get { return UIButtonSettings.Instance; } }
        private static NamesDatabase Database { get { return UIButtonSettings.Database; } }

        private static UIAnimations Animations { get { return UIAnimations.Instance; } }
        private UIAnimationsDatabase m_punchDatabase, m_stateDatabase, m_loopDatabase;

        private readonly Dictionary<UIButtonBehavior, List<DGUI.IconGroup.Data>> m_behaviorAnimationIconsDatabase = new Dictionary<UIButtonBehavior, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIButtonBehavior, List<DGUI.IconGroup.Data>> m_buttonBehaviorIconsDatabase = new Dictionary<UIButtonBehavior, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIAction, List<DGUI.IconGroup.Data>> m_behaviorActionsIconsDatabase = new Dictionary<UIAction, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIButtonLoopAnimation, List<DGUI.IconGroup.Data>> m_buttonLoopAnimationIconsDatabase = new Dictionary<UIButtonLoopAnimation, List<DGUI.IconGroup.Data>>();

        private SerializedProperty
            m_buttonCategory,
            m_buttonName,
            m_allowMultipleClicks,
            m_disableButtonBetweenClicksInterval,
            m_deselectButtonAfterClick,
            m_clickMode,
            m_doubleClickRegisterInterval,
            m_longClickRegisterInterval,
            m_onPointerEnter,
            m_onPointerExit,
            m_onPointerDown,
            m_onPointerUp,
            m_onClick,
            m_onDoubleClick,
            m_onLongClick,
            m_onRightClick,
            m_onSelected,
            m_onDeselected,
            m_normalLoopAnimation,
            m_selectedLoopAnimation,
            m_inputData,
            m_targetLabel,
            m_textLabel,
            m_textMeshProLabel;

        private AnimBool
            m_onPointerEnterExpanded,
            m_onPointerExitExpanded,
            m_onPointerDownExpanded,
            m_onPointerUpExpanded,
            m_onClickExpanded,
            m_onDoubleClickExpanded,
            m_onLongClickExpanded,
            m_onRightClickExpanded,
            m_onSelectedExpanded,
            m_onDeselectedExpanded,
            m_normalLoopAnimationExpanded,
            m_selectedLoopAnimationExpanded,
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded;

        private SerializedObject m_textSerializedObject;
        private SerializedProperty m_textProperty;

        private Image m_targetImage;
        private string m_buttonLabel;
        private Color m_imageColor, m_textColor;

        private string GetRuntimePresetCategoryAndName(UIButtonBehavior behavior) { return string.Format(UILabels.RuntimePreset + ": {0} / {1}", behavior.PresetCategory, behavior.PresetName); }
        private string GetRuntimePresetCategoryAndName(UIButtonLoopAnimation loopAnimation) { return string.Format(UILabels.RuntimePreset + ": {0} / {1}", loopAnimation.PresetCategory, loopAnimation.PresetName); }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_buttonCategory = GetProperty(PropertyName.ButtonCategory);
            m_buttonName = GetProperty(PropertyName.ButtonName);
            m_allowMultipleClicks = GetProperty(PropertyName.AllowMultipleClicks);
            m_disableButtonBetweenClicksInterval = GetProperty(PropertyName.DisableButtonBetweenClicksInterval);
            m_deselectButtonAfterClick = GetProperty(PropertyName.DeselectButtonAfterClick);
            m_clickMode = GetProperty(PropertyName.ClickMode);
            m_doubleClickRegisterInterval = GetProperty(PropertyName.DoubleClickRegisterInterval);
            m_longClickRegisterInterval = GetProperty(PropertyName.LongClickRegisterInterval);
            m_onPointerEnter = GetProperty(PropertyName.OnPointerEnter);
            m_onPointerExit = GetProperty(PropertyName.OnPointerExit);
            m_onPointerDown = GetProperty(PropertyName.OnPointerDown);
            m_onPointerUp = GetProperty(PropertyName.OnPointerUp);
            m_onClick = GetProperty(PropertyName.OnClick);
            m_onDoubleClick = GetProperty(PropertyName.OnDoubleClick);
            m_onLongClick = GetProperty(PropertyName.OnLongClick);
            m_onRightClick = GetProperty(PropertyName.OnRightClick);
            m_onSelected = GetProperty(PropertyName.OnSelected);
            m_onDeselected = GetProperty(PropertyName.OnDeselected);
            m_normalLoopAnimation = GetProperty(PropertyName.NormalLoopAnimation);
            m_selectedLoopAnimation = GetProperty(PropertyName.SelectedLoopAnimation);
            m_inputData = GetProperty(PropertyName.InputData);
            m_targetLabel = GetProperty(PropertyName.TargetLabel);
            m_textLabel = GetProperty(PropertyName.TextLabel);

#if dUI_TextMeshPro
            m_textMeshProLabel = GetProperty(PropertyName.TextMeshProLabel);
#endif
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_onPointerEnterExpanded = GetAnimBool(m_onPointerEnter.propertyPath, m_onPointerEnter.isExpanded);
            m_onPointerExitExpanded = GetAnimBool(m_onPointerExit.propertyPath, m_onPointerExit.isExpanded);
            m_onPointerDownExpanded = GetAnimBool(m_onPointerDown.propertyPath, m_onPointerDown.isExpanded);
            m_onPointerUpExpanded = GetAnimBool(m_onPointerUp.propertyPath, m_onPointerUp.isExpanded);
            m_onClickExpanded = GetAnimBool(m_onClick.propertyPath, m_onClick.isExpanded);
            m_onDoubleClickExpanded = GetAnimBool(m_onDoubleClick.propertyPath, m_onDoubleClick.isExpanded);
            m_onLongClickExpanded = GetAnimBool(m_onLongClick.propertyPath, m_onLongClick.isExpanded);
            m_onRightClickExpanded = GetAnimBool(m_onRightClick.propertyPath, m_onRightClick.isExpanded);
            m_onSelectedExpanded = GetAnimBool(m_onSelected.propertyPath, m_onSelected.isExpanded);
            m_onDeselectedExpanded = GetAnimBool(m_onDeselected.propertyPath, m_onDeselected.isExpanded);
            m_normalLoopAnimationExpanded = GetAnimBool(m_normalLoopAnimation.propertyPath, m_normalLoopAnimation.isExpanded);
            m_selectedLoopAnimationExpanded = GetAnimBool(m_selectedLoopAnimation.propertyPath, m_selectedLoopAnimation.isExpanded);

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
            m_target = (UIButton) target;

            AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);

            m_punchDatabase = Animations.Get(AnimationType.Punch);
            m_stateDatabase = Animations.Get(AnimationType.State);
            m_loopDatabase = Animations.Get(AnimationType.Loop);

            AddInfoMessage(UIButtonBehaviorType.OnPointerEnter.ToString(), new InfoMessage(DGUI.Icon.OnPointerEnter, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnPointerExit.ToString(), new InfoMessage(DGUI.Icon.OnPointerExit, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnPointerDown.ToString(), new InfoMessage(DGUI.Icon.OnPointerDown, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnPointerUp.ToString(), new InfoMessage(DGUI.Icon.OnPointerUp, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnClick.ToString(), new InfoMessage(DGUI.Icon.OnClick, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnDoubleClick.ToString(), new InfoMessage(DGUI.Icon.OnDoubleClick, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnLongClick.ToString(), new InfoMessage(DGUI.Icon.OnLongClick, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnRightClick.ToString(), new InfoMessage(DGUI.Icon.OnClick, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnSelected.ToString(), new InfoMessage(DGUI.Icon.OnButtonSelect, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(UIButtonBehaviorType.OnDeselected.ToString(), new InfoMessage(DGUI.Icon.OnButtonDeselect, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(ButtonLoopAnimationType.Normal.ToString(), new InfoMessage(DGUI.Icon.Loop, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));
            AddInfoMessage(ButtonLoopAnimationType.Selected.ToString(), new InfoMessage(DGUI.Icon.Loop, DGUI.Colors.LightOrDarkColorName, DGUI.Colors.DarkOrLightColorName, "", false, Repaint));

            UpdateTextSerializedObject();
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
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIButton), "UIButton", MenuUtils.UIButton_Manual, MenuUtils.UIButton_YouTube);
            DrawDebugModeAndCreateParentAndCenterPivot();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawCategoryAndName();
            GUILayout.Space(DGUI.Properties.Space());
            DrawRenameGameObjectAndOpenDatabaseButtons();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAllowMultipleClicks();
            GUILayout.Space(DGUI.Properties.Space());
            DrawDeselectButtonAfterClick();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawInputData();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawBehaviors();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawLoopAnimations();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawLabelOptions();
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

        private void DrawCategoryAndName()
        {
            DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                    m_buttonCategory, UILabels.ButtonCategory,
                                                    m_buttonName, UILabels.ButtonName,
                                                    Database, ComponentColorName);
        }

        private void DrawRenameGameObjectAndOpenDatabaseButtons()
        {
            DGUI.Doozy.DrawRenameGameObjectAndOpenDatabaseButtons(Settings.RenamePrefix,
                                                                  m_buttonName.stringValue,
                                                                  Settings.RenameSuffix,
                                                                  Target.gameObject,
                                                                  DoozyWindow.View.Buttons,
                                                                  false,
                                                                  () =>
                                                                  {
                                                                      DoozyWindow.Instance.GetAnimBool(DoozyWindow.View.Buttons + Target.ButtonCategory).target = true;
                                                                  });
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

            if (!Settings.ShowOnPointerDown) Target.OnPointerDown.Enabled = false;
            else DrawBehavior(UILabels.OnPointerDown, Target.OnPointerDown, m_onPointerDown, DGUI.Icon.OnPointerDown, m_onPointerDownExpanded);
            if (!Settings.ShowOnPointerUp) Target.OnPointerUp.Enabled = false;
            else DrawBehavior(UILabels.OnPointerUp, Target.OnPointerUp, m_onPointerUp, DGUI.Icon.OnPointerUp, m_onPointerUpExpanded);

            if (Settings.ShowOnPointerDown || Settings.ShowOnPointerUp) GUILayout.Space(DGUI.Properties.Space(4));

            if (!Settings.ShowOnClick) Target.OnClick.Enabled = false;
            else DrawBehavior(UILabels.OnClick, Target.OnClick, m_onClick, DGUI.Icon.OnClick, m_onClickExpanded);
            if (!Settings.ShowOnDoubleClick) Target.OnDoubleClick.Enabled = false;
            else DrawBehavior(UILabels.OnDoubleClick, Target.OnDoubleClick, m_onDoubleClick, DGUI.Icon.OnDoubleClick, m_onDoubleClickExpanded);
            if (!Settings.ShowOnLongClick) Target.OnLongClick.Enabled = false;
            else DrawBehavior(UILabels.OnLongClick, Target.OnLongClick, m_onLongClick, DGUI.Icon.OnLongClick, m_onLongClickExpanded);
            if (!Settings.ShowOnRightClick) Target.OnRightClick.Enabled = false;
            else DrawBehavior(UILabels.OnRightClick, Target.OnRightClick, m_onRightClick, DGUI.Icon.OnClick, m_onRightClickExpanded);
            
            if (Settings.ShowOnClick || Settings.ShowOnDoubleClick || Settings.ShowOnLongClick || Settings.ShowOnRightClick) GUILayout.Space(DGUI.Properties.Space(4));

            if (!Settings.ShowOnButtonSelected) Target.OnSelected.Enabled = false;
            else DrawBehavior(UILabels.OnSelected, Target.OnSelected, m_onSelected, DGUI.Icon.OnButtonSelect, m_onSelectedExpanded);
            if (!Settings.ShowOnButtonDeselected) Target.OnDeselected.Enabled = false;
            else DrawBehavior(UILabels.OnDeselected, Target.OnDeselected, m_onDeselected, DGUI.Icon.OnButtonDeselect, m_onDeselectedExpanded);
        }

        private void DrawBehavior(string behaviorName, UIButtonBehavior behavior, SerializedProperty behaviorProperty, GUIStyle animationIcon, AnimBool behaviorExpanded)
        {
            SerializedProperty enabledProperty = GetProperty(PropertyName.Enabled, behaviorProperty);
            SerializedProperty triggerEventsAfterAnimationProperty = GetProperty(PropertyName.TriggerEventsAfterAnimation, behaviorProperty);
            SerializedProperty selectButtonProperty = GetProperty(PropertyName.SelectButton, behaviorProperty);
            SerializedProperty deselectButtonProperty = GetProperty(PropertyName.DeselectButton, behaviorProperty);
            SerializedProperty loadSelectedPresetAtRuntimeProperty = GetProperty(PropertyName.LoadSelectedPresetAtRuntime, behaviorProperty);
            SerializedProperty presetCategoryProperty = GetProperty(PropertyName.PresetCategory, behaviorProperty);
            SerializedProperty presetNameProperty = GetProperty(PropertyName.PresetName, behaviorProperty);
            SerializedProperty onTriggerProperty = GetProperty(PropertyName.OnTrigger, behaviorProperty);
            SerializedProperty buttonAnimationTypeProperty = GetProperty(PropertyName.ButtonAnimationType, behaviorProperty);
            var buttonAnimationType = (ButtonAnimationType) buttonAnimationTypeProperty.enumValueIndex;

            AnimBool animationTypeExpanded = GetAnimBool(buttonAnimationTypeProperty.propertyPath, buttonAnimationTypeProperty.isExpanded);
            AnimBool onTriggerExpanded = GetAnimBool(onTriggerProperty.propertyPath, onTriggerProperty.isExpanded);

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
                                onTriggerExpanded.value = false;
                            }

                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        if (enabledProperty.boolValue)
                        {
                            GUILayout.Space(-NormalBarHeight);
                            DrawButtonBehaviorIcons(behavior, behaviorExpanded);

                            InfoMessage presetInfoMessage = GetInfoMessage(behavior.BehaviorType.ToString());
                            presetInfoMessage.Message = GetRuntimePresetCategoryAndName(behavior);
                            GUILayout.Space(-DGUI.Properties.Space(2) * presetInfoMessage.Show.faded);
                            presetInfoMessage.DrawMessageOnly(behavior.LoadSelectedPresetAtRuntime);
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
                                switch (behavior.BehaviorType)
                                {
                                    case UIButtonBehaviorType.OnClick:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(triggerEventsAfterAnimationProperty, UILabels.TriggerEventsAfterAnimation, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () =>
                                                       {
                                                           DGUI.Line.Draw(false, ComponentColorName,
                                                                          () =>
                                                                          {
                                                                              GUILayout.Space(DGUI.Properties.Space(2));
                                                                              DGUI.Label.Draw(UILabels.ClickMode, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                                              DGUI.Property.Draw(m_clickMode, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                                                              GUILayout.Space(DGUI.Properties.Space());
                                                                          });
                                                       }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnDoubleClick:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(triggerEventsAfterAnimationProperty, UILabels.TriggerEventsAfterAnimation, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawRegisterInterval(m_doubleClickRegisterInterval); }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnLongClick:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(triggerEventsAfterAnimationProperty, UILabels.TriggerEventsAfterAnimation, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawRegisterInterval(m_longClickRegisterInterval); }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnPointerEnter:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(selectButtonProperty, UILabels.SelectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawButtonBehaviorDisableInterval(behaviorProperty); }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnPointerExit:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(deselectButtonProperty, UILabels.DeselectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace,
                                                       () => { DrawButtonBehaviorDisableInterval(behaviorProperty); }
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnPointerDown:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(selectButtonProperty, UILabels.SelectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace
                                                      );
                                        addSpace = true;
                                        break;
                                    case UIButtonBehaviorType.OnPointerUp:
                                        DGUI.Line.Draw(false,
                                                       () => { DGUI.Toggle.Checkbox.Draw(deselectButtonProperty, UILabels.DeselectButton, ComponentColorName, true, false); },
                                                       GUILayout.FlexibleSpace
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
                                                                                     behavior.PunchAnimation,
                                                                                     ComponentColorName,
                                                                                     out behavior.PunchAnimation))
                                            {
                                                if (serializedObject.isEditingMultipleObjects)
                                                {
                                                    foreach (Object targetObject in serializedObject.targetObjects)
                                                    {
                                                        var targetButton = (UIButton) targetObject;
                                                        switch (behavior.BehaviorType)
                                                        {
                                                            case UIButtonBehaviorType.OnClick:
                                                                targetButton.OnClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnClick.PresetName = behavior.PresetName;
                                                                targetButton.OnClick.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnDoubleClick:
                                                                targetButton.OnDoubleClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnDoubleClick.PresetName = behavior.PresetName;
                                                                targetButton.OnDoubleClick.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnLongClick:
                                                                targetButton.OnLongClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnLongClick.PresetName = behavior.PresetName;
                                                                targetButton.OnLongClick.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerEnter:
                                                                targetButton.OnPointerEnter.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerEnter.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerEnter.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerExit:
                                                                targetButton.OnPointerExit.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerExit.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerExit.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerDown:
                                                                targetButton.OnPointerDown.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerDown.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerDown.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerUp:
                                                                targetButton.OnPointerUp.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerUp.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerUp.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnSelected:
                                                                targetButton.OnSelected.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnSelected.PresetName = behavior.PresetName;
                                                                targetButton.OnSelected.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnDeselected:
                                                                targetButton.OnDeselected.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnDeselected.PresetName = behavior.PresetName;
                                                                targetButton.OnDeselected.PunchAnimation = behavior.PunchAnimation.Copy();
                                                                break;
                                                        }
                                                    }
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
                                                                                     behavior.StateAnimation,
                                                                                     ComponentColorName,
                                                                                     out behavior.StateAnimation))
                                            {
                                                if (serializedObject.isEditingMultipleObjects)
                                                {
                                                    foreach (Object targetObject in serializedObject.targetObjects)
                                                    {
                                                        var targetButton = (UIButton) targetObject;
                                                        switch (behavior.BehaviorType)
                                                        {
                                                            case UIButtonBehaviorType.OnClick:
                                                                targetButton.OnClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnClick.PresetName = behavior.PresetName;
                                                                targetButton.OnClick.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnDoubleClick:
                                                                targetButton.OnDoubleClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnDoubleClick.PresetName = behavior.PresetName;
                                                                targetButton.OnDoubleClick.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnLongClick:
                                                                targetButton.OnLongClick.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnLongClick.PresetName = behavior.PresetName;
                                                                targetButton.OnLongClick.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerEnter:
                                                                targetButton.OnPointerEnter.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerEnter.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerEnter.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerExit:
                                                                targetButton.OnPointerExit.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerExit.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerExit.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerDown:
                                                                targetButton.OnPointerDown.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerDown.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerDown.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnPointerUp:
                                                                targetButton.OnPointerUp.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnPointerUp.PresetName = behavior.PresetName;
                                                                targetButton.OnPointerUp.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnSelected:
                                                                targetButton.OnSelected.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnSelected.PresetName = behavior.PresetName;
                                                                targetButton.OnSelected.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                            case UIButtonBehaviorType.OnDeselected:
                                                                targetButton.OnDeselected.PresetCategory = behavior.PresetCategory;
                                                                targetButton.OnDeselected.PresetName = behavior.PresetName;
                                                                targetButton.OnDeselected.StateAnimation = behavior.StateAnimation.Copy();
                                                                break;
                                                        }
                                                    }
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
                                                                         DGUI.Doozy.GetButtonBehaviorAnimationIcons(behavior,
                                                                                                                    m_behaviorAnimationIconsDatabase,
                                                                                                                    ComponentColorName)))
                                    {
                                        animationTypeExpanded.target = true;
                                        onTriggerExpanded.value = false;
                                        SoundyAudioPlayer.StopAllPlayers();
                                    }

                                    GUILayout.Space(DGUI.Properties.Space());

                                    if (DGUI.Doozy.DrawSectionButtonRight(onTriggerExpanded.target,
                                                                          UILabels.OnTrigger,
                                                                          DGUI.Icon.Action,
                                                                          ComponentColorName,
                                                                          DGUI.Doozy.GetActionsIcons(behavior.OnTrigger,
                                                                                                     m_behaviorActionsIconsDatabase,
                                                                                                     ComponentColorName)))
                                    {
                                        animationTypeExpanded.target = false;
                                        onTriggerExpanded.target = true;
                                        SoundyAudioPlayer.StopAllPlayers();
                                    }
                                }
                                GUILayout.EndHorizontal();

                                buttonAnimationTypeProperty.isExpanded = animationTypeExpanded.target;
                                onTriggerProperty.isExpanded = onTriggerExpanded.target;

                                GUILayout.Space(DGUI.Properties.Space(3) * behaviorExpanded.faded);

                                DrawButtonBehaviorAnimation(behavior, behaviorProperty);
                                DrawBehaviorActions(behavior.OnTrigger, onTriggerProperty, onTriggerExpanded, behaviorName + "." + PropertyName.OnTrigger);

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

        private void DrawRegisterInterval(SerializedProperty property)
        {
            DGUI.Line.Draw(false, ComponentColorName,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(UILabels.RegisterInterval, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               DGUI.Property.Draw(property, ComponentColorName, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                               DGUI.Label.Draw(UILabels.Seconds, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space(2));
                           });
        }

        private void DrawButtonBehaviorIcons(UIButtonBehavior behavior, AnimBool expanded)
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

        private void DrawButtonBehaviorAnimation(UIButtonBehavior behavior, SerializedProperty behaviorProperty)
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

        private void DrawBehaviorAnimationTypeSelector(UIButtonBehavior behavior, SerializedProperty buttonAnimationTypeProperty)
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

        private void DrawBehaviorActions(UIAction actions, SerializedProperty actionsProperty, AnimBool expanded, string unityEventDisplayPath)
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
                                                            ComponentColorName,
                                                            DGUI.IconGroup.GetIcon(
                                                                                   actions.HasSound,
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
                                                                                     actions.HasEffect,
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
                                                                                                actions.HasAnimatorEvents,
                                                                                                actions.AnimatorEventsCount,
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
                                                                                                actions.HasGameEvents,
                                                                                                actions.GameEventsCount,
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
                                                                                               actions.HasUnityEvent,
                                                                                               actions.UnityEventListenerCount,
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
                                            actions.Effect,
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
                    DGUI.Property.UnityEventWithFade(unityEventProperty, m_unityEventsExpanded, unityEventDisplayPath + ".Event", ComponentColorName, actions.UnityEventListenerCount);
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

        private void DrawLoopAnimations()
        {
            if (!Settings.ShowNormalLoopAnimation) Target.NormalLoopAnimation.Enabled = false;
            else DrawLoopAnimation(UILabels.NormalLoopAnimation, Target.NormalLoopAnimation, m_normalLoopAnimation, m_normalLoopAnimationExpanded);
            if (!Settings.ShowSelectedLoopAnimation) Target.SelectedLoopAnimation.Enabled = false;
            else DrawLoopAnimation(UILabels.SelectedLoopAnimation, Target.SelectedLoopAnimation, m_selectedLoopAnimation, m_selectedLoopAnimationExpanded);
        }

        private void DrawLoopAnimation(string loopAnimationName, UIButtonLoopAnimation buttonLoopAnimation, SerializedProperty loopAnimationProperty, AnimBool loopAnimationExpanded)
        {
            SerializedProperty enabled = GetProperty(PropertyName.Enabled, loopAnimationProperty);
            SerializedProperty loadSelectedPresetAtRuntimeProperty = GetProperty(PropertyName.LoadSelectedPresetAtRuntime, loopAnimationProperty);
            SerializedProperty presetCategoryProperty = GetProperty(PropertyName.PresetCategory, loopAnimationProperty);
            SerializedProperty presetNameProperty = GetProperty(PropertyName.PresetName, loopAnimationProperty);
            SerializedProperty animationProperty = GetProperty(PropertyName.Animation, loopAnimationProperty);

            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(enabled, ComponentColorName, NormalBarHeight);
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginVertical();
                {
                    bool enabledState = GUI.enabled;
                    GUI.enabled = enabled.boolValue;
                    {
                        if (!enabled.boolValue && loopAnimationExpanded.target) loopAnimationExpanded.target = false;
                        if (DGUI.Bar.Draw(loopAnimationName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, loopAnimationExpanded))
                        {
                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        if (enabled.boolValue)
                        {
                            GUILayout.Space(-NormalBarHeight);
                            DrawButtonLoopAnimationIcons(buttonLoopAnimation, loopAnimationExpanded);

                            InfoMessage presetInfoMessage = GetInfoMessage(buttonLoopAnimation.LoopAnimationType.ToString());
                            presetInfoMessage.Message = GetRuntimePresetCategoryAndName(buttonLoopAnimation);
                            GUILayout.Space(-DGUI.Properties.Space(2) * presetInfoMessage.Show.faded);
                            presetInfoMessage.DrawMessageOnly(buttonLoopAnimation.LoadSelectedPresetAtRuntime);
                            GUILayout.Space(DGUI.Properties.Space(3) * presetInfoMessage.Show.faded);
                        }
                        else
                        {
                            GUILayout.Space(DGUI.Properties.Space());
                        }


                        if (DGUI.FadeOut.Begin(loopAnimationExpanded, false))
                        {
                            GUILayout.BeginVertical();
                            {
                                if (UIAnimationUtils.DrawAnimationPreset(serializedObject,
                                                                         loadSelectedPresetAtRuntimeProperty,
                                                                         presetCategoryProperty,
                                                                         presetNameProperty,
                                                                         m_loopDatabase,
                                                                         buttonLoopAnimation.Animation,
                                                                         ComponentColorName,
                                                                         out buttonLoopAnimation.Animation))
                                {
                                    if (serializedObject.isEditingMultipleObjects)
                                    {
                                        foreach (Object targetObject in serializedObject.targetObjects)
                                        {
                                            var targetButton = (UIButton) targetObject;
                                            switch (buttonLoopAnimation.LoopAnimationType)
                                            {
                                                case ButtonLoopAnimationType.Normal:
                                                    targetButton.NormalLoopAnimation.PresetCategory = buttonLoopAnimation.PresetCategory;
                                                    targetButton.NormalLoopAnimation.PresetName = buttonLoopAnimation.PresetName;
                                                    targetButton.NormalLoopAnimation.Animation = buttonLoopAnimation.Animation.Copy();
                                                    break;
                                                case ButtonLoopAnimationType.Selected:
                                                    targetButton.SelectedLoopAnimation.PresetCategory = buttonLoopAnimation.PresetCategory;
                                                    targetButton.SelectedLoopAnimation.PresetName = buttonLoopAnimation.PresetName;
                                                    targetButton.SelectedLoopAnimation.Animation = buttonLoopAnimation.Animation.Copy();
                                                    break;
                                            }
                                        }
                                    }
                                }

                                GUILayout.Space(DGUI.Properties.Space() * loopAnimationExpanded.faded);

                                SerializedProperty move = GetProperty(PropertyName.Move, animationProperty);
                                SerializedProperty rotate = GetProperty(PropertyName.Rotate, animationProperty);
                                SerializedProperty scale = GetProperty(PropertyName.Scale, animationProperty);
                                SerializedProperty fade = GetProperty(PropertyName.Fade, animationProperty);
                                AnimBool moveExpanded = GetAnimBool(move.propertyPath);
                                AnimBool rotateExpanded = GetAnimBool(rotate.propertyPath);
                                AnimBool scaleExpanded = GetAnimBool(scale.propertyPath);
                                AnimBool fadeExpanded = GetAnimBool(fade.propertyPath);
                                moveExpanded.target = GetProperty(PropertyName.Enabled, move).boolValue;
                                rotateExpanded.target = GetProperty(PropertyName.Enabled, rotate).boolValue;
                                scaleExpanded.target = GetProperty(PropertyName.Enabled, scale).boolValue;
                                fadeExpanded.target = GetProperty(PropertyName.Enabled, fade).boolValue;

                                const Size barSize = Size.M;

                                GUILayout.BeginVertical();
                                {
                                    DGUI.Doozy.DrawPreviewAnimationButtons(serializedObject, Target, buttonLoopAnimation, ComponentColorName);

                                    GUILayout.Space(DGUI.Properties.Space(2) * loopAnimationExpanded.faded);

                                    GUILayout.BeginHorizontal();
                                    {
                                        if (DGUI.Bar.Draw(DGUI.Doozy.GetAnimationTypeName(UILabels.Move, (AnimationType) move.FindPropertyRelative(PropertyName.AnimationType.ToString()).enumValueIndex),
                                                          barSize,
                                                          DGUI.Bar.Caret.CaretType.Move,
                                                          moveExpanded.target ? DGUI.Colors.MoveColorName : DGUI.Colors.DisabledTextColorName,
                                                          moveExpanded))
                                            GetProperty(PropertyName.Enabled, move).boolValue = !GetProperty(PropertyName.Enabled, move).boolValue;

                                        GUILayout.Space(DGUI.Properties.Space());

                                        if (DGUI.Bar.Draw(DGUI.Doozy.GetAnimationTypeName(UILabels.Rotate, (AnimationType) rotate.FindPropertyRelative(PropertyName.AnimationType.ToString()).enumValueIndex),
                                                          barSize,
                                                          DGUI.Bar.Caret.CaretType.Rotate,
                                                          rotateExpanded.target ? DGUI.Colors.RotateColorName : DGUI.Colors.DisabledTextColorName,
                                                          rotateExpanded))
                                            GetProperty(PropertyName.Enabled, rotate).boolValue = !GetProperty(PropertyName.Enabled, rotate).boolValue;

                                        GUILayout.Space(DGUI.Properties.Space());

                                        if (DGUI.Bar.Draw(DGUI.Doozy.GetAnimationTypeName(UILabels.Scale, (AnimationType) scale.FindPropertyRelative(PropertyName.AnimationType.ToString()).enumValueIndex),
                                                          barSize,
                                                          DGUI.Bar.Caret.CaretType.Scale,
                                                          scaleExpanded.target ? DGUI.Colors.ScaleColorName : DGUI.Colors.DisabledTextColorName,
                                                          scaleExpanded))
                                            GetProperty(PropertyName.Enabled, scale).boolValue = !GetProperty(PropertyName.Enabled, scale).boolValue;

                                        GUILayout.Space(DGUI.Properties.Space());

                                        if (DGUI.Bar.Draw(DGUI.Doozy.GetAnimationTypeName(UILabels.Fade, (AnimationType) fade.FindPropertyRelative(PropertyName.AnimationType.ToString()).enumValueIndex),
                                                          barSize,
                                                          DGUI.Bar.Caret.CaretType.Fade,
                                                          fadeExpanded.target ? DGUI.Colors.FadeColorName : DGUI.Colors.DisabledTextColorName,
                                                          fadeExpanded))
                                            GetProperty(PropertyName.Enabled, fade).boolValue = !GetProperty(PropertyName.Enabled, fade).boolValue;
                                    }
                                    GUILayout.EndHorizontal();

                                    DGUI.Property.DrawWithFade(move, moveExpanded);
                                    DGUI.Property.DrawWithFade(rotate, rotateExpanded);
                                    DGUI.Property.DrawWithFade(scale, scaleExpanded);
                                    DGUI.Property.DrawWithFade(fade, fadeExpanded);

                                    GUILayout.Space(DGUI.Properties.Space(4) * loopAnimationExpanded.faded);
                                }

                                GUILayout.EndVertical();
                            }
                            GUILayout.EndVertical();
                        }

                        DGUI.FadeOut.End(loopAnimationExpanded, false);
                    }
                    GUI.enabled = enabledState;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            loopAnimationProperty.isExpanded = loopAnimationExpanded.target;
        }

        private void DrawButtonLoopAnimationIcons(UIButtonLoopAnimation buttonLoopAnimation, AnimBool expanded)
        {
            if (DGUI.AlphaGroup.Begin(Mathf.Clamp(1 - expanded.faded, 0.2f, 1f)))
            {
                GUILayout.BeginHorizontal(GUILayout.Height(NormalBarHeight));
                {
                    GUILayout.FlexibleSpace();

                    if (!m_buttonLoopAnimationIconsDatabase.ContainsKey(buttonLoopAnimation)) m_buttonLoopAnimationIconsDatabase.Add(buttonLoopAnimation, new List<DGUI.IconGroup.Data>());
                    m_buttonLoopAnimationIconsDatabase[buttonLoopAnimation] = DGUI.Doozy.GetBehaviorUIAnimationIcons(m_buttonLoopAnimationIconsDatabase[buttonLoopAnimation],
                                                                                                                     AnimationType.Loop,
                                                                                                                     buttonLoopAnimation.Animation.Move.Enabled,
                                                                                                                     buttonLoopAnimation.Animation.Rotate.Enabled,
                                                                                                                     buttonLoopAnimation.Animation.Scale.Enabled,
                                                                                                                     buttonLoopAnimation.Animation.Fade.Enabled,
                                                                                                                     ComponentColorName);

                    if (m_buttonLoopAnimationIconsDatabase[buttonLoopAnimation].Count > 0) DGUI.IconGroup.Draw(m_buttonLoopAnimationIconsDatabase[buttonLoopAnimation], NormalBarHeight - DGUI.Properties.Space(4), false);

                    GUILayout.Space(DGUI.Properties.Space(3));
                }
                GUILayout.EndHorizontal();
            }

            DGUI.AlphaGroup.End();
        }


        private void DrawLabelOptions()
        {
            var targetLabel = (TargetLabel) m_targetLabel.enumValueIndex;
            bool hasLabel = targetLabel != TargetLabel.None;

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(hasLabel, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(hasLabel, ComponentColorName);

            DGUI.Doozy.DrawTitleWithIcon(DGUI.Icon.LabelIcon, UILabels.ButtonLabel, Size.L, NormalBarHeight, textColorName);
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                DGUI.Property.Draw(m_targetLabel, UILabels.TargetLabel, backgroundColorName, textColorName);
                if (EditorGUI.EndChangeCheck()) UpdateTextSerializedObject();
                if (hasLabel)
                {
                    switch (targetLabel)
                    {
                        case TargetLabel.Text:
                            GUILayout.Space(DGUI.Properties.Space());
                            EditorGUI.BeginChangeCheck();
                            DGUI.Property.Draw(m_textLabel, UILabels.TextLabel, backgroundColorName, textColorName, Target.TextLabel == null);
                            if (EditorGUI.EndChangeCheck()) UpdateTextSerializedObject();
                            break;
                        case TargetLabel.TextMeshPro:
#if dUI_TextMeshPro
                            GUILayout.Space(DGUI.Properties.Space());
                            EditorGUI.BeginChangeCheck();
                            DGUI.Property.Draw(m_textMeshProLabel, UILabels.TextMeshProLabel, backgroundColorName, textColorName, Target.TextMeshProLabel == null);
                            if (EditorGUI.EndChangeCheck()) UpdateTextSerializedObject();
#endif
                            break;
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (!hasLabel) return;
            if (m_textSerializedObject == null || m_textProperty == null) return;
            m_textSerializedObject.Update();
            DGUI.Property.Draw(m_textProperty);
            m_textSerializedObject.ApplyModifiedProperties();
        }

        private void UpdateTextSerializedObject()
        {
            m_textSerializedObject = null;
            m_textProperty = null;

            switch ((TargetLabel) m_targetLabel.enumValueIndex)
            {
                case TargetLabel.Text:
                    if (Target.TextLabel == null) return;
                    m_textSerializedObject = new SerializedObject(Target.TextLabel, this);
                    if (m_textSerializedObject == null) return;
                    m_textProperty = m_textSerializedObject.FindProperty("m_Text");
                    break;
                case TargetLabel.TextMeshPro:
#if dUI_TextMeshPro
                    if (Target.TextMeshProLabel == null) return;
                    m_textSerializedObject = new SerializedObject(Target.TextMeshProLabel, this);
                    if (m_textSerializedObject == null) return;
                    m_textProperty = m_textSerializedObject.FindProperty("m_text");
#endif
                    break;
            }
        }
    }
}