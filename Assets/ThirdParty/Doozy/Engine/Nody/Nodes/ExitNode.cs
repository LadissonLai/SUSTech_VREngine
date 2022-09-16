// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.Utils;

namespace Doozy.Engine.Nody.Nodes
{
    /// <summary>
    ///     Is the last node in a SubGraph to be activated.
    ///     It activates instantly the next node in the parent Graph.
    ///     <para />
    ///     The next node in the parent Graph is the one connected to the SubGraphNode's output socket, that referenced the parent SubGraph of this ExitNode.
    /// </summary>
    [NodeMenu(MenuUtils.HiddenNode, MenuUtils.BaseNodeOrder)]
    public class ExitNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNodeIsNotConnected; } }
        public bool ErrorNodeIsNotConnected;
#endif
        
        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = false;
            SetNodeType(NodeType.Exit);
            SetName(UILabels.ExitNodeName);
            SetWidth(NodySettings.Instance.ExitNodeWidth);
        }

        public override float GetDefaultNodeWidth() { return NodySettings.Instance.ExitNodeWidth; }
        
        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(UIConnection), false, false);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNodeIsNotConnected = !InputSockets[0].IsConnected;
#endif
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            if (ActiveGraph.ParentGraph == null) return;
            if (ActiveGraph.ParentSubGraphNode == null) return;
            ActiveGraph.DeactivateGlobalNodes();
            ActiveGraph.ParentSubGraphNode.ExitSubGraphNode();
        }
    }
}