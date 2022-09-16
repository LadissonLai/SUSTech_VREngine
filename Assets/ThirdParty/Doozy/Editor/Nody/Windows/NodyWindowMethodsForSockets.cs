// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody.Models;
using UnityEditor;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        public void DisconnectSocket(Socket socket, bool recordUndo, bool saveAssets = false)
        {
            if (!socket.IsConnected) return;

            if (recordUndo) RecordUndo(GraphAction.Disconnect.ToString());
            List<string> socketConnectionIds = socket.GetConnectionIds();
            foreach (string connectionId in socketConnectionIds)
            {
                if (!ConnectionsDatabase.ContainsKey(connectionId)) continue;
                VirtualConnection vc = ConnectionsDatabase[connectionId];
                if (vc == null)
                {
                    ConnectionsDatabase.Remove(connectionId);
                    continue;
                }

                if (vc.OutputSocket != null)
                {
                    vc.OutputSocket.RemoveConnection(connectionId);
                    EditorUtility.SetDirty(vc.OutputNode);
                }

                if (vc.InputSocket != null)
                {
                    vc.InputSocket.RemoveConnection(connectionId);
                    EditorUtility.SetDirty(vc.InputNode);
                }
                
                ConnectionsDatabase.Remove(connectionId);
            }

            CurrentGraph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_DISCONNECTED, socket.NodeId);
        }

        public void RemoveSocket(Socket socket, bool recordUndo, bool saveAssets = false, bool ignoreCanDeleteSocketFlag = false)
        {
            if (!ignoreCanDeleteSocketFlag && !NodesDatabase[socket.NodeId].CanDeleteSocket(socket)) return;
            if (recordUndo) RecordUndo(UILabels.Delete + " " + UILabels.Socket);
            DisconnectSocket(socket, false);
            PointsDatabase.Remove(socket.Id);
            SocketsDatabase.Remove(socket.Id);
            Node node = NodesDatabase[socket.NodeId];
            if (node == null) return; //sanity check
            if (socket.IsInput) node.InputSockets.Remove(socket);
            if (socket.IsOutput) node.OutputSockets.Remove(socket);
            EditorUtility.SetDirty(node);
            CurrentGraph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_UPDATED, socket.NodeId);
        }

        private void ClearNodesConnections(IEnumerable<Node> nodes, bool recordUndo, bool saveAssets = false)
        {
            if (recordUndo) RecordUndo(GraphAction.Disconnect.ToString());
            foreach (Node node in nodes)
            {
                foreach (Socket inputSocket in node.InputSockets) DisconnectSocket(inputSocket, false);
                foreach (Socket outputSocket in node.OutputSockets) DisconnectSocket(outputSocket, false);
                EditorUtility.SetDirty(node);
            }

            CurrentGraph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_DISCONNECTED);
        }

        private void ClearNodeConnections(Node node, bool recordUndo, bool saveAssets = false)
        {
            if (recordUndo) RecordUndo(GraphAction.Disconnect.ToString());
            foreach (Socket inputSocket in node.InputSockets) DisconnectSocket(inputSocket, false);
            foreach (Socket outputSocket in node.OutputSockets) DisconnectSocket(outputSocket, false);
            EditorUtility.SetDirty(node);
            CurrentGraph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_DISCONNECTED, node.Id);
        }

        private void ConnectSockets(Socket outputSocket, Socket inputSocket, bool recordUndo = true, bool saveAssets = false)
        {
            if (recordUndo) RecordUndo(GraphAction.Connect.ToString());
            if (outputSocket.OverrideConnection) DisconnectSocket(outputSocket, false);
            if (inputSocket.OverrideConnection) DisconnectSocket(inputSocket, false);
            GraphUtils.ConnectSockets(CurrentGraph, outputSocket, inputSocket, saveAssets);
        }

        private void DisconnectVirtualPoint(VirtualPoint virtualPoint, bool recordUndo, bool saveAssets = false)
        {
            if (!virtualPoint.Socket.IsConnected) return;
            if (!virtualPoint.IsConnected) return;

            if (recordUndo) RecordUndo(GraphAction.Disconnect.ToString());

            var connections = new List<VirtualConnection>();
            foreach (VirtualConnection connection in ConnectionsDatabase.Values)
                if (connection.InputVirtualPoint == virtualPoint || connection.OutputVirtualPoint == virtualPoint)
                    connections.Add(connection);

            foreach (VirtualConnection vc in connections)
            {
                vc.OutputSocket.RemoveConnection(vc.ConnectionId);
                vc.InputSocket.RemoveConnection(vc.ConnectionId);
                EditorUtility.SetDirty(vc.OutputNode);
                EditorUtility.SetDirty(vc.InputNode);
                ConnectionsDatabase.Remove(vc.ConnectionId);
            }

            CurrentGraph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_CONNECTION_DELETED);
        }

        private void ReorderSocketMove(Socket socket, bool moveUp, bool recordUndo, bool saveAssets = false)
        {
            if (socket == null) return;
            if (!socket.CanBeReordered) return;
            Node node = NodesDatabase[socket.NodeId];
            if (node == null) return;
            List<Socket> sockets = socket.IsInput ? node.InputSockets : node.OutputSockets;
            int socketIndex = sockets.IndexOf(socket);
            if (moveUp && socketIndex == 0) return;                  //moving up and this is the first socket - it cannot go up in the sockets order
            if (!moveUp && socketIndex == sockets.Count - 1) return; //moving down and this is the last socket - it cannot go down in the sockets order
            if (recordUndo) RecordUndo(moveUp ? UILabels.MoveUp : UILabels.MoveDown);
            if (moveUp) //MOVE UP
            {
                Socket previousSocket = sockets[socketIndex - 1];
                sockets[socketIndex - 1] = new Socket(socket);
                sockets[socketIndex] = new Socket(previousSocket);
            }
            else //MOVE DOWN
            {
                Socket nextSocket = sockets[socketIndex + 1];
                sockets[socketIndex + 1] = new Socket(socket);
                sockets[socketIndex] = new Socket(nextSocket);
            }

            EditorUtility.SetDirty(node);
            CurrentGraph.SetDirty(saveAssets);
            ConstructGraphGUI();
            RecalculateAllPointRects();
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_UPDATED, socket.NodeId);
        }
    }
}