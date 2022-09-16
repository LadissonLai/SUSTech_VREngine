// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable InconsistentNaming
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable ConvertToAutoProperty
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Animates a value between a minimum and a maximum interval. It also calculates the progress value (from 0 to 1) and the inverse progress (from 1 to 0).
    /// This component is meant to help visualize a progression of an operation (e.g. a scene load progress).
    /// It can be used to update visual elements of an energy bar, a health bar or any type of progress indicator.
    /// It can be linked to anything that takes a value (from min to max) or a progress (from 0 to 1 or from 0% to 100%).
    /// </summary>
    [AddComponentMenu(MenuUtils.Progressor_AddComponentMenu_MenuName, MenuUtils.Progressor_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESSOR)]
    public class Progressor : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.Progressor_MenuItem_ItemName, false, MenuUtils.Progressor_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Constants

        /// <summary> Tolerance used for float comparisons </summary>
        public const float TOLERANCE = 0.001f;

        public const bool DEFAULT_ANIMATE_VALUE = false;
        public const float DEFAULT_DURATION = 0.5f;
        public const Ease DEFAULT_EASE = Ease.Linear;
        public const bool DEFAULT_IGNORE_UNITY_TIMESCALE = true;

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Progress targets that will get updated every time the current Value or Progress value change </summary>
        public List<ProgressTarget> ProgressTargets;

        /// <summary> If TRUE, when the current Value or Progress value get updated, the change will be animated </summary>
        public bool AnimateValue = DEFAULT_ANIMATE_VALUE;

        /// <summary> Duration for the current Value to reach the target value when AnimateValue is enabled </summary>
        public float AnimationDuration = DEFAULT_DURATION;

        /// <summary> The ease used by the value animator when AnimateValue is enabled </summary>
        public Ease AnimationEase = DEFAULT_EASE;

        /// <summary> Determines if the value animator should respect Unity's Timescale, when AnimateValue is enabled  </summary>
        public bool AnimationIgnoresUnityTimescale = DEFAULT_IGNORE_UNITY_TIMESCALE;

        /// <summary> Reset behavior for the current Value that happens OnEnable </summary>
        public ResetValue OnEnableResetValue = ResetValue.ToMinValue;

        /// <summary> Reset behavior for the current Value that happens OnDisable </summary>
        public ResetValue OnDisableResetValue = ResetValue.Disabled;

        /// <summary> Custom reset value for the current Value, used if ResetValue.ToCustomValue is set for either OnEnableResetValue or OnDisableResetValue </summary>
        public float CustomResetValue;

        /// <summary>
        ///     Callback executed when the current Value has changed
        ///     <para />
        ///     Passes the current Value (float between MinValue and MaxValue)
        /// </summary>
        public ProgressEvent OnValueChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the current Value has changed and the Progress has been updated.
        ///     <para />
        ///     Passes the Progress value (float between 0 and 1)
        /// </summary>
        public ProgressEvent OnProgressChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the current Value has changed and the Progress has been updated.
        ///     <para />
        ///     Passes the InverseProgress value (float between 1 and 0)
        ///     <para />
        ///     InverseProgress = 1 - Progress
        /// </summary>
        public ProgressEvent OnInverseProgressChanged = new ProgressEvent();

        #endregion

        #region Properties

        /// <summary> Returns the current progress value (float between 0 and 1) </summary>
        public float Progress
        {
            get
            {
                return Math.Abs(MinValue - MaxValue) < TOLERANCE
                           ? 0
                           : Mathf.Clamp01((Value - MinValue) / (MaxValue - MinValue));
            }
        }

        /// <summary> Returns the inverse value of current Progress value (float between 1 and 0) </summary>
        public float InverseProgress { get { return 1 - Progress; } }

        /// <summary> Returns the current Value (float between MinValue and MaxValue) </summary>
        public float Value { get { return m_currentValue; } private set { m_currentValue = value; } }

        /// <summary> Returns the minimum value that the current Value can have </summary>
        public float MinValue { get { return m_minValue; } protected set { m_minValue = value; } }

        /// <summary> Returns the maximum value that the current Value can have </summary>
        public float MaxValue { get { return m_maxValue; } protected set { m_maxValue = value; } }

        /// <summary> Returns TRUE if the current Value will rounded to the nearest integer </summary>
        public bool WholeNumbers { get { return m_wholeNumbers; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugProgressor; } }
        private string GetAnimationId { get { return GetInstanceID() + " - Progressor Animation"; } }

        #endregion

        #region Private Variables

        /// <summary> The minimum value that the current Value can have </summary>
        [SerializeField] private float m_minValue;

        /// <summary> The maximum value that the current Value can have </summary>
        [SerializeField] private float m_maxValue = 1f;

        /// <summary> Should the current Value get rounded to the nearest integer? </summary>
        [SerializeField] private bool m_wholeNumbers;

        /// <summary> Internal variable that holds the current value that determines the Progress </summary>
        [SerializeField] private float m_currentValue;

        /// <summary> Internal variable that holds the previous value that determines the Progress </summary>
        private float m_previousValue;

        /// <summary> Internal variable used to hold a reference to the sequence used to animate the current Value, when AnimateValue is enabled </summary>
        private Sequence m_animationSequence;

        /// <summary> Internal variable used to update the progress targets </summary>
        private float m_value, m_progress, m_inverseProgress;

        /// <summary> Internal variable used to update the m_previousValue variable when the script is enabled </summary>
        private bool m_updatePreviousValue = true;

        /// <summary> Internal variable used to keep track of the Tween used by this Progressor </summary>
        private Tweener m_tween;

        /// <summary> Internal variable used to keep track if the Tween used by this Progressor has been initialized or not </summary>
        private bool m_tweenInitialized = false;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            KillAnimation();
            ResetValueTo(OnEnableResetValue);
            m_updatePreviousValue = true;
            KillTweener(true);
        }

        private void OnDisable()
        {
            KillAnimation();
            ResetValueTo(OnDisableResetValue);
            KillTweener(true);
        }

        private void Update()
        {
            m_currentValue = ClampValueBetweenMinAndMax(m_currentValue, m_wholeNumbers);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!m_updatePreviousValue && m_previousValue == Value) return;
            OnValueUpdated();
            m_previousValue = Value;
            m_updatePreviousValue = false;
        }

        #endregion

        #region Public Methods

        /// <summary> Method called every time the current Value gets updated </summary>
        public void OnValueUpdated()
        {
            OnValueChanged.Invoke(Value);
            UpdateProgress();
            UpdateProgressTargets();
        }

        /// <summary> Update all the progress targets by calling the UpdateTarget method for each target </summary>
        public void UpdateProgressTargets()
        {
            if (ProgressTargets == null) return;
            bool foundNullTarget = false;
            foreach (ProgressTarget target in ProgressTargets)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                if (target == null)
                {
                    foundNullTarget = true;
                    continue; //sanity check in case one of the targets became null
                }

                target.UpdateTarget(this);
            }

            if (!foundNullTarget) return;
            //a null progress target reference was found -> remove it from the list
            for (int i = ProgressTargets.Count - 1; i >= 0; i--)
                if (ProgressTargets[i] == null)
                    ProgressTargets.RemoveAt(i);
        }

        #region SetValue

        /// <summary> Update the current Value for this Progressor </summary>
        /// <param name="value"> The new current Value </param>
        public void SetValue(float value) { SetValue(value, false); }

        /// <summary> Update the current Value for this Progressor instantly, without animating the value </summary>
        /// <param name="value"> The new current Value </param>
        public void InstantSetValue(float value) { SetValue(value, true); }

        /// <summary> Update the current Value for this Progressor. Ignores the AnimateValue option if instantUpdate is passed as TRUE </summary>
        /// <param name="value"> The new current Value </param>
        /// <param name="instantUpdate"> If TRUE, the current Value will not get animated even if AnimateValue is set to TRUE </param>
        public void SetValue(float value, bool instantUpdate)
        {
//            m_previousValue = Value;
            value = ClampValueBetweenMinAndMax(value, m_wholeNumbers);
            if (Math.Abs(value - Value) < TOLERANCE) return;

            if (AnimateValue) KillAnimation();

            if (instantUpdate || !AnimateValue)
            {
                Value = value;
                return;
            }

            if (!m_tweenInitialized)
                m_tween = GetAnimationTween(value, AnimationDuration, AnimationEase, AnimationIgnoresUnityTimescale);
            else
                m_tween.ChangeEndValue(value, true);

            m_animationSequence.Append(m_tween)
                               .Play();
        }

        #endregion

        #region SetProgress, GetProgress, UpdateProgress

        /// <summary>
        ///     Updates the current Value for this Progressor by updating the Progress value (progressValue is clamped between 0 and 1)
        ///     <para />
        ///     current Value = MinValue + progressValue * (MaxValue - MinValue)
        /// </summary>
        /// <param name="progressValue"> The new progress value for the progress bar </param>
        public void SetProgress(float progressValue) { SetProgress(progressValue, false); }

        /// <summary>
        ///     Updates the current Value for this Progressor instantly, without animating the value, by updating the Progress value (progressValue is clamped between 0 and 1)
        ///     <para />
        ///     current Value = MinValue + progressValue * (MaxValue - MinValue)
        /// </summary>
        /// <param name="progressValue"> The new progress value for the progress bar </param>
        public void InstantSetProgress(float progressValue) { SetProgress(progressValue, true); }

        /// <summary>
        ///     Updates the current Value for this Progressor by updating the Progress value (progressValue is clamped between 0 and 1).
        ///     Ignores the AnimateValue option if instantUpdate is passed as TRUE.
        ///     <para />
        ///     current Value = MinValue + progressValue * (MaxValue - MinValue)
        /// </summary>
        /// <param name="progressValue"> The new progress value for the progress bar </param>
        /// <param name="instantUpdate"> If TRUE, the value will not get animated even if AnimateValue is set to TRUE </param>
        public void SetProgress(float progressValue, bool instantUpdate) { SetValue(m_minValue + Mathf.Clamp01(progressValue) * (m_maxValue - m_minValue), instantUpdate); }

        /// <summary> Depending on the direction, returns either the Progress or the InverseProgress value </summary>
        /// <param name="direction"> Progress Direction to return </param>
        public float GetProgress(TargetProgress direction)
        {
            switch (direction)
            {
                case TargetProgress.Progress:        return Progress;
                case TargetProgress.InverseProgress: return InverseProgress;
                default:                             throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        /// <summary> Invoke the OnProgressChanged and OnInverseProgressChanged events </summary>
        public void UpdateProgress()
        {
            if (DebugComponent) DDebug.Log("[" + name + "] Value: " + Value + " / Progress: " + Progress + " / Inverse Progress: " + InverseProgress, this);
            OnProgressChanged.Invoke(Progress);
            OnInverseProgressChanged.Invoke(InverseProgress);
        }

        #endregion

        #region SetMin, SetMax

        /// <summary> Update the MinValue </summary>
        /// <param name="value"> The new MinValue </param>
        public void SetMin(float value)
        {
            float result = value > m_maxValue ? m_maxValue : value;
            m_minValue = m_wholeNumbers ? RoundValue(result) : result;
            if (Value < m_minValue) SetValue(m_minValue);
            UpdateProgress();
        }

        /// <summary> Update the MaxValue </summary>
        /// <param name="value"> The new MaxValue </param>
        public void SetMax(float value)
        {
            float result = value < m_minValue ? m_minValue : value;
            m_maxValue = m_wholeNumbers ? RoundValue(result) : result;
            if (Value > m_maxValue) SetValue(m_maxValue);
            UpdateProgress();
        }

        #endregion

        #region EnableWholeNumbers, DisableWholeNumbers

        /// <summary> Force the current Value only be allowed to be whole numbers </summary>
        public void EnableWholeNumbers() { m_wholeNumbers = true; }

        /// <summary> Allow the current Value to be any fractional number </summary>
        public void DisableWholeNumbers() { m_wholeNumbers = false; }

        #endregion

        #region ResetValueTo

        /// <summary> Resets the current Value to the set reset value instantly </summary>
        /// <param name="resetValue"> Target reset value </param>
        public void ResetValueTo(ResetValue resetValue) { ResetValueTo(resetValue, true); }

        /// <summary> Resets the current Value to the set reset value. The reset happens instantly if instantUpdate is passed as TRUE </summary>
        /// <param name="resetValue"> Target reset value </param>
        /// <param name="instantUpdate"> If TRUE, the value will not get animated and be changed instantly </param>
        public void ResetValueTo(ResetValue resetValue, bool instantUpdate)
        {
            switch (resetValue)
            {
                case ResetValue.ToMinValue:
                    if (DebugComponent) DDebug.Log("[" + name + "] Resetting Value to MinValue: " + MinValue, this);
                    SetValue(MinValue, instantUpdate);
                    break;
                case ResetValue.ToMaxValue:
                    if (DebugComponent) DDebug.Log("[" + name + "] Resetting Value to MaxValue: " + MaxValue, this);
                    SetValue(MaxValue, instantUpdate);
                    break;
                case ResetValue.ToCustomValue:
                    if (DebugComponent) DDebug.Log("[" + name + "] Resetting Value to CustomResetValue: " + CustomResetValue, this);
                    SetValue(CustomResetValue, instantUpdate);
                    break;
            }

            OnValueUpdated();
        }

        #endregion

        /// <summary> Returns the passed value clamped between MinValue and MaxValue </summary>
        /// <param name="value"> Target value </param>
        /// <param name="roundValue"> Should the returned be rounded to the nearest integer </param>
        public float ClampValueBetweenMinAndMax(float value, bool roundValue = false)
        {
            value = Mathf.Clamp(value, MinValue, m_maxValue);
            return roundValue ? RoundValue(value) : value;
        }

        /// <summary> Returns a new Tween that will be used to animate the current Value, if AnimateValue is enabled </summary>
        /// <param name="targetValue"> Target value for the current value </param>
        /// <param name="duration"> The tween's duration </param>
        /// <param name="ease"> Ease curve used by the animation </param>
        /// <param name="ignoreTimescale"> Should the tween ignore Unity's timescale </param>
        public Tweener GetAnimationTween(float targetValue, float duration, Ease ease, bool ignoreTimescale)
        {
            return DOTween.To(() => m_currentValue, x => m_currentValue = x, targetValue, duration)
//                          .SetSpeedBased(true)
                          .SetId(this)
                          .SetEase(ease)
                          .SetUpdate(ignoreTimescale)
                          .SetAutoKill(false)
                          .SetRecyclable(true);
        }

        #endregion

        #region Private Methods

        /// <summary> If the animation tween is running it will get killed </summary>
        /// <param name="complete"> If TRUE completes the tween before killing it </param>
        private void KillAnimation(bool complete = false)
        {
            DOTween.Kill(this, complete);
            if (m_animationSequence == null) return;
            m_animationSequence.Kill(complete);
            m_animationSequence = null;
        }

        private void KillTweener(bool complete = false)
        {
            if(!m_tweenInitialized) return;
            m_tween.Kill(complete);
            m_tweenInitialized = false;
        }

        #endregion

        #region Static Mehtods

        /// <summary> Returns the given value rounded to the nearest integer </summary>
        /// <param name="value"> Target value </param>
        private static float RoundValue(float value) { return Mathf.Round(value); }

        /// <summary> Adds Progressor to scene and returns a reference to it </summary>
        private static Progressor AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<Progressor>(MenuUtils.Progressor_GameObject_Name, false, selectGameObjectAfterCreation); }

        #endregion
    }
}