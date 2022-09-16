// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Models;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private Vector2 m_lastMousePosition = Vector2.zero;
        private Vector2 m_dragNodesDistance = Vector2.zero;
        private readonly Dictionary<Node, Vector2> m_initialDragNodePositions = new Dictionary<Node, Vector2>();

        private void PrepareToDragSelectedNodes(Vector2 mousePosition)
        {
            m_lastMousePosition = mousePosition;
            m_dragNodesDistance = Vector2.zero;
            m_initialDragNodePositions.Clear();
            foreach (Node node in WindowSettings.SelectedNodes) m_initialDragNodePositions.Add(node, node.GetPosition());
        }

        private void EndDragSelectedNodes()
        {
            var selectionBoxNodes = new List<Node>();

            foreach (Node selectedNode in m_selectedNodesWhileSelecting) selectionBoxNodes.Add(selectedNode);

            //if shift key is not pressed, clear current selection
            SelectNodes(selectionBoxNodes, false, true);

            m_startSelectPoint = null;
        }

        private void UpdateSelectedNodesWhileDragging()
        {
            m_dragNodesDistance += CurrentMousePosition - m_lastMousePosition;
            m_lastMousePosition = CurrentMousePosition;

            foreach (Node node in WindowSettings.SelectedNodes)
            {
                Vector2 initialPosition = m_initialDragNodePositions[node];
                Vector2 newPosition = node.GetPosition();
                newPosition.x = initialPosition.x + m_dragNodesDistance.x / CurrentZoom;
                newPosition.y = initialPosition.y + m_dragNodesDistance.y / CurrentZoom;
                node.SetPosition(SnapPositionToGrid(newPosition));
            }

            UpdateVirtualPointsIsOccupiedStates();
        }

        private static Vector2 SnapPositionToGrid(Vector2 position)
        {
            int xCell = Mathf.RoundToInt(position.x / NodyWindowSettings.SNAP_TO_GRID_SIZE);
            int yCell = Mathf.RoundToInt(position.y / NodyWindowSettings.SNAP_TO_GRID_SIZE);
            position.x = xCell * NodyWindowSettings.SNAP_TO_GRID_SIZE;
            position.y = yCell * NodyWindowSettings.SNAP_TO_GRID_SIZE;
            return position;
        }
    }
}