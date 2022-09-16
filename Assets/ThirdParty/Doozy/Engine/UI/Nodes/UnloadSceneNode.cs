// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Utils;
using UnityEngine.SceneManagement;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Tells the SceneDirector to unload a Scene either by scene name or scene build index.
    ///     <para />
    ///     This will destroy all GameObjects associated with the given Scene and remove the Scene from the SceneManager.
    ///     <para />
    ///     <para />
    ///     Besides unloading a Scene, the Unload Scene Node can wait until the target Scene has been unloaded before activating the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.UnloadSceneNode_CreateNodeMenu_Name, MenuUtils.UnloadSceneNode_CreateNodeMenu_Order)]
    public class UnloadSceneNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoSceneName || ErrorBadBuildIndex; } }
        public bool ErrorNoSceneName, ErrorBadBuildIndex;
#endif

        public GetSceneBy GetSceneBy = SceneLoader.DEFAULT_GET_SCENE_BY;
        public int SceneBuildIndex = SceneLoader.DEFAULT_BUILD_INDEX;
        public string SceneName = SceneLoader.DEFAULT_SCENE_NAME;
        public bool WaitForSceneToUnload;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.UnloadSceneNodeName);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var node = (UnloadSceneNode) original;
            GetSceneBy = node.GetSceneBy;
            SceneBuildIndex = node.SceneBuildIndex;
            SceneName = node.SceneName;
            WaitForSceneToUnload = node.WaitForSceneToUnload;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            if (WaitForSceneToUnload) SceneDirector.Instance.OnSceneUnloaded.AddListener(SceneUnloaded);
            UnloadScene();
            if (WaitForSceneToUnload) return;
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void OnExit(Node nextActiveNode, Connection connection)
        {
            base.OnExit(nextActiveNode, connection);

            if (WaitForSceneToUnload) SceneDirector.Instance.OnSceneUnloaded.RemoveListener(SceneUnloaded);
        }

        private void UnloadScene()
        {
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    SceneDirector.UnloadSceneAsync(SceneName);
                    break;
                case GetSceneBy.BuildIndex:
                    SceneDirector.UnloadSceneAsync(SceneBuildIndex);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void SceneUnloaded(Scene unloadedScene)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    if (!unloadedScene.name.Equals(SceneName))
                        return;
                    break;
                case GetSceneBy.BuildIndex:
                    if (!unloadedScene.name.Equals(SceneManager.GetSceneByBuildIndex(SceneBuildIndex).name))
                        return;
                    break;
            }

            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNoSceneName = GetSceneBy == GetSceneBy.Name && string.IsNullOrEmpty(SceneName.Trim());
            ErrorBadBuildIndex = GetSceneBy == GetSceneBy.BuildIndex && (SceneBuildIndex < 0 || SceneBuildIndex + 1 > SceneManager.sceneCountInBuildSettings);
#endif
        }
    }
}