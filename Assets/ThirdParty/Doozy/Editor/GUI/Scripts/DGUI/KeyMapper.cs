// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class KeyMapper
        {
            public static bool DetectKey(Event current, KeyCode keyCode, EventType eventType)
            {
                bool result = current.isKey && current.keyCode == keyCode && current.type == eventType;
                if (result) current.Use();

                return result;
            }

            public static bool DetectKey(Event current, KeyCode keyCode, EventType eventType, string focusedControlName) { return GUI.GetNameOfFocusedControl().Equals(focusedControlName) && DetectKey(current, keyCode, eventType); }

            public static bool DetectKeyDown(Event current, KeyCode keyCode) { return DetectKey(current, keyCode, EventType.KeyDown); }
            public static bool DetectKeyDown(Event current, KeyCode keyCode, string focusedControlName) { return GUI.GetNameOfFocusedControl().Equals(focusedControlName) && DetectKeyDown(current, keyCode); }

            public static bool DetectKeyUp(Event current, KeyCode keyCode) { return DetectKey(current, keyCode, EventType.KeyUp); }
            public static bool DetectKeyUp(Event current, KeyCode keyCode, string focusedControlName) { return GUI.GetNameOfFocusedControl().Equals(focusedControlName) && DetectKeyUp(current, keyCode); }

            public static bool DetectKeyDownCombo(Event current, EventModifiers modifier, KeyCode key) { return current.modifiers == modifier && DetectKeyDown(current, key); }
            public static bool DetectKeyUpCombo(Event current, EventModifiers modifier, KeyCode key) { return current.modifiers == modifier && DetectKeyUp(current, key); }
        }
    }
}