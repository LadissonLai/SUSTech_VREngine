// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

namespace Doozy.Engine.SceneManagement
{
    /// <inheritdoc />
    /// <summary>
    ///     Loads any Scene either by scene name or scene build index and updates a Progressor to show the loading progress.
    ///     It can also trigger a set of 'actions' when the scene started loading (at 0% load progress) and/or when the scene has been loaded (but not activated) (at 90% load progress)
    /// </summary>
    [AddComponentMenu(MenuUtils.SceneLoader_AddComponentMenu_MenuName, MenuUtils.SceneLoader_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.SCENE_LOADER)]
    public class SceneLoader : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.SceneLoader_MenuItem_ItemName, false, MenuUtils.SceneLoader_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Constants

        public const GetSceneBy DEFAULT_GET_SCENE_BY = GetSceneBy.Name;
        public const LoadSceneMode DEFAULT_LOAD_SCENE_MODE = LoadSceneMode.Single;
        public const bool DEFAULT_AUTO_SCENE_ACTIVATION = true;
        public const bool DEFAULT_SELF_DESTRUCT_AFTER_SCENE_LOADED = false;
        public const float DEFAULT_SCENE_ACTIVATION_DELAY = 0.2f;
        public const int DEFAULT_BUILD_INDEX = 0;
        public const string DEFAULT_SCENE_NAME = "";

        #endregion

        #region Static Properties

        // ReSharper disable once InconsistentNaming
        /// <summary> Database used to keep track of all the SceneLoaders </summary>
        public static readonly List<SceneLoader> Database = new List<SceneLoader>();

        #endregion

        #region Properties

        /// <summary> Keeps track and manages the asyncOperation started when the scene loader begins to load a scene </summary>
        public AsyncOperation CurrentAsyncOperation { get; private set; }

        /// <summary> Returns the inverse value of current load Progress value (float between 1 and 0) </summary>
        public float InverseProgress { get { return 1 - Progress; } }

