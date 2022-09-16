// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable NotAccessedField.Global

namespace Doozy.Engine.UI.Animation
{
    /// <inheritdoc />
    /// <summary>
    ///     Data container, for an UIAnimation, used for saving to and loading from of animation presets
    /// </summary>
    [Serializable]
    public class UIAnimationData : ScriptableObject
    {
        #region Public Variables

        /// <summary> Animation settings </summary>
        public UIAnimation Animation;

        /// <summary> The UIAnimationDatabase name this UIAnimationData belongs to </summary>
        public string Category;

        /// <summary> Animation name as defined in the UIAnimationDatabase </summary>
        public string Name;

        #endregion
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }
    }
}