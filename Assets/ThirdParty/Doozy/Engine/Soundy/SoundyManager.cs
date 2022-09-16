// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Soundy
{
    /// <inheritdoc />
    /// <summary>
    /// Central component of the Soundy system that binds all the sound sub-systems together.
    /// It gets the SoundGroupData references from the SoundyDatabase and passes them to the SoundyPooler, that in turn manages and uses SoundyControllers to play the sounds. 
    /// </summary>
    [AddComponentMenu(MenuUtils.SoundyManager_AddComponentMenu_MenuName, MenuUtils.SoundyManager_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.SOUNDY_MANAGER)]
    public class SoundyManager : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.SoundyManager_MenuItem_ItemName, false, MenuUtils.SoundyManager_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected SoundyManager() { }

        private static SoundyManager s_instance;

        /// <summary> Returns a reference to the SoundyManager in the Scene. If one does not exist, it gets created. </summary>
        public static SoundyManager Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<SoundyManager>();
                // ReSharper disable once RedundantArgumentDefaultValue
                if (s_instance == null) DontDestroyOnLoad(AddToScene(false).gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Constants

        public const string DATABASE = "Database";
        public const string GENERAL = "General";
        public const string NEW_SOUND_GROUP = "New Sound Group";
        public const string NO_SOUND = "No Sound";
        public const string SOUNDS = "Sounds";
        public const string SOUNDY = "Soundy";

        #endregion

        #region Static Properties        

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool ApplicationIsQuitting = false;

        /// <summary> Internal variable that keeps track if this class has been initialized </summary>
        private static bool s_initialized;

        /// <summary> Internal variable that keeps a reference to the SoundyPooler component attached to this GameObject </summary>
        private static SoundyPooler s_pooler;


        /// <summary> Returns a reference to the SoundyPooler component on this GameObject. If one does not exist, it gets added. </summary>
        public static SoundyPooler Pooler
        {
            get
            {
                if (s_pooler != null) return s_pooler;
                s_pooler = Instance.gameObject.GetComponent<SoundyPooler>();
                if (s_pooler == null) s_pooler = Instance.gameObject.AddComponent<SoundyPooler>();
                return s_pooler;
            }
        }

        /// <summary> Direct reference to the SoundyDatabase asset </summary>
        public static SoundyDatabase Database { get { return SoundySettings.Database; } }

        #endregion

        #region Properties

        private bool DebugComponent { get { return DoozySettings.Instance.DebugSoundyManager; } }

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            ApplicationIsQuitting = false;
            s_initialized = false;
            s_pooler = null;
        }
#endif
        
        private void Awake() { s_initialized = true; }

        #endregion

        #region Static Methods

        /// <summary> Add SoundyManager to scene and returns a reference to it </summary>
        public static SoundyManager AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<SoundyManager>(MenuUtils.SoundyManager_GameObject_Name, true, selectGameObjectAfterCreation); }

        /// <summary> Create a new SoundyController in the current scene and get a reference to it </summary>
        public static SoundyController GetController() { return SoundyController.GetController(); }

        /// <summary> Returns a proper formatted filename for a given database name </summary>
        /// <param name="databaseName"> Database name </param>
        public static string GetSoundDatabaseFilename(string databaseName) { return "SoundDatabase_" + databaseName.Trim(); }

        /// <summary> Initializes the SoundyManager Instance </summary>
        public static void Init()
        {
            if (s_initialized || s_instance != null) return;
            s_instance = Instance;
            for (int i = 0; i < SoundyPooler.MinimumNumberOfControllers + 1; i++)
                SoundyPooler.GetControllerFromPool().Stop();
        }

        /// <summary> Stop all SoundyControllers from playing and destroys the GameObjects they are attached to </summary>
        public static void KillAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Kill All Controllers", Instance);
            SoundyController.KillAll();
        }

        /// <summary> Mute all the SoundyControllers </summary>
        public static void MuteAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Mute All Controllers", Instance);
            SoundyController.MuteAll();
        }

        /// <summary> Mute all sound sources (including MasterAudio) </summary>
        public static void MuteAllSounds()
        {
            if (Instance.DebugComponent) DDebug.Log("Mute All Sounds", Instance);
            MuteAllControllers();
#if dUI_MasterAudio
            DarkTonic.MasterAudio.MasterAudio.MuteEverything();
#endif
        }

        /// <summary> Pause all the SoundyControllers that are currently playing </summary>
        public static void PauseAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Pause All Controllers", Instance);
            SoundyController.PauseAll();
        }

        /// <summary> Pause all sound sources (including MasterAudio) </summary>
        public static void PauseAllSounds()
        {
            if (Instance.DebugComponent) DDebug.Log("Pause All Sounds", Instance);

            PauseAllControllers();
#if dUI_MasterAudio
            DarkTonic.MasterAudio.MasterAudio.PauseEverything();
#endif
        }

        /// <summary>
        /// Play the specified sound at the given position.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if no sound is found.
        /// </summary>
        /// <param name="databaseName"> The sound category </param>
        /// <param name="soundName"> Sound Name of the sound </param>
        /// <param name="position"> The position from where this sound will play from </param>
        public static SoundyController Play(string databaseName, string soundName, Vector3 position)
        {
            if (!s_initialized) s_instance = Instance;
            if (Database == null) return null;
            if (soundName.Equals(NO_SOUND)) return null;
            SoundGroupData soundGroupData = Database.GetAudioData(databaseName, soundName);
            if (soundGroupData == null) return null;
            if (Instance.DebugComponent) DDebug.Log("Play '" + databaseName + "' / '" + soundName + "' SoundGroupData at " + position + " position", Instance);
            return soundGroupData.Play(position, Database.GetSoundDatabase(databaseName).OutputAudioMixerGroup);
        }

        /// <summary>
        /// Play the specified sound, at the given position.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if the AudioClip is null.
        /// </summary>
        /// <param name="audioClip"> The AudioClip to play </param>
        /// <param name="position"> The position from where this sound will play from </param>
        public static SoundyController Play(AudioClip audioClip, Vector3 position)
        {
            if (!s_initialized) s_instance = Instance;
            return Play(audioClip, null, position);
        }

        /// <summary>
        /// Play the specified sound and follow a given target Transform while playing.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if no sound is found.
        /// </summary>
        /// <param name="databaseName"> The sound category </param>
        /// <param name="soundName"> Sound Name of the sound </param>
        /// <param name="followTarget"> The target transform that the sound will follow while playing </param>
        public static SoundyController Play(string databaseName, string soundName, Transform followTarget)
        {
            if (!s_initialized) s_instance = Instance;
            if (Database == null) return null;
            if (soundName.Equals(NO_SOUND)) return null;
            SoundGroupData soundGroupData = Database.GetAudioData(databaseName, soundName);
            if (soundGroupData == null) return null;
            if (Instance.DebugComponent) DDebug.Log("Play '" + databaseName + "' / '" + soundName + "' SoundGroupData and follow the '" + followTarget.name + "' GameObject", Instance);
            return soundGroupData.Play(followTarget, Database.GetSoundDatabase(databaseName).OutputAudioMixerGroup);
        }

        /// <summary>
        /// Play the specified sound, at the set position.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if the AudioClip is null.
        /// </summary>
        /// <param name="audioClip"> The AudioClip to play </param>
        /// <param name="followTarget"> The target transform that the sound will follow while playing </param>
        public static SoundyController Play(AudioClip audioClip, Transform followTarget)
        {
            if (!s_initialized) s_instance = Instance;
            return Play(audioClip, null, followTarget);
        }

        /// <summary>
        /// Play the specified sound with the given category, name and type.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if no sound is found.
        /// </summary>
        /// <param name="databaseName"> The sound category </param>
        /// <param name="soundName"> Sound Name of the sound </param>
        public static SoundyController Play(string databaseName, string soundName)
        {
            if (!s_initialized) s_instance = Instance;
            if (Database == null) return null;
            if (soundName.Equals(NO_SOUND)) return null;
            if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(databaseName.Trim())) return null;
            if (string.IsNullOrEmpty(soundName) || string.IsNullOrEmpty(soundName.Trim())) return null;

            SoundDatabase soundDatabase = Database.GetSoundDatabase(databaseName);
            if (soundDatabase == null) return null;
            SoundGroupData soundGroupData = soundDatabase.GetData(soundName);
            if (soundGroupData == null) return null;
            return soundGroupData.Play(Pooler.transform, soundDatabase.OutputAudioMixerGroup);
        }

        /// <summary>
        /// Play the passed AudioClip.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if the AudioClip is null.
        /// </summary>
        /// <param name="audioClip"> The AudioClip to play </param>
        public static SoundyController Play(AudioClip audioClip)
        {
            if (!s_initialized) s_instance = Instance;
            return Play(audioClip, null, Pooler.transform);
        }

        /// <summary>
        /// Play the specified audio clip with the given parameters, at the set position.
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if the AudioClip is null.
        /// </summary>
        /// <param name="audioClip"> The AudioClip to play </param>
        /// <param name="outputAudioMixerGroup"> The output audio mixer group that this sound will get routed through </param>
        /// <param name="position"> The position from where this sound will play from </param>
        /// <param name="volume"> The volume of the audio source (0.0 to 1.0) </param>
        /// <param name="pitch"> The pitch of the audio source </param>
        /// <param name="loop"> Is the audio clip looping? </param>
        /// <param name="spatialBlend">
        ///     Sets how much this AudioSource is affected by 3D space calculations (attenuation,
        ///     doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D
        /// </param>
        public static SoundyController Play(AudioClip audioClip, AudioMixerGroup outputAudioMixerGroup, Vector3 position,
                                            float volume = 1, float pitch = 1, bool loop = false, float spatialBlend = 1)
        {
            if (!s_initialized) s_instance = Instance;
            if (audioClip == null) return null;
            SoundyController controller = SoundyPooler.GetControllerFromPool();
            controller.SetSourceProperties(audioClip, volume, pitch, loop, spatialBlend);
            controller.SetOutputAudioMixerGroup(outputAudioMixerGroup);
            controller.SetPosition(position);
            controller.gameObject.name = "[AudioClip]-(" + audioClip.name + ")";
            controller.Play();
            if (Instance.DebugComponent) DDebug.Log("Play '" + audioClip.name + "' AudioClip", Instance);
            return controller;
        }

        /// <summary>
        /// Play the specified audio clip with the given parameters (and follow a given Transform while playing).
        /// Returns a reference to the SoundyController that is playing the sound.
        /// Returns null if the AudioClip is null.
        /// </summary>
        /// <param name="audioClip"> The AudioClip to play </param>
        /// <param name="outputAudioMixerGroup"> The output audio mixer group that this sound will get routed through </param>
        /// <param name="followTarget"> The target transform that the sound will follow while playing </param>
        /// <param name="volume"> The volume of the audio source (0.0 to 1.0) </param>
        /// <param name="pitch"> The pitch of the audio source </param>
        /// <param name="loop"> Is the audio clip looping? </param>
        /// <param name="spatialBlend">
        ///     Sets how much this AudioSource is affected by 3D space calculations (attenuation,
        ///     doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D
        /// </param>
        public static SoundyController Play(AudioClip audioClip, AudioMixerGroup outputAudioMixerGroup,
                                            Transform followTarget = null, float volume = 1, float pitch = 1, bool loop = false,
                                            float spatialBlend = 1)
        {
            if (!s_initialized) s_instance = Instance;
            if (audioClip == null) return null;
            SoundyController controller = SoundyPooler.GetControllerFromPool();
            controller.SetSourceProperties(audioClip, volume, pitch, loop, spatialBlend);
            controller.SetOutputAudioMixerGroup(outputAudioMixerGroup);
            if (followTarget == null)
            {
                spatialBlend = 0;
                controller.SetFollowTarget(Pooler.transform);
            }
            else
            {
                controller.SetFollowTarget(followTarget);
            }

            controller.gameObject.name = "[AudioClip]-(" + audioClip.name + ")";
            controller.Play();
            if (Instance.DebugComponent) DDebug.Log("Play '" + audioClip.name + "' AudioClip", Instance);
            return controller;
        }

        /// <summary>
        /// Play a sound according to the settings in the SoundyData reference.
        /// Returns a reference to the SoundyController that is playing the sound if data.SoundSource is set to either Soundy or AudioClip.
        /// If data is null or data.SoundSource is set to MasterAudio, it will always return null because MasterAudio is the one playing the sound and not a SoundyController </summary>
        /// <param name="data"> Sound settings </param>
        public static SoundyController Play(SoundyData data)
        {
            if (data == null) return null;
            if (!s_initialized) s_instance = Instance;
            switch (data.SoundSource)
            {
                case SoundSource.Soundy:
                    return Play(data.DatabaseName, data.SoundName);
                case SoundSource.AudioClip:
                    return Play(data.AudioClip, data.OutputAudioMixerGroup);
                case SoundSource.MasterAudio:
                    if (Instance.DebugComponent) DDebug.Log("Play '" + data.SoundName + "' with MasterAudio", Instance);
#if dUI_MasterAudio
                    DarkTonic.MasterAudio.MasterAudio.PlaySound(data.SoundName);
                    //DDebug.Log("MasterAudio - Play Sound: " + data.SoundName);
#endif
                    break;
            }

            return null;
        }

        /// <summary> Stop all the SoundyControllers that are currently playing </summary>
        public static void StopAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Stop All Controllers", Instance);
            SoundyController.StopAll();
        }

        /// <summary> Stop all sound sources (including MasterAudio) </summary>
        public static void StopAllSounds()
        {
            if (Instance.DebugComponent) DDebug.Log("Stop All Sounds", Instance);
            StopAllControllers();
#if dUI_MasterAudio
            DarkTonic.MasterAudio.MasterAudio.StopEverything();
#endif
        }

        /// <summary> Unmute all the SoundyControllers that were previously muted </summary>
        public static void UnmuteAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Unmute All Controllers", Instance);
            SoundyController.UnmuteAll();
        }

        /// <summary> Unmute all sound sources (including MasterAudio) </summary>
        public static void UnmuteAllSounds()
        {
            if (Instance.DebugComponent) DDebug.Log("Unmute All Sounds", Instance);
            UnmuteAllControllers();
#if dUI_MasterAudio
            DarkTonic.MasterAudio.MasterAudio.UnmuteEverything();
#endif
        }

        /// <summary> Unpause all the SoundyControllers that were previously paused </summary>
        public static void UnpauseAllControllers()
        {
            if (Instance.DebugComponent) DDebug.Log("Unpause All Controllers", Instance);
            SoundyController.UnpauseAll();
        }

        /// <summary> Unpause all sound sources (including MasterAudio) </summary>
        public static void UnpauseAllSounds()
        {
            if (Instance.DebugComponent) DDebug.Log("Unpause All Sounds", Instance);
            UnpauseAllControllers();
#if dUI_MasterAudio
            DarkTonic.MasterAudio.MasterAudio.UnpauseEverything();
#endif
        }

        #endregion
    }
}