// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to trigger an UIAction at a set progress or value
    /// </summary>
    [AddComponentMenu(MenuUtils.ProgressTargetAction_AddComponentMenu_MenuName, MenuUtils.ProgressTargetAction_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_ACTION)]
    public class ProgressTargetAction : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetAction_MenuItem_ItemName, false, MenuUtils.ProgressTargetAction_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetAction>(MenuUtils.ProgressTargetAction_GameObject_Name, false, true); }
#endif

        #endregion

        #region Constants

        private const float TRIGGER_VALUE_DEFAULT_VALUE = 1f;
        private const float TOLERANCE_DEFAULT_VALUE = 0.01f;
        private const bool DISABLE_TRIGGER_AFTER_ACTIVATION_DEFAULT_VALUE = true;
        private const bool RESET_ON_ENABLE_DEFAULT_VALUE = true;
        private const bool RESET_ON_DISABLE_DEFAULT_VALUE = true;
        private const bool RESET_AFTER_DELAY_DEFAULT_VALUE = true;
        private const float RESET_DELAY_DEFAULT_VALUE = 3f;
        private const bool USE_UNSCALED_TIME_DEFAULT_VALUE = true;

        #endregion

        #region Properties

        /// <summary> Returns TRUE if the UIAction can be triggered </summary>
        public bool IsActive { get { return !m_actionTriggered; } }

        #endregion

        #region Public Variables

        /// <summary> UIAction executed when triggered </summary>
        public UIAction Actions = new UIAction();

        /// <summary> Determines how the trigger value should be compared to the target value </summary>
        public CompareType CompareMethod = CompareType.EqualTo;

        /// <summary> If TRUE, the trigger will get disabled after it has been activated </summary>
        public bool DisableTriggerAfterActivation = DISABLE_TRIGGER_AFTER_ACTIVATION_DEFAULT_VALUE;

        /// <summary> If TRUE, the trigger will reset after a set reset delay </summary>
        public bool ResetAfterDelay = RESET_AFTER_DELAY_DEFAULT_VALUE;

        /// <summary> Time duration to wait before enabling the trigger, after it has been activated. If Reset After Delay is enabled. </summary>
        public float ResetDelay = RESET_DELAY_DEFAULT_VALUE;

        /// <summary> If TRUE, the trigger will reset OnDisable </summary>
        public bool ResetOnDisable = RESET_ON_DISABLE_DEFAULT_VALUE;

        /// <summary> If TRUE, the trigger will reset OnEnable </summary>
        public bool ResetOnEnable = RESET_ON_ENABLE_DEFAULT_VALUE;

        /// <summary> The value that will trigger the UIAction </summary>
        public float TriggerValue = TRIGGER_VALUE_DEFAULT_VALUE;

        /// <summary> The minimum value that will trigger the UIAction </summary>
        public float TriggerMinValue = TRIGGER_VALUE_DEFAULT_VALUE;

        /// <summary> The maximum value that will trigger the UIAction </summary>
        public float TriggerMaxValue = TRIGGER_VALUE_DEFAULT_VALUE;

        /// <summary> The Progressor variable that will get compared with the TargetValue </summary>
        public ProgressorVariable TargetVariable = ProgressorVariable.Progress;

        /// <summary> Minimum tolerance to trigger this UIAction </summary>
        public float Tolerance = TOLERANCE_DEFAULT_VALUE;

        /// <summary> If TRUE, unscaled delta time will be used for the reset delay. Otherwise, the scaled delta time well get used </summary>
        public bool UseUnscaledTime = USE_UNSCALED_TIME_DEFAULT_VALUE;

        #endregion

        #region Private Variables

        /// <summary> Internal variable used to keep track if this trigger has been activated </summary>
        private bool m_actionTriggered;

        /// <summary> Internal variable used to keep track of the reset coroutine activated if reset after delay is used </summary>
        private Coroutine m_resetCoroutine;

        /// <summary> Internal variable used to keep track of the last Progressor that updated this Progress Target </summary>
        private Progressor m_progressor;

        /// <summary> Internal variable used a ticker to check the value </summary>
        private float m_updateInterval = 0.1f;

        /// <summary> Internal variable used to keep track of the update ticker </summary>
        private float m_nextUpdateTime;

        #endregion

        #region Unity Methods

        private void Awake() { ResetTrigger(); }

        public override void OnEnable()
        {
            base.OnEnable();
            if (ResetOnEnable) ResetTrigger();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (ResetOnDisable) ResetTrigger();
        }

        private void Update()
        {
            if (!(Time.deltaTime + m_updateInterval > m_nextUpdateTime)) return;
            m_nextUpdateTime += m_updateInterval;
            CheckTriggerValue();
        }

        #endregion

        #region Public Methods

        /// <summary> Reset the trigger </summary>
        public void ResetTrigger()
        {
            m_actionTriggered = false;

            if (m_resetCoroutine == null) return;
            StopCoroutine(m_resetCoroutine);
            m_resetCoroutine = null;
        }

        /// <summary> Trigger the Actions </summary>
        public void TriggerActions()
        {
            Actions.Invoke(gameObject);

            if (!DisableTriggerAfterActivation) return;
            m_actionTriggered = true;
            if (ResetAfterDelay)
                m_resetCoroutine = StartCoroutine(ExecuteResetTrigger());
        }

        /// <inheritdoc />
        /// <summary> Method used by a Progressor to when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update </param>
        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);
            m_progressor = progressor;
            if (!IsActive || Actions == null) return;
            CheckTriggerValue();
        }

        #endregion

        #region Private Methods

        private void CheckTriggerValue()
        {
            if (m_progressor == null) return;

            float progressorTargetValue;
            switch (TargetVariable)
            {
                case ProgressorVariable.Value:
                    progressorTargetValue = m_progressor.Value;
                    break;
                case ProgressorVariable.Progress:
                    progressorTargetValue = m_progressor.Progress;
                    break;
                case ProgressorVariable.InverseProgress:
                    progressorTargetValue = m_progressor.InverseProgress;
                    break;
                default:
                    progressorTargetValue = m_progressor.Value;
                    break;
            }

            switch (CompareMethod)
            {
                case CompareType.Between:
                    if (progressorTargetValue >= TriggerMinValue && progressorTargetValue <= TriggerMaxValue)
                        TriggerActions();

                    break;
                case CompareType.NotBetween:
                    if (progressorTargetValue < TriggerMinValue || progressorTargetValue > TriggerMaxValue)
                        TriggerActions();

                    break;
                case CompareType.EqualTo:
                    if (Math.Abs(progressorTargetValue - TriggerValue) < Tolerance)
                        TriggerActions();

                    break;
                case CompareType.NotEqualTo:
                    if (Math.Abs(progressorTargetValue - TriggerValue) > Tolerance)
                        TriggerActions();

                    break;
                case CompareType.GreaterThan:
                    if (progressorTargetValue > TriggerValue)
                        TriggerActions();

                    break;
                case CompareType.LessThan:
                    if (progressorTargetValue < TriggerValue)
                        TriggerActions();

                    break;
                case CompareType.GreaterThanOrEqualTo:
                    if (progressorTargetValue >= TriggerValue)
                        TriggerActions();

                    break;
                case CompareType.LessThanOrEqualTo:
                    if (progressorTargetValue <= TriggerValue)
                        TriggerActions();

                    break;
            }
        }

        #endregion

        #region IEnumerators

        private IEnumerator ExecuteResetTrigger()
        {
            if (ResetDelay > 0)
            {
                if (UseUnscaledTime)
                    yield return new WaitForSecondsRealtime(ResetDelay);
                else
                    yield return new WaitForSeconds(ResetDelay);
            }

            m_resetCoroutine = null;
            ResetTrigger();
        }

        #endregion
    }
}