// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Internal;
using Doozy.Engine.Events;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(AnimatorEvent), true)]
    public class AnimatorEventDrawer : BaseDrawer
    {
        private readonly Dictionary<string, bool> m_initialized = new Dictionary<string, bool>();

        private static DColor DrawerColor { get { return EditorColors.Instance.Gray; } }


        private void Init(SerializedProperty property)
        {
            if (m_initialized.ContainsKey(property.propertyPath) && m_initialized[property.propertyPath]) return;

            NumberOfLines.Add(property.propertyPath, 1);

            Elements.AddLabel(Contents.Add(AnimatorEvent.ParameterType.Bool.ToString().ToLower()));
            Elements.AddLabel(Contents.Add(AnimatorEvent.ParameterType.Float.ToString().ToLower()));
            Elements.AddLabel(Contents.Add(AnimatorEvent.ParameterType.Int.ToString().ToLower()));
            Elements.AddLabel(Contents.Add(AnimatorEvent.ParameterType.Trigger.ToString().ToLower()));

            Elements.AddLabel(Contents.Add(UILabels.TriggerName));

            Elements.Add(Properties.Add(PropertyName.Animator, property), Contents.Add(UILabels.TargetAnimator));
            Elements.Add(Properties.Add(PropertyName.TargetParameterType, property), Contents.Add(UILabels.ParameterType));
            Elements.Add(Properties.Add(PropertyName.ParameterName, property), Contents.Add(UILabels.ParameterName));
            Elements.Add(Properties.Add(PropertyName.BoolValue, property), Contents.Add(UILabels.SetBoolValueTo), 0f, DGUI.Toggle.Checkbox.Width);
            Elements.Add(Properties.Add(PropertyName.FloatValue, property), Contents.Add(UILabels.SetFloatValueTo));
            Elements.Add(Properties.Add(PropertyName.IntValue, property), Contents.Add(UILabels.SetIntValueTo));
            Elements.Add(Properties.Add(PropertyName.ResetTrigger, property), Contents.Add(UILabels.ResetTrigger), 0f, DGUI.Toggle.Checkbox.Width);

            UpdateParameters(property);

            if (!m_initialized.ContainsKey(property.propertyPath))
                m_initialized.Add(property.propertyPath, true);
            else
                m_initialized[property.propertyPath] = true;
        }

        private void UpdateParameters(SerializedProperty property)
        {
            SerializedProperty animator = Properties.Get(PropertyName.Animator, property);
            SerializedProperty parameterName = Properties.Get(PropertyName.ParameterName, property);

            if (animator == null ||                    //if the property is null
                animator.objectReferenceValue == null) //or if no animator has been referenced
                return;                                //stop here

            var anim = (Animator) animator.objectReferenceValue;
            if (!anim.gameObject.activeSelf) return;

            AnimatorControllerParameter[] parameters = anim.parameters; //get the parameters from the animator
            if (parameters == null || parameters.Length == 0) return;   //if the animator does not have any parameters -> stop here

            List<string> parameterNames = parameters.Select(parameter => parameter.name).ToList();

            if (!parameterNames.Contains(parameterName.stringValue)) //check if the previously set parameter name can be found in the parameter names list
            {
                parameterName.stringValue = parameterNames[0]; //update the parameter name value
                property.serializedObject.ApplyModifiedProperties();
            }

            UpdateParameterType(property, parameterName, parameters);
        }

        private void UpdateParameterType(SerializedProperty property, SerializedProperty parameterName, AnimatorControllerParameter[] parameters)
        {
            foreach (AnimatorControllerParameter parameter in parameters)
            {
                if (!parameter.name.Equals(parameterName.stringValue)) continue;

                SerializedProperty parameterType = Properties.Get(PropertyName.TargetParameterType, property);

                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Float:
                        parameterType.enumValueIndex = (int) AnimatorEvent.ParameterType.Float;
                        break;
                    case AnimatorControllerParameterType.Int:
                        parameterType.enumValueIndex = (int) AnimatorEvent.ParameterType.Int;
                        break;
                    case AnimatorControllerParameterType.Bool:
                        parameterType.enumValueIndex = (int) AnimatorEvent.ParameterType.Bool;
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        parameterType.enumValueIndex = (int) AnimatorEvent.ParameterType.Trigger;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
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

                Draw(position, property);

                // set indent back to what it was
                EditorGUI.indentLevel = indent;
            }
            EditorGUI.EndProperty();
        }


        private void Draw(Rect position, SerializedProperty property)
        {
            Color initialColor = GUI.color; //save the GUI color

            string key = property.propertyPath;
            SerializedProperty animatorProperty = Properties.Get(PropertyName.Animator, property);
            var animator = (Animator) animatorProperty.objectReferenceValue;
            var parameterNames = new List<string>();
            AnimatorControllerParameter[] parameters = null;
            bool hasAnimator = animatorProperty.objectReferenceValue != null;
            bool hasController = hasAnimator && animator.runtimeAnimatorController != null;
            if (hasAnimator && hasController)
            {
                parameters = animator.parameters; //get the parameters from the animator
                parameterNames = parameters.Select(parameter => parameter.name).ToList();
            }

            bool hasParameters = parameterNames.Count > 0;
            NumberOfLines[key] = 1 + (hasAnimator ? 1 : 0) + (hasAnimator && hasParameters ? 1 : 0);

            Rect drawRect = GetDrawRectAndDrawBackground(position,
                                                         NumberOfLines[key],
                                                         NumberOfLines[key] == 1
                                                             ? ColorName.Red
                                                             : DGUI.Utility.IsProSkin
                                                                 ? ColorName.Gray
                                                                 : ColorName.White); //calculate draw rect and draw background


            drawRect.x += DGUI.Properties.Space(2);
            drawRect.width -= DGUI.Properties.Space(3);

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            GUI.color = hasAnimator ? GUI.color : DGUI.Colors.BackgroundColor(ColorName.Red);

            EditorGUI.BeginChangeCheck();
            Elements.DrawLine(drawRect,
                              Elements.GetLayout(animatorProperty, 1f));

            if (EditorGUI.EndChangeCheck())
                if (animatorProperty.objectReferenceValue != null)
                    UpdateParameters(property);

            if (hasAnimator)
            {
                //LINE 2
                drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
                DrawParameterSelector(drawRect, property, parameters, parameterNames, true, hasController);

                if (hasParameters)
                {
                    //LINE 3
                    drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;

                    switch ((AnimatorEvent.ParameterType) Properties.Get(PropertyName.TargetParameterType, property).enumValueIndex)
                    {
                        case AnimatorEvent.ParameterType.Float:
                            Elements.DrawLine(drawRect,
                                              Elements.GetLayout(Properties.Get(PropertyName.FloatValue, property), 1f));
                            break;
                        case AnimatorEvent.ParameterType.Int:
                            Elements.DrawLine(drawRect,
                                              Elements.GetLayout(Properties.Get(PropertyName.IntValue, property), 1f));
                            break;
                        case AnimatorEvent.ParameterType.Bool:
                            Elements.DrawLine(drawRect,
                                              Elements.GetLayout(Properties.Get(PropertyName.BoolValue, property), 0f));
                            break;
                        case AnimatorEvent.ParameterType.Trigger:
                            Elements.DrawLine(drawRect,
                                              Elements.GetLayout(Properties.Get(PropertyName.ResetTrigger, property), 0f));
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                }
            }

            GUI.color = initialColor; //restore the GUI color
        }

        private void DrawParameterSelector(Rect drawRect, SerializedProperty property, AnimatorControllerParameter[] parameters, List<string> parameterNames, bool hasAnimator, bool hasController)
        {
            if (parameterNames.Count > 0)
            {
                SerializedProperty parameterName = Properties.Get(PropertyName.ParameterName, property);

                string parameterTypeName = ("(" + (AnimatorEvent.ParameterType) Properties.Get(PropertyName.TargetParameterType, property).enumValueIndex + ") ").ToLower();
                float parameterTypeNameLabelWidth = DGUI.Label.Style().CalcSize(new GUIContent(parameterTypeName)).x;
                var parameterTypeNameRect = new Rect(drawRect.x + DGUI.Properties.StandardVerticalSpacing, drawRect.y, parameterTypeNameLabelWidth, DGUI.Properties.SingleLineHeight);
                DGUI.Label.Draw(parameterTypeNameRect, parameterTypeName, Size.M);

                var selectorRect = new Rect(drawRect.x + parameterTypeNameLabelWidth, drawRect.y + 1, drawRect.width - parameterTypeNameLabelWidth - DGUI.Properties.StandardVerticalSpacing, DGUI.Properties.SingleLineHeight);
                int selectedParameterIndex = parameterNames.IndexOf(parameterName.stringValue);
                string[] parametersNames = parameterNames.ToArray();

                EditorGUI.BeginChangeCheck();
                selectedParameterIndex = EditorGUI.Popup(selectorRect, selectedParameterIndex, parametersNames);

                if (!EditorGUI.EndChangeCheck()) return;
                parameterName.stringValue = parameterNames[selectedParameterIndex];
                property.serializedObject.ApplyModifiedProperties();
                UpdateParameterType(property, Properties.Get(PropertyName.ParameterName, property), parameters);
            }
            else
            {
                var labelRect = new Rect(drawRect.x + DGUI.Properties.StandardVerticalSpacing, drawRect.y, drawRect.width - DGUI.Properties.StandardVerticalSpacing, DGUI.Properties.SingleLineHeight);
                DGUI.Label.Draw(labelRect,
                                !hasAnimator
                                    ? UILabels.NoAnimatorFound
                                    : !hasController
                                        ? UILabels.TargetAnimatorDoesNotHaveAnAnimatorController
                                        : UILabels.TargetAnimatorDoesNotHaveAnyParameters,
                                Size.M);
            }
        }
    }
}