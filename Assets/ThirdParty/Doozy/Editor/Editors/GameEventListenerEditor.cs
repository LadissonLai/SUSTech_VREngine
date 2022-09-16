// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor
{
    [CustomEditor(typeof(GameEventListener))]
    [CanEditMultipleObjects]
    public class GameEventListenerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.GameEventListenerColorName; } }
        private GameEventListener m_target;

        private GameEventListener Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (GameEventListener) target;
                return m_target;
            }
        }

        private const string ENTER_GAME_EVENT_TO_LISTEN_FOR = "EnterGameEventToListenFor";
        private const string ANY_GAME_EVENT_WILL_TRIGGER_THIS_LISTENER = "AnyGameEventWillTriggerThisListener";

        private SerializedProperty m_gameEventProperty,
                                   m_listenForAllGameEventsProperty,
                                   m_eventProperty;

        private AnimBool m_showGameEventAnimBool,
                         m_showEventAnimBool;

        public override bool RequiresConstantRepaint() { return true; }

        private bool ShowEvent { get { return m_listenForAllGameEventsProperty.boolValue || !string.IsNullOrEmpty(m_gameEventProperty.stringValue.Trim()); } }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_gameEventProperty = GetProperty(PropertyName.GameEvent);
            m_listenForAllGameEventsProperty = GetProperty(PropertyName.ListenForAllGameEvents);
            m_eventProperty = GetProperty(PropertyName.Event);

            m_showGameEventAnimBool = GetAnimBool(m_listenForAllGameEventsProperty.propertyPath, !m_listenForAllGameEventsProperty.boolValue);
            m_showEventAnimBool = GetAnimBool(m_eventProperty.propertyPath, ShowEvent);

            AddInfoMessage(ENTER_GAME_EVENT_TO_LISTEN_FOR, new InfoMessage(InfoMessage.MessageType.Error, UILabels.EnterGameEventToListenFor, false, Repaint));
            AddInfoMessage(ANY_GAME_EVENT_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.AnyGameEventWillTriggerThisListener, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderGameEventListener), MenuUtils.GameEventListener_Manual, MenuUtils.GameEventListener_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));

            DGUI.Toggle.Switch.Draw(m_listenForAllGameEventsProperty, UILabels.ListenForAllGameEvents, ComponentColorName, true, false);
            GetInfoMessage(ANY_GAME_EVENT_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_listenForAllGameEventsProperty.boolValue);
            m_showGameEventAnimBool.target = !m_listenForAllGameEventsProperty.boolValue;

            if (DGUI.FadeOut.Begin(m_showGameEventAnimBool, false))
            {
                GUILayout.Space(DGUI.Properties.Space(2) * m_showGameEventAnimBool.faded);
                DGUI.Line.Draw(false, ComponentColorName,
                               () =>
                               {
                                   GUILayout.Space(DGUI.Properties.Space(2));
                                   DGUI.Label.Draw(UILabels.GameEvent, Size.M, TextAlign.Left, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                   GUILayout.Space(DGUI.Properties.Space());
                                   DGUI.Property.Draw(m_gameEventProperty, ComponentColorName, DGUI.Properties.SingleLineHeight);
                                   GUILayout.Space(DGUI.Properties.Space(2));
                               });

                GetInfoMessage(ENTER_GAME_EVENT_TO_LISTEN_FOR).DrawMessageOnly(!ShowEvent);
            }

            DGUI.FadeOut.End(m_showGameEventAnimBool, false);

            GUILayout.Space(DGUI.Properties.Space(2));

            m_showEventAnimBool.target = ShowEvent;
            DGUI.Property.UnityEventWithFade(m_eventProperty, m_showEventAnimBool, UILabels.Event, ComponentColorName, Target.Event.GetPersistentEventCount());

            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
    }
}