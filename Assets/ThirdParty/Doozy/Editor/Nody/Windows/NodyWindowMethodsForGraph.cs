// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Nody.Nodes;
using UnityEditor;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        public void CloseCurrentGraph(bool clearGraphTabs = true, bool recordUndo = true)
        {
            if (CurrentGraph == null) return;

            if (recordUndo) RecordUndo("Close Graph");

            if (Selection.activeObject == CurrentGraph) //check if the graph asset is selected in the Inspector -> if it is -> deselect the graph
                Selection.activeObject = null;
            else
                foreach (Node node in CurrentGraph.Nodes) //the graph was not selected in the Inspector -> check each node
                {
                    if (Selection.activeObject != node) continue;
                    Selection.activeObject = null; //one of the graph's nodes was selected in the Inspector -> deselect the node
                    break;
                }

//            int undoGroupIndex = Undo.GetCurrentGroup() - 1;
//            if (undoGroupIndex < 0) undoGroupIndex = 0;
//            Undo.CollapseUndoOperations(undoGroupIndex);

            ResetZoom();
            ResetPanOffset();
            WindowSettings.SelectedNodes.Clear();

            for (int i = CurrentGraph.Nodes.Count - 1; i >= 0; i--)
            {
                Node node = CurrentGraph.Nodes[i];
                if (WindowSettings.DeletedNodes.Contains(node))
                    CurrentGraph.Nodes.RemoveAt(i);
            }

            WindowSettings.DeletedNodes.Clear();

            DeleteUnreferencedAssetNodes();

            CurrentGraph = null;
            SetView(View.General);

            InvalidateDatabases();

            if (!clearGraphTabs) return;
            WindowSettings.GraphTabs.Clear();
            WindowSettings.SetDirty(false);
        }

        /// <summary> Opens a save file panel in the project.</summary>
        /// <param name="createSubGraph"> Set TRUE if the newly created graph is a sub graph. </param>
        public void CreateAndOpenGraphWidthDialog(bool createSubGraph)
        {
            var graph = GraphUtils.CreateGraphWidthDialog<Graph>(createSubGraph);
            if (graph == null) return;
            OpenGraph(graph, true, true, true);
            WindowSettings.GraphTabs.Clear();
            WindowSettings.GraphTabs.Add(new GraphTab(graph, CurrentZoom, CurrentPanOffset, new List<Node>(), new List<Node>()));
        }

        /// <summary> Opens a save file panel in the project. Creates a SubGraph and returns a reference to it</summary>
        public static Graph CreateSubGraphWidthDialog() { return GraphUtils.CreateGraphWidthDialog<Graph>(true); }

        private static void DeselectCurrentGraphOrAnyOfItsNodes()
        {
            if (CurrentGraph == null) return;
            if (Selection.activeObject == CurrentGraph)
            {
                Selection.activeObject = null;
                return;
            }

            foreach (Node node in CurrentGraph.Nodes)
            {
                if (Selection.activeObject != node) continue;
                Selection.activeObject = null;
                return;
            }
        }
        
        /// <summary> Loads the target graph.</summary>
        /// <param name="graph">Target graph.</param>
        public void LoadGraph(Graph graph)
        {
            if (graph == null) return; //check that the graph the developer is trying to load that it's not null
            OpenGraph(graph, true, true, true);
            GraphTabs.Clear();
            GraphTabs.Add(new GraphTab(graph, CurrentZoom, CurrentPanOffset, new List<Node>(), new List<Node>()));
            m_graphAssetPath = AssetDatabase.GetAssetPath(CurrentGraph);
        }

        /// <summary> Opens a file panel set to filter ony Graph asset files.</summary>
        public void LoadGraphWidthDialog() { LoadGraph(GraphUtils.LoadGraphWithDialog<Graph>()); }

        private void OpenGraph(Graph graph, bool closeCurrentlyOpenedGraph, bool clearGraphTabs, bool centerAllNodesInWindow, bool recordUndo = true)
        {
            if (graph == null) return;
            if (CurrentGraph == graph) return;
            if (closeCurrentlyOpenedGraph) CloseCurrentGraph(clearGraphTabs);
            if (recordUndo) RecordUndo("Open Graph");
            WindowSettings.AddGraphToRecentlyOpenedGraphs(graph);
            CurrentGraph = graph;
            GraphUtils.CheckAndCreateAnyMissingSystemNodes(graph);
            ConstructGraphGUI();
            if (centerAllNodesInWindow) CenterAllNodesInWindow();
//            Undo.IncrementCurrentGroup();
//            Undo.SetCurrentGroupName(m_graphAssetName);
            SetView(View.Graph);
            Repaint();
            GraphEvent.Send(GraphEvent.EventType.EVENT_GRAPH_OPENED);
            m_graphAssetPath = AssetDatabase.GetAssetPath(CurrentGraph);
        }

        public void OpenSubGraph(SubGraphNode subGraphNode)
        {
            if (subGraphNode == null || subGraphNode.SubGraph == null) return;

            GraphTab currentTab = GraphTabs[GraphTabs.Count - 1];
            currentTab.Zoom = CurrentZoom;
            currentTab.PanOffset = CurrentPanOffset;
            currentTab.SelectedNodes = new List<Node>(SelectedNodes);
            var newTab = new GraphTab(CurrentGraph, subGraphNode, CurrentZoom, CurrentPanOffset, SelectedNodes, DeletedNodes);
            OpenGraph(subGraphNode.SubGraph, true, false, true);
            GraphTabs.Add(newTab);
        }

        private bool ReopenCurrentGraph()
        {
            if (GraphTabs.Count == 0) return false;
            GraphTab activeTab = GraphTabs[GraphTabs.Count - 1];
            Graph graph = activeTab.IsRootTab ? activeTab.Graph : activeTab.SubGraphNode.SubGraph;
            if (graph == null)
            {
                GraphTabs.Clear();
                return false;
            }

            OpenGraph(graph, false, false, false, false);

            //remove any node, that has been soft deleted, from the graph's Nodes list
            foreach (Node deletedNode in DeletedNodes)
            {
                if (!CurrentGraph.Nodes.Contains(deletedNode)) continue;
                CurrentGraph.Nodes.Remove(deletedNode);
                CurrentGraph.SetDirty(false);
            }

            GraphEvent.Send(GraphEvent.EventType.EVENT_GRAPH_OPENED);

            m_graphAssetPath = AssetDatabase.GetAssetPath(CurrentGraph);

            return true;
        }
        
        private static void SaveGraph()
        {
            AssetDatabase.SaveAssets();
            GraphEvent.Send(GraphEvent.EventType.EVENT_SAVED_ASSETS);
            if (CurrentGraph == null) return;
            CurrentGraph.IsDirty = false;
        }

        private static void SaveGraphAs()
        {
            SaveGraph();
            if (CurrentGraph == null) return;

            string path = EditorUtility.SaveFilePanelInProject(UILabels.SaveAs,
                                                               CurrentGraph.name + " Copy",
                                                               "asset",
                                                               UILabels.CreateGraph); //open save file view to get the path
            if (string.IsNullOrEmpty(path)) return;
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(CurrentGraph), path);
        }
    }
}