// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Nody.NodeGUI;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private Vector2 GetClosestConnectionPointWorldPositionFromSocketToMousePosition(Socket socket, Vector2 mousePosition)
        {
            Node parentNode = NodesDatabase[socket.NodeId];
            if (parentNode == null) return Vector2.zero;
            IEnumerable<Vector2> pointsInWorldSpace = GetSocketConnectionPointsInWorldSpace(socket, parentNode);
            float minDistance = 100000;
            Vector2 worldPosition = parentNode.GetPosition();
            foreach (Vector2 connectionPointWorldPosition in pointsInWorldSpace)
            {
                float currentDistance = Vector2.Distance(connectionPointWorldPosition, mousePosition);
                if (currentDistance > minDistance) continue;
                worldPosition = connectionPointWorldPosition;
                minDistance = currentDistance;
            }

            return worldPosition;
        }
        
        private Node GetNodeAtWorldPosition(Vector2 worldPosition)
        {
            foreach (BaseNodeGUI nodeGUI in NodesGUIsDatabase.Values.Reverse())
            {
                if (!nodeGUI.IsVisible) continue; //node not visible -> do not process it
                if (GetNodeGridRect(nodeGUI.Node).Contains(worldPosition)) return nodeGUI.Node;
            }

            return null;
        }

        private Rect GetNodeFooterGridRect(Node node) { return new Rect(GridToWorldPosition(node.GetFooterRect().position), node.GetFooterRect().size * CurrentZoom); }

        private Rect GetNodeGridRect(Node node) { return new Rect(GridToWorldPosition(node.GetRect().position), node.GetRect().size * CurrentZoom); }
        
        private Rect GetNodeHeaderGridRect(Node node) { return new Rect(GridToWorldPosition(node.GetHeaderRect().position), node.GetHeaderRect().size * CurrentZoom); }

        private BaseNodeGUI GetNodeGUIOfDeleteButtonAtWorldPosition(Vector2 mousePosition)
        {
            Vector2 gridMousePosition = mousePosition / CurrentZoom;

            foreach (BaseNodeGUI nodeGUI in NodesGUIsDatabase.Values)
                if (nodeGUI.DeleteButtonRectInGridSpace.Contains(gridMousePosition))
                    return nodeGUI;

            return null;
        }

        private Vector2 GridToWorldPosition(Vector2 gridPosition) { return gridPosition * CurrentZoom + CurrentPanOffset; }

        private VirtualPoint GetVirtualPointAtWorldPosition(Vector2 worldPosition)
        {
            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_CONNECTION_POINTS) return null; //if the graph is too zoomed out do not search for connection points

            //in order to help the developer click on these points, we create a small rect at the mouse position and Overlap with it
            //this technique makes for a better use experience when clicking on small items
            float size = 8;
            var mouseRect = new Rect(worldPosition.x,
                                     worldPosition.y,
                                     size,
                                     size);

            mouseRect.x -= size / 2;
            mouseRect.y -= size / 2;

