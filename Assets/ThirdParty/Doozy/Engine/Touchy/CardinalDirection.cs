// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Engine.Touchy
{
    /// <summary> Contains static values for all the cardinal directions </summary>
    public static class CardinalDirection
    {
        /// <summary> Direction Unknown. Returns Vector2(0, 0) </summary>
        public static readonly Vector2 None = new Vector2(0, 0);

        /// <summary> Direction Up. Returns Vector2(0, 1) </summary>
        public static readonly Vector2 Up = new Vector2(0, 1);

        /// <summary> Direction Down. Returns Vector2(0, -1) </summary>
        public static readonly Vector2 Down = new Vector2(0, -1);

        /// <summary> Direction Right. Returns Vector2(1, 0) </summary>
        public static readonly Vector2 Right = new Vector2(1, 0);

        /// <summary> Direction Left. Returns Vector2(-1, 0) </summary>
        public static readonly Vector2 Left = new Vector2(-1, 0);

        /// <summary> Direction Up-Right. Returns Vector2(1, 1) </summary>
        public static readonly Vector2 UpRight = new Vector2(1, 1);

        /// <summary> Direction Up-Left. Returns Vector2(-1, 1) </summary>
        public static readonly Vector2 UpLeft = new Vector2(-1, 1);

        /// <summary> Direction Down-Right. Returns Vector2(1, -1) </summary>
        public static readonly Vector2 DownRight = new Vector2(1, -1);

        /// <summary> Direction Down-Left. Returns Vector2(-1, 1) </summary>
        public static readonly Vector2 DownLeft = new Vector2(-1, -1);

        /// <summary> Returns the Vector2 representation of the given swipe direction </summary>
        /// <param name="swipe"> Target swipe direction </param>
        public static Vector2 Get(Swipe swipe)
        {
            switch (swipe)
            {
                case Swipe.None:      return None;
                case Swipe.UpLeft:    return UpLeft;
                case Swipe.Up:        return Up;
                case Swipe.UpRight:   return UpRight;
                case Swipe.Left:      return Left;
                case Swipe.Right:     return Right;
                case Swipe.DownLeft:  return DownLeft;
                case Swipe.Down:      return Down;
                case Swipe.DownRight: return DownRight;
                default:              return None;
            }
        }
    }
}