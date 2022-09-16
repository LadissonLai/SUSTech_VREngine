// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using DG.Tweening;
using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     Sets the scale at which the time is passing (it updates Time.timeScale). This can be used for slow motion effects.
    ///     <para />
    ///     It does that either instantly or over a set duration (animated).
    ///     <para />
    ///     The node can wait until the current Time.timeScale value has reached the target value, before activating the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.TimeScaleNode_CreateNodeMenu_Name, MenuUtils.TimeScaleNode_CreateNodeMenu_Order)]
    public class TimeScaleNode : Node
    {
        private const float DEFAULT_TARGET_VALUE = 1f;
        private const bool DEFAULT_ANIMATE_VALUE = false;
        private const float DEFAULT_ANIMATION_DURATION = 1f;
        private const Ease DEFAULT_ANIMATION_EASE = Ease.Linear;
        private const bool DEFAULT_WAIT_FOR_ANIMATION_TO_FINISH = false;

        public float TargetValue = DEFAULT_TARGET_VALUE;
        public bool AnimateValue = DEFAULT_ANIMATE_VALUE;
        public float AnimationDuration = DEFAULT_ANIMATION_DURATION;
        public Ease AnimationEase = DEFAULT_ANIMATION_EASE;
        public bool WaitForAnimationToFinish = DEFAULT_WAIT_FOR_ANIMATION_TO_FINISH;

        private string GetAnimationId { get { return "TimeScale Animation"; } }
        public float TimerProgress { get { return Mathf.Clamp01(m_timerIsActive ? (float) (Time.realtimeSinceStartup - m_timerStart) / m_timeDuration : 0f); } }

        [NonSerialized] private Sequence m_animationSequence;
        [NonSerialized] private bool m_timerIsActive;
        [NonSerialized] private double m_timerStart;
        [NonSerialized] private float m_timeDuration;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.TimeScaleNodeName);
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
            var node = (TimeScaleNode) original;
            TargetValue = node.TargetValue;
            AnimateValue = node.AnimateValue;
            AnimationDuration = node.AnimationDuration;
            AnimationEase = node.AnimationEase;
            WaitForAnimationToFinish = node.WaitForAnimationToFinish;
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            if (ActiveGraph == null) return;
            DOTween.Kill(GetAnimationId, true);
            ExecuteActions();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!m_timerIsActive) return;
            if (TimerProgress < 1) return;
            m_timerIsActive = false;
            m_timerStart = Time.realtimeSinceStartup;
            if (WaitForAnimationToFinish) ContinueToNextNode();
        }

        private void ContinueToNextNode()
        {
            if (!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        private void ExecuteActions()
        {
            if (AnimateValue) KillAnimation(true);

            if (AnimationDuration <= 0) AnimationDuration = 0;
            if (!AnimateValue || AnimationDuration <= 0 || Math.Abs(Time.timeScale - TargetValue) < 0.001f)
            {
                Time.timeScale = TargetValue;
                ContinueToNextNode();
                return;
            }

            m_animationSequence.Append(GetAnimationTween(TargetValue, AnimationDuration, AnimationEase, GetAnimationId))
                               .OnComplete(StopTimer)
                               .Play();
            ActivateTimer();

            if (!WaitForAnimationToFinish) ContinueToNextNode();
        }

        private void ActivateTimer()
        {
            m_timerIsActive = true;
            m_timerStart = Time.realtimeSinceStartup;
            m_timeDuration = AnimationDuration;
            UseUpdate = true;
        }

        private void StopTimer()
        {
            m_timerIsActive = false;
            UseUpdate = false;
        }

        /// <summary> If the animation tween is running it will get killed </summary>
        /// <param name="complete"> If TRUE completes the tween before killing it </param>
        private void KillAnimation(bool complete = false)
        {
            if (m_animationSequence == null) return;
            m_animationSequence.Kill(complete);
            m_animationSequence = null;
            StopTimer();
        }

        /// <summary> Returns a new Tween that will be used to animate the current Time.timeScale value, if AnimateValue is enabled </summary>
        /// <param name="targetValue"> Target value for the current value </param>
        /// <param name="duration"> The tween's duration </param>
        /// <param name="ease"> Ease curve used by the animation </param>
        /// <param name="id"> Animation id </param>
        private static Tween GetAnimationTween(float targetValue, float duration, Ease ease, string id)
        {
            return DOTween.To(() => Time.timeScale, x => Time.timeScale = x, targetValue, duration)
                          .SetEase(ease)
                          .SetId(id)
                          .SetUpdate(true);
        }
    }
}