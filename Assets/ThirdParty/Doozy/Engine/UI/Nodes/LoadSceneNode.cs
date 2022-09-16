// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Utils;
using UnityEngine.SceneManagement;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Loads any Scene either by scene name or scene build index via a SceneLoader and jumps instantly to the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.LoadSceneNode_CreateNodeMenu_Name, MenuUtils.LoadSceneNode_CreateNodeMenu_Order)]
    public class LoadSceneNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoSceneName || ErrorBadBuildIndex; } }
        public bool ErrorNoSceneName, ErrorBadBuildIndex;
#endif

        public GetSceneBy GetSceneBy = SceneLoader.DEFAULT_GET_SCENE_BY;
        public LoadSceneMode LoadSceneMode = SceneLoader.DEFAULT_LOAD_SCENE_MODE;
        public bool AllowSceneActivation = SceneLoader.DEFAULT_AUTO_SCENE_ACTIVATION;
        public float SceneActivationDelay = SceneLoader.DEFAULT_SCENE_ACTIVATION_DELAY;
        public int SceneBuildIndex = SceneLoader.DEFAULT_BUILD_INDEX;
        public string SceneName = SceneLoader.DEFAULT_SCENE_NAME;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.LoadSceneNodeName);
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
            var node = (LoadSceneNode) original;
            AllowSceneActivation = node.AllowSceneActivation;
            GetSceneBy = node.GetSceneBy;
            LoadSceneMode = node.LoadSceneMode;
            SceneActivationDelay = node.SceneActivationDelay;
            SceneBuildIndex = node.SceneBuildIndex;
            SceneName = node.SceneName;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            LoadScene();
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        private void LoadScene()
        {
            SceneLoader.GetLoader()
                       .SetLoadSceneMode(LoadSceneMode)
                       .SetLoadSceneBy(GetSceneBy)
                       .SetSceneName(SceneName)
                       .SetSceneBuildIndex(SceneBuildIndex)
                       .SetAllowSceneActivation(AllowSceneActivation)
                       .SetSceneActivationDelay(SceneActivationDelay)
                       .SetSelfDestructAfterSceneLoaded(true)
                       .LoadSceneAsync();
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