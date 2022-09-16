// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEngine.UI;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Layouts
{
    /// <summary> Special component used to rebuild and update LayoutGroups on demand </summary>
    public class LayoutController : MonoBehaviour
    {
        #region Constants

        private const bool DEFAULT_NEEDS_REBUILD = true;

        #endregion

        #region Public Variables

        /// <summary> Marks the controller to trigger rebuild when the next Update runs </summary>
        public bool NeedsRebuild = DEFAULT_NEEDS_REBUILD;

        #endregion

        #region Properties

        /// <summary> Reference to the target LayoutGroup managed by this controller </summary>
        public LayoutGroup Layout
        {
            get
            {
                if (m_layoutGroup != null) return m_layoutGroup;
                UpdateReference();
                m_rectTransform = null;
                return m_layoutGroup;
            }
            set
            {
                m_layoutGroup = value;
                m_rectTransform = null;
            }
        }


        /// <summary> Reference to the RectTransform component of the target LayoutGroup </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform != null) return m_rectTransform;
                if (Layout == null) return null;
                m_rectTransform = Layout.GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        #endregion

        #region Private Variables

        /// <summary> Internal variable that keeps track of when the last Rebuild was triggered </summary>
        private float m_lastRebuildTime;

        /// <summary> Internal variable that holds a reference to the target LayoutGroup </summary>
        private LayoutGroup m_layoutGroup;

        /// <summary> Internal variable that holds a reference to the RectTransform component of the target LayoutGroup </summary>
        private RectTransform m_rectTransform;

        #endregion

        #region Unity Methods

        private void Reset()
        {
            UpdateReference();
            NeedsRebuild = DEFAULT_NEEDS_REBUILD;
        }

        private void Awake()
        {
            enabled = RectTransform != null;
            NeedsRebuild = DEFAULT_NEEDS_REBUILD;
            Rebuild();
        }

        private void Update()
        {
            Rebuild();
        }

        #endregion

        #region Public Methods

        /// <summary> Disables the target LayoutGroup </summary>
        public void DisableLayoutGroup()
        {
            if(Layout == null) return;
            Layout.enabled = false;
        }

        /// <summary> Enables the target LayoutGroup </summary>
        public void EnableLayoutGroup()
        {
            if(Layout == null) return;
            Layout.enabled = true;
        }
        
        /// <summary> Triggers an immediate rebuild of the target LayoutGroup </summary>
        /// <param name="forced"> Forces the rebuild regardless if NeedRebuild if FALSE or if a Rebuild was already triggered this frame </param>
        public void Rebuild(bool forced = false)
        {
            if (RectTransform == null) return;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (NeedsRebuild && m_lastRebuildTime == Time.unscaledTime) NeedsRebuild = false;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!forced && (!NeedsRebuild || m_lastRebuildTime != Time.unscaledTime)) return;
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
            NeedsRebuild = false;
            m_lastRebuildTime = Time.unscaledTime;
        }

        #endregion

        #region Private Methods

        private void UpdateReference()
        {
            if (m_layoutGroup == null)
                m_layoutGroup = GetComponent<LayoutGroup>();
        }

        #endregion
    }
}