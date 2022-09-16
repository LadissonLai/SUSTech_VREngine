using System;
using UnityEngine;

namespace Doozy.Engine.Utils.ColorModels
{
   /// <summary>
    /// The CIE XYZ color space encompasses all color sensations that an average person can experience.
    /// <para/> That is why CIE XYZ (Tristimulus values) is a device invariant color representation.
    /// <para/> It serves as a standard reference against which many other color spaces are defined.
    /// <para/> All human-visible colors have positive X, Y, and Z values
    /// </summary>
    [Serializable]
    public class XYZ
    {
        public XYZ(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        ///  The CIE XYZ model defines X as a mix (a linear combination) of cone response curves chosen to be nonnegative.
        ///  <para/> x ∊ [0, 0.95047]
        /// </summary>
        public float x;
        /// <summary>
        /// Luminance. The CIE XYZ model defines Y as luminance.
        /// <para/> y ∊ [0, 1.00000]
        /// </summary>
        public float y;
        /// <summary>
        /// Blue stimulation. The CIE XYZ model defines Z as being quasi-equal to blue stimulation (or the S cone response).
        /// <para/> z ∊ [0, 1.08883]
        /// </summary>
        public float z;

        public XYZ Copy()
        {
            return new XYZ(x, y, z);
        }

        public Color Color(float alpha = 1)
        {
            return ColorUtils.XYZtoRGB(this).Validate().Color();
        }

        public RGB ToRGB()
        {
            return ColorUtils.XYZtoRGB(this);
        }

        public XYZ Validate()
        {
            x = ValidateColor(x, X.MIN, X.MAX);
            y = ValidateColor(y, Y.MIN, Y.MAX);
            z = ValidateColor(z, Z.MIN, Z.MAX);
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
                x = FactorizeColor(x, X.MIN, X.MAX, X.F),
                y = FactorizeColor(y, Y.MIN, Y.MAX, Y.F),
                z = FactorizeColor(z, Z.MIN, Z.MAX, Z.F)
            };
        }

        private int FactorizeColor(float value, float min, float max, float f)
        {
            return (int)Mathf.Max(min * f, Mathf.Min(max * f, Mathf.Round(value * f)));
        }

        public string ToString(bool factorize = false)
        {
            return factorize ? "XYZ(" + Factorize().x + ", " + Factorize().y + ", " + Factorize().z + ")" : "XYZ(" + x + ", " + y + ", " + z + ")";
        }

        /// <summary>
        /// The CIE XYZ model defines X as a mix (a linear combination) of cone response curves chosen to be nonnegative.
        /// </summary>
        public class X
        {
            public const float MIN = 0;
            public const float MAX = 0.95047f;
            public const int F = 100;
        }
        /// <summary>
        /// The CIE XYZ model defines Y as luminance.
        /// </summary>
        public class Y
        {
            public const float MIN = 0;
            public const float MAX = 1.00000f;
            public const int F = 100;
        }
        /// <summary>
        /// The CIE XYZ model defines Z as being quasi-equal to blue stimulation (or the S cone response).
        /// </summary>
        public class Z
        {
            public const float MIN = 0;
            public const float MAX = 1.08883f;
            public const int F = 100;
        }
        
    }
}