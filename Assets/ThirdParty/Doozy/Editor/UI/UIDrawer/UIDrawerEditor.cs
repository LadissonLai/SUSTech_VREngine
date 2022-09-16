// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Internal;
using Doozy.Editor.Soundy;
using Doozy.Editor.Windows;
using Doozy.Engine.Extensions;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIDrawer))]
    [CanEditMultipleObjects]
    public class UIDrawerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIDrawerColorName; } }
        private UIDrawer m_target;

        private UIDrawer Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIDrawer) target;
                return m_target;
            }
        }

        private static UIDrawerSettings Settings { get { return UIDrawerSettings.Instance; } }
        private static NamesDatabase Database { get { return UIDrawerSettings.Database; } }

        private readonly Dictionary<UIAction, List<DGUI.IconGroup.Data>> m_behaviorActionsIconsDatabase = new Dictionary<UIAction, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<UIDrawerBehavior, List<DGUI.IconGroup.Data>> m_drawerBehaviorIconsDatabase = new Dictionary<UIDrawerBehavior, List<DGUI.IconGroup.Data>>();

        private SerializedProperty
            m_drawerName,
            m_customDrawerName,
            m_closeDirection,
            m_blockBackButton,
            m_hideOnBackButton,
            m_detectGestures,
            m_useCustomStartAnchoredPosition,
            m_customStartAnchoredPosition,
            m_openSpeed,
            m_closeSpeed,
            m_openBehavior,
            m_closeBehavior,
            m_dragBehavior,
            m_container,
            m_overlay,
            m_arrow,
            m_progressor,
            m_onProgressChanged,
            m_onInverseProgressChanged;

        private AnimBool
            m_useCustomStartPositionExpanded,
            m_openExpanded,
            m_closeExpanded,
            m_dragExpanded,
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded,
            m_containerExpanded,
            m_overlayExpanded,
            m_arrowExpanded,
            m_arrowPositionButtonsExpanded,
            m_arrowReferencesExpanded,
            m_onProgressChangedExpanded,
            m_onInverseProgressChangedExpanded;

        private int OnProgressChangedEventCount { get { return Target.OnProgressChanged.GetPersistentEventCount(); } }
        private int OnInverseProgressChangedEventCount { get { return Target.OnInverseProgressChanged.GetPersistentEventCount(); } }

        private bool m_instantAction;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_drawerName = GetProperty(PropertyName.DrawerName);
            m_customDrawerName = GetProperty(PropertyName.CustomDrawerName);
            m_closeDirection = GetProperty(PropertyName.CloseDirection);
            m_blockBackButton = GetProperty(PropertyName.BlockBackButton);
            m_hideOnBackButton = GetProperty(PropertyName.HideOnBackButton);
            m_detectGestures = GetProperty(PropertyName.DetectGestures);
            m_useCustomStartAnchoredPosition = GetProperty(PropertyName.UseCustomStartAnchoredPosition);
            m_customStartAnchoredPosition = GetProperty(PropertyName.CustomStartAnchoredPosition);
            m_openSpeed = GetProperty(PropertyName.OpenSpeed);
            m_closeSpeed = GetProperty(PropertyName.CloseSpeed);
            m_openBehavior = GetProperty(PropertyName.OpenBehavior);
            m_closeBehavior = GetProperty(PropertyName.CloseBehavior);
            m_dragBehavior = GetProperty(PropertyName.DragBehavior);
            m_container = GetProperty(PropertyName.Container);
            m_overlay = GetProperty(PropertyName.Overlay);
            m_arrow = GetProperty(PropertyName.Arrow);
            m_progressor = GetProperty(PropertyName.Progressor);
            m_onProgressChanged = GetProperty(PropertyName.OnProgressChanged);
            m_onInverseProgressChanged = GetProperty(PropertyName.OnInverseProgressChanged);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_useCustomStartPositionExpanded = GetAnimBool(m_useCustomStartAnchoredPosition.propertyPath, m_useCustomStartAnchoredPosition.boolValue);

            m_openExpanded = GetAnimBool(m_openBehavior.propertyPath, m_openBehavior.isExpanded);
            m_closeExpanded = GetAnimBool(m_closeBehavior.propertyPath, m_closeBehavior.isExpanded);
            m_dragExpanded = GetAnimBool(m_dragBehavior.propertyPath, m_dragBehavior.isExpanded);

            m_soundDataExpanded = GetAnimBool("SOUND");
            m_effectExpanded = GetAnimBool("EFFECT");
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");

            m_containerExpanded = GetAnimBool(m_container.propertyPath, m_container.isExpanded);
            m_overlayExpanded = GetAnimBool(m_overlay.propertyPath, m_overlay.isExpanded);
            m_arrowExpanded = GetAnimBool(m_arrow.propertyPath, m_arrow.isExpanded);
            m_arrowPositionButtonsExpanded = GetAnimBool("ArrowPositionButtons", Target.ArrowEnabled);
            m_arrowReferencesExpanded = GetAnimBool("ArrowReferences");
            m_onProgressChangedExpanded = GetAnimBool(m_onProgressChanged.propertyPath, OnProgressChangedEventCount > 0);
            m_onInverseProgressChangedExpanded = GetAnimBool(m_onInverseProgressChanged.propertyPath, OnInverseProgressChangedEventCount > 0);
        }

        public override bool RequiresConstantRepaint() { return true; }

        #region OnSceneGUI

        private void OnSceneGUI() { DrawDrawerNameAndCloseDirectionOnSceneGUI(); }

        private RectTransform m_arrowRoot, m_arrowClosed, m_arrowOpened;

