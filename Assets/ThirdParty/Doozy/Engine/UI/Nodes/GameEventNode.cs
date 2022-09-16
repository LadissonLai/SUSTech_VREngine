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
    ///     Sends a Game Event (string value) and jumps instantly to the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.GameEventNode_CreateNodeMenu_Name, MenuUtils.GameEventNode_CreateNodeMenu_Order)]
    public class GameEventNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNotSendingAnyGameEvent; } }
        public bool ErrorNotSendingAnyGameEvent;
#endif

        public string GameEvent;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.SendGameEvents);
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
            var node = (GameEventNode) original;
            GameEvent = node.GameEvent;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            SendGameEvent();
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNotSendingAnyGameEvent = string.IsNullOrEmpty(GameEvent);
#endif
        }

        private void SendGameEvent()
        {
            if (string.IsNullOrEmpty(GameEvent)) return;
            GameEventMessage.SendEvent(GameEvent, null);
        }
    }
}