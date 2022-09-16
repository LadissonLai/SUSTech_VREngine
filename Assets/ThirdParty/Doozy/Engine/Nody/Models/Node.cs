// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.AnimatedValues;

#endif

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

namespace Doozy.Engine.Nody.Models
{
    /// <summary>
    ///     Base class for all nodes.
    /// </summary>
    [Serializable]
    [NodeMenu(MenuUtils.HiddenNode, MenuUtils.BaseNodeOrder)]
    public class Node : ScriptableObject
    {
        #region UNITY EDITOR ONLY

#if UNITY_EDITOR
        private AnimBool m_showHover;
        public AnimBool IsHovered { get { return m_showHover ?? (m_showHover = new AnimBool(false) {speed = 2f}); } }

        public virtual bool HasErrors { get { return ErrorNodeNameIsEmpty || ErrorDuplicateNameFoundInGraph; } }

        public bool ErrorNodeNameIsEmpty;
        public bool ErrorDuplicateNameFoundInGraph;
#endif

        #endregion

        #region Private Variables

        /**
        * In order to support Unity serialization for Undo, cyclic reference need to be avoided
        * For that reason, we are storing a graph id instead of pointer to the parent graph
        */

        [SerializeField] private List<Socket> m_inputSockets;
        [SerializeField] private List<Socket> m_outputSockets;
        [SerializeField] private NodeType m_nodeType;
        [SerializeField] private bool m_allowDuplicateNodeName;
        [SerializeField] private bool m_allowEmptyNodeName;
        [SerializeField] private bool m_canBeDeleted;
        [SerializeField] private bool m_debugMode;
        [SerializeField] private bool m_useFixedUpdate;
        [SerializeField] private bool m_useLateUpdate;
        [SerializeField] private bool m_useUpdate;
        [SerializeField] private float m_height;
        [SerializeField] private float m_width;
        [SerializeField] private float m_x;
        [SerializeField] private float m_y;
        [SerializeField] private int m_minimumInputSocketsCount;
        [SerializeField] private int m_minimumOutputSocketsCount;
        [SerializeField] private string m_graphId;
        [SerializeField] private string m_id;
        [SerializeField] private string m_name;
        [SerializeField] private string m_notes;

        [NonSerialized] private Graph m_activeGraph;
        [NonSerialized] protected bool m_activated;

        #endregion

        #region Properties

        /// <summary> Direct reference to the active language pack </summary>
        protected static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        /// <summary> Returns TRUE if this node can have the same name as another node </summary>
        public bool AllowDuplicateNodeName { get { return m_allowDuplicateNodeName; } }

        /// <summary> Returns TRUE if this node can have an empty node name </summary>
        public bool AllowEmptyNodeName { get { return m_allowEmptyNodeName; } }

        /// <summary> Returns TRUE if this can be deleted </summary>
        public bool CanBeDeleted { get { return m_canBeDeleted; } set { m_canBeDeleted = value; } }

        /// <summary> Toggle debug mode for this node </summary>
        public bool DebugMode { get { return m_debugMode; } set { m_debugMode = value; } }

        /// <summary> Trigger a visual cue for this node, in the Editor, at runtime. Mostly used when this node has been activated </summary>
        public bool Ping { get; set; }

        /// <summary> Activate the OnFixedUpdate method for this node, when it is active </summary>
        public bool UseFixedUpdate { get { return m_useFixedUpdate; } set { m_useFixedUpdate = value; } }

        /// <summary> Activate the OnLateUpdate method for this node, when it is active </summary>
        public bool UseLateUpdate { get { return m_useLateUpdate; } set { m_useLateUpdate = value; } }

        /// <summary> Activate the OnUpdate method for this node, when it is active </summary>
        public bool UseUpdate { get { return m_useUpdate; } set { m_useUpdate = value; } }

        /// <summary> Returns a reference to the currently active graph </summary>
        public Graph ActiveGraph { get { return m_activeGraph; } set { m_activeGraph = value; } }

        /// <summary> The minimum number of input sockets for this node. This value is checked when deleting input sockets </summary>
        public int MinimumInputSocketsCount { get { return m_minimumInputSocketsCount; } set { m_minimumInputSocketsCount = value; } }

