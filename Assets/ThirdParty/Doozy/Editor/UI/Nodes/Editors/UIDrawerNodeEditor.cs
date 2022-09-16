// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Editors;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(UIDrawerNode))]
    public class UIDrawerNodeEditor : BaseNodeEditor
    {
        private const string ERROR_NO_DRAWER_NAME = "ErrorNoDrawerName";

        private UIDrawerNode TargetNode { get { return (UIDrawerNode) target; } }

        private static NamesDatabase Database { get { return UIDrawerSettings.Database; } }

        private SerializedProperty
            m_drawerName,
            m_customDrawerName,
            m_action;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_drawerName = GetProperty(PropertyName.DrawerName);
            m_customDrawerName = GetProperty(PropertyName.CustomDrawerName);
            m_action = GetProperty(PropertyName.Action);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(ERROR_NO_DRAWER_NAME, new InfoMessage(InfoMessage.MessageType.Error, UILabels.MissingDrawerNameTitle, UILabels.MissingDrawerNameMessage, TargetNode.ErrorNoDrawerName, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIDrawerNode), MenuUtils.UIDrawerNode_Manual, MenuUtils.UIDrawerNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets(BaseNode);
            GUILayout.Space(DGUI.Properties.Space(16));
            EditorGUI.BeginChangeCheck();
            DrawOptions();
            if (EditorGUI.EndChangeCheck()) NodeUpdated = true;
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOptions()
        {
            ColorName backgroundColorName = DGUI.Colors.ActionColorName;
            ColorName textColorName = DGUI.Colors.ActionColorName;
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(ERROR_NO_DRAWER_NAME).Draw(TargetNode.ErrorNoDrawerName, InspectorWidth);
            DGUI.Property.Draw(m_action, UILabels.Action, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            DGUI.Database.DrawItemsDatabaseSelectorForGeneralCategoryOnly(UIDrawer.DefaultDrawerCategory, m_drawerName, UILabels.DrawerName, m_customDrawerName, Database, backgroundColorName);
        }
    }
}