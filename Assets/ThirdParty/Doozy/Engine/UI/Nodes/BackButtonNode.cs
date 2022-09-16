// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Input;
using Doozy.Engine.Utils;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Enables or disables the 'Back' button functionality and jumps instantly to the next node in the Graph.
    ///     <para/>
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.BackButtonNode_CreateNodeMenu_Name, MenuUtils.BackButtonNode_CreateNodeMenu_Order)]
    public class BackButtonNode : Node
    {
        public enum BackButtonState
        {
            Disable,
            Enable,
            EnableByForce
        }

        public BackButtonState BackButtonAction = BackButtonState.Enable;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.BackButtonNodeName);
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
            var node = (BackButtonNode) original;
            BackButtonAction = node.BackButtonAction;
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
            switch (BackButtonAction)
            {
                case BackButtonState.Disable:
                    BackButton.Disable();
                    break;
                case BackButtonState.Enable:
                    BackButton.Enable();
                    break;
                case BackButtonState.EnableByForce:
                    BackButton.EnableByForce();
                    break;
            }
        }
    }
}