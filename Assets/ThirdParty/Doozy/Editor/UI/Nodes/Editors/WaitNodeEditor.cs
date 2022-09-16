// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Editors;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable NotAccessedField.Local

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(WaitNode))]
    public class WaitNodeEditor : BaseNodeEditor
    {
        private const string ERROR_NO_GAME_EVENT = "ErrorNoGameEvent";
        private const string ERROR_NO_SCENE_NAME = "ErrorNoSceneName";
        private const string ERROR_BAD_BUILD_INDEX = "ErrorBadBuildIndex";

        private static NamesDatabase UIViewDatabase { get { return UIViewSettings.Database; } }
        private static NamesDatabase UIButtonDatabase { get { return UIButtonSettings.Database; } }
        private static NamesDatabase UIDrawerDatabase { get { return UIDrawerSettings.Database; } }
        
        private WaitNode TargetNode { get { return (WaitNode) target; } }

        private SerializedProperty
            m_getSceneBy,
            m_waitFor,
            m_anyValue,
//            m_ignoreUnityTimescale,
            m_randomDuration,
            m_duration,
            m_durationMax,
            m_durationMin,
            m_sceneBuildIndex,
            m_gameEvent,
            m_sceneName,
            m_uiViewTriggerAction,
            m_viewCategory,
            m_viewName,
            m_uiButtonTriggerAction,
            m_buttonCategory,
            m_buttonName,
            m_uiDrawerTriggerAction,
            m_drawerName,
            m_customDrawerName;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_getSceneBy = GetProperty(PropertyName.GetSceneBy);
            m_waitFor = GetProperty(PropertyName.WaitFor);
            m_anyValue = GetProperty(PropertyName.AnyValue);
//            m_ignoreUnityTimescale = GetProperty(PropertyName.IgnoreUnityTimescale);
            m_randomDuration = GetProperty(PropertyName.RandomDuration);
            m_duration = GetProperty(PropertyName.Duration);
            m_durationMax = GetProperty(PropertyName.DurationMax);
            m_durationMin = GetProperty(PropertyName.DurationMin);
            m_sceneBuildIndex = GetProperty(PropertyName.SceneBuildIndex);
            m_gameEvent = GetProperty(PropertyName.GameEvent);
            m_sceneName = GetProperty(PropertyName.SceneName);
            m_uiViewTriggerAction = GetProperty(PropertyName.UIViewTriggerAction);
            m_viewCategory = GetProperty(PropertyName.ViewCategory);
            m_viewName = GetProperty(PropertyName.ViewName);
            m_uiButtonTriggerAction = GetProperty(PropertyName.UIButtonTriggerAction);
            m_buttonCategory = GetProperty(PropertyName.ButtonCategory);
            m_buttonName = GetProperty(PropertyName.ButtonName);
            m_uiDrawerTriggerAction = GetProperty(PropertyName.UIDrawerTriggerAction);
            m_drawerName = GetProperty(PropertyName.DrawerName);
            m_customDrawerName = GetProperty(PropertyName.CustomDrawerName);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(ERROR_NO_GAME_EVENT, new InfoMessage(InfoMessage.MessageType.Error, UILabels.MissingGameEventTitle, UILabels.MissingGameEventMessage, TargetNode.ErrorNoGameEvent, Repaint));
            AddInfoMessage(ERROR_NO_SCENE_NAME, new InfoMessage(InfoMessage.MessageType.Error, UILabels.MissingSceneNameTitle, UILabels.MissingSceneNameMessage, TargetNode.ErrorNoSceneName, Repaint));
            AddInfoMessage(ERROR_BAD_BUILD_INDEX, new InfoMessage(InfoMessage.MessageType.Error, UILabels.WrongSceneBuildIndexTitle, UILabels.WrongSceneBuildIndexMessage, TargetNode.ErrorBadBuildIndex, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderWaitNode), MenuUtils.WaitNode_Manual, MenuUtils.WaitNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            EditorGUI.BeginChangeCheck();
            DrawOptions();
            if (EditorGUI.EndChangeCheck())
            {
                if (m_duration.floatValue < 0) m_duration.floatValue = 0;
                if (m_durationMin.floatValue < 0) m_durationMin.floatValue = 0;
                if (m_durationMax.floatValue < 0) m_durationMax.floatValue = 0;
                if (m_durationMax.floatValue < m_durationMin.floatValue) m_durationMax.floatValue = m_durationMin.floatValue;
                if (m_sceneBuildIndex.intValue < 0) m_sceneBuildIndex.intValue = 0;
                NodeUpdated = true;
            }

            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOptions()
        {
            ColorName backgroundColorName = DGUI.Colors.ActionColorName;
            ColorName textColorName = DGUI.Colors.ActionColorName;
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(ERROR_NO_GAME_EVENT).Draw(TargetNode.ErrorNoGameEvent, InspectorWidth);
            GetInfoMessage(ERROR_NO_SCENE_NAME).Draw(TargetNode.ErrorNoSceneName, InspectorWidth);
            GetInfoMessage(ERROR_BAD_BUILD_INDEX).Draw(TargetNode.ErrorBadBuildIndex, InspectorWidth);
            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_waitFor, UILabels.WaitFor, backgroundColorName, textColorName);
                if (TargetNode.WaitFor != WaitNode.WaitType.Time)
                {
                    GUILayout.Space(DGUI.Properties.Space());
                    string anyLabel = string.Empty;
                    switch (TargetNode.WaitFor)
                    {
                        case WaitNode.WaitType.GameEvent:
                            anyLabel = UILabels.AnyGameEvent;
                            break;
                        case WaitNode.WaitType.SceneLoad: 
                        case WaitNode.WaitType.SceneUnload: 
                        case WaitNode.WaitType.ActiveSceneChange:
                            anyLabel = UILabels.AnyScene;
                            break;
                        case WaitNode.WaitType.UIView: 
                            DGUI.Property.Draw(m_uiViewTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                            GUILayout.Space(DGUI.Properties.Space());
                            if ((UIViewBehaviorType) m_uiViewTriggerAction.enumValueIndex == UIViewBehaviorType.Unknown)
                                m_uiViewTriggerAction.enumValueIndex = (int) UIViewBehaviorType.Show;
                            anyLabel = UILabels.AnyUIView;
                            break;
                        case WaitNode.WaitType.UIButton: 
                            DGUI.Property.Draw(m_uiButtonTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                            GUILayout.Space(DGUI.Properties.Space());
                            anyLabel = UILabels.AnyUIButton;
                            break;
                        case WaitNode.WaitType.UIDrawer: 
                            DGUI.Property.Draw(m_uiDrawerTriggerAction, UILabels.TriggerAction, backgroundColorName, textColorName);
                            GUILayout.Space(DGUI.Properties.Space());
                            anyLabel = UILabels.AnyUIDrawer;
                            break;
                    }
                    DGUI.Toggle.Switch.Draw(m_anyValue, anyLabel, textColorName, true, false);
                    if (m_anyValue.boolValue)
                    {
                        backgroundColorName = DGUI.Colors.DisabledBackgroundColorName;
                        textColorName = DGUI.Colors.DisabledTextColorName;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            EditorGUILayout.BeginHorizontal();
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (TargetNode.WaitFor)
                {
                    case WaitNode.WaitType.Time:
                        DGUI.Toggle.Switch.Draw(m_randomDuration, UILabels.RandomDuration, textColorName, true, false);
                        GUILayout.Space(DGUI.Properties.Space());
                        if (TargetNode.RandomDuration)
                        {
                            DGUI.Property.Draw(m_durationMin, UILabels.Min, backgroundColorName, textColorName);
                            GUILayout.Space(DGUI.Properties.Space());
                            DGUI.Property.Draw(m_durationMax, UILabels.Max, backgroundColorName, textColorName);
                        }
                        else
                        {
                            DGUI.Property.Draw(m_duration, UILabels.Duration, backgroundColorName, textColorName);
                        }

                        break;
                    case WaitNode.WaitType.GameEvent:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Property.Draw(m_gameEvent, UILabels.GameEvent, backgroundColorName, textColorName, TargetNode.ErrorNoGameEvent);
                        GUI.enabled = true;
                        break;
                    case WaitNode.WaitType.SceneLoad:
                    case WaitNode.WaitType.SceneUnload:
                    case WaitNode.WaitType.ActiveSceneChange:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Property.Draw(m_getSceneBy, UILabels.GetSceneBy, backgroundColorName, textColorName, DGUI.Properties.DefaultFieldWidth * 2);
                        GUILayout.Space(DGUI.Properties.Space());
                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (TargetNode.GetSceneBy)
                        {
                            case GetSceneBy.Name:
                                DGUI.Property.Draw(m_sceneName, UILabels.SceneName, backgroundColorName, textColorName, TargetNode.ErrorNoSceneName);
                                break;
                            case GetSceneBy.BuildIndex:
                                DGUI.Property.Draw(m_sceneBuildIndex, UILabels.SceneBuildIndex, backgroundColorName, textColorName, TargetNode.ErrorBadBuildIndex);
                                break;
                        }

                        GUI.enabled = true;
                        break;
                    case WaitNode.WaitType.UIView:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                                m_viewCategory, UILabels.ViewCategory,
                                                                m_viewName, UILabels.ViewName,
                                                                UIViewDatabase,
                                                                backgroundColorName);
                        GUI.enabled = true;
                        break;
                    case WaitNode.WaitType.UIButton:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelector(serializedObject,
                                                                m_buttonCategory, UILabels.ButtonCategory,
                                                                m_buttonName, UILabels.ButtonName,
                                                                UIButtonDatabase,
                                                                backgroundColorName);
                        GUI.enabled = true;
                        break;
                    case WaitNode.WaitType.UIDrawer:
                        GUI.enabled = !TargetNode.AnyValue;
                        DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UIDrawer.DefaultDrawerCategory,
                                                                                      m_drawerName, UILabels.DrawerName,
                                                                                      m_customDrawerName,
                                                                                      UIDrawerDatabase,
                                                                                      backgroundColorName);
                        GUI.enabled = true;
                        break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}