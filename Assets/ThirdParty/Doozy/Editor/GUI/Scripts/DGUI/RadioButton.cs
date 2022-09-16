// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class RadioButton
        {
            public static bool Draw(bool value, ColorName colorName)
            {
                Color initialColor = GUI.color;
                GUI.color = EditorGUIUtility.isProSkin ? Colors.GetDColor(colorName).Normal.WithAlpha(GUI.color.a) : Colors.GetDColor(colorName).Light.WithAlpha(GUI.color.a);
                bool result = GUILayout.Toggle(value, GUIContent.none, Styles.GetStyle(value ? Styles.StyleName.RadioButtonEnabled : Styles.StyleName.RadioButtonDisabled));
                GUI.color = initialColor;
                if (result) Properties.ResetKeyboardFocus();
                return result;
            }
        }

        public static class RadioButtonGroup
        {
            public static int Draw(int index, int numberOfButtons, float spacing, ColorName colorNameOld)
            {
                for (int i = 0; i < numberOfButtons; i++)
                {
                    bool toggle = index == i;
                    toggle = RadioButton.Draw(toggle, colorNameOld);
                    if (toggle) index = i;
                    GUILayout.Space(spacing);
                }

                return index;
            }
        }
    }
}