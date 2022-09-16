// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Events;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Contains settings and 'actions' used and triggered by the UIButton when it changes its state
    /// </summary>
    [Serializable]
    public class UIButtonBehavior
    {
        #region Constants

        public const ButtonAnimationType DEFAULT_BUTTON_ANIMATION_TYPE = ButtonAnimationType.Punch;
        public const bool DEFAULT_DESELECT_BUTTON = false;
        public const bool DEFAULT_ENABLED = false;
        public const bool DEFAULT_LOAD_SELECTED_PRESET_AT_RUNTIME = false;
        public const bool DEFAULT_READY = true;
        public const bool DEFAULT_SELECT_BUTTON = false;
        public const bool DEFAULT_TRIGGER_EVENTS_AFTER_ANIMATION = false;
        public const float ON_BUTTON_DESELECTED_DISABLE_INTERVAL = 0f;
        public const float ON_BUTTON_SELECTED_DISABLE_INTERVAL = 0f;
        public const float ON_CLICK_DISABLE_INTERVAL = 0.4f;
        public const float ON_DOUBLE_CLICK_DISABLE_INTERVAL = 0.2f;
        public const float ON_LONG_CLICK_DISABLE_INTERVAL = 0.2f;
        public const float ON_POINTER_DOWN_DISABLE_INTERVAL = 0f;
        public const float ON_POINTER_ENTER_DISABLE_INTERVAL = 0.4f;
        public const float ON_POINTER_EXIT_DISABLE_INTERVAL = 0.4f;
        public const float ON_POINTER_UP_DISABLE_INTERVAL = 0f;

        #endregion

        #region Static Properties

        /// <summary> Returns the default preset category name for an UIButton </summary>
        public static string DefaultPresetCategory { get { return UIAnimations.DEFAULT_DATABASE_NAME; } }

        /// <summary> Returns the default preset name for an UIButton </summary>
        public static string DefaultPresetName { get { return UIAnimations.DEFAULT_PRESET_NAME; } }

        #endregion

        #region Properties

        /// <summary> Returns the number of AnimatorEvents that have been added to the Animator list </summary>
        public int AnimatorsCount { get { return Animators == null ? 0 : Animators.Count; } }

        /// <summary> Returns for what type of behavior the settings and 'actions' are configured for </summary>
        public UIButtonBehaviorType BehaviorType { get { return m_behaviorType; } }

        /// <summary>
        ///     If ButtonAnimationType is either Punch or State, it will return TRUE if at least one animation is enabled or if a preset is set to be loaded at runtime.
        ///     <para />
        ///     If ButtonAnimationType is Animator, it will return TRUE if an AnimatorEvent has been added to the Animators list.
        /// </summary>
        public bool HasAnimation
        {
            get
            {
                switch (ButtonAnimationType)
                {
                    case ButtonAnimationType.Punch:    return HasPunchAnimation;
                    case ButtonAnimationType.State:    return HasStateAnimation;
                    case ButtonAnimationType.Animator: return HasAnimators;
                }

                return false;
            }
        }

        /// <summary> Returns TRUE if ButtonAnimationType.Animator and at least one Animator has been added to the Animators list </summary>
        public bool HasAnimators { get { return ButtonAnimationType == ButtonAnimationType.Animator && AnimatorsCount > 0; } }

        /// <summary> Returns TRUE if the OnTrigger UIAction has at least one AnimatorEvent in the AnimatorEvents list </summary>
        public bool HasAnimatorEvents { get { return OnTrigger.HasAnimatorEvents; } }

        /// <summary> Returns TRUE if the OnTrigger UIAction Effect (UIEffect) has a target ParticleSystem referenced </summary>
        public bool HasEffect { get { return OnTrigger.HasEffect; } }

        /// <summary> Returns TRUE if the OnTrigger UIAction has at least one game event in the GameEvents list </summary>
        public bool HasGameEvents { get { return OnTrigger.HasGameEvents; } }

        /// <summary> Returns TRUE if ButtonAnimationType.Punch and at least one punch animation is enabled or if a preset is set to be loaded at runtime </summary>
        public bool HasPunchAnimation { get { return ButtonAnimationType == ButtonAnimationType.Punch && PunchAnimation != null && (PunchAnimation.Enabled || LoadSelectedPresetAtRuntime); } }

        /// <summary> Returns TRUE if the OnTrigger UIAction has valid sound settings </summary>
        public bool HasSound { get { return OnTrigger.HasSound; } }

        /// <summary> Returns TRUE if ButtonAnimationType.State and at least one state animation is enabled or if a preset is set to be loaded at runtime </summary>
        public bool HasStateAnimation { get { return ButtonAnimationType == ButtonAnimationType.State && StateAnimation != null && (StateAnimation.Enabled || LoadSelectedPresetAtRuntime); } }

        /// <summary> Returns TRUE if the OnTrigger UIAction Event (UnityEvent) has at least one registered persistent listener </summary>
        public bool HasUnityEvents { get { return OnTrigger.HasUnityEvent; } }

        #endregion

        #region Public Variables

        /// <summary> Animator animation settings </summary>
        public List<AnimatorEvent> Animators;

        /// <summary> Determines what type of animation is enabled on this behavior </summary>
        public ButtonAnimationType ButtonAnimationType;

        /// <summary> Determines if the button should get deselected after this behavior has been triggered (works only for OnPointerExit and OnPointerUp) </summary>
        public bool DeselectButton;

        /// <summary> Time interval after this behavior has been fired while it cannot be fired again (works only for OnPointerEnter and OnPointerExit) </summary>
        public float DisableInterval;

        /// <summary> Toggles this behavior </summary>
        public bool Enabled;

        /// <summary> Determines if the selected preset should override at runtime the current editor settings or not </summary>
        public bool LoadSelectedPresetAtRuntime;

        /// <summary> Actions executed when the behavior is triggered </summary>
        public UIAction OnTrigger;

        /// <summary> Preset category name </summary>
        public string PresetCategory;

        /// <summary> Preset name </summary>
        public string PresetName;

        /// <summary> Punch animation settings </summary>
        public UIAnimation PunchAnimation;

        /// <summary> Keeps track if this behavior is ready to get fired again. This is needed if a disable interval has been set </summary>
        public bool Ready;

        /// <summary> Determines if the button should get selected after this behavior has been triggered (works only for OnPointerEnter and OnPointerDown) </summary>
        public bool SelectButton;

        /// <summary> State animation settings </summary>
        public UIAnimation StateAnimation;

        /// <summary> If TRUE, all the events will get fired after the animation finished playing. This is useful if you want to be sure the user sees the button animation </summary>
        public bool TriggerEventsAfterAnimation;

        #endregion

        #region Private Variables

        // ReSharper disable once InconsistentNaming
        /// <summary> Internal variable that keeps track for what type of behavior the settings and 'actions' are configured for </summary>
        [SerializeField] private UIButtonBehaviorType m_behaviorType;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        /// <param name="behaviorType"> Behavior type </param>
        /// <param name="enabled"> Is the behavior enabled? </param>
        public UIButtonBehavior(UIButtonBehaviorType behaviorType, bool enabled = false)
        {
            Reset(behaviorType);
            Enabled = enabled;
        }

        #endregion

        #region Public Methods

        /// <summary> Returns the maximum duration (including startDelay) of the punch or state animation (depending on what ButtonAnimationType is set on this behavior) </summary>
        public float GetAnimationTotalDuration()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ButtonAnimationType)
            {
                case ButtonAnimationType.Punch: return PunchAnimation.TotalDuration;
                case ButtonAnimationType.State: return StateAnimation.TotalDuration;
                default:                        return 0f;
            }
        }

        /// <summary> Loads the selected preset settings </summary>
        public void LoadPreset() { LoadPreset(PresetCategory, PresetName); }

        /// <summary> Loads the preset, with the given category name and preset name, settings </summary>
        /// <param name="presetCategory"> Preset category name </param>
        /// <param name="presetName"> Preset name (found in the presetCategory) </param>
        public void LoadPreset(string presetCategory, string presetName)
        {
            if (ButtonAnimationType == ButtonAnimationType.Animator) return;
            UIAnimationData animationData = UIAnimations.Instance.Get(GetAnimationType(ButtonAnimationType), presetCategory, presetName);
            if (animationData == null) return;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ButtonAnimationType)
            {
                case ButtonAnimationType.Punch:
                    PunchAnimation = animationData.Animation.Copy();
                    break;
                case ButtonAnimationType.State:
                    StateAnimation = animationData.Animation.Copy();
                    break;
            }
        }

        /// <summary> Plays the currently active animation </summary>
        /// <param name="button"> The button reference </param>
        /// <param name="withSound"> If set to <c>true</c> [with sound] </param>
        /// <param name="onStartCallback"> Callback fired when the animation starts playing </param>
        /// <param name="onCompleteCallback"> Callback fired when the animation completed playing </param>
        public void PlayAnimation(UIButton button, bool withSound = true, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ButtonAnimationType)
            {
                case ButtonAnimationType.Punch:
                    if (PunchAnimation == null) return;
                    UIAnimator.StopAnimations(button.RectTransform, AnimationType.Punch);
                    if (PunchAnimation.Move.Enabled) button.ResetPosition();
                    if (PunchAnimation.Rotate.Enabled) button.ResetRotation();
                    if (PunchAnimation.Scale.Enabled) button.ResetScale();
                    UIAnimator.MovePunch(button.RectTransform, PunchAnimation, button.StartPosition);   //play the move punch animation
                    UIAnimator.RotatePunch(button.RectTransform, PunchAnimation, button.StartRotation); //play the rotate punch animation
                    UIAnimator.ScalePunch(button.RectTransform, PunchAnimation, button.StartScale);     //play the scale punch animation
                    Coroutiner.Start(InvokeCallbacks(PunchAnimation, onStartCallback, onCompleteCallback));
                    break;
                case ButtonAnimationType.State:
                    if (StateAnimation == null) return;
                    UIAnimator.StopAnimations(button.RectTransform, AnimationType.State);
                    UIAnimator.MoveState(button.RectTransform, StateAnimation, button.StartPosition);
                    UIAnimator.RotateState(button.RectTransform, StateAnimation, button.StartRotation);
                    UIAnimator.ScaleState(button.RectTransform, StateAnimation, button.StartScale);
                    UIAnimator.FadeState(button.RectTransform, StateAnimation, button.StartAlpha);
                    Coroutiner.Start(InvokeCallbacks(StateAnimation, onStartCallback, onCompleteCallback));
                    break;

                case ButtonAnimationType.Animator:
                    if (Animators == null || Animators.Count == 0) return;
                    foreach (AnimatorEvent animatorEvent in Animators)
                        animatorEvent.Invoke();
                    break;
            }

            if (withSound) OnTrigger.PlaySound();
        }

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="behaviorType"> Behavior type </param>
        public void Reset(UIButtonBehaviorType behaviorType)
        {
            m_behaviorType = behaviorType;
            Enabled = DEFAULT_ENABLED;
            Ready = DEFAULT_READY;
            DisableInterval = GetDefaultDisableInterval(behaviorType);
            SelectButton = DEFAULT_SELECT_BUTTON;
            DeselectButton = DEFAULT_DESELECT_BUTTON;

            ButtonAnimationType = DEFAULT_BUTTON_ANIMATION_TYPE;
            LoadSelectedPresetAtRuntime = DEFAULT_LOAD_SELECTED_PRESET_AT_RUNTIME;

            PresetCategory = DefaultPresetCategory;
            PresetName = DefaultPresetName;

            PunchAnimation = new UIAnimation(AnimationType.Punch);
            StateAnimation = new UIAnimation(AnimationType.State);
            Animators = new List<AnimatorEvent>();

            TriggerEventsAfterAnimation = DEFAULT_TRIGGER_EVENTS_AFTER_ANIMATION;
            OnTrigger = new UIAction();
        }

        #endregion

        #region IEnumerators

        private static IEnumerator InvokeCallbacks(UIAnimation animation, UnityAction onStartCallback, UnityAction onCompleteCallback)
        {
            if (animation == null || !animation.Enabled) yield break;
            yield return new WaitForSecondsRealtime(animation.StartDelay);
            if (onStartCallback != null) onStartCallback.Invoke();
            yield return new WaitForSecondsRealtime(animation.TotalDuration - animation.StartDelay);
            if (onCompleteCallback != null) onCompleteCallback.Invoke();
        }

        #endregion

        #region Static Methods

        /// <summary> Converts an AnimationType to a ButtonAnimationType. This is used to convert Punch and State animations from one enum type to the other </summary>
        /// <param name="type"> Target ButtonAnimationType </param>
        public static AnimationType GetAnimationType(ButtonAnimationType type)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (type)
            {
                case ButtonAnimationType.Punch: return AnimationType.Punch;
                case ButtonAnimationType.State: return AnimationType.State;
                default: return AnimationType.Undefined;
            }
        }

        /// <summary> Returns the default disable interval for the given UIButtonBehaviorType </summary>
        /// <param name="type"> Target UIButtonBehaviorType </param>
        public static float GetDefaultDisableInterval(UIButtonBehaviorType type)
        {
            switch (type)
            {
                case UIButtonBehaviorType.OnClick:        return ON_CLICK_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnDoubleClick:  return ON_DOUBLE_CLICK_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnLongClick:    return ON_LONG_CLICK_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnPointerEnter: return ON_POINTER_ENTER_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnPointerExit:  return ON_POINTER_EXIT_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnPointerDown:  return ON_POINTER_DOWN_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnPointerUp:    return ON_POINTER_UP_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnSelected:     return ON_BUTTON_SELECTED_DISABLE_INTERVAL;
                case UIButtonBehaviorType.OnDeselected:   return ON_BUTTON_DESELECTED_DISABLE_INTERVAL;
                default:                                  return 0f;
            }
        }

        #endregion
    }
}