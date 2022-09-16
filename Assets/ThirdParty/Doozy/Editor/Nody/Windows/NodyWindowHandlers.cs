// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Nodes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private const float DOUBLE_CLICK_INTERVAL = 0.2f;
        private double m_lastClickTime;

        private bool RegisteredDoubleClick
        {
            get
            {
                bool doubleClick = EditorApplication.timeSinceStartup - m_lastClickTime < DOUBLE_CLICK_INTERVAL; //check if this click happened in the double click interval
                m_lastClickTime = EditorApplication.timeSinceStartup;                                            //update the last click time
                return doubleClick;                                                                              //return TRUE if a double click has been registered
            }
        }

        private bool m_spaceKeyDown;
        private bool m_recalculateAllPointRects;
        private Socket m_activeSocket;

        private Node m_previousHoveredNode;
        private Node m_currentHoveredNode;

        private Socket m_previousHoveredSocket;
        private Socket m_currentHoveredSocket;

//#pragma warning disable 0414 //The private field is assigned but its value is never used
//        private VirtualPoint m_previousHoveredVirtualPoint;
//#pragma warning restore 0414

        private VirtualPoint m_currentHoveredVirtualPoint;

        public void RecalculateAllPointRects() { m_recalculateAllPointRects = true; }

        private void HandleOnGraphEvent(GraphEvent graphEvent)
        {
//            DDebug.Log("GraphEvent: " + graphEvent.eventType);

            switch (graphEvent.eventType)
            {
                case GraphEvent.EventType.EVENT_NONE:             break;
                case GraphEvent.EventType.EVENT_CONNECTING_BEGIN: break;
                case GraphEvent.EventType.EVENT_CONNECTING_END:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.EventType.EVENT_CONNECTION_ESTABLISHED:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.EventType.EVENT_NODE_CREATED:
                    ConstructGraphGUI();
                    RecalculateAllPointRects();
                    break;
                case GraphEvent.EventType.EVENT_NODE_DELETED:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.EventType.EVENT_NODE_UPDATED:
                    ConstructGraphGUI();
                    RecalculateAllPointRects();
                    break;
                case GraphEvent.EventType.EVENT_NODE_CLICKED: break;
                case GraphEvent.EventType.EVENT_SOCKET_CREATED:
                    ConstructGraphGUI();
                    RecalculateAllPointRects();
                    break;
                case GraphEvent.EventType.EVENT_SOCKET_ADDED:   break;
                case GraphEvent.EventType.EVENT_SOCKET_REMOVED: break;
                case GraphEvent.EventType.EVENT_SOCKET_CLEARED_CONNECTIONS:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.EventType.EVENT_CONNECTION_CREATED: break;
                case GraphEvent.EventType.EVENT_CONNECTION_TAPPED:  break;
                case GraphEvent.EventType.EVENT_CONNECTION_DELETED:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.EventType.EVENT_RECORD_UNDO:  break;
                case GraphEvent.EventType.EVENT_SAVED_ASSETS: break;

                case GraphEvent.EventType.EVENT_NODE_DISCONNECTED:
                    ConstructGraphGUI();
                    break;

                case GraphEvent.EventType.EVENT_GRAPH_OPENED:
                    ConstructGraphGUI();
                    RecalculateAllPointRects();
                    break;
                case GraphEvent.EventType.EVENT_UNDO_REDO_PERFORMED:
                    RecalculateAllPointRects();
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            switch (graphEvent.commandType)
            {
                case GraphEvent.CommandType.NONE: break;
                case GraphEvent.CommandType.CONSTRUCT_GRAPH:
                    ConstructGraphGUI();
                    break;
                case GraphEvent.CommandType.RECALCULATE_ALL_POINTS:
                    RecalculateAllPointRects();
                    break;
                case GraphEvent.CommandType.DISCONNECT_SOCKET:
                    DisconnectSocket(graphEvent.sourceSocket, true);
                    break;
            }

            Repaint();
        }

        private void HandleMouseHover()
        {
            //if the graph is in Select, Drag or Pan mode -> there is no need to detect hover events as this operation is irrelevant at this point (this makes the graph a bit more efficient)
            switch (m_mode)
            {
                case GraphMode.Select:
                case GraphMode.Drag:
                case GraphMode.Pan:
                    m_currentHoveredNode = null;
                    m_currentHoveredSocket = null;
                    m_currentHoveredVirtualPoint = null;
                    return;
                case GraphMode.None:    break;
                case GraphMode.Connect: break;
                case GraphMode.Delete:  break;
            }


            m_currentHoveredNode = null;
            m_currentHoveredSocket = null;
            m_currentHoveredVirtualPoint = null;

            m_currentHoveredVirtualPoint = GetVirtualPointAtWorldPosition(CurrentMousePosition);

            if (m_currentHoveredVirtualPoint == null)
            {
                m_currentHoveredSocket = GetSocketAtWorldPositionFromHoverRect(CurrentMousePosition);

                if (m_currentHoveredSocket == null)
                    m_currentHoveredNode = GetNodeAtWorldPosition(CurrentMousePosition);
            }

            if (m_currentHoveredSocket != null)
            {
                //show hover over a socket only if not in delete mode OR if in delete mode AND the socket can be deleted
                if (!m_altKeyPressed || m_altKeyPressed && NodesDatabase[m_currentHoveredSocket.NodeId].CanDeleteSocket(m_currentHoveredSocket))
                    m_currentHoveredSocket.ShowHover.target = true;

                Repaint();
            }

            if (m_previousHoveredSocket != null &&
                m_previousHoveredSocket != m_currentHoveredSocket)
                m_previousHoveredSocket.ShowHover.target = false;


            if (m_currentHoveredNode != null &&
                (GetNodeHeaderGridRect(m_currentHoveredNode).Contains(CurrentMousePosition) ||
                 GetNodeFooterGridRect(m_currentHoveredNode).Contains(CurrentMousePosition)))
            {
                m_currentHoveredNode.IsHovered.target = true;
                Repaint();
            }

            if (m_previousHoveredNode != null &&
                m_previousHoveredNode != m_currentHoveredNode)
                m_previousHoveredNode.IsHovered.target = false;

            m_previousHoveredNode = m_currentHoveredNode;
            m_previousHoveredSocket = m_currentHoveredSocket;
//            m_previousHoveredVirtualPoint = m_currentHoveredVirtualPoint;

//            if (m_currentHoveredSocket != null) DDebug.Log("m_currentHoveredSocket: " + m_currentHoveredSocket.Id);
        }

        private void HandleMouseLeftClicks()
        {
            Event current = Event.current;
            if (!current.isMouse) return;
            if (current.button != 0) return;

            if (current.type == EventType.MouseDown)
            {
                //left mouse button is down and the space key is down as well -> enter panning mode
                if (m_spaceKeyDown)
                {
                    m_mode = GraphMode.Pan;
                    current.Use();
                    return;
                }

                if (m_altKeyPressed) //delete mode
                {
                    BaseNodeGUI nodeGUI = GetNodeGUIOfDeleteButtonAtWorldPosition(CurrentMousePosition);
                    if (nodeGUI != null)
                    {
                        SoftDeleteNode(nodeGUI.Node, true, false);
                        current.Use();
                        return;
                    }
                }

                //pressed left mouse button over a socket point -> but we have at least two nodes selected -> do not allow starting any connections
                if (WindowSettings.SelectedNodes.Count < 2)
                {
                    if (m_currentHoveredVirtualPoint != null)
                    {
//                        if (current.alt)
                        if (m_altKeyPressed)
                        {
                            //pressed left mouse button over a socket virtual point while holding down Alt -> Disconnect Virtual Point
                            DisconnectVirtualPoint(m_currentHoveredVirtualPoint, true);
                        }
                        else
                        {
                            //pressed left mouse button over a socket connection point -> it's a possible start of a connection
                            m_activeSocket = m_currentHoveredVirtualPoint.Socket; //set the socket as the active socket
                            m_mode = GraphMode.Connect;                           //set the graph in connection mode
                        }

                        current.Use();
                        return;
                    }

                    Socket socket = GetSocketAtWorldPositionFromHoverRect(CurrentMousePosition);
                    if (socket != null)
                    {
//                        if (current.alt)
                        if (m_altKeyPressed)
                        {
                            //pressed left mouse button over a socket while holding down Alt -> Remove Socket
                            RemoveSocket(socket, true);
                        }
                        else
                        {
                            //pressed left mouse button over a socket -> it's a possible start of a connection
                            m_activeSocket = socket;    //set the socket as the active socket
                            m_mode = GraphMode.Connect; //set the graph in connection mode
                        }

                        current.Use();
                        return;
                    }
                }

                //pressed left mouse button over a node -> check to see if it's inside the header (if no node is currently selected) or it just over a node (if at least 2 nodes are selected)
                if (m_currentHoveredNode != null)
                {
                    if (GetNodeGridRect(m_currentHoveredNode).Contains(CurrentMousePosition) || //if mouse is inside node -> allow dragging
                        WindowSettings.SelectedNodes.Count > 1)                                 //OR if there are at least 2 nodes selected -> allow dragging from any point on the node
                    {
                        //pressed left mouse button over a node -> select/deselect it
                        if (current.shift || current.control || current.command)               //if using modifiers -> create custom selection
                            SelectNodes(new List<Node> {m_currentHoveredNode}, true, true);    //add/remove the node to/from selection
                        else if (!WindowSettings.SelectedNodes.Contains(m_currentHoveredNode)) //we may have a selection and we do not want to override it in order to be able to start dragging
                            SelectNodes(new List<Node> {m_currentHoveredNode}, false, true);   //select this node only

                        //allow dragging ONLY IF the mouse is over a selected node
                        //in the previous lines we only checked if it's over a node, but not if the node we are hovering over is currently selected
                        if (WindowSettings.SelectedNodes.Contains(m_currentHoveredNode))
                        {
                            if (Selection.activeObject != m_currentHoveredNode)
                                Selection.activeObject = m_currentHoveredNode; //select the node

                            //pressed left mouse button over a node -> it's a possible start drag
                            PrepareToDragSelectedNodes(CurrentMousePosition);
                            m_mode = GraphMode.Drag;
                        }
                    }

                    current.Use();
                    return;
                }

                //pressed left mouse button over nothing -> it's a possible start selection
                PrepareToCreateSelectionBox(CurrentMousePosition);
                current.Use();
                return;
            }

            if (current.type == EventType.MouseDrag)
            {
                //left mouse click is dragging and the graph is in panning mode
                if (m_mode == GraphMode.Pan)
                {
                    //check that the space key is held down -> otherwise exit pan mode
                    if (!m_spaceKeyDown)
                        m_mode = GraphMode.None;
                    else
                        DoPanning(current);

                    current.Use();
                    return;
                }

                //mouse left click is dragging a connection
                if (m_mode == GraphMode.Connect)
                {
                    if (m_currentHoveredSocket != null) //mouse is over a socket -> color the line to green if connection is possible or red otherwise
                        m_createConnectionLineColor = m_activeSocket.CanConnect(m_currentHoveredSocket) ? Color.green : Color.red;
                    else if (m_currentHoveredVirtualPoint != null) //mouse is over a socket connection point -> color the line to green if connection is possible or red otherwise
                        m_createConnectionLineColor = m_activeSocket.CanConnect(m_currentHoveredVirtualPoint.Socket) ? Color.green : Color.red;
                    else //mouse is not over anything connectable -> show the connection point color to look for
                        m_createConnectionLineColor =
                            m_activeSocket.IsInput
                                ? DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal //source socket is input -> looking for an output socket -> color the line to the output color
                                : DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal; //source socket is output -> looking for an input socket -> color the line to the input color

                    current.Use();
                    return;
                }

                //mouse left click is dragging one or more nodes
                if (m_mode == GraphMode.Drag) // && GUIUtility.hotControl == dragNodesControlId)
                {
                    m_isDraggingAnimBool.target = false;
                    RecordUndo("Move Nodes");
                    UpdateSelectedNodesWhileDragging();
                    current.Use();
                    return;
                }

                //mouse left click is dragging and creating a selection box <- we know this because the the mouse is not over a point nor a node
                if (m_startSelectPoint != null) m_mode = GraphMode.Select;
                if (m_mode == GraphMode.Select)
                {
                    UpdateSelectionBox(CurrentMousePosition);
                    UpdateSelectBoxSelectedNodesWhileSelecting(current);
                    UpdateNodesSelectedState(m_selectedNodesWhileSelecting);
                    current.Use();
                    return;
                }
            }

            if (current.type == EventType.MouseUp)
            {
                if (RegisteredDoubleClick)
                    if (m_previousHoveredNode != null)
                        NodesGUIsDatabase[m_previousHoveredNode.Id].OnDoubleClick(this);

                //lifted left mouse button and was panning (space key was/is down) -> reset graph to idle
                if (m_mode == GraphMode.Pan)
                {
                    m_mode = GraphMode.None;
                    current.Use();
                    return;
                }

                //lifted left mouse button and was dragging -> reset graph to idle
                if (m_mode == GraphMode.Drag)
                {
                    m_initialDragNodePositions.Clear();
                    m_isDraggingAnimBool.target = true;
                    m_mode = GraphMode.None;
                    current.Use();
                    return;
                }

                //lifted left mouse button and was selecting via selection box -> end selections and reset graph to idle mode
                if (m_mode == GraphMode.Select)
                {
                    EndDragSelectedNodes();
                    m_mode = GraphMode.None;
                    current.Use();
                    return;
                }


                //check if this happened over another socket or connection point
                if (m_currentHoveredSocket != null)
                {
                    //lifted left mouse button over a socket
                    if (m_activeSocket != null &&                                   //if there is an active socket
                        m_activeSocket != m_currentHoveredSocket &&                 //and it's not this one
                        m_activeSocket.CanConnect(m_currentHoveredSocket))          //and the two sockets can get connected
                        ConnectSockets(m_activeSocket, m_currentHoveredSocket);     //connect the two sockets
                    else                                                            //this was a failed connection attempt
                        GraphEvent.Send(GraphEvent.EventType.EVENT_CONNECTING_END); //send a graph event

                    m_activeSocket = null;   //clear the active socket
                    m_mode = GraphMode.None; //set the graph in idle mode
                    current.Use();
                    return;
                }

                if (m_currentHoveredVirtualPoint != null)
                {
                    //lifted left mouse button over a socket connection point
                    if (m_activeSocket != null &&                                       //if there is an active socket
                        m_activeSocket != m_currentHoveredVirtualPoint.Socket &&        //and it's not this one
                        m_activeSocket.CanConnect(m_currentHoveredVirtualPoint.Socket)) //and the two sockets can get connected
                    {
                        ConnectSockets(m_activeSocket, m_currentHoveredVirtualPoint.Socket); //connect the two sockets
                    }
                    else                                                            //this was a failed connection attempt
                        GraphEvent.Send(GraphEvent.EventType.EVENT_CONNECTING_END); //send a graph event

                    m_activeSocket = null;   //clear the active socket
                    m_mode = GraphMode.None; //set the graph in idle mode
                    current.Use();
                    return;
                }

                //it a connecting process was under way -> clear it
                //lifted left mouse button, but no virtual point was under the mouse position
                if (m_mode == GraphMode.Connect)
                {
                    m_activeSocket = null;   //clear the active socket
                    m_mode = GraphMode.None; //set the graph in idle mode
                }

                Node node = GetNodeAtWorldPosition(CurrentMousePosition);
                if (node != null)
                {
                    m_mode = GraphMode.None; //set the graph in idle mode
                    current.Use();
                    return;
                }

                //lifted mouse left button over nothing -> deselect all and select the graph itself
                ExecuteGraphAction(GraphAction.DeselectAll); //deselect all nodes and select the graph itself
                m_mode = GraphMode.None;                     //set the graph in idle mode
                current.Use();
                return;
            }

            //check if the developer released the left mouse button outside of the graph window
            if (current.rawType == EventType.MouseUp || current.rawType == EventType.MouseLeaveWindow)
                switch (m_mode)
                {
                    case GraphMode.Select:
                        EndDragSelectedNodes();
                        m_mode = GraphMode.None;
                        current.Use();
                        break;
                }
        }

        private void HandleMouseRightClicks()
        {
            Event current = Event.current;
            if (!current.isMouse) return;
            if (current.type != EventType.MouseUp) return;
            if (current.button != 1) return;


            if (m_currentHoveredVirtualPoint != null)
            {
                ShowConnectionPointContextMenu(m_currentHoveredVirtualPoint);
                current.Use();
                return;
            }


            if (m_currentHoveredSocket != null)
            {
                ShowSocketContextMenu(m_currentHoveredSocket);
                current.Use();
                return;
            }

            Node node = GetNodeAtWorldPosition(CurrentMousePosition);
            if (node != null)
            {
                //right click performed over a node
                //if the node has not been added to the ActiveSelection -> deselect all the selected nodes and select this one
                if (!WindowSettings.SelectedNodes.Contains(node)) SelectNodes(new List<Node> {node}, false, true);
                ShowNodeContextMenu(node);
                current.Use();
                return;
            }

            ShowGraphContextMenu();
            current.Use();
        }

        private void HandleMouseMiddleClicks()
        {
            Event current = Event.current;
            if (current.button != 2) return;

            switch (current.type)
            {
                case EventType.MouseDown:
                    m_mode = GraphMode.Pan;
                    break;
                case EventType.MouseUp:
                    m_mode = GraphMode.None;
                    break;
            }

            DoPanning(current);
        }

        private void HandlePanning()
        {
            Event current = Event.current;

            if (current.type == EventType.KeyDown && current.keyCode == KeyCode.Space)
            {
                m_spaceKeyDown = true;
                EditorGUIUtility.SetWantsMouseJumping(1);
            }

            if (current.rawType == EventType.KeyUp && current.keyCode == KeyCode.Space)
            {
                m_spaceKeyDown = false;
                EditorGUIUtility.SetWantsMouseJumping(0);
                if (m_mode == GraphMode.Pan) m_mode = GraphMode.None;
            }
        }

        private void DoPanning(Event current)
        {
            switch (current.rawType)
            {
                case EventType.MouseDown:
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                    break;

                case EventType.MouseUp:
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseMove:
                case EventType.MouseDrag:
                    CurrentPanOffset += current.delta;

                    current.Use();

                    ConstructGraphGUI(); //because we are updating the panning we need to recalculate all the points and curves
                    break;
            }
        }

        private void HandleKeys()
        {
            if (!HasFocus) return;

            Event e = Event.current;

            //Alt Key down -> hide selections
            if (m_altKeyPressed && e.alt)
                UpdateNodesSelectedState(new List<Node>());
            else if (m_altKeyPressed != e.alt)
                UpdateNodesSelectedState(WindowSettings.SelectedNodes);
            m_altKeyPressed = e.alt && HasFocus;


            if (e.type != EventType.KeyUp) return;


            switch (e.keyCode)
            {
                case KeyCode.N: //Create new UINode
                    ExecuteGraphAction(GraphAction.CreateNode, typeof(UINode).AssemblyQualifiedName);
                    break;

                case KeyCode.Escape: //Cancel (Escape)
                    switch (m_mode)
                    {
                        case GraphMode.None:
                            ExecuteGraphAction(GraphAction.DeselectAll);
                            break;
                        case GraphMode.Connect:
                            m_activeSocket = null;
                            break;
                        case GraphMode.Select:
                            m_mode = GraphMode.None;
                            break;
                        case GraphMode.Drag:
                            ExecuteGraphAction(GraphAction.DeselectAll);
                            break;
                    }

                    m_mode = GraphMode.None;
                    break;

                case KeyCode.C: //Copy
                    if (e.control) ExecuteGraphAction(GraphAction.Copy);
                    break;

                case KeyCode.V: //Paste
                    if (e.control) ExecuteGraphAction(GraphAction.Paste);
                    break;

                case KeyCode.Delete: //Delete
                    ExecuteGraphAction(GraphAction.DeleteNodes);
                    break;

                case KeyCode.F: //Center Selected
                    if (!CenterSelectedNodesInWindow()) CenterAllNodesInWindow();
                    break;

                case KeyCode.A: //Select All
                    if (e.control || e.command) ExecuteGraphAction(GraphAction.SelectAll);
                    break;

                case KeyCode.S: //Select start node
                    if (e.modifiers == EventModifiers.None)
                        GoToStartOrEnterNode();
                    break;
            }
        }

        private void HandleZoom()
        {
            //get the current event
            Event current = Event.current;
            //check that the developer is scrolling the mouse wheel
            if (current.type != EventType.ScrollWheel) return;
            //get the current mouse position
            Vector2 mousePosition = current.mousePosition;

            //zoom out
            if (current.delta.y > 0 && CurrentZoom > NodyWindowSettings.ZOOM_MIN)
            {
                CurrentZoom -= NodyWindowSettings.ZOOM_STEP;

                //perform pan to zoom out from mouse position
                CurrentPanOffset = new Vector2(CurrentPanOffset.x + (mousePosition.x - position.width / 2 + WorldToGridPosition(mousePosition).x) * NodyWindowSettings.ZOOM_STEP,
                                               CurrentPanOffset.y + (mousePosition.y - position.height / 2 + WorldToGridPosition(mousePosition).y) * NodyWindowSettings.ZOOM_STEP);
//                m_panOffsetX += (mousePosition.x - position.width / 2 + WorldToGridPosition(mousePosition).x) * NodySettings.Instance.CurrentZoomStep;
//                m_panOffsetY += (mousePosition.y - position.height / 2 + WorldToGridPosition(mousePosition).y) * NodySettings.Instance.CurrentZoomStep;
            }

            //zoom in
            if (current.delta.y < 0 && CurrentZoom < NodyWindowSettings.ZOOM_MAX)
            {
                CurrentZoom += NodyWindowSettings.ZOOM_STEP;

                //perform pan to zoom in to mouse position
                CurrentPanOffset = new Vector2(CurrentPanOffset.x - (mousePosition.x - position.width / 2 + WorldToGridPosition(mousePosition).x) * NodyWindowSettings.ZOOM_STEP,
                                               CurrentPanOffset.y - (mousePosition.y - position.height / 2 + WorldToGridPosition(mousePosition).y) * NodyWindowSettings.ZOOM_STEP);
//                m_panOffsetX -= (mousePosition.x - position.width / 2 + WorldToGridPosition(mousePosition).x) * NodySettings.Instance.CurrentZoomStep;
//                m_panOffsetY -= (mousePosition.y - position.height / 2 + WorldToGridPosition(mousePosition).y) * NodySettings.Instance.CurrentZoomStep;
            }

            //clamp the zoom value between min and max zoom targets
            CurrentZoom = Mathf.Clamp(CurrentZoom, NodyWindowSettings.ZOOM_MIN, NodyWindowSettings.ZOOM_MAX);
            //set the max number of decimals
            CurrentZoom = (float) Math.Round(CurrentZoom, 2);
            //use this event
            current.Use();
            //recalculate all the points and curves to reflect the new values/positions
            ConstructGraphGUI();
        }
    }
}