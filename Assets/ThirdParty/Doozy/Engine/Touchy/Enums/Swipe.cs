// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Touchy
{
    /// <summary>
    ///     Describes a swipe in 8 directions
    /// </summary>
    public enum Swipe
    {
        /// <summary>
        ///     Unknown swipe direction (disabled state)
        /// </summary>
        None = 0,

        /// <summary>
        ///     Swipe Up-Left (diagonal swipe)
        /// </summary>
        UpLeft = 1,

        /// <summary>
        ///     Swipe Up
        /// </summary>
        Up = 2,

        /// <summary>
        ///     Swipe Up-Right (diagonal swipe)
        /// </summary>
        UpRight = 3,

        /// <summary>
        ///     Swipe Left
        /// </summary>
        Left = 4,

        /// <summary>
        ///     Swipe Right
        /// </summary>
        Right = 5,

        /// <summary>
        ///     Swipe Down-Left (diagonal swipe)
        /// </summary>
        DownLeft = 6,

        /// <summary>
        ///     Swipe Down
        /// </summary>
        Down = 7,

        /// <summary>
        ///     Swipe Down-Right (diagonal swipe)
        /// </summary>
        DownRight = 8
    }
}