// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils.ColorModels;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Doozy.Engine.Utils
{
#pragma warning disable 0219
    public static class ColorUtils
    {
        public enum Conversions
        {
            RGB_TO_RGB,
            HEX_TO_RGB,
            RGB_TO_HEX,
            RGB_TO_FGC,
            HSL_TO_RGB,
            RGB_TO_HSL,
            HSV_TO_RGB,
            RGB_TO_HSV,
            CMY_TO_RGB,
            RGB_TO_CMY,
            CMYK_TO_RGB,
            RGB_TO_CMYK,
            XYZ_TO_RGB,
            RGB_TO_XYZ,
            Yxy_TO_RGB,
            RGB_TO_Yxy,
            LAB_TO_RGB,
            RGB_TO_LAB
        }

        /// <summary>
        ///     Convert pure hue to RGB
        /// </summary>
        public static Vector3 HUEtoRGB(float H)
        {
            float R = Mathf.Abs(H * 6 - 3) - 1;
            float G = 2 - Mathf.Abs(H * 6 - 2);
            float B = 2 - Mathf.Abs(H * 6 - 4);
            return new Vector3(R, G, B);
        }

        public static RGB HSLtoRGB(HSL values) //http://www.rapidtables.com/convert/color/hsl-to-rgb.htm
        {
            HSL hsl = new HSL(values.h, values.s, values.l).Validate();

            float H = hsl.Factorize().x;
            float S = hsl.s;
            float L = hsl.l;

            float C = (1 - Mathf.Abs(2 * L - 1)) * S;
            float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
            float m = L - C / 2;

            float r = 0, g = 0, b = 0;

            if (0 <= H && H < 60)
            {
                r = C;
                g = X;
                b = 0;
            }
            else if (60 <= H && H < 120)
            {
                r = X;
                g = C;
                b = 0;
            }
            else if (120 <= H && H < 180)
            {
                r = 0;
                g = C;
                b = X;
            }
            else if (180 <= H && H < 240)
            {
                r = 0;
                g = X;
                b = C;
            }
            else if (240 <= H && H < 300)
            {
                r = X;
                g = 0;
                b = C;
            }
            else if (300 <= H && H < 360)
            {
                r = C;
                g = 0;
                b = X;
            }

            return new RGB(r + m, g + m, b + m).Validate();
        }

        public static HSL RGBtoHSL(RGB values) //http://www.rapidtables.com/convert/color/rgb-to-hsl.htm
        {
            float r = values.r;
            float g = values.g;
            float b = values.b;

            float Cmax = Mathf.Max(r, g, b);
            float Cmin = Mathf.Min(r, g, b);
            float delta = Cmax - Cmin;

            float L = (Cmax + Cmin) / 2;

            float H = 0;
            if (delta != 0)
            {
                if (Cmax == r) H = 60 * ((g - b) / delta % 6);
                if (Cmax == g) H = 60 * ((b - r) / delta + 2);
                if (Cmax == b) H = 60 * ((r - g) / delta + 4);
            }

            float S = 0;
            if (delta != 0) S = delta / (1 - Mathf.Abs(2 * L - 1));

            H /= 360;

            return new HSL(H, S, L).Validate();
        }

        public static RGB HSVtoRGB(HSV values) //http://www.rapidtables.com/convert/color/hsv-to-rgb.htm
        {
            var hsv = new HSV(values.h, values.s, values.v);

            float H = hsv.Factorize().x;
            float S = hsv.s;
            float V = hsv.v;

            float C = V * S;
            float X = C * (1 - Mathf.Abs(H / 60 % 2 - 1));
            float m = V - C;

            float r = 0, g = 0, b = 0;

            if (0 <= H && H < 60)
            {
                r = C;
                g = X;
                b = 0;
            }
            else if (60 <= H && H < 120)
            {
                r = X;
                g = C;
                b = 0;
            }
            else if (120 <= H && H < 180)
            {
                r = 0;
                g = C;
                b = X;
            }
            else if (180 <= H && H < 240)
            {
                r = 0;
                g = X;
                b = C;
            }
            else if (240 <= H && H < 300)
            {
                r = X;
                g = 0;
                b = C;
            }
            else if (300 <= H && H < 360)
            {
                r = C;
                g = 0;
                b = X;
            }

            return new RGB(r + m, g + m, b + m);
        }

        public static HSV RGBtoHSV(RGB values) //http://www.rapidtables.com/convert/color/rgb-to-hsv.htm
        {
            float r = values.r;
            float g = values.g;
            float b = values.b;

            float Cmax = Mathf.Max(r, g, b);
            float Cmin = Mathf.Min(r, g, b);
            float delta = Cmax - Cmin;

            float H = 0;
            if (delta != 0)
            {
                if (Cmax == r) H = 60 * ((g - b) / delta % 6);
                if (Cmax == g) H = 60 * ((b - r) / delta + 2);
                if (Cmax == b) H = 60 * ((r - g) / delta + 4);
            }

            float S = 0;
            if (Cmax != 0) S = delta / Cmax;

            float V = Cmax;

            H /= 360;

            return new HSV(H, S, V).Validate();
        }

        public static RGB CMYtoRGB(CMY values) //http://www.easyrgb.com/index.php?X=MATH&H=12#text12
        {
            CMY cmy = new CMY(values.c, values.m, values.y).Validate();
            float r = 1 - cmy.c;
            float g = 1 - cmy.m;
            float b = 1 - cmy.y;
            return new RGB(r, g, b).Validate();
        }

        public static CMY RGBtoCMY(RGB values) //http://www.easyrgb.com/index.php?X=MATH&H=11#text11
        {
            RGB rgb = new RGB(values.r, values.g, values.b).Validate();
            float c = 1 - rgb.r;
            float m = 1 - rgb.g;
            float y = 1 - rgb.b;
            return new CMY(c, m, y).Validate();
        }

        public static RGB CMYKtoRGB(CMYK values) //http://www.easyrgb.com/index.php?X=MATH&H=21#text21
        {
            CMYK cmyk = new CMYK(values.c, values.m, values.y, values.k).Validate();
            float c = cmyk.c * (1 - cmyk.k) + cmyk.k;
            float m = cmyk.m * (1 - cmyk.k) + cmyk.k;
            float y = cmyk.y * (1 - cmyk.k) + cmyk.k;
            return CMYtoRGB(new CMY(c, m, y).Validate()).Validate();
        }

        public static CMYK RGBtoCMYK(RGB values) //http://www.easyrgb.com/index.php?X=MATH&H=13#text13
        {
            CMY cmy = RGBtoCMY(values).Validate();
            float k = Mathf.Min(1, cmy.c, cmy.m, cmy.y);
            float c = cmy.c;
            float m = cmy.m;
            float y = cmy.y;
            if (k > 0.997) c = 0;
            if (k > 0.997) m = 0;
            if (k > 0.997) y = 0;
            if (k > 0.003) c = (cmy.c - k) / (1 - k);
            if (k > 0.003) m = (cmy.m - k) / (1 - k);
            if (k > 0.003) y = (cmy.y - k) / (1 - k);
            return new CMYK(c, m, y, k).Validate();
        }

        public static RGB XYZtoRGB(XYZ values) //http://www.easyrgb.com/index.php?X=MATH&H=14#text14
        {
            XYZ xyz = new XYZ(values.x, values.y, values.z).Validate();
            float r = xyz.x * 3.2406f + xyz.y * -1.5372f + xyz.z * -0.4986f;
            float g = xyz.x * -0.9689f + xyz.y * 1.8758f + xyz.z * 0.0415f;
            float b = xyz.x * 0.0557f + xyz.y * -0.2040f + xyz.z * 1.0570f;
            r = r > 0.0031308f ? 1.055f * Mathf.Pow(r, 1f / 2.4f) - 0.055f : 12.92f * r;
            g = g > 0.0031308f ? 1.055f * Mathf.Pow(g, 1f / 2.4f) - 0.055f : 12.92f * g;
            b = b > 0.0031308f ? 1.055f * Mathf.Pow(b, 1f / 2.4f) - 0.055f : 12.92f * b;
            return new RGB(r, g, b).Validate();
        }

        public static XYZ RGBtoXYZ(RGB values) //http://easyrgb.com/index.php?X=MATH&H=02#text2
        {
            RGB rgb = new RGB(values.r, values.g, values.b).Validate();
            float r = rgb.r > 0.04045f ? Mathf.Pow((rgb.r + 0.055f) / 1.055f, 2.4f) : rgb.r / 12.92f;
            float g = rgb.g > 0.04045f ? Mathf.Pow((rgb.g + 0.055f) / 1.055f, 2.4f) : rgb.g / 12.92f;
            float b = rgb.b > 0.04045f ? Mathf.Pow((rgb.b + 0.055f) / 1.055f, 2.4f) : rgb.b / 12.92f;
            float x = rgb.r * 0.4124f + rgb.g * 0.3576f + rgb.b * 0.1805f;
            float y = rgb.r * 0.2126f + rgb.g * 0.7152f + rgb.b * 0.0722f;
            float z = rgb.r * 0.0193f + rgb.g * 0.1192f + rgb.b * 0.9505f;
            return new XYZ(x, y, z).Validate();
        }
    }
#pragma warning restore 0219
}