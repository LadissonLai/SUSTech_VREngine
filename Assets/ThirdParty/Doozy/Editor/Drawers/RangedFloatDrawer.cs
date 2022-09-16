// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine;
using Doozy.Engine.Attributes;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(RangedFloat), true)]
    public class RangedFloatDrawer : BaseDrawer
    {
        private const string
            MIN_VALUE_PROPERTY = "MinValue",
            MAX_VALUE_PROPERTY = "MaxValue";

        private const string
            MIN_VALUE_LABEL = "min",
            MAX_VALUE_LABEL = "max";

        private void Init(SerializedProperty property)
        {
            if (Initialized.ContainsKey(property.propertyPath) && Initialized[property.propertyPath]) return;

            Elements.Add(Properties.Add(MIN_VALUE_PROPERTY, property), Contents.Add(MIN_VALUE_LABEL));
            Elements.Add(Properties.Add(MAX_VALUE_PROPERTY, property), Contents.Add(MAX_VALUE_LABEL));

            if (!Initialized.ContainsKey(property.propertyPath))
                Initialized.Add(property.propertyPath, true);
            else
                Initialized[property.propertyPath] = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Init(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Draw(position, property, label);

                // set indent back to what it was
                EditorGUI.indentLevel = indent;

                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        private void Draw(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty min = Properties.Get(MIN_VALUE_PROPERTY, property);
            SerializedProperty max = Properties.Get(MAX_VALUE_PROPERTY, property);

            float minValue = min.floatValue;
            float maxValue = max.floatValue;

            float rangeMin = 0;
            float rangeMax = 1;

            var ranges = (MinMaxRangeAttribute[]) fieldInfo.GetCustomAttributes(typeof(MinMaxRangeAttribute), true);
            if (ranges.Length > 0)
            {
                rangeMin = ranges[0].Min;
                rangeMax = ranges[0].Max;
            }

            GUIStyle labelStyle = DGUI.Label.Style(Size.S);

            var minValueContent = new GUIContent("min " + minValue.ToString("F2"));
            var maxValueContent = new GUIContent(maxValue.ToString("F2") + " max");
            
            Vector2 minValueContentSize = labelStyle.CalcSize(minValueContent);
            Vector2 maxValueContentSize = labelStyle.CalcSize(maxValueContent);
            
            var minValueLabelRect = new Rect(position.xMin, position.y + (DGUI.Properties.SingleLineHeight - minValueContentSize.y) / 2, minValueContentSize.x, minValueContentSize.y);
            var maxValueLabelRect = new Rect(position.xMax - maxValueContentSize.x, position.y + (DGUI.Properties.SingleLineHeight - maxValueContentSize.y) / 2, maxValueContentSize.x, maxValueContentSize.y);
            var sliderRect = new Rect(position.xMin + minValueContentSize.x + DGUI.Properties.Space(2),
                                      position.y - 1,
                                      position.width - minValueContentSize.x - maxValueContentSize.x - DGUI.Properties.Space(4),
                                      DGUI.Properties.SingleLineHeight - DGUI.Properties.Space());

            GUI.Label(minValueLabelRect, minValueContent, labelStyle);
            GUI.Label(maxValueLabelRect, maxValueContent, labelStyle);
            
            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                min.floatValue = minValue;
                max.floatValue = maxValue;
            }
        }
    }
}