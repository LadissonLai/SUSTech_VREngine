// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Base
{
    /// <summary>
    ///     Manages a target ParticleSystem by dynamically changing its sorting layer and sorting order, in relation to any Canvas component, and also plays or stops the effect when invoked
    /// </summary>
    [Serializable]
    public class UIEffect
    {
        #region Constants

        public const DynamicSorting DEFAULT_AUTO_SORT = DynamicSorting.InFront;
        public const UIEffectBehavior DEFAULT_BEHAVIOR = UIEffectBehavior.Play;
        public const int DEFAULT_SORTING_ORDER = 0;
        public const int DEFAULT_SORTING_STEPS = 1;
        public const ParticleSystemStopBehavior DEFAULT_STOP_BEHAVIOR = ParticleSystemStopBehavior.StopEmitting;
        public const string DEFAULT_SORTING_LAYER = "Default";

        #endregion

        #region Properties

        /// <summary> Returns a reference to the MainModule of the target ParticleSystem </summary>
        public ParticleSystem.MainModule MainModule { get { return ParticleSystem.main; } }

        /// <summary> Returns a reference to the Renderer component of the target ParticleSystem </summary>
        public Renderer[] Renderers
        {
            get
            {
                if (m_renderers != null) return m_renderers;
                m_renderers = ParticleSystem.GetComponentsInChildren<Renderer>();
                return m_renderers;
            }
        }

        /// <summary> Returns the SortingLayer name of the target ParticleSystem Renderer </summary>
        public string SortingLayerName { get { return Renderers[0].sortingLayerName; } }

        /// <summary> Returns the sorting order of the target ParticleSystem Renderer </summary>
        public int SortingOrder { get { return Renderers[0].sortingOrder; } }

        #endregion

        #region Public Variables

        /// <summary> Determines if the effect should get automatically sorted or not </summary>
        public DynamicSorting AutoSort = DEFAULT_AUTO_SORT;

        /// <summary> Determines if, when invoked,  this UIEffect will Play or Stop its target ParticleSystem </summary>
        public UIEffectBehavior Behavior = DEFAULT_BEHAVIOR;

        /// <summary> Custom sorting order value used if AutoSort is set to DynamicSorting.Custom </summary>
        public int CustomSortingOrder = DEFAULT_SORTING_ORDER;

        /// <summary> Number of sorting steps that will get added (if AutoSort is set to DynamicSorting.InFront) or subtracted (if AutoSort is set to DynamicSorting.Behind) </summary>
        public int SortingSteps = DEFAULT_SORTING_STEPS;

        /// <summary> Target ParticleSystem that this UIEffect manages </summary>
        public ParticleSystem ParticleSystem;

        /// <summary> The behavior to apply when calling Stop on the target ParticleSystem </summary>
        public ParticleSystemStopBehavior StopBehavior = DEFAULT_STOP_BEHAVIOR;

        /// <summary> Custom SortingLayer name used if AutoSort is set to DynamicSorting.Custom </summary>
        public string CustomSortingLayer = DEFAULT_SORTING_LAYER;

        #endregion

        #region Private Variables

        /// <summary>
        ///     Internal variable that holds a reference to the target ParticleSystem renderer
        /// </summary>
        private Renderer[] m_renderers;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        public UIEffect() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary> Removes all existing emitted particles of the target ParticleSystem </summary>
        public void Clear()
        {
            if (ParticleSystem == null) return;
            ParticleSystem.Clear();
        }

        /// <summary> Emit count particles immediately by the target ParticleSystem </summary>
        /// <param name="count"> Number of particles to emit </param>
        public void Emit(int count)
        {
            if (ParticleSystem == null) return;
            ParticleSystem.Emit(count);
        }

        /// <summary> Executes the UIEffect's action (Play or Stop) and does not override ParticleSystem SortingLayer or sorting order </summary>
        public void Execute()
        {
            switch (Behavior)
            {
                case UIEffectBehavior.Play:
                    Play();
                    break;
                case UIEffectBehavior.Stop:
                    Stop(StopBehavior);
                    break;
            }
        }

        /// <summary> Executes the UIEffect's action (Play or Stop) and sets the given sortingLayer and sortingOrder to the target ParticleSystem </summary>
        /// <param name="sortingLayer"> Target sorting layer name </param>
        /// <param name="sortingOrder"> Target order in layer </param>
        public void Execute(string sortingLayer, int sortingOrder)
        {
            switch (Behavior)
            {
                case UIEffectBehavior.Play:
                    Play(sortingLayer, sortingOrder);
                    break;
                case UIEffectBehavior.Stop:
                    Stop(StopBehavior);
                    break;
            }
        }

        /// <summary> Overrides the current sorting layer and sorting order, on the target ParticleSystem, with the passed value and then starts playing the effect </summary>
        /// <param name="overrideSortingLayer"> Sorting layer name that overrides the current set value, regardless of the UIEffect's settings </param>
        /// <param name="overrideSortingOrder"> Sorting order that overrides the current set value, regardless of the UIEffect's settings </param>
        public void OverrideSortingAndPlay(string overrideSortingLayer, int overrideSortingOrder)
        {
            if (ParticleSystem == null) return;
            SetSortingLayer(overrideSortingLayer);
            SetSortingOrder(overrideSortingOrder);
            Play();
        }

        /// <summary> Starts playing the effect after it takes into account if OverrideSorting or AutoSort are enabled and updates the sorting layer name and sorting order of the target ParticleSystem </summary>
        /// <param name="sortingLayer"> Target sorting layer name </param>
        /// <param name="sortingOrder"> Target order in layer </param>
        public void Play(string sortingLayer, int sortingOrder)
        {
            if (ParticleSystem == null) return;
            UpdateSorting(sortingLayer, sortingOrder);
            Play();
        }

        /// <summary> Starts playing the effect with the current setting </summary>
        public void Play()
        {
            if (ParticleSystem == null) return;
            ParticleSystem.Play(true);
        }


        /// <summary> Resets this instance to the default values </summary>
        public void Reset()
        {
            Behavior = DEFAULT_BEHAVIOR;
            StopBehavior = DEFAULT_STOP_BEHAVIOR;
            AutoSort = DEFAULT_AUTO_SORT;
            SortingSteps = DEFAULT_SORTING_STEPS;
            CustomSortingLayer = DEFAULT_SORTING_LAYER;
            CustomSortingOrder = DEFAULT_SORTING_ORDER;
        }

        /// <summary> Updates the SortingLayer of the target ParticleSystem. If the new layerName has not been defined in the SortingLayer list this operation will fail and return false </summary>
        /// <param name="sortingLayerName"> The new sorting layer name (defined in the SortingLayers list) </param>
        public bool SetSortingLayer(string sortingLayerName)
        {
            if (ParticleSystem == null) return false;
            foreach (Renderer renderer in Renderers)
                renderer.sortingLayerName = sortingLayerName;
            return true;
        }

        /// <summary> Updates the order in layer of the target ParticleSystem </summary>
        /// <param name="sortingOrder"></param>
        public void SetSortingOrder(int sortingOrder)
        {
            if (ParticleSystem == null) return;
            foreach (Renderer renderer in Renderers)
                renderer.sortingOrder = sortingOrder;
        }

        /// <summary> Stops the target ParticleSystem from emitting any further particles using the set StopBehavior </summary>
        public void Stop()
        {
            if (ParticleSystem == null) return;
            Stop(StopBehavior);
        }

        /// <summary> Stops the target ParticleSystem from emitting any further particles using the given stopBehavior </summary>
        /// <param name="stopBehavior"> Behavior applied when stopping </param>
        public void Stop(ParticleSystemStopBehavior stopBehavior)
        {
            if (ParticleSystem == null) return;
            ParticleSystem.Stop(true, stopBehavior);
        }


        /// <summary> Updates the target ParticleSystem sorting taking into account the UIEffect's current settings </summary>
        /// <param name="sortingLayer"> Target sorting layer name </param>
        /// <param name="sortingOrder"> Target order in layer </param>
        public void UpdateSorting(string sortingLayer, int sortingOrder)
        {
            if (ParticleSystem == null) return;

            switch (AutoSort)
            {
                case DynamicSorting.InFront:
                    SetSortingLayer(sortingLayer);
                    SetSortingOrder(sortingOrder + SortingSteps);
                    break;
                case DynamicSorting.Behind:
                    SetSortingLayer(sortingLayer);
                    SetSortingOrder(sortingOrder - SortingSteps);
                    break;
                case DynamicSorting.Custom:
                    SetSortingLayer(CustomSortingLayer);
                    SetSortingOrder(CustomSortingOrder);
                    break;
            }
        }

        #endregion
    }
}