        /// <summary> The minimum number of output sockets for this node. This value is checked when deleting output sockets </summary>
        public int MinimumOutputSocketsCount { get { return m_minimumOutputSocketsCount; } set { m_minimumOutputSocketsCount = value; } }

        /// <summary> List of all the input sockets this node has </summary>
        public List<Socket> InputSockets { get { return m_inputSockets ?? (m_inputSockets = new List<Socket>()); } set { m_inputSockets = value; } }

        /// <summary> List of all the output sockets this node has </summary>
        public List<Socket> OutputSockets { get { return m_outputSockets ?? (m_outputSockets = new List<Socket>()); } set { m_outputSockets = value; } }

        /// <summary> Returns the type of this node </summary>
        public NodeType NodeType { get { return m_nodeType; } }

        /// <summary> Returns the first input socket. If there isn't one, it returns null </summary>
        public Socket FirstInputSocket { get { return InputSockets.Count > 0 ? InputSockets[0] : null; } }

        /// <summary> Returns the first output socket. If there isn't one, it returns null </summary>
        public Socket FirstOutputSocket { get { return OutputSockets.Count > 0 ? OutputSockets[0] : null; } }

        /// <summary> Returns this node's parent graph id </summary>
        public string GraphId { get { return m_graphId; } set { m_graphId = value; } }

        /// <summary> Returns this node's id </summary>
        public string Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Returns this node's name </summary>
        public string Name { get { return m_name; } }

        #endregion

        #region Unity Methods

        protected virtual void OnEnable() { }

        #endregion

        #region Public Methods

        /// <summary> Activate is called when the parent graph is started and this is a global node </summary>
        public virtual void Activate(Graph portalGraph)
        {
            if (m_activated) return;
            if (m_debugMode) DDebug.Log("Node '" + m_name + "': Activated");
            m_activated = true;
        }

        /// <summary> [Editor Only] AddDefaultSockets is used when the node is created, only in the editor, and is intended to inject the default or custom sockets for this particular node type </summary>
        public virtual void AddDefaultSockets() { }

        /// <summary>
        ///     [Editor Only] Checks if this node has any errors. Because each type of node can have different errors, this method is used to define said custom errors and reflect that in the NodeGraph (for the NodeGUI) and in the Inspector (for
        ///     the NodeEditor)
        /// </summary>
        public virtual void CheckForErrors()
        {
#if UNITY_EDITOR
            CheckThatNodeNameIsNotEmpty();
#endif
        }

        /// <summary> Used to create copies </summary>
        /// <param name="original"> Target node that provides the data that needs to be copied </param>
        public virtual void CopyNode(Node original)
        {
            name = original.name;
            m_id = original.Id;
            m_graphId = original.GraphId;
            m_name = original.Name;
            m_inputSockets = new List<Socket>();
            foreach (Socket otherSocket in original.m_inputSockets) m_inputSockets.Add(new Socket(otherSocket));
            m_outputSockets = new List<Socket>();
            foreach (Socket otherSocket in original.m_outputSockets) m_outputSockets.Add(new Socket(otherSocket));
            m_canBeDeleted = original.m_canBeDeleted;
            m_useUpdate = original.m_useUpdate;
            m_useFixedUpdate = original.m_useFixedUpdate;
            m_useLateUpdate = original.m_useLateUpdate;
            m_nodeType = original.m_nodeType;
            m_minimumInputSocketsCount = original.m_minimumInputSocketsCount;
            m_minimumOutputSocketsCount = original.m_minimumOutputSocketsCount;
            m_allowEmptyNodeName = original.m_allowEmptyNodeName;
            m_allowDuplicateNodeName = original.m_allowDuplicateNodeName;
            m_x = original.GetX();
            m_y = original.GetY();
            m_width = original.GetWidth();
            m_height = original.GetHeight();

            hideFlags = original.hideFlags;
        }

        /// <summary> Deactivate is called when the parent graph is stopped and this is a global node </summary>
        public virtual void Deactivate()
        {
            if (!m_activated) return;
            if (m_debugMode) DDebug.Log("Node '" + m_name + "': Deactivated ");
            m_activated = false;
        }

