// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Audio;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <summary>
    ///     Data settings container used by the Actions class in order to trigger the playing of a sound via Soundy, a direct AudioClip reference or a MasterAudio sound name.
    /// </summary>
    [Serializable]
    public class SoundyData
    {
        #region Public Variables

        /// <summary> Selects the sound source that will provide the sound that will get played </summary>
        public SoundSource SoundSource;

        /// <summary> SoundDatabase database name that contains the sound name (enabled only for SoundSource.Soundy) </summary>
        public string DatabaseName;
        
        /// <summary> Sound name of a SoundGroupData that holds settings and references to one or more audio clips (enabled only for SoundSource.Soundy and SoundSource.MasterAudio) </summary>
        public string SoundName;

        /// <summary> Direct reference to an AudioClip (enabled only for SoundSource.AudioClip) </summary>
        public AudioClip AudioClip;

        /// <summary> Direct reference to an AudioMixerGroup that the referenced AudioClip will get routed through when played (enabled only for SoundSource.AudioClip) </summary>
        public AudioMixerGroup OutputAudioMixerGroup;

        #endregion

        #region Constructors

        /// <summary> Creates a new instance for this class </summary>
        public SoundyData() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Returns a SoundGroupData reference from the database with the set DatabaseName and SoundName.
        ///     If the sound database does not exist, or no SoundGroupData with the given sound name is found, it returns null.
        /// </summary>
        public SoundGroupData GetAudioData() { return SoundySettings.Database.GetAudioData(DatabaseName, SoundName); }

        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            SoundSource = SoundSource.Soundy;
            DatabaseName = SoundyManager.GENERAL;
            SoundName = SoundyManager.NO_SOUND;
            AudioClip = null;
        }

        /// <summary> Sets the AudioClip (usable only if the SoundSource is set to AudioClip) </summary>
        /// <param name="audioClip"> Direct reference to an AudioClip (usable only for SoundSource.AudioClip) </param>
        public SoundyData SetAudioClip(AudioClip audioClip)
        {
            AudioClip = audioClip;
            return this;
        }

        /// <summary> Sets the DatabaseName that contains the sound name (usable only if the SoundSource is set to Soundy) </summary>
        /// <param name="databaseName"> SoundDatabase database name that contains the sound name (enabled only for SoundSource.Soundy) </param>
        public SoundyData SetDatabaseName(string databaseName)
        {
            DatabaseName = databaseName;
            return this;
        }

        /// <summary> Sets the AudioMixerGroup (usable only if the SoundSource is set to AudioClip) </summary>
        /// <param name="audioMixerGroup"> Direct reference to an AudioMixerGroup that the referenced AudioClip will get routed through when played (enabled only for SoundSource.AudioClip) </param>
        public SoundyData SetOutputAudioMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            OutputAudioMixerGroup = audioMixerGroup;
            return this;
        }

        /// <summary> Sets the Sound name of a SoundGroupData that holds settings and references to one or more audio clips (usable only if the SoundSource is set to Soundy or MasterAudio) </summary>
        /// <param name="soundName"> Sound name of a SoundGroupData that holds settings and references to one or more audio clips (enabled only for SoundSource.Soundy and SoundSource.MasterAudio) </param>
        public SoundyData SetSoundName(string soundName)
        {
            SoundName = soundName;
            return this;
        }

        /// <summary> Sets the SoundSource that will provide the sound that will get played </summary>
        /// <param name="soundSource"> Selects the sound source that will provide the sound that will get played  </param>
        public SoundyData SetSoundSource(SoundSource soundSource)
        {
            SoundSource = soundSource;
            return this;
        }

        #endregion
    }
}