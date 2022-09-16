using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
   /// <summary>
    /// HSV stands for hue, saturation, and value, and is also often called HSB (B for brightness).
    /// <para/> In each cylinder, the angle around the central vertical axis corresponds to "hue", the distance from the axis corresponds to "saturation", and the distance along the axis corresponds to "lightness", "value" or "brightness".
    /// <para/> Note that while "hue" in HSL and HSV refers to the same attribute, their definitions of "saturation" differ dramatically.
    /// </summary>
    [Serializable]
    public class HSV
    {
        public HSV(float H, float S, float V)
        {
            h = H;
            s = S;
            v = V;
        }

        /// <summary>
        /// Hue - h ∊ [0, 1]
        /// </summary>
        public float h;
        /// <summary>
        /// Saturation - s ∊ [0, 1]
        /// </summary>
        public float s;
        /// <summary>
        /// Value - l ∊ [0, 1]
        /// </summary>
        public float v;

        public HSV Copy()
        {
            return new HSV(h, s, v);
        }

        public Color Color(float alpha = 1)
        {
            return ColorUtils.HSVtoRGB(this).Validate().Color();
        }

        public RGB ToRGB()
        {
            return ColorUtils.HSVtoRGB(this);
        }

        public HSV Validate()
        {
            h = ValidateColor(h, H.MIN, H.MAX);
            s = ValidateColor(s, S.MIN, S.MAX);
            v = ValidateColor(v, V.MIN, V.MAX);
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
                x = FactorizeColor(h, H.MIN, H.MAX, H.F),
                y = FactorizeColor(s, S.MIN, S.MAX, S.F),
                z = FactorizeColor(v, V.MIN, V.MAX, V.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f)
        {
            return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
        }

        public string ToString(bool factorize = false)
        {
            return factorize ? "hsv(" + Factorize().x + ", " + Factorize().y + "%, " + Factorize().z + "%)" : "hsv(" + h + ", " + s + "%, " + v + "%)";
        }

        /// <summary>
        /// Hue
        /// </summary>
        public class H
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 360;
        }
        /// <summary>
        /// Saturation
        /// </summary>
        public class S
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
        /// <summary>
        /// Value
        /// </summary>
        public class V
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
    }
}