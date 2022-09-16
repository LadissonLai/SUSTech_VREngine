// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.Nody.Nodes
{
    /// <summary>
    ///     Activates the referenced SubGraph inside the parent Graph. It allows for endless nesting and helps managing complex UI flow structures.
    ///     <para />
    ///     This node is particularly important as it allows for Graphs to be split in multiple SubGraphs and be easier to manage.
    ///     <para />
    ///     Also a SubGraph can be split into two or more SubGraphs allowing for the creation of very deep and complex UI flows.
    /// </summary>
    [NodeMenu(MenuUtils.SubGraphNode_CreateNodeMenu_Name, MenuUtils.SubGraphNode_CreateNodeMenu_Order, true)]
    public class SubGraphNode : Node
    {
        [SerializeField] private Graph m_subGraph;

        public Graph SubGraph { get { return m_subGraph; } }

#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoGraphReferenced || ErrorReferencedGraphIsNotSubGraph; } }
#endif
        public bool ErrorNoGraphReferenced;
        public bool ErrorReferencedGraphIsNotSubGraph;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.SubGraph);
            SetName(UILabels.SubGraphNodeName);
            SetWidth(NodySettings.Instance.SubGraphNodeWidth);
        }

        public override float GetDefaultNodeWidth() { return NodySettings.Instance.SubGraphNodeWidth; }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
            ErrorNoGraphReferenced = m_subGraph == null;
            ErrorReferencedGraphIsNotSubGraph = m_subGraph == null || !m_subGraph.IsSubGraph;
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var subGraphNode = (SubGraphNode) original;
            m_subGraph = subGraphNode.SubGraph;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            if (ErrorNoGraphReferenced) return;
            if (ErrorReferencedGraphIsNotSubGraph) return;
            var enterNode = m_subGraph.GetEnterNode() as EnterNode;
            if (enterNode == null) return;
            ActiveGraph.DeactivateGlobalNodes();
            ActiveGraph.ActiveSubGraph = m_subGraph;
            ActiveGraph.ActiveSubGraph.Enabled = true;
            m_subGraph.ParentGraph = ActiveGraph;
            m_subGraph.ParentSubGraphNode = this;
            m_subGraph.SetActiveNode(enterNode, null);
        }

        public void ExitSubGraphNode()
        {
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.ActiveSubGraph.Enabled = false;
            ActiveGraph.ActiveSubGraph = null;
            ActiveGraph.ActivateGlobalNodes();
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }
    }
}