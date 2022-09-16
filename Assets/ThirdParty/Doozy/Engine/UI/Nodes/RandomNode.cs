// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Picks a weighted random connection in a memory-efficient way.
    ///     <para />
    ///     It does that by picking a random number between 1 and the sum of all the weights (similar to a raffle).
    ///     <para />
    ///     The result is that the higher weighted connections are selected more often than lower weighted ones.
    /// </summary>
    [NodeMenu(MenuUtils.RandomNode_CreateNodeMenu_Name, MenuUtils.RandomNode_CreateNodeMenu_Order)]
    public class RandomNode : Node
    {
        private readonly List<int> m_selectChances = new List<int>();

        public int MaxChance { get; private set; }
        public int ConnectedOutputSockets { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.RandomNodeName);
            SetAllowDuplicateNodeName(true);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(WeightedConnection), true, false);
            AddOutputSocket(ConnectionMode.Override, typeof(WeightedConnection), true, false);
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            SelectRandomOutputSocket();
        }

        public void UpdateMaxChance()
        {
            MaxChance = 0;
            foreach (Socket socket in OutputSockets)
            {
                if (!socket.IsConnected) continue;
                WeightedConnection connection = WeightedConnection.GetValue(socket);
                if (connection.Weight <= 0) continue;
                MaxChance += connection.Weight;
            }
        }

        public void UpdateConnectedOutputSockets()
        {
            int connectedOutputSockets = 0;
            foreach (Socket outputSocket in OutputSockets)
                if (outputSocket.IsConnected)
                    connectedOutputSockets++;

            if (connectedOutputSockets == ConnectedOutputSockets) return;
            ConnectedOutputSockets = connectedOutputSockets;
            UpdateMaxChance();
        }

        private void SelectRandomOutputSocket()
        {
            m_selectChances.Clear();
            MaxChance = 0;
            foreach (Socket socket in OutputSockets)
            {
                if (!socket.IsConnected) continue;

                WeightedConnection connection = WeightedConnection.GetValue(socket);
                if (connection.Weight <= 0)
                {
                    m_selectChances.Add(-1);
                }
                else
                {
                    MaxChance += connection.Weight;
                    m_selectChances.Add(MaxChance);
                }
            }

            int randomSocketIndex = 0;
            int randomChance = Random.Range(0, MaxChance);
            for (int i = 0; i < m_selectChances.Count; i++)
            {
                if (m_selectChances[i] == -1) continue;
                if (m_selectChances[i] < randomChance) continue;
                randomSocketIndex = i;
                break;
            }

            ActiveGraph.SetActiveNodeByConnection(OutputSockets[randomSocketIndex].FirstConnection);
        }
    }
}