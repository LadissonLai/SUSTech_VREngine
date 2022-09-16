// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.UI;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI
{
    [CustomEditor(typeof(UIDrawerArrowAnimator))]
    public class ArrowAnimatorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.UIDrawerColorName; } }
        
#pragma warning disable 0414 //The private field is assigned but its value is never used
        private UIDrawerArrowAnimator m_target;
#pragma warning restore 0414

        private SerializedProperty
            m_rotator,
            m_leftBar,
            m_rightBar;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_rotator = GetProperty(PropertyName.Rotator);
            m_leftBar = GetProperty(PropertyName.LeftBar);
            m_rightBar = GetProperty(PropertyName.RightBar);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_target = (UIDrawerArrowAnimator) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DGUI.Property.Draw(m_rotator, "Rotator", ComponentColorName, ComponentColorName, m_rotator.objectReferenceValue == null);
            GUILayout.Space(1);
            DGUI.Property.Draw(m_leftBar, "Left Bar", ComponentColorName, ComponentColorName, m_leftBar.objectReferenceValue == null);
            GUILayout.Space(1);
            DGUI.Property.Draw(m_rightBar, "Right Bar", ComponentColorName, ComponentColorName, m_rightBar.objectReferenceValue == null);
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
    }
}