// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Line
        {
            public static void AddSpace(bool add)
            {
                if (!add) return;
                GUILayout.Space(Properties.Space(2));
            }

            public static void Draw(bool insertSpaceBetweenEachCallback, params UnityAction[] drawCallbacks)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                AddSpace(insertSpaceBetweenEachCallback);
                foreach (UnityAction drawCallback in drawCallbacks)
                {
                    drawCallback.Invoke();
                    AddSpace(insertSpaceBetweenEachCallback);
                }

                GUILayout.EndHorizontal();
            }

            public static void Draw(bool insertSpaceBetweenEachCallback, DColor backgroundColor, bool drawBackground, float lineHeight, params UnityAction[] drawCallbacks)
            {
                float backgroundHeight = lineHeight + Properties.Space(2);
                GUILayout.BeginVertical(GUILayout.Height(backgroundHeight));
                if (drawBackground)
                {
                    Background.Draw(backgroundColor, backgroundHeight);
                    GUILayout.Space(-backgroundHeight + Properties.Space());
                }

                Draw(insertSpaceBetweenEachCallback, drawCallbacks);
                GUILayout.EndVertical();
            }

            public static void Draw(bool insertSpaceBetweenEachCallback, DColor backgroundColor, bool drawBackground, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, backgroundColor, drawBackground, Properties.SingleLineHeight, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, ColorName backgroundColorName, bool drawBackground, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, Colors.GetDColor(backgroundColorName), drawBackground, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, ColorName backgroundColorName, bool drawBackground, float lineHeight, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, Colors.GetDColor(backgroundColorName), drawBackground, lineHeight, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, DColor backgroundColor, float lineHeight, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, backgroundColor, true, lineHeight, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, ColorName backgroundColorName, float lineHeight, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, Colors.GetDColor(backgroundColorName), lineHeight, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, DColor backgroundColor, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, backgroundColor, true, Properties.SingleLineHeight, drawCallbacks); }
            public static void Draw(bool insertSpaceBetweenEachCallback, ColorName backgroundColorName, params UnityAction[] drawCallbacks) { Draw(insertSpaceBetweenEachCallback, Colors.GetDColor(backgroundColorName), Properties.SingleLineHeight, drawCallbacks); }
        }
    }
}