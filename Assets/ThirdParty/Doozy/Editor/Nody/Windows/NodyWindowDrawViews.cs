// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor.AnimatedValues;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        public enum View
        {
            General,
            Graph
        }

        private AnimBool m_showGeneralView,
                         m_showGraphView;

        private void DrawViews()
        {
            //update the AnimBool show variables
            m_showGeneralView.target = CurrentView == View.General;
            m_showGraphView.target = CurrentView == View.Graph;
            m_altKeyPressedAnimBool.target = m_altKeyPressed;

            //draw the currently selected view 
            DrawView(DrawViewGeneral, m_showGeneralView);
            DrawView(DrawViewGraph, m_showGraphView);
        }

        private void SetView(View view)
        {
            CurrentView = view;
            SetGraphMode(GraphMode.None); //view changed -> reset the current GraphMode to White
        }
    }
}