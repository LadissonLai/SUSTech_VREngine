// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_termsa

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.Utils;

namespace Doozy.Engine.Nody.Nodes
{
    /// <summary>
    ///     Is the fist node in a SubGraph to be activated.
    ///     It activates instantly the next node in the SubGraph.
    ///     <para />
    ///     The next node in the SubGraph is the one connected to this node's output socket.
    /// </summary>
    [NodeMenu(MenuUtils.HiddenNode, MenuUtils.BaseNodeOrder)]
    public class EnterNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNodeIsNotConnected; } }
        public bool ErrorNodeIsNotConnected;
#endif

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = false;
            SetNodeType(NodeType.Enter);
            SetName(UILabels.EnterNodeName);
            SetWidth(NodySettings.Instance.EnterNodeWidth);
        }

        public override float GetDefaultNodeWidth() { return NodySettings.Instance.EnterNodeWidth; }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddOutputSocket(ConnectionMode.Override, typeof(UIConnection), false, false);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNodeIsNotConnected = !FirstOutputSocket.IsConnected;
#endif
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            ActiveGraph.ActivateGlobalNodes();
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }
    }
}