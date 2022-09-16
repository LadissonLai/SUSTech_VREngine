// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//https://johnleonardfrench.com/articles/the-right-way-to-make-a-volume-slider-in-unity-using-logarithmic-conversion/

using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to update the value of an AudioMixer exposed parameter
    /// </summary>
    [AddComponentMenu(MenuUtils.ProgressTargetAudioMixer_AddComponentMenu_MenuName, MenuUtils.ProgressTargetAudioMixer_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_AUDIOMIXERGROUP)]
    public class ProgressTargetAudioMixer : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetAudioMixer_MenuItem_ItemName, false, MenuUtils.ProgressTargetAudioMixer_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetAudioMixer>(MenuUtils.ProgressTargetAudioMixer_GameObject_Name, false, true); }
#endif

        #endregion

        #region Constants

        private const float MIN_VALUE = 0.0001f;
        private const float MAX_VALUE = 1f;

        #endregion

        #region Public Variables

        /// <summary> Name of exposed parameter in the target AudioMixer </summary>
        public string ExposedParameterName;

        /// <summary> Target AudioMixer </summary>
        public AudioMixer TargetMixer;

        /// <summary>
        /// Lower the sensitivity of the slider by using a logarithmic conversion.
        /// <para/> Should be TRUE if, for example, setting the volume level (the attenuation) for a AudioMixerGroup.
        /// <para/> If TRUE the progressor. Progress value will be used (converted to its logarithmic value), if FALSE progressor.Value value will be used (as is).
        /// </summary>
        public bool UseLogarithmicConversion = true;
        
        #endregion

        #region Public Methods

        /// <inheritdoc />
        /// <summary> Method used by a Progressor to update the target exposed parameter when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update </param>
        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);

            if (TargetMixer == null) return;
            TargetMixer.SetFloat(ExposedParameterName, UseLogarithmicConversion ? GetLogarithmicValue(progressor.Progress) : progressor.Value);
        }

        #endregion

        #region Private Methods

        private static float GetLogarithmicValue(float value) { return Mathf.Log10(Mathf.Clamp(value, MIN_VALUE, MAX_VALUE)) * 20; }

        #endregion        
    }
}