        /// <summary> [Editor Only] Returns the default node height set in NodySettings </summary>
        public virtual float GetDefaultNodeHeight() { return NodySettings.Instance.DefaultNodeHeight; }

        /// <summary> [Editor Only] Returns the default node width set in NodySettings </summary>
        public virtual float GetDefaultNodeWidth() { return NodySettings.Instance.DefaultNodeWidth; }

        /// <summary> Initializes the node with the given values </summary>
        /// <param name="graph"> Parent graph </param>
        /// <param name="pos"> Graph position </param>
        /// <param name="minimumInputSocketsCount"> Sets the minimum number of input sockets this node can have </param>
        /// <param name="minimumOutputSocketsCount"> Sets the minimum number of output sockets this node can have </param>
        public virtual void InitNode(Graph graph, Vector2 pos, int minimumInputSocketsCount = 1, int minimumOutputSocketsCount = 1)
        {
            name = GetType().Name;
            GenerateNewId();
            m_graphId = graph.Id;
            m_name = name;
            m_inputSockets = new List<Socket>();
            m_outputSockets = new List<Socket>();
            m_canBeDeleted = true;
            m_nodeType = NodeType.General;
            m_useUpdate = false;
            m_useFixedUpdate = false;
            m_useLateUpdate = false;
            m_minimumInputSocketsCount = minimumInputSocketsCount;
            m_minimumOutputSocketsCount = minimumOutputSocketsCount;
            m_x = pos.x;
            m_y = pos.y;
            m_width = GetDefaultNodeWidth();
            m_height = GetDefaultNodeHeight();
        }

        /// <summary> [Editor Only] Used when the node is created, only in the editor, and is intended to set the default settings for this particular node type </summary>
        public virtual void OnCreate() { }

        /// <summary> OnEnterNode is called on the frame when this node becomes active just before any of the node's Update methods are called for the first time </summary>
        /// <param name="previousActiveNode"> The node that was active before this one </param>
        /// <param name="connection"> The connection that activated this node </param>
        public virtual void OnEnter(Node previousActiveNode, Connection connection)
        {
            if (m_debugMode) DDebug.Log("Node '" + m_name + "': OnEnter");
        }

        /// <summary> OnExitNode is called just before this node becomes inactive </summary>
        /// <param name="nextActiveNode"> The node that will become active next</param>
        /// <param name="connection"> The connection that activates the next node </param>
        public virtual void OnExit(Node nextActiveNode, Connection connection)
        {
            if (m_debugMode) DDebug.Log("Node '" + m_name + "': OnExit");
            Ping = true;
            if (connection != null) connection.Ping = true;
        }

        /// <summary> OnFixedUpdate is called every frame, if the node is enabled and active (FixedUpdate Method) </summary>
        public virtual void OnFixedUpdate() { }

        /// <summary> OnLateUpdate is called every frame, if the node is enabled and active (LateUpdate Method) </summary>
        public virtual void OnLateUpdate() { }

        /// <summary> OnUpdate is called every frame, if the node is enabled and active (Update Method) </summary>
        public virtual void OnUpdate() { }


        /// <summary> Convenience method to add a new input socket to this node </summary>
        /// <param name="socketName"> The name of the socket (if null or empty, it will be auto-generated) </param>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="connectionPoints"> The socket connection points locations (if null or empty, it will automatically add two connection points to the left of and the right of the socket) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddInputSocket(string socketName, ConnectionMode connectionMode, List<Vector2> connectionPoints, Type valueType, bool canBeDeleted, bool canBeReordered = true) { return AddSocket(socketName, SocketDirection.Input, connectionMode, connectionPoints, valueType, canBeDeleted, canBeReordered); }

        /// <summary> Convenience method to add a new input socket to this node. This socket will have two connection points automatically added to it and they will be to the left of and the right the socket </summary>
        /// <param name="socketName"> The name of the socket (if null or empty, it will be auto-generated) </param>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddInputSocket(string socketName, ConnectionMode connectionMode, Type valueType, bool canBeDeleted, bool canBeReordered) { return AddSocket(socketName, SocketDirection.Input, connectionMode, GetLeftAndRightConnectionPoints(), valueType, canBeDeleted, canBeReordered); }

