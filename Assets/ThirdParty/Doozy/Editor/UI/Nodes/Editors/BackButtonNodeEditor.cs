// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Editors;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(BackButtonNode))]
    public class BackButtonNodeEditor : BaseNodeEditor
    {
        private BackButtonNode TargetNode { get { return (BackButtonNode) target; } }

        private SerializedProperty
            m_backButtonAction;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_backButtonAction = GetProperty(PropertyName.BackButtonAction);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderBackButtonNode), MenuUtils.BackButtonNode_Manual, MenuUtils.BackButtonNode_YouTube);
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
            DGUI.Property.Draw(m_backButtonAction, UILabels.BackButton, DGUI.Colors.ActionColorName, DGUI.Colors.ActionColorName);
        }
    }
}