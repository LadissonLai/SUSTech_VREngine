// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.SceneManagement;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     It waits for a set Time duration or for a Game Event (string value) or for a Scene to load or for a Scene to unload or for the active scene to change.
    ///     Then it activates the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.WaitNode_CreateNodeMenu_Name, MenuUtils.WaitNode_CreateNodeMenu_Order)]
    public class WaitNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoGameEvent || ErrorNoSceneName || ErrorBadBuildIndex; } }
        public bool ErrorNoGameEvent, ErrorNoSceneName, ErrorBadBuildIndex;
#endif

        private const WaitType DEFAULT_WAIT_TYPE = WaitType.Time;
        private const bool DEFAULT_ANY_VALUE = false;
        private const bool DEFAULT_IGNORE_UNITY_TIMESCALE = true;
        private const bool DEFAULT_RANDOM_DURATION = false;
        private const float DEFAULT_DURATION = 1f;
        private const float DEFAULT_DURATION_MAX = 1f;
        private const float DEFAULT_DURATION_MIN = 0f;
        private const string DEFAULT_GAME_EVENT = "";

        public enum WaitType
        {
            Time,
            GameEvent,
            SceneLoad,
            SceneUnload,
            ActiveSceneChange,
            UIView,
            UIButton,
            UIDrawer
        }


        public GetSceneBy GetSceneBy;
        public WaitType WaitFor = DEFAULT_WAIT_TYPE;
        public bool AnyValue = DEFAULT_ANY_VALUE;
        public bool IgnoreUnityTimescale = DEFAULT_IGNORE_UNITY_TIMESCALE;
        public bool RandomDuration = DEFAULT_RANDOM_DURATION;
        public float Duration = DEFAULT_DURATION;
        public float DurationMax = DEFAULT_DURATION_MAX;
        public float DurationMin = DEFAULT_DURATION_MIN;
        public int SceneBuildIndex;
        public string GameEvent = DEFAULT_GAME_EVENT;
        public string SceneName;
        public UIViewBehaviorType UIViewTriggerAction = UIViewBehaviorType.Show;
        public string ViewCategory = UIView.DefaultViewCategory;
        public string ViewName = UIView.DefaultViewName;
        public UIButtonBehaviorType UIButtonTriggerAction = UIButtonBehaviorType.OnClick;
        public string ButtonCategory = UIButton.DefaultButtonCategory;
        public string ButtonName = UIButton.DefaultButtonName;
        public UIDrawerBehaviorType UIDrawerTriggerAction = UIDrawerBehaviorType.Open;
        public string DrawerName = UIDrawer.DefaultDrawerName;
        public bool CustomDrawerName = false;

        [NonSerialized] public float CurrentDuration;
        [NonSerialized] private bool m_timerIsActive;
        [NonSerialized] private double m_timerStart;
        [NonSerialized] private float m_timeDelay;

        public float TimerProgress { get { return Mathf.Clamp01(m_timerIsActive ? (float) (Time.realtimeSinceStartup - m_timerStart) / m_timeDelay : 0f); } }

        public string WaitForInfoTitle
        {
            get
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (WaitFor)
                {
                    case WaitType.Time:
                        return (RandomDuration
                                    ? "[" + DurationMin + " - " + DurationMax + "]"
                                    : Duration + "")
                               + " " + UILabels.Seconds;
                    case WaitType.GameEvent:         return UILabels.GameEvent;
                    case WaitType.SceneLoad:         return UILabels.SceneLoad;
                    case WaitType.SceneUnload:       return UILabels.SceneUnload;
                    case WaitType.ActiveSceneChange: return UILabels.ActiveSceneChange;
                    case WaitType.UIView:            return "UIView " + UIViewTriggerAction;
                    case WaitType.UIButton:          return "UIButton " + UIButtonTriggerAction;
                    case WaitType.UIDrawer:          return "UIDrawer " + UIDrawerTriggerAction;
                }

                return "---";
            }
        }

        public string WaitForInfoDescription
        {
            get
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (WaitFor)
                {
                    case WaitType.Time: return "";
                    case WaitType.GameEvent:
                        return AnyValue
                                   ? UILabels.AnyGameEvent
                                   : string.IsNullOrEmpty(GameEvent)
                                       ? "---"
                                       : GameEvent;
                    case WaitType.SceneLoad:
                    case WaitType.SceneUnload:
                    case WaitType.ActiveSceneChange:
                        if (AnyValue) return UILabels.AnyScene;
                        // ReSharper disable once SwitchStatementMissingSomeCases
                        switch (GetSceneBy)
                        {
                            case GetSceneBy.Name:
                                return UILabels.Scene + ": " + (string.IsNullOrEmpty(SceneName)
                                                                    ? "---"
                                                                    : SceneName);
                            case GetSceneBy.BuildIndex:
                                return UILabels.BuildIndex + ": " + SceneBuildIndex;
                        }

                        break;
                    case WaitType.UIView:   return AnyValue ? UILabels.AnyUIView : ViewCategory + " / " + ViewName;
                    case WaitType.UIButton: return AnyValue ? UILabels.AnyUIButton : ButtonCategory + " / " + ButtonName;
                    case WaitType.UIDrawer: return AnyValue ? UILabels.AnyUIDrawer : DrawerName;
                }

                return "---";
            }
        }

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.WaitNodeName);
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
            var node = (WaitNode) original;
            GetSceneBy = node.GetSceneBy;
            WaitFor = node.WaitFor;
            AnyValue = node.AnyValue;
            IgnoreUnityTimescale = node.IgnoreUnityTimescale;
            RandomDuration = node.RandomDuration;
            Duration = node.Duration;
            DurationMax = node.DurationMax;
            DurationMin = node.DurationMin;
            SceneBuildIndex = node.SceneBuildIndex;
            GameEvent = node.GameEvent;
            SceneName = node.SceneName;
            UIViewTriggerAction = node.UIViewTriggerAction;
            ViewCategory = node.ViewCategory;
            ViewName = node.ViewName;
            UIButtonTriggerAction = node.UIButtonTriggerAction;
            ButtonCategory = node.ButtonCategory;
            ButtonName = node.ButtonName;
            UIDrawerTriggerAction = node.UIDrawerTriggerAction;
            DrawerName = node.DrawerName;
            CustomDrawerName = node.CustomDrawerName;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (WaitFor == WaitType.Time) UpdateCurrentDuration();
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            StartWait();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!m_timerIsActive) return;
            if (TimerProgress < 1) return;
            m_timerIsActive = false;
            m_timerStart = Time.realtimeSinceStartup;
            ContinueToNextNode();
        }

        public override void OnExit(Node nextActiveNode, Connection connection)
        {
            base.OnExit(nextActiveNode, connection);
            EndWait();
        }

        private void UpdateCurrentDuration()
        {
            CurrentDuration = RandomDuration ? Random.Range(DurationMin, DurationMax) : Duration;
            CurrentDuration = (float) Math.Round(CurrentDuration, 2);
        }

        private void StartWait()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (WaitFor)
            {
                case WaitType.Time:
                    ActivateTimer();
                    break;
                case WaitType.GameEvent:
                    Message.AddListener<GameEventMessage>(OnGameEventMessage);
                    break;
                case WaitType.SceneLoad:
                    SceneDirector.Instance.OnSceneLoaded.AddListener(SceneLoaded);
                    break;
                case WaitType.SceneUnload:
                    SceneDirector.Instance.OnSceneUnloaded.AddListener(SceneUnloaded);
                    break;
                case WaitType.ActiveSceneChange:
                    SceneDirector.Instance.OnSceneUnloaded.AddListener(SceneUnloaded);
                    break;
                case WaitType.UIView:
                    Message.AddListener<UIViewMessage>(OnUIViewMessage);
                    break;
                case WaitType.UIButton:
                    Message.AddListener<UIButtonMessage>(OnUIButtonMessage);
                    break;
                case WaitType.UIDrawer:
                    Message.AddListener<UIDrawerMessage>(OnUIDrawerMessage);
                    break;
            }
        }

        private void EndWait()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (WaitFor)
            {
                case WaitType.Time:
                    StopTimer();
                    UpdateCurrentDuration();
                    break;
                case WaitType.GameEvent:
                    Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
                    break;
                case WaitType.SceneLoad:
                    SceneDirector.Instance.OnSceneLoaded.RemoveListener(SceneLoaded);
                    break;
                case WaitType.SceneUnload:
                    SceneDirector.Instance.OnSceneUnloaded.RemoveListener(SceneUnloaded);
                    break;
                case WaitType.ActiveSceneChange:
                    SceneDirector.Instance.OnActiveSceneChanged.RemoveListener(ActiveSceneChanged);
                    break;
                case WaitType.UIView:
                    Message.RemoveListener<UIViewMessage>(OnUIViewMessage);
                    break;
                case WaitType.UIButton:
                    Message.RemoveListener<UIButtonMessage>(OnUIButtonMessage);
                    break;
                case WaitType.UIDrawer:
                    Message.RemoveListener<UIDrawerMessage>(OnUIDrawerMessage);
                    break;
            }
        }

        private void ActivateTimer()
        {
            m_timerIsActive = true;
            m_timerStart = Time.realtimeSinceStartup;
            m_timeDelay = CurrentDuration;
            UseUpdate = true;
        }

        private void StopTimer()
        {
            m_timerIsActive = false;
            UseUpdate = false;
        }

        private void OnGameEventMessage(GameEventMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (DebugMode) DDebug.Log("GameEvent received: " + message.EventName + " // Listening for: " + GameEvent, this);
            if (AnyValue || GameEvent.Equals(message.EventName))
                ContinueToNextNode();
        }

        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (DebugMode) DDebug.Log("Scene Loaded - Scene: " + scene.name + " // LoadSceneMode: " + mode, this);
            if (AnyValue || IsTargetScene(scene))
                ContinueToNextNode();
        }

        private void SceneUnloaded(Scene unloadedScene)
        {
            if (DebugMode) DDebug.Log("Scene Unloaded - Scene: " + unloadedScene.name, this);
            if (AnyValue || IsTargetScene(unloadedScene))
                ContinueToNextNode();
        }

        private void ActiveSceneChanged(Scene current, Scene next)
        {
            if (DebugMode) DDebug.Log("Active Scene Changed - Replaced Scene: " + current.name + " // Next Scene: " + next.name, this);
            if (AnyValue || IsTargetScene(next))
                ContinueToNextNode();
        }

        private bool IsTargetScene(Scene scene)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    if (scene.name.Equals(SceneName))
                        return true;
                    break;
                case GetSceneBy.BuildIndex:
                    if (!scene.name.Equals(SceneManager.GetSceneByBuildIndex(SceneBuildIndex).name))
                        return true;
                    break;
            }

            return false;
        }

        private void OnUIViewMessage(UIViewMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (WaitFor != WaitType.UIView) return;
            if (DebugMode) DDebug.Log("UIViewMessage received: " + message.Type + " " + message.View.ViewCategory + " / " + message.View.ViewName + " // Listening for: " + ViewCategory + " / " + ViewName, this);
            if (message.Type == UIViewBehaviorType.Unknown) return;
            if (message.Type != UIViewTriggerAction) return;
            if (AnyValue ||
                message.View.ViewCategory.Equals(ViewCategory) &&
                message.View.ViewName.Equals(ViewName))
                ContinueToNextNode();
        }

        private void OnUIButtonMessage(UIButtonMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (WaitFor != WaitType.UIButton) return;
            if (DebugMode) DDebug.Log("UIButtonMessage received: " + message.Type + " " + message.ButtonName + " // Listening for: " + ButtonName, this);
            if (message.Type != UIButtonTriggerAction) return;

            bool listeningForBackButton = ButtonName.Equals(UIButton.BackButtonName);
            if (listeningForBackButton && (message.ButtonName.Equals(UIButton.BackButtonName) || message.Button != null && message.Button.IsBackButton))
            {
                ContinueToNextNode();
                return;
            }

            if (AnyValue)
            {
                ContinueToNextNode();
                return;
            }

            if (message.Button == null || !message.Button.ButtonCategory.Equals(ButtonCategory) || !message.Button.ButtonName.Equals(ButtonName)) return;
            ContinueToNextNode();
        }

        private void OnUIDrawerMessage(UIDrawerMessage message)
        {
            if (ActiveGraph != null && !ActiveGraph.Enabled) return;
            if (WaitFor != WaitType.UIDrawer) return;
            if (DebugMode) DDebug.Log("UIDrawerMessage received: " + message.Type + " " + message.Drawer.DrawerName + " // Listening for: " + DrawerName, this);
            if (message.Type != UIDrawerTriggerAction) return;
            if (AnyValue || message.Drawer.DrawerName.Equals(DrawerName))
                ContinueToNextNode();
        }


        private void ContinueToNextNode()
        {
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNoSceneName = false;
            ErrorBadBuildIndex = false;
            ErrorNoGameEvent = false;

            if (AnyValue) return;

            switch (WaitFor)
            {
                case WaitType.GameEvent:
                    ErrorNoGameEvent = string.IsNullOrEmpty(GameEvent.Trim());
                    break;
                case WaitType.SceneLoad:
                case WaitType.SceneUnload:
                case WaitType.ActiveSceneChange:
                    ErrorNoSceneName = GetSceneBy == GetSceneBy.Name && string.IsNullOrEmpty(SceneName.Trim());
                    ErrorBadBuildIndex = GetSceneBy == GetSceneBy.BuildIndex && (SceneBuildIndex < 0 || SceneBuildIndex + 1 > SceneManager.sceneCountInBuildSettings);
                    break;
            }

#endif
        }
    }
}