        /// <summary> Convenience method to add a new input socket to this node. This socket will have two connection points automatically added to it and they will be to the left of and the right the socket </summary>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddInputSocket(ConnectionMode connectionMode, Type valueType, bool canBeDeleted, bool canBeReordered) { return AddSocket("", SocketDirection.Input, connectionMode, GetLeftAndRightConnectionPoints(), valueType, canBeDeleted, canBeReordered); }

        /// <summary> Convenience method to add a new output socket to this node </summary>
        /// <param name="socketName"> The name of the socket (if null or empty, it will be auto-generated) </param>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="connectionPoints"> The socket connection points locations (if null or empty, it will automatically add two connection points to the left of and the right of the socket) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddOutputSocket(string socketName, ConnectionMode connectionMode, List<Vector2> connectionPoints, Type valueType, bool canBeDeleted, bool canBeReordered) { return AddSocket(socketName, SocketDirection.Output, connectionMode, connectionPoints, valueType, canBeDeleted, canBeReordered); }

        /// <summary> Convenience method to add a new output socket to this node. This socket will have two connection points automatically added to it and they will be to the left of and the right the socket </summary>
        /// <param name="socketName"> The name of the socket (if null or empty, it will be auto-generated) </param>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddOutputSocket(string socketName, ConnectionMode connectionMode, Type valueType, bool canBeDeleted, bool canBeReordered) { return AddSocket(socketName, SocketDirection.Output, connectionMode, GetLeftAndRightConnectionPoints(), valueType, canBeDeleted, canBeReordered); }

        /// <summary> Convenience method to add a new output socket to this node. This socket will have two connection points automatically added to it and they will be to the left of and the right the socket </summary>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        public Socket AddOutputSocket(ConnectionMode connectionMode, Type valueType, bool canBeDeleted, bool canBeReordered) { return AddSocket("", SocketDirection.Output, connectionMode, GetLeftAndRightConnectionPoints(), valueType, canBeDeleted, canBeReordered); }

