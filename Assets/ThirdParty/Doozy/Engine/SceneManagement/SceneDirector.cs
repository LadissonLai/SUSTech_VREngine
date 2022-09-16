// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Progress;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.SceneManagement
{
    /// <summary>
    ///     Loads and unloads scenes and has callbacks for when the active scene changed, a scene was loaded and a scene was unloaded.
    /// </summary>
    [AddComponentMenu(MenuUtils.SceneDirector_AddComponentMenu_MenuName, MenuUtils.SceneDirector_AddComponentMenu_Order)]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(DoozyExecutionOrder.SCENE_DIRECTOR)]
    public class SceneDirector : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.SceneDirector_MenuItem_ItemName, false, MenuUtils.SceneDirector_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Singleton

        protected SceneDirector() { }

        private static SceneDirector s_instance;

        /// <summary> Returns a reference to the SceneDirector in the Scene. If one does not exist, it gets created </summary>
        public static SceneDirector Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<SceneDirector>();
                if (s_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        private static bool ApplicationIsQuitting { get; set; }

        #endregion

        #region Properties

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugSceneDirector; } }

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary>
        ///     UnityEvent executed when the active Scene has changed.
        ///     <para />
        ///     Includes references to the replaced Scene and the next Scene.
        /// </summary>
        public ActiveSceneChangedEvent OnActiveSceneChanged = new ActiveSceneChangedEvent();

        /// <summary>
        ///     UnityEvent executed when a Scene has loaded.
        ///     <para />
        ///     Includes references to the loaded Scene and the used LoadSceneMode.
        /// </summary>
        public SceneLoadedEvent OnSceneLoaded = new SceneLoadedEvent();

        /// <summary>
        ///     UnityEvent executed when a Scene has unloaded.
        ///     <para />
        ///     Includes a reference to the unloaded Scene.
        /// </summary>
        public SceneUnloadedEvent OnSceneUnloaded = new SceneUnloadedEvent();

        #endregion

        #region Unity Methods

#if UNITY_2019_3_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RunOnStart()
        {
            ApplicationIsQuitting = false;
        }
#endif
        
        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                DDebug.Log("There cannot be two " + typeof(SceneDirector) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
        }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Private Methods

        /// <summary> Method called by the SceneManager.activeSceneChanged UnityAction </summary>
        /// <param name="current"> Replaced Scene </param>
        /// <param name="next"> Next Scene </param>
        private void ActiveSceneChanged(Scene current, Scene next)
        {
            if (OnActiveSceneChanged == null) return;
            OnActiveSceneChanged.Invoke(current, next);
            if (DebugComponent) DDebug.Log("Active Scene Changed - Replaced Scene: " + current.name + " / Next Scene: " + next.name, this);
        }

        /// <summary> Method called by the SceneManager.sceneLoaded UnityAction </summary>
        /// <param name="scene"> Loaded Scene </param>
        /// <param name="mode"> LoadSceneMode used to load the scene </param>
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (OnSceneLoaded == null) return;
            OnSceneLoaded.Invoke(scene, mode);
            if (DebugComponent) DDebug.Log("Scene Loaded - Scene: " + scene.name + " / LoadSceneMode: " + mode, this);
        }

        /// <summary> Method called by the SceneManager.sceneUnloaded UnityAction </summary>
        /// <param name="unloadedScene"> Unloaded Scene</param>
        private void SceneUnloaded(Scene unloadedScene)
        {
            if (OnSceneUnloaded == null) return;
            OnSceneUnloaded.Invoke(unloadedScene);
            if (DebugComponent) DDebug.Log("Scene Unloaded - Scene: " + unloadedScene.name, this);
        }

        #endregion

        #region Static Methods

        /// <summary> Create a SceneLoader that loads the Scene asynchronously in the background by its index in Build Settings, then returns a reference to the newly created SceneLoader </summary>
        /// <param name="sceneBuildIndex"> Index of the Scene in the Build Settings to load </param>
        /// <param name="loadSceneMode"> If LoadSceneMode.Single then all current Scenes will be unloaded before loading </param>
        /// <param name="progressor"> Progressor that will get referenced to the SceneLoader, to get updated while the scene loads </param>
        public static SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode, Progressor progressor = null)
        {
            if (Instance.DebugComponent) DDebug.Log("LoadSceneAsync - sceneBuildIndex: " + sceneBuildIndex + " / loadSceneMode: " + loadSceneMode + " / has Progressor: " + (progressor == null ? "No" : "Yes"), Instance);
            SceneLoader loader = SceneLoader.GetLoader();
            loader.SetSceneBuildIndex(sceneBuildIndex)
                  .SetLoadSceneBy(GetSceneBy.BuildIndex)
                  .SetProgressor(progressor)
                  .SetLoadSceneMode(loadSceneMode)
                  .LoadSceneAsync();
            return loader;
        }

        /// <summary> Create a SceneLoader that loads the Scene asynchronously in the background by its name in Build Settings, then returns a reference to the newly created SceneLoader </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        /// <param name="loadSceneMode"> If LoadSceneMode.Single then all current Scenes will be unloaded before loading </param>
        /// <param name="progressor"> Progressor that will get referenced to the SceneLoader, to get updated while the scene loads </param>
        public static SceneLoader LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode, Progressor progressor = null)
        {
            if (Instance.DebugComponent) DDebug.Log("LoadSceneAsync - sceneName: " + sceneName + " / loadSceneMode: " + loadSceneMode + " / has Progressor: " + (progressor == null ? "No" : "Yes"), Instance);
            SceneLoader loader = SceneLoader.GetLoader();
            loader.SetSceneName(sceneName)
                  .SetLoadSceneBy(GetSceneBy.Name)
                  .SetProgressor(progressor)
                  .SetLoadSceneMode(loadSceneMode)
                  .LoadSceneAsync();
            return loader;
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="scene"> Scene to unload. </param>
        public static AsyncOperation UnloadSceneAsync(Scene scene)
        {
            if (Instance.DebugComponent) DDebug.Log("UnloadSceneAsync - scene: " + scene.name, Instance);
            return SceneManager.UnloadSceneAsync(scene);
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="sceneBuildIndex"> Index of the Scene in BuildSettings </param>
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            if (Instance.DebugComponent) DDebug.Log("UnloadSceneAsync - sceneBuildIndex: " + sceneBuildIndex, Instance);
            return SceneManager.UnloadSceneAsync(sceneBuildIndex);
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="sceneName"> Name or path of the Scene to unload. </param>
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            if (Instance.DebugComponent) DDebug.Log("UnloadSceneAsync - sceneName: " + sceneName, Instance);
            return SceneManager.UnloadSceneAsync(sceneName);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> Adds SceneDirector to scene and returns a reference to it </summary>
        public static SceneDirector AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<SceneDirector>(MenuUtils.SceneDirector_GameObject_Name, true, selectGameObjectAfterCreation); }

        #endregion
    }
}