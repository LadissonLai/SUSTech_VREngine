// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary> Contains a pair of PopupName and UIPopup prefab reference, used by the UIPopupDatabase as its data entries </summary>
    [Serializable]
    public class UIPopupLink : ScriptableObject
    {
        /// <summary> Name used to retrieve the prefab reference from the database (this is not the prefab name) </summary>
        public string PopupName;

        /// <summary> UIPopup prefab reference </summary>
        public GameObject Prefab;
        
        /// <summary> [Editor Only] Marks target object as dirty. (Only suitable for non-scene objects) </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        public void SetDirty(bool saveAssets) { DoozyUtils.SetDirty(this, saveAssets); }
        
        public void UpdateAssetName(bool saveAsset)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), "UIPopupLink_" + PopupName);
            SetDirty(saveAsset);
#endif
        }
    }
}