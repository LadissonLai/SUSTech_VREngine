// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Editors;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(TimeScaleNode))]
    public class TimeScaleNodeEditor : BaseNodeEditor
    {
        private BackButtonNode TargetNode { get { return (BackButtonNode) target; } }

        private SerializedProperty
            m_targetValue,
            m_animateValue,
            m_animationDuration,
            m_animationEase,
            m_waitForAnimationToFinish;

        private AnimBool
            m_animateValueExpanded;
        
        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();
            
            m_targetValue = GetProperty(PropertyName.TargetValue);
            m_animateValue = GetProperty(PropertyName.AnimateValue);
            m_animationDuration = GetProperty(PropertyName.AnimationDuration);
            m_animationEase = GetProperty(PropertyName.AnimationEase);
            m_waitForAnimationToFinish = GetProperty(PropertyName.WaitForAnimationToFinish);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();
            
            m_animateValueExpanded = GetAnimBool(m_animateValue.propertyPath, m_animateValue.boolValue);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderTimeScaleNode), MenuUtils.TimeScaleNode_Manual, MenuUtils.TimeScaleNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            DrawOptions();
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOptions()
        {
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawTargetValue();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawAnimateValue();
        }

        private void DrawTargetValue()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_animateValue, UILabels.AnimateValue, DGUI.Colors.ActionColorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Property.Draw(m_targetValue, UILabels.TargetValue, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawAnimateValue()
        {
            m_animateValueExpanded.target = m_animateValue.boolValue;
            if (DGUI.FadeOut.Begin(m_animateValueExpanded, false))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(DGUI.Properties.Space());
                    GUILayout.BeginHorizontal(GUILayout.Width(DGUI.Properties.DefaultFieldWidth));
                    DGUI.Property.Draw(m_animationDuration, UILabels.Duration, DGUI.Colors.ActionColorName);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space());
                    DGUI.Property.Draw(m_animationEase, UILabels.Ease, DGUI.Colors.ActionColorName);
                    GUILayout.Space(DGUI.Properties.Space());
                    DGUI.Toggle.Switch.Draw(m_waitForAnimationToFinish, UILabels.WaitForAnimationToFinish, DGUI.Colors.ActionColorName, true, false);
                }
                GUILayout.EndHorizontal();
            }

            DGUI.FadeOut.End(m_animateValueExpanded, false);
        }
    }
}