// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Utils;
using Doozy.Engine.Soundy;
using Doozy.Editor.Soundy;
using Doozy.Engine.UI.Base;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.SceneManagement
{
    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.SceneLoaderColorName; } }
        private SceneLoader m_target;

        private SceneLoader Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (SceneLoader) target;
                return m_target;
            }
        }

        private SerializedProperty
            m_allowSceneActivation,
            m_sceneName,
            m_sceneBuildIndex,
            m_getSceneBy,
            m_loadSceneMode,
            m_sceneActivationDelay,
            m_loadBehavior,
            m_progressor,
            m_onProgressChanged,
            m_onInverseProgressChanged;

        private AnimBool
            m_loadBehaviorExpanded,
            m_onProgressChangedExpanded,
            m_onInverseProgressChangedExpanded,
            m_soundDataExpanded,
            m_effectExpanded,
            m_animatorEventsExpanded,
            m_gameEventsExpanded,
            m_unityEventsExpanded;

        private readonly Dictionary<UIAction, List<DGUI.IconGroup.Data>> m_behaviorActionsIconsDatabase = new Dictionary<UIAction, List<DGUI.IconGroup.Data>>();
        private readonly Dictionary<SceneLoadBehavior, List<DGUI.IconGroup.Data>> m_loadBehaviorIconsDatabase = new Dictionary<SceneLoadBehavior, List<DGUI.IconGroup.Data>>();

        private int OnProgressChangedEventCount { get { return Target.OnProgressChanged.GetPersistentEventCount(); } }
        private int OnInverseProgressChangedEventCount { get { return Target.OnInverseProgressChanged.GetPersistentEventCount(); } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_allowSceneActivation = GetProperty(PropertyName.AllowSceneActivation);
            m_sceneName = GetProperty(PropertyName.SceneName);
            m_sceneBuildIndex = GetProperty(PropertyName.SceneBuildIndex);
            m_getSceneBy = GetProperty(PropertyName.GetSceneBy);
            m_loadSceneMode = GetProperty(PropertyName.LoadSceneMode);
            m_sceneActivationDelay = GetProperty(PropertyName.SceneActivationDelay);
            m_loadBehavior = GetProperty(PropertyName.LoadBehavior);
            m_progressor = GetProperty(PropertyName.Progressor);
            m_onProgressChanged = GetProperty(PropertyName.OnProgressChanged);
            m_onInverseProgressChanged = GetProperty(PropertyName.OnInverseProgressChanged);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_loadBehaviorExpanded = GetAnimBool(m_loadBehavior.propertyPath, m_loadBehavior.isExpanded);
            m_onProgressChangedExpanded = GetAnimBool(m_onProgressChanged.propertyPath, OnProgressChangedEventCount > 0);
            m_onInverseProgressChangedExpanded = GetAnimBool(m_onInverseProgressChanged.propertyPath, OnInverseProgressChangedEventCount > 0);

            m_soundDataExpanded = GetAnimBool("SOUND");
            m_effectExpanded = GetAnimBool("EFFECT");
            m_animatorEventsExpanded = GetAnimBool("ANIMATOR_EVENTS");
            m_gameEventsExpanded = GetAnimBool("GAME_EVENTS");
            m_unityEventsExpanded = GetAnimBool("UNITY_EVENT");
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnDisable()
        {
            base.OnDisable();
            SoundyAudioPlayer.StopAllPlayers();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderSceneLoader), MenuUtils.SceneLoader_Manual, MenuUtils.SceneLoader_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawLoadOptions(m_loadSceneMode, m_getSceneBy, m_sceneName, m_sceneBuildIndex, ComponentColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawAllowSceneActivationAndDelay(m_allowSceneActivation, m_sceneActivationDelay, ComponentColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawBehavior(UILabels.LoadBehavior, Target.LoadBehavior, m_loadBehavior, m_loadBehaviorExpanded);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawProgressor();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawUnityEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        public static void DrawLoadOptions(SerializedProperty loadSceneMode, SerializedProperty getSceneBy, SerializedProperty sceneName, SerializedProperty sceneBuildIndex, ColorName componentColorName)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(loadSceneMode, UILabels.LoadSceneMode, componentColorName, 70);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(getSceneBy, UILabels.GetSceneBy, componentColorName, 80);
                GUILayout.Space(DGUI.Properties.Space());
                switch ((GetSceneBy) getSceneBy.enumValueIndex)
                {
                    case GetSceneBy.Name:
                        DGUI.Property.Draw(sceneName, UILabels.SceneName, componentColorName, string.IsNullOrEmpty(sceneName.stringValue.Trim()));
                        break;
                    case GetSceneBy.BuildIndex:
                        DGUI.Property.Draw(sceneBuildIndex, UILabels.SceneBuildIndex, componentColorName, sceneBuildIndex.intValue < 0);
                        break;
                }
            }
            GUILayout.EndHorizontal();
        }

        public static void DrawAllowSceneActivationAndDelay(SerializedProperty allowSceneActivation, SerializedProperty sceneActivationDelay, ColorName componentColorName)
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(allowSceneActivation, UILabels.AllowSceneActivation, componentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                bool enabled = GUI.enabled;
                GUI.enabled = allowSceneActivation.boolValue;
                DGUI.Property.Draw(sceneActivationDelay, UILabels.SceneActivationDelay, componentColorName);
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawBehavior(string behaviorName, SceneLoadBehavior behavior, SerializedProperty behaviorProperty, AnimBool behaviorExpanded)
        {
            SerializedProperty loadSceneProperty = GetProperty(PropertyName.OnLoadScene, behaviorProperty);
            SerializedProperty sceneLoadedProperty = GetProperty(PropertyName.OnSceneLoaded, behaviorProperty);

            AnimBool loadSceneExpanded = GetAnimBool(loadSceneProperty.propertyPath, loadSceneProperty.isExpanded);
            AnimBool sceneLoadedExpanded = GetAnimBool(sceneLoadedProperty.propertyPath, sceneLoadedProperty.isExpanded);

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
                        if (DGUI.Doozy.DrawSectionButtonLeft(loadSceneExpanded.target,
                                                             UILabels.OnLoadScene + " (0%)",
                                                             DGUI.Icon.ActionStart,
                                                             ComponentColorName,
                                                             DGUI.Doozy.GetActionsIcons(behavior.OnLoadScene,
                                                                                        m_behaviorActionsIconsDatabase,
                                                                                        ComponentColorName)))
                        {
                            loadSceneExpanded.target = true;
                            sceneLoadedExpanded.value = false;
                            SoundyAudioPlayer.StopAllPlayers();
                        }

                        GUILayout.Space(DGUI.Properties.Space());

                        if (DGUI.Doozy.DrawSectionButtonRight(sceneLoadedExpanded.target,
                                                              UILabels.OnSceneLoaded + " (90%)",
                                                              DGUI.Icon.ActionFinished,
                                                              ComponentColorName,
                                                              DGUI.Doozy.GetActionsIcons(behavior.OnSceneLoaded,
                                                                                         m_behaviorActionsIconsDatabase,
                                                                                         ComponentColorName)))
                        {
                            loadSceneExpanded.value = false;
                            sceneLoadedExpanded.target = true;
                            SoundyAudioPlayer.StopAllPlayers();
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (behaviorExpanded.target && !loadSceneExpanded.target && !sceneLoadedExpanded.target)
                        loadSceneExpanded.value = true;

                    loadSceneProperty.isExpanded = loadSceneExpanded.target;
                    sceneLoadedProperty.isExpanded = sceneLoadedExpanded.target;

                    GUILayout.Space(DGUI.Properties.Space(3) * behaviorExpanded.faded);

                    DrawBehaviorActions(behavior.OnLoadScene, loadSceneProperty, loadSceneExpanded, "OnLoadScene");
                    DrawBehaviorActions(behavior.OnSceneLoaded, sceneLoadedProperty, sceneLoadedExpanded, "OnSceneLoaded");
                }
                GUILayout.EndVertical();
            }

            DGUI.FadeOut.End(behaviorExpanded);

            behaviorProperty.isExpanded = behaviorExpanded.target;
        }

        private void DrawBehaviorIcons(SceneLoadBehavior behavior, AnimBool expanded)
        {
            if (DGUI.AlphaGroup.Begin(Mathf.Clamp(1 - expanded.faded, 0.2f, 1f)))
            {
                GUILayout.BeginHorizontal(GUILayout.Height(NormalBarHeight));
                {
                    GUILayout.FlexibleSpace();

                    if (!m_loadBehaviorIconsDatabase.ContainsKey(behavior)) m_loadBehaviorIconsDatabase.Add(behavior, new List<DGUI.IconGroup.Data>());

                    //GET Actions Icons
                    m_loadBehaviorIconsDatabase[behavior] = DGUI.Doozy.GetBehaviorActionsIcons(m_loadBehaviorIconsDatabase[behavior],
                                                                                               behavior.HasSound,
                                                                                               behavior.HasEffect,
                                                                                               behavior.HasAnimatorEvents,
                                                                                               behavior.HasGameEvents,
                                                                                               behavior.HasUnityEvents,
                                                                                               ComponentColorName);

                    //DRAW Actions Icons
                    if (m_loadBehaviorIconsDatabase[behavior].Count > 0)
                    {
                        GUILayout.Space(DGUI.Properties.Space(4));
                        DGUI.IconGroup.Draw(m_loadBehaviorIconsDatabase[behavior], NormalBarHeight - DGUI.Properties.Space(4), false);
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