//            GUI.color = Color.magenta;
//            GUI.Box(mouseRect, "");
//            GUI.color = Color.white;

            foreach (string key in PointsDatabase.Keys)
            foreach (VirtualPoint point in PointsDatabase[key])
            {
                if (!NodesGUIsDatabase[point.Node.Id].IsVisible) continue; //the node, that his point belongs to, is not visible -> do not process it
                var pointGridRect = new Rect(point.Rect.position * CurrentZoom, point.Rect.size * CurrentZoom);
                pointGridRect.x -= pointGridRect.width / 2;
                pointGridRect.y -= pointGridRect.height / 4;

//                GUI.color = Color.yellow;
//                GUI.Box(pointGridRect, "");
//                GUI.color = Color.white;

                if (pointGridRect.Overlaps(mouseRect)) return point;
            }

            return null;
        }

        // ReSharper disable once UnusedMember.Local
        private Socket GetSocketAtWorldPosition(Vector2 worldPosition)
        {
            //because the socket position is in local node space
            //we need to calculate the world rect for said socket
            //this is why we get the parent node and calculate the socketWorldRect
            //after we get the world rect, we need to take into account the PanOffset and the zoom in order to get the accurate socketGridRect
            foreach (Socket socket in SocketsDatabase.Values)
            {
                Node nodeParent = NodesDatabase[socket.NodeId];
                if (nodeParent == null) continue;
                var socketWorldRect = new Rect(nodeParent.GetX(),
                                               nodeParent.GetY() + socket.GetY(),
                                               socket.GetWidth(),
                                               socket.GetHeight());

                var socketGridRect = new Rect(socketWorldRect.position * CurrentZoom + CurrentPanOffset,
                                              socketWorldRect.size * CurrentZoom);

                if (socketGridRect.Contains(worldPosition))
                    return socket;
            }

            return null;
        }

        private Socket GetSocketAtWorldPositionFromHoverRect(Vector2 worldPosition)
        {
            if (CurrentZoom <= NodyWindowSettings.ZOOM_DRAW_THRESHOLD_FOR_SOCKETS) return null; //if the graph is too zoomed out do not search for sockets

            foreach (Socket socket in SocketsDatabase.Values)
            {
                BaseNodeGUI nodeParentGUI = NodesGUIsDatabase[socket.NodeId];
                if (nodeParentGUI == null) continue;
                if (!nodeParentGUI.IsVisible) continue; //the node, that the socket belongs to, is not visible -> do not process it
                var socketWorldRect = new Rect(nodeParentGUI.Node.GetX() + (nodeParentGUI.Node.GetWidth() - socket.HoverRect.width) / 2, //(nodeParent.GetWidth() - socket.HoverRect.width) / 2 -> is the X offset of the hover rect
                                               nodeParentGUI.Node.GetY() + socket.GetY() + (socket.GetHeight() - socket.HoverRect.height) / 2, //(socket.GetHeight() - socket.HoverRect.height) / 2 -> is the Y offset of the hover rect
                                               socket.HoverRect.width,
                                               socket.HoverRect.height);

                var socketGridRect = new Rect(socketWorldRect.position * CurrentZoom + CurrentPanOffset,
                                              socketWorldRect.size * CurrentZoom);

//                GUI.color = Color.yellow;
//                GUI.Box(socketGridRect, GUIContent.none);
//                GUI.color = Color.white;

                if (socketGridRect.Contains(worldPosition))
                    return socket;
            }

            return null;
        }

        private IEnumerable<Vector2> GetSocketConnectionPointsInWorldSpace(Socket socket, Node parentNode)
        {
            var pointsInWorldSpace = new List<Vector2>();
            if (socket == null) return pointsInWorldSpace;
            if (parentNode == null) return pointsInWorldSpace;
            foreach (Vector2 connectionPoint in socket.ConnectionPoints)
            {
                var socketWorldRect = new Rect(parentNode.GetX(),
                                               parentNode.GetY() + socket.GetY(),
                                               socket.GetWidth(),
                                               socket.GetHeight());

                socketWorldRect.position += CurrentPanOffset / CurrentZoom; //this is the calculated socketGridRect

//                GUI.color = Color.magenta;
//                GUI.Box(socketWorldRect, "");
//                GUI.color = Color.white;

                pointsInWorldSpace.Add(new Vector2(socketWorldRect.x + connectionPoint.x + NodySettings.Instance.ConnectionPointWidth / 2,
                                                   socketWorldRect.y + connectionPoint.y + NodySettings.Instance.ConnectionPointHeight / 2));
            }

            return pointsInWorldSpace;
        }

        private Rect GetSocketWorldRect(Socket socket)
        {
            if (socket == null) return new Rect();
            Node parentNode = NodesDatabase[socket.NodeId];
            if (parentNode == null) return new Rect();
            var socketWorldRect = new Rect(parentNode.GetX(),
                                           parentNode.GetY() + socket.GetY(),
                                           socket.GetWidth(),
                                           socket.GetHeight());
            return socketWorldRect;
        }

        private Vector2 WorldToGridPosition(Vector2 worldPosition) { return (worldPosition - CurrentPanOffset) / CurrentZoom; }
    }
}