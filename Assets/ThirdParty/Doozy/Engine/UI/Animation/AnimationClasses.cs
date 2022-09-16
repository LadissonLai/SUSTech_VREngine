// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using DG.Tweening;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Animation
{
    /// <summary> Move animation settings </summary>
    [Serializable]
    public class Move
    {
        #region Properties

        /// <summary> Returns the maximum duration (including StartDelay) of the animation </summary>
        public float TotalDuration
        {
            get { return StartDelay + Duration; }
        }

        #endregion

        #region Public Variables

        /// <summary>
        ///     The animation type that determines the behavior of this animation
        /// </summary>
        public AnimationType AnimationType = AnimationType.Undefined;

        /// <summary> Determines if this animation is enabled or not </summary>
        public bool Enabled;

        /// <summary> Start value for the animation </summary>
        public Vector3 From;

        /// <summary> End value for the animation </summary>
        public Vector3 To;

        /// <summary> By value for the animation (used to perform relative changes to the current values or punch animations) </summary>
        public Vector3 By;

        /// <summary> Allows the usage of custom from and to values for the Move animation </summary>
        public bool UseCustomFromAndTo;

        /// <summary>
        ///     The vibrato indicates how much will a punch animation will vibrate
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public int Vibrato;

        /// <summary>
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting position when bouncing backwards after a punch
        ///     <para> 1 creates a full oscillation between the punch direction and the opposite direction, while 0 oscillates only between the punch and the start position </para>
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public float Elasticity;

        /// <summary>
        ///     The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public int NumberOfLoops;

        /// <summary>
        ///     The loop type
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public LoopType LoopType;

        /// <summary> The direction this animation is set to </summary>
        public Direction Direction;

        /// <summary> Custom value used only when Direction is set to CustomPosition </summary>
        public Vector3 CustomPosition;

        /// <summary> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </summary>
        public EaseType EaseType;

        /// <summary> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if easeType is set to EaseType.Ease </summary>
        public Ease Ease;

        /// <summary> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </summary>
        public AnimationCurve AnimationCurve;

        /// <summary> Start delay duration for the animation </summary>
        public float StartDelay;

        /// <summary> Length of time for the animation (does not include the StartDelay) </summary>
        public float Duration;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="Move" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public Move(AnimationType animationType) { Reset(animationType); }


        /// <inheritdoc />
        /// <summary> Initializes a new instance of the <see cref="Move" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        /// <param name="enabled"> Determines if this animation is enabled or not </param>
        /// <param name="from"> Start value for the animation </param>
        /// <param name="to"> End value for the animation </param>
        /// <param name="by"> By value for the animation (used to perform relative changes to the current values) </param>
        /// <param name="useCustomFromAndTo"> Allows the usage of custom from and to values for the Move animation </param>
        /// <param name="vibrato"> The vibrato indicates how much will a punch animation will vibrate (used only by punch animations)</param>
        /// <param name="elasticity">
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting position when bouncing backwards after a punch. 1 creates a full oscillation between the punch direction and the opposite
        ///     direction, while 0 oscillates only between the punch and the start position (used only by punch animations)
        /// </param>
        /// <param name="numberOfLoops"> The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops </param>
        /// <param name="loopType"> The loop type </param>
        /// <param name="direction"> The direction </param>
        /// <param name="customPosition"> Custom value used only when Direction is set to CustomPosition </param>
        /// <param name="easeType"> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </param>
        /// <param name="ease"> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if easeType is set to EaseType.Ease </param>
        /// <param name="animationCurve"> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if easeType is set to EaseType.AnimationCurve </param>
        /// <param name="startDelay"> Start delay duration for the animation </param>
        /// <param name="duration"> Length of time for the animation </param>
        public Move(AnimationType animationType,
                    bool enabled,
                    Vector3 from, Vector3 to, Vector3 by,
                    bool useCustomFromAndTo,
                    int vibrato, float elasticity,
                    int numberOfLoops, LoopType loopType,
                    Direction direction,
                    Vector3 customPosition,
                    EaseType easeType, Ease ease, AnimationCurve animationCurve,
                    float startDelay, float duration) : this(animationType)
        {
            AnimationType = animationType;
            Enabled = enabled;
            From = from;
            To = to;
            By = by;
            UseCustomFromAndTo = useCustomFromAndTo;
            Vibrato = vibrato;
            Elasticity = elasticity;
            NumberOfLoops = numberOfLoops;
            LoopType = loopType;
            Direction = direction;
            CustomPosition = customPosition;
            EaseType = easeType;
            Ease = ease;
            AnimationCurve = new AnimationCurve(animationCurve.keys);
            StartDelay = startDelay;
            Duration = duration;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public void Reset(AnimationType animationType)
        {
            AnimationType = animationType;
            Enabled = UIAnimator.DefaultAnimationEnabledState;
            From = Vector3.zero;
            To = Vector3.zero;
            By = Vector3.zero;
            UseCustomFromAndTo = false;
            Vibrato = UIAnimator.DefaultVibrato;
            Elasticity = UIAnimator.DefaultElasticity;
            NumberOfLoops = UIAnimator.DefaultNumberOfLoops;
            LoopType = UIAnimator.DefaultLoopType;
            Direction = UIAnimator.DefaultDirection;
            CustomPosition = Vector3.zero;
            EaseType = UIAnimator.DefaultEaseType;
            Ease = UIAnimator.DefaultEase;
            AnimationCurve = new AnimationCurve();
            StartDelay = UIAnimator.DefaultStartDelay;
            Duration = UIAnimator.DefaultDuration;
        }

        /// <summary> Returns a deep copy </summary>
        public Move Copy()
        {
            return new Move(AnimationType)
                   {
                       AnimationType = AnimationType,
                       Enabled = Enabled,
                       From = From,
                       To = To,
                       By = By,
                       UseCustomFromAndTo = UseCustomFromAndTo,
                       Vibrato = Vibrato,
                       Elasticity = Elasticity,
                       NumberOfLoops = NumberOfLoops,
                       LoopType = LoopType,
                       Direction = Direction,
                       CustomPosition = CustomPosition,
                       EaseType = EaseType,
                       Ease = Ease,
                       AnimationCurve = new AnimationCurve(AnimationCurve.keys),
                       StartDelay = StartDelay,
                       Duration = Duration
                   };
        }

        #endregion
    }

    /// <summary> Rotate animation settings </summary>
    [Serializable]
    public class Rotate
    {
        #region Properties

        /// <summary> Returns the maximum duration (including StartDelay) of the animation </summary>
        public float TotalDuration
        {
            get { return StartDelay + Duration; }
        }

        #endregion

        #region Public Variables

        /// <summary>
        ///     The animation type that determines the behavior of this animation
        /// </summary>
        public AnimationType AnimationType = AnimationType.Undefined;

        /// <summary> Determines if this animation is enabled or not </summary>
        public bool Enabled;

        /// <summary> Start value for the animation </summary>
        public Vector3 From;

        /// <summary> End value for the animation </summary>
        public Vector3 To;

        /// <summary> By value for the animation (used to perform relative changes to the current values) </summary>
        public Vector3 By;

        /// <summary> Allows the usage of custom from and to values for the Rotate animation </summary>
        public bool UseCustomFromAndTo;

        /// <summary>
        ///     The vibrato indicates how much will a punch animation will vibrate
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public int Vibrato;

        /// <summary>
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting rotation when bouncing backwards after a punch
        ///     <para> 1 creates a full oscillation between the punch direction and the opposite direction, while 0 oscillates only between the punch and the start rotation </para>
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public float Elasticity;

        /// <summary>
        ///     The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public int NumberOfLoops;

        /// <summary>
        ///     The loop type
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public LoopType LoopType;

        /// <summary> Rotation mode used with DORotate methods </summary>
        public RotateMode RotateMode;

        /// <summary> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </summary>
        public EaseType EaseType;

        /// <summary> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </summary>
        public Ease Ease;

        /// <summary> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </summary>
        public AnimationCurve AnimationCurve;

        /// <summary> Start delay duration for the animation </summary>
        public float StartDelay;

        /// <summary> Length of time for the animation (does not include the StartDelay) </summary>
        public float Duration;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="Rotate" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public Rotate(AnimationType animationType) { Reset(animationType); }

        /// <inheritdoc />
        /// <summary> Initializes a new instance of the <see cref="Rotate" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        /// <param name="enabled"> Determines if this animation is enabled or not </param>
        /// <param name="from"> Start value for the animation </param>
        /// <param name="to"> End value for the animation </param>
        /// <param name="by"> By value for the animation (used to perform relative changes to the current values) </param>
        /// <param name="useCustomFromAndTo"> Allows the usage of custom from and to values for the Rotate animation </param>
        /// <param name="vibrato"> The vibrato indicates how much will a punch animation will vibrate (used only by punch animations)</param>
        /// <param name="elasticity">
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting rotation when bouncing backwards after a punch. 1 creates a full oscillation between the punch direction and the opposite
        ///     direction, while 0 oscillates only between the punch and the start rotation (used only by punch animations)
        /// </param>
        /// <param name="numberOfLoops"> The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops </param>
        /// <param name="loopType"> The loop type </param>
        /// <param name="rotateMode"> Rotation mode used with DORotate methods </param>
        /// <param name="easeType"> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </param>
        /// <param name="ease"> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </param>
        /// <param name="animationCurve"> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </param>
        /// <param name="startDelay"> Start delay duration for the animation </param>
        /// <param name="duration"> Length of time for the animation </param>
        public Rotate(AnimationType animationType,
                      bool enabled,
                      Vector3 from, Vector3 to, Vector3 by,
                      bool useCustomFromAndTo,
                      int vibrato, float elasticity,
                      int numberOfLoops, LoopType loopType, RotateMode rotateMode,
                      EaseType easeType, Ease ease, AnimationCurve animationCurve,
                      float startDelay, float duration) : this(animationType)
        {
            AnimationType = animationType;
            Enabled = enabled;
            From = from;
            To = to;
            By = by;
            UseCustomFromAndTo = useCustomFromAndTo;
            Vibrato = vibrato;
            Elasticity = elasticity;
            NumberOfLoops = numberOfLoops;
            LoopType = loopType;
            RotateMode = rotateMode;
            EaseType = easeType;
            Ease = ease;
            AnimationCurve = new AnimationCurve(animationCurve.keys);
            StartDelay = startDelay;
            Duration = duration;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public void Reset(AnimationType animationType)
        {
            AnimationType = animationType;
            Enabled = UIAnimator.DefaultAnimationEnabledState;
            From = Vector3.zero;
            To = Vector3.zero;
            By = Vector3.zero;
            UseCustomFromAndTo = false;
            Vibrato = UIAnimator.DefaultVibrato;
            Elasticity = UIAnimator.DefaultElasticity;
            NumberOfLoops = UIAnimator.DefaultNumberOfLoops;
            LoopType = UIAnimator.DefaultLoopType;
            RotateMode = UIAnimator.DefaultRotateMode;
            EaseType = UIAnimator.DefaultEaseType;
            Ease = UIAnimator.DefaultEase;
            AnimationCurve = new AnimationCurve();
            StartDelay = UIAnimator.DefaultStartDelay;
            Duration = UIAnimator.DefaultDuration;
        }

        /// <summary> Returns a deep copy </summary>
        public Rotate Copy()
        {
            return new Rotate(AnimationType)
                   {
                       AnimationType = AnimationType,
                       Enabled = Enabled,
                       From = From,
                       To = To,
                       By = By,
                       UseCustomFromAndTo = UseCustomFromAndTo,
                       Vibrato = Vibrato,
                       Elasticity = Elasticity,
                       NumberOfLoops = NumberOfLoops,
                       LoopType = LoopType,
                       RotateMode = RotateMode,
                       EaseType = EaseType,
                       Ease = Ease,
                       AnimationCurve = new AnimationCurve(AnimationCurve.keys),
                       StartDelay = StartDelay,
                       Duration = Duration
                   };
        }

        #endregion
    }

    /// <summary> Scale animation settings </summary>
    [Serializable]
    public class Scale
    {
        #region Properties

        /// <summary> Returns the maximum duration (including StartDelay) of the animation </summary>
        public float TotalDuration
        {
            get { return StartDelay + Duration; }
        }

        #endregion

        #region Public Variables

        /// <summary>
        ///     The animation type that determines the behavior of this animation
        /// </summary>
        public AnimationType AnimationType = AnimationType.Undefined;

        /// <summary> Determines if this animation is enabled or not </summary>
        public bool Enabled;

        /// <summary> Start value for the animation </summary>
        public Vector3 From;

        /// <summary> End value for the animation </summary>
        public Vector3 To;

        /// <summary> By value for the animation (used to perform relative changes to the current values) </summary>
        public Vector3 By;

        /// <summary> Allows the usage of custom from and to values for the Scale animation </summary>
        public bool UseCustomFromAndTo;

        /// <summary>
        ///     The vibrato indicates how much will a punch animation will vibrate
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public int Vibrato;

        /// <summary>
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting scale when bouncing backwards after a punch
        ///     <para> 1 creates a full oscillation between the punch direction and the opposite direction, while 0 oscillates only between the punch and the start scale </para>
        ///     <para> (used only by punch animations) </para>
        /// </summary>
        public float Elasticity;

        /// <summary>
        ///     The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public int NumberOfLoops;

        /// <summary>
        ///     The loop type
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public LoopType LoopType;

        /// <summary> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </summary>
        public EaseType EaseType;

        /// <summary> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </summary>
        public Ease Ease;

        /// <summary> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </summary>
        public AnimationCurve AnimationCurve;

        /// <summary> Start delay duration for the animation </summary>
        public float StartDelay;

        /// <summary> Length of time for the animation (does not include the StartDelay) </summary>
        public float Duration;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="Scale" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public Scale(AnimationType animationType) { Reset(animationType); }

        /// <summary> Initializes a new instance of the <see cref="Scale" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        /// <param name="enabled"> Determines if this animation is enabled or not </param>
        /// <param name="from"> Start value for the animation </param>
        /// <param name="to"> End value for the animation </param>
        /// <param name="by"> By value for the animation (used to perform relative changes to the current values) </param>
        /// <param name="useCustomFromAndTo"> Allows the usage of custom from and to values for the Scale animation </param>
        /// <param name="vibrato"> The vibrato indicates how much will a punch animation will vibrate (used only by punch animations)</param>
        /// <param name="elasticity">
        ///     The elasticity represents how much (0 to 1) the vector will go beyond the starting scale when bouncing backwards after a punch. 1 creates a full oscillation between the punch direction and the opposite
        ///     direction, while 0 oscillates only between the punch and the start scale (used only by punch animations)
        /// </param>
        /// <param name="numberOfLoops"> The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops </param>
        /// <param name="loopType"> The loop type </param>
        /// <param name="easeType"> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </param>
        /// <param name="ease"> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </param>
        /// <param name="animationCurve"> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </param>
        /// <param name="startDelay"> Start delay duration for the animation </param>
        /// <param name="duration"> Length of time for the animation </param>
        public Scale(AnimationType animationType,
                     bool enabled,
                     Vector3 from, Vector3 to, Vector3 by,
                     bool useCustomFromAndTo,
                     int vibrato, float elasticity,
                     int numberOfLoops, LoopType loopType,
                     EaseType easeType, Ease ease, AnimationCurve animationCurve,
                     float startDelay, float duration) : this(animationType)
        {
            AnimationType = animationType;
            Enabled = enabled;
            From = from;
            To = to;
            By = by;
            UseCustomFromAndTo = useCustomFromAndTo;
            Vibrato = vibrato;
            Elasticity = elasticity;
            NumberOfLoops = numberOfLoops;
            LoopType = loopType;
            EaseType = easeType;
            Ease = ease;
            AnimationCurve = new AnimationCurve(animationCurve.keys);
            StartDelay = startDelay;
            Duration = duration;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public void Reset(AnimationType animationType)
        {
            AnimationType = animationType;
            Enabled = UIAnimator.DefaultAnimationEnabledState;
            From = Vector3.zero;
            To = Vector3.zero;
            By = Vector3.zero;
            UseCustomFromAndTo = false;
            Vibrato = UIAnimator.DefaultVibrato;
            Elasticity = UIAnimator.DefaultElasticity;
            NumberOfLoops = UIAnimator.DefaultNumberOfLoops;
            LoopType = UIAnimator.DefaultLoopType;
            EaseType = UIAnimator.DefaultEaseType;
            Ease = UIAnimator.DefaultEase;
            AnimationCurve = new AnimationCurve();
            StartDelay = UIAnimator.DefaultStartDelay;
            Duration = UIAnimator.DefaultDuration;
        }

        /// <summary> Returns a deep copy </summary>
        public Scale Copy()
        {
            return new Scale(AnimationType)
                   {
                       AnimationType = AnimationType,
                       Enabled = Enabled,
                       From = From,
                       To = To,
                       By = By,
                       UseCustomFromAndTo = UseCustomFromAndTo,
                       Vibrato = Vibrato,
                       Elasticity = Elasticity,
                       NumberOfLoops = NumberOfLoops,
                       LoopType = LoopType,
                       EaseType = EaseType,
                       Ease = Ease,
                       AnimationCurve = new AnimationCurve(AnimationCurve.keys),
                       StartDelay = StartDelay,
                       Duration = Duration
                   };
        }

        #endregion
    }

    /// <summary> Fade animation settings </summary>
    [Serializable]
    public class Fade
    {
        #region Properties

        /// <summary> Returns the maximum duration (including StartDelay) of the animation </summary>
        public float TotalDuration
        {
            get { return StartDelay + Duration; }
        }

        #endregion

        #region Public Variables

        /// <summary>
        ///     The animation type that determines the behavior of this animation
        /// </summary>
        public AnimationType AnimationType = AnimationType.Undefined;

        /// <summary> Determines if this animation is enabled or not </summary>
        public bool Enabled;

        /// <summary> Start value for the animation </summary>
        public float From;

        /// <summary> End value for the animation </summary>
        public float To;

        /// <summary> By value for the animation (used to perform relative changes to the current values) </summary>
        public float By;

        /// <summary> Allows the usage of custom from and to values for the Fade animation </summary>
        public bool UseCustomFromAndTo;

        /// <summary>
        ///     The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public int NumberOfLoops;

        /// <summary>
        ///     The loop type
        ///     <para> (used only by loop animations) </para>
        /// </summary>
        public LoopType LoopType;

        /// <summary> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </summary>
        public EaseType EaseType;

        /// <summary> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </summary>
        public Ease Ease;

        /// <summary> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </summary>
        public AnimationCurve AnimationCurve;

        /// <summary> Start delay duration for the animation </summary>
        public float StartDelay;

        /// <summary> Length of time for the animation (does not include the StartDelay) </summary>
        public float Duration;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="Fade" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public Fade(AnimationType animationType) { Reset(animationType); }

        /// <summary> Initializes a new instance of the <see cref="Fade" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        /// <param name="enabled"> Determines if this animation is enabled or not </param>
        /// <param name="from"> Start value for the animation </param>
        /// <param name="to"> End value for the animation </param>
        /// <param name="by"> By value for the animation (used to perform relative changes to the current values) </param>
        /// <param name="useCustomFromAndTo"> Allows the usage of custom from and to values for the Fade animation </param>
        /// <param name="numberOfLoops"> The number of loops this animation performs until it stops. If set to -1 it will perform infinite loops </param>
        /// <param name="loopType"> The loop type </param>
        /// <param name="easeType"> Determines if the animation should use an Ease or an AnimationCurve in order to calculate the rate of change over time </param>
        /// <param name="ease"> Sets the ease of the tween. Easing functions specify the rate of change of a parameter over time. To see how default ease curves look, check out easings.net. Enabled only if EaseType is set to EaseType.Ease </param>
        /// <param name="animationCurve"> AnimationCurve used to calculate the rate of change of the animation over time. Enabled only if EaseType is set to EaseType.AnimationCurve </param>
        /// <param name="startDelay"> Start delay duration for the animation </param>
        /// <param name="duration"> Length of time for the animation </param>
        public Fade(AnimationType animationType,
                    bool enabled,
                    float from, float to, float by,
                    bool useCustomFromAndTo,
                    int numberOfLoops, LoopType loopType,
                    EaseType easeType, Ease ease, AnimationCurve animationCurve,
                    float startDelay, float duration) : this(animationType)
        {
            AnimationType = animationType;
            Enabled = enabled;
            From = from;
            To = to;
            By = by;
            UseCustomFromAndTo = useCustomFromAndTo;
            NumberOfLoops = numberOfLoops;
            LoopType = loopType;
            EaseType = easeType;
            Ease = ease;
            AnimationCurve = new AnimationCurve(animationCurve.keys);
            StartDelay = startDelay;
            Duration = duration;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public void Reset(AnimationType animationType)
        {
            AnimationType = animationType;
            Enabled = UIAnimator.DefaultAnimationEnabledState;
            From = 0f;
            To = 0f;
            By = 0.5f;
            UseCustomFromAndTo = false;
            NumberOfLoops = UIAnimator.DefaultNumberOfLoops;
            LoopType = UIAnimator.DefaultLoopType;
            EaseType = UIAnimator.DefaultEaseType;
            Ease = UIAnimator.DefaultEase;
            AnimationCurve = new AnimationCurve();
            StartDelay = UIAnimator.DefaultStartDelay;
            Duration = UIAnimator.DefaultDuration;
        }

        /// <summary> Returns a deep copy </summary>
        public Fade Copy()
        {
            return new Fade(AnimationType)
                   {
                       AnimationType = AnimationType,
                       Enabled = Enabled,
                       From = From,
                       To = To,
                       By = By,
                       UseCustomFromAndTo = UseCustomFromAndTo,
                       NumberOfLoops = NumberOfLoops,
                       LoopType = LoopType,
                       EaseType = EaseType,
                       Ease = Ease,
                       AnimationCurve = new AnimationCurve(AnimationCurve.keys),
                       StartDelay = StartDelay,
                       Duration = Duration
                   };
        }

        #endregion
    }
}