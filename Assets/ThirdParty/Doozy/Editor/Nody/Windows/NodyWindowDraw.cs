// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Extensions;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private string m_currentEventType;
        private Color m_createConnectionLineColor = Color.white;

        /// <summary>
        ///     Draws all the connection points for each socket
        /// </summary>
        private void DrawSocketsConnectionPoints()
        {
            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_CONNECTION_POINTS) return;

            foreach (Socket socket in SocketsDatabase.Values)
            {
                if (!NodesGUIsDatabase[socket.NodeId].IsVisible) continue;      //node is not visible -> do not draw the connection points
                DrawSocketConnectionPoints(socket, m_isDraggingAnimBool.faded); //draw the connection points
            }
        }

        /// <summary>
        ///     Draws all the connection points for the target socket
        /// </summary>
        /// <param name="socket"> Target socket </param>
        /// <param name="showPercentage"> Connection point size ratio (used to be able to hide the point when dragging) </param>
        private void DrawSocketConnectionPoints(Socket socket, float showPercentage)
        {
            //if show percentage is below 20% do not show anything
            //this is used when the graph is in dragging mode -> we hide the connection points 
            if (showPercentage < 0.5f) return;

            foreach (VirtualPoint virtualPoint in PointsDatabase[socket.Id])
            {
                bool mouseIsOverThisPoint = virtualPoint == m_currentHoveredVirtualPoint;
                //if the 'Alt' or 'Option' key is held down - and the virtualPoint is connected -> color it RED to signal the developer that he can remove the connection 
                if (m_altKeyPressedAnimBool.faded > 0.5f && virtualPoint.IsConnected)
                {
                    //set the virtualPoint delete color to RED
                    //set the virtualPoint style to show to the dev that he can disconnect the socket (it's a minus sign)
                    DrawConnectionPoint(virtualPoint, mouseIsOverThisPoint, ConnectionPointMinus, Color.red, showPercentage * m_altKeyPressedAnimBool.faded);
                    continue;
                }

                //init a color variable for the virtualPoint by socket direction
                Color pointColor = socket.IsInput ? DGUI.Colors.GetDColor(DGUI.Colors.NodyInputColorName).Normal : DGUI.Colors.GetDColor(DGUI.Colors.NodyOutputColorName).Normal;

                GUIStyle pointStyle; //set the virtualPoint style (OCCUPIED/EMPTY) and the connection mode (MULTIPLE/OVERRIDE)
                switch (socket.GetConnectionMode())
                {
                    case ConnectionMode.Override:
                        pointStyle = virtualPoint.IsConnected ? ConnectionPointOverrideConnected : ConnectionPointOverrideEmpty;
                        break;
                    case ConnectionMode.Multiple:
                        pointStyle = virtualPoint.IsConnected ? ConnectionPointMultipleConnected : ConnectionPointMultipleEmpty;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }

                DrawConnectionPoint(virtualPoint, mouseIsOverThisPoint, pointStyle, pointColor, showPercentage * (1 - m_altKeyPressedAnimBool.faded));
            }
        }

        private static void DrawConnectionPoint(VirtualPoint virtualPoint, bool mouseIsOverThisPoint, GUIStyle pointStyle, Color pointColor, float showPercentage)
        {
            if (showPercentage < 0.5f) return; //if show is below a magic value -> return 

            pointColor.a = virtualPoint.IsConnected || mouseIsOverThisPoint
                               ? 1f    //if the virtualPoint is occupied or the mouse is currently hovering over it -> fade to 100%
                               : 0.4f; //if the virtualPoint is not occupied -> fade to 40%

            float occupiedRatioChange = virtualPoint.IsConnected ? 1f : 0.8f; //this makes an unoccupied connector a bit smaller than an occupied one (looks nicer)

            float pointWidth = NodySettings.Instance.ConnectionPointWidth * showPercentage * occupiedRatioChange * (mouseIsOverThisPoint ? 1.2f : 1f);
            float pointHeight = NodySettings.Instance.ConnectionPointHeight * showPercentage * occupiedRatioChange * (mouseIsOverThisPoint ? 1.2f : 1f);

            var pointRect = new Rect(virtualPoint.Rect.position.x - pointWidth / 2,
                                     virtualPoint.Rect.position.y - pointHeight / 2,
                                     pointWidth,
                                     pointHeight);

            GUI.color = pointColor;
            GUI.Box(pointRect, GUIContent.none, pointStyle);
            GUI.color = Color.white;
        }

        private void DrawNodes(Rect graphArea)
        {
            if (NodesGUIsDatabase == null || NodesGUIsDatabase.Keys.Count == 0) return; //sanity check

            BeginWindows();

            foreach (string key in NodesGUIsDatabase.Keys)
            {
                BaseNodeGUI nodeGUI = NodesGUIsDatabase[key];                                                           //get the nodeGUI
                var nodeGUIWorldRect = new Rect(nodeGUI.Node.GetPosition() + CurrentPanOffset / CurrentZoom, nodeGUI.Node.GetSize()); //calculate the world rect of the nodeGUI
                nodeGUI.IsVisible = m_scaledGraphArea.Overlaps(nodeGUIWorldRect);                                       //check the the world rect is in view

                if (!nodeGUI.IsVisible) continue; //if nodeGUI node in view -> do not draw it
                //if the graph is too zoomed out it's pointless to process and draw the node contents as it is almost invisible and no relevant info can be read on the screen
                nodeGUI.ZoomedBeyondSocketDrawThreshold = CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_SOCKETS; //let the NodeGUI know if it should draw its contents (we do this to optimize the viewing speed of the graph when there are a lot of nodes visible)
                nodeGUI.DrawNodeGUI(graphArea, CurrentPanOffset, CurrentZoom);                                                  //draw the node
            }

            EndWindows();

            if (!m_altKeyPressed) return;                                                //if not in delete mode (Alt is not pressed) -> do not draw the delete buttons
            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_CONNECTION_POINTS) return; //if the graph is too zoomed out, beyond the connection points draw threshold -> do not draw the delete buttons
            foreach (string key in NodesGUIsDatabase.Keys)
            {
                BaseNodeGUI nodeGUI = NodesGUIsDatabase[key]; //get the nodeGUI
                if (!nodeGUI.IsVisible) continue;             //if nodeGUI node in view -> do not draw it
                DrawNodeDeleteButton(nodeGUI);                //draw the delete button
            }
        }

        private void DrawNodeDeleteButton(BaseNodeGUI nodeGUI)
        {
            if (!nodeGUI.Node.CanBeDeleted) return;
            switch (nodeGUI.Node.NodeType)
            {
                case NodeType.Start:
                case NodeType.Enter:
                case NodeType.Exit:
                    return;
            }

            nodeGUI.SetDeleteButtonSizeScale(m_altKeyPressedAnimBool.faded * m_isDraggingAnimBool.faded);

            Vector2 gridPosition = nodeGUI.DeleteButtonRect.position + CurrentPanOffset / CurrentZoom;
            nodeGUI.DeleteButtonRectInGridSpace = new Rect(gridPosition, nodeGUI.DeleteButtonRect.size);

            GUI.color = Color.red;
            GUI.Box(nodeGUI.DeleteButtonRectInGridSpace, GUIContent.none, NodeButtonDelete);
            GUI.color = Color.white;
        }

        private void DrawConnections()
        {
            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_CONNECTIONS) return;

            if (NodesGUIsDatabase == null) return;

            foreach (VirtualConnection virtualConnection in ConnectionsDatabase.Values)
            {
                if (virtualConnection.OutputNode == null ||
                    virtualConnection.InputNode == null) continue;

                if (!NodesGUIsDatabase[virtualConnection.OutputNode.Id].IsVisible &&
                    !NodesGUIsDatabase[virtualConnection.InputNode.Id].IsVisible) continue; //if both nodes are not visible -> do not draw the connection

                if (virtualConnection.OutputVirtualPoint == null ||
                    virtualConnection.InputVirtualPoint == null)
                    continue;

                DrawConnectionCurve(virtualConnection);
            }
        }

        private void DrawSelectionBox()
        {
            if (m_mode != GraphMode.Select) return;
            Color initialColor = GUI.color;
            GUI.color = (EditorGUIUtility.isProSkin ? Color.black : Color.white).WithAlpha(0.3f);
            GUI.Label(m_selectionRect, string.Empty, Editor.Styles.GetStyle(Editor.Styles.StyleName.BackgroundSquare));
            GUI.color = initialColor;
        }

        // ReSharper disable once UnusedMember.Local
        private void DrawWindowInfo()
        {
            float offsetFromBottom = 40;
            GUI.Label(new Rect(8, position.height - offsetFromBottom - 100, 400, 20), "WorldToGridPosition: " + WorldToGridPosition(Event.current.mousePosition));
            GUI.Label(new Rect(8, position.height - offsetFromBottom - 80, 400, 20), "PanOffset: " + CurrentPanOffset);
            GUI.Label(new Rect(8, position.height - offsetFromBottom - 60, 400, 20), "Zoom: " + CurrentZoom);
            GUI.Label(new Rect(8, position.height - offsetFromBottom - 40, 400, 20), "w: " + position.width + " h: " + position.height);                              //Nody Window Rect Info
            GUI.Label(new Rect(8, position.height - offsetFromBottom - 20, 400, 20), "x: " + Event.current.mousePosition.x + " y: " + Event.current.mousePosition.y); //Current Mouse Position
            Repaint();
        }

        // ReSharper disable once UnusedMember.Local
        private void DrawGraphInfo()
        {
            GUI.Label(new Rect(8, 8, 200, 20), "Graph SocketName: " + CurrentGraph.name);       //Graph SocketName
            GUI.Label(new Rect(8, 28, 200, 20), "Last Modified: " + CurrentGraph.LastModified); //Graph Last Modified Time
            GUI.Label(new Rect(8, 48, 200, 20), "Version: " + CurrentGraph.Version);            //Graph Script Version
            GUI.Label(new Rect(8, 68, 200, 20), "GraphMode: " + m_mode);                        //GraphMode
            if (Event.current.type != EventType.Repaint &&
                Event.current.type != EventType.Layout)
                m_currentEventType = Event.current.type.ToString();

            GUI.Label(new Rect(8, 88, 200, 20), "EventType: " + m_currentEventType); //GraphMode
        }

        private void DrawLineFromSocketToPosition(Socket activeSocket, Vector2 worldPosition)
        {
            if (m_mode != GraphMode.Connect) return;

            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_SOCKETS) return;

            Vector2 from = GetClosestConnectionPointWorldPositionFromSocketToMousePosition(activeSocket, worldPosition / CurrentZoom); //this works as the positions have been converted to world space from node space
            Vector2 to = worldPosition / CurrentZoom;

            float connectionLineWidth = NodySettings.Instance.CurveWidth;
            var connectionBackgroundColor = new Color(m_createConnectionLineColor.r * 0.2f,
                                                      m_createConnectionLineColor.g * 0.2f,
                                                      m_createConnectionLineColor.b * 0.2f,
                                                      0.8f);
            Handles.DrawBezier(from, to, to, from, connectionBackgroundColor, null, connectionLineWidth + 2);
            Handles.DrawBezier(from, to, to, from, m_createConnectionLineColor, null, connectionLineWidth);

            float dotSize = connectionLineWidth * 3;
            GUI.color = new Color(m_createConnectionLineColor.r * 1.2f, m_createConnectionLineColor.g * 1.2f, m_createConnectionLineColor.b * 1.2f, 1f);
            GUI.Box(new Rect(to.x - dotSize / 2, to.y - dotSize / 2, dotSize, dotSize), "", Dot);
            GUI.color = Color.white;
            HandleUtility.Repaint();
            Repaint();
        }
    }
}