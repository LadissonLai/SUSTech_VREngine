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
    /// <summary> Structure that stores a string (Label) and Guid pair </summary>
    [Serializable]
    public struct LabelId : ISerializationCallbackReceiver
    {
        #region Private Variables

        /// <summary> Label value </summary>
        [SerializeField]
        private string m_label;

        /// <summary> Vector array describing a System.Guid </summary>
        [SerializeField]
        private byte[] SerializedGuid;

        /// <summary> System.Guid unique id for this LabelId </summary>
        [SerializeField]
        private Guid m_id;

        #endregion

        #region Properties

        /// <summary> Id of this struct </summary>
        public Guid Id { get { return m_id; } }
        
        /// <summary> Label of this struct </summary>
        public string Label { get { return m_label; } set { m_label = value; } }

        #endregion

        #region Constructors

        /// <summary> Construct a new LabelId with the given label and a unique Id </summary>
        /// <param name="label"> Label value </param>
        public LabelId(string label) : this()
        {
            m_id = Guid.NewGuid();
            m_label = label;
        }

        /// <summary> Construct a new LabelId with the given Id and label values </summary>
        /// <param name="guid"> Guid value </param>
        /// <param name="label"> Label value </param>
        public LabelId(Guid guid, string label) : this()
        {
            m_id = guid;
            m_label = label;
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

        /// <summary> Set the label value </summary>
        /// <param name="label"> New label value </param>
        public void SetLabel(string label) { m_label = label; }

        #endregion
    }
}