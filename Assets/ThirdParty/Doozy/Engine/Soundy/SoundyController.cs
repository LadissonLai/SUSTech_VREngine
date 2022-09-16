// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.Audio;

// ReSharper disable ConvertToAutoProperty
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <inheritdoc />
    /// <summary>
    ///     This is an audio controller used by the Soundy system to play sounds. Each sound has its own controller that handles it.
    ///     Every SoundyController is also added to the SoundyPooler to work seamlessly with the dynamic sound pooling system.
    /// </summary>
    [DefaultExecutionOrder(DoozyExecutionOrder.SOUNDY_CONTROLLER)]
    public class SoundyController : MonoBehaviour
    {
        #region Static Properties

        /// <summary> Internal list of all the available controllers </summary>
        private static List<SoundyController> s_database = new List<SoundyController>();

        private static bool DebugComponent { get { return DoozySettings.Instance.DebugSoundyController; } }

        /// <summary> Global variable that keeps track if all controllers are paused or not </summary>
        private static bool s_pauseAllControllers;
        
        /// <summary> Global toggle to pause / unpause all controllers </summary>
        public static bool PauseAllControllers
        {
            get
            {
                return s_pauseAllControllers;
            }
            set
            {
                s_pauseAllControllers = value;
                if (s_pauseAllControllers) return;
                RemoveNullControllersFromDatabase();
                foreach (SoundyController controller in s_database)
                    controller.Unpause();
            }
        }

        /// <summary> Global variable that keeps track if all controllers are muted or not </summary>
        private static bool s_muteAllControllers;
        
        /// <summary> Global toggle to mute / unmute all controllers </summary>
        public static bool MuteAllControllers
        {
            get
            {
                return s_muteAllControllers;
            }
            set
            {
                s_muteAllControllers = value;
                if (s_muteAllControllers) return;
                RemoveNullControllersFromDatabase();
                foreach (SoundyController controller in s_database)
                    controller.Unmute();
            }
        }

        #endregion

        #region Properties

        /// <summary> Target AudioSource component </summary>
        public AudioSource AudioSource { get { return m_audioSource; } private set { m_audioSource = value; } }

        /// <summary> Keeps track if this controller is in use or idle </summary>
        public bool InUse { get { return m_inUse; } private set { m_inUse = value; } }

        /// <summary> Keeps track of the currently playing AudioClip play progress </summary>
        public float PlayProgress { get { return m_playProgress; } private set { m_playProgress = value; } }

        /// <summary> Keeps track if this controller is paused or not </summary>
        public bool IsPaused { get { return m_isPaused || s_pauseAllControllers; } private set { m_isPaused = value; } }

        /// <summary> Keeps track if this controller is muted or not </summary>
        public bool IsMuted { get { return m_isMuted || MuteAllControllers; } private set { m_isMuted = value; } }

        /// <summary> Keeps track of when was the last time this controller was used (info needed for the dynamic pooling system) </summary>
        public float LastPlayedTime { get { return m_lastPlayedTime; } private set { m_lastPlayedTime = value; } }

        /// <summary> Returns the duration since this controller has been used last </summary>
        public float IdleDuration { get { return Time.realtimeSinceStartup - LastPlayedTime; } }

        #endregion

        #region Private Variables

        /// <summary> Internal variable that keeps a reference to the self Transform </summary>
        private Transform m_transform;

        /// <summary> Internal variable that keeps a reference to the current follow target </summary>
        private Transform m_followTarget;

        /// <summary> Internal variable that keeps a reference to the target AudioSource </summary>
        private AudioSource m_audioSource;

        /// <summary> Internal variable that keeps track if this controller is in use or idle </summary>
        private bool m_inUse;

        /// <summary> Internal variable that keeps track of the currently playing AudioClip play progress </summary>
        private float m_playProgress;

        /// <summary> Internal variable that keeps track if this controller is paused or not </summary>
        private bool m_isPaused;

        /// <summary> Internal variable that keeps track if this controller is muted or not </summary>
        private bool m_isMuted;

        /// <summary> Internal variable that keeps track of when was the last time this controller was used </summary>
        private float m_lastPlayedTime;

        /// <summary> Internal variable that keeps track if the Play() method has been called on this controller. It's set to FALSE when the Stop() method is called. </summary>
        private bool m_isPlaying;

        /// <summary> Internal variable used to detect then Unity Pauses this controller's AudioSource (this happens on app switch for example and Unity does not give any info about this happening) </summary>
        private bool m_autoPaused;

        private bool m_muted, m_paused;

        #endregion

        #region Unity Methods

        private void Reset() { ResetController(); }

        private void Awake()
        {
            s_database.Add(this);
            m_transform = transform;
            AudioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
            ResetController();
        }

        private void OnDestroy() { s_database.Remove(this); }

        private void Update()
        {
            if (IsMuted || IsPaused || AudioSource.isPlaying) UpdateLastPlayedTime();
            
            if (IsMuted != m_muted)
            {
                AudioSource.mute = IsMuted;
                m_muted = IsMuted;
            }

            if (IsPaused != m_paused)
            {
                if (IsPaused && AudioSource.isPlaying) AudioSource.Pause();
                if (!IsPaused) AudioSource.UnPause();
                m_paused = IsPaused;
            }

            UpdatePlayProgress();

            if (PlayProgress >= 1f) //check if the sound finished playing
            {
                Stop();
                PlayProgress = 0;
                return;
            }

//            if (DebugComponent) DDebug.Log("InUse: " + InUse + " / AudioSource.isPlaying: " + AudioSource.isPlaying + " / IsPaused: " + IsPaused + " / IsMuted: " + IsMuted);

            m_autoPaused = InUse && m_isPlaying && !AudioSource.isPlaying && PlayProgress > 0;

            if (InUse && !m_autoPaused && !AudioSource.isPlaying && !IsPaused && !IsMuted) //second check if the sound finished playing
            {
                Stop();
                return;
            }

            FollowTarget();
        }

        #endregion

        #region Public Methods

        /// <summary> Stop playing and destroy the GameObject this controller is attached to </summary>
        public void Kill()
        {
            Stop();
            if (DebugComponent) DDebug.Log("Kill '" + name + "' SoundyController", this);
            Destroy(gameObject);
        }

        /// <summary> Mute the target AudioSource </summary>
        public void Mute()
        {
            IsMuted = true;
            if (DebugComponent) DDebug.Log("Mute '" + name + "' SoundyController", this);
        }

        /// <summary> Pause the target AudioSource </summary>
        public void Pause()
        {
            IsPaused = true;
            if (DebugComponent) DDebug.Log("Pause '" + name + "' SoundyController", this);
        }

        /// <summary> Start Play on the target AudioSource </summary>
        public void Play()
        {
            InUse = true;
            IsPaused = false;
            m_isPlaying = true;
            AudioSource.Play();
            if (DebugComponent) DDebug.Log("Play '" + name + "' SoundyController", this);
        }

        /// <summary> Set a follow target Transform that this controller needs to follow while playing </summary>
        /// <param name="followTarget"> The target Transform </param>
        public void SetFollowTarget(Transform followTarget) { m_followTarget = followTarget; }

        /// <summary> Set an output AudioMixerGroup to the target AudioSource of this controller </summary>
        /// <param name="outputAudioMixerGroup"> Target output AudioMixerGroup </param>
        public void SetOutputAudioMixerGroup(AudioMixerGroup outputAudioMixerGroup)
        {
            if (outputAudioMixerGroup == null) return;
            AudioSource.outputAudioMixerGroup = outputAudioMixerGroup;
        }

        /// <summary> Set the position in world space from where this controller will be playing from </summary>
        /// <param name="position"> The new position </param>
        public void SetPosition(Vector3 position) { m_transform.position = position; }

        /// <summary> Set the given settings to the target AudioSource </summary>
        /// <param name="clip"> The AudioClip to play </param>
        /// <param name="volume"> The volume of the audio source (0.0 to 1.0) </param>
        /// <param name="pitch"> The pitch of the audio source </param>
        /// <param name="loop"> Is the audio clip looping? </param>
        /// <param name="spatialBlend"> Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D </param>
        public void SetSourceProperties(AudioClip clip, float volume, float pitch, bool loop, float spatialBlend)
        {
            if (clip == null)
            {
                Stop();
                return;
            }

            AudioSource.clip = clip;
            AudioSource.volume = volume;
            AudioSource.pitch = pitch;
            AudioSource.loop = loop;
            AudioSource.spatialBlend = spatialBlend;
        }

        /// <summary> Stop the target AudioSource from playing </summary>
        public void Stop()
        {
            Unpause();
            Unmute();
            AudioSource.Stop();
            m_isPlaying = false;
            if (DebugComponent) DDebug.Log("Stop '" + name + "' SoundyController", this);
            ResetController();
            SoundyPooler.PutControllerInPool(this);
        }

        /// <summary> Unmute the target AudioSource if it was previously muted </summary>
        public void Unmute()
        {
            IsMuted = false;
            if (DebugComponent) DDebug.Log("Unmute '" + name + "' SoundyController", this);
        }

        /// <summary> Unpause the target AudioSource if it was previously paused </summary>
        public void Unpause()
        {
            IsPaused = false;
            if (DebugComponent) DDebug.Log("Unpause '" + name + "' SoundyController", this);
        }

        #endregion

        #region Private Methods

        private void FollowTarget()
        {
            if (m_followTarget == null) return;
            m_transform.position = m_followTarget.position;
            if (DebugComponent) DDebug.Log(name + " is following the '" + m_followTarget.name + "' GameObject", this);
        }

        private void ResetController()
        {
            InUse = false;
            IsPaused = false;
            m_followTarget = null;
            UpdateLastPlayedTime();
        }

        private void UpdateLastPlayedTime() { LastPlayedTime = Time.realtimeSinceStartup; }

        private void UpdatePlayProgress()
        {
            if (AudioSource == null) return;
            if (AudioSource.clip == null) return;
            PlayProgress = Mathf.Clamp01(AudioSource.time / AudioSource.clip.length);
        }

        #endregion

        #region Static Methods

        /// <summary> Create a new SoundyController in the current scene and get a reference to it </summary>
        public static SoundyController GetController()
        {
            var controller = new GameObject("SoundyController", typeof(AudioSource), typeof(SoundyController)).GetComponent<SoundyController>();
            return controller;
        }

        /// <summary> Stop all controllers from playing and destroy the GameObjects they are attached to </summary>
        public static void KillAll()
        {
            if (DebugComponent) DDebug.Log("Kill All");
            RemoveNullControllersFromDatabase();
            foreach (SoundyController controller in s_database)
                controller.Kill();
        }

        /// <summary> Mute all the controllers </summary>
        public static void MuteAll()
        {
            if (DebugComponent) DDebug.Log("Mute All");
            RemoveNullControllersFromDatabase();
            MuteAllControllers = true;
        }

        /// <summary> Pause all the controllers that are currently playing </summary>
        public static void PauseAll()
        {
            if (DebugComponent) DDebug.Log("Pause All");
            RemoveNullControllersFromDatabase();
            PauseAllControllers = true;
        }

        /// <summary> Remove any null controller references from the database </summary>
        public static void RemoveNullControllersFromDatabase() { s_database = s_database.Where(sc => sc != null).ToList(); }

        /// <summary> Stop all the controllers that are currently playing </summary>
        public static void StopAll()
        {
            if (DebugComponent) DDebug.Log("Stop All");
            RemoveNullControllersFromDatabase();
            foreach (SoundyController controller in s_database)
            {
//                if (!controller.AudioSource.isPlaying) return;
                controller.Stop();
            }
        }

        /// <summary> Unmute all the controllers that were previously muted </summary>
        public static void UnmuteAll()
        {
            if (DebugComponent) DDebug.Log("Unmute All");
            RemoveNullControllersFromDatabase();
            MuteAllControllers = false;
        }

        /// <summary> Unpause all the controllers that were previously paused </summary>
        public static void UnpauseAll()
        {
            if (DebugComponent) DDebug.Log("Unpause All");
            PauseAllControllers = false;
        }

        #endregion
    }
}