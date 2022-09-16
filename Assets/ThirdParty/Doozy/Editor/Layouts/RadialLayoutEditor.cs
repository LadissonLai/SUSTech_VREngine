// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Layouts;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Layouts
{
    [CustomEditor(typeof(RadialLayout))]
    public class RadialLayoutEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.RadialLayoutColorName; } }
        private RadialLayout m_target;

        private RadialLayout Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (RadialLayout) target;
                return m_target;
            }
        }

        private SerializedProperty
            m_autoRebuild,
            m_childHeight,
            m_childRotation,
            m_childWidth,
            m_clockwise,
            m_controlChildHeight,
            m_controlChildWidth,
            m_maxAngle,
            m_maxRadius,
            m_minAngle,
            m_radius,
            m_radiusControlsHeight,
            m_radiusControlsWidth,
            m_radiusHeightFactor,
            m_radiusWidthFactor,
            m_rotateChildren,
            m_spacing,
            m_startAngle;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_autoRebuild = GetProperty(PropertyName.m_AutoRebuild);
            m_childHeight = GetProperty(PropertyName.m_ChildHeight);
            m_childRotation = GetProperty(PropertyName.m_ChildRotation);
            m_childWidth = GetProperty(PropertyName.m_ChildWidth);
            m_clockwise = GetProperty(PropertyName.m_Clockwise);
            m_controlChildHeight = GetProperty(PropertyName.m_ControlChildHeight);
            m_controlChildWidth = GetProperty(PropertyName.m_ControlChildWidth);
            m_maxAngle = GetProperty(PropertyName.m_MaxAngle);
            m_maxRadius = GetProperty(PropertyName.m_MaxRadius);
            m_minAngle = GetProperty(PropertyName.m_MinAngle);
            m_radius = GetProperty(PropertyName.m_Radius);
            m_radiusControlsHeight = GetProperty(PropertyName.m_RadiusControlsHeight);
            m_radiusControlsWidth = GetProperty(PropertyName.m_RadiusControlsWidth);
            m_radiusHeightFactor = GetProperty(PropertyName.m_RadiusHeightFactor);
            m_radiusWidthFactor = GetProperty(PropertyName.m_RadiusWidthFactor);
            m_rotateChildren = GetProperty(PropertyName.m_RotateChildren);
            m_spacing = GetProperty(PropertyName.m_Spacing);
            m_startAngle = GetProperty(PropertyName.m_StartAngle);
        }

        protected override void InitAnimBool() { base.InitAnimBool(); }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderRadialLayout), MenuUtils.RadialLayout_Manual, MenuUtils.RadialLayout_YouTube);

            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconFaCircleNotch),
                                                      UILabels.Settings,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUI.BeginChangeCheck();
            {
                DrawRadius();
                GUILayout.Space(DGUI.Properties.Space());
                DrawMinMaxStartAngles();
                GUILayout.Space(DGUI.Properties.Space());
                DrawSpacing();
                GUILayout.Space(DGUI.Properties.Space());
                DrawClockwiseAndAutoRebuild();
                GUILayout.Space(DGUI.Properties.Space(4));
                DrawChildRotation();
                GUILayout.Space(DGUI.Properties.Space(4));
                DrawChildWidth();
                GUILayout.Space(DGUI.Properties.Space(4));
                DrawChildHeight();
            }
            if (EditorGUI.EndChangeCheck()) Target.CalculateRadial();
            
            GUILayout.Space(DGUI.Properties.Space(2));

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRadius()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (m_radius.floatValue > m_maxRadius.floatValue) m_radius.floatValue = m_maxRadius.floatValue;
                DrawSlider(m_radius, UILabels.Radius, 0, m_maxRadius.floatValue);
                GUILayout.Space(DGUI.Properties.Space());
                EditorGUI.BeginChangeCheck();
                DGUI.Property.Draw(m_maxRadius, UILabels.MaxRadius, ComponentColorName, ComponentColorName, DGUI.Properties.DefaultFieldWidth * 2);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_maxRadius.floatValue <= RadialLayout.RADIUS_DEFAULT_VALUE) m_maxRadius.floatValue = RadialLayout.RADIUS_DEFAULT_VALUE;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMinMaxStartAngles()
        {
            DrawSlider(m_minAngle, UILabels.MinAngle, RadialLayout.MIN_ANGLE, RadialLayout.MAX_ANGLE);
            GUILayout.Space(DGUI.Properties.Space());
            DrawSlider(m_maxAngle, UILabels.MaxAngle, RadialLayout.MIN_ANGLE, RadialLayout.MAX_ANGLE);
            GUILayout.Space(DGUI.Properties.Space());
            DrawSlider(m_startAngle, UILabels.StartAngle, RadialLayout.MIN_ANGLE, RadialLayout.MAX_ANGLE);
        }

        private void DrawSpacing() { DGUI.Property.Draw(m_spacing, UILabels.Spacing, ComponentColorName, ComponentColorName); }

        private void DrawClockwiseAndAutoRebuild()
        {
            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_clockwise, UILabels.Clockwise, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Toggle.Switch.Draw(m_autoRebuild, UILabels.AutoRebuild, ComponentColorName, true, false);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawChildRotation()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconReset),
                                                      UILabels.ChildRotation,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_rotateChildren, UILabels.RotateChildren, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DrawSlider(m_childRotation, UILabels.ChildRotation, RadialLayout.MIN_ANGLE, RadialLayout.MAX_ANGLE);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawChildWidth()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconFaArrowsAltH),
                                                      UILabels.ChildWidth,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_controlChildWidth, UILabels.ControlChildWidth, ComponentColorName, true, false);
                GUI.enabled = m_controlChildWidth.boolValue;
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(m_childWidth, UILabels.ChildWidth, ComponentColorName, ComponentColorName);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_radiusControlsWidth, UILabels.RadiusControlsWidth, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                bool enabled = GUI.enabled;
                GUI.enabled = m_radiusControlsWidth.boolValue;
                DrawSlider(m_radiusWidthFactor, UILabels.RadiusWidthFactor, 0, 100);
                GUI.enabled = enabled;
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private void DrawChildHeight()
        {
            DGUI.Doozy.DrawTitleWithIconAndBackground(Styles.GetStyle(Styles.StyleName.IconFaArrowsAltV),
                                                      UILabels.ChildHeight,
                                                      Size.L,
                                                      DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                      ComponentColorName,
                                                      ComponentColorName);

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_controlChildHeight, UILabels.ControlChildHeight, ComponentColorName, true, false);
                GUI.enabled = m_controlChildHeight.boolValue;
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(m_childHeight, UILabels.ChildHeight, ComponentColorName, ComponentColorName);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(DGUI.Properties.Space());

            EditorGUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_radiusControlsHeight, UILabels.RadiusControlsHeight, ComponentColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                bool enabled = GUI.enabled;
                GUI.enabled = m_radiusControlsHeight.boolValue;
                DrawSlider(m_radiusHeightFactor, UILabels.RadiusHeightFactor, 0, 100);
                GUI.enabled = enabled;
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;
        }

        private void DrawSlider(SerializedProperty property, string propertyName, float minValue, float maxValue)
        {
            DGUI.Line.Draw(false, ComponentColorName, true, DGUI.Properties.SingleLineHeight,
                           () =>
                           {
                               GUILayout.Space(DGUI.Properties.Space(2));
                               DGUI.Label.Draw(propertyName, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                               GUILayout.Space(DGUI.Properties.Space());
                               GUILayout.BeginVertical(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                               {
                                   GUILayout.Space(0f);
                                   GUI.color = DGUI.Colors.PropertyColor(ComponentColorName);
                                   EditorGUILayout.Slider(property, minValue, maxValue, GUIContent.none);
                                   GUI.color = InitialGUIColor;
                               }
                               GUILayout.EndVertical();
                           }
                          );
        }
    }
}