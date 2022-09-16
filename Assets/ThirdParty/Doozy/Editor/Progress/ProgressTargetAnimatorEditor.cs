// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressTargetAnimator))]
    [CanEditMultipleObjects]
    public class ProgressTargetAnimatorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }

        private const string PARAMETER_INFO = "ParameterInfo";

        private SerializedProperty
            m_animator,
            m_parameterName,
            m_targetProgress;

        private bool HasReference { get { return m_animator.objectReferenceValue != null; } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_animator = GetProperty(PropertyName.Animator);
            m_parameterName = GetProperty(PropertyName.ParameterName);
            m_targetProgress = GetProperty(PropertyName.TargetProgress);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(PARAMETER_INFO,
                           new InfoMessage(InfoMessage.MessageType.Info, UILabels.ProgressTargetAnimatorParameterInfo, true, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTargetAnimator), MenuUtils.ProgressTargetAnimator_Manual, MenuUtils.ProgressTargetAnimator_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));

            bool hasReference = HasReference;
            ColorName colorName = hasReference ? ComponentColorName : ColorName.Red;

            DGUI.Property.Draw(m_animator, UILabels.Animator, colorName);
            GUI.enabled = hasReference;

            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_parameterName, UILabels.ParameterName, colorName);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_targetProgress, UILabels.TargetProgress, colorName);

            GUI.enabled = true;

            GUILayout.Space(DGUI.Properties.Space(4));
            GetInfoMessage(PARAMETER_INFO).Draw(true, InspectorWidth);

            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }
    }
}