// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Connections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AnimatedValues;
#endif


// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Nody.Models
{
    /// <summary> Integral part of a Node that manages a list Connections and connection points </summary>
    [Serializable]
    public class Socket
    {
        #region Constants

        public const string DEFAULT_INPUT_SOCKET_NAME_PREFIX = "InputSocket_";
        public const string DEFAULT_OUTPUT_SOCKET_NAME_PREFIX = "OutputSocket_";

        #endregion
      
        #region Properties

        /// <summary> Returns TRUE if this socket can establish multiple connections </summary>
        public bool AcceptsMultipleConnections { get { return m_connectionMode == ConnectionMode.Multiple; } }

        /// <summary> [Editor Only] Returns TRUE if this socket can be removed. This is used to make sure important sockets cannot be deleted by the developer and break the node settings / graph functionality / user experience </summary>
        public bool CanBeDeleted { get { return m_canBeDeleted; } }

        /// <summary> [Editor Only] Returns TRUE if this socket can be reordered. This is used to prevent special sockets from being reordered in the node </summary>
        public bool CanBeReordered { get { return m_canBeReordered; } }

        /// <summary> [Editor Only] Keeps track of all the Connection Points this socket has </summary>
        public List<Vector2> ConnectionPoints { get { return m_connectionPoints ?? (m_connectionPoints = new List<Vector2>()); } set { m_connectionPoints = value; } }

        /// <summary> Keeps track of all the Connections this socket has </summary>
        public List<Connection> Connections { get { return m_connections ?? (m_connections = new List<Connection>()); } set { m_connections = value; } }

        /// <summary> [Editor Only] Returns the curve modifier for this socket. Editor option to adjust the connections curve strength </summary>
        public float CurveModifier { get { return m_curveModifier; } set { m_curveModifier = value; } }

        /// <summary> Returns the first Connection this socket has. Returns null if no connection exists </summary>
        public Connection FirstConnection { get { return Connections.Count > 0 ? Connections[0] : null; } }

        /// <summary> [Editor Only] Overlay image Rect that is drawn on mouse hover. Also used to calculate if to show the socket context menu </summary>
        public Rect HoverRect { get { return m_hoverRect; } set { m_hoverRect = value; } }

        /// <summary> Returns this socket's id </summary>
        public string Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Returns TRUE if this socket has at least one connection (it checks the Connections count) </summary>
        public bool IsConnected { get { return m_connections.Count > 0; } }

        /// <summary> Returns TRUE if this is an Input socket </summary>
        public bool IsInput { get { return m_direction == SocketDirection.Input; } }

        /// <summary> Returns TRUE if this is an Output socket </summary>
        public bool IsOutput { get { return m_direction == SocketDirection.Output; } }

        /// <summary> Returns TRUE if this socket can establish only ONE connection </summary>
        public bool OverrideConnection { get { return m_connectionMode == ConnectionMode.Override; } }

        /// <summary> Returns this socket's parent node id </summary>
        public string NodeId { get { return m_nodeId; } set { m_nodeId = value; } }

        /// <summary> Returns this socket's name </summary>
        public string SocketName { get { return m_socketName; } }

        /// <summary> Returns the socket's value as a string </summary>
        public string Value { get { return m_value; } set { m_value = value; } }

        /// <summary> Returns the value type this socket has and automatically updates the m_valueType if needed </summary>
        public Type ValueType
        {
            get
            {
                if (m_valueType != null) return m_valueType;
                if (string.IsNullOrEmpty(m_valueTypeQualifiedName)) return null;
                m_valueType = Type.GetType(m_valueTypeQualifiedName, false);
                return m_valueType;
            }
            private set
            {
                m_valueType = value;
                if (value == null) return;
                m_valueTypeQualifiedName = value.AssemblyQualifiedName;
            }
        }

        /// <summary> Returns the Connection TypeQualifiedName and automatically updates the m_valueType if needed </summary>
        private string ValueTypeQualifiedName
        {
            get { return m_valueTypeQualifiedName; }
            set
            {
                m_valueTypeQualifiedName = value;
                m_valueType = Type.GetType(value, false);
            }
        }

        #endregion

        #region Private Variables

        /**
		* In order to support Unity serialization for Undo, cyclic reference need to be avoided
		* For that reason, we are storing a node id instead of pointer to the parent node
		*/

        [SerializeField] private ConnectionMode m_connectionMode;
        [SerializeField] private List<Connection> m_connections;
        [SerializeField] private List<Vector2> m_connectionPoints;
        [SerializeField] private SocketDirection m_direction;
        [SerializeField] private Type m_valueType;
        [SerializeField] private bool m_canBeDeleted;
        [SerializeField] private bool m_canBeReordered;
        [SerializeField] private float m_curveModifier;
        [SerializeField] private float m_height;
        [SerializeField] private float m_width;
        [SerializeField] private float m_x;
        [SerializeField] private float m_y;
        [SerializeField] private string m_id;
        [SerializeField] private string m_nodeId;
        [SerializeField] private string m_socketName;
        [SerializeField] private string m_value;
        [SerializeField] private string m_valueTypeQualifiedName;

        [NonSerialized] private Rect m_hoverRect;
#if UNITY_EDITOR
        [NonSerialized] private AnimBool m_showHover;
        public AnimBool ShowHover { get { return m_showHover ?? (m_showHover = new AnimBool(false)); } }
#endif

        #endregion

        #region Constructors

        /// <summary> Construct a new socket </summary>
        /// <param name="node"> The node it belongs to </param>
        /// <param name="socketName"> The socket name. Needs to be unique on the node </param>
        /// <param name="direction"> Input or Output socket </param>
        /// <param name="connectionMode"> Can establish multiple connections OR can establish only one connection </param>
        /// <param name="connectionPoints"> The connection points positions available for this socket </param>
        /// <param name="valueType"> Serialized data type used by this socket </param>
        /// <param name="canBeDeleted"> This is used to make sure important sockets cannot be deleted by the developer and break the node settings / graph functionality / user experience </param>
        /// <param name="canBeReordered"> This is used to prevent special sockets from being reordered </param>
        public Socket(Node node, string socketName, SocketDirection direction, ConnectionMode connectionMode, List<Vector2> connectionPoints, Type valueType, bool canBeDeleted, bool canBeReordered)
        {
            GenerateNewId();
            m_nodeId = node.Id;
            m_socketName = socketName;
            m_direction = direction;
            m_connectionMode = connectionMode;
            m_connectionPoints = connectionPoints;
            m_valueType = valueType;
            m_canBeDeleted = canBeDeleted;
            m_canBeReordered = canBeReordered;
            m_valueTypeQualifiedName = valueType.AssemblyQualifiedName;
            m_value = JsonUtility.ToJson(Activator.CreateInstance(ValueType));
            m_connections = new List<Connection>();
            m_curveModifier = 0;
        }

        /// <summary> Create a deep copy of another socket </summary>
        /// <param name="other"> The other socket we are copying data from </param>
        public Socket(Socket other)
        {
            m_id = other.Id;
            m_nodeId = other.m_nodeId;
            m_socketName = other.m_socketName;
            m_direction = other.m_direction;
            m_connectionMode = other.m_connectionMode;
            m_connectionPoints = new List<Vector2>(other.ConnectionPoints);
            m_x = other.m_x;
            m_y = other.m_y;
            m_width = other.m_width;
            m_height = other.m_height;
            m_valueType = other.m_valueType;
            m_valueTypeQualifiedName = other.m_valueTypeQualifiedName;
            m_value = other.Value;
            m_canBeDeleted = other.CanBeDeleted;
            m_canBeReordered = other.m_canBeReordered;
            m_connections = new List<Connection>();
            foreach (Connection connection in other.m_connections) m_connections.Add(new Connection(connection));
            m_curveModifier = other.m_curveModifier;
        }

        #endregion

        #region Public Methods

        /// <summary> Returns TRUE if this socket can connect to another socket </summary>
        /// <param name="other"> The other socket we are trying to determine if this socket can connect to </param>
        /// <param name="ignoreValueType"> If true, this check will not make sure that the sockets valueTypes match </param>
        public bool CanConnect(Socket other, bool ignoreValueType = false)
        {
            if (other == null) return false;                                                                                                                     //check that the other socket is not null
            if (IsConnectedToSocket(other.Id)) return false;                                                                                                     //check that this socket is not already connected to the other socket
            if (Id == other.Id) return false;                                                                                                                    //make sure we're not trying to connect the same socket
            if (NodeId == other.NodeId) return false;                                                                                                            //check that we are not connecting sockets on the same baseNode
            if (IsInput && other.IsInput) return false;                                                                                                          //check that the sockets are not both input sockets
            if (IsOutput && other.IsOutput) return false;                                                                                                        //check that the sockets are not both output sockets
            if (ignoreValueType) return true;                                                                                                                    //since we're not comparing valueTypes and we passed all the tests -> we conclude that the two sockets can connect
            if (ValueType.BaseType == typeof(PassthroughConnection).BaseType || other.ValueType.BaseType == typeof(PassthroughConnection).BaseType) return true; //this is allows any connection to happen without further checks
            if (ValueType.BaseType != other.ValueType.BaseType) return false;                                                                                    //check that the sockets are of the same type (in order to be able to connect)
            return true;                                                                                                                                         //all the 'can connect' conditions have been met -> we conclude that the two sockets can connect
        }

        /// <summary> Returns TRUE if this socket contains a Connection with the given connection id </summary>
        /// <param name="connectionId"> The connection id to search for </param>
        public bool ContainsConnection(string connectionId)
        {
            foreach (Connection connection in Connections)
                if (connection.Id == connectionId)
                    return true;

            return false;
        }

        /// <summary> Returns TRUE if this socket contains the given Connection </summary>
        /// <param name="connection"> The Connection to search for </param>
        public bool ContainsConnection(Connection connection) { return ContainsConnection(connection.Id); }

        /// <summary> Remove ALL the connections this socket has, by clearing the Connections list </summary>
        public void Disconnect() { Connections.Clear(); }

        /// <summary> Disconnect this socket from the given node id </summary>
        /// <param name="nodeId"> The node id we want this socket to disconnect from </param>
        public void DisconnectFromNode(string nodeId)
        {
            if (!IsConnected) return;                        //socket is not connected -> return
            for (int i = Connections.Count - 1; i >= 0; i--) //iterate through all the connections of this socket
            {
                Connection c = Connections[i];
                if (IsInput && c.OutputNodeId == nodeId) Connections.RemoveAt(i); //found and input connection from the given node id -> remove connection
                if (IsOutput && c.InputNodeId == nodeId) Connections.RemoveAt(i); //found an output connection to the given node id -> remove connection
            }
        }

        /// <summary> [Editor Only] Returns the closest own connection point to position </summary>
        public Vector2 GetClosestConnectionPointToPosition(Vector2 position)
        {
            float minDistance = 100000;                    //arbitrary value that will surely be greater than any other possible distance
            Vector2 closestPoint = ConnectionPoints[0];    //set the closest point as the first connection point
            foreach (Vector2 ownPoint in ConnectionPoints) //iterate through this socket's own connection points list
            {
                float distance = Vector2.Distance(ownPoint, position); //compare the distance between the connection point and the given position
                if (distance > minDistance) continue;                  //the distance is greater than the current minimum distance -> continue
                closestPoint = ownPoint;                               //the distance is smaller than the current minimum distance -> update the selected connection point
                minDistance = distance;                                //update the current minimum distance
            }

            return closestPoint; //return the closest connection point
        }

        /// <summary> [Editor Only] Returns the closest own connection point to the closest connection point on the other socket </summary>
        public Vector2 GetClosestConnectionPointToSocket(Socket other)
        {
            float minDistance = 100000;                            //arbitrary value that will surely be greater than any other possible distance
            Vector2 closestPoint = ConnectionPoints[0];            //set the closest point as the first connection point
            foreach (Vector2 ownPoint in ConnectionPoints)         //iterate through this socket's own connection points list
            foreach (Vector2 otherPoint in other.ConnectionPoints) //iterate through the other socket's connection points list
            {
                float distance = Vector2.Distance(ownPoint, otherPoint); //compare the distance between the connection points
                if (distance > minDistance) continue;                    //the distance is greater than the current minimum distance -> continue
                closestPoint = ownPoint;                                 //the distance is smaller than the current minimum distance -> update the selected connection point
                minDistance = distance;                                  //update the current minimum distance
            }

            return closestPoint; //return the closest connection point
        }

        /// <summary> Returns a list of all the node ids that are connected to this socket. If not connected to any other node, it will return an empty list </summary>
        public List<string> GetConnectedNodesIds()
        {
            var list = new List<string>();
            if (!IsConnected) return list; //if this socket is not currently connected to any other node -> return an empty list
            foreach (Connection connection in Connections)
                list.Add(IsInput ? connection.OutputNodeId : connection.InputNodeId);

            return list;
        }

        /// <summary> Returns a list of all the socket ids that are connected with this socket. If not connected to any other socket, it will return an empty list </summary>
        public List<string> GetConnectedSocketIds()
        {
            var list = new List<string>();
            if (!IsConnected) return list; //if this socket is not currently connected to any other socket -> return an empty list
            foreach (Connection connection in Connections)
                list.Add(IsInput ? connection.OutputSocketId : connection.InputSocketId);

            return list;
        }

        /// <summary> Returns the Connection with the given id. If not found it will return null </summary>
        /// <param name="connectionId"> The connection id to look for and return its reference </param>
        public Connection GetConnection(string connectionId)
        {
            foreach (Connection connection in Connections)
                if (connection.Id == connectionId)
                    return connection;

            return null;
        }

        /// <summary> Returns a list of all the connections ids of this socket </summary>
        public List<string> GetConnectionIds()
        {
            var list = new List<string>();
            foreach (Connection connection in Connections) list.Add(connection.Id);

            return list;
        }

        /// <summary>
        ///     Returns the connection mode this socket has (Multiple/Override)
        ///     <para />
        ///     The connection mode determines if this socket can establish multiple connection or just one
        /// </summary>
        public ConnectionMode GetConnectionMode() { return m_connectionMode; }

        /// <summary> Returns the direction this socket has (Input/Output) </summary>
        public SocketDirection GetDirection() { return m_direction; }

        /// <summary> Generates a new unique socket id for this socket and returns the newly generated id value </summary>
        public string GenerateNewId()
        {
            m_id = Guid.NewGuid().ToString();
            return m_id;
        }

        /// <summary> [Editor Only] Returns the height of this socket </summary>
        public float GetHeight() { return m_height; }

        /// <summary> [Editor Only] Returns the position of this socket </summary>
        public Vector2 GetPosition() { return new Vector2(m_x, m_y); }

        /// <summary> [Editor Only] Returns the Rect of this socket </summary>
        public Rect GetRect() { return new Rect(m_x, m_y, m_width, m_height); }

        /// <summary> [Editor Only] Returns the size of this socket (x is width, y is height) </summary>
        public Vector2 GetSize() { return new Vector2(m_width, m_height); }

        /// <summary> [Editor Only] Returns the width of this socket </summary>
        public float GetWidth() { return m_width; }

        /// <summary> [Editor Only] Returns the x coordinate of this socket </summary>
        public float GetX() { return m_x; }

        /// <summary> [Editor Only] Returns the y coordinate of this socket </summary>
        public float GetY() { return m_y; }

        /// <summary> Returns TRUE if this socket is connected to the given node id </summary>
        /// <param name="nodeId"> The node id to search for and determine if this socket is connected to or not </param>
        public bool IsConnectedToNode(string nodeId)
        {
            foreach (Connection connection in Connections) //iterate through all the connections list
            {
                if (IsInput && connection.OutputNodeId == nodeId) return true; //if this is an input socket -> look for the node id at the output socket of the connection
                if (IsOutput && connection.InputNodeId == nodeId) return true; //if this is an output socket -> look for the node id at the input socket of the connection
            }

            return false;
        }

        /// <summary> Returns TRUE if this socket is connected to the given socket id </summary>
        /// <param name="socketId"> The socket id to search for and determine if this socket is connected to or not </param>
        public bool IsConnectedToSocket(string socketId)
        {
            foreach (Connection connection in Connections) //iterate through all the connections list
            {
                if (IsInput && connection.OutputSocketId == socketId) return true; //if this is an input socket -> look for the socket id at the output socket of the connection
                if (IsOutput && connection.InputSocketId == socketId) return true; //if this is an output socket -> look for the socket id at the input socket of the connection
            }

            return false;
        }

        /// <summary> Removes a connection with the given connection id from this socket </summary>
        /// <param name="connectionId"> The connection id we want removed from this socket </param>
        public void RemoveConnection(string connectionId)
        {
            if (!ContainsConnection(connectionId)) return;   //if the connections list does not contain this connection id -> return;
            for (int i = Connections.Count - 1; i >= 0; i--) //iterate through all the connections list
                if (Connections[i].Id == connectionId)       //if a connection has the given connection id -> remove connection
                    Connections.RemoveAt(i);
        }

        /// <summary> [Editor Only] Sets the height of this socket's Rect </summary>
        /// <param name="value"> The new height value </param>
        public void SetHeight(float value) { m_height = value; }

        /// <summary> Set the name for this socket </summary>
        /// <param name="value"> The new socket name value </param>
        public void SetName(string value) { m_socketName = value; }

        /// <summary> [Editor Only] Sets the position of this socket's Rect </summary>
        /// <param name="position"> The new position value </param>
        public void SetPosition(Vector2 position)
        {
            m_x = position.x;
            m_y = position.y;
        }

        /// <summary> [Editor Only] Sets the position of this socket's Rect </summary>
        /// <param name="x"> The new x coordinate value </param>
        /// <param name="y"> The new y coordinate value </param>
        public void SetPosition(float x, float y)
        {
            m_x = x;
            m_y = y;
        }

        /// <summary> [Editor Only] Sets the Rect value for this socket </summary>
        /// <param name="rect"> The new rect values </param>
        public void SetRect(Rect rect)
        {
            m_x = rect.x;
            m_y = rect.y;
            m_width = rect.width;
            m_height = rect.height;
        }

        /// <summary> [Editor Only] Sets the Rect values for this socket </summary>
        /// <param name="position"> The new position value </param>
        /// <param name="size"> The new size value </param>
        public void SetRect(Vector2 position, Vector2 size)
        {
            m_x = position.x;
            m_y = position.y;
            m_width = size.x;
            m_height = size.y;
        }

        /// <summary> [Editor Only] Sets the Rect values for this socket </summary>
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

        /// <summary> [Editor Only] Sets the size of this socket's Rect </summary>
        /// <param name="size"> The new socket size (x is width, y is height) </param>
        public void SetSize(Vector2 size)
        {
            m_width = size.x;
            m_height = size.y;
        }

        /// <summary> [Editor Only] Sets the size of this socket's Rect </summary>
        /// <param name="width"> The new width value </param>
        /// <param name="height"> The new height value </param>
        public void SetSize(float width, float height)
        {
            m_width = width;
            m_height = height;
        }

        /// <summary> [Editor Only] Sets the width of this socket's Rect </summary>
        /// <param name="value"> The new width value </param>
        public void SetWidth(float value) { m_width = value; }

        /// <summary> [Editor Only] Sets the x coordinate of this socket's Rect </summary>
        /// <param name="value"> The new x value </param>
        public void SetX(float value) { m_x = value; }

        /// <summary> [Editor Only] Sets the y coordinate of this socket's Rect </summary>
        /// <param name="value"> The new y value </param>
        public void SetY(float value) { m_y = value; }

        /// <summary> [Editor Only] Updates the socket hover Rect. This is the 'selection' box that appears when the mouse is over the socket </summary>
        public void UpdateHoverRect()
        {
            //calculate the socket hover rect -> this is the 'selection' box that appears when the mouse is over the socket
            HoverRect = new Rect(GetRect().x + 6, // + NodySettings.Singleton.ConnectionPointWidth / 2 + 6,
                                 GetRect().y + 2,
                                 GetRect().width - 12, //- 24 - NodySettings.Singleton.ConnectionPointWidth,
                                 GetRect().height - 3);
        }

        #endregion
    }
}