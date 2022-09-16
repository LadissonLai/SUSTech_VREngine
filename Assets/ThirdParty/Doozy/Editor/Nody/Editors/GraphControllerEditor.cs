// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Nody;
using Doozy.Editor.Nody.Windows;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(GraphController), true)]
    public class GraphControllerEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.GraphControllerColorName; } }
        private GraphController m_target;

        private GraphController Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (GraphController) target;
                return m_target;
            }
        }

        protected override bool HasErrors { get { return base.HasErrors || m_errorNoGraphReferenced || m_errorReferencedGraphIsSubGraph; } }
        private bool m_errorNoGraphReferenced;
        private bool m_errorReferencedGraphIsSubGraph;

        private SerializedProperty
            m_controllerName,
            m_dontDestroyControllerOnLoad;

        private InfoMessage
            m_infoMessageErrorNoGraphReferenced,
            m_infoMessageErrorReferencedGraphIsSubGraph;

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_controllerName = GetProperty(PropertyName.ControllerName);
            m_dontDestroyControllerOnLoad = GetProperty(PropertyName.DontDestroyControllerOnLoad);
        }

        public override bool RequiresConstantRepaint() { return true; }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_infoMessageErrorNoGraphReferenced = new InfoMessage(InfoMessage.MessageType.Error, UILabels.NoGraphReferencedTitle, UILabels.NoGraphReferencedMessage);
            m_infoMessageErrorReferencedGraphIsSubGraph = new InfoMessage(InfoMessage.MessageType.Error, UILabels.ReferencedGraphIsSubGraphTitle, UILabels.ReferencedGraphIsSubGraphMessage);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_errorNoGraphReferenced = Target.GraphModel == null;
            m_errorReferencedGraphIsSubGraph = Target.GraphModel != null && Target.GraphModel.IsSubGraph;
            serializedObject.Update();
            DrawHeader(Nody.Styles.GetStyle(Nody.Styles.StyleName.ComponentHeaderGraphController), MenuUtils.GraphController_Manual, MenuUtils.GraphController_YouTube);
            DrawDebugModeAndDontDestroyOnLoad();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawControllerName();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawGraphModel();
            if (Target.GraphModel != null && !Target.GraphModel.IsSubGraph)
            {
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Doozy.DrawRenameGameObjectButton("Graph Controller - ", Target.GraphModel.name, "", Target.gameObject);
            }

            DrawInfoMessages();
            GUILayout.Space(DGUI.Properties.Space());
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDebugModeAndDontDestroyOnLoad()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Toggle.Switch.Draw(GetProperty(PropertyName.DebugMode), UILabels.DebugMode, ColorName.Red, true, false);
                GUILayout.FlexibleSpace();
                DGUI.Toggle.Switch.Draw(m_dontDestroyControllerOnLoad, UILabels.DontDestroyGameObjectOnLoad, ComponentColorName, true, false);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawControllerName() { DGUI.Property.Draw(m_controllerName, UILabels.ControllerName, ComponentColorName); }

        private void DrawGraphModel()
        {
            GUILayout.BeginHorizontal();
            {
                DGUI.Property.Draw(GetProperty(PropertyName.m_graphModel), UILabels.GraphModel,
                                   HasErrors ? ColorName.Red : ColorName.White,
                                   HasErrors ? ColorName.Red : DGUI.Colors.DisabledTextColorName);
                
                GUILayout.Space(DGUI.Properties.Space());
                
                if (!HasErrors)
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconNody), UILabels.OpenGraph, Size.S, TextAlign.Left, ComponentColorName, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        NodyWindow.Instance.LoadGraph(Target.GraphModel);
                }
                else
                {
                    if (DGUI.Button.Dynamic.DrawIconButton(Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconNody), UILabels.OpenNody, Size.S, TextAlign.Left, ComponentColorName, ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                        NodyWindow.Open();
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawInfoMessages()
        {
            m_infoMessageErrorNoGraphReferenced.Draw(m_errorNoGraphReferenced, InspectorWidth);
            if (m_errorNoGraphReferenced) return;
            m_infoMessageErrorReferencedGraphIsSubGraph.Draw(m_errorReferencedGraphIsSubGraph, InspectorWidth);
        }
    }
}