// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Nody;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.UI.Internal;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable InconsistentNaming

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Core Navigation Node. It shows and/or hides UIViews and listens for button events, game events or waits for a set delay before signaling the Graph to activate another node.
    /// </summary>
    [NodeMenu(MenuUtils.UINode_CreateNodeMenu_Name, MenuUtils.UINode_CreateNodeMenu_Order)]
    public class UINode : Node
    {
        [SerializeField] private List<UIViewCategoryName> m_onEnterShowViews;
        [SerializeField] private List<UIViewCategoryName> m_onEnterHideViews;
        [SerializeField] private List<UIViewCategoryName> m_onExitShowViews;
        [SerializeField] private List<UIViewCategoryName> m_onExitHideViews;

        public List<UIViewCategoryName> OnEnterShowViews { get { return m_onEnterShowViews; } }
        public List<UIViewCategoryName> OnEnterHideViews { get { return m_onEnterHideViews; } }
        public List<UIViewCategoryName> OnExitShowViews { get { return m_onExitShowViews; } }
        public List<UIViewCategoryName> OnExitHideViews { get { return m_onExitHideViews; } }

        [NonSerialized] private bool m_timerIsActive;
        [NonSerialized] private double m_timerStart;
        [NonSerialized] private float m_timeDelay;
        [NonSerialized] private Socket m_activeSocketAfterTimeDelay;

        public float TimerProgress { get { return Mathf.Clamp01(m_timerIsActive ? (float) (Time.realtimeSinceStartup - m_timerStart) / m_timeDelay : 0f); } }

        public enum NodeState
        {
            OnEnter,
            OnExit
        }

        public enum ViewAction
        {
            ShowView,
            HideView
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);

            var node = (UINode) original;
            m_onEnterShowViews = UIViewCategoryNameListCopy(node.OnEnterShowViews);
            m_onEnterHideViews = UIViewCategoryNameListCopy(node.OnEnterHideViews);
            m_onExitShowViews = UIViewCategoryNameListCopy(node.OnExitShowViews);
            m_onExitHideViews = UIViewCategoryNameListCopy(node.OnExitHideViews);
        }

        private List<UIViewCategoryName> UIViewCategoryNameListCopy(List<UIViewCategoryName> original)
        {
            var list = new List<UIViewCategoryName>();
            if (original == null) return list;
            foreach (UIViewCategoryName uiViewCategoryName in original)
                list.Add(uiViewCategoryName.Copy());
            return list;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.UINodeNodeName);
            SetWidth(NodySettings.Instance.DefaultNodeWidth);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(UIConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(UIConnection), true, true);
            UIConnection value = UIConnection.GetValue(OutputSockets[0]);
            value.Trigger = UIConnectionTrigger.ButtonClick;
            value.ButtonCategory = NamesDatabase.GENERAL;
            value.ButtonName = NamesDatabase.BACK;
            UIConnection.SetValue(OutputSockets[0], value);
        }

        public void SortShowViewsList()
        {
            if (m_onEnterShowViews == null) m_onEnterShowViews = new List<UIViewCategoryName>();
            if (m_onExitShowViews == null) m_onExitShowViews = new List<UIViewCategoryName>();

            m_onEnterShowViews = SortViewsList(m_onEnterShowViews);
            m_onExitShowViews = SortViewsList(m_onExitShowViews);
        }

        public void SortHideViewsList()
        {
            if (m_onEnterHideViews == null) m_onEnterHideViews = new List<UIViewCategoryName>();
            if (m_onExitHideViews == null) m_onExitHideViews = new List<UIViewCategoryName>();

            m_onEnterHideViews = SortViewsList(m_onEnterHideViews);
            m_onExitHideViews = SortViewsList(m_onExitHideViews);
        }

        private static List<UIViewCategoryName> SortViewsList(IEnumerable<UIViewCategoryName> list)
        {
            if (list == null) return new List<UIViewCategoryName>();
            return list.OrderBy(x => x.Category)
                       .ThenBy(x => x.Name)
                       .ToList();
        }

        private void AddListeners()
        {
            Message.AddListener<UIButtonMessage>(OnButtonMessage);
            Message.AddListener<GameEventMessage>(OnGameEventMessage);
        }

        private void RemoveListeners()
        {
            Message.RemoveListener<UIButtonMessage>(OnButtonMessage);
            Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
        }

        private void OnButtonMessage(UIButtonMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (OutputSockets == null || OutputSockets.Count == 0) return;

            UIConnectionTrigger trigger;
            switch (message.Type)
            {
                case UIButtonBehaviorType.OnClick:
                    trigger = UIConnectionTrigger.ButtonClick;
                    break;
                case UIButtonBehaviorType.OnDoubleClick:
                    trigger = UIConnectionTrigger.ButtonDoubleClick;
                    break;
                case UIButtonBehaviorType.OnLongClick:
                    trigger = UIConnectionTrigger.ButtonLongClick;
                    break;
                case UIButtonBehaviorType.OnRightClick:
                case UIButtonBehaviorType.OnPointerEnter:
                case UIButtonBehaviorType.OnPointerExit:
                case UIButtonBehaviorType.OnPointerDown:
                case UIButtonBehaviorType.OnPointerUp:
                case UIButtonBehaviorType.OnSelected:
                case UIButtonBehaviorType.OnDeselected:
                    return;
                default: throw new ArgumentOutOfRangeException();
            }

            foreach (Socket socket in OutputSockets)
            {
                if (!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if (value.Trigger != trigger) continue;
                if (!value.ButtonName.Equals(message.Button != null ? message.Button.ButtonName : message.ButtonName)) continue;
                ActivateOutputSocketInputNode(socket);
                break;
            }
        }

        private void OnGameEventMessage(GameEventMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (OutputSockets == null || OutputSockets.Count == 0) return;

            foreach (Socket socket in OutputSockets)
            {
                if (!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if (value.Trigger != UIConnectionTrigger.GameEvent) continue;
                if (!value.GameEvent.Equals(message.EventName)) continue;
                ActivateOutputSocketInputNode(socket);
                break;
            }
        }

        private void LookForTimeDelay()
        {
            m_timerIsActive = false;
            UseUpdate = false;
            if (OutputSockets == null || OutputSockets.Count == 0) return;
            foreach (Socket socket in OutputSockets)
            {
                if (!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if (value.Trigger != UIConnectionTrigger.TimeDelay) continue;
                if (value.TimeDelay < 0) continue; //sanity check
                ActivateTimer(value.TimeDelay, socket);
                break;
            }
        }

        private void ActivateTimer(float timeDelay, Socket socket)
        {
            m_timerIsActive = true;
            m_timerStart = Time.realtimeSinceStartup;
            m_timeDelay = timeDelay;
            m_activeSocketAfterTimeDelay = socket;
            UseUpdate = true;
        }

        private void ActivateOutputSocketInputNode(Socket socket)
        {
            if (ActiveGraph == null || socket == null) return; //sanity check
            ActiveGraph.SetActiveNodeByConnection(socket.Connections[0]);
        }

        public override void Activate(Graph portalGraph)
        {
            if (m_activated) return;
            base.Activate(portalGraph);
            AddListeners();
        }

        public override void Deactivate()
        {
            if (!m_activated) return;
            base.Deactivate();
            RemoveListeners();
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            Activate(ActiveGraph);
            LookForTimeDelay();
            ShowViews(m_onEnterShowViews);
            HideViews(m_onEnterHideViews);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!m_timerIsActive) return;
            if (TimerProgress < 1) return;
            m_timerIsActive = false;
            m_timerStart = Time.realtimeSinceStartup;
            ActivateOutputSocketInputNode(m_activeSocketAfterTimeDelay);
        }

        public override void OnExit(Node nextActiveNode, Connection connection)
        {
            base.OnExit(nextActiveNode, connection);
            Deactivate();
            ShowViews(m_onExitShowViews);
            HideViews(m_onExitHideViews);
        }

        public void ShowViews(List<UIViewCategoryName> views)
        {
            foreach (UIViewCategoryName view in views)
            {
                if (DebugMode) DDebug.Log("Show UIView: " + view.Category + " / " + view.Name);
                Coroutiner.Start(UIView.ShowViewNextFrame(view.Category, view.Name, view.InstantAction));
//                UIView.ShowView(view.Category, view.Name, view.InstantAction);
            }
        }

        public void HideViews(List<UIViewCategoryName> views)
        {
            foreach (UIViewCategoryName view in views)
            {
                if (DebugMode) DDebug.Log("Hide UIView: " + view.Category + " / " + view.Name);
                Coroutiner.Start(UIView.HideViewNextFrame(view.Category, view.Name, view.InstantAction));
//                UIView.HideView(view.Category, view.Name, view.InstantAction);
            }
        }

        public void AddView(UIViewCategoryName view, NodeState nodeState, ViewAction viewAction, bool saveAssets = false)
        {
            if (view == null) return;
            switch (nodeState)
            {
                case NodeState.OnEnter:
                    switch (viewAction)
                    {
                        case ViewAction.ShowView:
                            if (m_onEnterShowViews == null) m_onEnterShowViews = new List<UIViewCategoryName>();
                            m_onEnterShowViews.Add(view);
                            break;
                        case ViewAction.HideView:
                            if (m_onEnterHideViews == null) m_onEnterHideViews = new List<UIViewCategoryName>();
                            m_onEnterHideViews.Add(view);
                            break;
                    }

                    break;
                case NodeState.OnExit:
                    switch (viewAction)
                    {
                        case ViewAction.ShowView:
                            if (m_onExitShowViews == null) m_onExitShowViews = new List<UIViewCategoryName>();
                            m_onExitShowViews.Add(view);
                            break;
                        case ViewAction.HideView:
                            if (m_onExitHideViews == null) m_onExitHideViews = new List<UIViewCategoryName>();
                            m_onExitHideViews.Add(view);
                            break;
                    }

                    break;
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            if (saveAssets) AssetDatabase.SaveAssets();
#endif
        }
    }
}