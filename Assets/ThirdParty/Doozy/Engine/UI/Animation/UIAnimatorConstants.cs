// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

using DG.Tweening;
using Doozy.Engine.Settings;
using UnityEngine;

namespace Doozy.Engine.UI.Animation
{
    public static partial class UIAnimator
    {
        private static DoozySettings Settings { get { return DoozySettings.Instance; } }

        public static Vector3 DEFAULT_START_POSITION = Vector3.zero;
        public static Vector3 DEFAULT_START_ROTATION = Vector3.zero;
        public static Vector3 DEFAULT_START_SCALE = Vector3.one;
        public const float DEFAULT_START_ALPHA = 1f;
        
        /// <summary> Default animation enabled state set to an animation </summary>
        public const bool DefaultAnimationEnabledState = false;

        /// <summary> Default direction set to a move animation </summary>
        public const Direction DefaultDirection = Direction.Left;

        /// <summary> Default rotate mode set to an animation </summary>
        public const RotateMode DefaultRotateMode = RotateMode.FastBeyond360;

        /// <summary> Default loop type set to an animation </summary>
        public const LoopType DefaultLoopType = LoopType.Yoyo;

        /// <summary> Default ease type set to an animation </summary>
        public const EaseType DefaultEaseType = EaseType.Ease;

        /// <summary> Default ease set to an animation </summary>
        public const Ease DefaultEase = Ease.Linear;

        /// <summary> Default duration set to an animation </summary>
        public const float DefaultDuration = 1f;

        /// <summary> Default start delay duration set to an animation </summary>
        public const float DefaultStartDelay = 0f;

        /// <summary> Default loops set to a loop animation (-1 means infinite loops) </summary>
        public const int DefaultNumberOfLoops = -1;

        /// <summary> Default reset duration after a punch animation. This reset is needed to be sure the animation's initial values are restored </summary>
        public const float DefaultDurationOnComplete = 0.05f;

        /// <summary> Default loop setup duration. This is the time a loop animation is setup for its cycle to start </summary>
        public const float DefaultDurationInitLoop = 0.2f;

        /// <summary> Default target reset. This is the time a 'target' (RectTransfrom) is reset to its start values (runtime values) </summary>
        public const float DefaultDurationResetTarget = 0.1f;

        /// <summary> The default vibrato value. Used by the punch animations </summary>
        public const int DefaultVibrato = 10;

        /// <summary> The default elasticity value. Used by the punch animations </summary>
        public const float DefaultElasticity = 1;
    }
}