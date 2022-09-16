// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Animation;
using Doozy.Editor.UI.Animation;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using Doozy.Editor.Soundy;
using Doozy.Engine.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIPopup))]
    public class UIPopupEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIPopupColorName; } }
        private UIPopup m_target;

        private UIPopup Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIPopup) target;
                return m_target;
            }
        }

        private const string NO_SHOW_ANIMATION = "NoShowAnimation";
        private const string SHOW_INSTANT_ANIMATION = "ShowInstantAnimation";
        private const string NO_HIDE_ANIMATION = "NoHideAnimation";
        private const string HIDE_INSTANT_ANIMATION = "HideInstantAnimation";

        private static UIPopupSettings Settings { get { return UIPopupSettings.Instance; } }
        private static NamesDatabase CanvasDatabase { get { return UICanvasSettings.Database; } }

        private static UIAnimations Animations { get { return UIAnimations.Instance; } }
        private UIAnimationsDatabase m_showDatabase, m_hideDatabase;

        private readonly Dictionary<UIAnimation, List<DGUI.IconGroup.Data>> m_behaviorAnimationIconsDatabase = new Dictionary<UIAnimation, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIAction, List<DGUI.IconGroup.Data>> m_behaviorActionsIconsDatabase = new Dictionary<UIAction, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIPopupBehavior, List<DGUI.IconGroup.Data>> m_viewBehaviorIconsDatabase = new Dictionary<UIPopupBehavior, List<DGUI.IconGroup.Data>>();

        private SerializedProperty
            m_addToPopupQueue,
            m_autoHideAfterShow,
            m_autoHideAfterShowDelay,
            m_autoSelectButtonAfterShow,
            m_autoSelectPreviouslySelectedButtonAfterHide,
            m_blockBackButton,
            m_buttons,
            m_canvasName,
            m_container,
            m_customCanvasName,
            m_data,
            m_destroyAfterHide,
            m_displayTarget,
            m_hideBehavior,
            m_hideInstantAnimation,
            m_hideOnAnyButton,
            m_hideOnBackButton,
            m_hideOnClickAnywhere,
            m_hideOnClickContainer,
            m_hideOnClickOverlay,
            m_hideProgressor,
            m_images,
            m_labels,
            m_onInverseVisibilityChanged,
            m_onVisibilityChanged,
            m_overlay,
            m_selectedButton,
            m_showBehavior,
            m_showInstantAnimation,
            m_showProgressor,
            m_updateHideProgressorOnShow,
            m_updateShowProgressorOnHide;

        private AnimBool
            m_animatorEventsExpanded,
            m_autoHideExpanded,
            m_autoSelectButtonExpanded,
            m_buttonsExpanded,
            m_canvasNameExpanded,
            m_effectExpanded,
            m_gameEventsExpanded,
            m_hideExpanded,
            m_imagesExpanded,
            m_labelsExpanded,
            m_onInverseVisibilityChangedExpanded,
            m_onVisibilityChangedExpanded,
            m_showExpanded,
            m_soundDataExpanded,
            m_unityEventsExpanded;

        private InfoMessage
            m_noShowAnimationInfoMessage,
            m_showInstantAnimationInfoMessage,
            m_noHideAnimationInfoMessage,
            m_hideInstantAnimationInfoMessage;

        private bool m_instantAction;

        private string GetRuntimePresetCategoryAndName(UIPopupBehavior behavior) { return string.Format(UILabels.RuntimePreset + ": {0} / {1}", behavior.PresetCategory, behavior.PresetName); }

        private int OnVisibilityChangedEventCount { get { return Target.OnVisibilityChanged.GetPersistentEventCount(); } }
        private int OnInverseVisibilityChangedEventCount { get { return Target.OnInverseVisibilityChanged.GetPersistentEventCount(); } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_data = GetProperty(PropertyName.Data);
            m_buttons = GetProperty(PropertyName.Buttons, m_data);
            m_images = GetProperty(PropertyName.Images, m_data);
            m_labels = GetProperty(PropertyName.Labels, m_data);

            m_addToPopupQueue = GetProperty(PropertyName.AddToPopupQueue);
            m_autoHideAfterShow = GetProperty(PropertyName.AutoHideAfterShow);
            m_autoHideAfterShowDelay = GetProperty(PropertyName.AutoHideAfterShowDelay);
            m_autoSelectButtonAfterShow = GetProperty(PropertyName.AutoSelectButtonAfterShow);
            m_autoSelectPreviouslySelectedButtonAfterHide = GetProperty(PropertyName.AutoSelectPreviouslySelectedButtonAfterHide);
            m_blockBackButton = GetProperty(PropertyName.BlockBackButton);
            m_canvasName = GetProperty(PropertyName.CanvasName);
            m_container = GetProperty(PropertyName.Container);
            m_customCanvasName = GetProperty(PropertyName.CustomCanvasName);
            m_destroyAfterHide = GetProperty(PropertyName.DestroyAfterHide);
            m_displayTarget = GetProperty(PropertyName.DisplayTarget);
            m_hideBehavior = GetProperty(PropertyName.HideBehavior);
            m_hideInstantAnimation = GetProperty(PropertyName.InstantAnimation, m_hideBehavior);
            m_hideOnAnyButton = GetProperty(PropertyName.HideOnAnyButton);
            m_hideOnBackButton = GetProperty(PropertyName.HideOnBackButton);
            m_hideOnClickAnywhere = GetProperty(PropertyName.HideOnClickAnywhere);
            m_hideOnClickContainer = GetProperty(PropertyName.HideOnClickContainer);
            m_hideOnClickOverlay = GetProperty(PropertyName.HideOnClickOverlay);
            m_hideProgressor = GetProperty(PropertyName.HideProgressor);
            m_onInverseVisibilityChanged = GetProperty(PropertyName.OnInverseVisibilityChanged);
            m_onVisibilityChanged = GetProperty(PropertyName.OnVisibilityChanged);
            m_overlay = GetProperty(PropertyName.Overlay);
            m_selectedButton = GetProperty(PropertyName.SelectedButton);
            m_showBehavior = GetProperty(PropertyName.ShowBehavior);
            m_showInstantAnimation = GetProperty(PropertyName.InstantAnimation, m_showBehavior);
            m_showProgressor = GetProperty(PropertyName.ShowProgressor);
            m_updateHideProgressorOnShow = GetProperty(PropertyName.UpdateHideProgressorOnShow);
            m_updateShowProgressorOnHide = GetProperty(PropertyName.UpdateShowProgressorOnHide);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_autoHideExpanded = GetAnimBool(m_autoHideAfterShow.propertyPath, m_autoHideAfterShow.boolValue);
            m_autoSelectButtonExpanded = GetAnimBool(m_autoSelectButtonAfterShow.propertyPath, m_autoSelectButtonAfterShow.boolValue);
            m_buttonsExpanded = GetAnimBool(m_buttons.propertyPath, m_buttons.isExpanded);
            m_canvasNameExpanded = GetAnimBool(m_canvasName.propertyPath, m_canvasName.isExpanded);
            m_effectExpanded = GetAnimBool("EFFECT");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_hideExpanded = GetAnimBool(m_hideBehavior.propertyPath, m_hideBehavior.isExpanded);
            m_imagesExpanded = GetAnimBool(m_images.propertyPath, m_images.isExpanded);
            m_labelsExpanded = GetAnimBool(m_labels.propertyPath, m_labels.isExpanded);
            m_onInverseVisibilityChangedExpanded = GetAnimBool(m_onInverseVisibilityChanged.propertyPath, OnInverseVisibilityChangedEventCount > 0);
            m_onVisibilityChangedExpanded = GetAnimBool(m_onVisibilityChanged.propertyPath, OnVisibilityChangedEventCount > 0);
            m_showExpanded = GetAnimBool(m_showBehavior.propertyPath, m_showBehavior.isExpanded);
            m_soundDataExpanded = GetAnimBool("SOUND");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();
            AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);
            if (Target.HasContainer) AdjustPositionRotationAndScaleToRoundValues(Target.Container.RectTransform);
            if (Target.HasOverlay) AdjustPositionRotationAndScaleToRoundValues(Target.Overlay.RectTransform);

            m_showDatabase = Animations.Get(AnimationType.Show);
            m_hideDatabase = Animations.Get(AnimationType.Hide);

            AddInfoMessage(AnimationType.Show.ToString(), new InfoMessage(DGUI.Icon.Show, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(AnimationType.Hide.ToString(), new InfoMessage(DGUI.Icon.Hide, ComponentColorName, DGUI.Colors.DarkOrWhiteColorName, "", false, Repaint));
            AddInfoMessage(NO_SHOW_ANIMATION, new InfoMessage(DGUI.Icon.Show, InfoMessage.GetIconColor(InfoMessage.MessageType.Error), InfoMessage.GetIconColor(InfoMessage.MessageType.Error), UILabels.NoAnimationEnabled, UILabels.ShowAnimationWillNotWork, false, Repaint));
            AddInfoMessage(SHOW_INSTANT_ANIMATION, new InfoMessage(DGUI.Icon.Show, InfoMessage.GetIconColor(InfoMessage.MessageType.Info), InfoMessage.GetIconColor(InfoMessage.MessageType.Info), UILabels.InstantAnimation, false, Repaint));
            AddInfoMessage(NO_HIDE_ANIMATION, new InfoMessage(DGUI.Icon.Hide, InfoMessage.GetIconColor(InfoMessage.MessageType.Error), InfoMessage.GetIconColor(InfoMessage.MessageType.Error), UILabels.NoAnimationEnabled, UILabels.HideAnimationWillNotWork, false, Repaint));
            AddInfoMessage(HIDE_INSTANT_ANIMATION, new InfoMessage(DGUI.Icon.Hide, InfoMessage.GetIconColor(InfoMessage.MessageType.Info), InfoMessage.GetIconColor(InfoMessage.MessageType.Info), UILabels.InstantAnimation, false, Repaint));

            UpdateOverlayEnabledState();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
            if (UIAnimatorUtils.PreviewIsPlaying) UIAnimatorUtils.StopPopupPreview(Target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIPopup), MenuUtils.UIPopup_Manual, MenuUtils.UIPopup_YouTube);
            DrawRuntimeOptions();
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAddToPopupQueueAndDisplayTarget();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawBehaviors();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAutoHide();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawDisableBackButton();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawHideOptions();
            GUILayout.Space(DGUI.Properties.Space());
            DrawDestroyAfterHide();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAutoSelectButton();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawAutoSelectPreviouslySelectedButtonAfterHide();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawOverlay();
            GUILayout.Space(DGUI.Properties.Space());
            DrawContainer();
            GUILayout.Space(DGUI.Properties.Space());
            DrawWhenHidden();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawDataList(m_labelsExpanded, m_labels, UILabels.Labels);
            GUILayout.Space(DGUI.Properties.Space());
            DrawDataList(m_imagesExpanded, m_images, UILabels.Images);
            GUILayout.Space(DGUI.Properties.Space());
            DrawDataList(m_buttonsExpanded, m_buttons, UILabels.Buttons);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawProgressors();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawUnityEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuntimeOptions()
        {
            AnimBool expanded = GetAnimBool(UILabels.RuntimeOptions, EditorApplication.isPlaying);
            expanded.target = EditorApplication.isPlaying;

            if (DGUI.AlphaGroup.Begin(expanded))
            {
                var textSize = Size.XL;
                float buttonHeight = DGUI.Sizes.BarHeight(textSize);

                ColorName showButtonBackgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.Visibility == VisibilityState.NotVisible, ComponentColorName);
                ColorName showButtonTextColorName = DGUI.Colors.GetTextColorName(Target.Visibility == VisibilityState.NotVisible, ComponentColorName);
                ColorName hideButtonBackgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.Visibility == VisibilityState.Visible, ComponentColorName);
                ColorName hideButtonTextColorName = DGUI.Colors.GetTextColorName(Target.Visibility == VisibilityState.Visible, ComponentColorName);
                ColorName showingIconBackgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.Visibility == VisibilityState.Showing, ComponentColorName);
                ColorName hidingIconBackgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.Visibility == VisibilityState.Hiding, ComponentColorName);

                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Show, UILabels.Show, textSize, TextAlign.Left, showButtonBackgroundColorName, showButtonTextColorName, buttonHeight))
                        {
                            Target.gameObject.SetActive(true);
                            Target.Show(m_instantAction);
                        }

                        GUILayout.Space(DGUI.Properties.Space(4));

                        float alpha = GUI.color.a;
                        switch (Target.Visibility)
                        {
                            case VisibilityState.Visible:
                                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                                DGUI.Icon.Draw(DGUI.Icon.ArrowLeft, buttonHeight, hidingIconBackgroundColorName);
                                break;
                            case VisibilityState.NotVisible:
                                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                                DGUI.Icon.Draw(DGUI.Icon.ArrowRight, buttonHeight, showingIconBackgroundColorName);
                                break;
                            case VisibilityState.Hiding:
                                DGUI.Icon.Draw(DGUI.Icon.ArrowLeft, buttonHeight, hidingIconBackgroundColorName);
                                break;
                            case VisibilityState.Showing:
                                DGUI.Icon.Draw(DGUI.Icon.ArrowRight, buttonHeight, showingIconBackgroundColorName);
                                break;
                            default: throw new ArgumentOutOfRangeException();
                        }

                        GUI.color = GUI.color.WithAlpha(alpha);

                        GUILayout.Space(DGUI.Properties.Space(4));

                        if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Hide, UILabels.Hide, textSize, TextAlign.Left, hideButtonBackgroundColorName, hideButtonTextColorName, buttonHeight))
                        {
                            Target.gameObject.SetActive(true);
                            Target.Hide(m_instantAction);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space() * expanded.faded);
                    m_instantAction = DGUI.Toggle.Switch.Draw(m_instantAction, UILabels.PlayAnimationInZeroSeconds, ComponentColorName, serializedObject.isEditingMultipleObjects, true, false);
                    GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                }
                GUILayout.EndVertical();
            }

            DGUI.AlphaGroup.End();
        }

        private void DrawAddToPopupQueueAndDisplayTarget()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_addToPopupQueue, UILabels.AddToPopupQueue, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginVertical();
                {
                    DGUI.Property.Draw(m_displayTarget, UILabels.DisplayTarget, ComponentColorName, ComponentColorName);
                    m_canvasNameExpanded.target = Target.DisplayTarget == PopupDisplayOn.TargetCanvas;
                    m_canvasName.isExpanded = m_canvasNameExpanded.target;
                    if (DGUI.FadeOut.Begin(m_canvasNameExpanded, false))
                    {
                        GUILayout.Space(DGUI.Properties.Space() * m_canvasNameExpanded.faded);
                        DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UICanvas.DefaultCanvasCategory,
                                                                                      m_canvasName, UILabels.TargetCanvas,
                                                                                      m_customCanvasName,
                                                                                      CanvasDatabase,
                                                                                      ComponentColorName);
                    }

                    DGUI.FadeOut.End(m_canvasNameExpanded, false);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBehaviors()
        {
            DrawBehavior(UILabels.ShowPopup, Target.ShowBehavior, m_showBehavior, m_showDatabase, DGUI.Icon.Show, m_showExpanded);
            GUILayout.Space(DGUI.Properties.Space());
            DrawBehavior(UILabels.HidePopup, Target.HideBehavior, m_hideBehavior, m_hideDatabase, DGUI.Icon.Hide, m_hideExpanded);
            GUILayout.Space(DGUI.Properties.Space());
        }

        private void DrawBehavior(string behaviorName, UIPopupBehavior behavior, SerializedProperty behaviorProperty, UIAnimationsDatabase database, GUIStyle animationIcon, AnimBool behaviorExpanded)
        {
            SerializedProperty animationProperty = GetProperty(PropertyName.Animation, behaviorProperty);
            var animationType = (AnimationType) GetProperty(PropertyName.AnimationType, animationProperty).enumValueIndex;
            SerializedProperty startProperty = GetProperty(PropertyName.OnStart, behaviorProperty);
            SerializedProperty finishedProperty = GetProperty(PropertyName.OnFinished, behaviorProperty);
            SerializedProperty instantAnimationProperty = null;

            AnimBool animationExpanded = GetAnimBool(animationProperty.propertyPath, animationProperty.isExpanded);
            AnimBool startExpanded = GetAnimBool(startProperty.propertyPath, startProperty.isExpanded);
            AnimBool finishedExpanded = GetAnimBool(finishedProperty.propertyPath, finishedProperty.isExpanded);

            if (DGUI.Bar.Draw(behaviorName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, behaviorExpanded))
            {
                if (!behaviorExpanded.target)
                {
                    animationExpanded.target = false;
                    startExpanded.target = false;
                    finishedExpanded.target = false;
                }
                else
                {
                    animationExpanded.target = true;
                    startExpanded.target = false;
                    finishedExpanded.target = false;
                }

                SoundyAudioPlayer.StopAllPlayers();
            }

            GUILayout.Space(-NormalBarHeight);

            DrawBehaviorIcons(behavior, behaviorExpanded);

            switch (animationType)
            {
                case AnimationType.Show:
                    instantAnimationProperty = m_showInstantAnimation;

                    m_showInstantAnimationInfoMessage = GetInfoMessage(SHOW_INSTANT_ANIMATION);
                    GUILayout.Space(-DGUI.Properties.Space(2) * m_showInstantAnimationInfoMessage.Show.faded);
                    m_showInstantAnimationInfoMessage.DrawMessageOnly(behavior.InstantAnimation);
                    GUILayout.Space(DGUI.Properties.Space(3) * m_showInstantAnimationInfoMessage.Show.faded);

                    m_noShowAnimationInfoMessage = GetInfoMessage(NO_SHOW_ANIMATION);
                    GUILayout.Space(-DGUI.Properties.Space(2) * m_noShowAnimationInfoMessage.Show.faded);
                    m_noShowAnimationInfoMessage.Draw(!behavior.HasAnimation && !behavior.InstantAnimation, InspectorWidth);
                    GUILayout.Space(DGUI.Properties.Space(3) * m_noShowAnimationInfoMessage.Show.faded);

                    break;
                case AnimationType.Hide:
                    instantAnimationProperty = m_hideInstantAnimation;

                    m_hideInstantAnimationInfoMessage = GetInfoMessage(HIDE_INSTANT_ANIMATION);
                    GUILayout.Space(-DGUI.Properties.Space(2) * m_hideInstantAnimationInfoMessage.Show.faded);
                    m_hideInstantAnimationInfoMessage.DrawMessageOnly(behavior.InstantAnimation);
                    GUILayout.Space(DGUI.Properties.Space(3) * m_hideInstantAnimationInfoMessage.Show.faded);

                    m_noHideAnimationInfoMessage = GetInfoMessage(NO_HIDE_ANIMATION);
                    GUILayout.Space(-DGUI.Properties.Space(2) * m_noHideAnimationInfoMessage.Show.faded);
                    m_noHideAnimationInfoMessage.Draw(!behavior.HasAnimation && !behavior.InstantAnimation, InspectorWidth);
                    GUILayout.Space(DGUI.Properties.Space(3) * m_noHideAnimationInfoMessage.Show.faded);

                    break;
            }

            InfoMessage presetInfoMessage = GetInfoMessage(behavior.Animation.AnimationType.ToString());
            presetInfoMessage.Message = GetRuntimePresetCategoryAndName(behavior);
            GUILayout.Space(-DGUI.Properties.Space(2) * presetInfoMessage.Show.faded);
            presetInfoMessage.DrawMessageOnly(behavior.LoadSelectedPresetAtRuntime);
            GUILayout.Space(DGUI.Properties.Space(3) * presetInfoMessage.Show.faded);

            if (DGUI.FadeOut.Begin(behaviorExpanded, false))
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);

                    if (instantAnimationProperty != null)
                        GUI.enabled = !instantAnimationProperty.boolValue;

                    DrawAnimationPreset(behaviorExpanded,
                                        animationExpanded,
                                        behaviorProperty,
                                        GetProperty(PropertyName.PresetCategory, behaviorProperty),
                                        GetProperty(PropertyName.PresetName, behaviorProperty),
                                        database,
                                        behavior);

                    GUI.enabled = true;

                    GUILayout.BeginHorizontal();
                    {
                        //BUTTON ANIMATION
                        if (DGUI.Doozy.DrawSectionButtonLeft(animationExpanded.target,
                                                             UILabels.Animation,
                                                             animationIcon,
                                                             ComponentColorName,
                                                             DGUI.Doozy.GetUIAnimationIcons(behavior.Animation,
                                                                                            m_behaviorAnimationIconsDatabase)))
                        {
                            animationExpanded.target = true;
                            startExpanded.value = false;
                            finishedExpanded.value = false;
                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        GUILayout.Space(DGUI.Properties.Space());

                        //BUTTON AT START
                        if (DGUI.Doozy.DrawSectionButtonMiddle(startExpanded.target,
                                                               UILabels.AtStart,
                                                               DGUI.Icon.ActionStart,
                                                               ComponentColorName,
                                                               DGUI.Doozy.GetActionsIcons(behavior.OnStart,
                                                                                          m_behaviorActionsIconsDatabase,
                                                                                          ComponentColorName)))
                        {
                            animationExpanded.target = false;
                            startExpanded.target = true;
                            finishedExpanded.value = false;
                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        GUILayout.Space(DGUI.Properties.Space());

                        //BUTTON AT FINISH
                        if (DGUI.Doozy.DrawSectionButtonRight(finishedExpanded.target,
                                                              UILabels.AtFinish,
                                                              DGUI.Icon.ActionFinished,
                                                              ComponentColorName,
                                                              DGUI.Doozy.GetActionsIcons(behavior.OnFinished,
                                                                                         m_behaviorActionsIconsDatabase,
                                                                                         ComponentColorName)))
                        {
                            animationExpanded.target = false;
                            startExpanded.value = false;
                            finishedExpanded.target = true;
                            SoundyAudioPlayer.StopAllPlayers();
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (behaviorExpanded.target && !animationExpanded.target && !startExpanded.target && !finishedExpanded.target)
                        animationExpanded.value = true;

                    animationProperty.isExpanded = animationExpanded.target;
                    startProperty.isExpanded = startExpanded.target;
                    finishedProperty.isExpanded = finishedExpanded.target;

                    GUILayout.Space(DGUI.Properties.Space(3) * behaviorExpanded.faded);

                    if (animationType == AnimationType.Loop)
                    {
                        if (DGUI.FadeOut.Begin(animationExpanded, false))
                        {
                            DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.AutoStartLoopAnimation, behaviorProperty), UILabels.AutoStartLoopAnimation, ComponentColorName, true, true, DGUI.Properties.SingleLineHeight);
                            GUILayout.Space(DGUI.Properties.Space(2) * animationExpanded.faded);
                        }

                        DGUI.FadeOut.End(animationExpanded, false);
                    }

                    DrawBehaviorAnimation(behavior, animationProperty, instantAnimationProperty, animationExpanded);
                    DrawBehaviorActions(behavior.OnStart, startProperty, startExpanded, animationType + "Behavior.OnStart");
                    DrawBehaviorActions(behavior.OnFinished, finishedProperty, finishedExpanded, animationType + "Behavior.OnFinished");
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(behaviorExpanded);

            behaviorProperty.isExpanded = behaviorExpanded.target;
        }

        private void DrawAnimationPreset(AnimBool behaviorExpanded, AnimBool animationExpanded, SerializedProperty behaviorProperty, SerializedProperty presetCategoryProperty, SerializedProperty presetNameProperty, UIAnimationsDatabase database, UIPopupBehavior behavior)
        {
            if (DGUI.FadeOut.Begin(animationExpanded.faded, false))
            {
                if (UIAnimationUtils.DrawAnimationPreset(serializedObject,
                                                         GetProperty(PropertyName.LoadSelectedPresetAtRuntime, behaviorProperty),
                                                         presetCategoryProperty,
                                                         presetNameProperty,
                                                         database,
                                                         behavior.Animation,
                                                         ComponentColorName,
                                                         out behavior.Animation))
                {
                    if (serializedObject.isEditingMultipleObjects)
                        foreach (Object targetObject in serializedObject.targetObjects)
                        {
                            var targetView = (UIView) targetObject;
                            switch (behavior.Animation.AnimationType)
                            {
                                case AnimationType.Show:
                                    targetView.ShowBehavior.PresetCategory = behavior.PresetCategory;
                                    targetView.ShowBehavior.PresetName = behavior.PresetName;
                                    targetView.ShowBehavior.Animation = behavior.Animation.Copy();
                                    break;
                                case AnimationType.Hide:
                                    targetView.HideBehavior.PresetCategory = behavior.PresetCategory;
                                    targetView.HideBehavior.PresetName = behavior.PresetName;
                                    targetView.HideBehavior.Animation = behavior.Animation.Copy();
                                    break;
                                case AnimationType.Loop:
                                    targetView.LoopBehavior.PresetCategory = behavior.PresetCategory;
                                    targetView.LoopBehavior.PresetName = behavior.PresetName;
                                    targetView.LoopBehavior.Animation = behavior.Animation.Copy();
                                    break;
                            }
                        }
                }


                GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);
            }

            DGUI.FadeOut.End(animationExpanded.faded, false);
        }

        private void DrawBehaviorIcons(UIPopupBehavior behavior, AnimBool expanded)
        {
            if (DGUI.AlphaGroup.Begin(Mathf.Clamp(1 - expanded.faded, 0.2f, 1f)))
            {
                GUILayout.BeginHorizontal(GUILayout.Height(NormalBarHeight));
                {
                    GUILayout.FlexibleSpace();

                    if (!m_viewBehaviorIconsDatabase.ContainsKey(behavior)) m_viewBehaviorIconsDatabase.Add(behavior, new List<DGUI.IconGroup.Data>());

                    //GET Animation Icons
                    m_viewBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorUIAnimationIcons(m_viewBehaviorIconsDatabase[behavior],
                                                                                                   behavior.Animation.AnimationType,
                                                                                                   behavior.Animation.Move.Enabled,
                                                                                                   behavior.Animation.Rotate.Enabled,
                                                                                                   behavior.Animation.Scale.Enabled,
                                                                                                   behavior.Animation.Fade.Enabled,
                                                                                                   ComponentColorName);

                    //DRAW Animation Icons
                    if (m_viewBehaviorIconsDatabase[behavior].Count > 0 && !behavior.InstantAnimation)
                        DGUI.IconGroup.Draw(m_viewBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);

                    //GET Actions Icons
                    m_viewBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorActionsIcons(m_viewBehaviorIconsDatabase[behavior],
                                                                                               behavior.HasSound,
                                                                                               behavior.HasEffect,
                                                                                               behavior.HasAnimatorEvents,
                                                                                               behavior.HasGameEvents,
                                                                                               behavior.HasUnityEvents,
                                                                                               ComponentColorName);

                    //DRAW Actions Icons
                    if (m_viewBehaviorIconsDatabase[behavior].Count > 0)
                    {
                        GUILayout.Space(DGUI.Properties.Space(4));
                        DGUI.IconGroup.Draw(m_viewBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);
                    }

                    GUILayout.Space(DGUI.Properties.Space(3));
                }
                GUILayout.EndHorizontal();
            }

            DGUI.AlphaGroup.End();
        }

        private void DrawBehaviorAnimation(UIPopupBehavior behavior, SerializedProperty animationProperty, SerializedProperty instantAnimationProperty, AnimBool expanded)
        {
            float alpha = GUI.color.a;
            if (DGUI.FadeOut.Begin(expanded.faded, false))
            {
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
                    if (instantAnimationProperty != null)
                    {
                        DGUI.Toggle.Switch.Draw(instantAnimationProperty, UILabels.InstantAnimation, ComponentColorName, true, true, DGUI.Properties.SingleLineHeight);
                        GUILayout.Space(DGUI.Properties.Space(2) * expanded.faded);
                        GUI.enabled = !instantAnimationProperty.boolValue;
                    }

                    DGUI.Doozy.DrawPreviewAnimationButtons(serializedObject, Target, behavior, ComponentColorName);

                    GUILayout.Space(DGUI.Properties.Space(2) * expanded.faded);

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

                    GUILayout.Space(DGUI.Properties.Space(4) * expanded.faded);

                    GUI.enabled = true;
                }

                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(expanded, true, alpha);
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

            DGUI.FadeOut.End(expanded, true, alpha);
        }

        private void DrawContainer() { DGUI.Property.Draw(GetProperty(PropertyName.RectTransform, m_container), UILabels.Container, ComponentColorName, ComponentColorName, !Target.HasContainer); }

        private void DrawOverlay()
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.UseOverlay, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.UseOverlay, ComponentColorName);

            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.Enabled, m_overlay), UILabels.Enabled, ComponentColorName, true, false);
                if (EditorGUI.EndChangeCheck()) UpdateOverlayEnabledState();
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(GetProperty(PropertyName.RectTransform, m_overlay), UILabels.Overlay, backgroundColorName, textColorName, !Target.HasOverlay);
            }
            GUILayout.EndHorizontal();
        }

        private void UpdateOverlayEnabledState()
        {
            if (!Target.HasOverlay) return;
            Target.Overlay.Init();
            bool enabled = GetProperty(PropertyName.Enabled, m_overlay).boolValue;
            Target.Overlay.RectTransform.gameObject.SetActive(enabled);
            Target.Overlay.Canvas.enabled = enabled;
            Target.Overlay.GraphicRaycaster.enabled = enabled;
        }

        private void DrawDataList(AnimBool expanded, SerializedProperty property, string propertyName)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            if (DGUI.FadeOut.Begin(expanded))
            {
                DGUI.List.Draw(property, ComponentColorName, UILabels.Empty);
            }

            DGUI.FadeOut.End(expanded);
            property.isExpanded = expanded.target;
        }

        private void DrawProgressors()
        {
            DrawProgressor(m_showProgressor, UILabels.ShowProgressor, m_updateShowProgressorOnHide, UILabels.UpdateOnHide);
            GUILayout.Space(DGUI.Properties.Space());
            DrawProgressor(m_hideProgressor, UILabels.HideProgressor, m_updateHideProgressorOnShow, UILabels.UpdateOnShow);
        }

        private void DrawProgressor(SerializedProperty progressorProperty, string progressorLabel, SerializedProperty updateProperty, string updateLabel)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(progressorProperty, progressorLabel, ComponentColorName);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(updateProperty, updateLabel, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawUnityEvents()
        {
            DrawUnityEvent(m_onVisibilityChangedExpanded, m_onVisibilityChanged, "OnVisibilityChanged", OnVisibilityChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space());
            DrawUnityEvent(m_onInverseVisibilityChangedExpanded, m_onInverseVisibilityChanged, "OnInverseVisibilityChanged", OnInverseVisibilityChangedEventCount);
        }

        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }

        private void DrawHideOptions()
        {
            bool enabled = Target.HideOnBackButton ||
                           Target.HideOnClickOverlay ||
                           Target.HideOnClickAnywhere ||
                           Target.HideOnClickContainer;
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
                DGUI.Label.Draw(UILabels.HideUIPopupBy, Size.S, textColorName);
                GUI.color = GUI.color.WithAlpha(alpha);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnAnyButton, UILabels.AnyButton, m_hideOnAnyButton.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnBackButton, UILabels.BackButton, m_hideOnBackButton.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnClickAnywhere, UILabels.ClickAnywhere, m_hideOnClickAnywhere.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnClickOverlay, UILabels.ClickOverlay, m_hideOnClickOverlay.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnClickContainer, UILabels.ClickContainer, m_hideOnClickContainer.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
        }

        private void DrawAutoHide()
        {
            m_autoHideExpanded.target = m_autoHideAfterShow.boolValue;

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(m_autoHideAfterShow.boolValue, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(m_autoHideAfterShow.boolValue, ComponentColorName);

            float alpha = GUI.color.a;
            DGUI.Line.Draw(true, backgroundColorName,
                           () =>
                           {
                               DGUI.Toggle.Switch.Draw(m_autoHideAfterShow, backgroundColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space());
                               GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(m_autoHideAfterShow.boolValue));
                               DGUI.Label.Draw(UILabels.AutoHideAfterShow, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                               GUI.color = GUI.color.WithAlpha(alpha);
                               if (DGUI.AlphaGroup.Begin(m_autoHideExpanded.faded))
                               {
                                   GUILayout.Space(DGUI.Properties.Space());
                                   DGUI.Label.Draw(UILabels.After, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                   DGUI.Property.Draw(m_autoHideAfterShowDelay, ComponentColorName, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                                   DGUI.Label.Draw(UILabels.SecondsDelay, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               }

                               DGUI.AlphaGroup.End(alpha);
                           },
                           GUILayout.FlexibleSpace);
        }

        private void DrawDisableBackButton() { DGUI.Toggle.Switch.Draw(m_blockBackButton, UILabels.BlockBackButton, ComponentColorName, true, true); }

        private void DrawDestroyAfterHide() { DGUI.Toggle.Switch.Draw(m_destroyAfterHide, UILabels.DestroyAfterHide, ComponentColorName, true, true); }

        private void DrawAutoSelectButton()
        {
            m_autoSelectButtonExpanded.target = m_autoSelectButtonAfterShow.boolValue;

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(m_autoSelectButtonAfterShow.boolValue, ComponentColorName);

            DGUI.Line.Draw(false, backgroundColorName,
                           () =>
                           {
                               DGUI.Toggle.Switch.Draw(m_autoSelectButtonAfterShow, UILabels.AutoSelectButtonAfterShow, backgroundColorName, false, true);
                               GUILayout.Space(DGUI.Properties.Space());
                               DGUI.Property.DrawWithFade(m_selectedButton, m_autoSelectButtonExpanded, DGUI.Properties.SingleLineHeight, ComponentColorName);
                               GUILayout.FlexibleSpace();
                           });
        }

        private void DrawAutoSelectPreviouslySelectedButtonAfterHide()
        {
            DGUI.Toggle.Switch.Draw(m_autoSelectPreviouslySelectedButtonAfterHide, UILabels.AutoSelectPreviouslySelectedButtonAfterHide, ComponentColorName, true, true);
        }
        
        private void DrawWhenHidden()
        {
            SerializedProperty disableGameObject = GetProperty(PropertyName.DisableGameObject, m_container);
            SerializedProperty disableCanvas = GetProperty(PropertyName.DisableCanvas, m_container);
            SerializedProperty disableGraphicRaycaster = GetProperty(PropertyName.DisableGraphicRaycaster, m_container);

            bool enabled = Target.HasContainer &&
                           Target.Container.DisableGameObject || Target.Container.DisableCanvas || Target.Container.GraphicRaycaster;

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
                DGUI.Label.Draw(UILabels.WhenUIPopupIsHiddenDisable, Size.S, textColorName);
                GUI.color = GUI.color.WithAlpha(alpha);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(disableGameObject, UILabels.GameObject, disableGameObject.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(disableCanvas, UILabels.Canvas, disableCanvas.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(disableGraphicRaycaster, UILabels.GraphicRaycaster, disableGraphicRaycaster.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName, false, true);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }
    }
}