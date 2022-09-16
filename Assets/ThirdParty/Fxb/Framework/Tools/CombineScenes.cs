using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using EditorOpenSceneMode = UnityEditor.SceneManagement.OpenSceneMode;

#endif

namespace Framework.Tools
{
    /// <summary>
    /// 合并场景
    /// </summary>
    [ExecuteInEditMode]
    public class CombineScenes : MonoBehaviour
    {
        public event Action OnSceneReady;

        public Scene placeHoldScene { get; private set;}

        [Serializable]
        public struct ChildSceneParams
        {
            public string sceneName;

            public bool async;

            public bool autoActived;
        }

        Func<string, bool, bool,IEnumerator> SceneLoadHandle;

        /// <summary>
        /// 所有需要被合并进的子场景
        /// </summary>
        public ChildSceneParams[] childScenes;

        void Awake()
        {
            placeHoldScene = SceneManager.GetActiveScene();

#if UNITY_EDITOR
             
            if (Application.isPlaying)
                SceneLoadHandle = LoadChildScene;
            else
                SceneLoadHandle = OpenChildSceneInEditor;

#else
                SceneLoadHandle = LoadChildScene;
#endif
        }

        protected IEnumerator Start()
        {
            if (childScenes == null)
                yield break;

            foreach (var scene in childScenes)
            {
                if (string.IsNullOrEmpty(scene.sceneName))
                    continue;

                yield return SceneLoadHandle(scene.sceneName, scene.async, scene.autoActived);
            }

            OnSceneReady?.Invoke();
        }

#if UNITY_EDITOR

        protected virtual IEnumerator OpenChildSceneInEditor(string sceneName, bool async = true, bool autoActived = true)
        {
            var scenePath = FindScenePathInEditorBuild(sceneName);

            if (string.IsNullOrEmpty(scenePath))
            {
                Debug.LogError($"场景未找到.  scene name:{sceneName}");

                yield break;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, EditorOpenSceneMode.Additive);

            if (autoActived)
                EditorSceneManager.SetActiveScene(scene);
        }

        static string FindScenePathInEditorBuild(string sceneName)
        {
            var sceneFile = $"{sceneName}.unity";

            var res = Array.Find(EditorBuildSettings.scenes, (buildScene) => {
                return buildScene.path.EndsWith(sceneFile);
            });

            return res?.path;
        }
 
#endif

        /// <summary>
        /// 非编辑器下加载子场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        /// <param name="async">是否异步</param>
        /// <param name="autoActived">是否自动激活</param>
        /// <returns></returns>
        public virtual IEnumerator LoadChildScene(string sceneName, bool async = true, bool autoActived = true)
        {
            var existScene = SceneManager.GetSceneByName(sceneName);

            if (existScene.IsValid())
            {
                yield break;
            }

            if (async)
            {
                var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                yield return asyncOperation;
            }
            else
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

                yield return null;
            }

            if (autoActived)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            }
        }
    }
}