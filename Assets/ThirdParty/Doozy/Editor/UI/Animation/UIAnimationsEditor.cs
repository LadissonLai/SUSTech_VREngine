// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.UI.Animation
{
    [CustomEditor(typeof(UIAnimations), true)]
    public class UIAnimationsEditor : BaseEditor
    {
        private UIAnimations Target { get { return (UIAnimations) target; } }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            
            if (DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconSearch), 
                                                   UILabels.SearchForDatabases,
                                                   Size.M, TextAlign.Left, 
                                                   DGUI.Colors.DisabledBackgroundColorName, DGUI.Colors.DisabledTextColorName, 
                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), true))
            {
                Target.SearchForUnregisteredDatabases(true);
            }
            GUILayout.Space(DGUI.Properties.Space(2));

            DrawDefaultInspector();
        }
    }

}