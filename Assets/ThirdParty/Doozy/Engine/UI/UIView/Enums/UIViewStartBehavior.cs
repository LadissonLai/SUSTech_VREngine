// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the actions an UIView can perform at Start </summary>
    public enum UIViewStartBehavior
    {
        /// <summary>
        ///     Do Nothing.
        ///     <para />
        ///     Used when the UIView is visible and should not do anything else.
        /// </summary>
        DoNothing,

        /// <summary>
        ///     Start hidden.
        ///     <para />
        ///     Used when the UIView should be out of view at Start.
        ///     <para />
        ///     This triggers an instant auto HIDE animation, thus the UIView hides in zero seconds.
        /// </summary>
        Hide,

        /// <summary> Start by playing the SHOW animation.
        ///     <para />
        ///     Used when the UIView should animate becoming visible at Start.
        ///     <para />
        ///     This triggers and instant HIDE and then an automated SHOW animation immediately after that.
        /// </summary>
        PlayShowAnimation
    }
}