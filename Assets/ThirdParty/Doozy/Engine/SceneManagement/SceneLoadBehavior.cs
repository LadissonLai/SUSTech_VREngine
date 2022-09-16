// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.SceneManagement
{
    /// <summary>
    ///     Contains settings and 'actions' used and triggered by the SceneLoader when loading a scene
    /// </summary>>
    [Serializable]
    public class SceneLoadBehavior
    {
        #region Properties

        /// <summary> Returns TRUE if either OnLoadScene UIAction or OnSceneLoaded UIAction have at least one AnimatorEvent in their AnimatorEvents list </summary>
        public bool HasAnimatorEvents { get { return OnLoadScene.HasAnimatorEvents || OnSceneLoaded.HasAnimatorEvents; } }

        /// <summary> Returns TRUE if either OnLoadScene UIAction Effect (UIEffect) or OnSceneLoaded UIAction Effect (UIEffect) have a target ParticleSystem referenced </summary>
        public bool HasEffect { get { return OnLoadScene.HasEffect || OnSceneLoaded.HasEffect; } }
       
        /// <summary> Returns TRUE if either OnLoadScene UIAction or OnSceneLoaded UIAction have at least one game event in their GameEvents list </summary>
        public bool HasGameEvents { get { return OnLoadScene.HasGameEvents || OnSceneLoaded.HasGameEvents; } }
        
        /// <summary> Returns TRUE if either OnLoadScene UIAction or OnSceneLoaded UIAction have valid sound settings </summary>
        public bool HasSound { get { return OnLoadScene.HasSound || OnSceneLoaded.HasSound; } }

        /// <summary> Returns TRUE if either OnLoadScene UIAction Event (UnityEvent) or OnSceneLoaded UIAction Event (UnityEvent) have at least one registered persistent listener </summary>
        public bool HasUnityEvents { get { return OnLoadScene.HasUnityEvent || OnSceneLoaded.HasUnityEvent; } }

        #endregion
        
        #region Public Variables

        /// <summary> Actions performed when a scene started loading </summary>
        public UIAction OnLoadScene = new UIAction();

        /// <summary> Actions performed when the scene has been loaded (the progress is at 0.9 (90%)) and has not been activated yet (the reset 0.1 (10%)).
        /// <para/> When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        /// <para/> This action is triggered after the scene has been loaded and before its activation (at 90% load progress)
        /// </summary>
        public UIAction OnSceneLoaded = new UIAction();
        
        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        public SceneLoadBehavior() { Reset(); }

        #endregion

        #region Public Methods
        
        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            OnLoadScene = new UIAction();
            OnSceneLoaded = new UIAction();
        }
        
        #endregion
    }
}