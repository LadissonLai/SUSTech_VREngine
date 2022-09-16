// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Nodes;
using UnityEngine;

namespace Doozy.Engine.Nody.Models
{
    /// <summary>
    /// Base class for graphs that contains all logic needed to manage multiple Node and SubGraph references.
    /// </summary>
    [Serializable]
    public class Graph : ScriptableObject
    {
        #region Constants

        public const int FILE_VERSION = 1; //Important: FILE_VERSION must be increased by one when any structure change(s) happen
        public const string NODE_NOT_FOUND = "Node Not Found";

        #endregion

        #region Properties

        /// <summary> Returns the last date and time of day when this Graph was modified </summary>
        public DateTime LastModified
        {
            get
            {
                long utcFileTime = long.Parse(m_lastModified);
                DateTime dateTime = DateTime.FromFileTimeUtc(utcFileTime);
                return dateTime;
            }
        }

        /// <summary> Returns a reference to any SubGraph that is currently active </summary>
        public Graph ActiveSubGraph { get; set; }

        /// <summary> Keeps track of all the global Nodes this graph contains </summary>
        public List<Node> GlobalNodes { get { return m_globalNodes ?? (m_globalNodes = new List<Node>()); } }

        /// <summary> Keeps track of all the Nodes this graph contains </summary>
        public List<Node> Nodes { get { return m_nodes ?? (m_nodes = new List<Node>()); } set { m_nodes = value; } }

        /// <summary> Returns the Node that is currently active </summary>
        public Node ActiveNode { get; set; }

        /// <summary> Returns the Node that was previously active </summary>
        public Node PreviousActiveNode { get; set; }

        /// <summary> Returns TRUE if this Graph contains at least one global Node </summary>
        public bool HasGlobalNodes { get { return GlobalNodes.Count > 0; } }

        /// <summary> [Editor Only] Keeps track if this Graph is dirty or not (needs to be saved) </summary>
        public bool IsDirty { get { return m_isDirty; } set { m_isDirty = value; } }

        /// <summary> Returns TRUE if this Graph is a SubGraph </summary>
        public bool IsSubGraph { get { return m_isSubGraph; } set { m_isSubGraph = value; } }

        /// <summary> Returns this graph's description </summary>
        public string Description { get { return m_description; } }

        /// <summary> Returns this graph's id </summary>
        public string Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Returns the class version of this graph </summary>
        public int Version { get { return m_version; } }

        /// <summary> Toggle Enable/Disable state for the Graph. Controlled by the GraphController </summary>
        public bool Enabled
        {
            get { return m_enabled; }
            set
            {
                m_enabled = value;
                if (ActiveSubGraph != null) ActiveSubGraph.Enabled = m_enabled;
            }
        }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Reference to the Graph that contains this Graph (if this is a SubGraph) </summary>
        [NonSerialized] public Graph ParentGraph;

        /// <summary> Reference to the SubGraphNode that points this Graph (if this is a SubGraph) </summary>
        [NonSerialized] public SubGraphNode ParentSubGraphNode;

        #endregion

        #region Private Variable

        [NonSerialized] private List<Node> m_activatedNodesHistory = new List<Node>();
        [NonSerialized] private List<Node> m_globalNodes;
        [NonSerialized] private Node m_enterNode;
        [NonSerialized] private Node m_exitNode;
        [NonSerialized] private Node m_startNode;
        [NonSerialized] private bool m_isDirty;
        [NonSerialized] private double m_infiniteLoopTimerStart;
        [NonSerialized] private float m_infiniteLoopTimeBreak = 0.1f;
        [NonSerialized] private bool m_enabled;

        [SerializeField] private List<Node> m_nodes;
        [SerializeField] private bool m_isSubGraph;
        [SerializeField] private int m_version;
#pragma warning disable 0649
        [SerializeField] private string m_description;
#pragma warning restore 0649
        [SerializeField] private string m_id;
        [SerializeField] private string m_lastModified;

        #endregion

        #region Public Methods

        /// <summary> Activates all the global nodes </summary>
        public virtual void ActivateGlobalNodes()
        {
            GlobalNodes.Clear();
            foreach (Node node in Nodes)
            {
                if (node.NodeType != NodeType.Global) continue;
                GlobalNodes.Add(node);
                node.Activate(this);
            }
        }

        /// <summary> Deactivates all the global nodes </summary>
        public virtual void DeactivateGlobalNodes()
        {
            if (!HasGlobalNodes) return;
            foreach (Node node in GlobalNodes)
                node.Deactivate();

            if (ActiveSubGraph != null) ActiveSubGraph.DeactivateGlobalNodes();
        }

