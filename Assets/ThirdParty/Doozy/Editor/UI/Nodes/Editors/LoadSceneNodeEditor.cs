// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Editors;
using Doozy.Editor.SceneManagement;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(LoadSceneNode))]
    public class LoadSceneNodeEditor : BaseNodeEditor
    {
        private const string ERROR_NO_SCENE_NAME = "ErrorNoSceneName";
        private const string ERROR_BAD_BUILD_INDEX = "ErrorBadBuildIndex";

        private LoadSceneNode TargetNode { get { return (LoadSceneNode) target; } }

        private SerializedProperty
            m_allowSceneActivation,
            m_sceneName,
            m_sceneBuildIndex,
            m_getSceneBy,
            m_loadSceneMode,
            m_sceneActivationDelay;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_allowSceneActivation = GetProperty(PropertyName.AllowSceneActivation);
            m_sceneName = GetProperty(PropertyName.SceneName);
            m_sceneBuildIndex = GetProperty(PropertyName.SceneBuildIndex);
            m_getSceneBy = GetProperty(PropertyName.GetSceneBy);
            m_loadSceneMode = GetProperty(PropertyName.LoadSceneMode);
            m_sceneActivationDelay = GetProperty(PropertyName.SceneActivationDelay);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(ERROR_NO_SCENE_NAME, new InfoMessage(InfoMessage.MessageType.Error, UILabels.MissingSceneNameTitle, UILabels.MissingSceneNameMessage, TargetNode.ErrorNoSceneName, Repaint));
            AddInfoMessage(ERROR_BAD_BUILD_INDEX, new InfoMessage(InfoMessage.MessageType.Error, UILabels.WrongSceneBuildIndexTitle, UILabels.WrongSceneBuildIndexMessage, TargetNode.ErrorBadBuildIndex, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderLoadSceneNode), MenuUtils.LoadSceneNode_Manual, MenuUtils.LoadSceneNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space());
            DrawRenameNodeButton();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            DrawOptions();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawRenameNodeButton()
        {
            string renameTo = "---";

            switch (TargetNode.GetSceneBy)
            {
                case GetSceneBy.Name:
                    GUI.enabled = !string.IsNullOrEmpty(TargetNode.SceneName);
                    renameTo = TargetNode.SceneName;
                    break;
                case GetSceneBy.BuildIndex:
                    renameTo = "Load Index: " + TargetNode.SceneBuildIndex;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            DrawRenameButton(renameTo);
            GUI.enabled = true;
        }

        private void DrawOptions()
        {
            EditorGUI.BeginChangeCheck();
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(ERROR_NO_SCENE_NAME).Draw(TargetNode.ErrorNoSceneName, InspectorWidth);
            GetInfoMessage(ERROR_BAD_BUILD_INDEX).Draw(TargetNode.ErrorBadBuildIndex, InspectorWidth);
            SceneLoaderEditor.DrawLoadOptions(m_loadSceneMode, m_getSceneBy, m_sceneName, m_sceneBuildIndex, DGUI.Colors.ActionColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            SceneLoaderEditor.DrawAllowSceneActivationAndDelay(m_allowSceneActivation, m_sceneActivationDelay, DGUI.Colors.ActionColorName);
            if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
        }
    }
}