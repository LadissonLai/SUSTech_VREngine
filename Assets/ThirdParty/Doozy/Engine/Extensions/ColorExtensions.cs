// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Globalization;
using UnityEngine;

namespace Doozy.Engine.Extensions
{
    public static class ColorExtensions
    {
        #region Constants

        private const float LIGHT_OFFSET = 0.0625f;
        private const float DARKER_FACTOR = 0.9f;

        #endregion

        /// <summary>
        ///     Returns a new color from a hex value
        /// </summary>
        /// <param name="color"> The color</param>
        /// <param name="hexValue"> FFFFFF or #FFFFFF or #FFFFFFFF</param>
        /// <param name="alpha"> Custom alpha (default: 1) </param>
        /// <returns></returns>
        public static Color FromHex(this Color color, string hexValue, float alpha = 1)
        {
            if (string.IsNullOrEmpty(hexValue)) return Color.clear;

            if (hexValue[0] == '#') hexValue = hexValue.TrimStart('#');
            if (hexValue.Length > 6) hexValue = hexValue.Remove(6, hexValue.Length - 6);

            int value = int.Parse(hexValue, NumberStyles.HexNumber);
            int r = value >> 16 & 255;
            int g = value >> 8 & 255;
            int b = value & 255;
            float a = 255 * alpha;

            return new Color().ColorFrom256(r, g, b, a);
        }


        /// <summary>
        ///     Returns a new Color with the given settings.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">alpha</param>
        /// <returns>The new Color.</returns>
        public static Color ColorFrom256(this Color color, float r, float g, float b, float a = 255) { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }

        /// <summary>
        ///     Returns a new Color with the given settings.
        /// </summary>
        /// <param name="r">red</param>
        /// <param name="g">green</param>
        /// <param name="b">blue</param>
        /// <param name="a">alpha</param>
        public static Color ColorFrom256(float r, float g, float b, float a = 255) { return new Color(r / 255f, g / 255f, b / 255f, a / 255f); }

        /// <summary>
        ///     Returns a Color lighter than the given color.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns>The new Color.</returns>
        public static Color Lighter(this Color color)
        {
            return new Color(
                color.r + LIGHT_OFFSET,
                color.g + LIGHT_OFFSET,
                color.b + LIGHT_OFFSET,
                color.a);
        }

        /// <summary>
        ///     Returns a Color darker than the given color.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns>The new Color.</returns>
        public static Color Darker(this Color color)
        {
            return new Color(
                color.r - LIGHT_OFFSET,
                color.g - LIGHT_OFFSET,
                color.b - LIGHT_OFFSET,
                color.a);
        }

        /// <summary>
        ///     Returns the brightness of the Color, defined as the average off the three Color channels.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns>The new Color.</returns>
        public static float Brightness(this Color color) { return (color.r + color.g + color.b) / 3; }

        /// <summary>
        ///     Returns a new Color with the RGB values scaled so that the color has the given brightness.
        ///     <para>If the Color is too dark, a grey is returned with the right brighness.The alpha is left uncanged.</para>
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="brightness">New brightness.</param>
        /// <returns>The new Color.</returns>
        public static Color WithBrightness(this Color color, float brightness)
        {
            if (color.IsApproximatelyBlack()) return new Color(brightness, brightness, brightness, color.a);

            float factor = brightness / color.Brightness();

            float r = color.r * factor;
            float g = color.g * factor;
            float b = color.b * factor;

            float a = color.a;

            return new Color(r, g, b, a);
        }

        /// <summary>
        ///     Returns true if the Color is black or almost black, false otherwise.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns></returns>
        public static bool IsApproximatelyBlack(this Color color) { return color.r + color.g + color.b <= Mathf.Epsilon; }

        /// <summary>
        ///     Returns true if the Color is white or almost white, false otherwise.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns></returns>
        public static bool IsApproximatelyWhite(this Color color) { return color.r + color.g + color.b >= 1 - Mathf.Epsilon; }

        /// <summary>
        ///     Returns an opaque (no transparency) version of the given Color.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns></returns>
        public static Color Opaque(this Color color) { return new Color(color.r, color.g, color.b); }

        /// <summary>
        ///     Returns a new Color that is the inversion of this Color.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <returns></returns>
        public static Color Invert(this Color color) { return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a); }

        /// <summary>
        ///     Returns a new Color with the same settings and a new alpha.
        /// </summary>
        /// <param name="color">The Color.</param>
        /// <param name="alpha">Alpha for the Color.</param>
        /// <returns></returns>
        public static Color WithAlpha(this Color color, float alpha) { return new Color(color.r, color.g, color.b, alpha); }
    }
}