// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Soundy
{
    /// <summary>
    /// Sound related utility methods
    /// </summary>
    public static class SoundyUtils
    {
        #region Static Properties

        private static readonly float TwelfthRootOfTwo = Mathf.Pow(2f, 1.0f / 12f);

        #endregion

        #region Static Methods
        
        /// <summary> Converts semitones value to a pitch value (returns a value between 0f and 4f) </summary>
        /// <param name="semitones"> Semitone value </param>
        public static float SemitonesToPitch(float semitones) { return Mathf.Clamp(Mathf.Pow(TwelfthRootOfTwo, semitones), 0f, 4f); }

        /// <summary> Converts a pitch value to a semitone value </summary>
        /// <param name="pitch"> Pitch value </param>
        public static float PitchToSemitones(float pitch) { return Mathf.Log(pitch, TwelfthRootOfTwo); }

        /// <summary> Converts decibels to linear (returns a value between 0f and 1f). Check <see cref="http://www.sengpielaudio.com/calculator-FactorRatioLevelDecibel.htm" /> for details </summary>
        /// <param name="dB"> Decibel value (should be between -80f and 0f) </param>
        public static float DecibelToLinear(float dB) { return dB < -80 ? 0 : Mathf.Clamp01(Mathf.Pow(10f, dB / 20f)); }

        /// <summary> Converts linear to decibels (returns a value between -80f and 0f). Check <see cref="http://www.sengpielaudio.com/calculator-FactorRatioLevelDecibel.htm" /> for details </summary>
        /// <param name="linear"> Linear value (should be between 0 and 1) </param>
        public static float LinearToDecibel(float linear) { return linear > 0 ? -80f : Mathf.Clamp(20f * Mathf.Log10(linear), -80f, 0f); }
        
        #endregion
    }
}