// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Touchy
{
    /// <summary> Describes the types of gestures available </summary>
    public enum GestureType
    {
        /// <summary>
        ///     A light touch on a touch-sensitive screen
        /// </summary>
        Tap = 0,

        /// <summary>
        ///     A light touch on a touch-sensitive screen, held down down for a second or two
        /// </summary>
        LongTap = 1,

        /// <summary>
        ///     Sliding a finger or a stylus pen across a touch-sensitive screen to scroll or move items around
        /// </summary>
        Swipe = 2
    }
}