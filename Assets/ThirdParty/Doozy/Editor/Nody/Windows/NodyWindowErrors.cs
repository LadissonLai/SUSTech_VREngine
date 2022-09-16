// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Nody.Models;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private void CheckAllNodesForErrors()
        {
            foreach (Node node in NodesDatabase.Values)
            {
                node.CheckForErrors();
                CheckGraphForDuplicateNodeName(node);
            }

            Repaint();
        }

        private void CheckGraphForDuplicateNodeName(Node node)
        {
            if (node.AllowDuplicateNodeName) return;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (node.NodeType)
            {
                case NodeType.Start:
                case NodeType.Enter:
                case NodeType.Exit:
                    return; //do not check duplicate name error for system nodes
            }

            node.ErrorDuplicateNameFoundInGraph = FoundDuplicateNodeName(node);
        }

        private bool FoundDuplicateNodeName(Node node)
        {
            foreach (KeyValuePair<Node, string> keyValuePair in NodeNames)
            {
                if (keyValuePair.Key == node) continue;
                if (keyValuePair.Value.Equals(node.Name, StringComparison.OrdinalIgnoreCase)) return true; //there is another node with the same node name as the target node name -> return true
            }

            return false; //no other node has the target node's name -> return false
        }
    }
}