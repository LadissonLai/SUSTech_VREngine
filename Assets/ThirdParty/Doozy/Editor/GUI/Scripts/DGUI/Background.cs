// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Background
        {
            public const float BACKGROUND_ALPHA = 0.3f;
            public const CornerType DEFAULT_CORNER_TYPE = CornerType.Round;

            public static GUIStyle Style(CornerType type)
            {
                switch (type)
                {
                    case CornerType.Round:  return Styles.GetStyle(Styles.StyleName.BackgroundRound);
                    case CornerType.Square: return Styles.GetStyle(Styles.StyleName.BackgroundSquare);
                    default:                throw new ArgumentOutOfRangeException("type", type, null);
                }
            }

            public static void Draw(Rect rect, DColor dColor, CornerType cornerType = DEFAULT_CORNER_TYPE)
            {
                Color color = GUI.color;
                GUI.color = Colors.BackgroundColor(dColor).WithAlpha(BACKGROUND_ALPHA * color.a);
                GUI.Label(rect, GUIContent.none, Style(cornerType));
                GUI.color = color;
            }

            public static void Draw(Rect rect, ColorName colorName, CornerType cornerType = DEFAULT_CORNER_TYPE) { Draw(rect, Colors.GetDColor(colorName), cornerType); }

            public static void Draw(DColor dColor, CornerType cornerType, params GUILayoutOption[] options)
            {
                Color color = GUI.color;
                GUI.color = Colors.BackgroundColor(dColor).WithAlpha(BACKGROUND_ALPHA * color.a);
                GUILayout.Label(GUIContent.none, Style(cornerType), options);
                GUI.color = color;
            }

            public static void Draw(DColor dColor, params GUILayoutOption[] options) { Draw(dColor, DEFAULT_CORNER_TYPE, options); }
            public static void Draw(ColorName colorName, CornerType cornerType, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), cornerType, options); }
            public static void Draw(ColorName colorName, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), DEFAULT_CORNER_TYPE, options); }

            public static void Draw(DColor dColor, CornerType cornerType, float height, params GUILayoutOption[] options)
            {
                var list = new List<GUILayoutOption>();
                if (options != null) list.AddRange(options);
                list.Add(GUILayout.Height(height));
                Draw(dColor, cornerType, list.ToArray());
            }

            public static void Draw(DColor dColor, float height, params GUILayoutOption[] options) { Draw(dColor, DEFAULT_CORNER_TYPE, height, options); }
            public static void Draw(ColorName colorName, float height, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), DEFAULT_CORNER_TYPE, height, options); }
            public static void Draw(ColorName colorName, CornerType cornerType, float height, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), cornerType, height, options); }

            public static void Draw(DColor dColor, CornerType cornerType, float height, float width, params GUILayoutOption[] options)
            {
                var list = new List<GUILayoutOption>();
                if (options != null) list.AddRange(options);
                list.Add(GUILayout.Height(height));
                list.Add(GUILayout.Width(width));
                Draw(dColor, cornerType, list.ToArray());
            }

            public static void Draw(DColor dColor, float height, float width, params GUILayoutOption[] options) { Draw(dColor, DEFAULT_CORNER_TYPE, height, width, options); }
            public static void Draw(ColorName colorName, float height, float width, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), DEFAULT_CORNER_TYPE, height, width, options); }
            public static void Draw(ColorName colorName, CornerType cornerType, float height, float width, params GUILayoutOption[] options) { Draw(Colors.GetDColor(colorName), cornerType, height, width, options); }
        }
    }
}