        /// <summary> FixedUpdate is called every fixed framerate frame, if this graph has been loaded by a controller </summary>
        public virtual void FixedUpdate()
        {
            if (ActiveNode != null && ActiveNode.UseFixedUpdate) ActiveNode.OnFixedUpdate();
            if (ActiveSubGraph != null) ActiveSubGraph.FixedUpdate();
            foreach (Node node in GlobalNodes)
                if (node.UseFixedUpdate)
                    node.OnFixedUpdate();
        }

        /// <summary> LateUpdate is called every frame, after all Update functions have been called and if this graph has been loaded by a controller </summary>
        public virtual void LateUpdate()
        {
            if (ActiveNode != null && ActiveNode.UseLateUpdate) ActiveNode.OnLateUpdate();
            if (ActiveSubGraph != null) ActiveSubGraph.LateUpdate();
            foreach (Node node in GlobalNodes)
                if (node.UseLateUpdate)
                    node.OnLateUpdate();
        }

        /// <summary> Update is called every frame, if this graph has been loaded by a controller </summary>
        public virtual void Update()
        {
            if (ActiveNode != null && ActiveNode.UseUpdate) ActiveNode.OnUpdate();
            if (ActiveSubGraph != null) ActiveSubGraph.Update();
            foreach (Node node in GlobalNodes)
                if (node.UseUpdate)
                    node.OnUpdate();
        }

        /// <summary> Activates this graph's first node (either the StartNode or the EnterNode) </summary>
        public void ActivateStartOrEnterNode()
        {
            PreviousActiveNode = null;
            ActiveNode = GetStartOrEnterNode();
            ActiveNode.SetActiveGraph(this);
            ActiveNode.OnEnter(null, null);
//            ActivateGlobalNodes();
        }

        /// <summary> Returns TRUE if the passed Node is found in the graph </summary>
        /// <param name="node"> Target Node to search for </param>
        public bool ContainsNode(Node node) { return Nodes != null && Nodes.Contains(node); }

        /// <summary> Returns TRUE if a Node, with the passed node id, is found in the graph </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public bool ContainsNodeById(string nodeId) { return GetNodeById(nodeId) != null; }

        /// <summary> Returns TRUE if a Node, with the given node name, is found in the graph </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public bool ContainsNodeByName(string nodeName) { return GetNodeByName(nodeName) != null; }

        /// <summary> Returns TRUE if a Socket, with the passed socket id, is found in the graph </summary>
        /// <param name="socketId"> Socket id to search for </param>
        public bool ContainsSocket(string socketId) { return GetSocket(socketId) != null; }

        /// <summary> Returns the EnterNode if this is a SubGraph. Returns null if the Node is not found or if this is a Graph </summary>
        public Node GetEnterNode()
        {
            foreach (Node node in Nodes)
                if (node.NodeType == NodeType.Enter)
                    return node as EnterNode;
            return null;
        }

        /// <summary> Returns the ExitNode if this is a SubGraph. Returns null if the Node is not found or if this is a Graph </summary>
        public Node GetExitNode()
        {
            foreach (Node node in Nodes)
                if (node.NodeType == NodeType.Exit)
                    return node as ExitNode;
            return null;
        }

        /// <summary> Returns the Node, with the passed node id, found in the Graph. Returns null if the Node is not found </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public Node GetNodeById(string nodeId)
        {
            foreach (Node node in Nodes)
                if (node.Id.Equals(nodeId))
                    return node;

            return null;
        }

        /// <summary> Returns the first Node, with the passed node name, found in the Graph. Returns null if the Node is not found </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public Node GetNodeByName(string nodeName)
        {
            foreach (Node node in Nodes)
                if (node.Name.Equals(nodeName))
                    return node;

            return null;
        }

        /// <summary> Returns the node id of the first Node, with the passed node name, found in the Graph. Returns 'Node Not Found' if the Node is not found </summary>
        /// <param name="nodeName"> Node name to search for </param>
        public string GetNodeIdFromNodeName(string nodeName)
        {
            Node node = GetNodeByName(nodeName);
            return node == null ? NODE_NOT_FOUND : node.Name;
        }

        /// <summary> Returns the node name of the Node, with the passed node id, found in the Graph. Returns 'Node Not Found' if the Node is not found </summary>
        /// <param name="nodeId"> Node id to search for </param>
        public string GetNodeNameFromNodeId(string nodeId)
        {
            Node node = GetNodeById(nodeId);
            return node == null ? NODE_NOT_FOUND : node.Name;
        }

