// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private Rect m_selectionRect;
        private SelectPoint m_startSelectPoint;
        private List<Node> m_tempNodesList;
        private List<Node> m_selectedNodesWhileSelecting;

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool CenterAllNodesInWindow()
        {
            if (CurrentGraph.Nodes.Count == 0) return false; //if the graph has no nodes -> return
            SelectAll(false);
            CurrentZoom = NodyWindowSettings.ZOOM_OVERVIEW;
            Node firstNode = WindowSettings.SelectedNodes[0];
            float xMin = firstNode.GetRect().xMin;
            float yMin = firstNode.GetRect().yMin;
            float xMax = firstNode.GetRect().xMax;
            float yMax = firstNode.GetRect().yMax;
            foreach (Node selectedNode in WindowSettings.SelectedNodes)
            {
                if (xMin > selectedNode.GetRect().xMin) xMin = selectedNode.GetRect().xMin;
                if (yMin > selectedNode.GetRect().yMin) yMin = selectedNode.GetRect().yMin;
                if (xMax < selectedNode.GetRect().xMax) xMax = selectedNode.GetRect().xMax;
                if (yMax < selectedNode.GetRect().yMax) yMax = selectedNode.GetRect().yMax;
            }

            var selectedNodesRect = new Rect(xMin, yMin, Mathf.Abs(xMax - xMin), Mathf.Abs(yMax - yMin));
            selectedNodesRect.position = GridToWorldPosition(selectedNodesRect.position);
            selectedNodesRect.size *= CurrentZoom;
            Vector2 selectionInCenterPosition = position.size / 2 - selectedNodesRect.size / 2;
            Vector2 panOffsetToCenter = (WorldToGridPosition(selectionInCenterPosition) - WorldToGridPosition(selectedNodesRect.position)) * CurrentZoom;
            CurrentPanOffset += panOffsetToCenter;
            DeselectAll(false);
            ConstructGraphGUI();
            return true;
        }

        private bool CenterSelectedNodesInWindow()
        {
            if (WindowSettings.SelectedNodes.Count == 0) return false; //no node is currently selected -> return
            CurrentZoom = NodyWindowSettings.Instance.DefaultZoom;
            Node firstNode = WindowSettings.SelectedNodes[0];
            float xMin = firstNode.GetRect().xMin;
            float yMin = firstNode.GetRect().yMin;
            float xMax = firstNode.GetRect().xMax;
            float yMax = firstNode.GetRect().yMax;
            foreach (Node selectedNode in WindowSettings.SelectedNodes)
            {
                if (xMin > selectedNode.GetRect().xMin) xMin = selectedNode.GetRect().xMin;
                if (yMin > selectedNode.GetRect().yMin) yMin = selectedNode.GetRect().yMin;
                if (xMax < selectedNode.GetRect().xMax) xMax = selectedNode.GetRect().xMax;
                if (yMax < selectedNode.GetRect().yMax) yMax = selectedNode.GetRect().yMax;
            }

            var selectedNodesRect = new Rect(xMin, yMin, Mathf.Abs(xMax - xMin), Mathf.Abs(yMax - yMin));
            selectedNodesRect.position = GridToWorldPosition(selectedNodesRect.position);
            selectedNodesRect.size *= CurrentZoom;
            Vector2 selectionInCenterPosition = position.size / 2 - selectedNodesRect.size / 2;
            Vector2 panOffsetToCenter = (WorldToGridPosition(selectionInCenterPosition) - WorldToGridPosition(selectedNodesRect.position)) * CurrentZoom;
            CurrentPanOffset += panOffsetToCenter;
            ConstructGraphGUI();
            return true;
        }

        private void DeselectAll(bool recordUndo)
        {
            if (recordUndo) RecordUndo(GraphAction.DeselectAll.ToString());
            WindowSettings.SelectedNodes.Clear();
            UpdateNodesSelectedState(WindowSettings.SelectedNodes);
            Selection.activeObject = CurrentGraph;
            WindowSettings.SetDirty(false);
        }

        private void GoToStartOrEnterNode()
        {
            Node targetNode = null;
            NodeType nodeType = CurrentGraph.IsSubGraph ? NodeType.Enter : NodeType.Start;

            foreach (Node node in NodesDatabase.Values)
            {
                if (node.NodeType != nodeType) continue;
                targetNode = node;
                break;
            }

            if (targetNode == null) return; //no start/enter node found -> return as we have nothing to select

            SelectNodes(new List<Node> {targetNode}, false, true);
            CenterSelectedNodesInWindow();
        }

        private void PrepareToCreateSelectionBox(Vector2 currentMousePosition)
        {
            currentMousePosition = currentMousePosition / CurrentZoom;

            m_selectionRect = new Rect();                                   //clear the previous selection rect
            m_startSelectPoint = new SelectPoint(currentMousePosition);     //get the start position for the selection rect
            m_tempNodesList = new List<Node>();                             //clear the previous saved start selection box selected nodes
            m_tempNodesList = new List<Node>(WindowSettings.SelectedNodes); //save the currently selected node (we need this in order to be able to invert the selection)
            m_selectedNodesWhileSelecting = new List<Node>();               //clear the nodes contained in the previous selection
        }

        private void SelectAll(bool recordUndo)
        {
            if (recordUndo) RecordUndo(GraphAction.SelectAll.ToString());
            WindowSettings.SelectedNodes.Clear();
            WindowSettings.SelectedNodes.AddRange(CurrentGraph.Nodes);
            UpdateNodesSelectedState(WindowSettings.SelectedNodes);
            DGUI.Utility.SelectObjectsInInspector(WindowSettings.SelectedNodes.ToArray());
            WindowSettings.SetDirty(false);
        }

        private void SelectNodes(IEnumerable<Node> nodes, bool addToCurrentSelection, bool recordUndo)
        {
            if (recordUndo) RecordUndo(GraphAction.SelectNodes.ToString());   //record undo select nodes
            if (!addToCurrentSelection) WindowSettings.SelectedNodes.Clear(); //if this is a new selection -> clear the previous one

            //check if the GUI needs to get reconstructed
            bool needsToReconstructGraphGUI = false;
            foreach (Node node in nodes)
            {
                if (!NodesDatabase.ContainsKey(node.Id)) //found a node id not in the constructed database -> skip this entry and trigger a construct gui
                {
                    needsToReconstructGraphGUI = true;
                    continue;
                }

                if (WindowSettings.SelectedNodes.Contains(node))
                {
                    WindowSettings.SelectedNodes.Remove(node);
                    continue;
                }

                WindowSettings.SelectedNodes.Add(node);
            }

            if (needsToReconstructGraphGUI) ConstructGraphGUI();
            UpdateNodesSelectedState(WindowSettings.SelectedNodes);                        //update the currently selected nodes visual (normal style or selected style)
            DGUI.Utility.SelectObjectsInInspector(WindowSettings.SelectedNodes.ToArray()); //select the nodes in the Inspector view
        }

        private void UpdateSelectBoxSelectedNodesWhileSelecting(Event e)
        {
//            if (m_mode != GraphMode.Select) return;
            //update the current selection
            m_selectedNodesWhileSelecting.Clear();
            foreach (Node node in NodesDatabase.Values)
            {
                var gridRect = new Rect(node.GetPosition() + CurrentPanOffset / CurrentZoom, node.GetSize());
                if (m_selectionRect.Overlaps(gridRect))
                    m_selectedNodesWhileSelecting.Add(node);
            }

            //shift is pressed -> toggle selection
            if (e.shift)
                foreach (Node tempNode in m_tempNodesList)
                    if (!m_selectedNodesWhileSelecting.Contains(tempNode))
                        m_selectedNodesWhileSelecting.Add(tempNode);
                    else
                        m_selectedNodesWhileSelecting.Remove(tempNode);
        }
        
        private void UpdateSelectionBox(Vector2 currentMousePosition)
        {
            currentMousePosition = currentMousePosition / CurrentZoom;

            if (m_startSelectPoint == null)
            {
                m_selectionRect = new Rect();
                return;
            }

            //calculate the selection rect position and size
            float lx = Mathf.Max(m_startSelectPoint.X, currentMousePosition.x);
            float ly = Mathf.Max(m_startSelectPoint.Y, currentMousePosition.y);
            float sx = Mathf.Min(m_startSelectPoint.X, currentMousePosition.x);
            float sy = Mathf.Min(m_startSelectPoint.Y, currentMousePosition.y);
            m_selectionRect = new Rect(sx, sy, lx - sx, ly - sy); //set the new values to the selection rect
        }
    }
}