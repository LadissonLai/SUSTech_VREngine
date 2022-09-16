// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to update the fillAmount value of an Image component
    /// </summary>
    [AddComponentMenu(MenuUtils.ProgressTargetImage_AddComponentMenu_MenuName, MenuUtils.ProgressTargetImage_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_IMAGE)]
    public class ProgressTargetImage : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetImage_MenuItem_ItemName, false, MenuUtils.ProgressTargetImage_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetImage>(MenuUtils.ProgressTargetImage_GameObject_Name, false, true); }
#endif

        #endregion
        
        #region Public Variables
        
        /// <summary> Target Image component </summary>
        public Image Image;
        
        /// <summary> Progress direction to be used (Progress or InverseProgress) </summary>
        public TargetProgress TargetProgress;
        
        #endregion
        
        #region Public Methods
        
        /// <inheritdoc />
        /// <summary> Method used by a Progressor to when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update </param>
        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);
            
            if(Image == null) return;
            Image.fillAmount = TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress;
        }
        
        #endregion

        #region Private Methods

        private void Reset() { UpdateReference(); }

        private void UpdateReference()
        {
            if (Image == null)
                Image = GetComponent<Image>();
        }
        
        #endregion
    }
}