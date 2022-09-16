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
    /// <summary> Structure that stores a Font reference and Guid pair </summary>
    [Serializable]
    public struct FontId : ISerializationCallbackReceiver
    {
        #region Private Variables

        /// <summary> Font reference </summary>
        [SerializeField]
        private Font m_font;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this FontId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } set { m_id = value; } }
        
        /// <summary> Font reference of this struct </summary>
        public Font Font { get { return m_font; } set { m_font = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new FontId with the given Font reference and a unique Id </summary>
        /// <param name="font"> Font reference </param>
        public FontId(Font font) : this()
        {
            m_id = Guid.NewGuid();
            m_font = font;
        }

        /// <summary> Construct a new FontId with the given Id and Font reference </summary>
        /// <param name="id"> Guid value </param>
        /// <param name="font"> Font reference </param>
        public FontId(Guid id, Font font) : this()
        {
            m_id = id;
            m_font = font;
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

        /// <summary> Set the Font reference </summary>
        /// <param name="font"> New Font reference </param>
        public void SetFont(Font font) { m_font = font; }

        #endregion
    }
}