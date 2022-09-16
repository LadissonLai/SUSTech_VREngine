// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///    Global node that listens for a set game event (string value). When triggered it jumps instantly to the next node in the Graph.
    ///    A global node is active as long as its parent Graph is active.
    ///    This particular node allows for jumping from one part of the UI flow to another, without the need of a direct connection.
    ///    Due to the way it works, this node can also be considered as a 'virtual connection' between multiple active Graphs.
    /// </summary>
    [NodeMenu(MenuUtils.PortalNode_CreateNodeMenu_Name, MenuUtils.PortalNode_CreateNodeMenu_Order)]
    public class PortalNode : Node
    {
        public enum ListenerType
        {
            GameEvent,
            UIButton,
            UIView,
            UIDrawer
        }

#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNotListeningForAnyGameEvent && ListenFor == ListenerType.GameEvent; } }
        public bool ErrorNotListeningForAnyGameEvent;
#endif

        private const ListenerType DEFAULT_LISTENER_TYPE = ListenerType.GameEvent;
        private const bool DEFAULT_ANY_VALUE = false;
        private const string DEFAULT_GAME_EVENT = "";

        [SerializeField] private string m_gameEvent = DEFAULT_GAME_EVENT;
        public string GameEventToListenFor { get { return m_gameEvent; } }

        [NonSerialized] private Graph m_portalGraph;
        public Graph PortalGraph { get { return m_portalGraph; } set { m_portalGraph = value; } }

        public ListenerType ListenFor = DEFAULT_LISTENER_TYPE;
        public bool AnyValue = DEFAULT_ANY_VALUE;
        public UIViewBehaviorType UIViewTriggerAction = UIViewBehaviorType.Show;
        public string ViewCategory = UIView.DefaultViewCategory;
        public string ViewName = UIView.DefaultViewName;
        public UIButtonBehaviorType UIButtonTriggerAction = UIButtonBehaviorType.OnClick;
        public string ButtonCategory = UIButton.DefaultButtonCategory;
        public string ButtonName = UIButton.DefaultButtonName;
        public UIDrawerBehaviorType UIDrawerTriggerAction = UIDrawerBehaviorType.Open;
        public string DrawerName = UIDrawer.DefaultDrawerName;
        public bool CustomDrawerName = false;
        public bool SwitchBackMode = false;

        private Node m_sourceNode;
        private bool m_activatedByEvent;
        public bool HasSource { get { return m_sourceNode != null; } }
        public Node Source { get { return m_sourceNode; } }

        public string WaitForInfoTitle
        {
            get
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (ListenFor)
                {
                    case ListenerType.GameEvent: return UILabels.GameEvent;
                    case ListenerType.UIView:    return "UIView " + UIViewTriggerAction;
                    case ListenerType.UIButton:  return "UIButton " + UIButtonTriggerAction;
                    case ListenerType.UIDrawer:  return "UIDrawer " + UIDrawerTriggerAction;
                }

                return "---";
            }
        }

        public string WaitForInfoDescription
        {
            get
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (ListenFor)
                {
                    case ListenerType.GameEvent:
                        return AnyValue
                                   ? UILabels.AnyGameEvent
                                   : string.IsNullOrEmpty(GameEventToListenFor)
                                       ? "---"
                                       : GameEventToListenFor;
                    case ListenerType.UIView:   return AnyValue ? UILabels.AnyUIView : ViewCategory + " / " + ViewName;
                    case ListenerType.UIButton: return AnyValue ? UILabels.AnyUIButton : ButtonCategory + " / " + ButtonName;
                    case ListenerType.UIDrawer: return AnyValue ? UILabels.AnyUIDrawer : DrawerName;
                }

                return "---";
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.Global);
            SetName(UILabels.PortalNodeName);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
