// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Divider
        {
            public enum Type
            {
                One,
                Two
            }

            private static GUIStyle GetStyle(Type type)
            {
                switch (type)
                {
                    case Type.One: return Styles.GetStyle(Styles.StyleName.DividerOne);
                    case Type.Two: return Styles.GetStyle(Styles.StyleName.DividerTwo);
                    default:       throw new ArgumentOutOfRangeException("type", type, null);
                }
            }

            public static void Draw(Type type, ColorName colorName)
            {
                Color initialColor = GUI.color;
                GUI.color = Colors.GetDColor(colorName).Normal.WithAlpha(GUI.color.a);
                GUILayout.Label(GUIContent.none, GetStyle(type));
                GUI.color = initialColor;
            }

            public static void Draw(Type type) { GUILayout.Label(GUIContent.none, GetStyle(type)); }
        }
    }
}