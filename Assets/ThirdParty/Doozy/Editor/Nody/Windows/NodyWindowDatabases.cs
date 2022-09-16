// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private Dictionary<Node, string> m_nodeNames;
        private Dictionary<string, BaseNodeGUI> m_nodesGUIsDatabase;
        private Dictionary<string, List<VirtualPoint>> m_pointsDatabase;
        private Dictionary<string, Node> m_nodesDatabase;
        private Dictionary<string, Socket> m_socketsDatabase;
        private Dictionary<string, VirtualConnection> m_connectionsDatabase;

        private Dictionary<string, BaseNodeGUI> NodesGUIsDatabase
        {
            get
            {
                if (m_nodesGUIsDatabase != null) return m_nodesGUIsDatabase;
                m_nodesGUIsDatabase = new Dictionary<string, BaseNodeGUI>();
                if (CurrentGraph == null) return m_nodesGUIsDatabase;
                var nodeGUIList = new List<BaseNodeGUI>();
                foreach (Node node in NodesDatabase.Values)
                {
                    if (node == null) continue;                                                //sanity check
                    BaseNodeGUI editor = BaseNodeGUI.GetEditor(node);                          //get the editor for this particular node type
                    editor.Init(BaseNodeGUI.GetSafeWindowId(nodeGUIList), node, CurrentGraph); //initialize the custom BaseNodeGUI editor and also get an unique window id for it
                    nodeGUIList.Add(editor);                                                   //add this editor to the nodyNodeGUI list
                }

                foreach (BaseNodeGUI nodeGUI in nodeGUIList) m_nodesGUIsDatabase.Add(nodeGUI.Node.Id, nodeGUI);
                return m_nodesGUIsDatabase;
            }
        }

        private void ValidateNodesGUIsDatabase()
        {
            if (m_nodesGUIsDatabase == null) return;
            ;
            var invalidKeys = new List<string>(); //temp keys list
            foreach (string key in NodesGUIsDatabase.Keys)
            {
                if (NodesGUIsDatabase[key] == null)
                {
                    invalidKeys.Add(key); //null NodeGUI editor -> remove key
                    continue;
                }

                if (NodesGUIsDatabase[key].Node == null) invalidKeys.Add(key); //null Node reference -> remove key
            }

            foreach (string invalidKey in invalidKeys) NodesGUIsDatabase.Remove(invalidKey); //remove invalid keys
        }

        private Dictionary<string, Node> NodesDatabase
        {
            get
            {
                if (m_nodesDatabase != null) return m_nodesDatabase;
                m_nodesDatabase = new Dictionary<string, Node>();
                for (int i = CurrentGraph.Nodes.Count - 1; i >= 0; i--)
                {
                    Node node = CurrentGraph.Nodes[i];
                    if (node == null)
                    {
                        CurrentGraph.Nodes.RemoveAt(i);
                        GraphUtils.SetDirty(CurrentGraph);
                        continue;
                    }

                    m_nodesDatabase.Add(node.Id, node);
                }

                return m_nodesDatabase;
            }
        }

        private void ValidateNodesDatabase()
        {
            if (m_nodesDatabase == null) return;
            var invalidKeys = new List<string>(); //temp keys list
            foreach (string key in NodesDatabase.Keys)
                if (NodesDatabase[key] == null)
                    invalidKeys.Add(key);                                                //null Node reference -> remove key
            foreach (string invalidKey in invalidKeys) NodesDatabase.Remove(invalidKey); //remove invalid keys
        }

        private Dictionary<Node, string> NodeNames
        {
            get
            {
                if (m_nodeNames != null) return m_nodeNames;
                m_nodeNames = new Dictionary<Node, string>();
                foreach (Node node in NodesDatabase.Values)
                    NodeNames.Add(node, node.Name);
                return m_nodeNames;
            }
        }

        private Dictionary<string, Socket> SocketsDatabase
        {
            get
            {
                if (m_socketsDatabase != null) return m_socketsDatabase;
                m_socketsDatabase = new Dictionary<string, Socket>();
                foreach (Node node in NodesDatabase.Values)
                {
                    for (int i = node.InputSockets.Count - 1; i >= 0; i--)
                    {
                        Socket inputSocket = node.InputSockets[i];
                        if (inputSocket == null)
                        {
                            node.InputSockets.RemoveAt(i);
                            EditorUtility.SetDirty(node);
                            continue;
                        }

                        m_socketsDatabase.Add(inputSocket.Id, inputSocket);
                    }

                    for (int i = node.OutputSockets.Count - 1; i >= 0; i--)
                    {
                        Socket outputSocket = node.OutputSockets[i];
                        if (outputSocket == null)
                        {
                            node.OutputSockets.RemoveAt(i);
                            EditorUtility.SetDirty(node);
                            continue;
                        }

                        m_socketsDatabase.Add(outputSocket.Id, outputSocket);
                    }
                }

                return m_socketsDatabase;
            }
        }

        private void ValidateSocketsDatabase()
        {
            if (m_socketsDatabase == null) return;
            var invalidKeys = new List<string>(); //temp keys list
            foreach (string key in SocketsDatabase.Keys)
            {
                if (SocketsDatabase[key] == null)
                {
                    invalidKeys.Add(key); //null socket -> remove it
                    continue;
                }

                if (!NodesDatabase.ContainsKey(SocketsDatabase[key].NodeId)) invalidKeys.Add(key); //null parent -> remove it
            }

            foreach (string invalidKey in invalidKeys) SocketsDatabase.Remove(invalidKey); //remove invalid keys
        }

        private Dictionary<string, List<VirtualPoint>> PointsDatabase
        {
            get
            {
                if (m_pointsDatabase != null) return m_pointsDatabase;
                m_pointsDatabase = new Dictionary<string, List<VirtualPoint>>();
                foreach (Socket socket in SocketsDatabase.Values) //go through all the sockets
                {
                    m_pointsDatabase.Add(socket.Id, new List<VirtualPoint>()); //create a new entry with this socket id

                    foreach (Vector2 connectionPoint in socket.ConnectionPoints)
                        //create a new virtual point and add it to the virtualPoints list
                        m_pointsDatabase[socket.Id].Add(new VirtualPoint(NodesDatabase[socket.NodeId],
                                                                         SocketsDatabase[socket.Id],
                                                                         connectionPoint + CurrentPanOffset / CurrentZoom,
                                                                         connectionPoint));
                }

                return m_pointsDatabase;
            }
        }

        private void ValidatePointsDatabase()
        {
            if (m_pointsDatabase == null) return;
            var invalidKeys = new List<string>(); //temp keys list
            foreach (string key in PointsDatabase.Keys)
            {
                if (PointsDatabase[key] == null)
                {
                    invalidKeys.Add(key); //null virtual points list -> remove key
                    continue;
                }

                List<VirtualPoint> vList = PointsDatabase[key];
                bool foundInvalidVirtualPoint = false;
                foreach (VirtualPoint virtualPoint in vList)
                {
                    if (virtualPoint == null)
                    {
                        foundInvalidVirtualPoint = true; //null virtual point -> mark virtual point as invalid
                        break;
                    }

                    if (virtualPoint.Node == null)
                    {
                        foundInvalidVirtualPoint = true; //null virtual point parent Node -> mark virtual point as invalid
                        break;
                    }

                    if (virtualPoint.Socket == null)
                    {
                        foundInvalidVirtualPoint = true; //null virtual point parent Socket -> mark virtual point as invalid
                        break;
                    }
                }

                if (foundInvalidVirtualPoint) invalidKeys.Add(key); //found an invalid virtual point in the virtual points list -> remove key
            }

            foreach (string invalidKey in invalidKeys) PointsDatabase.Remove(invalidKey); //remove invalid keys
        }

        private Dictionary<string, VirtualConnection> ConnectionsDatabase
        {
            get
            {
                if (m_connectionsDatabase != null) return m_connectionsDatabase;
                m_connectionsDatabase = new Dictionary<string, VirtualConnection>();
                foreach (Socket socket in SocketsDatabase.Values) //go through all the sockets
                    for (int i = socket.Connections.Count - 1; i >= 0; i--)
                    {
                        Connection connection = socket.Connections[i];
                        if (connection == null)
                        {
                            socket.Connections.RemoveAt(i);
                            continue;
                        }

                        if (!NodesDatabase.ContainsKey(connection.InputNodeId) ||
                            !NodesDatabase.ContainsKey(connection.OutputNodeId))
                        {
                            socket.RemoveConnection(connection.Id);
                            continue;
                        }

                        //check if the connection id has been added to the dictionary
                        if (!m_connectionsDatabase.ContainsKey(connection.Id)) m_connectionsDatabase.Add(connection.Id, new VirtualConnection {ConnectionId = connection.Id}); //if not -> create a new entry with this connection id
                        if (connection.InputSocketId == socket.Id) m_connectionsDatabase[connection.Id].InputSocket = socket;                                                  //reference this socket if it is the connection's InputSocket
                        if (connection.InputNodeId == socket.NodeId) m_connectionsDatabase[connection.Id].InputNode = NodesDatabase[socket.NodeId];                            //reference this socket's parent as the InputNode
                        if (connection.OutputSocketId == socket.Id) m_connectionsDatabase[connection.Id].OutputSocket = socket;                                                //reference this socket if it is the connection's OutputSocket
                        if (connection.OutputNodeId == socket.NodeId) m_connectionsDatabase[connection.Id].OutputNode = NodesDatabase[socket.NodeId];                          //reference this socket's parent as the OutputNode
                    }

                return m_connectionsDatabase;
            }
        }

        private void ValidateConnectionsDatabase()
        {
            if (m_connectionsDatabase == null) return;
            var invalidKeys = new List<string>(); //temp keys list
            foreach (string key in ConnectionsDatabase.Keys)
            {
                if (ConnectionsDatabase[key] == null)
                {
                    invalidKeys.Add(key); //null virtual connection -> remove key
                    continue;
                }

                VirtualConnection v = ConnectionsDatabase[key];
                if (v.OutputNode == null)
                {
                    invalidKeys.Add(key); //null output node -> remove key
                    continue;
                }

                if (v.InputNode == null)
                {
                    invalidKeys.Add(key); //null input node -> remove key
                    continue;
                }

                if (v.OutputSocket == null)
                {
                    invalidKeys.Add(key); //null output socket -> remove key
                    continue;
                }

                if (v.InputSocket == null) invalidKeys.Add(key); //null input socket -> remove key
            }

            foreach (string invalidKey in invalidKeys) ConnectionsDatabase.Remove(invalidKey); //remove invalid keys
        }

        private void InvalidateDatabases()
        {
            m_nodeNames = null;
            m_nodesDatabase = null;
            m_socketsDatabase = null;
            m_pointsDatabase = null;
            m_connectionsDatabase = null;
            m_nodesGUIsDatabase = null;
        }

        private void ValidateDatabases()
        {
            ValidateNodesDatabase();
            ValidateSocketsDatabase();
            ValidatePointsDatabase();
            ValidateConnectionsDatabase();
            ValidateNodesGUIsDatabase();
        }
    }
}