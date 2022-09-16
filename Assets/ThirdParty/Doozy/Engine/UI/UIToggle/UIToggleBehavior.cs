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
    ///     Contains settings and 'actions' used and triggered by the UIToggle when it changes its state
    /// </summary>
    [Serializable]
    public class UIToggleBehavior
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
        public const float ON_POINTER_ENTER_DISABLE_INTERVAL = 0.4f;
        public const float ON_POINTER_EXIT_DISABLE_INTERVAL = 0.4f;

        #endregion

        #region Static Properties

        /// <summary> Returns the default preset category name for an UIToggle </summary>
        public static string DefaultPresetCategory { get { return UIAnimations.DEFAULT_DATABASE_NAME; } }

        /// <summary> Returns the default preset name for an UIToggle </summary>
        public static string DefaultPresetName { get { return UIAnimations.DEFAULT_PRESET_NAME; } }

        #endregion

        #region Properties

        /// <summary> Returns the number of AnimatorEvents that have been added to the Animator list </summary>
        public int AnimatorsCount { get { return Animators == null ? 0 : Animators.Count; } }

        /// <summary> Returns for what type of behavior the settings and 'actions' are configured for </summary>
        public UIToggleBehaviorType BehaviorType { get { return m_behaviorType; } }

        /// <summary>
        ///     If ButtonAnimationType is either Punch or State, it will return TRUE if at least one animation is enabled or if a preset is set to be loaded at runtime
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

        /// <summary> Returns TRUE if OnToggleOn UIAction or OnToggleOff UIAction have at least one AnimatorEvent in the AnimatorEvents list </summary>
        public bool HasAnimatorEvents { get { return OnToggleOn.HasAnimatorEvents || OnToggleOff.HasAnimatorEvents; } }

        /// <summary> Returns TRUE if OnToggleOn UIAction Effect (UIEffect) or OnToggleOff UIAction Effect (UIEffect) have a target ParticleSystem referenced </summary>
        public bool HasEffect { get { return OnToggleOn.HasEffect || OnToggleOff.HasEffect; } }

        /// <summary> Returns TRUE if OnToggleOn UIAction or OnToggleOff UIAction have at least one game event in the GameEvents list </summary>
        public bool HasGameEvents { get { return OnToggleOn.HasGameEvents || OnToggleOff.HasGameEvents; } }

        /// <summary> Returns TRUE if ButtonAnimationType.Punch and at least one punch animation is enabled or if a preset is set to be loaded at runtime </summary>
        public bool HasPunchAnimation { get { return ButtonAnimationType == ButtonAnimationType.Punch && PunchAnimation != null && (PunchAnimation.Enabled || LoadSelectedPresetAtRuntime); } }

        /// <summary> Returns TRUE if OnToggleOn UIAction or OnToggleOff UIAction have valid sound settings </summary>
        public bool HasSound { get { return OnToggleOn.HasSound || OnToggleOff.HasSound; } }

        /// <summary> Returns TRUE if ButtonAnimationType.State and at least one state animation is enabled or if a preset is set to be loaded at runtime </summary>
        public bool HasStateAnimation { get { return ButtonAnimationType == ButtonAnimationType.State && StateAnimation != null && (StateAnimation.Enabled || LoadSelectedPresetAtRuntime); } }

        /// <summary> Returns TRUE if OnToggleOn UIAction Event (UnityEvent) or OnToggleOff UIAction Event (UnityEvent) have  at least one registered persistent listener </summary>
        public bool HasUnityEvents { get { return OnToggleOn.HasUnityEvent || OnToggleOff.HasUnityEvent; } }

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

        /// <summary> Actions performed when the behavior is triggered and the toggle is off </summary>
        public UIAction OnToggleOff;

        /// <summary> Actions performed when the behavior is triggered and the toggle is on </summary>
        public UIAction OnToggleOn;

        /// <summary> Preset category name </summary>
        public string PresetCategory;

        /// <summary> Preset name </summary>
        public string PresetName;

        // <summary> Punch animation settings </summary>
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
        [SerializeField] private UIToggleBehaviorType m_behaviorType;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        /// <param name="behaviorType"> Behavior type </param>
        /// <param name="enabled"> Is the behavior enabled? </param>
        public UIToggleBehavior(UIToggleBehaviorType behaviorType, bool enabled = false)
        {
            Reset(behaviorType);
            Enabled = enabled;
        }

        #endregion

        #region Public Methods

        /// <summary> Returns the maximum duration (including startDelay) of the punch or state animation (depending on what AnimationType is set on this behavior) </summary>
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

        /// <summary> Triggers this behavior by executing its actions </summary>
        /// <param name="toggle"> UIToggle that triggered this behavior </param>
        /// <param name="playAnimation"> Play the set animation </param>
        /// <param name="playSound"> Play the set sound </param>
        /// <param name="executeEffect"> Run the effect </param>
        /// <param name="executeAnimatorEvents"> Trigger all the animator events </param>
        /// <param name="sendGameEvents"> Send all the game events </param>
        /// <param name="executeUnityEvent"> Execute the Action and the UnityEvent </param>
        public void Invoke(UIToggle toggle, bool playAnimation = true, bool playSound = true, bool executeEffect = true, bool executeAnimatorEvents = true, bool sendGameEvents = true, bool executeUnityEvent = true)
        {
            if (toggle == null) return;
            UIAction uiAction = toggle.IsOn ? OnToggleOn : OnToggleOff;

            if (playAnimation) PlayAnimation(toggle, false);                                  //Animation
            if (playSound) uiAction.PlaySound();                                              //Sound
            if (executeEffect) uiAction.ExecuteEffect(uiAction.GetCanvas(toggle.gameObject)); //Effect
            if (executeAnimatorEvents) uiAction.InvokeAnimatorEvents();                       //Animator Events
            if (!sendGameEvents && !executeUnityEvent) return;
            if (!TriggerEventsAfterAnimation)
            {
                if (sendGameEvents) uiAction.SendGameEvents(toggle.gameObject); //Game Events
                if (!executeUnityEvent) return;
                uiAction.InvokeAction(toggle.gameObject); //Action
                uiAction.InvokeUnityEvent();              //UnityEvent
            }
            else
            {
                Coroutiner.Start(InvokeCallbackAfterDelay(() =>
                                                          {
                                                              if (toggle == null) return;
                                                              if (sendGameEvents) uiAction.SendGameEvents(toggle.gameObject); //Game Events
                                                              if (!executeUnityEvent) return;
                                                              uiAction.InvokeAction(toggle.gameObject); //Action
                                                              uiAction.InvokeUnityEvent();              //UnityEvent
                                                          },
                                                          GetAnimationTotalDuration()));
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
        /// <param name="toggle"> The toggle reference </param>
        /// <param name="withSound"> If set to <c>true</c> [with sound] </param>
        /// <param name="onStartCallback"> Callback fired when the animation starts playing </param>
        /// <param name="onCompleteCallback"> Callback fired when the animation completed playing </param>
        public void PlayAnimation(UIToggle toggle, bool withSound = true, UnityAction onStartCallback = null, UnityAction onCompleteCallback = null)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (ButtonAnimationType)
            {
                case ButtonAnimationType.Punch:
                    if (PunchAnimation == null) return;
                    UIAnimator.StopAnimations(toggle.RectTransform, AnimationType.Punch);
                    if (PunchAnimation.Move.Enabled) toggle.ResetPosition();
                    if (PunchAnimation.Rotate.Enabled) toggle.ResetRotation();
                    if (PunchAnimation.Scale.Enabled) toggle.ResetScale();
                    UIAnimator.MovePunch(toggle.RectTransform, PunchAnimation, toggle.StartPosition);   //play the move punch animation
                    UIAnimator.RotatePunch(toggle.RectTransform, PunchAnimation, toggle.StartRotation); //play the rotate punch animation
                    UIAnimator.ScalePunch(toggle.RectTransform, PunchAnimation, toggle.StartScale);     //play the scale punch animation
                    Coroutiner.Start(InvokeCallbacks(PunchAnimation, onStartCallback, onCompleteCallback));
                    break;
                case ButtonAnimationType.State:
                    if (StateAnimation == null) return;
                    UIAnimator.StopAnimations(toggle.RectTransform, AnimationType.State);
                    UIAnimator.MoveState(toggle.RectTransform, StateAnimation, toggle.StartPosition);
                    UIAnimator.RotateState(toggle.RectTransform, StateAnimation, toggle.StartRotation);
                    UIAnimator.ScaleState(toggle.RectTransform, StateAnimation, toggle.StartScale);
                    UIAnimator.FadeState(toggle.RectTransform, StateAnimation, toggle.StartAlpha);
                    Coroutiner.Start(InvokeCallbacks(StateAnimation, onStartCallback, onCompleteCallback));
                    break;

                case ButtonAnimationType.Animator:
                    if (Animators == null || Animators.Count == 0) return;
                    foreach (AnimatorEvent animatorEvent in Animators)
                        animatorEvent.Invoke();
                    break;
            }

            if (!withSound) return;
            if (toggle.IsOn) OnToggleOn.PlaySound();
            else OnToggleOff.PlaySound();
        }

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="behaviorType"> Behavior type </param>
        public void Reset(UIToggleBehaviorType behaviorType)
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

            OnToggleOn = new UIAction();
            OnToggleOff = new UIAction();
        }

        #endregion

        #region IEnumerators

        private static IEnumerator InvokeCallbacks(UIAnimation animation, UnityAction onStartCallback, UnityAction onCompleteCallback)
        {
            if (animation == null || !animation.Enabled) yield break;
            yield return new WaitForSecondsRealtime(animation.StartDelay); //wait for seconds realtime (ignore Unity's Time.Timescale)
            if (onStartCallback != null) onStartCallback.Invoke();
            yield return new WaitForSecondsRealtime(animation.TotalDuration - animation.StartDelay); //wait for seconds realtime (ignore Unity's Time.Timescale)
            if (onCompleteCallback != null) onCompleteCallback.Invoke();
        }

        private static IEnumerator InvokeCallbackAfterDelay(UnityAction callback, float delay)
        {
            if (callback == null) yield break;
            if (delay <= 0)
            {
                callback.Invoke();
                yield break;
            }

            yield return new WaitForSecondsRealtime(delay); //wait for seconds realtime (ignore Unity's Time.Timescale)
            callback.Invoke();
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
                default:                        return AnimationType.Undefined;
            }
        }

        /// <summary> Returns the default disable interval for the given UIToggleBehaviorType </summary>
        /// <param name="type"> Target UIToggleBehaviorType </param>
        public static float GetDefaultDisableInterval(UIToggleBehaviorType type)
        {
            switch (type)
            {
                case UIToggleBehaviorType.OnClick:        return ON_CLICK_DISABLE_INTERVAL;
                case UIToggleBehaviorType.OnPointerEnter: return ON_POINTER_ENTER_DISABLE_INTERVAL;
                case UIToggleBehaviorType.OnPointerExit:  return ON_POINTER_EXIT_DISABLE_INTERVAL;
                case UIToggleBehaviorType.OnSelected:     return ON_BUTTON_SELECTED_DISABLE_INTERVAL;
                case UIToggleBehaviorType.OnDeselected:   return ON_BUTTON_DESELECTED_DISABLE_INTERVAL;
                default:                                  return 0f;
            }
        }

        #endregion
    }
}