// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class AlphaGroup
        {
            /// <summary> Begins a group that can be be made visible/invisible just by making the GUI 100% transparent </summary>
            /// <param name="visibility"> Target animation bool that animates this group's visibility. The 'faded' parameter is used for this purpose, having a value between 0 and 1, 0 being invisible, and 1 being fully visible. </param>
            public static bool Begin(AnimBool visibility) { return Begin(visibility.faded); }

            /// <summary> Begins a group that can be be made visible/invisible just by making the GUI 100% transparent </summary>
            /// <param name="faded"> A value between 0 and 1, 0 being invisible, and 1 being fully visible. </param>
            public static bool Begin(float faded)
            {
                GUI.color = GUI.color.WithAlpha(faded);
                return faded > 0;
            }

            /// <summary> Closes a group started with AlphaGroup.Begin() by resetting the GUI.color to a custom alpha value </summary>
            public static void End(float customAlpha = 1f) { GUI.color = GUI.color.WithAlpha(customAlpha); }
        }
    }
}