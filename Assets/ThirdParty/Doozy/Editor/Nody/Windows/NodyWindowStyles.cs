// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private static GUIStyle SplashScreen { get { return Styles.GetStyle(Styles.StyleName.SplashScreen); } }
        private static GUIStyle IconNody { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.IconNody); } }

        private static GUIStyle ToolbarTabTopToBottom { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WindowToolbarTabTopToBottom); } }
        private static GUIStyle ToolbarBackgroundTopToBottom { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WindowToolbarBackgroundTopToBottom); } }

        private static GUIStyle WhiteGradientTopToBottom { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WhiteGradientTopToBottom); } }
        private static GUIStyle WhiteGradientBottomToTop { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WhiteGradientBottomToTop); } }
        private static GUIStyle WhiteGradientLeftToRight { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WhiteGradientLeftToRight); } }
        private static GUIStyle WhiteGradientRightToLeft { get { return Doozy.Editor.Styles.GetStyle(Doozy.Editor.Styles.StyleName.WhiteGradientRightToLeft); } }

        private static GUIStyle s_nodeButtonDelete;
        private static GUIStyle NodeButtonDelete { get { return s_nodeButtonDelete ?? (s_nodeButtonDelete = Styles.GetStyle(Styles.StyleName.NodeButtonDelete)); } }
        private static GUIStyle s_dot;
        private static GUIStyle Dot { get { return s_dot ?? (s_dot = Styles.GetStyle(Styles.StyleName.NodeDot)); } }

        private static GUIStyle s_connectionPointOverrideEmpty;
        private static GUIStyle ConnectionPointOverrideEmpty { get { return s_connectionPointOverrideEmpty ?? (s_connectionPointOverrideEmpty = Styles.GetStyle(Styles.StyleName.ConnectionPointOverrideEmpty)); } }
        private static GUIStyle s_connectionPointOverrideConnected;
        private static GUIStyle ConnectionPointOverrideConnected { get { return s_connectionPointOverrideConnected ?? (s_connectionPointOverrideConnected = Styles.GetStyle(Styles.StyleName.ConnectionPointOverrideConnected)); } }
        private static GUIStyle s_connectionPointMultipleEmpty;
        private static GUIStyle ConnectionPointMultipleEmpty { get { return s_connectionPointMultipleEmpty ?? (s_connectionPointMultipleEmpty = Styles.GetStyle(Styles.StyleName.ConnectionPointMultipleEmpty)); } }
        private static GUIStyle s_connectionPointMultipleConnected;
        private static GUIStyle ConnectionPointMultipleConnected { get { return s_connectionPointMultipleConnected ?? (s_connectionPointMultipleConnected = Styles.GetStyle(Styles.StyleName.ConnectionPointMultipleConnected)); } }
        private static GUIStyle s_connectionPointMinus;
        private static GUIStyle ConnectionPointMinus { get { return s_connectionPointMinus ?? (s_connectionPointMinus = Styles.GetStyle(Styles.StyleName.ConnectionPointMinus)); } }
    }
}