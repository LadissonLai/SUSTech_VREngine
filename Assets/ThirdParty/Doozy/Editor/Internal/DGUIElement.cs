// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    [Serializable]
    public class DGUIElement
    {
        public enum DrawMode
        {
            Label,
            Field,
            LabelAndField,
            FieldAndLabel
        }

        private readonly SerializedProperty m_property;
        public SerializedProperty Property { get { return m_property; } }
        private readonly GUIContent m_label;
        private float m_fieldWeight;

        private bool m_hasLabel;
        private Rect m_labelRect;
//        private float m_labelWidth;

        private bool m_hasField;
        private Rect m_fieldRect;
        private float m_fieldOffsetY;
        private float m_fieldWidth;
        private float m_fieldFixedWidth;

        public static GUIStyle DefaultLabelStyle { get { return DGUI.Label.Style(); } }

        public float LabelWidth { get { return m_hasLabel ? DefaultLabelStyle.CalcSize(m_label).x + DGUI.Properties.StandardVerticalSpacing : 0; } }

        public float TotalWidth(float totalFieldsWidth, DrawMode drawMode)
        {
            switch (drawMode)
            {
                case DrawMode.Label:
                    return (m_hasLabel ? LabelWidth : 0);
                case DrawMode.Field:
                    return (HasFixedFieldWidth ? m_fieldFixedWidth : totalFieldsWidth * m_fieldWeight);
                case DrawMode.LabelAndField:
                case DrawMode.FieldAndLabel:
                    return (m_hasLabel ? LabelWidth + DGUI.Properties.StandardVerticalSpacing * 2 : 0) + (HasFixedFieldWidth ? m_fieldFixedWidth : totalFieldsWidth * m_fieldWeight);
                default: throw new ArgumentOutOfRangeException("drawMode", drawMode, null);
            }
        }

        public float FixedFieldWidth { get { return HasFixedFieldWidth ? m_fieldFixedWidth : 0; } }

        public bool HasLabel { get { return m_hasLabel; } }
        public bool HasField { get { return m_hasField; } }
        public bool HasFixedFieldWidth { get { return m_fieldFixedWidth > 0; } }

        public DGUIElement(GUIContent label)
        {
            m_label = label;
            m_hasLabel = m_label != null;
//            m_labelWidth = m_hasLabel ? DefaultLabelStyle.CalcSize(m_label).x : 0;
            m_property = null;
            m_hasField = m_property != null;
            m_fieldWeight = 1;
            m_fieldFixedWidth = 0;
            m_fieldOffsetY = 0;
        }

        public DGUIElement(SerializedProperty property, GUIContent label, float fieldOffsetY = 1, float fieldFixedWidth = 0)
        {
            m_label = label;
            m_hasLabel = m_label != null;
//            m_labelWidth = m_hasLabel ? DefaultLabelStyle.CalcSize(m_label).x : 0;
            m_property = property;
            m_hasField = m_property != null;
            m_fieldWeight = 1;
            m_fieldFixedWidth = fieldFixedWidth;
            m_fieldOffsetY = fieldOffsetY;
        }

        public DGUIElement(SerializedProperty property, float fieldOffsetY = 1, float fieldFixedWidth = 0)
        {
            m_label = null;
            m_hasLabel = m_label != null;
//            m_labelWidth = m_hasLabel ? DefaultLabelStyle.CalcSize(m_label).x : 0;
            m_property = property;
            m_hasField = m_property != null;
            m_fieldWeight = 1;
            m_fieldFixedWidth = fieldFixedWidth;
            m_fieldOffsetY = fieldOffsetY;
        }

        public float Draw(float x, float y, float totalFieldsWidth, float height, float fieldWeight, DrawMode drawMode)
        {
            m_fieldWeight = fieldWeight;
            m_fieldWidth = HasFixedFieldWidth ? m_fieldFixedWidth : totalFieldsWidth * m_fieldWeight;

            switch (drawMode)
            {
                case DrawMode.Label: //draw LABEL ONLY
                    if (!HasLabel) return 0; //no label -> return
                    m_labelRect = new Rect(x, y, LabelWidth, height); //calculate label rect
                    DGUI.Label.Draw(m_labelRect, m_label.text, Size.M); //draw label
                    return m_labelRect.width;

                case DrawMode.Field: //draw FIELD ONLY
                    if (!HasField) return 0; //no field -> return
                    m_fieldRect = new Rect(x, y + m_fieldOffsetY, m_fieldWidth, height); //calculate field rect
                    EditorGUI.PropertyField(m_fieldRect, m_property, GUIContent.none, true); //draw field
                    return m_fieldRect.width;

                case DrawMode.LabelAndField: //draw LABEL & FIELD
                    if (!HasLabel && !HasField) return 0; //no label and no field -> return
                    m_labelRect = new Rect(x, y, LabelWidth, height); //calculate label rect
                    m_fieldRect = new Rect(x + (HasLabel ? LabelWidth + DGUI.Properties.StandardVerticalSpacing : 0), y + m_fieldOffsetY, m_fieldWidth, height); //calculate field rect
                    if (HasLabel) DGUI.Label.Draw(m_labelRect, m_label.text, Size.M); //has label -> draw label
                    if (HasField) EditorGUI.PropertyField(m_fieldRect, m_property, GUIContent.none, true); //has field -> draw field
                    return (HasLabel ? m_labelRect.width : 0) +
                           (HasField ? m_fieldRect.width : 0) +
                           (HasLabel && HasField ? DGUI.Properties.StandardVerticalSpacing : 0);

                case DrawMode.FieldAndLabel: //draw FIELD & LABEL
                    if (!HasLabel && !HasField) return 0; //no label and no field -> return
                    m_labelRect = new Rect(x + (HasField ? m_fieldRect.width + DGUI.Properties.StandardVerticalSpacing : 0), y, LabelWidth, height); //calculate label rect
                    m_fieldRect = new Rect(x, y + m_fieldOffsetY, m_fieldWidth, height); //calculate field rect
                    if (HasLabel) DGUI.Label.Draw(m_labelRect, m_label.text, Size.M); //has label -> draw label
                    if (HasField) EditorGUI.PropertyField(m_fieldRect, m_property, GUIContent.none, true); //has field -> draw field
                    return (HasLabel ? m_labelRect.width : 0) +
                           (HasField ? m_fieldRect.width : 0) +
                           (HasLabel && HasField ? DGUI.Properties.StandardVerticalSpacing : 0);
            }

            return 0;
        }
    }
}