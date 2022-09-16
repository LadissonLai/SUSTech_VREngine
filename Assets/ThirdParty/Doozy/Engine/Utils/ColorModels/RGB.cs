// ReSharper disable InconsistentNaming

using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
   /// <summary>
    /// The RGB color model is an additive color model in which red, green and blue light are added together in various ways to reproduce a broad array of colors.
    /// </summary>
    [Serializable]
    public class RGB
    {
        public RGB(float R, float G, float B)
        {
            r = R;
            g = G;
            b = B;
        }

        /// <summary>
        /// Red - r ∊ [0, 1]
        /// </summary>
        public float r;
        /// <summary>
        /// Green - g ∊ [0, 1]
        /// </summary>
        public float g;
        /// <summary>
        /// Blue - b ∊ [0, 1]
        /// </summary>
        public float b;

        public RGB Copy()
        {
            return new RGB(r, g, b);
        }

        public Color Color(float alpha = 1)
        {
            return new Color(r, g, b, Mathf.Clamp(alpha, 0, 1));
        }

        public HSL ToHSL()
        {
            return ColorUtils.RGBtoHSL(this);
        }

        public HSV ToHSV()
        {
            return ColorUtils.RGBtoHSV(this);
        }

        public CMY ToCMY()
        {
            return ColorUtils.RGBtoCMY(this);
        }

        public CMYK ToCMYK()
        {
            return ColorUtils.RGBtoCMYK(this);
        }

        public XYZ ToXYZ()
        {
            return ColorUtils.RGBtoXYZ(this);
        }

        public RGB Validate()
        {
            r = ValidateColor(r, R.MIN, R.MAX);
            g = ValidateColor(g, G.MIN, G.MAX);
            b = ValidateColor(b, B.MIN, B.MAX);
            return this;
        }

        private float ValidateColor(float value, float min, float max)
        {
            return Mathf.Max(min, Mathf.Min(max, value));
        }

        public Vector3 Factorize()
        {
            return new Vector3
            {
                x = FactorizeColor(r, R.MIN, R.MAX, R.F),
                y = FactorizeColor(g, G.MIN, G.MAX, G.F),
                z = FactorizeColor(b, B.MIN, B.MAX, B.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f)
        {
            return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
        }

        public string ToString(bool factorize = false)
        {
            return factorize ? "rgb(" + Factorize().x + ", " + Factorize().y + ", " + Factorize().z + ")" : "rgb(" + r + ", " + g + ", " + b + ")";
        }

        /// <summary>
        /// A hex triplet is a six-digit, three-byte hexadecimal number used in HTML, CSS, SVG, and other computing applications to represent colors.
        /// <para/>The bytes represent the red, green and blue components of the color.
        /// <para/>One byte represents a number in the range 00 to FF (in hexadecimal notation), or 0 to 255 in decimal notation.
        /// </summary>
        public string ToHEX(bool addHashTag = true)
        {
            return (addHashTag ? "#" : "") + ColorUtility.ToHtmlStringRGB(Color(1));
        }

        /// <summary>
        /// Red
        /// </summary>
        public class R
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
        /// <summary>
        /// Green
        /// </summary>
        public class G
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
        /// <summary>
        /// Blue
        /// </summary>
        public class B
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 255;
        }
    }

}