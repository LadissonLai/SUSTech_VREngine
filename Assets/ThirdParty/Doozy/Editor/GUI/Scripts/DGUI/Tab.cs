// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Tab
        {
            public enum Direction
            {
                ToRight,
                ToBottom,
                ToLeft,
                ToTop
            }

            public static GUIStyle GetStyle(Direction direction, bool selectedStyle = false)
            {
                switch (direction)
                {
                    case Direction.ToRight:  break;
                    case Direction.ToBottom: break;
                    case Direction.ToLeft:   break;
                    case Direction.ToTop:
                        return Styles.GetStyle(selectedStyle ? Styles.StyleName.TabToTopSelected : Styles.StyleName.TabToTop);
                    default: throw new ArgumentOutOfRangeException("direction", direction, null);
                }

                return null;
            }

            public static bool Draw(string label, bool isSelected = false, Direction direction = Direction.ToTop, params GUILayoutOption[] options)
            {
                bool result = GUILayout.Button(label, GetStyle(direction, isSelected), options);
                return result;
            }
        }
    }
}