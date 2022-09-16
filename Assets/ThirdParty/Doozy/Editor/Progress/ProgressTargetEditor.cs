// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Progress;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressTarget))]
    public class ProgressTargetEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }
        private ProgressTarget Target { get { return (ProgressTarget) target; } }

        private const string INFO = "Info";
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            AddInfoMessage(INFO, new InfoMessage(InfoMessage.MessageType.Warning, UILabels.ThisClassShouldBeExtended, true, Repaint));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTarget));
            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(INFO).Draw(true, InspectorWidth);
            GUILayout.Space(DGUI.Properties.Space(4));
        }
    }
}