        /// <summary> If an async operation is running, it returns the current load progress (float between 0 and 1) </summary>
        public float Progress
        {
            get { return m_progress; }
            private set
            {
                m_progress = value;
                if (Progressor != null) Progressor.SetProgress(value);
                OnProgressChanged.Invoke(value);
                OnInverseProgressChanged.Invoke(1 - value);
            }
        }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugSceneLoader; } }

        #endregion

        #region Public Variables

        /// <summary>
        ///     Allow Scenes to be activated as soon as it is ready.
        ///     <para />
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This option can stop the scene activation (at 90% load progress), after the scene has been loaded and is ready.
        ///     <para />
        ///     <para />
        ///     Useful if you need to load several scenes at once and activate them in a specific order and/or at a specific time.
        /// </summary>
        public bool AllowSceneActivation = DEFAULT_AUTO_SCENE_ACTIVATION;

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Behavior when loading a scene </summary>
        public SceneLoadBehavior LoadBehavior = new SceneLoadBehavior();

        /// <summary> Determines what load method this SceneLoader will use by default if the load scene method is called without any parameters </summary>
        public GetSceneBy GetSceneBy = DEFAULT_GET_SCENE_BY;

        /// <summary> Determines how the new scene is loaded by this SceneLoader if the load scene method is called without any parameters </summary>
        public LoadSceneMode LoadSceneMode = DEFAULT_LOAD_SCENE_MODE;

        /// <summary>
        ///     Event triggered when an async operation is running and its progress has been updated.
        ///     <para />
        ///     Passes the Progress (float between 0 and 1)
        /// </summary>
        public ProgressEvent OnProgressChanged = new ProgressEvent();

        /// <summary>
        ///     Event triggered when an async operation is running and its progress has been updated.
        ///     <para />
        ///     Passes the InverseProgress (float between 1 and 0)
        ///     <para />
        ///     InverseProgress = 1 - Progress
        /// </summary>
        public ProgressEvent OnInverseProgressChanged = new ProgressEvent();

        /// <summary> Reference to a Progressor that allows animating anything (texts, images, animations...) in order to show the current scene load progress in a visual manner </summary>
        public Progressor Progressor;

        /// <summary>
        ///     Sets for how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled).
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This delay is after the scene has been loaded and before its activation (at 90% load progress)
        /// </summary>
        public float SceneActivationDelay = DEFAULT_SCENE_ACTIVATION_DELAY;

        /// <summary> Index of the Scene in the Build Settings to load (when GetSceneBy is set to GetSceneBy.BuildIndex) </summary>
        public int SceneBuildIndex = DEFAULT_BUILD_INDEX;

        /// <summary> Name or path of the Scene to load (when GetSceneBy is set to GetSceneBy.Name) </summary>
        public string SceneName = DEFAULT_SCENE_NAME;

        /// <summary> Marks this SceneLoader to self destruct (to destroy itself) after it loads a Scene </summary>
        public bool SelfDestructAfterSceneLoaded = DEFAULT_SELF_DESTRUCT_AFTER_SCENE_LOADED;

        #endregion

        #region Private Variables

        /// <summary> Internal variable used to keep track if a scene load process is currently running </summary>
        private bool m_loadInProgress;

        private bool m_sceneLoadedAndReady; //mark that the scene has not been loaded (load progress has not reached 90%)
        private bool m_activatingScene;
        private float m_sceneLoadedAndReadyTime;

        /// <summary> Internal variable that is updated when an async operation is running (float between 0 and 1) </summary>
        private float m_progress;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            SceneName = DEFAULT_SCENE_NAME;
            SceneBuildIndex = DEFAULT_BUILD_INDEX;
            GetSceneBy = DEFAULT_GET_SCENE_BY;
            LoadSceneMode = DEFAULT_LOAD_SCENE_MODE;
            SceneActivationDelay = DEFAULT_SCENE_ACTIVATION_DELAY;
            LoadBehavior = new SceneLoadBehavior();
            OnProgressChanged = new ProgressEvent();
            OnInverseProgressChanged = new ProgressEvent();
            ResetProgress();
        }

        private void Awake() { Database.Add(this); }

        private void OnEnable() { ResetProgress(); }

        private void OnDestroy() { Database.Remove(this); }

        private void Update()
        {
//            if (!m_loadInProgress) return;
//            Debug.Log("CurrentAsyncOperation == null? " + (CurrentAsyncOperation == null));
            if (CurrentAsyncOperation == null) return;
            Progress = Mathf.Clamp01(CurrentAsyncOperation.progress / 0.9f); //update load progress [0, 0.9] > [0, 1]
            if (DebugComponent && !m_activatingScene) DDebug.Log("[" + name + "] Load progress: " + Mathf.Round(Progress * 100) + "%", this);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!m_sceneLoadedAndReady && CurrentAsyncOperation.progress == 0.9f) // Loading completed
            {
                if (DebugComponent) DDebug.Log("[" + name + "] Scene is ready to be activated.", this);
                LoadBehavior.OnSceneLoaded.Invoke(gameObject);
                m_sceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once)
                m_sceneLoadedAndReadyTime = Time.realtimeSinceStartup;
            }

            if (m_sceneLoadedAndReady && !m_activatingScene && AllowSceneActivation)
            {
                if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                if (SceneActivationDelay >= 0 && Time.realtimeSinceStartup - m_sceneLoadedAndReadyTime > SceneActivationDelay)
                {
                    ActivateLoadedScene();
                    m_activatingScene = true;
                }
            }

            if (!CurrentAsyncOperation.isDone) return;
            if (DebugComponent) DDebug.Log("[" + name + "] Scene has been activated.", this);
            m_loadInProgress = false;
            CurrentAsyncOperation = null;
            if (SelfDestructAfterSceneLoaded) Coroutiner.Start(SelfDestruct());
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Activate the current loaded scene.
        ///     <para />
        ///     <para />
        ///     Works only if the SceneLoader has loaded a scene and its AllowSceneActivation option is set to false.
        ///     <para />
        ///     This method enables the 'allowSceneActivation' for the CurrentAsyncOperation that has been paused at 90%.
        ///     <para />
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This method is meant to be used for after the scene has been loaded and before its activation (at 90% load progress).
        /// </summary>
        public void ActivateLoadedScene()
        {
            if (CurrentAsyncOperation == null) return; //no load process is running
            if (DebugComponent) DDebug.Log("[" + name + "] Activating Scene...", this);
            CurrentAsyncOperation.allowSceneActivation = true;
        }

        /// <summary> Loads the Scene, with the current settings, asynchronously in the background </summary>
        public void LoadSceneAsync()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    LoadSceneAsync(SceneName, LoadSceneMode);
                    break;
                case GetSceneBy.BuildIndex:
                    LoadSceneAsync(SceneBuildIndex, LoadSceneMode);
                    break;
            }
        }

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        /// <param name="mode"> If LoadSceneMode.Single then all current Scenes will be unloaded before activating the newly loaded scene </param>
        public Progressor LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
        {
            ResetProgress();
            LoadBehavior.OnLoadScene.Invoke(gameObject);
            CurrentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            StartSceneLoad();
            return Progressor;
        }

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        /// <param name="mode"> If LoadSceneMode.Single then all current Scenes will be unloaded before activating the newly loaded scene </param>
        public Progressor LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            ResetProgress();
            LoadBehavior.OnLoadScene.Invoke(gameObject);
            CurrentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            StartSceneLoad();
            return Progressor;
        }

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings, with the LoadSceneMode.Additive setting </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public void LoadSceneAsyncAdditive(int sceneBuildIndex) { LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive); }

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings, with the LoadSceneMode.Additive setting </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public void LoadSceneAsyncAdditive(string sceneName) { LoadSceneAsync(sceneName, LoadSceneMode.Additive); }

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings, with the LoadSceneMode.Single setting </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public void LoadSceneAsyncSingle(int sceneBuildIndex) { LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single); }

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings, with the LoadSceneMode.Single setting </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public void LoadSceneAsyncSingle(string sceneName) { LoadSceneAsync(sceneName, LoadSceneMode.Single); }

        /// <summary> Set the AllowSceneActivation that that allows for a Scene to be activated as soon as it is ready </summary>
        /// <param name="allowSceneActivation"> Allow Scenes to be activated as soon as it is ready </param>
        public SceneLoader SetAllowSceneActivation(bool allowSceneActivation)
        {
            AllowSceneActivation = allowSceneActivation;
            return this;
        }

        /// <summary> Set the GetSceneBy value, that determines what load method this SceneLoader will use by default </summary>
        /// <param name="getSceneBy"> Load method this SceneLoader will use if the load scene method is called without any parameters </param>
        public SceneLoader SetLoadSceneBy(GetSceneBy getSceneBy)
        {
            GetSceneBy = getSceneBy;
            return this;
        }

        /// <summary> Set the LoadSceneMode value, that determines how the new scene is loaded by this SceneLoader </summary>
        /// <param name="loadSceneMode"> Load mode used when loading a scene </param>
        public SceneLoader SetLoadSceneMode(LoadSceneMode loadSceneMode)
        {
            LoadSceneMode = loadSceneMode;
            return this;
        }

        /// <summary> Set the Progressor reference that will get updates when this SceneLoader loads a scene </summary>
        /// <param name="progressor"> The Progressor that will get updates when this SceneLoader loads a scene </param>
        public SceneLoader SetProgressor(Progressor progressor)
        {
            Progressor = progressor;
            return this;
        }

        /// <summary> Set the activation delay that determines how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled) </summary>
        /// <param name="sceneActivationDelay"> How long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process </param>
        public SceneLoader SetSceneActivationDelay(float sceneActivationDelay)
        {
            SceneActivationDelay = sceneActivationDelay;
            return this;
        }

        /// <summary> Set the SceneBuildIndex, in the Build Settings, of the Scene to load </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public SceneLoader SetSceneBuildIndex(int sceneBuildIndex)
        {
            SceneBuildIndex = sceneBuildIndex;
            return this;
        }

        /// <summary> Set the SceneName, name or path, of the Scene to load </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public SceneLoader SetSceneName(string sceneName)
        {
            SceneName = sceneName;
            return this;
        }

        /// <summary> Set this SceneLoader to self destruct (to destroy itself) after it loads a Scene </summary>
        /// <param name="selfDestruct"> Should this SceneLoader self destruct? </param>
        public SceneLoader SetSelfDestructAfterSceneLoaded(bool selfDestruct)
        {
            SelfDestructAfterSceneLoaded = selfDestruct;
            return this;
        }

        #endregion

        #region Private Methods

        /// <summary> Sets the scene load Progress to zero </summary>
        private void ResetProgress() { Progress = 0; }

        private void StartSceneLoad()
        {
            CurrentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_loadInProgress = true;                            //mark that a scene load process is running
            m_sceneLoadedAndReady = false;                      //mark that the scene has not been loaded (load progress has not reached 90%)
            m_activatingScene = false;
        }

        #endregion

        #region IEnumerators

        private IEnumerator AsynchronousLoad(string sceneName, LoadSceneMode mode)
        {
//            yield return null;
            ResetProgress();
            LoadBehavior.OnLoadScene.Invoke(gameObject);

            CurrentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

            if (CurrentAsyncOperation == null) yield break;
            CurrentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_loadInProgress = true;                            //mark that a scene load process is running
            bool sceneLoadedAndReady = false;                   //mark that the scene has not been loaded (load progress has not reached 90%)
            bool activatingScene = false;

//            while (!CurrentAsyncOperation.isDone)
            while (m_loadInProgress)
            {
//                if (CurrentAsyncOperation == null) yield break;

                // [0, 0.9] > [0, 1]
                Progress = Mathf.Clamp01(CurrentAsyncOperation.progress / 0.9f); //update load progress
                if (DebugComponent && !activatingScene) DDebug.Log("[" + name + "] Load progress: " + Mathf.Round(Progress * 100) + "%", this);
                DDebug.Log("[" + name + "] Load progress: " + Mathf.Round(Progress * 100) + "%", this);
                // Loading completed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!sceneLoadedAndReady && CurrentAsyncOperation.progress == 0.9f)
                {
//                    Progress = 1f;
                    if (DebugComponent) DDebug.Log("[" + name + "] Scene is ready to be activated.", this);
                    LoadBehavior.OnSceneLoaded.Invoke(gameObject);

                    sceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once) 
                }

                if (sceneLoadedAndReady && !activatingScene)
                {
                    if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                    if (SceneActivationDelay > 0) yield return new WaitForSecondsRealtime(SceneActivationDelay);

                    if (AllowSceneActivation)
                    {
                        ActivateLoadedScene();
                        activatingScene = true;
                    }
                }

                if (CurrentAsyncOperation.isDone)
                {
                    if (DebugComponent) DDebug.Log("[" + name + "] Scene has been activated.", this);
                    m_loadInProgress = false;
//                    CurrentAsyncOperation = null;
                    if (SelfDestructAfterSceneLoaded) Coroutiner.Start(SelfDestruct());
                }

                yield return null;
            }
        }

        private IEnumerator AsynchronousLoad(int sceneBuildIndex, LoadSceneMode mode)
        {
//            yield return null;
            ResetProgress();
            LoadBehavior.OnLoadScene.Invoke(gameObject);

            CurrentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);

            if (CurrentAsyncOperation == null) yield break;
            CurrentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_loadInProgress = true;                            //mark that a scene load process is running
            bool sceneLoadedAndReady = false;                   //mark that the scene has not been loaded (load progress has not reached 90%)
            bool activatingScene = false;