//        private Quaternion m_arrowClosedHandleRotation, m_arrowOpenedHandleRotation;
        private const float HANDLE_SIZE = 0.4f;

        private void DrawDrawerNameAndCloseDirectionOnSceneGUI()
        {
//            Handles.Label(Target.transform.position, "UIDrawer: " + Target.DrawerName +
//                                                     "\n" +
//                                                     UILabels.CloseDirection + ": " + Target.CloseDirection);

            if (!Target.ArrowEnabled || Target.CloseDirection == SimpleSwipe.None) return;

            switch (Target.CloseDirection)
            {
                case SimpleSwipe.Left:
                    m_arrowRoot = Target.Arrow.Left.Root;
                    m_arrowClosed = Target.Arrow.Left.Closed;
                    m_arrowOpened = Target.Arrow.Left.Opened;
//                    m_arrowClosedHandleRotation = Quaternion.Euler(0, 90, 0);
//                    m_arrowOpenedHandleRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case SimpleSwipe.Right:
                    m_arrowRoot = Target.Arrow.Right.Root;
                    m_arrowClosed = Target.Arrow.Right.Closed;
                    m_arrowOpened = Target.Arrow.Right.Opened;
//                    m_arrowClosedHandleRotation = Quaternion.Euler(0, -90, 0);
//                    m_arrowOpenedHandleRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case SimpleSwipe.Up:
                    m_arrowRoot = Target.Arrow.Up.Root;
                    m_arrowClosed = Target.Arrow.Up.Closed;
                    m_arrowOpened = Target.Arrow.Up.Opened;
//                    m_arrowClosedHandleRotation = Quaternion.Euler(90, 90, 0);
//                    m_arrowOpenedHandleRotation = Quaternion.Euler(-90, -90, 0);
                    break;
                case SimpleSwipe.Down:
                    m_arrowRoot = Target.Arrow.Down.Root;
                    m_arrowClosed = Target.Arrow.Down.Closed;
                    m_arrowOpened = Target.Arrow.Down.Opened;
//                    m_arrowClosedHandleRotation = Quaternion.Euler(-90, 90, 0);
//                    m_arrowOpenedHandleRotation = Quaternion.Euler(90, -90, 0);
                    break;
            }

//            if (Handles.Button(m_arrowRoot.position, m_arrowRoot.rotation, m_handleSize, m_handleSize, Handles.SphereHandleCap))
//            {
//                Selection.activeGameObject = m_arrowRoot.gameObject;
//            }

            Vector3 rootPosition = m_arrowRoot.position;
            Vector3 closedPosition = m_arrowClosed.position;
            Vector3 openedPosition = m_arrowOpened.position;

            Handles.DrawDottedLine(rootPosition, closedPosition, HANDLE_SIZE);
            Handles.DrawDottedLine(rootPosition, openedPosition, HANDLE_SIZE);

//            Handles.Label(closedPosition, "CLOSED");
//            Handles.Label(openedPosition, "OPENED");

            Vector3 arrowRootPosition = rootPosition;
            EditorGUI.BeginChangeCheck();
            arrowRootPosition = Handles.PositionHandle(arrowRootPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_arrowRoot, "Update Arrow Root");
                m_arrowRoot.position = arrowRootPosition;
            }


            Vector3 arrowClosedPosition = closedPosition;
            EditorGUI.BeginChangeCheck();
            arrowClosedPosition = Handles.PositionHandle(arrowClosedPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_arrowClosed, "Update Closed Arrow");
                m_arrowClosed.position = arrowClosedPosition;
            }

            Vector3 arrowOpenedPosition = m_arrowOpened.position;
            EditorGUI.BeginChangeCheck();
            arrowOpenedPosition = Handles.PositionHandle(arrowOpenedPosition, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_arrowOpened, "Update Opened Arrow");
                m_arrowOpened.position = arrowOpenedPosition;
            }
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            AdjustPositionRotationAndScaleToRoundValues(Target.RectTransform);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
        }

        public override void OnInspectorGUI()
        {
            if (Target.CloseDirection == SimpleSwipe.None) Target.CloseDirection = SimpleSwipe.Left;

            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIDrawer), "UIDrawer", MenuUtils.UIDrawer_Manual, MenuUtils.UIDrawer_YouTube);
            DrawRuntimeOptions();
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawCloseDirectionHideOnBackButtonAndDetectGestures();
            GUILayout.Space(DGUI.Properties.Space());
            DrawDrawerName();
            GUILayout.Space(DGUI.Properties.Space());
            DrawRenameGameObjectAndOpenDatabaseButtons();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawCustomStartPosition();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawOpenAndCloseSpeed();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawBehaviors();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawContainer();
            GUILayout.Space(DGUI.Properties.Space());
            DrawOverlay();
            GUILayout.Space(DGUI.Properties.Space());
            DrawArrow();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawProgressor();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawUnityEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRuntimeOptions()
        {
            AnimBool expanded = GetAnimBool(DGUI.Properties.Labels.RuntimeOptions, EditorApplication.isPlaying);
            expanded.target = EditorApplication.isPlaying;

            if (DGUI.AlphaGroup.Begin(expanded))
            {
                var textSize = Size.XL;
                float buttonHeight = DGUI.Sizes.BarHeight(textSize);

                ColorName showButtonBackgroundColorName = Target.Visibility == VisibilityState.NotVisible ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
                ColorName showButtonTextColorName = Target.Visibility == VisibilityState.NotVisible ? ComponentColorName : DGUI.Colors.DisabledTextColorName;
                ColorName hideButtonBackgroundColorName = Target.Visibility == VisibilityState.Visible ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
                ColorName hideButtonTextColorName = Target.Visibility == VisibilityState.Visible ? ComponentColorName : DGUI.Colors.DisabledTextColorName;
                ColorName showingIconBackgroundColorName = Target.Visibility == VisibilityState.Showing ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
                ColorName hidingIconBackgroundColorName = Target.Visibility == VisibilityState.Hiding ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;

                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconShow),
                                                               DGUI.Properties.Labels.Show,
                                                               textSize, TextAlign.Left,
                                                               showButtonBackgroundColorName,
                                                               showButtonTextColorName,
                                                               buttonHeight))
                            Target.Open(m_instantAction);

                        GUILayout.Space(DGUI.Properties.Space(4));

                        float alpha = GUI.color.a;
                        switch (Target.Visibility)
                        {
                            case VisibilityState.Visible:
                                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                                DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconFaArrowAltLeft), buttonHeight, hidingIconBackgroundColorName);
                                break;
                            case VisibilityState.NotVisible:
                                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                                DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconFaArrowAltRight), buttonHeight, showingIconBackgroundColorName);
                                break;
                            case VisibilityState.Hiding:
                                DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconFaArrowAltLeft), buttonHeight, hidingIconBackgroundColorName);
                                break;
                            case VisibilityState.Showing:
                                DGUI.Icon.Draw(Styles.GetStyle(Styles.StyleName.IconFaArrowAltRight), buttonHeight, showingIconBackgroundColorName);
                                break;
                            default: throw new ArgumentOutOfRangeException();
                        }

                        GUI.color = GUI.color.WithAlpha(alpha);

                        GUILayout.Space(DGUI.Properties.Space(4));

                        if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconHide),
                                                               DGUI.Properties.Labels.Hide,
                                                               textSize, TextAlign.Left,
                                                               hideButtonBackgroundColorName,
                                                               hideButtonTextColorName,
                                                               buttonHeight))
                            Target.Close(m_instantAction);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space() * expanded.faded);
                    m_instantAction = DGUI.Toggle.Switch.Draw(m_instantAction, DGUI.Properties.Labels.PlayAnimationInZeroSeconds, ComponentColorName, serializedObject.isEditingMultipleObjects, true, false);
                    GUILayout.Space(DGUI.Properties.Space(8) * expanded.faded);
                }
                GUILayout.EndVertical();
            }

            DGUI.AlphaGroup.End();
        }

        private void DrawCloseDirectionHideOnBackButtonAndDetectGestures()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_closeDirection, UILabels.CloseDirection, ComponentColorName, Target.CloseDirection == SimpleSwipe.None);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_blockBackButton, UILabels.BlockBackButton, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_hideOnBackButton, UILabels.HideOnBackButton, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_detectGestures, UILabels.DetectGestures, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDrawerName()
        {
            DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UIDrawer.DefaultDrawerCategory,
                                                                          m_drawerName, UILabels.DrawerName,
                                                                          m_customDrawerName,
                                                                          Database,
                                                                          ComponentColorName);
        }

        private void DrawRenameGameObjectAndOpenDatabaseButtons()
        {
            DGUI.Doozy.DrawRenameGameObjectAndOpenDatabaseButtons(Settings.RenamePrefix,
                                                                  m_drawerName.stringValue,
                                                                  Settings.RenameSuffix,
                                                                  Target.gameObject,
                                                                  DoozyWindow.View.Drawers);
        }

        private void DrawCustomStartPosition()
        {
            DGUI.Doozy.DrawCustomStartPosition(Target.RectTransform,
                                               m_useCustomStartAnchoredPosition,
                                               m_customStartAnchoredPosition,
                                               m_useCustomStartPositionExpanded,
                                               ComponentColorName);
        }

        private void DrawOpenAndCloseSpeed()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_openSpeed, UILabels.OpenSpeed, ComponentColorName, ComponentColorName);
                GUILayout.Space(1);
                DGUI.Property.Draw(m_closeSpeed, UILabels.CloseSpeed, ComponentColorName, ComponentColorName);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBehaviors()
        {
            DrawBehavior(UILabels.OpenDrawer, Target.OpenBehavior, m_openBehavior, DGUI.Icon.Show, m_openExpanded);
            GUILayout.Space(DGUI.Properties.Space());
            DrawBehavior(UILabels.CloseDrawer, Target.CloseBehavior, m_closeBehavior, DGUI.Icon.Hide, m_closeExpanded);
            GUILayout.Space(DGUI.Properties.Space());
            DrawBehavior(UILabels.DragDrawer, Target.DragBehavior, m_dragBehavior, DGUI.Icon.Touchy, m_dragExpanded);
            GUILayout.Space(DGUI.Properties.Space());
        }

        private void DrawBehavior(string behaviorName, UIDrawerBehavior behavior, SerializedProperty behaviorProperty, GUIStyle behaviorIcon, AnimBool behaviorExpanded)
        {
            SerializedProperty drawerBehaviorTypeProperty = GetProperty(PropertyName.DrawerBehaviorType, behaviorProperty);
            var drawerBehaviorType = (UIDrawerBehaviorType) drawerBehaviorTypeProperty.enumValueIndex;
            SerializedProperty startProperty = GetProperty(PropertyName.OnStart, behaviorProperty);
            SerializedProperty finishedProperty = GetProperty(PropertyName.OnFinished, behaviorProperty);

            AnimBool startExpanded = GetAnimBool(startProperty.propertyPath, startProperty.isExpanded);
            AnimBool finishedExpanded = GetAnimBool(finishedProperty.propertyPath, finishedProperty.isExpanded);

            if (DGUI.Bar.Draw(behaviorName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, behaviorExpanded))
                SoundyAudioPlayer.StopAllPlayers();

            GUILayout.Space(-NormalBarHeight);

            DrawBehaviorIcons(behavior, behaviorExpanded);

            if (DGUI.FadeOut.Begin(behaviorExpanded, false))
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(DGUI.Properties.Space() * behaviorExpanded.faded);

                    GUILayout.BeginHorizontal();
                    {
                        if (DGUI.Doozy.DrawSectionButtonLeft(startExpanded.target,
                                                             UILabels.AtStart,
                                                             DGUI.Icon.ActionStart,
                                                             ComponentColorName,
                                                             DGUI.Doozy.GetActionsIcons(behavior.OnStart,
                                                                                        m_behaviorActionsIconsDatabase,
                                                                                        ComponentColorName)))
                        {
                            startExpanded.target = true;
                            finishedExpanded.value = false;
                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        GUILayout.Space(DGUI.Properties.Space());

                        if (DGUI.Doozy.DrawSectionButtonRight(finishedExpanded.target,
                                                              UILabels.AtFinish,
                                                              DGUI.Icon.ActionFinished,
                                                              ComponentColorName,
                                                              DGUI.Doozy.GetActionsIcons(behavior.OnFinished,
                                                                                         m_behaviorActionsIconsDatabase,
                                                                                         ComponentColorName)))
                        {
                            startExpanded.value = false;
                            finishedExpanded.target = true;
                            SoundyAudioPlayer.StopAllPlayers();
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (behaviorExpanded.target && !startExpanded.target && !finishedExpanded.target)
                        startExpanded.value = true;

                    startProperty.isExpanded = startExpanded.target;
                    finishedProperty.isExpanded = finishedExpanded.target;

                    GUILayout.Space(DGUI.Properties.Space(3) * behaviorExpanded.faded);

                    DrawBehaviorActions(behavior.OnStart, startProperty, startExpanded, drawerBehaviorType + "Behavior");
                    DrawBehaviorActions(behavior.OnFinished, finishedProperty, finishedExpanded, drawerBehaviorType + "Behavior");
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(behaviorExpanded);

            behaviorProperty.isExpanded = behaviorExpanded.target;
        }

        private void DrawBehaviorIcons(UIDrawerBehavior behavior, AnimBool expanded)
        {
            if (DGUI.AlphaGroup.Begin(Mathf.Clamp(1 - expanded.faded, 0.2f, 1f)))
            {
                GUILayout.BeginHorizontal(GUILayout.Height(NormalBarHeight));
                {
                    GUILayout.FlexibleSpace();

                    if (!m_drawerBehaviorIconsDatabase.ContainsKey(behavior)) m_drawerBehaviorIconsDatabase.Add(behavior, new List<DGUI.IconGroup.Data>());
                    //GET Actions Icons
                    m_drawerBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorActionsIcons(m_drawerBehaviorIconsDatabase[behavior],
                                                                                                 behavior.HasSound,
                                                                                                 behavior.HasEffect,
                                                                                                 behavior.HasAnimatorEvents,
                                                                                                 behavior.HasGameEvents,
                                                                                                 behavior.HasUnityEvents,
                                                                                                 ComponentColorName);

                    //DRAW Actions Icons
                    if (m_drawerBehaviorIconsDatabase[behavior].Count > 0)
                    {
                        GUILayout.Space(DGUI.Properties.Space(4));
                        DGUI.IconGroup.Draw(m_drawerBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);
                    }

                    GUILayout.Space(DGUI.Properties.Space(3));
                }
                GUILayout.EndHorizontal();
            }

            DGUI.AlphaGroup.End();
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

        private void DrawContainer()
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.HasContainer, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.HasContainer, ComponentColorName);

            DGUI.Bar.Draw(UILabels.Container, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, Target.HasContainer ? ComponentColorName : ColorName.Red, m_containerExpanded);

            if (DGUI.FadeOut.Begin(m_containerExpanded))
            {
                DGUI.Property.Draw(GetProperty(PropertyName.RectTransform, m_container), UILabels.Container, backgroundColorName, textColorName, !Target.HasContainer);
                GUILayout.Space(DGUI.Properties.Space());
                DrawContainerWhenClosed();
                GUILayout.Space(DGUI.Properties.Space(2));
                DrawContainerSize();
            }

            DGUI.FadeOut.End(m_containerExpanded);
        }

        private void DrawContainerWhenClosed()
        {
            SerializedProperty fadeOut = GetProperty(PropertyName.FadeOut, m_container);
            SerializedProperty disableGameObject = GetProperty(PropertyName.DisableGameObject, m_container);
            SerializedProperty disableCanvas = GetProperty(PropertyName.DisableCanvas, m_container);

            bool whenClosedEnabled = Target.HasContainer && (fadeOut.boolValue || disableGameObject.boolValue || disableCanvas.boolValue);

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(whenClosedEnabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(whenClosedEnabled, ComponentColorName);

            float backgroundHeight = DGUI.Properties.SingleLineHeight * 2 + DGUI.Properties.Space();
            DGUI.Background.Draw(backgroundColorName, GUILayout.Height(backgroundHeight));
            GUILayout.Space(-backgroundHeight);
            GUILayout.Space(DGUI.Properties.Space());
            float alpha = GUI.color.a;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.Space(2));
                GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(whenClosedEnabled));
                DGUI.Label.Draw(UILabels.WhenUIDrawerIsClosed, Size.S, textColorName);
                GUI.color = GUI.color.WithAlpha(alpha);
                GUILayout.Space(DGUI.Properties.Space(2));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(fadeOut, UILabels.FadeOutContainer, DGUI.Colors.GetBackgroundColorName(fadeOut.boolValue, backgroundColorName), false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(disableGameObject, UILabels.DisableGameObject, DGUI.Colors.GetBackgroundColorName(disableGameObject.boolValue, backgroundColorName), false, true);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(disableCanvas, UILabels.DisableCanvas, DGUI.Colors.GetBackgroundColorName(disableCanvas.boolValue, backgroundColorName), false, true);
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawContainerSize()
        {
            SerializedProperty size = GetProperty(PropertyName.Size, m_container);

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.HasContainer, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.HasContainer, ComponentColorName);

            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(size, UILabels.ContainerSize, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space());
                if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Reset, UILabels.UpdateContainer, Size.M, TextAlign.Left, backgroundColorName, textColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false)) UpdateContainerAndArrow();
            }
            GUILayout.EndHorizontal();

            bool percentageOfScreenSelected = (UIDrawerContainerSize) size.enumValueIndex == UIDrawerContainerSize.PercentageOfScreen;
            AnimBool containerPercentageOfScreenExpanded = GetAnimBool(UIDrawerContainerSize.PercentageOfScreen.ToString(), percentageOfScreenSelected);
            containerPercentageOfScreenExpanded.target = percentageOfScreenSelected;

            bool fixedSizeSelected = (UIDrawerContainerSize) size.enumValueIndex == UIDrawerContainerSize.FixedSize;
            AnimBool containerFixedSize = GetAnimBool(UIDrawerContainerSize.FixedSize.ToString(), fixedSizeSelected);
            containerFixedSize.target = fixedSizeSelected;

            //PercentageOfScreen
            if (DGUI.FadeOut.Begin(containerPercentageOfScreenExpanded, false))
            {
                GUILayout.Space(1);
                SerializedProperty percentageOfScreen = GetProperty(PropertyName.PercentageOfScreen, m_container);
                SerializedProperty minimumSize = GetProperty(PropertyName.MinimumSize, m_container);
                GUILayout.BeginHorizontal();
                {
                    DGUI.Property.Draw(percentageOfScreen, UILabels.PercentageOfScreenZeroToOne, backgroundColorName, textColorName);
                    GUILayout.Space(DGUI.Properties.Space());
                    DGUI.Property.Draw(minimumSize, UILabels.MinimumSize, backgroundColorName, textColorName);
                }
                GUILayout.EndHorizontal();
            }

            DGUI.FadeOut.End(containerPercentageOfScreenExpanded, false);

            //FixedSize
            if (DGUI.FadeOut.Begin(containerFixedSize, false))
            {
                GUILayout.Space(1);
                SerializedProperty fixedSize = GetProperty(PropertyName.FixedSize, m_container);
                DGUI.Property.Draw(fixedSize, UILabels.FixedSize, backgroundColorName, textColorName);
            }

            DGUI.FadeOut.End(containerFixedSize, false);
        }

        private void UpdateContainerAndArrow()
        {
            if (Target.HasArrow)
            {
                Undo.RecordObjects(new Object[] {Target.Container.RectTransform, Target.Arrow.Container}, "Update Container");
                Target.UpdateContainer();
                Target.UpdateArrowContainer();
            }
            else
            {
                Undo.RecordObject(Target.Container.RectTransform, "Update Container");
                Target.UpdateContainer();
            }
        }

        private void DrawOverlay()
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.HasOverlay, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.HasOverlay, ComponentColorName);

            DGUI.Bar.Draw(UILabels.Overlay, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, textColorName, m_overlayExpanded);

            if (DGUI.FadeOut.Begin(m_overlayExpanded))
            {
                DGUI.Property.Draw(GetProperty(PropertyName.RectTransform, m_overlay), UILabels.Overlay, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space(2));
            }

            DGUI.FadeOut.End(m_overlayExpanded);
        }

        private void DrawArrow()
        {
            ColorName barColorName = DGUI.Colors.GetTextColorName(Target.ArrowEnabled, ComponentColorName);

            DGUI.Bar.Draw(UILabels.Arrow, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, barColorName, m_arrowExpanded);

            if (DGUI.FadeOut.Begin(m_arrowExpanded))
            {
                DrawArrowEnabledScaleAndAnimator();
                GUILayout.Space(DGUI.Properties.Space());
                DrawArrowOverrideColor();
                GUILayout.Space(DGUI.Properties.Space());
                DrawArrowPositionButtons();
                GUILayout.Space(DGUI.Properties.Space());
                DrawArrowReferences();
            }

            DGUI.FadeOut.End(m_arrowExpanded);
        }

        private void DrawArrowEnabledScaleAndAnimator()
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.ArrowEnabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.ArrowEnabled, ComponentColorName);

            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.Enabled, m_arrow), UILabels.Enabled, backgroundColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(GetProperty(PropertyName.Scale, m_arrow), UILabels.Scale, backgroundColorName, textColorName);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawArrowOverrideColor()
        {
            SerializedProperty overrideColor = GetProperty(PropertyName.OverrideColor, m_arrow);

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.ArrowEnabled && overrideColor.boolValue, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.ArrowEnabled && overrideColor.boolValue, ComponentColorName);

            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(overrideColor, UILabels.OverrideColor, backgroundColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                bool enabled = GUI.enabled;
                GUI.enabled = overrideColor.boolValue;
                DGUI.Property.Draw(GetProperty(PropertyName.OpenedColor, m_arrow), UILabels.Opened, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(GetProperty(PropertyName.ClosedColor, m_arrow), UILabels.Closed, backgroundColorName, textColorName);
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawArrowPositionButtons()
        {
            if (DGUI.FadeOut.Begin(m_arrowPositionButtonsExpanded, false))
            {
                float buttonHeight = DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2);
                GUIStyle root = Styles.GetStyle(Styles.StyleName.IconFaCircle);
                GUIStyle closedArrow = null;
                GUIStyle openedArrow = null;

                switch (Target.CloseDirection)
                {
                    case SimpleSwipe.Left:
                        closedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronRight);
                        openedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronLeft);
                        break;
                    case SimpleSwipe.Right:
                        closedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronLeft);
                        openedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronRight);
                        break;
                    case SimpleSwipe.Up:
                        closedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronDown);
                        openedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronUp);
                        break;
                    case SimpleSwipe.Down:
                        closedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronUp);
                        openedArrow = Styles.GetStyle(Styles.StyleName.IconFaChevronDown);
                        break;
                }

                GUILayout.Space(DGUI.Properties.Space(2));
                GUILayout.BeginHorizontal();
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(closedArrow, UILabels.ResetClosedPosition, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                    {
                        Undo.RecordObject(Target.Arrow.GetHolder(Target.CloseDirection).Closed, "Reset Arrow Closed Position");
                        Target.Arrow.ResetArrowClosedPosition(Target.CloseDirection);
                    }

                    GUILayout.Space(DGUI.Properties.Space());

                    if (DGUI.Button.Dynamic.DrawIconButton(root, UILabels.ResetRoot, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                    {
                        Undo.RecordObject(Target.Arrow.GetHolder(Target.CloseDirection).Root, "Reset Arrow Root");
                        Target.Arrow.ResetArrowRootPosition(Target.CloseDirection);
                    }

                    GUILayout.Space(DGUI.Properties.Space());

                    if (DGUI.Button.Dynamic.DrawIconButton(openedArrow, UILabels.ResetOpenedPosition, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                    {
                        Undo.RecordObject(Target.Arrow.GetHolder(Target.CloseDirection).Opened, "Reset Arrow Opened Position");
                        Target.Arrow.ResetArrowOpenedPosition(Target.CloseDirection);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginHorizontal();
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Copy, UILabels.ClosedPosition + " = " + UILabels.OpenedPosition, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                    {
                        Undo.RecordObject(Target.Arrow.GetHolder(Target.CloseDirection).Closed, "Update Closed Position");
                        Target.Arrow.GetHolder(Target.CloseDirection).Closed.Copy(Target.Arrow.GetHolder(Target.CloseDirection).Opened);
                    }

                    GUILayout.Space(DGUI.Properties.Space());

                    if (DGUI.Button.Dynamic.DrawIconButton(DGUI.Icon.Copy, UILabels.OpenedPosition + " = " + UILabels.ClosedPosition, Size.M, TextAlign.Left, ComponentColorName, ComponentColorName, buttonHeight))
                    {
                        Undo.RecordObject(Target.Arrow.GetHolder(Target.CloseDirection).Opened, "Update Opened Position");
                        Target.Arrow.GetHolder(Target.CloseDirection).Opened.Copy(Target.Arrow.GetHolder(Target.CloseDirection).Closed);
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(DGUI.Properties.Space(2));
            }

            DGUI.FadeOut.End(m_arrowPositionButtonsExpanded, false);

            m_arrowPositionButtonsExpanded.target = Target.ArrowEnabled && Target.CloseDirection != SimpleSwipe.None;
        }

        private void DrawArrowReferences()
        {
            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(Target.ArrowEnabled, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(Target.ArrowEnabled, ComponentColorName);

            DGUI.Bar.Draw(UILabels.ArrowComponents, Size.M, DGUI.Bar.Caret.CaretType.Caret, backgroundColorName, m_arrowReferencesExpanded);
            if (DGUI.FadeOut.Begin(m_arrowReferencesExpanded))
            {
                SerializedProperty animator = GetProperty(PropertyName.Animator, m_arrow);
                SerializedProperty container = GetProperty(PropertyName.Container, m_arrow);
                SerializedProperty left = GetProperty(PropertyName.Left, m_arrow);
                SerializedProperty right = GetProperty(PropertyName.Right, m_arrow);
                SerializedProperty up = GetProperty(PropertyName.Up, m_arrow);
                SerializedProperty down = GetProperty(PropertyName.Down, m_arrow);

                DGUI.Property.Draw(animator, UILabels.Animator, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space() * m_arrowReferencesExpanded.faded);
                DGUI.Property.Draw(container, UILabels.Container, backgroundColorName, textColorName, container.objectReferenceValue == null);
                GUILayout.Space(DGUI.Properties.Space(2) * m_arrowReferencesExpanded.faded);
                DrawArrowReferenceHolder(left, UILabels.Left, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space() * m_arrowReferencesExpanded.faded);
                DrawArrowReferenceHolder(right, UILabels.Right, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space() * m_arrowReferencesExpanded.faded);
                DrawArrowReferenceHolder(up, UILabels.Up, backgroundColorName, textColorName);
                GUILayout.Space(DGUI.Properties.Space() * m_arrowReferencesExpanded.faded);
                DrawArrowReferenceHolder(down, UILabels.Down, backgroundColorName, textColorName);
            }

            DGUI.FadeOut.End(m_arrowReferencesExpanded);

            m_arrow.isExpanded = m_arrowReferencesExpanded.target;
        }

        private void DrawArrowReferenceHolder(SerializedProperty holder, string holderName, ColorName backgroundColorName, ColorName textColorName)
        {
            SerializedProperty root = GetProperty(PropertyName.Root, holder);
            SerializedProperty opened = GetProperty(PropertyName.Opened, holder);
            SerializedProperty closed = GetProperty(PropertyName.Closed, holder);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(DGUI.Properties.INDENT_WIDTH * m_arrowReferencesExpanded.faded);
                GUILayout.BeginVertical();
                {
                    DGUI.Property.Draw(root, holderName, backgroundColorName, textColorName, root.objectReferenceValue == null);
                    GUILayout.Space(1);
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(DGUI.Properties.INDENT_WIDTH * m_arrowReferencesExpanded.faded);
                        GUILayout.BeginVertical();
                        {
                            DGUI.Property.Draw(opened, UILabels.OpenedPosition, backgroundColorName, textColorName, opened.objectReferenceValue == null);
                            GUILayout.Space(1);
                            DGUI.Property.Draw(closed, UILabels.ClosedPosition, backgroundColorName, textColorName, closed.objectReferenceValue == null);
                            GUILayout.Space(1);
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawProgressor() { DGUI.Property.Draw(m_progressor, UILabels.Progressor, ComponentColorName); }

        private void DrawUnityEvents()
        {
            DrawUnityEvent(m_onProgressChangedExpanded, m_onProgressChanged, "OnProgressChanged", OnProgressChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space());
            DrawUnityEvent(m_onInverseProgressChangedExpanded, m_onInverseProgressChanged, "OnInverseProgressChanged", OnInverseProgressChangedEventCount);
        }

        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }
    }
}