        /// <summary> Returns the StartNode if this is a Graph. Returns null if the Node is not found or if this is a SubGraph </summary>
        public Node GetStartNode()
        {
            foreach (Node node in Nodes)
                if (node.NodeType == NodeType.Start)
                    return node as StartNode;
            return null;
        }

        /// <summary> Returns the graph's first node (either the StartNode or the EnterNode) </summary>
        public Node GetStartOrEnterNode() { return m_isSubGraph ? GetEnterNode() : GetStartNode(); }

        /// <summary> Returns the Socket, with the passed socket id, from the graph (if it exists) </summary>
        /// <param name="socketId"> Socket id to search for </param>
        public Socket GetSocket(string socketId)
        {
            foreach (Node node in Nodes)
            {
                Socket socket = node.GetSocketFromId(socketId);
                if (socket != null) return socket;
            }

            return null;
        }

        /// <summary> Activate a Node </summary>
        /// <param name="nextActiveNode"> Target Node </param>
        /// <param name="connection"> Connection to ping (used as a visual cue in the editor) </param>
        public void SetActiveNode(Node nextActiveNode, Connection connection = null)
        {
            if (ActiveNode != null)
            {
                ActiveNode.OnExit(nextActiveNode, connection);
                ActiveNode.SetActiveGraph(null);
            }

            PreviousActiveNode = ActiveNode;

            if (InfiniteLoopDetected(nextActiveNode))
                throw new OverflowException("Nody detected an Infinite Loop." +
                                            "\n" +
                                            "This loop was detected in the '" + name + "' " + (m_isSubGraph ? "SubGraph" : "Graph") + ", at the '" + nextActiveNode.Name + "' Node. " +
                                            (PreviousActiveNode == null ? "" : "The connection that triggered this exception was between '" + PreviousActiveNode.Name + "' --and--> '" + nextActiveNode.Name + "' Nodes. ") +
                                            "This issue happened because the sockets are connected in such a manner that a closed infinite loop was created. " +
                                            "Please redesign the current UI flow to and from the '" + nextActiveNode.Name + "' Node, because Nody cannot automatically do that for you. " +
                                            "Look at how the '" + nextActiveNode.Name + "' Node is connected to other nodes and try to follow the UI flow in order to understand where the infinite loop happens. " +
                                            "If this exception were not in place, the Unity Editor would freeze, then you would have had to close it by force and lose any unsaved changes in the process." +
                                            "\n");

            ActiveNode = nextActiveNode;
            if (ActiveNode == null) return;

            if (ActiveNode.NodeType == NodeType.Exit) DeactivateGlobalNodes(); //if the ActiveNode is an Exit Node -> then this means that this is a SubGraph that just finished it's cycle -> stop all of its GlobalNodes

            ActiveNode.SetActiveGraph(this);
            ActiveNode.OnEnter(PreviousActiveNode, connection);
        }

        /// <summary> Activate the input Node of the passed Connection </summary>
        /// <param name="connection"> Target Connection </param>
        public void SetActiveNodeByConnection(Connection connection) { SetActiveNode(GetNodeById(connection.InputNodeId), connection); }

        /// <summary> Activate a Node by node id </summary>
        /// <param name="nodeId"> Node id to search for </param>
        /// <param name="connection"> Connection to ping (used as a visual cue in the editor) </param>
        public void SetActiveNodeById(string nodeId, Connection connection = null) { SetActiveNode(GetNodeById(nodeId), connection); }

        /// <summary> Activate a Node by node name </summary>
        /// <param name="nodeName"> Node name to search for </param>
        /// <param name="connection"> Connection to ping (used as a visual cue in the editor) </param>
        public void SetActiveNodeByName(string nodeName, Connection connection = null) { SetActiveNode(GetNodeByName(nodeName), connection); }

        /// <summary> Updates the last modified date and time for this graph </summary>
        /// <param name="time"> New time </param>
        public void SetLastModified(string time) { m_lastModified = time; }

        /// <summary> Updates the class version of this graph </summary>
        /// <param name="version"> New version </param>
        public void SetVersion(int version) { m_version = version; }

        #endregion

        #region Private Methods

        private bool InfiniteLoopDetected(Node nextActiveNode)
        {
            if (nextActiveNode == null) return false;

            if (Time.realtimeSinceStartup - m_infiniteLoopTimerStart < m_infiniteLoopTimeBreak)
            {
                if (m_activatedNodesHistory.Contains(nextActiveNode)) return true;
                m_activatedNodesHistory.Add(nextActiveNode);
            }
            else
            {
                m_infiniteLoopTimerStart = Time.realtimeSinceStartup;
                m_activatedNodesHistory.Clear();
            }

            return false;
        }

        #endregion
    }
}