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
    [CustomEditor(typeof(UIDrawerListener))]
    [CanEditMultipleObjects]
    public class UIDrawerListenerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIDrawerListenerColorName; } }
        private UIDrawerListener m_target;

        private UIDrawerListener Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIDrawerListener) target;
                return m_target;
            }
        }

        private const string SET_UIDRAWER_TO_LISTEN_FOR = "SetUIDrawerToListenFor";
        private const string ANY_UIDRAWER_WILL_TRIGGER_THIS_LISTENER = "AnyUIDrawerWillTriggerThisListener";

        private static NamesDatabase Database { get { return UIDrawerSettings.Database; } }

        private SerializedProperty
            m_drawerName,
            m_customDrawerName,
            m_listenForAllUIDrawers,
            m_triggerAction,
            m_event;

        private AnimBool
            m_showNameExpanded,
            m_showEventExpanded;


        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_drawerName = GetProperty(PropertyName.DrawerName);
            m_customDrawerName = GetProperty(PropertyName.CustomDrawerName);
            m_listenForAllUIDrawers = GetProperty(PropertyName.ListenForAllUIDrawers);
            m_triggerAction = GetProperty(PropertyName.TriggerAction);
            m_event = GetProperty(PropertyName.Event);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_showNameExpanded = GetAnimBool(m_listenForAllUIDrawers.propertyPath, !m_listenForAllUIDrawers.boolValue);
            m_showEventExpanded = GetAnimBool(m_event.propertyPath, ShowEvent);
        }

        private bool ShowEvent
        {
            get
            {
                return m_listenForAllUIDrawers.boolValue ||
                       !string.IsNullOrEmpty(m_drawerName.stringValue.Trim());
            }
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(SET_UIDRAWER_TO_LISTEN_FOR, new InfoMessage(InfoMessage.MessageType.Error, UILabels.SetUIDrawerToListenFor, false, Repaint));
            AddInfoMessage(ANY_UIDRAWER_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.AnyUIDrawerWillTriggerThisListener, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIDrawerListener), MenuUtils.UIDrawerListener_Manual, MenuUtils.UIDrawerListener_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));

            GUILayout.BeginHorizontal();
            DGUI.Toggle.Switch.Draw(m_listenForAllUIDrawers, UILabels.ListenForAllUIDrawers, ComponentColorName, true, false);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_triggerAction, UILabels.TriggerAction, ComponentColorName);
            GUILayout.EndHorizontal();

            GetInfoMessage(ANY_UIDRAWER_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_listenForAllUIDrawers.boolValue);
            m_showNameExpanded.target = !m_listenForAllUIDrawers.boolValue;

            if (DGUI.FadeOut.Begin(m_showNameExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space(2) * m_showNameExpanded.faded);

                DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UIDrawer.DefaultDrawerCategory,
                                                                              m_drawerName, UILabels.DrawerName,
                                                                              m_customDrawerName,
                                                                              Database,
                                                                              ComponentColorName);

                GetInfoMessage(SET_UIDRAWER_TO_LISTEN_FOR).DrawMessageOnly(!ShowEvent);
            }

            DGUI.FadeOut.End(m_showNameExpanded, false);

            GUILayout.Space(DGUI.Properties.Space(2));

            m_showEventExpanded.target = ShowEvent;
            DGUI.Property.UnityEventWithFade(m_event, m_showEventExpanded, UILabels.Event, ComponentColorName, Target.Event.GetPersistentEventCount());

            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
    }
}