//            while (!CurrentAsyncOperation.isDone)
            while (m_loadInProgress)
            {
//                if (CurrentAsyncOperation == null) yield break;

                // [0, 0.9] > [0, 1]
                Progress = Mathf.Clamp01(CurrentAsyncOperation.progress / 0.9f); //update load progress
                if (DebugComponent && !activatingScene) DDebug.Log("[" + name + "] Load progress: " + Mathf.Round(Progress * 100) + "%", this);
                DDebug.Log("[" + name + "] Load progress: " + Mathf.Round(Progress * 100) + "%", this);
                // Loading completed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!sceneLoadedAndReady && CurrentAsyncOperation.progress == 0.9f)
                {
//                    Progress = 1f;
                    if (DebugComponent) DDebug.Log("[" + name + "] Scene is ready to be activated.", this);
                    LoadBehavior.OnSceneLoaded.Invoke(gameObject);

                    sceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once) 
                }

                if (sceneLoadedAndReady && !activatingScene && AllowSceneActivation)
                {
                    if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                    if (SceneActivationDelay > 0) yield return new WaitForSecondsRealtime(SceneActivationDelay);
                    ActivateLoadedScene();
                    activatingScene = true;
                }

                if (CurrentAsyncOperation.isDone)
                {
                    if (DebugComponent) DDebug.Log("[" + name + "] Scene has been activated.", this);
                    m_loadInProgress = false;
//                    CurrentAsyncOperation = null;
                    if (SelfDestructAfterSceneLoaded) Coroutiner.Start(SelfDestruct());
                }

                yield return null;
            }
        }

        private IEnumerator SelfDestruct()
        {
            yield return null;
            Destroy(gameObject);
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Activates all the loaded scenes for all the SceneLoaders that have scenes ready to be activated.
        ///     <para />
        ///     A scene is ready to be activated if the load progress is at 0.9 (90%).
        /// </summary>
        public static void ActivateLoadedScenes()
        {
            RemoveNullReferencesFromDatabase();
            foreach (SceneLoader sceneLoader in Database)
                sceneLoader.ActivateLoadedScene();
            if (DoozySettings.Instance.DebugSceneLoader) DDebug.Log("Activate Loaded Scenes");
        }

        /// <summary> Creates a new GameObject with a SceneLoader script attached and then returns the reference to the newly created script </summary>
        /// <param name="parent"> Sets a parent for the newly created GameObject </param>
        public static SceneLoader GetLoader(Transform parent = null)
        {
            var loader = new GameObject(typeof(SceneLoader).Name).AddComponent<SceneLoader>();
            if (parent != null) loader.transform.SetParent(parent);
            return loader;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        /// <summary> Adds SceneLoader to scene and returns a reference to it </summary>
        private static SceneLoader AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<SceneLoader>(MenuUtils.SceneLoader_GameObject_Name, false, selectGameObjectAfterCreation); }

        private static void RemoveNullReferencesFromDatabase()
        {
            for (int i = Database.Count - 1; i >= 0; i--)
                if (Database[i] == null)
                    Database.RemoveAt(i);
        }

        #endregion
    }
}