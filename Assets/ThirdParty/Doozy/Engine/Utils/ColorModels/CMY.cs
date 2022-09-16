using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
     /// <summary>
    /// This CMY color model stands for cyan-magenta-yellow and is used for hardcopy devices.
    /// <para/> In contrast to color on the monitor, the color in printing acts subtractive and not additive.
    /// <para/> A printed color that looks red absorbs the other two components Green and Blue and reflects Red. Thus its (internal) color is G+B=CYAN. Similarly R+B=MAGENTA and R+G=YELLOW.
    /// <para/> Thus the C-M-Y coordinates are just the complements of the R-G-B coordinates:
    /// </summary>
    [Serializable]
    public class CMY
    {
        public CMY(float C, float M, float Y)
        {
            c = C;
            m = M;
            y = Y;
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

        public CMY Copy()
        {
            return new CMY(c, m, y);
        }

        public Color Color(float alpha = 1)
        {
            return ColorUtils.CMYtoRGB(this).Validate().Color();
        }

        public RGB ToRGB()
        {
            return ColorUtils.CMYtoRGB(this);
        }

        public CMY Validate()
        {
            c = ValidateColor(c, C.MIN, C.MAX);
            m = ValidateColor(m, M.MIN, M.MAX);
            y = ValidateColor(y, Y.MIN, Y.MAX);
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
                x = FactorizeColor(c, C.MIN, C.MAX, C.F),
                y = FactorizeColor(m, M.MIN, M.MAX, M.F),
                z = FactorizeColor(y, Y.MIN, Y.MAX, Y.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f)
        {
            return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
        }

        public string ToString(bool factorize = false)
        {
            return factorize ? "cmy(" + Factorize().x + ", " + Factorize().y + ", " + Factorize().z + ")" : "cmy(" + c + ", " + m + ", " + y + ")";
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
    }
}