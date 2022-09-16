// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressTargetImage))]
    [CanEditMultipleObjects]
    public class ProgressTargetImageEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }

        private SerializedProperty
            m_image,
            m_targetProgress;

        private bool HasReference { get { return m_image.objectReferenceValue != null; } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_image = GetProperty(PropertyName.Image);
            m_targetProgress = GetProperty(PropertyName.TargetProgress);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTargetImage), MenuUtils.ProgressTargetImage_Manual, MenuUtils.ProgressTargetImage_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));
            
            bool hasReference = HasReference;
            ColorName colorName = hasReference ? ComponentColorName : ColorName.Red;
            
            DGUI.Property.Draw(m_image, UILabels.Image, colorName);
            GUI.enabled = hasReference;
            
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_targetProgress, UILabels.TargetProgress, colorName);
            
            GUI.enabled = true;
            
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }
    }
}