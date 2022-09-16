// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Nody.Models;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private enum GraphMode
        {
            None,
            Connect,
            Select,
            Drag,
            Pan,
            Delete
        }

        private AnimBool m_connectMode,
                         m_selectMode,
                         m_dragMode,
                         m_panMode,
                         m_deleteMode;


        public static Graph CurrentGraph { get { return WindowSettings.CurrentGraph; } private set { WindowSettings.CurrentGraph = value; } }

        [SerializeField] private string m_graphAssetPath = "";
//        [SerializeField] private string m_graphAssetName = "";

        public static string GetGraphName(Graph graph, bool addAsteriskIfDirty = true) { return graph.name + (addAsteriskIfDirty && graph.IsDirty ? "*" : ""); }

        private GraphMode m_mode = GraphMode.None;

        private Rect m_graphAreaIncludingTab;
        private Rect m_scaledGraphArea;

        internal bool m_altKeyPressed;
        private AnimBool m_altKeyPressedAnimBool;

        private void SetGraphMode(GraphMode mode) { m_mode = mode; }

        public void ConstructGraphGUI()
        {
//            DDebug.Log("ConstructGraphGUI");

            if (CurrentGraph == null) return; //no graph is loaded -> stop here

            //null all databases in order to be able to regenerate them
            InvalidateDatabases();

            //Construct the Nodes Dictionary - all the nodes
            m_nodesDatabase = NodesDatabase;

            //Construct the Sockets Dictionary - all the sockets
            m_socketsDatabase = SocketsDatabase;

            //Construct the Points Dictionary - this is a special dictionary as it is made out of VirtualPoints
            //a VirtualPoint contains a reference to its parent socket and its parent node
            //also it has a Rect that is converted to WorldSpace - the sockets connection points vector2 values are in NodeSpace - we needed them in WorldSpace and to be able to do things with them -> this is the solution
            m_pointsDatabase = PointsDatabase;

            //Construct the Connections Dictionary - this is a special dictionary as it is made out of VirtualConnections
            //a VirtualConnection contains a reference to both an OutputSocket and an InputSocket
            //remember when a Connection is created it is copied both on the OutputSocket as well as on the InputSocket
            //this is why we need this dual reference in order to speed up the search later on
            //we just search by socket id and use it a s key for the dictionary -> thus it will be a super fast search
            m_connectionsDatabase = ConnectionsDatabase;

            //Construct the actual node GUI editors references database
            m_nodesGUIsDatabase = NodesGUIsDatabase;

            //calculate all the connection points initial values
            CalculateAllPointRects();

            //calculate all the connections initial values
            //we do this here because in normal operation we want to update only the connections that are referencing nodes that are being dragged
            CalculateAllConnectionCurves();

            //update the visual state of all the connection points
            UpdateVirtualPointsIsOccupiedStates();

            //check for errors
            CheckAllNodesForErrors();
        }

        private void RemoveSoftDeletedNodesFromGraph()
        {
            foreach (Node deletedNode in WindowSettings.DeletedNodes)
                if (CurrentGraph.Nodes.Contains(deletedNode))
                    CurrentGraph.Nodes.Remove(deletedNode);
        }

        private void UpdateVirtualPointsIsOccupiedStates()
        {
            //mark all virtual points a not connected
            foreach (List<VirtualPoint> points in PointsDatabase.Values)
            foreach (VirtualPoint point in points)
                point.IsConnected = false;

            //go through all the virtual connections and mark their virtual points as connected 
            foreach (VirtualConnection vc in ConnectionsDatabase.Values)
            {
                if (vc == null) continue;
                if (vc.InputVirtualPoint == null) continue;
                if (vc.OutputVirtualPoint == null) continue;

                vc.InputVirtualPoint.IsConnected = true;
                vc.OutputVirtualPoint.IsConnected = true;

                //UPDATE THE ACTUAL CONNECTION POSITIONS IN THE SOCKETS -> we do this here as we are already doing an enumeration and is a bit more efficient no to do this twice
                //this is a very important step, as we update the positions of the input and output connection points in the connections found on the sockets
                foreach (Connection connection in vc.InputSocket.Connections)
                    if (connection.Id == vc.ConnectionId)
                    {
                        connection.InputConnectionPoint = vc.InputVirtualPoint.LocalPointPosition;
                        connection.OutputConnectionPoint = vc.OutputVirtualPoint.LocalPointPosition;
                    }

                foreach (Connection connection in vc.OutputSocket.Connections)
                    if (connection.Id == vc.ConnectionId)
                    {
                        connection.InputConnectionPoint = vc.InputVirtualPoint.LocalPointPosition;
                        connection.OutputConnectionPoint = vc.OutputVirtualPoint.LocalPointPosition;
                    }
            }
        }
    }
}