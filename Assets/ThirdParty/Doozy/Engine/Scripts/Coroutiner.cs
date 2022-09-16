// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable Unity.IncorrectMethodSignature
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary> Special class used to run Coroutines on. When using any of its public static methods, it will instantiate itself and run any number of coroutines </summary>
    /// <seealso cref="T:UnityEngine.MonoBehaviour" />
    public class Coroutiner : MonoBehaviour
    {
        #region Static Properties

        private static Coroutiner s_instance;

        /// <summary> Returns a reference to the Coroutiner in the Scene. If one does not exist, it gets created </summary>
        public static Coroutiner Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                if (ApplicationIsQuitting) return null;
                s_instance = FindObjectOfType<Coroutiner>();
                if (s_instance == null) s_instance = new GameObject("Coroutiner", typeof(Coroutiner)).GetComponent<Coroutiner>();
                return s_instance;
            }
        }

        #endregion

        #region Static Properties

        /// <summary> Internal variable used as a flag when the application is quitting </summary>
        public static bool ApplicationIsQuitting { get; private set; }

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
//                DDebug.Log("There cannot be two " + typeof(Coroutiner) + "' active at the same time. Destroying this one!");
                Destroy(gameObject);
                return;
            }

            s_instance = this;
//            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit() { ApplicationIsQuitting = true; }

        #endregion

        #region Public Methods

        /// <summary> Starts a Coroutine and returns a reference to it </summary>
        /// <param name="enumerator"> The enumerator </param>
        public Coroutine StartLocalCoroutine(IEnumerator enumerator) { return StartCoroutine(enumerator); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="coroutine"> The coroutine </param>
        public void StopLocalCoroutine(Coroutine coroutine) { StopCoroutine(coroutine); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="enumerator"> The enumerator </param>
        public void StopLocalCoroutine(IEnumerator enumerator) { StopCoroutine(enumerator); }

        /// <summary> Stops all Coroutines running on this behaviour </summary>
        public void StopAllLocalCoroutines() { StopAllCoroutines(); }

        #endregion

        #region Static Methods

        /// <summary> Starts a Coroutine and returns a reference to it </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static Coroutine Start(IEnumerator enumerator)
        {
            return Instance != null && enumerator != null 
                       ? Instance.StartLocalCoroutine(enumerator) 
                       : null;
        }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static void Stop(IEnumerator enumerator)
        {
            if (Instance == null || enumerator == null) return;
            Instance.StopLocalCoroutine(enumerator);
        }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="coroutine"> The coroutine </param>
        public static void Stop(Coroutine coroutine)
        {
            if (Instance == null || coroutine == null) return;
            Instance.StopLocalCoroutine(coroutine);
        }

        /// <summary> Stops all Coroutines running on this behaviour </summary>
        public static void StopAll()
        {
            if (Instance == null) return;
            Instance.StopAllLocalCoroutines();
        }

        #endregion
    }
}