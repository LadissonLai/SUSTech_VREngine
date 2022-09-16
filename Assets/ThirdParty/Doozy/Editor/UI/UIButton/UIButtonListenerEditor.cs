// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIButtonListener))]
    [CanEditMultipleObjects]
    public class UIButtonListenerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIButtonListenerColorName; } }
        private UIButtonListener m_target;

        private UIButtonListener Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIButtonListener) target;
                return m_target;
            }
        }

        private const string SET_UIBUTTON_TO_LISTEN_FOR = "SetUIButtonToListenFor";
        private const string ANY_UIBUTTON_WILL_TRIGGER_THIS_LISTENER = "AnyUIButtonWillTriggerThisListener";

        private static NamesDatabase Database { get { return UIButtonSettings.Database; } }

        private SerializedProperty m_buttonCategory,
                                   m_buttonName,
                                   m_listenForAllUIButtons,
                                   m_triggerAction,
                                   m_event;

        private AnimBool m_categoryAndNameExpanded,
                         m_eventExpanded;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_buttonCategory = GetProperty(PropertyName.ButtonCategory);
            m_buttonName = GetProperty(PropertyName.ButtonName);
            m_listenForAllUIButtons = GetProperty(PropertyName.ListenForAllUIButtons);
            m_triggerAction = GetProperty(PropertyName.TriggerAction);
            m_event = GetProperty(PropertyName.Event);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_categoryAndNameExpanded = GetAnimBool(m_listenForAllUIButtons.propertyPath, !m_listenForAllUIButtons.boolValue);
            m_eventExpanded = GetAnimBool(m_event.propertyPath, ShowEvent);
        }

        public override bool RequiresConstantRepaint() { return true; }

        private bool ShowEvent
        {
            get
            {
                return m_listenForAllUIButtons.boolValue ||
                       !string.IsNullOrEmpty(m_buttonCategory.stringValue.Trim()) && !string.IsNullOrEmpty(m_buttonName.stringValue.Trim()) &&
                       !m_buttonName.stringValue.Equals(UIButton.DefaultButtonName);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(SET_UIBUTTON_TO_LISTEN_FOR, new InfoMessage(InfoMessage.MessageType.Error, UILabels.SetUIButtonToListenFor, false, Repaint));
            AddInfoMessage(ANY_UIBUTTON_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.AnyUIButtonWillTriggerThisListener, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIButtonListener), MenuUtils.UIButtonListener_Manual, MenuUtils.UIButtonListener_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));

            GUILayout.BeginHorizontal();
            DGUI.Toggle.Switch.Draw(m_listenForAllUIButtons, UILabels.ListenForAllUIButtons, ComponentColorName, true, false);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_triggerAction, UILabels.TriggerAction, ComponentColorName);
            GUILayout.EndHorizontal();

            GetInfoMessage(ANY_UIBUTTON_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_listenForAllUIButtons.boolValue);
            m_categoryAndNameExpanded.target = !m_listenForAllUIButtons.boolValue;

            if (DGUI.FadeOut.Begin(m_categoryAndNameExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space(2) * m_categoryAndNameExpanded.faded);
                DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                        m_buttonCategory, UILabels.ButtonCategory,
                                                        m_buttonName, UILabels.ButtonName,
                                                        Database,
                                                        ComponentColorName);

                GetInfoMessage(SET_UIBUTTON_TO_LISTEN_FOR).DrawMessageOnly(!ShowEvent);
            }

            DGUI.FadeOut.End(m_categoryAndNameExpanded, false);

            GUILayout.Space(DGUI.Properties.Space(2));

            m_eventExpanded.target = ShowEvent;
            DGUI.Property.UnityEventWithFade(m_event, m_eventExpanded, UILabels.Event, ComponentColorName, Target.Event.GetPersistentEventCount());

            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
    }
}