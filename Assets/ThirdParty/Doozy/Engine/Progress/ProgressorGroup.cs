// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine.Settings;
using Doozy.Engine.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ReSharper disable UnusedMember.Global
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Local

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Calculates the arithmetic mean of all the progress values of all the referenced Progressors.
    /// Progressor Groups are useful if you need the mean progress value of two or more progress values.
    /// E.g. Progressor Group (PG) has two Progressors (P1 and P2). If P1 progress = 1f and P2 progress = 0f then PG progress = 0.5f.
    /// </summary>
    [AddComponentMenu(MenuUtils.ProgressorGroup_AddComponentMenu_MenuName, MenuUtils.ProgressorGroup_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESSOR_GROUP)]
    public class ProgressorGroup : MonoBehaviour
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressorGroup_MenuItem_ItemName, false, MenuUtils.ProgressorGroup_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { AddToScene(true); }
#endif

        #endregion

        #region Constants

        /// <summary> Tolerance used for float comparisons </summary>
        public const float TOLERANCE = 0.001f;

        #endregion

        #region Public Variables

        /// <summary> Enables relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        /// <summary> Progressor sources that this ProgressorGroup listens for, to calculate the arithmetic mean of all their progress values </summary>
        public List<Progressor> Progressors;

        /// <summary>
        ///     Callback executed when the Progress value has been updated.
        ///     <para />
        ///     Passes the Progress value (float between 0 and 1)
        /// </summary>
        public ProgressEvent OnProgressChanged = new ProgressEvent();

        /// <summary>
        ///     Callback executed when the Progress value has been updated.
        ///     <para />
        ///     Passes the InverseProgress value (float between 1 and 0)
        ///     <para />
        ///     InverseProgress = 1 - Progress
        /// </summary>
        public ProgressEvent OnInverseProgressChanged = new ProgressEvent();

        #endregion

        #region Properties

        /// <summary> Returns the current progress value (float between 0 and 1) </summary>
        public float Progress
        {
            get { return m_progress; }
            private set
            {
                float safeValue = Mathf.Clamp01(value);
                m_progress = safeValue;
                OnProgressUpdated();
            }
        }

        /// <summary> Returns the inverse value of current Progress value (float between 1 and 0) </summary>
        public float InverseProgress { get { return 1 - Progress; } }

        private bool DebugComponent { get { return DebugMode || DoozySettings.Instance.DebugProgressorGroup; } }
        
        #endregion

        #region Private Variables

        /// <summary> Internal variable used to hold a reference to the sequence used to animate the current progress value, when AnimateProgress is set to TRUE </summary>
        private Sequence m_animationSequence;

        /// <summary> Internal variable that holds the previous progress value </summary>
        private float m_previousProgress;

        /// <summary> Internal variable used to hold the progress value </summary>
        private float m_progress;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            RemoveAnyNullProgressors();
            UpdateProgress();
        }

        private void OnDisable()
        {
            RemoveAnyNullProgressors();
        }

        private void Update()
        {
            UpdateProgress();
            if (m_previousProgress == Progress) return;
            OnProgressUpdated();
            m_previousProgress = Progress;
        }

        #endregion

        #region Public Methods

        /// <summary> Update the progress value by checking all the Progressors Progress values </summary>
        public void UpdateProgress()
        {
            if (Progressors == null || Progressors.Count == 0) return;

            float totalProgress = 0;
            int progressorCount = 0;
            
            for (int i = Progressors.Count - 1; i >= 0; i--)
            {
                if (Progressors[i] == null) continue;
                progressorCount++;
                totalProgress += Progressors[i].Progress;
            }

            float newProgressValue = progressorCount == 0 ? 0 : Mathf.Clamp01(totalProgress / progressorCount);
            if (newProgressValue < TOLERANCE) newProgressValue = 0;
            if (newProgressValue > 1 - TOLERANCE) newProgressValue = 1;
            Progress = newProgressValue;
        }
        
        /// <summary> Get either the Progress or the InverseProgress value, depending on the requested direction </summary>
        /// <param name="direction"> Progress Direction to return </param>
        public float GetProgress(TargetProgress direction)
        {
            switch (direction)
            {
                case TargetProgress.Progress:   return Progress;
                case TargetProgress.InverseProgress: return InverseProgress;
                default:                      throw new ArgumentOutOfRangeException("direction", direction, null);
            }
        }

        #endregion

        #region Private Methods

        /// <summary> Iterates through the Progressors list and removes any null references </summary>
        private void RemoveAnyNullProgressors()
        {
            if (Progressors == null) Progressors = new List<Progressor>();
            Progressors = Progressors.Where(p => p != null).ToList();
            UpdateProgress();
        }

        /// <summary> Invokes the OnProgressChanged and OnInverseProgressChanged events </summary>
        private void OnProgressUpdated()
        {
            if (DebugComponent) DDebug.Log( "[" + name + "] Progress: " + Progress + " / Inverse Progress: " + InverseProgress, this);
            OnProgressChanged.Invoke(Progress);
            OnInverseProgressChanged.Invoke(InverseProgress);
        }

        #endregion

        #region Static Mehtods

        /// <summary> Adds ProgressorGroup to scene and returns a reference to it </summary>
        private static ProgressorGroup AddToScene(bool selectGameObjectAfterCreation = false) { return DoozyUtils.AddToScene<ProgressorGroup>(MenuUtils.ProgressorGroup_GameObject_Name, false, selectGameObjectAfterCreation); }

        #endregion
    }
}