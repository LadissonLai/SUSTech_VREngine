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
    ///     Exits play mode (if in editor) or quits the application if in build mode
    ///     <para/>
    ///     It does that by sending the system Game Event ‘ApplicationQuit‘
    /// </summary>
    [NodeMenu(MenuUtils.ApplicationQuitNode_CreateNodeMenu_Name, MenuUtils.ApplicationQuitNode_CreateNodeMenu_Order)]
    public class ApplicationQuitNode : Node
    {
        private const float NODE_WIDTH = 180f;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.ApplicationQuit);
            SetWidth(NODE_WIDTH);
        }

        public override float GetDefaultNodeWidth() { return NODE_WIDTH; }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            GameEventMessage.SendEvent(SystemGameEvent.ApplicationQuit);
        }
    }
}