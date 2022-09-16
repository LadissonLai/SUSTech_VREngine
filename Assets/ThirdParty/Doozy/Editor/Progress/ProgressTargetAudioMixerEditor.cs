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
    [CustomEditor(typeof(ProgressTargetAudioMixer))]
    [CanEditMultipleObjects]
    public class ProgressTargetAudioMixerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }

        private SerializedProperty
            m_targetMixer,
            m_exposedParameterName,
            m_useLogarithmicConversion;
        
        private bool HasReference { get { return m_targetMixer.objectReferenceValue != null; } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_targetMixer = GetProperty(PropertyName.TargetMixer);
            m_exposedParameterName = GetProperty(PropertyName.ExposedParameterName);
            m_useLogarithmicConversion = GetProperty(PropertyName.UseLogarithmicConversion);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTargetAudioMixer), MenuUtils.ProgressTargetAudioMixer_Manual, MenuUtils.ProgressTargetAudioMixer_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));
            
            bool hasReference = HasReference;
            ColorName colorName = hasReference ? ComponentColorName : ColorName.Red;
            
            DGUI.Property.Draw(m_targetMixer, UILabels.TargetMixer, colorName);
            GUILayout.Space(DGUI.Properties.Space());
            
            GUI.enabled = hasReference;
            DGUI.Property.Draw(m_exposedParameterName, UILabels.ExposedParameterName, colorName);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Toggle.Switch.Draw(m_useLogarithmicConversion, UILabels.UseLogarithmicConversion, colorName, true, false);
            GUI.enabled = true;
            
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }
    }
}