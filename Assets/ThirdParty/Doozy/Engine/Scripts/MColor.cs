// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Engine
{
    [Serializable]
    public class MColor
    {
        public string Name;
        public Color M50;
        public Color M100;
        public Color M200;
        public Color M300;
        public Color M400;
        public Color M500;
        public Color M600;
        public Color M700;
        public Color M800;
        public Color M900;
        public Color A100;
        public Color A200;
        public Color A400;
        public Color A700;

        public MColor(string name, 
                      Color m50, Color m100, Color m200, Color m300, Color m400, Color m500, Color m600, Color m700, Color m800, Color m900, 
                      Color a100, Color a200, Color a400, Color a700)
        {
            Name = name;
            M50 = m50;
            M100 = m100;
            M200 = m200;
            M300 = m300;
            M400 = m400;
            M500 = m500;
            M600 = m600;
            M700 = m700;
            M800 = m800;
            M900 = m900;
            A100 = a100;
            A200 = a200;
            A400 = a400;
            A700 = a700;
        }
        
        public MColor(string name, 
                      string m50Hex, string m100Hex, string m200Hex, string m300Hex, string m400Hex, string m500Hex, string m600Hex, string m700Hex, string m800Hex, string m900Hex, 
                      string a100Hex, string a200Hex, string a400Hex, string a700Hex)
        {
            Name = name;
            ColorUtility.TryParseHtmlString(m50Hex, out M50);
            ColorUtility.TryParseHtmlString(m100Hex, out M100);
            ColorUtility.TryParseHtmlString(m200Hex, out M200);
            ColorUtility.TryParseHtmlString(m300Hex, out M300);
            ColorUtility.TryParseHtmlString(m400Hex, out M400);
            ColorUtility.TryParseHtmlString(m500Hex, out M500);
            ColorUtility.TryParseHtmlString(m600Hex, out M600);
            ColorUtility.TryParseHtmlString(m700Hex, out M700);
            ColorUtility.TryParseHtmlString(m800Hex, out M800);
            ColorUtility.TryParseHtmlString(m900Hex, out M900);
            ColorUtility.TryParseHtmlString(a100Hex, out A100);
            ColorUtility.TryParseHtmlString(a200Hex, out A200);
            ColorUtility.TryParseHtmlString(a400Hex, out A400);
            ColorUtility.TryParseHtmlString(a700Hex, out A700);
        }
    }
}