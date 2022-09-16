// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Extensions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class List
        {
            public const string EMPTY_MESSAGE = "Empty...";
            private static float LineSpacing { get { return Properties.Space(); } }

            private static float DefaultLineHeight { get { return Properties.SingleLineHeight + Properties.Space(2); } }

            public static void DrawWithFade(SerializedProperty property, AnimBool expanded, ColorName colorName, string emptyMessage = EMPTY_MESSAGE, float extraLineSpacing = 0, params GUILayoutOption[] options) { DrawWithFade(property, expanded.faded, colorName, emptyMessage, extraLineSpacing, options); }

            public static void DrawWithFade(SerializedProperty property, float faded, ColorName colorName, string emptyMessage = EMPTY_MESSAGE, float extraLineSpacing = 0, params GUILayoutOption[] options)
            {
                float alpha = GUI.color.a;
                if (FadeOut.Begin(faded, false)) Draw(property, colorName, emptyMessage, extraLineSpacing, options);
                FadeOut.End(faded, true, alpha);
            }

            public static void Draw(SerializedProperty property, ColorName colorName, string emptyMessage = EMPTY_MESSAGE, float extraLineSpacing = 0, params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical();
                {
                    float alpha = GUI.color.a;
                    float backgroundHeight = Properties.Space() + Properties.SingleLineHeight + Properties.Space();
                    if (property.arraySize > 0)
                        for (int i = 0; i < property.arraySize; i++)
                        {
                            backgroundHeight += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
                            backgroundHeight += LineSpacing;
                            backgroundHeight += extraLineSpacing;
                        }

                    Background.Draw(DGUI.Colors.GetBackgroundColorName(property.arraySize > 0, colorName), backgroundHeight);
                    GUILayout.Space(-backgroundHeight + Properties.Space());

                    if (property.arraySize == 0)
                    {
                        GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                        {
                            GUILayout.Space(Properties.Space(2));
                            GUI.color = GUI.color.WithAlpha(Properties.TextIconAlphaValue(false));
                            Label.Draw(emptyMessage, Size.S, Colors.DisabledTextColorName, Properties.SingleLineHeight);
                            GUI.color = GUI.color.WithAlpha(alpha);
                            GUILayout.FlexibleSpace();
                            if (Button.IconButton.Plus(Properties.SingleLineHeight)) property.InsertArrayElementAtIndex(property.arraySize);
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                        return;
                    }

                    for (int i = 0; i < property.arraySize; i++)
                    {
                        SerializedProperty childProperty = property.GetArrayElementAtIndex(i);
                        float propertyHeight = EditorGUI.GetPropertyHeight(childProperty);
                        GUILayout.BeginHorizontal(GUILayout.Height(propertyHeight));
                        {
                            GUILayout.Space(Properties.Space(2));
                            GUI.color = GUI.color.WithAlpha(Properties.TextIconAlphaValue(false));
                            Label.Draw(i.ToString(), Size.S, Colors.DisabledTextColorName, propertyHeight);
                            GUI.color = GUI.color.WithAlpha(alpha);
                            Property.Draw(childProperty);
                            if (Button.IconButton.Minus(propertyHeight)) property.DeleteArrayElementAtIndex(i);
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(LineSpacing);
                    }

                    GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                    {
                        GUILayout.FlexibleSpace();
                        if (Button.IconButton.Plus(Properties.SingleLineHeight))
                        {
                            property.InsertArrayElementAtIndex(property.arraySize);

                            //Reset the newly created serialized property to its default value 
                            SerializedProperty p = property.GetArrayElementAtIndex(property.arraySize - 1);
                            switch (p.propertyType)
                            {
                                case SerializedPropertyType.Generic: break;
                                case SerializedPropertyType.Integer:
                                    p.intValue = default(int);
                                    break;
                                case SerializedPropertyType.Boolean:
                                    p.boolValue = default(bool);
                                    break;
                                case SerializedPropertyType.Float:
                                    p.floatValue = default(float);
                                    break;
                                case SerializedPropertyType.String:
                                    p.stringValue = default(string);
                                    break;
                                case SerializedPropertyType.Color:
                                    p.colorValue = default(Color);
                                    break;
                                case SerializedPropertyType.ObjectReference:
                                    p.objectReferenceValue = default(Object);
                                    ;
                                    break;
                                case SerializedPropertyType.LayerMask: break;
                                case SerializedPropertyType.Enum:
                                    p.enumValueIndex = 0;
                                    break;
                                case SerializedPropertyType.Vector2:
                                    p.vector2Value = default(Vector2);
                                    break;
                                case SerializedPropertyType.Vector3:
                                    p.vector3Value = default(Vector3);
                                    break;
                                case SerializedPropertyType.Vector4:
                                    p.vector4Value = default(Vector4);
                                    break;
                                case SerializedPropertyType.Rect:
                                    p.rectValue = default(Rect);
                                    break;
                                case SerializedPropertyType.ArraySize: break;
                                case SerializedPropertyType.Character:
                                    p.stringValue = default(string);
                                    break;
                                case SerializedPropertyType.AnimationCurve:
                                    p.animationCurveValue = default(AnimationCurve);
                                    break;
                                case SerializedPropertyType.Bounds:
                                    p.boundsValue = default(Bounds);
                                    break;
                                case SerializedPropertyType.Gradient: break;
                                case SerializedPropertyType.Quaternion:
                                    p.quaternionValue = default(Quaternion);
                                    break;
                                case SerializedPropertyType.ExposedReference:
                                    p.objectReferenceValue = default(Object);
                                    break;
                                case SerializedPropertyType.FixedBufferSize: break;
                                case SerializedPropertyType.Vector2Int:      break;
                                case SerializedPropertyType.Vector3Int:      break;
                                case SerializedPropertyType.RectInt:         break;
                                case SerializedPropertyType.BoundsInt:       break;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }
    }
}