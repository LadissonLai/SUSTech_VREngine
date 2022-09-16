// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     The UIDrawer Node opens, closes or toggles a target UIDrawer (identified by name) and jumps instantly to the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.UIDrawerNode_CreateNodeMenu_Name, MenuUtils.UIDrawerNode_CreateNodeMenu_Order)]
    public class UIDrawerNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoDrawerName; } }
        public bool ErrorNoDrawerName;
#endif

        public enum DrawerAction
        {
            Open,
            Close,
            Toggle
        }

        public string DrawerName = UIDrawer.DefaultDrawerName;
        public bool CustomDrawerName;
        public DrawerAction Action = DrawerAction.Open;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.UIDrawerNodeName);
            SetAllowDuplicateNodeName(true);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var node = (UIDrawerNode) original;
            DrawerName = node.DrawerName;
            CustomDrawerName = node.CustomDrawerName;
            Action = node.Action;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            ExecuteActions();
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        private void ExecuteActions()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (Action)
            {
                case DrawerAction.Open:
                    UIDrawer.Open(DrawerName, DebugMode);
                    break;
                case DrawerAction.Close:
                    UIDrawer.Close(DrawerName, DebugMode);
                    break;
                case DrawerAction.Toggle:
                    UIDrawer.Toggle(DrawerName, DebugMode);
                    break;
            }
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNoDrawerName = string.IsNullOrEmpty(DrawerName.Trim());
#endif
        }
    }
}