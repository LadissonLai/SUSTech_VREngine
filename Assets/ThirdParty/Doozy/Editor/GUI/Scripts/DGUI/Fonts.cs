// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.IO;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Fonts
        {
            public static Font SansationBold { get { return AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(DoozyPath.EDITOR_FONTS_PATH, "Sansation-Bold.ttf")); } }
            public static Font SansationLight { get { return AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(DoozyPath.EDITOR_FONTS_PATH, "Sansation-Light.ttf")); } }
            public static Font SansationRegular { get { return AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(DoozyPath.EDITOR_FONTS_PATH, "Sansation-Regular.ttf")); } }
        }
    }
}