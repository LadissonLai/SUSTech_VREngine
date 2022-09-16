// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to update a float parameter value of an Animator component
    /// </summary>
    [AddComponentMenu(MenuUtils.ProgressTargetAnimator_AddComponentMenu_MenuName, MenuUtils.ProgressTargetAnimator_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_ANIMATOR)]
    public class ProgressTargetAnimator : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetAnimator_MenuItem_ItemName, false, MenuUtils.ProgressTargetAnimator_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetAnimator>(MenuUtils.ProgressTargetAnimator_GameObject_Name, false, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> Target Animator component </summary>
        public Animator Animator;

        /// <summary>
        /// Target parameter name (defined in the Animator as a float parameter)
        /// This parameter needs to be selected as the 'Normalized Time' parameter in the target animation.
        /// </summary>
        public string ParameterName = "Progress";

        /// <summary> Progress direction to be used (Progress or InverseProgress) </summary>
        public TargetProgress TargetProgress = TargetProgress.Progress;

        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <summary> Method used by a Progressor to when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update </param>
        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);
            if (Animator == null) return;
            if (!Animator.gameObject.activeSelf) return;
            if (!Animator.isActiveAndEnabled) return;
            Animator.SetFloat(ParameterName, TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress);
        }

        #endregion

        #region Private Methods

        private void Reset() { UpdateReference(); }

        private void UpdateReference()
        {
            if(Animator == null)
                Animator = GetComponent<Animator>();
        }
        
        #endregion
    }
}