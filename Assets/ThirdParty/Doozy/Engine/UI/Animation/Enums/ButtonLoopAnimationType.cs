// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Animation
{
    /// <summary> Describes the types of loop animations available for buttons (and similar components) </summary>
    public enum ButtonLoopAnimationType
    {
        /// <summary>
        ///     Normal loop animation that runs when the button is NOT selected
        /// </summary>
        Normal = 0,

        /// <summary>
        ///     Selected loop animation that runs when the button is selected
        /// </summary>
        Selected = 1
    }
}