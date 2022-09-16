// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

// ReSharper disable ClassNeverInstantiated.Global

namespace Doozy.Editor
{
    public partial class Styles
    {
        private const string DARK_SKIN_FILENAME = "DarkSkin.guiskin";
        private const string LIGHT_SKIN_FILENAME = "LightSkin.guiskin";
        private static readonly string DarkSkinPath = Path.Combine(DoozyPath.EDITOR_SKINS_PATH, DARK_SKIN_FILENAME);
        private static readonly string LightSkinPath = Path.Combine(DoozyPath.EDITOR_SKINS_PATH, LIGHT_SKIN_FILENAME);

        private static GUISkin s_darkSkin;
        private static GUISkin DarkSkin { get { return s_darkSkin == null ? s_darkSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(DarkSkinPath) : s_darkSkin; } }
        private static GUISkin s_lightSkin;
        private static GUISkin LightSkin { get { return s_lightSkin == null ? s_lightSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(LightSkinPath) : s_lightSkin; } }

        private static Dictionary<string, GUIStyle> s_darkStyles;
        private static Dictionary<string, GUIStyle> DarkStyles { get { return s_darkStyles ?? (s_darkStyles = new Dictionary<string, GUIStyle>()); } }
        private static Dictionary<string, GUIStyle> s_lightStyles;
        private static Dictionary<string, GUIStyle> LightStyles { get { return s_lightStyles ?? (s_lightStyles = new Dictionary<string, GUIStyle>()); } }

        // ReSharper disable once MemberCanBePrivate.Global
        public static GUIStyle GetStyle(string styleName)
        {
            if (EditorGUIUtility.isProSkin)
            {
                if (DarkStyles.ContainsKey(styleName)) return DarkStyles[styleName];
                GUIStyle newDarkStyle = DarkSkin.GetStyle(styleName);
                if(newDarkStyle != null) DarkStyles.Add(styleName, newDarkStyle);
                return new GUIStyle(newDarkStyle);
            }

            if (LightStyles.ContainsKey(styleName)) return LightStyles[styleName];
            GUIStyle newLightStyle = LightSkin.GetStyle(styleName);
            if(newLightStyle != null) LightStyles.Add(styleName, newLightStyle);
            return new GUIStyle(newLightStyle);
        }
        
        public static GUIStyle GetStyle(StyleName styleName) { return GetStyle(styleName.ToString()); }
    }
}