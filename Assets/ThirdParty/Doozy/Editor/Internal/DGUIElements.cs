// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    public class DGUIElements
    {
        private readonly Dictionary<string, DGUIElement> m_elements = new Dictionary<string, DGUIElement>();
        private readonly Dictionary<string, DGUIElement> m_labels = new Dictionary<string, DGUIElement>();


        private static string LabelKey(string text, GUIStyle style) { return style.name + ":" + text; }

        public DGUIElement Add(SerializedProperty property, GUIContent label, float fieldOffsetY = 1, float fieldFixedWidth = 0)
        {
            if (m_elements.ContainsKey(property.propertyPath)) return m_elements[property.propertyPath];
            m_elements.Add(property.propertyPath, new DGUIElement(property, label, fieldOffsetY, fieldFixedWidth));
            return m_elements[property.propertyPath];
        }

        public DGUIElement Add(SerializedProperty property, float fieldOffsetY = 1, float fieldFixedWidth = 0)
        {
            if (m_elements.ContainsKey(property.propertyPath)) return m_elements[property.propertyPath];
            m_elements.Add(property.propertyPath, new DGUIElement(property, fieldOffsetY, fieldFixedWidth));
            return m_elements[property.propertyPath];
        }

        public DGUIElement AddLabel(GUIContent label)
        {
            string key = LabelKey(label.text, DGUIElement.DefaultLabelStyle);
            if (m_labels.ContainsKey(key)) return m_labels[key];
            m_labels.Add(key, new DGUIElement(label));
            return m_labels[key];
        }

        public DGUIElement AddLabel(string text) { return AddLabel(new GUIContent(text)); }

        private DGUIElement Get(SerializedProperty property) { return m_elements[property.propertyPath]; }
        public DGUILayoutElement GetLayout(DGUIElement element, float weight, DGUIElement.DrawMode drawMode = DGUIElement.DrawMode.LabelAndField) { return new DGUILayoutElement(element, weight, drawMode); }
        public DGUILayoutElement GetLayout(DGUIElement element, DGUIElement.DrawMode drawMode) { return new DGUILayoutElement(element, 1f, drawMode); }
        public DGUILayoutElement GetLayout(SerializedProperty property, float weight, DGUIElement.DrawMode drawMode = DGUIElement.DrawMode.LabelAndField) { return new DGUILayoutElement(Get(property), weight, drawMode); }
        public DGUILayoutElement GetLayout(SerializedProperty property, DGUIElement.DrawMode drawMode) { return new DGUILayoutElement(Get(property), 1f, drawMode); }

        private DGUIElement GetLabel(GUIContent label, GUIStyle style) { return m_labels[LabelKey(label.text, style)]; }
        private DGUIElement GetLabel(string text, GUIStyle style) { return m_labels[LabelKey(text, style)]; }
        public DGUILayoutElement GetLayout(GUIContent label, GUIStyle style) { return new DGUILayoutElement(GetLabel(label, style), 1f, DGUIElement.DrawMode.Label); }
        public DGUILayoutElement GetLayout(GUIContent label) { return new DGUILayoutElement(GetLabel(label, DGUIElement.DefaultLabelStyle), 1f, DGUIElement.DrawMode.Label); }
        public DGUILayoutElement GetLayout(string text, GUIStyle style) { return new DGUILayoutElement(GetLabel(text, style), 1f, DGUIElement.DrawMode.Label); }
        public DGUILayoutElement GetLayout(string text) { return new DGUILayoutElement(GetLabel(text, DGUIElement.DefaultLabelStyle), 1f, DGUIElement.DrawMode.Label); }

        public void Clear()
        {
            m_elements.Clear();
            m_labels.Clear();
        }

        public void DrawLine(Rect drawRect, params DGUILayoutElement[] layoutElements)
        {
            if (drawRect.width < 2 || layoutElements == null || layoutElements.Length == 0) return;

            float totalEmptySpaceWidth = DGUI.Properties.StandardVerticalSpacing * 1;
            float totalLabelsWidth = 0;
            foreach (DGUILayoutElement layout in layoutElements)
                switch (layout.DrawMode)
                {
                    case DGUIElement.DrawMode.Label:
                        if (layout.Element.HasLabel)
                        {
                            totalEmptySpaceWidth += DGUI.Properties.StandardVerticalSpacing;
                            totalLabelsWidth += layout.Element.LabelWidth;
                        }

                        break;
                    case DGUIElement.DrawMode.Field:
                        if (layout.Element.HasField)
                        {
                            totalEmptySpaceWidth += DGUI.Properties.StandardVerticalSpacing;
                            if (layout.Element.HasFixedFieldWidth) totalLabelsWidth += layout.Element.FixedFieldWidth;
                        }

                        break;
                    case DGUIElement.DrawMode.LabelAndField:
                    case DGUIElement.DrawMode.FieldAndLabel:
                        if (layout.Element.HasLabel)
                        {
                            totalEmptySpaceWidth += DGUI.Properties.StandardVerticalSpacing;
                            totalLabelsWidth += layout.Element.LabelWidth;
                        }

                        if (layout.Element.HasField)
                        {
                            totalEmptySpaceWidth += DGUI.Properties.StandardVerticalSpacing;
                            if (layout.Element.HasFixedFieldWidth) totalLabelsWidth += layout.Element.FixedFieldWidth;
                        }

                        if (layout.Element.HasLabel && layout.Element.HasField) totalEmptySpaceWidth += DGUI.Properties.StandardVerticalSpacing;
                        break;
                }

            float totalFieldsWidth = drawRect.width - totalLabelsWidth - totalEmptySpaceWidth;
            float x = drawRect.x + DGUI.Properties.StandardVerticalSpacing;
            foreach (DGUILayoutElement layout in layoutElements)
            {
                x += layout.Element.Draw(x, drawRect.y, totalFieldsWidth, drawRect.height, layout.Weight, layout.DrawMode);
                x += DGUI.Properties.StandardVerticalSpacing;
                if (layout.DrawMode != DGUIElement.DrawMode.Label &&
                    layout.DrawMode != DGUIElement.DrawMode.Field &&
                    layout.Element.HasLabel)
                    x += DGUI.Properties.StandardVerticalSpacing;
            }
        }
    }
}