// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private enum GraphAction
        {
            Copy,
            Connect,
            CreateNode,
            DeleteNodes,
            DeselectAll,
            Disconnect,
            Paste,
            SelectAll,
            SelectNodes
        }

        private void ExecuteGraphAction(GraphAction graphAction, string nodeTypeQualifiedName = "")
        {
            switch (graphAction)
            {
                case GraphAction.Copy:
                    CopyNodes(WindowSettings.SelectedNodes);
                    break;

                case GraphAction.Connect:

                    break;
                case GraphAction.CreateNode:
                    Type nodeType = Type.GetType(nodeTypeQualifiedName, true);
                    CreateNodeInTheOpenedGraph(nodeType, WorldToGridPosition(CurrentMousePosition), true);
                    break;

                case GraphAction.DeleteNodes:
                    SoftDeleteNodes(WindowSettings.SelectedNodes, true, false);
                    break;

                case GraphAction.DeselectAll:
                    DeselectAll(true);
                    break;

                case GraphAction.Disconnect:
                    break;

                case GraphAction.Paste:
                    PasteNodes();
                    break;

                case GraphAction.SelectAll:
                    SelectAll(true);
                    break;

                case GraphAction.SelectNodes:
                    break;

                default: throw new ArgumentOutOfRangeException("graphAction", graphAction, null);
            }
        }
    }
}