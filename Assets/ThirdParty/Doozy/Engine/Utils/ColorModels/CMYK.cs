using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
    /// <summary>
    /// The CMYK color model (process color, four color) is a subtractive color model, used in color printing, and is also used to describe the printing process itself.
    /// <para/> CMYK refers to the four inks used in some color printing: cyan, magenta, yellow, and key (black).
    /// </summary>
    [Serializable]
    public class CMYK
    {
        public CMYK(float C, float M, float Y, float K)
        {
            c = C;
            m = M;
            y = Y;
            k = K;
        }

        /// <summary>
        /// Cyan - c ∊ [0, 1]
        /// </summary>
        public float c;
        /// <summary>
        /// Magenta - m ∊ [0, 1]
        /// </summary>
        public float m;
        /// <summary>
        /// Yellow - y ∊ [0, 1]
        /// </summary>
        public float y;
        /// <summary>
        /// Key (black) - k ∊ [0, 1]
        /// </summary>
        public float k;

        public CMYK Copy()
        {
            return new CMYK(c, m, y, k);
        }

        public Color Color(float alpha = 1)
        {
            return ColorUtils.CMYKtoRGB(this).Validate().Color();
        }

        public RGB ToRGB()
        {
            return ColorUtils.CMYKtoRGB(this);
        }

        public CMYK Validate()
        {
            c = ValidateColor(c, C.MIN, C.MAX);
            m = ValidateColor(m, M.MIN, M.MAX);
            y = ValidateColor(y, Y.MIN, Y.MAX);
            k = ValidateColor(k, K.MIN, K.MAX);
            return this;
        }

        private float ValidateColor(float value, float min, float max)
        {
            return Mathf.Max(min, Mathf.Min(max, value));
        }

        public Vector4 Factorize()
        {
            return new Vector4
            {
                x = FactorizeColor(c, C.MIN, C.MAX, C.F),
                y = FactorizeColor(m, M.MIN, M.MAX, M.F),
                z = FactorizeColor(y, Y.MIN, Y.MAX, Y.F),
                w = FactorizeColor(k, K.MIN, K.MAX, K.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f)
        {
            return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
        }

        public string ToString(bool factorize = false)
        {
            return factorize ? "cmyk(" + Factorize().x + ", " + Factorize().y + ", " + Factorize().z + ", " + Factorize().w + ")" : "cmyk(" + c + ", " + m + ", " + y + ", " + k + ")";
        }

        /// <summary>
        /// Cyan
        /// </summary>
        public class C
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
        /// <summary>
        /// Magenta
        /// </summary>
        public class M
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
        /// <summary>
        /// Yellow
        /// </summary>
        public class Y
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
        /// <summary>
        /// Key (black)
        /// </summary>
        public class K
        {
            public const float MIN = 0;
            public const float MAX = 1;
            public const int F = 100;
        }
    }
}