// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Themes
{
    /// <summary> Structure that stores a Texture reference and Guid pair </summary>
    [Serializable]
    public struct TextureId : ISerializationCallbackReceiver
    {
        #region Private Variables
        
        /// <summary> Texture reference </summary>
        [SerializeField]
        private Texture m_texture;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this TextureId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } set { m_id = value; } }
        
        /// <summary> Texture reference of this struct </summary>
        public Texture Texture { get { return m_texture; } set { m_texture = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new TextureId with the given Texture reference and a unique Id </summary>
        /// <param name="texture"> Texture reference </param>
        public TextureId(Texture texture) : this()
        {
            m_id = Guid.NewGuid();
            m_texture = texture;
        }

        /// <summary> Construct a new TextureId with the given Id and Texture reference </summary>
        /// <param name="id"> Guid value </param>
        /// <param name="texture"> Texture reference </param>
        public TextureId(Guid id, Texture texture) : this()
        {
            m_id = id;
            m_texture = texture;
        }

        #endregion

        #region Unity Methods

        public void OnBeforeSerialize() { SerializedGuid = GuidUtils.GuidToSerializedGuid(m_id); }

        public void OnAfterDeserialize() { m_id = GuidUtils.SerializedGuidToGuid(SerializedGuid); }
        
        #endregion

        #region Public Methods

        /// <summary> Set the Id value </summary>
        /// <param name="newGuid"> New Guid value </param>
        public void SetId(Guid newGuid) { m_id = newGuid; }

        /// <summary> Set the Texture reference </summary>
        /// <param name="texture"> Texture reference </param>
        public void SetTexture(Texture texture) { m_texture = texture; }

        #endregion
        

    }
}