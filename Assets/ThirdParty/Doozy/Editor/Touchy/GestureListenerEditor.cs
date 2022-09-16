// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Touchy;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Touchy
{
    [CustomEditor(typeof(GestureListener))]
    [CanEditMultipleObjects]
    public class GestureListenerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.GestureListenerColorName; } }
        private GestureListener m_target;

        private GestureListener Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (GestureListener) target;
                return m_target;
            }
        }

        private const string EVERY_TAP_WILL_TRIGGER_THIS_LISTENER = "Tap";
        private const string EVERY_LONG_TAP_WILL_TRIGGER_THIS_LISTENER = "LongTap";
        private const string EVERY_SWIPE_WILL_TRIGGER_THIS_LISTENER = "Swipe";
        private const string SET_TARGET_GAME_OBJECT = "SetTarget";
        private const string SELECT_SWIPE_DIRECTION = "SelectSwipeDirection";

        private SerializedProperty m_globalListenerProperty,
                                   m_overrideTargetProperty,
                                   m_targetGameObjectProperty,
                                   m_gestureTypeProperty,
                                   m_swipeDirectionProperty,
                                   m_onGestureEventProperty,
                                   m_gameEventsProperty;

        private AnimBool m_showOverrideTargetAnimBool;

        private bool ShowOverrideTargetAnimBool { get { return !Target.GlobalListener; } }
        private bool SwipeDirectionEnabled { get { return Target.GestureType == GestureType.Swipe; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_globalListenerProperty = GetProperty(PropertyName.GlobalListener);
            m_overrideTargetProperty = GetProperty(PropertyName.OverrideTarget);
            m_targetGameObjectProperty = GetProperty(PropertyName.TargetGameObject);
            m_gestureTypeProperty = GetProperty(PropertyName.GestureType);
            m_swipeDirectionProperty = GetProperty(PropertyName.SwipeDirection);
            m_onGestureEventProperty = GetProperty(PropertyName.OnGestureEvent);
            m_gameEventsProperty = GetProperty(PropertyName.GameEvents);

            m_showOverrideTargetAnimBool = GetAnimBool(m_overrideTargetProperty.propertyPath, ShowOverrideTargetAnimBool);

            AddInfoMessage(EVERY_TAP_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.EveryTapWillTriggerThisListener, false, Repaint));
            AddInfoMessage(EVERY_LONG_TAP_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.EveryLongTapWillTriggerThisListener, false, Repaint));
            AddInfoMessage(EVERY_SWIPE_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.EverySwipeWillTriggerThisListener, false, Repaint));
            AddInfoMessage(SET_TARGET_GAME_OBJECT, new InfoMessage(InfoMessage.MessageType.Error, UILabels.SetTargetGameObject, false, Repaint));
            AddInfoMessage(SELECT_SWIPE_DIRECTION, new InfoMessage(InfoMessage.MessageType.Error, UILabels.SelectSwipeDirection, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderGestureListener), MenuUtils.GestureListener_Manual, MenuUtils.GestureListener_YouTube);
            DrawDebugModeAndTouchySettings();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawIsGlobalListener();
            m_showOverrideTargetAnimBool.target = ShowOverrideTargetAnimBool;
            GUILayout.Space(DGUI.Properties.Space() * m_showOverrideTargetAnimBool.faded);
            if (DGUI.FadeOut.Begin(m_showOverrideTargetAnimBool, false))
                DrawOverrideTargetAndTargetGameObject();
            DGUI.FadeOut.End(m_showOverrideTargetAnimBool, false);
            GUILayout.Space(DGUI.Properties.Space());
            DrawGestureTypeAndSwipeDirection();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawOnGestureEvent();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawGameEvents();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebugModeAndTouchySettings()
        {
            GUILayout.BeginHorizontal();
            DGUI.Doozy.DrawDebugMode(GetProperty(PropertyName.DebugMode), ColorName.Red);
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconTouchy),
                                                   UILabels.TouchySettings,
                                                   Size.S, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.Touchy);
            GUILayout.EndHorizontal();
        }

        private void DrawIsGlobalListener()
        {
            DGUI.Toggle.Switch.Draw(m_globalListenerProperty, UILabels.GlobalListener, ComponentColorName, true, false);
            GetInfoMessage(EVERY_TAP_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_globalListenerProperty.boolValue && Target.GestureType == GestureType.Tap);
            GetInfoMessage(EVERY_LONG_TAP_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_globalListenerProperty.boolValue && Target.GestureType == GestureType.LongTap);
            GetInfoMessage(EVERY_SWIPE_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_globalListenerProperty.boolValue && Target.GestureType == GestureType.Swipe && Target.SwipeDirection != Swipe.None);
        }

        private void DrawOverrideTargetAndTargetGameObject()
        {
            ColorName backgroundColorName = m_overrideTargetProperty.boolValue ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName textColorName = m_overrideTargetProperty.boolValue ? ComponentColorName : DGUI.Colors.DisabledTextColorName;
            GUILayout.BeginHorizontal();
            {
                bool enabled = GUI.enabled;
                GUI.enabled = m_overrideTargetProperty.boolValue;
                DGUI.Property.Draw(m_targetGameObjectProperty, UILabels.TargetGameObject, backgroundColorName, textColorName);
                GUI.enabled = enabled;
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_overrideTargetProperty, UILabels.Override, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
            GetInfoMessage(SET_TARGET_GAME_OBJECT).DrawMessageOnly(!m_globalListenerProperty.boolValue && Target.TargetGameObject == null);
        }

        private void DrawGestureTypeAndSwipeDirection()
        {
            bool hasValidSwipeDirection = Target.SwipeDirection != Swipe.None;
            ColorName backgroundColorName = SwipeDirectionEnabled && hasValidSwipeDirection ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName textColorName = SwipeDirectionEnabled && hasValidSwipeDirection ? ComponentColorName : DGUI.Colors.DisabledTextColorName;

            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_gestureTypeProperty, UILabels.GestureType, ComponentColorName, ComponentColorName);
                GUILayout.Space(DGUI.Properties.Space());
                bool enabled = GUI.enabled;
                GUI.enabled = SwipeDirectionEnabled;
                DGUI.Property.Draw(m_swipeDirectionProperty, UILabels.SwipeDirection, backgroundColorName, textColorName);
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
            GetInfoMessage(SELECT_SWIPE_DIRECTION).DrawMessageOnly(Target.GestureType == GestureType.Swipe && Target.SwipeDirection == Swipe.None);
        }

        private void DrawOnGestureEvent()
        {
            ColorName backgroundColorName = Target.OnGestureEvent.GetPersistentEventCount() > 0 ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName textColorName = Target.OnGestureEvent.GetPersistentEventCount() > 0 ? ComponentColorName : DGUI.Colors.DisabledTextColorName;
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconUnityEvent), UILabels.UnityEvent, Size.M, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), backgroundColorName, textColorName);
            DGUI.Property.UnityEvent(m_onGestureEventProperty, "OnGestureEvent", ComponentColorName, Target.OnGestureEvent.GetPersistentEventCount());
        }

        private void DrawGameEvents()
        {
            ColorName backgroundColorName = m_gameEventsProperty.arraySize > 0 ? ComponentColorName : DGUI.Colors.DisabledBackgroundColorName;
            ColorName textColorName = m_gameEventsProperty.arraySize > 0 ? ComponentColorName : DGUI.Colors.DisabledTextColorName;
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconGameEvent), UILabels.GameEvents, Size.M, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.List.Draw(m_gameEventsProperty, ComponentColorName, UILabels.NotSendingAnyGameEventTitle);
        }
    }
}