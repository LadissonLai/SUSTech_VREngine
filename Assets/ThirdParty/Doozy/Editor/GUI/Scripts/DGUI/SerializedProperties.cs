// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class SerializedProperties
        {
            public static bool DrawProperty(SerializedProperty property, string label)
            {
                bool propertyChanged = false;
                Color initialColor = GUI.color;

                GUILayout.BeginVertical(GUILayout.Height(Properties.SingleLineHeight + Properties.Space(2)));
                {
                    Background.Draw(Colors.kWhite, GUILayout.Height(Properties.SingleLineHeight + Properties.Space(2)));

                    GUILayout.Space(Properties.Space(2));
                    GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                    {
                        GUILayout.Space(Properties.Space(2));
                        Label.Draw(label, Size.S);
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Space(-1);
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(property, GUIContent.none, true, GUILayout.ExpandWidth(true));
                            propertyChanged = EditorGUI.EndChangeCheck();
                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(Properties.Space(2));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(Properties.Space(2));
                }
                GUILayout.EndVertical();

                GUI.color = initialColor;

                return propertyChanged;
            }
        }
    }
}