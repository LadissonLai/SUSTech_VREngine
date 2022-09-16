// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.Themes
{
    /// <summary> Base class for all the theme targets. Used by the ThemeManager to update a targets when the active variant changes </summary>
    [Serializable]
    public abstract class ThemeTarget : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Public Variables

        /// <summary> System.Guid unique id for the target theme </summary>
        public Guid ThemeId = Guid.Empty;

        /// <summary> System.Guid unique id for the theme variant </summary>
        public Guid VariantId = Guid.Empty;

        /// <summary> System.Guid unique id for the variant property </summary>
        public Guid PropertyId = Guid.Empty;

        #endregion

        #region Private Variables

        /// <summary> Vector array describing the System.Guid for the target theme </summary>
        [SerializeField]
        private byte[] ThemeIdSerializedGuid;

        /// <summary> Vector array describing the System.Guid for the theme variant </summary>
        [SerializeField]
        private byte[] VariantIdSerializedGuid;

        /// <summary> Vector array describing the System.Guid for the variant property </summary>
        [SerializeField]
        private byte[] PropertyIdSerializedGuid;

        #endregion

        #region Unity Methods

        protected virtual void OnValidate()
        {
            if (ThemeId == Guid.Empty) return;
            if (PropertyId == Guid.Empty) return;
            UpdateTarget(ThemeManager.Database.GetThemeData(ThemeId));
        }

        public virtual void Awake() { }

        public virtual void OnEnable() { ThemeManager.RegisterTarget(this); }

        public virtual void OnDisable() { ThemeManager.UnregisterTarget(this); }

        public virtual void OnBeforeSerialize()
        {
            ThemeIdSerializedGuid = GuidUtils.GuidToSerializedGuid(ThemeId);
            VariantIdSerializedGuid = GuidUtils.GuidToSerializedGuid(VariantId);
            PropertyIdSerializedGuid = GuidUtils.GuidToSerializedGuid(PropertyId);
        }

        public virtual void OnAfterDeserialize()
        {
            ThemeId = GuidUtils.SerializedGuidToGuid(ThemeIdSerializedGuid);
            VariantId = GuidUtils.SerializedGuidToGuid(VariantIdSerializedGuid);
            PropertyId = GuidUtils.SerializedGuidToGuid(PropertyIdSerializedGuid);
        }

        #endregion

        #region Public Methods

        /// <summary> Method used by the ThemeManager when the active variant or selected theme have changed </summary>
        /// <param name="theme"> Target theme </param>
        public virtual void UpdateTarget(ThemeData theme) { }

        #endregion
    }
}