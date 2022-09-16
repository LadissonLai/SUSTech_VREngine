// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Touchy
{
    /// <summary> Describes a swipe in 4 directions </summary>
    public enum SimpleSwipe
    {
        /// <summary>
        ///     Unknown swipe direction (disabled state)
        /// </summary>
        None = 0,

        /// <summary>
        ///     Swipe Left
        /// </summary>
        Left = 1,

        /// <summary>
        ///     Swipe Right
        /// </summary>
        Right = 2,

        /// <summary>
        ///     Swipe Up
        /// </summary>
        Up = 3,

        /// <summary>
        ///     Swipe Down
        /// </summary>
        Down = 4
    }
}