        /// <summary> Adds a socket to this node </summary>
        /// <param name="socketName"> The name of the socket (if null or empty, it will be auto-generated) </param>
        /// <param name="direction"> The socket direction (Input/Output) </param>
        /// <param name="connectionMode"> The socket connection mode (Multiple/Override) </param>
        /// <param name="connectionPoints"> The socket connection points locations (if null or empty, it will automatically add two connection points to the left of and the right of the socket) </param>
        /// <param name="valueType"> The serialized class that holds additional socket values </param>
        /// <param name="canBeDeleted"> Determines if this socket is a special socket that cannot be deleted </param>
        /// <param name="canBeReordered"> Determines if this socket is a special socket that cannot be reordered </param>
        private Socket AddSocket(string socketName, SocketDirection direction, ConnectionMode connectionMode, List<Vector2> connectionPoints, Type valueType, bool canBeDeleted, bool canBeReordered = true)
        {
            if (connectionPoints == null) connectionPoints = new List<Vector2>(GetLeftAndRightConnectionPoints());
            if (connectionPoints.Count == 0) connectionPoints.AddRange(GetLeftAndRightConnectionPoints());
            var socketNames = new List<string>();
            int counter;
            switch (direction)
            {
                case SocketDirection.Input:
                    foreach (Socket socket in InputSockets)
                        socketNames.Add(socket.SocketName);
                    counter = 0;
                    if (string.IsNullOrEmpty(socketName)) socketName = Socket.DEFAULT_INPUT_SOCKET_NAME_PREFIX + counter;
                    while (socketNames.Contains(socketName)) socketName = Socket.DEFAULT_INPUT_SOCKET_NAME_PREFIX + counter++;
                    var inputSocket = new Socket(this, socketName, direction, connectionMode, connectionPoints, valueType, canBeDeleted, canBeReordered);
                    InputSockets.Add(inputSocket);
                    return inputSocket;
                case SocketDirection.Output:
                    foreach (Socket socket in OutputSockets)
                        socketNames.Add(socket.SocketName);
                    counter = 0;
                    if (string.IsNullOrEmpty(socketName)) socketName = Socket.DEFAULT_OUTPUT_SOCKET_NAME_PREFIX + counter;
                    while (socketNames.Contains(socketName)) socketName = Socket.DEFAULT_OUTPUT_SOCKET_NAME_PREFIX + counter++;
                    var outputSocket = new Socket(this, socketName, direction, connectionMode, connectionPoints, valueType, canBeDeleted, canBeReordered);
                    OutputSockets.Add(outputSocket);
                    return outputSocket;
                default: throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        /// <summary> Returns TRUE if the target socket can be deleted, after checking is it is marked as 'deletable' and that by deleting it the node minimum sockets count does not go below the set threshold </summary>
        /// <param name="socket">Target socket</param>
        public bool CanDeleteSocket(Socket socket)
        {
            if (!socket.CanBeDeleted) return false;                                        //if socket is market as cannot be deleted -> return false -> do not allow the dev to delete this socket
            if (socket.IsInput) return InputSockets.Count > m_minimumInputSocketsCount;    //if socket is input -> make sure the node has a minimum input sockets count before allowing deletion
            if (socket.IsOutput) return OutputSockets.Count > m_minimumOutputSocketsCount; //if socket is output -> make sure the node has a minimum output sockets count before allowing deletion
            return false;                                                                  //event though the socket can be deleted -> the node needs to hold a minimum number of sockets and will not allow to delete this socket
        }

        /// <summary> Returns TRUE if a Connection with the given id can be found on one of this node's sockets </summary>
        /// <param name="connectionId"> Target connection id </param>
        public bool ContainsConnection(string connectionId) { return GetConnection(connectionId) != null; }

        /// <summary> Returns TRUE if a Socket with the give id can be found on this node </summary>
        /// <param name="socketId"> Target socket id</param>
        public bool ContainsSocket(string socketId) { return GetSocketFromId(socketId) != null; }

        /// <summary> Removes all connections this node has. This means that all Input and Output connections will get removed </summary>
        public void Disconnect()
        {
            foreach (Socket inputSocket in InputSockets) inputSocket.Disconnect();
            foreach (Socket outputSocket in OutputSockets) outputSocket.Disconnect();
        }

        /// <summary> Removes all connections to and from the given node id. This means that both Input and Output connections to and from the target node id will get removed </summary>
        /// <param name="nodeId"> Target node id </param>
        public void DisconnectFromNode(string nodeId)
        {
            foreach (Socket inputSocket in InputSockets) inputSocket.DisconnectFromNode(nodeId);
            foreach (Socket outputSocket in OutputSockets) outputSocket.DisconnectFromNode(nodeId);
        }

        /// <summary> Generates a new unique node id for this node and returns the newly generated id value </summary>
        public string GenerateNewId()
        {
            m_id = Guid.NewGuid().ToString();
            return m_id;
        }

        /// <summary> [Editor Only] Returns the default center connection point position for a socket </summary>
        public Vector2 GetCenterConnectionPointPosition()
        {
            return new Vector2(GetWidth() / 2 - NodySettings.Instance.ConnectionPointWidth / 2,
                               NodySettings.Instance.SocketHeight / 2 - NodySettings.Instance.ConnectionPointHeight / 2);
        }

        /// <summary> Returns a connection, from this node, with the matching connection id. Returns null if no connection with the given id is found </summary>
        /// <param name="connectionId"> Target connection id </param>
        public Connection GetConnection(string connectionId)
        {
            Connection connection;
            foreach (Socket inputSocket in InputSockets)
            {
                connection = inputSocket.GetConnection(connectionId);
                if (connection != null) return connection;
            }

            foreach (Socket outputSocket in OutputSockets)
            {
                connection = outputSocket.GetConnection(connectionId);
                if (connection != null) return connection;
            }

            return null;
        }

        /// <summary> Returns a id list of all the input nodes connected to this node via its output sockets </summary>
        public List<string> GetConnectedInputNodesIds()
        {
            var list = new List<string>();
            foreach (Socket outputSocket in OutputSockets)
            {
                if (!outputSocket.IsConnected) continue;
                list.AddRange(outputSocket.GetConnectedNodesIds());
            }

            return list;
        }

        /// <summary> Returns a id list of all the input sockets connected to this node via its output sockets </summary>
        public List<string> GetConnectedInputSocketsIds()
        {
            var list = new List<string>();
            foreach (Socket outputSocket in OutputSockets)
            {
                if (!outputSocket.IsConnected) continue;
                list.AddRange(outputSocket.GetConnectedSocketIds());
            }

            return list;
        }

        /// <summary> Returns a id list of all the output nodes connected to this node via its input sockets </summary>
        public List<string> GetConnectedOutputNodesIds()
        {
            var list = new List<string>();
            foreach (Socket inputSocket in InputSockets)
            {
                if (!inputSocket.IsConnected) continue;
                list.AddRange(inputSocket.GetConnectedNodesIds());
            }

            return list;
        }

        /// <summary> Returns a id list of all the output sockets connected to this node via its input sockets </summary>
        public List<string> GetConnectedOutputSocketsIds()
        {
            var list = new List<string>();
            foreach (Socket inputSocket in InputSockets)
            {
                if (!inputSocket.IsConnected) continue;
                list.AddRange(inputSocket.GetConnectedSocketIds());
            }

            return list;
        }

        /// <summary> [Editor Only] Returns the Rect of this node's footer </summary>
        public Rect GetFooterRect() { return new Rect(GetX() + 6, GetY() - 6 + GetHeight() - NodySettings.Instance.FooterHeight, GetWidth() - 12, NodySettings.Instance.FooterHeight); }

        /// <summary> [Editor Only] Returns the Rect of this node's header </summary>
        public Rect GetHeaderRect() { return new Rect(GetX() + 6, GetY() + 6, GetWidth() - 12, NodySettings.Instance.NodeHeaderHeight); }

        /// <summary> [Editor Only] Returns the height of this node </summary>
        public float GetHeight() { return m_height; }

        /// <summary> Returns an input socket, of this node, with the matching socket id. Returns null if no socket with the given id is found </summary>
        /// <param name="socketId"> Target input socket id </param>
        public Socket GetInputSocketFromId(string socketId)
        {
            foreach (Socket socket in InputSockets)
                if (socket.Id == socketId)
                    return socket;

            return null;
        }

        /// <summary> Returns an input socket, of this node, with the matching socket name. Returns null if no socket with the given name is found </summary>
        /// <param name="socketName"> Target input socket id </param>
        public Socket GetInputSocketFromName(string socketName)
        {
            foreach (Socket socket in InputSockets)
                if (socket.SocketName == socketName)
                    return socket;

            return null;
        }

        /// <summary> [Editor Only] Returns a list of three connection points positions to the left of, the center of and right of the socket </summary>
        private List<Vector2> GetLeftAndCenterAndRightConnectionPoints() { return new List<Vector2> {GetLeftConnectionPointPosition(), GetCenterConnectionPointPosition(), GetRightConnectionPointPosition()}; }

        /// <summary> [Editor Only] Returns the default left connection point position for a socket </summary>
        public Vector2 GetLeftConnectionPointPosition()
        {
            return new Vector2(NodySettings.Instance.ConnectionPointOffsetFromLeftMargin,
                               NodySettings.Instance.SocketHeight / 2 - NodySettings.Instance.ConnectionPointHeight / 2);
        }

        /// <summary> [Editor Only] Returns a list of two connection points positions to the left of and the right of the socket </summary>
        public List<Vector2> GetLeftAndRightConnectionPoints() { return new List<Vector2> {GetLeftConnectionPointPosition(), GetRightConnectionPointPosition()}; }

        /// <summary> [Editor Only] Returns the position of this node </summary>
        public Vector2 GetPosition() { return new Vector2(m_x, m_y); }

        /// <summary> Returns an output socket, of this node, with the matching socket id. Returns null if no socket with the given id is found </summary>
        /// <param name="socketId"> Target output socket id </param>
        public Socket GetOutputSocketFromId(string socketId)
        {
            foreach (Socket socket in OutputSockets)
                if (socket.Id == socketId)
                    return socket;

            return null;
        }

        /// <summary> Returns an output socket, of this node, with the matching socket name. Returns null if no socket with the given name is found </summary>
        /// <param name="socketName"> Target output socket id </param>
        public Socket GetOutputSocketFromName(string socketName)
        {
            foreach (Socket socket in OutputSockets)
                if (socket.SocketName == socketName)
                    return socket;

            return null;
        }

        /// <summary> [Editor Only] Returns the Rect of this node </summary>
        public Rect GetRect() { return new Rect(m_x, m_y, GetWidth(), m_height); }

        /// <summary> [Editor Only] Returns the default right connection point position for a socket </summary>
        public Vector2 GetRightConnectionPointPosition()
        {
            return new Vector2(GetWidth() + NodySettings.Instance.ConnectionPointOffsetFromRightMargin - NodySettings.Instance.ConnectionPointWidth,
                               NodySettings.Instance.SocketHeight / 2 - NodySettings.Instance.ConnectionPointHeight / 2);
        }

        /// <summary> [Editor Only] Returns the width of this node </summary>
        public virtual float GetWidth() { return m_width; }

        /// <summary> [Editor Only] Returns the size of this node (x is width, y is height) </summary>
        public Vector2 GetSize() { return new Vector2(GetWidth(), m_height); }

        /// <summary> Returns a socket, of this node, with the matching socket id. Returns null if no socket with the given id is found </summary>
        /// <param name="socketId"> Target socket id </param>
        public Socket GetSocketFromId(string socketId)
        {
            foreach (Socket socket in InputSockets)
                if (socket.Id == socketId)
                    return socket;

            foreach (Socket socket in OutputSockets)
                if (socket.Id == socketId)
                    return socket;

            return null;
        }

        /// <summary> Returns a socket, of this node, with the matching socket name. Returns null if no socket with the given name is found </summary>
        /// <param name="socketName"> Target socket name </param>
        public Socket GetSocketFromName(string socketName)
        {
            foreach (Socket socket in InputSockets)
                if (socket.SocketName == socketName)
                    return socket;

            foreach (Socket socket in OutputSockets)
                if (socket.SocketName == socketName)
                    return socket;

            return null;
        }

        /// <summary> [Editor Only] Returns the x coordinate of this node </summary>
        public float GetX() { return m_x; }

        /// <summary> [Editor Only] Returns the y coordinate of this node </summary>
        public float GetY() { return m_y; }

        /// <summary> Returns TRUE if at least one socket is connected </summary>
        public bool IsConnected()
        {
            foreach (Socket inputSocket in InputSockets)
                if (inputSocket.IsConnected)
                    return true;
            foreach (Socket outputSocket in OutputSockets)
                if (outputSocket.IsConnected)
                    return true;

            return false;
        }

        /// <summary> Returns TRUE if this node is connected to the target nodeId </summary>
        /// <param name="nodeId"> Target node id</param>
        public bool IsConnectedToNode(string nodeId)
        {
            foreach (Socket inputSocket in InputSockets)
                if (inputSocket.IsConnectedToNode(nodeId))
                    return true;
            foreach (Socket outputSocket in OutputSockets)
                if (outputSocket.IsConnectedToNode(nodeId))
                    return true;

            return false;
        }

        /// <summary> Returns TRUE if one of this node's sockets is connected to the target socket </summary>
        /// <param name="socketId"> Target socket id </param>
        public bool IsConnectedToSocket(string socketId)
        {
            foreach (Socket inputSocket in InputSockets)
                if (inputSocket.IsConnectedToSocket(socketId))
                    return true;
            foreach (Socket outputSocket in OutputSockets)
                if (outputSocket.IsConnectedToSocket(socketId))
                    return true;

            return false;
        }

        /// <summary> Removes the connection with the given id. If a connection with the given id does not exist on this socket, nothing will happen </summary>
        /// <param name="connectionId"> Target connection id </param>
        public void RemoveConnection(string connectionId)
        {
            foreach (Socket inputSocket in InputSockets) inputSocket.RemoveConnection(connectionId);
            foreach (Socket outputSocket in OutputSockets) outputSocket.RemoveConnection(connectionId);
        }

        /// <summary> Set the active Graph for this node </summary>
        /// <param name="graph"> Target Graph </param>
        public void SetActiveGraph(Graph graph) { ActiveGraph = graph; }

        /// <summary> [Editor Only] Set to allow this node to have an empty node name </summary>
        /// <param name="value"> Disable error for empty node name </param>
        protected void SetAllowEmptyNodeName(bool value) { m_allowEmptyNodeName = value; }

        /// <summary> Set to allow this node to have a duplicate node name </summary>
        /// <param name="value"> Disable error for duplicate node name </param>
        protected void SetAllowDuplicateNodeName(bool value) { m_allowDuplicateNodeName = value; }

        /// <summary> Set the name for this node </summary>
        /// <param name="value"> The new node name value </param>
        public void SetName(string value) { m_name = value; }

        /// <summary> [Editor Only] Set this node's type (used mostly for system nodes) </summary>
        /// <param name="nodeType"> The new type of node for this node </param>
        public void SetNodeType(NodeType nodeType) { m_nodeType = nodeType; }

        /// <summary> [Editor Only] Set the position of this node's Rect </summary>
        /// <param name="position"> The new position value </param>
        public void SetPosition(Vector2 position)
        {
            m_x = position.x;
            m_y = position.y;
        }

        /// <summary> [Editor Only] Set the position of this node's Rect </summary>
        /// <param name="x"> The new x coordinate value </param>
        /// <param name="y"> The new y coordinate value </param>
        public void SetPosition(float x, float y)
        {
            m_x = x;
            m_y = y;
        }

        /// <summary> [Editor Only] Set the Rect values for this node </summary>
        /// <param name="rect"> The new rect values </param>
        public void SetRect(Rect rect)
        {
            m_x = rect.x;
            m_y = rect.y;
            m_width = rect.width;
            m_height = rect.height;
        }

        /// <summary> [Editor Only] Set the Rect values for this node </summary>
        /// <param name="position"> The new position value </param>
        /// <param name="size"> The new size value </param>
        public void SetRect(Vector2 position, Vector2 size)
        {
            m_x = position.x;
            m_y = position.y;
            m_width = size.x;
            m_height = size.y;
        }

        /// <summary> [Editor Only] Set the Rect values for this node </summary>
        /// <param name="x"> The new x coordinate value </param>
        /// <param name="y"> The new y coordinate value </param>
        /// <param name="width"> The new width value </param>
        /// <param name="height"> The new height value </param>
        public void SetRect(float x, float y, float width, float height)
        {
            m_x = x;
            m_x = y;
            m_width = width;
            m_height = height;
        }

        /// <summary> [Editor Only] Set the size of this node's Rect </summary>
        /// <param name="size"> The new node size (x is width, y is height) </param>
        public void SetSize(Vector2 size)
        {
            m_width = size.x;
            m_height = size.y;
        }

        /// <summary> [Editor Only] Set the size of this node's Rect </summary>
        /// <param name="width"> The new width value </param>
        /// <param name="height"> The new height value </param>
        public void SetSize(float width, float height)
        {
            m_width = width;
            m_height = height;
        }

        /// <summary> [Editor Only] Set the width of this node's Rect </summary>
        /// <param name="value"> The new width value </param>
        public void SetWidth(float value) { m_width = value; }

        /// <summary> [Editor Only] Set the height of this node's Rect </summary>
        /// <param name="value"> The new height value </param>
        public void SetHeight(float value) { m_height = value; }

        /// <summary> [Editor Only] Set the x coordinate of this node's Rect </summary>
        /// <param name="value"> The new x value </param>
        public void SetX(float value) { m_x = value; }

        /// <summary> [Editor Only] Set the y coordinate of this node's Rect </summary>
        /// <param name="value"> The new y value </param>
        public void SetY(float value) { m_y = value; }

        #endregion

        #region Private Methods

        private void CheckThatNodeNameIsNotEmpty()
        {
#if UNITY_EDITOR
            ErrorNodeNameIsEmpty = false;
            if (AllowEmptyNodeName) return;
            ErrorNodeNameIsEmpty = string.IsNullOrEmpty(m_name.Trim());
#endif
        }

        #endregion
    }
}