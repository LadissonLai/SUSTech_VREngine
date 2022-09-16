// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class FoldOut
        {
            public const float INDENT_WIDTH = Properties.INDENT_WIDTH;
            public const float SPACE_ADDED_AT_THE_END_WHEN_OPENED = Properties.SPACE * 4;

            public static bool Begin(AnimBool expanded, bool indentContent = true) { return Begin(expanded.faded, indentContent); }
            public static void End(AnimBool expanded, bool addExtraSpaceWhenExpanded = true) { End(expanded.faded, addExtraSpaceWhenExpanded); }

            public static bool Begin(float faded, bool indentContent = true)
            {
                if (faded < 0.05f) return false;
                EditorGUILayout.BeginFadeGroup(faded);
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                if (indentContent) GUILayout.Space(INDENT_WIDTH * faded);
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                return true;
            }

            public static void End(float faded, bool addExtraSpaceWhenExpanded = true)
            {
                if (faded < 0.05f) return;
                if (addExtraSpaceWhenExpanded) GUILayout.Space(SPACE_ADDED_AT_THE_END_WHEN_OPENED * faded);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndFadeGroup();
            }
        }
    }
}