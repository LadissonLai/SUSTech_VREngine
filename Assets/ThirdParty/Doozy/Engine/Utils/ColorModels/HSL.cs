using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
    /// <summary>
    ///     HSL stands for hue, saturation, and lightness (or luminosity).
    ///     <para />
    ///     In each cylinder, the angle around the central vertical axis corresponds to "hue", the distance from the axis corresponds to "saturation", and the distance along the axis corresponds to "lightness", "value" or "brightness".
    ///     <para />
    ///     Note that while "hue" in HSL and HSV refers to the same attribute, their definitions of "saturation" differ dramatically.
    /// </summary>
    [Serializable]
    public class HSL
    {
        public HSL(float H, float S, float L)
        {
            h = H;
            s = S;
            l = L;
        }

        /// <summary>
        ///     Hue - h ∊ [0, 1]
        /// </summary>
        public float h;

        /// <summary>
        ///     Saturation - s ∊ [0, 1]
        /// </summary>
        public float s;

        /// <summary>
        ///     Lightness - l ∊ [0, 1]
        /// </summary>
        public float l;

        public HSL Copy() { return new HSL(h, s, l); }

        public Color Color(float alpha = 1) { return ColorUtils.HSLtoRGB(this).Validate().Color(); }

        public RGB ToRGB() { return ColorUtils.HSLtoRGB(this); }

        public HSL Validate()
        {
            h = ValidateColor(h, H.MIN, H.MAX);
            s = ValidateColor(s, S.MIN, S.MAX);
            l = ValidateColor(l, L.MIN, L.MAX);
            return this;
        }

        private float ValidateColor(float value, float min, float max) { return Mathf.Max(min, Mathf.Min(max, value)); }

        public Vector3 Factorize()
        {
            return new Vector3
            {
                x = FactorizeColor(h, H.MIN, H.MAX, H.F),
                y = FactorizeColor(s, S.MIN, S.MAX, S.F),
                z = FactorizeColor(l, L.MIN, L.MAX, L.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f) { return (int) Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f))); }

        public string ToString(bool factorize = false) { return factorize ? "hsl(" + Factorize().x + ", " + Factorize().y + "%, " + Factorize().z + "%)" : "hsl(" + h + ", " + s + "%, " + l + "%)"; }

        /// <summary>
        ///     Hue
        /// </summary>
        public class H
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 360;
        }

        /// <summary>
        ///     Saturation
        /// </summary>
        public class S
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }

        /// <summary>
        ///     Lightness
        /// </summary>
        public class L
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
    }
}