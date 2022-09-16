// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Animation settings for the UIButton loop animations (NormalLoopAnimation and SelectedLoopAnimation)
    /// </summary>
    [Serializable]
    public class UIButtonLoopAnimation
    {
        #region Public Variables

        /// <summary> Loop animation settings </summary>
        public UIAnimation Animation;

        /// <summary> Toggles this behavior </summary>
        public bool Enabled;

        /// <summary> Keeps track if this animation is currently playing or not </summary>
        public bool IsPlaying;

        /// <summary> Determines what type of loop animation this is (Normal or Selected) </summary>
        public ButtonLoopAnimationType LoopAnimationType;

        /// <summary> Determines if the selected preset should override at runtime the current editor settings or not </summary>
        public bool LoadSelectedPresetAtRuntime;

        /// <summary> Preset category name </summary>
        public string PresetCategory;

        /// <summary> Preset name </summary>
        public string PresetName;

        #endregion

        #region Constructor

        /// <summary> Initializes a new instance of the class </summary>
        /// <param name="loopAnimationType"> The loop animation type </param>
        public UIButtonLoopAnimation(ButtonLoopAnimationType loopAnimationType) { Reset(loopAnimationType); }

        #endregion

        #region Public Methods

        /// <summary> Loads the selected preset settings </summary>
        public void LoadPreset()
        {
            UIAnimationData data = UIAnimations.Instance.Get(AnimationType.Loop, PresetCategory, PresetName);
            if (data == null) return;
            Animation = data.Animation.Copy();
        }

        /// <summary> Loads the preset, with the given category name and preset name, settings </summary>
        /// <param name="presetCategory"> Preset category name </param>
        /// <param name="presetName"> Preset nam </param>
        public void LoadPreset(string presetCategory, string presetName)
        {
            UIAnimationData data = UIAnimations.Instance.Get(AnimationType.Loop, presetCategory, presetName);
            if (data == null) return;
            Animation = data.Animation.Copy();
        }

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="loopAnimationType"> The loop animation type </param>
        public void Reset(ButtonLoopAnimationType loopAnimationType)
        {
            LoopAnimationType = loopAnimationType;
            Animation = new UIAnimation(AnimationType.Loop);
            IsPlaying = false;
        }

        /// <summary> Starts playing the loop animation on the target RectTransform </summary>
        /// <param name="target"> Target RectTransform being animated </param>
        /// <param name="startPosition"> Start anchoredPosition3D for the target RectTransform</param>
        /// <param name="startRotation"> Start localRotation for the target RectTransform</param>
        public void Start(RectTransform target, Vector3 startPosition, Vector3 startRotation)
        {
            if (!Enabled) return;
            if (Animation == null) return;
            if (IsPlaying) return;
            UIAnimator.MoveLoop(target, Animation, startPosition);
            UIAnimator.RotateLoop(target, Animation, startRotation);
            UIAnimator.ScaleLoop(target, Animation);
            UIAnimator.FadeLoop(target, Animation);
            IsPlaying = true;
        }

        /// <summary> Stops playing the loop animation on the target RectTransform </summary>
        /// <param name="target"></param>
        public void Stop(RectTransform target)
        {
            if (Animation == null) return;
            if (!IsPlaying) return;
            UIAnimator.StopAnimations(target, AnimationType.Loop);
            IsPlaying = false;
        }

        #endregion
    }
}