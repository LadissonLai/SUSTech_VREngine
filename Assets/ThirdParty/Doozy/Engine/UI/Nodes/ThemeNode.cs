// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Themes;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Activates a theme Variant (from a set Theme) and jumps instantly to the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.ThemeNode_CreateNodeMenu_Name, MenuUtils.ThemeNode_CreateNodeMenu_Order)]
    public class ThemeNode : Node, ISerializationCallbackReceiver
    {
        public Guid ThemeId = Guid.Empty;
        public Guid VariantId = Guid.Empty;

        [SerializeField]
        private byte[] ThemeIdSerializedGuid;

        [SerializeField]
        private byte[] VariantIdSerializedGuid;

        public virtual void OnBeforeSerialize()
        {
            ThemeIdSerializedGuid = GuidUtils.GuidToSerializedGuid(ThemeId);
            VariantIdSerializedGuid = GuidUtils.GuidToSerializedGuid(VariantId);
        }

        public virtual void OnAfterDeserialize()
        {
            ThemeId = GuidUtils.SerializedGuidToGuid(ThemeIdSerializedGuid);
            VariantId = GuidUtils.SerializedGuidToGuid(VariantIdSerializedGuid);
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.ThemeNode);
            SetAllowDuplicateNodeName(true);
            SetAllowEmptyNodeName(true);
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
            var node = (ThemeNode) original;
            ThemeId = node.ThemeId;
            VariantId = node.VariantId;
            ThemeIdSerializedGuid = node.ThemeIdSerializedGuid;
            VariantIdSerializedGuid = node.VariantIdSerializedGuid;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            ExecuteActions();
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        private void ExecuteActions() { ThemeManager.ActivateVariant(ThemeId, VariantId); }
    }
}