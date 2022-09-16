// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

#if dUI_TextMeshPro
using TMPro;
#endif

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Themes
{
    /// <summary> Structure that stores a TextMeshPro FontAsset and Guid pair </summary>
    [Serializable]
    public struct FontAssetId : ISerializationCallbackReceiver
    {
        #region Private Variables

#if dUI_TextMeshPro
        /// <summary> FontAsset reference </summary>
        [SerializeField]
        private TMP_FontAsset m_fontAsset;
#endif
        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this FontAssetId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } set { m_id = value; } }
        
#if dUI_TextMeshPro
        /// <summary> FontAsset reference of this struct </summary>
        public TMP_FontAsset FontAsset { get { return m_fontAsset; } set { m_fontAsset = value; } }
#endif

        #endregion

        #region Constructors

#if dUI_TextMeshPro
        /// <summary> Construct a new FontAssetId with the given FontAsset reference and a unique Id </summary>
        /// <param name="fontAsset"> FontAsset reference </param>
        public FontAssetId(TMP_FontAsset fontAsset) : this()
        {
            m_id = Guid.NewGuid();
            m_fontAsset = fontAsset;
        }

        /// <summary> Construct a new FontAssetId with the given Id and FontAsset reference </summary>
        /// <param name="id"> Guid value </param>        
        /// <param name="fontAsset"> FontAsset reference </param>
        public FontAssetId(Guid id, TMP_FontAsset fontAsset) : this()
        {
            m_id = id;
            m_fontAsset = fontAsset;
        }
#endif

        #endregion

        #region Unity Methods

        public void OnBeforeSerialize() { SerializedGuid = GuidUtils.GuidToSerializedGuid(m_id); }

        public void OnAfterDeserialize() { m_id = GuidUtils.SerializedGuidToGuid(SerializedGuid); }

        #endregion

        #region Public Methods

        /// <summary> Set the Id value </summary>
        /// <param name="newGuid"> New Guid value </param>
        public void SetId(Guid newGuid) { m_id = newGuid; }

#if dUI_TextMeshPro
        /// <summary> Set the FontAsset reference </summary>
        /// <param name="fontAsset"> New FontAsset reference </param>
        public void SetFontAsset(TMP_FontAsset fontAsset) { m_fontAsset = fontAsset; }
#endif

        #endregion
    }
}