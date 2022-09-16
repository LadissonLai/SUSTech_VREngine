// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Animation
{
    /// <summary> Base class for all animation settings </summary>
    [Serializable]
    public class UIAnimation
    {
        #region Properties

        /// <summary> Returns TRUE if at least one animation type is enabled (move, rotate, scale or fade), false otherwise </summary>
        public bool Enabled
        {
            get
            {
                switch (AnimationType)
                {
                    case AnimationType.Undefined: return false;
                    case AnimationType.Show:      return Move.Enabled || Rotate.Enabled || Scale.Enabled || Fade.Enabled;
                    case AnimationType.Hide:      return Move.Enabled || Rotate.Enabled || Scale.Enabled || Fade.Enabled;
                    case AnimationType.Loop:      return Move.Enabled || Rotate.Enabled || Scale.Enabled || Fade.Enabled;
                    case AnimationType.Punch:     return Move.Enabled || Rotate.Enabled || Scale.Enabled;
                    case AnimationType.State:     return Move.Enabled || Rotate.Enabled || Scale.Enabled || Fade.Enabled;
                    default:                      return false;
                }
            }
        }

        /// <summary> Returns the minimum start delay set for the animation  </summary>
        public float StartDelay
        {
            get
            {
                if (!Enabled) return 0;
                return Mathf.Min(Move.Enabled ? Move.StartDelay : 10000,
                                 Rotate.Enabled ? Rotate.StartDelay : 10000,
                                 Scale.Enabled ? Scale.StartDelay : 10000,
                                 Fade.Enabled ? Fade.StartDelay : 10000);
            }
        }
        
        /// <summary> Returns the maximum duration (including start delay) of the animation </summary>
        public float TotalDuration
        {
            get
            {
                return Mathf.Max(Move.Enabled ? Move.TotalDuration : 0,
                                 Rotate.Enabled ? Rotate.TotalDuration : 0,
                                 Scale.Enabled ? Scale.TotalDuration : 0,
                                 Fade.Enabled ? Fade.TotalDuration : 0);
            }
        }

        #endregion

        #region Public Variables

        /// <summary> The animation type that determines the animation behavior </summary>
        public AnimationType AnimationType = AnimationType.Undefined;

        /// <summary> Move animation settings </summary>
        public Move Move;

        /// <summary> Rotate animation settings </summary>
        public Rotate Rotate;

        /// <summary> Scale animation settings </summary>
        public Scale Scale;

        /// <summary> Fade animation settings </summary>
        public Fade Fade;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the <see cref="UIAnimation" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public UIAnimation(AnimationType animationType) { Reset(animationType); }

        /// <inheritdoc />
        /// <summary> Initializes a new instance of the <see cref="UIAnimation" /> class </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        /// <param name="move"> Move animation settings </param>
        /// <param name="rotate"> Rotate animation settings </param>
        /// <param name="scale"> Scale animation settings </param>
        /// <param name="fade"> Fade animation settings </param>
        public UIAnimation(AnimationType animationType, Move move, Rotate rotate, Scale scale, Fade fade) : this(animationType)
        {
            Move = move;
            Rotate = rotate;
            Scale = scale;
            Fade = fade;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="animationType"> The animation type that determines the behavior of this animation </param>
        public void Reset(AnimationType animationType)
        {
            AnimationType = animationType;
            Move = new Move(animationType);
            Rotate = new Rotate(animationType);
            Scale = new Scale(animationType);
            Fade = new Fade(animationType);
        }

        /// <summary> Returns a deep copy </summary>
        public UIAnimation Copy()
        {
            return new UIAnimation(AnimationType)
                   {
                       AnimationType = AnimationType,
                       Move = Move.Copy(),
                       Rotate = Rotate.Copy(),
                       Scale = Scale.Copy(),
                       Fade = Fade.Copy()
                   };
        }

        #endregion
    }
}