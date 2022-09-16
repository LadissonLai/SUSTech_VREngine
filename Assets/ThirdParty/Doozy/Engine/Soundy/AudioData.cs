// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <summary> Audio info for any referenced AudioClip in the Soundy system </summary>
    [Serializable]
    public class AudioData
    {
        #region Constants

        public const float DEFAULT_WEIGHT = 1f;
        public const float MAX_WEIGHT = 1f;
        public const float MIN_WEIGHT = 0f;

        #endregion

        #region Public Variables

        /// <summary> Direct reference to an AudioClip </summary>
        public AudioClip AudioClip;

        /// <summary> (Not Implemented) Weight of this AudioClip in the SoundGroupData </summary>
        [Range(MIN_WEIGHT, MAX_WEIGHT)]
        public float Weight = DEFAULT_WEIGHT;

        #endregion

        #region Constructors

        /// <summary> Creates a new instance for this class </summary>
        public AudioData() { Reset(); }

        /// <summary> Creates a new instance for this class and sets the given AudioClip reference </summary>
        /// <param name="audioClip"> AudioClip reference </param>
        public AudioData(AudioClip audioClip)
        {
            Reset();
            AudioClip = audioClip;
        }

        /// <summary> Creates a new instance for this class and sets the given AudioClip reference with the given weight </summary>
        /// <param name="audioClip"> AudioClip reference </param>
        /// <param name="weight"> (Not Implemented) AudioClip weight </param>
        public AudioData(AudioClip audioClip, float weight)
        {
            Reset();
            AudioClip = audioClip;
            Weight = weight;
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            AudioClip = null;
            Weight = 1;
        }

        #endregion
    }
}