//            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
        }

        private void AddListeners()
        {
            switch (ListenFor)
            {
                case ListenerType.GameEvent:
                    Message.AddListener<GameEventMessage>(OnGameEventMessage);
                    break;
                case ListenerType.UIButton:
                    Message.AddListener<UIButtonMessage>(OnUIButtonMessage);
                    break;
                case ListenerType.UIView:
                    Message.AddListener<UIViewMessage>(OnUIViewMessage);
                    break;
                case ListenerType.UIDrawer:
                    Message.AddListener<UIDrawerMessage>(OnUIDrawerMessage);
                    break;
            }
        }

        private void RemoveListeners()
        {
            switch (ListenFor)
            {
                case ListenerType.GameEvent:
                    Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
                    break;
                case ListenerType.UIButton:
                    Message.RemoveListener<UIButtonMessage>(OnUIButtonMessage);
                    break;
                case ListenerType.UIView:
                    Message.RemoveListener<UIViewMessage>(OnUIViewMessage);
                    break;
                case ListenerType.UIDrawer:
                    Message.RemoveListener<UIDrawerMessage>(OnUIDrawerMessage);
                    break;
            }
        }

        public override void Activate(Graph portalGraph)
        {
            if (m_activated) return;
            base.Activate(portalGraph);
            PortalGraph = portalGraph;
            AddListeners();
        }

        public override void Deactivate()
        {
            if (!m_activated) return;
            base.Deactivate();
            RemoveListeners();
        }

        private void UpdateSourceNode(Node node)
        {
            if (!SwitchBackMode) return;
            m_sourceNode = node;
        }


        private void OnGameEventMessage(GameEventMessage message)
        {
            if (PortalGraph != null && !PortalGraph.Enabled) return;
            if (message.EventName != GameEventToListenFor) return;
            m_activatedByEvent = true;
            PortalGraph.SetActiveNodeById(Id);
        }

        private void OnUIViewMessage(UIViewMessage message)
        {
            if (PortalGraph != null && !PortalGraph.Enabled) return;
            if (ListenFor != ListenerType.UIView) return;
            if (DebugMode) DDebug.Log("UIViewMessage received: " + message.Type + " " + message.View.ViewCategory + " / " + message.View.ViewName + " // Listening for: " + ViewCategory + " / " + ViewName, this);
            if (message.Type == UIViewBehaviorType.Unknown) return;
            if (message.Type != UIViewTriggerAction) return;
            if (AnyValue ||
                message.View.ViewCategory.Equals(ViewCategory) &&
                message.View.ViewName.Equals(ViewName))
            {
                m_activatedByEvent = true;
                PortalGraph.SetActiveNodeById(Id);
            }
        }

        private void OnUIButtonMessage(UIButtonMessage message)
        {
            if (PortalGraph != null && !PortalGraph.Enabled) return;
            if (ListenFor != ListenerType.UIButton) return;
            if (DebugMode) DDebug.Log("UIButtonMessage received: " + message.Type + " " + message.ButtonName + " // Listening for: " + ButtonName, this);
            if (message.Type != UIButtonTriggerAction) return;

            bool listeningForBackButton = ButtonName.Equals(UIButton.BackButtonName);
            if (listeningForBackButton && (message.ButtonName.Equals(UIButton.BackButtonName) || message.Button != null && message.Button.IsBackButton))
            {
                m_activatedByEvent = true;
                PortalGraph.SetActiveNodeById(Id);
                return;
            }

            if (AnyValue)
            {
                m_activatedByEvent = true;
                PortalGraph.SetActiveNodeById(Id);
                return;
            }

            if (message.Button == null ||
                !message.Button.ButtonCategory.Equals(ButtonCategory) ||
                !message.Button.ButtonName.Equals(ButtonName)) return;
            m_activatedByEvent = true;
            PortalGraph.SetActiveNodeById(Id);
        }

        private void OnUIDrawerMessage(UIDrawerMessage message)
        {
            if (PortalGraph != null && !PortalGraph.Enabled) return;
            if (ListenFor != ListenerType.UIDrawer) return;
            if (DebugMode) DDebug.Log("UIDrawerMessage received: " + message.Type + " " + message.Drawer.DrawerName + " // Listening for: " + DrawerName, this);
            if (message.Type != UIDrawerTriggerAction) return;
            if (AnyValue || message.Drawer.DrawerName.Equals(DrawerName))
            {
                m_activatedByEvent = true;
                PortalGraph.SetActiveNodeById(Id);
            }
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var node = (PortalNode) original;
            m_gameEvent = node.m_gameEvent;
            ListenFor = node.ListenFor;
            AnyValue = node.AnyValue;
            UIViewTriggerAction = node.UIViewTriggerAction;
            ViewCategory = node.ViewCategory;
            ViewName = node.ViewName;
            UIButtonTriggerAction = node.UIButtonTriggerAction;
            ButtonCategory = node.ButtonCategory;
            ButtonName = node.ButtonName;
            UIDrawerTriggerAction = node.UIDrawerTriggerAction;
            DrawerName = node.DrawerName;
            CustomDrawerName = node.CustomDrawerName;
            SwitchBackMode = node.SwitchBackMode;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            if (!FirstOutputSocket.IsConnected) return;

            if (!SwitchBackMode) //Switch Back Node disabled -> Activate the first connected node
            {
                PortalGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
                return;
            }

            if (!m_activatedByEvent && HasSource) //node activated by a direct connection and has a source -> go back
            {
                PortalGraph.SetActiveNodeById(m_sourceNode.Id);
                m_sourceNode = null; //reset source after going back
                return;
            }

            UpdateSourceNode(m_activatedByEvent ? previousActiveNode : null); //update the source to the previously active node if activated by an event
            PortalGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void OnExit(Node nextActiveNode, Connection connection)
        {
            base.OnExit(nextActiveNode, connection);
            m_activatedByEvent = false;
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNotListeningForAnyGameEvent = string.IsNullOrEmpty(m_gameEvent);
#endif
        }
    }
}