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
    [CustomEditor(typeof(ProgressTargetTextMeshPro))]
    [CanEditMultipleObjects]
    public class ProgressTargetTextMeshProEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }

        private const string SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED = "SupportForTextMeshProNotEnabled";
        
#if dUI_TextMeshPro
        private SerializedProperty
            m_textMeshPro,
            m_wholeNumbers,
            m_useMultiplier,
            m_multiplier,
            m_prefix,
            m_suffix,
            m_targetVariable;

        private bool HasReference { get { return m_textMeshPro.objectReferenceValue != null; } }
#endif

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

#if dUI_TextMeshPro
            m_textMeshPro = GetProperty(PropertyName.TextMeshPro);
            m_wholeNumbers = GetProperty(PropertyName.WholeNumbers);
            m_useMultiplier = GetProperty(PropertyName.UseMultiplier);
            m_multiplier = GetProperty(PropertyName.Multiplier);
            m_prefix = GetProperty(PropertyName.Prefix);
            m_suffix = GetProperty(PropertyName.Suffix);
            m_targetVariable = GetProperty(PropertyName.TargetVariable);
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            
            AddInfoMessage(SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED, new InfoMessage(InfoMessage.MessageType.Warning, UILabels.SupportForTextMeshProNotEnabled, false, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTargetTextMeshPro), MenuUtils.ProgressTargetTextMeshPro_Manual, MenuUtils.ProgressTargetTextMeshPro_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));

#if dUI_TextMeshPro
            bool hasReference = HasReference;
            ColorName colorName = hasReference ? ComponentColorName : ColorName.Red;

            DGUI.Property.Draw(m_textMeshPro, UILabels.TextMeshPro, colorName);
            GUI.enabled = hasReference;

            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(m_targetVariable, UILabels.TargetVariable, colorName);
                switch ((TargetVariable) m_targetVariable.enumValueIndex)
                {
                    case TargetVariable.Value:
                    case TargetVariable.Progress:
                    case TargetVariable.InverseProgress:
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Toggle.Checkbox.Draw(m_wholeNumbers, UILabels.WholeNumbers, colorName, true, false);
                        break;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(m_useMultiplier, UILabels.UseMultiplier, colorName, true, false);
                GUILayout.Space(DGUI.Properties.Space());

                bool enabled = GUI.enabled;
                GUI.enabled = m_useMultiplier.boolValue;
                DGUI.Property.Draw(m_multiplier, UILabels.Multiplier, DGUI.Colors.GetTextColorName(m_useMultiplier.boolValue, colorName));
                GUI.enabled = enabled;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_prefix, UILabels.Prefix, colorName);
            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_suffix, UILabels.Suffix, colorName);

            GUI.enabled = true;

#else
            GetInfoMessage(SUPPORT_FOR_TEXT_MESH_PRO_NOT_ENABLED).Draw(true, InspectorWidth);
#endif
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }
    }
}