// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Windows;
using Doozy.Engine.Orientation;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Orientation
{
    [CustomEditor(typeof(OrientationDetector))]
    public class OrientationDetectorEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.OrientationDetectorColorName; } }
        private OrientationDetector m_target;

        private OrientationDetector Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (OrientationDetector) target;
                return m_target;
            }
        }

        private SerializedProperty m_onOrientationEventProperty;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();
            
            m_onOrientationEventProperty = GetProperty(PropertyName.OnOrientationEvent);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderOrientationDetector), MenuUtils.OrientationDetector_Manual, MenuUtils.OrientationDetector_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Property.UnityEvent(m_onOrientationEventProperty, "OnOrientationEvent", ComponentColorName, Target.OnOrientationEvent.GetPersistentEventCount());
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawGeneralSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconDoozy),
                                                   UILabels.GeneralSettings,
                                                   Size.S, TextAlign.Left,
                                                   DGUI.Colors.DisabledBackgroundColorName,
                                                   DGUI.Colors.DisabledTextColorName,
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2),
                                                   false))
                DoozyWindow.Open(DoozyWindow.View.General);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

}