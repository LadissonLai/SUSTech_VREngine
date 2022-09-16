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
    [CustomEditor(typeof(UIViewListener))]
    [CanEditMultipleObjects]
    public class UIViewListenerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIViewListenerColorName; } }
        private UIViewListener m_target;

        private UIViewListener Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (UIViewListener) target;
                return m_target;
            }
        }

        private const string SET_UIVIEW_TO_LISTEN_FOR = "SetUIViewToListenFor";
        private const string ANY_UIVIEW_WILL_TRIGGER_THIS_LISTENER = "AnyUIViewWillTriggerThisListener";

        private static NamesDatabase Database { get { return UIViewSettings.Database; } }

        private SerializedProperty m_viewCategory,
                                   m_viewName,
                                   m_listenForAllUIViews,
                                   m_triggerAction,
                                   m_event;

        private AnimBool m_categoryAndNameExpanded,
                         m_eventExpanded;

        private bool ShowEvent
        {
            get
            {
                return m_listenForAllUIViews.boolValue ||
                       !string.IsNullOrEmpty(m_viewCategory.stringValue.Trim()) && !string.IsNullOrEmpty(m_viewName.stringValue.Trim()) &&
                       !m_viewName.stringValue.Equals(UIView.DefaultViewName);
            }
        }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();
            
            m_viewCategory = GetProperty(PropertyName.ViewCategory);
            m_viewName = GetProperty(PropertyName.ViewName);
            m_listenForAllUIViews = GetProperty(PropertyName.ListenForAllUIViews);
            m_triggerAction = GetProperty(PropertyName.TriggerAction);
            m_event = GetProperty(PropertyName.Event);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();
            
            m_categoryAndNameExpanded = GetAnimBool(m_listenForAllUIViews.propertyPath, !m_listenForAllUIViews.boolValue);
            m_eventExpanded = GetAnimBool(m_event.propertyPath, ShowEvent);
        }

        public override bool RequiresConstantRepaint() { return true; }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(SET_UIVIEW_TO_LISTEN_FOR, new InfoMessage(InfoMessage.MessageType.Error, UILabels.SetUIViewToListenFor, false, Repaint));
            AddInfoMessage(ANY_UIVIEW_WILL_TRIGGER_THIS_LISTENER, new InfoMessage(InfoMessage.MessageType.Info, UILabels.AnyUIViewWillTriggerThisListener, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIViewListener), MenuUtils.UIViewListener_Manual, MenuUtils.UIViewListener_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));

            GUILayout.BeginHorizontal();
            DGUI.Toggle.Switch.Draw(m_listenForAllUIViews, UILabels.ListenForAllUIViews, ComponentColorName, true, false);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_triggerAction, UILabels.TriggerAction, ComponentColorName);
            GUILayout.EndHorizontal();

            GetInfoMessage(ANY_UIVIEW_WILL_TRIGGER_THIS_LISTENER).DrawMessageOnly(m_listenForAllUIViews.boolValue);
            m_categoryAndNameExpanded.target = !m_listenForAllUIViews.boolValue;

            if (DGUI.FadeOut.Begin(m_categoryAndNameExpanded, false))
            {
                GUILayout.Space(DGUI.Properties.Space(2) * m_categoryAndNameExpanded.faded);
                DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                        m_viewCategory, UILabels.ViewCategory,
                                                        m_viewName, UILabels.ViewName,
                                                        Database,
                                                        ComponentColorName);

                GetInfoMessage(SET_UIVIEW_TO_LISTEN_FOR).DrawMessageOnly(!ShowEvent);
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