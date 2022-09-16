// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Base;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Contains settings and 'actions' used and triggered by the UIDrawer when it changes its state
    /// </summary>
    [Serializable]
    public class UIDrawerBehavior
    {
        #region Properties

        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have at least one AnimatorEvent in their AnimatorEvents list </summary>
        public bool HasAnimatorEvents { get { return OnStart.HasAnimatorEvents || OnFinished.HasAnimatorEvents; } }

        /// <summary> Returns TRUE if either OnStart UIAction Effect (UIEffect) or OnFinished UIAction Effect (UIEffect) have a target ParticleSystem referenced </summary>
        public bool HasEffect { get { return OnStart.HasEffect || OnFinished.HasEffect; } }
        
        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have has at least one game event in their GameEvents list </summary>
        public bool HasGameEvents { get { return OnStart.HasGameEvents || OnFinished.HasGameEvents; } }
        
        /// <summary> Returns TRUE if either OnStart UIAction or OnFinished UIAction have valid sound settings </summary>
        public bool HasSound { get { return OnStart.HasSound || OnFinished.HasSound; } }

        /// <summary> Returns TRUE if either OnStart UIAction Event (UnityEvent) or OnFinished UIAction Event (UnityEvent) have at least one registered persistent listener </summary>
        public bool HasUnityEvents { get { return OnStart.HasUnityEvent || OnFinished.HasUnityEvent; } }

        #endregion

        #region Public Variables

        // ReSharper disable once NotAccessedField.Global
        /// <summary> Determines what type of animation is enabled on this behavior </summary>
        public UIDrawerBehaviorType DrawerBehaviorType;

        /// <summary> Actions performed when the behavior finished </summary>
        public UIAction OnFinished = new UIAction();

        /// <summary> Actions performed when the behavior starts </summary>
        public UIAction OnStart = new UIAction();

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        /// <param name="behaviorType"> Behavior type </param>
        public UIDrawerBehavior(UIDrawerBehaviorType behaviorType)
        {
            Reset(behaviorType);
        }

        #endregion

        #region Public Methods

        /// <summary> Resets this instance to the default values </summary>
        /// <param name="behaviorType"> Behavior type </param>
        public void Reset(UIDrawerBehaviorType behaviorType)
        {
            DrawerBehaviorType = behaviorType;
            OnStart = new UIAction();
            OnFinished = new UIAction();
        }

        #endregion
    }
}