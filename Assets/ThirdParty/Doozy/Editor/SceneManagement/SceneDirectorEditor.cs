// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.SceneManagement
{
    [CustomEditor(typeof(SceneDirector))]
    public class SceneDirectorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.SceneDirectorColorName; } }

        private SceneDirector m_target;

        private SceneDirector Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (SceneDirector) target;
                return m_target;
            }
        }
        
        private int OnActiveSceneChangedEventCount { get { return Target.OnActiveSceneChanged.GetPersistentEventCount(); } }
        private int OnSceneLoadedEventCount { get { return Target.OnSceneLoaded.GetPersistentEventCount(); } }
        private int OnSceneUnloadedEventCount { get { return Target.OnSceneUnloaded.GetPersistentEventCount(); } }
        
        private SerializedProperty
            m_onActiveSceneChanged,
            m_onSceneLoaded,
            m_onSceneUnloaded;
        
        private AnimBool
            m_onActiveSceneChangedExpanded,
            m_onSceneLoadedExpanded,
            m_onSceneUnloadedExpanded;
            
        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_onActiveSceneChanged = GetProperty(PropertyName.OnActiveSceneChanged);
            m_onSceneLoaded = GetProperty(PropertyName.OnSceneLoaded);
            m_onSceneUnloaded = GetProperty(PropertyName.OnSceneUnloaded);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();
            
            m_onActiveSceneChangedExpanded = GetAnimBool(m_onActiveSceneChanged.propertyPath, m_onActiveSceneChanged.isExpanded);
            m_onSceneLoadedExpanded = GetAnimBool(m_onSceneLoaded.propertyPath, m_onSceneLoaded.isExpanded);
            m_onSceneUnloadedExpanded = GetAnimBool(m_onSceneUnloaded.propertyPath, m_onSceneUnloaded.isExpanded);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSceneDirector), MenuUtils.SceneDirector_Manual, MenuUtils.SceneDirector_YouTube);
            DGUI.Doozy.DrawDebugMode(GetProperty(PropertyName.DebugMode), ColorName.Red);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEvents()
        {
            DrawUnityEvent(m_onActiveSceneChangedExpanded, m_onActiveSceneChanged, "OnActiveSceneChanged", OnActiveSceneChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawUnityEvent(m_onSceneLoadedExpanded, m_onSceneLoaded, "OnSceneLoaded", OnSceneLoadedEventCount);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawUnityEvent(m_onSceneUnloadedExpanded, m_onSceneUnloaded, "OnSceneUnloaded", OnSceneUnloadedEventCount);
        }
        
        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }
        
    }

}