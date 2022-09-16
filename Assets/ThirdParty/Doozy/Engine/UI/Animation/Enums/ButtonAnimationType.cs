// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Animation
{
    /// <summary> Describes the types of animation available for buttons (and similar components) </summary>
    public enum ButtonAnimationType
    {
        /// <summary>
        /// Punch animation (fast animation that returns to the start values when finished)
        /// </summary>
        Punch,
        
        /// <summary>
        /// State animation (changes the state of the target by setting new values for position, rotation, scale and/or alpha)
        /// </summary>
        State,
        
        /// <summary>
        /// Animation managed by an Animator Controller which is an interface to control the Mecanim animation system
        /// </summary>
        Animator
    }
}