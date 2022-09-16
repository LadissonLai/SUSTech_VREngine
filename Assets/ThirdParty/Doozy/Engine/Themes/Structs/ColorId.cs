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
    /// <summary> Structure that stores a Color and Guid pair </summary>
    [Serializable]
    public struct ColorId : ISerializationCallbackReceiver
    {
        #region Private Variables

        /// <summary> Color data </summary>
        [SerializeField]
        private Color m_color;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this ColorId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } set { m_id = value; } }

        /// <summary> Color of this struct </summary>
        public Color Color { get { return m_color; } set { m_color = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new ColorId with the given Color and a unique Id </summary>
        /// <param name="color"> Color value </param>
        public ColorId(Color color) : this()
        {
            m_id = Guid.NewGuid();
            m_color = color;
        }

        /// <summary> Construct a new ColorId with the given Id and Color value </summary>
        /// <param name="id"> Guid value </param>
        /// <param name="color"> Color value </param>
        public ColorId(Guid id, Color color) : this()
        {
            m_id = id;
            m_color = color;
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

        /// <summary> Set the Color value </summary>
        /// <param name="color"> New Color value </param>
        public void SetColor(Color color) { m_color = color; }

        #endregion
    }
}