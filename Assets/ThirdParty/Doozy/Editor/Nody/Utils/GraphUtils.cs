// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.IO;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Nody.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Utils
{
    public static class GraphUtils
    {
        /// <summary> Direct reference to the active language pack </summary>
        private static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        /// <summary> Creates a new Graph of the given derived type at the given path </summary>
        /// <param name="path"> Asset path where the graph will be created </param>
        /// <param name="createSubGraph"> Set TRUE if the newly created graph is a sub graph. </param>
        public static T CreateGraph<T>(string path, bool createSubGraph) where T : Graph
        {
            if (string.IsNullOrEmpty(path)) return null;
            var graph = ScriptableObject.CreateInstance<T>();
            graph.name = Path.GetFileNameWithoutExtension(path);
            graph.Id = Guid.NewGuid().ToString();
            graph.SetGraphVersionAndLastModifiedTime();
            if (AssetDatabase.LoadAssetAtPath<ScriptableObject>(path) != null) AssetDatabase.MoveAssetToTrash(path); //if the graph exists (and overwrite has been selected) -> move the previous asset file to Trash
            AssetDatabase.CreateAsset(graph, path);
            if (createSubGraph)
                CreateEnterAndExitNodes(graph);
            else
                CreateStartNode(graph);
            graph.SetDirty(true);
            return graph;
        }

        /// <summary> Opens a save file panel in the project </summary>
        /// <param name="createSubGraph"> Set TRUE if the newly created graph is a sub graph. </param>
        public static T CreateGraphWidthDialog<T>(bool createSubGraph = false) where T : Graph
        {
            string path = EditorUtility.SaveFilePanelInProject(createSubGraph ? UILabels.CreateSubGraph : UILabels.CreateGraph,
                                                               createSubGraph ? UILabels.SubGraph : UILabels.Graph,
                                                               "asset",
                                                               createSubGraph ? UILabels.CreateNewGraphAsSubGraph : UILabels.CreateNewGraph); //open save file view to get the path
            return string.IsNullOrEmpty(path) ? null : CreateGraph<T>(path, createSubGraph);
        }

        /// <summary> Loads a graph from the given path </summary>
        /// <param name="path"> Asset path where the graph can be found </param>
        public static T LoadGraph<T>(string path) where T : Graph
        {
            if (string.IsNullOrEmpty(path)) return null;
            var graph = AssetDatabase.LoadAssetAtPath<T>(FileUtil.GetProjectRelativePath(path));
            return graph == null ? null : graph;
        }

        /// <summary> Opens a file panel set to filter only the given Graph type and returns the selected asset file </summary>
        public static T LoadGraphWithDialog<T>() where T : Graph
        {
            string path = EditorUtility.OpenFilePanelWithFilters(UILabels.OpenGraph, "", new[] {typeof(T).Name, "asset"});
            return LoadGraph<T>(path);
        }

        /// <summary> Sets the current script version to the graph and also the last modified time using DateTime.UtcNow </summary>
        /// <param name="graph"> Target graph </param>
        private static void SetGraphVersionAndLastModifiedTime<T>(this T graph) where T : Graph
        {
            graph.SetVersion(Graph.FILE_VERSION);
            graph.SetLastModified(DateTime.UtcNow.ToFileTimeUtc().ToString());
        }

        /// <summary> Sets this graph as dirty by flagging the IsDirty as true and setting EditorUtility.SetDirty to it. Also it updates the version and last modified time </summary>
        /// <param name="graph"> target graph </param>
        /// <param name="saveAssets"> Perform AssetDatabase.SaveAssets? </param>
        public static void SetDirty<T>(this T graph, bool saveAssets = false) where T : Graph
        {
            graph.SetGraphVersionAndLastModifiedTime();
            graph.IsDirty = true;
            EditorUtility.SetDirty(graph);
            if (saveAssets) AssetDatabase.SaveAssets();
        }


        /// <summary> Creates the StartNode if the passed graph is a Graph, or the EnterNode and ExitNode if the passed graph is a SubGraph </summary>
        /// <param name="graph"> Target Graph </param>
        public static void CheckAndCreateAnyMissingSystemNodes(Graph graph)
        {
            if (graph.IsSubGraph)
                CreateEnterAndExitNodes(graph);
            else
                CreateStartNode(graph);
        }

        /// <summary> Create a node of the given nodeType in the target graph at the set world position </summary>
        /// <param name="graph"> Target graph where the node will get created </param>
        /// <param name="nodeType"> NamesDatabaseType of node </param>
        /// <param name="nodeWorldPosition"> World position (in the graph) for the newly created node </param>
        public static Node CreateNode(Graph graph, Type nodeType, Vector2 nodeWorldPosition)
        {
            var node = ScriptableObject.CreateInstance(nodeType) as Node;      //create an instance of the new node
            Debug.Assert(node != null, "node != null");                        //assume that this node is not null by default
            nodeWorldPosition.x -= node.GetDefaultNodeWidth() / 2;             //calculate the X world position
            nodeWorldPosition.y -= NodySettings.Instance.NodeHeaderHeight / 2; //calculate the Y world position (of the header do that we can place the middle of the node header at the mouse position)
            node.InitNode(graph, nodeWorldPosition);                           //initialize the node with its default values
            node.OnCreate();                                                   //set custom setup values for this particular node type
            node.AddDefaultSockets();                                          //add the default custom sockets for this node
            node.hideFlags = NodySettings.Instance.DefaultHideFlagsForNodes;   //set the default hide flags for the newly created node
            EditorUtility.SetDirty(node);                                      //mark the new node as dirty
            return node;                                                       //return the node
        }

        /// <summary> Returns TRUE if it creates a StartNode in the given target graph. Note that if the graph is a sub-graph or if it already has a StartNode, it will return FALSE </summary>
        /// <param name="graph"> Target graph </param>
        public static void CreateStartNode(Graph graph)
        {
            //Check the graph for system nodes
            Node startNode = graph.GetStartNode();
            Node enterNode = graph.GetEnterNode();
            Node exitNode = graph.GetExitNode();

            bool startNodeWasCreated = false;
            if (startNode == null)
            {
                startNode = CreateNode(graph, typeof(StartNode), Vector2.zero);                               //create start node
                graph.Nodes.Add(startNode);                                                                   //add start node to Nodes list
                if (!EditorUtility.IsPersistent(startNode)) AssetDatabase.AddObjectToAsset(startNode, graph); //create the asset file for the node and attach it to the graph asset file
                startNodeWasCreated = true;
            }

            if (enterNode != null) //this graph has an enter node -> it needs to be deleted
            {
                //because start node was created and enter node is not null -> set start node position as the enter node position
                if (startNodeWasCreated) startNode.SetPosition(enterNode.GetPosition());

                if (enterNode.IsConnected()) //disconnect enter node
                {
                    string targetSocketId = enterNode.OutputSockets[0].Connections[0].InputSocketId;
                    Node targetNode = graph.GetNodeById(enterNode.OutputSockets[0].Connections[0].InputNodeId);
                    if (targetNode != null) //make sure a node connected to the EnterNode is not null (sanity check)
                    {
                        targetNode.GetSocketFromId(targetSocketId).DisconnectFromNode(enterNode.Id);
                        if (startNodeWasCreated) //because start node was created -> connect it to the same node enter node was connected
                            ConnectSockets(graph, startNode.OutputSockets[0], targetNode.GetSocketFromId(targetSocketId));
                    }
                }

                if (graph.Nodes.Contains(enterNode)) graph.Nodes.Remove(enterNode); //remove enter node from Nodes list
                Object.DestroyImmediate(enterNode, true);                           //destroy enter node asset
            }


            if (exitNode != null) //this graph has an exit node -> it needs to be deleted
            {
                if (exitNode.IsConnected()) //disconnect exit node
                    for (int i = exitNode.InputSockets[0].Connections.Count - 1; i >= 0; i--)
                    {
                        Connection connection = exitNode.InputSockets[0].Connections[i];
                        Node connectedNode = graph.GetNodeById(connection.OutputNodeId);
                        exitNode.InputSockets[0].DisconnectFromNode(connection.OutputNodeId);
                        if (connectedNode != null)
                            connectedNode.GetSocketFromId(connection.OutputSocketId).DisconnectFromNode(exitNode.Id);
                    }

                if (graph.Nodes.Contains(exitNode)) graph.Nodes.Remove(exitNode); //remove exit node from Nodes list
                Object.DestroyImmediate(exitNode, true);                          //destroy exit node asset
            }

            graph.IsSubGraph = false;                                 //update graph type
            graph.SetDirty(false);                                    //set the graph as dirty
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_CREATED); //StartNode has been created
        }

        /// <summary> Returns TRUE if it creates an Enter and/or an Exit node in the given target graph. Note that if the graph is a graph or if it already has an Enter and an Exit nodes, it will return FALSE </summary>
        /// <param name="graph"> Target Graph </param>
        public static void CreateEnterAndExitNodes(Graph graph)
        {
            const float spaceBetweenNodes = 24f;

            //Check the graph for system nodes
            Node startNode = graph.GetStartNode();
            Node enterNode = graph.GetEnterNode();
            Node exitNode = graph.GetExitNode();

            bool enterNodeWasCreated = false;
            if (enterNode == null)
            {
                enterNode = CreateNode(graph, typeof(EnterNode), Vector2.zero);                               //create enter node
                graph.Nodes.Add(enterNode);                                                                   //add start node to Nodes list
                if (!EditorUtility.IsPersistent(enterNode)) AssetDatabase.AddObjectToAsset(enterNode, graph); //create the asset file for the node and attach it to the graph asset file
                enterNodeWasCreated = true;
            }

            if (startNode != null)
            {
                //because enter node was created and start node is not null -> set enter node position as the start node position
                if (enterNodeWasCreated) enterNode.SetPosition(startNode.GetPosition());

                if (startNode.IsConnected()) //disconnect start node
                {
                    string targetSocketId = startNode.OutputSockets[0].Connections[0].InputSocketId;
                    Node targetNode = graph.GetNodeById(startNode.OutputSockets[0].Connections[0].InputNodeId);
                    if (targetNode != null) //make sure a node connected to the StartNode is not null (sanity check)
                    {
                        targetNode.GetSocketFromId(targetSocketId).DisconnectFromNode(startNode.Id);
                        ConnectSockets(graph, enterNode.OutputSockets[0], targetNode.GetSocketFromId(targetSocketId));
                    }
                }

                if (graph.Nodes.Contains(startNode)) graph.Nodes.Remove(startNode); //remove start node from Nodes list
                Object.DestroyImmediate(startNode, true);                           //destroy enter node asset
            }

            if (exitNode == null)
            {
                exitNode = CreateNode(graph, typeof(ExitNode), new Vector2(enterNode.GetX() + NodySettings.Instance.ExitNodeWidth * 2 + spaceBetweenNodes,
                                                                           enterNode.GetY() + NodySettings.Instance.NodeHeaderHeight / 2));
                graph.Nodes.Add(exitNode);                                                                  //add exit node to Nodes kist
                if (!EditorUtility.IsPersistent(exitNode)) AssetDatabase.AddObjectToAsset(exitNode, graph); //create the asset file for the node and attach it to the graph asset file
            }

            graph.IsSubGraph = true;                                  //update graph type
            graph.SetDirty(false);                                    //set the graph as dirty
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_CREATED); //EnterNode and/or ExitNode has/have been created
        }

        /// <summary> Connect two sockets </summary>
        /// <param name="graph"> Parent graph </param>
        /// <param name="outputSocket"> Output Socket</param>
        /// <param name="inputSocket"> Input Socket </param>
        /// <param name="saveAssets"> Perform AssetDatabase.Save </param>
        public static void ConnectSockets(Graph graph, Socket outputSocket, Socket inputSocket, bool saveAssets = false)
        {
//            if (outputSocket.OverrideConnection) DisconnectSocket(outputSocket, false);
//            if (inputSocket.OverrideConnection) DisconnectSocket(inputSocket, false);
            if (outputSocket.OverrideConnection)
            {
                outputSocket.Disconnect();
                GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_DISCONNECTED, outputSocket.NodeId);
            }

            if (inputSocket.OverrideConnection)
            {
                inputSocket.Disconnect();
                GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_DISCONNECTED, inputSocket.NodeId);
            }

            var connection = new Connection(outputSocket, inputSocket);
            outputSocket.Connections.Add(connection);
            inputSocket.Connections.Add(connection);
            graph.SetDirty(saveAssets);
            GraphEvent.Send(GraphEvent.EventType.EVENT_CONNECTION_ESTABLISHED);
        }
    }
}