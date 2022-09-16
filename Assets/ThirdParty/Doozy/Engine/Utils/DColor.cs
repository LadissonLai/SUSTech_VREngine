// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils.ColorModels;
using UnityEngine;

namespace Doozy.Engine.Utils
{
    [Serializable]
    public class DColor
    {
        private const string UNNAMED_COLOR = "Unnamed Color";

        public string ColorName;
        public Color Light;
        public Color Normal;
        public Color Dark;

        private static Color GetLightColor(Color normalColor)
        {
            //Light = new Color(normal.r, normal.g, normal.b, normal.a).Lighter();
            HSV lightHSV = ColorUtils.RGBtoHSV(new RGB(normalColor.r, normalColor.g, normalColor.b));
            lightHSV.s = 90f/255f;
            lightHSV.v = 240f/255f;
            return lightHSV.Color();
        }

        private static Color GetDarkColor(Color normalColor)
        {
            //Dark = new Color(normal.r, normal.g, normal.b, normal.a).Darker();
            HSV darkHSV = ColorUtils.RGBtoHSV(new RGB(normalColor.r, normalColor.g, normalColor.b));
            darkHSV.s = 220f/255f;
            darkHSV.v = 90f/255f;
            return darkHSV.Color();
        }
        
        public DColor(Color normal)
        {
            ColorName = UNNAMED_COLOR;
            Normal = new Color(normal.r, normal.g, normal.b, normal.a);
            Light = GetLightColor(Normal);
            Dark = GetDarkColor(Normal);
        }

        public DColor(string colorName)
        {
            ColorName = colorName;
            Light = Color.white;
            Normal = Color.gray;
            Dark = Color.black;
        }

        public DColor(string colorName, Color normal)
        {
            ColorName = colorName;
            Normal = new Color(normal.r, normal.g, normal.b, normal.a);
            Light = GetLightColor(Normal);
            Dark = GetDarkColor(Normal);
        }

        public DColor(Color light, Color normal, Color dark)
        {
            ColorName = UNNAMED_COLOR;
            Light = new Color(light.r, light.g, light.b, light.a);
            Normal = new Color(normal.r, normal.g, normal.b, normal.a);
            Dark = new Color(dark.r, dark.g, dark.b, dark.a);
        }

        public DColor(string colorName, Color light, Color normal, Color dark)
        {
            ColorName = colorName;
            Light = light;
            Normal = normal;
            Dark = dark;
        }

        public DColor(DColor dColor)
        {
            ColorName = dColor.ColorName;
            Light = new Color(dColor.Light.r, dColor.Light.g, dColor.Light.b, dColor.Light.a);
            Normal = new Color(dColor.Normal.r, dColor.Normal.g, dColor.Normal.b, dColor.Normal.a);
            Dark = new Color(dColor.Dark.r, dColor.Dark.g, dColor.Dark.b, dColor.Dark.a);
        }
    }
}