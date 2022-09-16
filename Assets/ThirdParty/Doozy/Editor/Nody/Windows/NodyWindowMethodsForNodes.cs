// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private List<Node> m_nodesListWhenGraphOpened;

        /// <summary> Returns TRUE if the target node can be deleted. A node can be deleted if its CanBeDeleted bool is true or if it is not a system node. </summary>
        /// <param name="node"> Target node </param>
        private static bool CanDeleteNode(Node node)
        {
            if (!node.CanBeDeleted) return false;
            switch (node.NodeType)
            {
                case NodeType.Start:
                case NodeType.Enter:
                case NodeType.Exit:
                    return false;
            }

            return true;
        }
        
        private static Node CreateNodeCopy(Node node)
        {
            var copy = CreateInstance(node.GetType()) as Node;
//            DDebug.Log("node.GetType(): " + node.GetType().Name);
            Debug.Assert(copy != null, "copy != null");
            copy.CopyNode(node);
            EditorUtility.SetDirty(copy);
            return copy;
        }

        // ReSharper disable once UnusedMember.Local
        private static T CreateNodeCopy<T>(T original) where T : Node
        {
            var copy = CreateInstance<T>();
            copy.CopyNode(original);
            EditorUtility.SetDirty(copy);
            return copy;
        }

        private static void CopyNodes(List<Node> nodes)
        {
            if (nodes == null) return;
            WindowSettings.CopiedNodes.Clear();
            WindowSettings.SetDirty(false);

            foreach (Node node in nodes)
            {
                switch (node.NodeType)
                {
                    case NodeType.Start:
                    case NodeType.Enter:
                    case NodeType.Exit:
                        continue;
                }

                WindowSettings.CopiedNodes.Add(node);
            }
        }

        private void CreateNodeInTheOpenedGraph(Type type, Vector2 worldPosition, bool recordUndo, bool saveAssets = false)
        {
            Node node = GraphUtils.CreateNode(CurrentGraph, type, worldPosition);
            if (recordUndo) RecordUndo("Create " + type.Name);                                         //record undo for the creation of this node
            CurrentGraph.Nodes.Add(node);                                                              //add the new node reference to the graph's nodes list
            if (!EditorUtility.IsPersistent(node)) AssetDatabase.AddObjectToAsset(node, CurrentGraph); //if the node does not have an asset file -> crete it and nest it under the graph asset file

            switch (node.NodeType)
            {
                case NodeType.General:
                case NodeType.SubGraph:
                    //AUTO CONNECT the new node TO either the START or ENTER node - if the start or enter nodes are not connected
                    Node startOrEnterNode = CurrentGraph.GetStartOrEnterNode();
                    if (startOrEnterNode != null &&                                              //if we have a start or enter node
                        !startOrEnterNode.OutputSockets[0].IsConnected &&                        //that is NOT connected
                        node.InputSockets != null &&                                             //and its inputSockets list is NOT null (sanity check)
                        node.InputSockets.Count > 0)                                             //and it has at least one input socket (again... sanity check)
                        ConnectSockets(startOrEnterNode.OutputSockets[0], node.InputSockets[0]); //connect the new created node with the Start or Enter node
                    break;
            }

            EditorUtility.SetDirty(node);                                             //mark the new node as dirty
            CurrentGraph.SetDirty(saveAssets);                                        //set the graph dirty
            if (recordUndo) GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_CREATED); //notify the system the a node has been created
            SelectNodes(new List<Node> {node}, false, false);                         //select the newly created node
            WindowSettings.SetDirty(false);
        }

        private void UpdateNodesSelectedState(ICollection<Node> selectedNodes)
        {
            foreach (BaseNodeGUI nodeGUI in NodesGUIsDatabase.Values) nodeGUI.IsSelected = selectedNodes.Contains(nodeGUI.Node);
        }
        
        private static void DeleteUnreferencedAssetNodes()
        {
            if (CurrentGraph == null) return;                                                               //check that the graph is not null
            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(CurrentGraph)); //load all of the graph sub assets
            if (objects == null) return;                                                                    //make sure they are not null
            List<Node> foundNodes = objects.OfType<Node>().ToList();                                        //create a temp list of all the found sub assets node
            if (CurrentGraph.Nodes == null) CurrentGraph.Nodes = new List<Node>();                          //sanity check
            bool saveGraph = false;                                                                         //mark true if any sub asset was destroyed
            foreach (Node node in foundNodes)
            {
                if (CurrentGraph.Nodes.Contains(node)) continue; //node reference was FOUND in the Nodes list -> continue
                DestroyImmediate(node, true);                    //node reference was NOT FOUND in the Nodes list -> destroy the asset (it was probably soft deleted)
                saveGraph = true;                                //mark true in order to set the graph dirty and save it
            }

            WindowSettings.DeletedNodes.Clear();
            if (!saveGraph) return; //if no sub asset was destroyed -> stop here
            WindowSettings.SetDirty(false);
            CurrentGraph.SetDirty(true); //save the graph
        }
        
        private void PasteNodes()
        {
            if (!WindowSettings.CanPasteNodes) return; //paste operation cannot work if no nodes have been added to the 'virtual clipboard'

            for (int i = WindowSettings.CopiedNodes.Count - 1; i >= 0; i--) //remove any null nodes
                if (WindowSettings.CopiedNodes[i] == null)
                    WindowSettings.CopiedNodes.RemoveAt(i);

            if (!WindowSettings.CanPasteNodes) return; //paste operation cannot work if no not null nodes are left to be copied

            //create current nodes list
            var copyNodes = new List<Node>();

            //get the first node to the left
            Node firstNodeToTheLeft = null;
            Vector2 firstNodePosition = Vector2.zero;
            foreach (Node node in WindowSettings.CopiedNodes)
            {
                if (node == null) continue;
                if (firstNodeToTheLeft == null)
                {
                    firstNodeToTheLeft = node;
                    firstNodePosition = node.GetPosition();
                }

                if (!(node.GetX() < firstNodePosition.x)) continue;
                firstNodeToTheLeft = node;
                firstNodePosition = node.GetPosition();
            }

            foreach (Node node in WindowSettings.CopiedNodes)
            {
                Node original = node;

                if (original == null) continue;
                Node copy = CreateNodeCopy(original);

                copy.hideFlags = original.hideFlags;

                Vector2 offsetFromFirstNodePosition = Vector2.zero;

                if (original != firstNodeToTheLeft) offsetFromFirstNodePosition = firstNodePosition - copy.GetPosition();

                copy.SetPosition(WorldToGridPosition(CurrentMousePosition));
                copy.SetPosition(copy.GetPosition() - new Vector2(copy.GetWidth() / 2, NodySettings.Instance.NodeHeaderHeight / 2));
                copy.SetPosition(copy.GetPosition() - offsetFromFirstNodePosition);

                copyNodes.Add(copy);
                if (!EditorUtility.IsPersistent(copy)) AssetDatabase.AddObjectToAsset(copy, CurrentGraph);
                CurrentGraph.SetDirty(false);
            }

            //create current sockets list
            var copySockets = new List<Socket>();
            foreach (Node copyNode in copyNodes)
            {
                copySockets.AddRange(copyNode.InputSockets);
                copySockets.AddRange(copyNode.OutputSockets);
            }

            //create current connections list
            var copyConnections = new List<Connection>();
            foreach (Socket copySocket in copySockets)
            foreach (Connection copyConnection in copySocket.Connections)
                copyConnections.Add(copyConnection);

            //generate new connection ids
            foreach (Connection copyConnection in copyConnections)
            {
                string oldConnectionId = copyConnection.Id;              //save previous connection id
                string newConnectionId = copyConnection.GenerateNewId(); //save new conenction id

                foreach (Socket socket in copySockets)                      //go through each socket
                foreach (Connection socketConnection in socket.Connections) //go through each socket's connections
                    if (socketConnection.Id == oldConnectionId)
                        socketConnection.Id = newConnectionId; //update the connection id
            }

            //generate new socket ids
            foreach (Socket copySocket in copySockets)
            {
                string oldSocketId = copySocket.Id;              //save previous socket id
                string newSocketId = copySocket.GenerateNewId(); //save new socket id

                foreach (Connection connection in copyConnections) //update all the connection socket ids
                {
                    if (connection.InputSocketId == oldSocketId) connection.InputSocketId = newSocketId;
                    if (connection.OutputSocketId == oldSocketId) connection.OutputSocketId = newSocketId;
                }
            }

            //generate new node ids
            foreach (Node copyNode in copyNodes)
            {
                string oldNodeId = copyNode.Id;              //save previous node id
                string newNodeId = copyNode.GenerateNewId(); //save new node id

                foreach (Socket copySocket in copySockets) //go through all the sockets
                {
                    if (copySocket.NodeId == oldNodeId) //check that the current node id is equal the the old node id
                        copySocket.NodeId = newNodeId;  //update the node id

                    foreach (Connection copyConnection in copySocket.Connections) //update all the connection node ids
                    {
                        if (copyConnection.OutputNodeId == oldNodeId) copyConnection.OutputNodeId = newNodeId;
                        if (copyConnection.InputNodeId == oldNodeId) copyConnection.InputNodeId = newNodeId;
                    }
                }
            }

            //a list of all the copied nodes ids is needed in order to be able to keep the connections between the copied nodes and remove any 'outside' connections
            var copiedNodesIds = new List<string>(); //create a list of all the copied nodes ids (list contains the newly generated ids)
            foreach (Node copyNode in copyNodes) copiedNodesIds.Add(copyNode.Id);

            foreach (Socket copySocket in copySockets)
                for (int i = copySocket.Connections.Count - 1; i >= 0; i--)
                {
                    Connection c = copySocket.Connections[i];
                    //remove any connection that is made with a node outside the copied nodes list
                    //keep the connections between the copied nodes and remove any 'outside' connections
                    if (copiedNodesIds.Contains(c.InputNodeId) && copiedNodesIds.Contains(c.OutputNodeId)) continue;
                    copySocket.Connections.RemoveAt(i);
                }

            //record undo for paste
            RecordUndo("Paste Nodes");

            //add the references of the pasted nodes to the graph's nodes list
            //do it in reverse order in order to be sorted on top of all the other nodes
            var nodesList = new List<Node>(copyNodes);
            nodesList.AddRange(CurrentGraph.Nodes);
            CurrentGraph.Nodes = nodesList;

            //add the node asset files under the graph asset
            foreach (Node copiedNode in copyNodes)
            {
                if (!EditorUtility.IsPersistent(copiedNode)) AssetDatabase.AddObjectToAsset(copiedNode, CurrentGraph);
                EditorUtility.SetDirty(copiedNode);
            }

            //mark the graph as dirty in order to be able to save the changes
            CurrentGraph.SetDirty(false);

            //reconstruct the databases in order to draw
            ConstructGraphGUI();
            RecalculateAllPointRects();

            //Select the pasted nodes
            SelectNodes(copyNodes, false, false);

            //Paste completed -> clear clipboard
            WindowSettings.CopiedNodes.Clear();
            WindowSettings.SetDirty(false);
        }

        private void SoftDeleteNode(Node node, bool recordUndo, bool saveAssets)
        {
            if (!node.CanBeDeleted) return;
            switch (node.NodeType)
            {
                case NodeType.Start:
                case NodeType.Enter:
                case NodeType.Exit:
                    return;
            }

            if (recordUndo) RecordUndo("Delete Node");

            foreach (VirtualConnection connection in ConnectionsDatabase.Values)
            {
                if (connection.InputNode == node)
                {
                    connection.OutputSocket.DisconnectFromNode(node.Id);
                    EditorUtility.SetDirty(connection.OutputNode);
                }

                if (connection.OutputNode == node)
                {
                    connection.InputSocket.DisconnectFromNode(node.Id);
                    EditorUtility.SetDirty(connection.InputNode);
                }
            }

            WindowSettings.DeletedNodes.Add(node);
            CurrentGraph.Nodes.Remove(node);

            if (WindowSettings.SelectedNodes.Contains(node)) WindowSettings.SelectedNodes.Remove(node);

            CurrentGraph.SetDirty(saveAssets);
            WindowSettings.SetDirty(false);

            ConstructGraphGUI();
        }

        // ReSharper disable once UnusedMember.Local
        private void SoftDeleteNodes(IEnumerable<string> nodeIds, bool recordUndo, bool saveAssets)
        {
            var nodes = new List<Node>();
            foreach (string nodeId in nodeIds)
                nodes.Add(NodesDatabase[nodeId]);
            SoftDeleteNodes(nodes, recordUndo, saveAssets);
        }

        private void SoftDeleteNodes(List<Node> nodes, bool recordUndo, bool saveAssets)
        {
            if (nodes == null || nodes.Count == 0) return;
            nodes = nodes.Where(n => n != null).ToList();

            if (recordUndo) RecordUndo("Delete Nodes");

            //disconnect all the nodes that need to be deleted
            foreach (Node node in nodes)
            {
                if (!CanDeleteNode(node)) continue;

                foreach (VirtualConnection connection in ConnectionsDatabase.Values)
                {
                    if (node == null || connection == null) continue;

                    if (connection.InputNode == node && connection.OutputSocket != null)
                    {
                        connection.OutputSocket.DisconnectFromNode(node.Id);
                        EditorUtility.SetDirty(connection.OutputNode);
                    }

                    if (connection.OutputNode == node && connection.InputSocket != null)
                    {
                        connection.InputSocket.DisconnectFromNode(node.Id);
                        EditorUtility.SetDirty(connection.InputNode);
                    }
                }
            }

            //at this point the nodes have been disconnected
            //'delete' the nodes by adding them the the DeletedNodes list
            foreach (Node node in nodes)
            {
                if (node == null) continue;
                if (!CanDeleteNode(node)) continue;
                WindowSettings.DeletedNodes.Add(node);
                CurrentGraph.Nodes.Remove(node);
            }

            DeselectAll(false);                //deselect any selected nodes
            CurrentGraph.SetDirty(saveAssets); //mark the graph as dirty
            WindowSettings.SetDirty(false);    //mark the editor settings as dirty
            ConstructGraphGUI();               //invalidate all the databases and reconstruct the GUI
        }

        private void UpdateNodesActiveNode()
        {
            if (!EditorApplication.isPlaying)
            {
                if (CurrentGraph == null || CurrentGraph.ActiveNode == null) return;
                NodesGUIsDatabase[CurrentGraph.ActiveNode.Id].IsActive = false;
                CurrentGraph.SetActiveNode(null);
                return;
            }

            foreach (BaseNodeGUI nodeGUI in NodesGUIsDatabase.Values) nodeGUI.IsActive = CurrentGraph.ActiveNode == nodeGUI.Node;
        }
        
        // ReSharper disable once UnusedMember.Local
        private void UpdateNodesSelectedState(ICollection<string> selectedNodeIds)
        {
            foreach (BaseNodeGUI nodeGUI in NodesGUIsDatabase.Values) nodeGUI.IsSelected = selectedNodeIds.Contains(nodeGUI.Node.Id);
        }
    }
}