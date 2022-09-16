// Copyright (c) 2015 - 22019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Contains settings and 'actions' used and triggered by the UIView when it changes its state
    /// </summary>>
    [Serializable]
    public class UIViewBehavior
    {
        #region Constants

        public const bool DEFAULT_INSTANT_ANIMATION = false;
        public const bool DEFAULT_LOAD_SELECTED_PRESET_AT_RUNTIME = false;
        public const bool DEFAULT_AUTO_START_LOOP_ANIMATION = true;

        #endregion

        #region Static Properties

        /// <summary> Returns the default preset category name for an UIView </summary>
        public static string DefaultPresetCategory { get { return UIAnimations.DEFAULT_DATABASE_NAME; } }

        /// <summary> Returns the default preset name for an UIView </summary>
        public static string DefaultPresetName { get { return UIAnimations.DEFAULT_PRESET_NAME; } }

        #endregion

        #region Properties

        /// <summary> Returns TRUE if either UIAnimation Animation is enabled or LoadSelectedPresetAtRuntime is true </summary>
        public bool HasAnimation { get { return Animation != null && (Animation.Enabled || LoadSelectedPresetAtRuntime); } }

        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have at least one AnimatorEvent in their AnimatorEvents list </summary>
        public bool HasAnimatorEvents { get { return OnStart.HasAnimatorEvents || OnFinished.HasAnimatorEvents; } }

        /// <summary> Returns TRUE if either OnStart UIAction Effect (UIEffect) or OnFinished UIAction Effect (UIEffect) have a target ParticleSystem referenced </summary>
        public bool HasEffect { get { return OnStart.HasEffect || OnFinished.HasEffect; } }

        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have at least one game event in their GameEvents list </summary>
        public bool HasGameEvents { get { return OnStart.HasGameEvents || OnFinished.HasGameEvents; } }

        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have valid sound settings </summary>
        public bool HasSound { get { return OnStart.HasSound || OnFinished.HasSound; } }

        /// <summary> Returns TRUE if either OnStart UIAction Event (UnityEvent) or OnFinished UIAction Event (UnityEvent) have at least one registered persistent listener </summary>
        public bool HasUnityEvents { get { return OnStart.HasUnityEvent || OnFinished.HasUnityEvent; } }

        #endregion

        #region Public Variables

        /// <summary> Animation settings </summary>
        public UIAnimation Animation;

        /// <summary>
        ///     Automatically starts the loop animation without waiting for a show (in) animation to finish playing.
        ///     <para />
        ///     Available only for the Loop behavior.
        /// </summary>
        public bool AutoStartLoopAnimation = DEFAULT_AUTO_START_LOOP_ANIMATION;

        /// <summary> Determines if the selected preset should override, at runtime, the current editor settings or not </summary>
        public bool LoadSelectedPresetAtRuntime = DEFAULT_LOAD_SELECTED_PRESET_AT_RUNTIME;

        /// <summary> Determines if this animation should happen instantly (in zero seconds) </summary>
        public bool InstantAnimation = DEFAULT_INSTANT_ANIMATION;
        
        /// <summary> Actions performed when the animations finished playing </summary>
        public UIAction OnFinished;

        /// <summary> Actions performed when the animations start playing </summary>
        public UIAction OnStart;

        /// <summary> Preset category name </summary>
        public string PresetCategory = DefaultPresetCategory;

        /// <summary> Preset name </summary>
        public string PresetName = DefaultPresetName;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that keeps track of the animation progress (float between 0 and 1) </summary>
        private float m_progress;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        /// <param name="animationType"> AnimationType for the UIAnimation Animation </param>
        public UIViewBehavior(AnimationType animationType) { Reset(animationType); }

        #endregion

        #region Public Methods

        /// <summary> Loads the selected preset settings </summary>
        public void LoadPreset() { LoadPreset(PresetCategory, PresetName); }

        /// <summary> Loads the preset, with the passed category name and preset name, settings </summary>
        /// <param name="presetCategory"> Preset category name </param>
        /// <param name="presetName"> Preset name (found in the presetCategory) </param>
        public void LoadPreset(string presetCategory, string presetName)
        {
            UIAnimationData data = UIAnimations.Instance.Get(Animation.AnimationType, presetCategory, presetName);
            if (data == null) return;
            Animation = data.Animation.Copy();
        }

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> AnimationType for the UIAnimation Animation </param>
        public void Reset(AnimationType animationType)
        {
            AutoStartLoopAnimation = DEFAULT_AUTO_START_LOOP_ANIMATION;
            LoadSelectedPresetAtRuntime = DEFAULT_LOAD_SELECTED_PRESET_AT_RUNTIME;
            PresetCategory = DefaultPresetCategory;
            PresetName = DefaultPresetName;
            Animation = new UIAnimation(animationType);
            OnStart = new UIAction();
            OnFinished = new UIAction();
        }

        #endregion
    }
}