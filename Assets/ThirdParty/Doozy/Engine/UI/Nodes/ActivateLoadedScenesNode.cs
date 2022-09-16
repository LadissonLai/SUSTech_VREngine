// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Activates all the Scenes that have been loaded by SceneLoaders and are ready to be activated and then jumps instantly to the next node in the Graph.
    ///     <para />
    ///     It does that by sending the system Game Event 'ActivateLoadedScenes'
    ///     <para />
    ///     <para />
    ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
    ///     <para />
    ///     A scene is ready to be activated if the load progress is at 0.9 (90%). This node activates these scenes (that have been loaded by SceneLoader and that have AllowSceneActivation set to false).
    ///     <para />
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.ActivateLoadedScenesNode_CreateNodeMenu_Name, MenuUtils.ActivateLoadedScenesNode_CreateNodeMenu_Order)]
    public class ActivateLoadedScenesNode : Node
    {
        private const float NODE_WIDTH = 236f;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.ActivateLoadedScenesNodeName);
            SetWidth(NODE_WIDTH);
        }

        public override float GetDefaultNodeWidth() { return NODE_WIDTH; }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            GameEventMessage.SendEvent(SystemGameEvent.ActivateLoadedScenes);
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }
    }
}