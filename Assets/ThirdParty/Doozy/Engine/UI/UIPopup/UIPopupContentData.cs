// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Engine.UI
{
    /// <summary>
    /// Contains data used to customize content elements of an UIPopup
    /// </summary>
    [Serializable]
    public class UIPopupContentData
    {
        #region Public Variables
        
        /// <summary> List of UnityActions. Used for callbacks </summary>
        public List<UnityAction> ButtonCallbacks = new List<UnityAction>();
        /// <summary> List of string button labels. Used for the UIButton label text value </summary>
        public List<string> ButtonLabels = new List<string>();
        /// <summary> List of string button names. Used for the UIButton button name value </summary>
        public List<string> ButtonNames = new List<string>();
        /// <summary> List of string labels. Used for any Text or TextMeshPro text values </summary>
        public List<string> Labels = new List<string>();
        /// <summary> List of sprites. Used for any Image sprite value </summary>
        public List<Sprite> Sprites = new List<Sprite>();
        
        #endregion
    }
}