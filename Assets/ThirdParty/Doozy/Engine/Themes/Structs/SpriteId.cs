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
    /// <summary> Structure that stores a Sprite reference and Guid pair </summary>
    [Serializable]
    public struct SpriteId : ISerializationCallbackReceiver
    {
        #region Private Variables

        /// <summary> Sprite reference </summary>
        [SerializeField]
        private Sprite m_sprite;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this SpriteId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Sprite reference of this struct </summary>
        public Sprite Sprite { get { return m_sprite; } set { m_sprite = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new SpriteId with the given Sprite reference and a unique Id </summary>
        /// <param name="sprite"> Sprite reference </param>
        public SpriteId(Sprite sprite) : this()
        {
            m_id = Guid.NewGuid();
            m_sprite = sprite;
        }

        /// <summary> Construct a new SpriteId with the given Id and Sprite reference </summary>
        /// <param name="id"> Guid value </param>
        /// <param name="sprite"> Sprite reference </param>
        public SpriteId(Guid id, Sprite sprite) : this()
        {
            m_id = id;
            m_sprite = sprite;
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

        /// <summary> Set the Sprite reference </summary>
        /// <param name="sprite"> Sprite reference </param>
        public void SetSprite(Sprite sprite) { m_sprite = sprite; }

        #endregion
    }
}