// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Animation
{
    /// <summary> Describes types of animations </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Unknown animation
        /// </summary>
        Undefined,
        
        /// <summary>
        /// Show animation (enter screen view)
        /// </summary>
        Show,
        
        /// <summary>
        /// Hide animation (exit screen view)
        /// </summary>
        Hide,
        
        /// <summary>
        /// Loop animation (repeats/restarts itself)
        /// </summary>
        Loop,
        
        /// <summary>
        /// Punch animation (fast animation that returns to the start values when finished)
        /// </summary>
        Punch,
        
        /// <summary>
        /// State animation (changes the state of the target by setting new values for position, rotation, scale and/or alpha)
        /// </summary>
        State
    }
}