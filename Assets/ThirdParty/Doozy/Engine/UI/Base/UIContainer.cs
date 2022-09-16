// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Animation;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI.Base
{
    /// <summary> Base class for any UI component that has a RectTransform and also needs to have the Canvas, GraphicRaycaster and CanvasGroup components attached </summary>
    [Serializable]
    public class UIContainer
    {
        #region Contants

        public const bool DEFAULT_DISABLE_CANVAS = true;
        public const bool DEFAULT_DISABLE_GAME_OBJECT = true;
        public const bool DEFAULT_DISABLE_GRAPHIC_RAYCASTER = true;
        public const bool DEFAULT_ENABLED = true;

        #endregion

        #region Public Variables

        /// <summary> Reference to the Canvas component </summary>
        public Canvas Canvas;

        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup CanvasGroup;

        /// <summary> If TRUE, the Canvas component can be automatically disabled when not in view </summary>
        public bool DisableCanvas = DEFAULT_DISABLE_CANVAS;

        /// <summary> If TRUE, the container's GameObject can be automatically disabled when not in view </summary>
        public bool DisableGameObject = DEFAULT_DISABLE_GAME_OBJECT;

        /// <summary> If TRUE, the GraphicRaycaster component can be automatically disabled when not in view </summary>
        public bool DisableGraphicRaycaster = DEFAULT_DISABLE_GRAPHIC_RAYCASTER;

        /// <summary> Deactivates the usage this UIContainer by other components </summary>
        public bool Enabled = DEFAULT_ENABLED;

        /// <summary> Reference to the GraphicRaycaster component </summary>
        public GraphicRaycaster GraphicRaycaster;

        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform RectTransform;

        /// <summary> Holds the start alpha. It does that by checking if a CanvasGroup component is attached (holding the alpha value) or it just remembers 1 (as in 100% visibility) </summary>
        public float StartAlpha = UIAnimator.DEFAULT_START_ALPHA;

        /// <summary> Holds the start RectTransform.anchoredPosition3D </summary>
        public Vector3 StartPosition = UIAnimator.DEFAULT_START_POSITION;

        /// <summary> Holds the start RectTransform.localEulerAngles </summary>
        public Vector3 StartRotation = UIAnimator.DEFAULT_START_ROTATION;

        /// <summary> Holds the start RectTransform.localScale </summary>
        public Vector3 StartScale = UIAnimator.DEFAULT_START_SCALE;

        #endregion

        #region Contsturctors

        /// <summary> Initializes a new instance of the class </summary>
        public UIContainer() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary>
        ///     If RectTransform != null
        ///     <para />
        ///     Disables the RectTransform.gameObject (if DisableGameObject is TRUE),
        ///     <para />
        ///     Disables the Canvas component (if DisableCanvas is TRUE),
        ///     <para />
        ///     Disables the GraphicRaycaster component (if DisableGraphicRaycaster is TRUE)
        /// </summary>
        public virtual void Disable()
        {
            if (RectTransform == null) return;
            if (DisableGameObject) RectTransform.gameObject.SetActive(false);
            if (DisableCanvas)
            {
                Canvas.enabled = false;
                if (DisableGraphicRaycaster)
                    GraphicRaycaster.enabled = false;
            }
        }

        /// <summary> If Enabled is TRUE and RectTransform != null, it enables the GameObject, the Canvas and the GraphicRaycaster components </summary>
        public virtual void Enable()
        {
            if (!Enabled) return;
            if (RectTransform == null) return;
            RectTransform.gameObject.SetActive(true);
            Canvas.enabled = true;
            GraphicRaycaster.enabled = true;
        }

        /// <summary> Makes the RectTransform match its parent size </summary>
        /// <param name="resetScaleToOne"> Reset LocalScale to Vector3.one </param>
        public void FullScreen(bool resetScaleToOne)
        {
            if (RectTransform == null) return;
            RectTransform.FullScreen(resetScaleToOne);
        }

        /// <summary> Initializes this instance by checking for a RectTransform and then getting the references for the Canvas, GraphicRaycaster and CanvasGroup components </summary>
        public virtual void Init()
        {
            if (RectTransform == null) return;
            
            if (Canvas == null)
            {
                Canvas = RectTransform.gameObject.GetComponent<Canvas>();
                if (Canvas == null)
                    Canvas = RectTransform.gameObject.AddComponent<Canvas>();
            }

            if (GraphicRaycaster == null)
            {
                GraphicRaycaster = RectTransform.gameObject.GetComponent<GraphicRaycaster>();
                if (GraphicRaycaster == null)
                    GraphicRaycaster = RectTransform.gameObject.AddComponent<GraphicRaycaster>();
            }

            if (CanvasGroup == null)
            {
                CanvasGroup = RectTransform.gameObject.GetComponent<CanvasGroup>();
                if(CanvasGroup == null)
                    CanvasGroup = RectTransform.gameObject.AddComponent<CanvasGroup>();
            }
            
            if (Canvas.enabled && DisableCanvas) Canvas.enabled = false;
            if (GraphicRaycaster.enabled & DisableGraphicRaycaster) GraphicRaycaster.enabled = false;

            if (!Enabled)
            {
                Disable();
                return;
            }

            UpdateStartValues();
        }

        /// <summary> Resets this instance to the default values </summary>
        public virtual void Reset()
        {
            DisableCanvas = DEFAULT_DISABLE_CANVAS;
            DisableGameObject = DEFAULT_DISABLE_GAME_OBJECT;
            DisableGraphicRaycaster = DEFAULT_DISABLE_GRAPHIC_RAYCASTER;
            Enabled = DEFAULT_ENABLED;
        }

        /// <summary> Resets the CanvasGroup.alpha to the StartAlpha value (if a CanvasGroup is attached) </summary>
        public virtual void ResetAlpha()
        {
            if (RectTransform.GetComponent<CanvasGroup>() != null) RectTransform.GetComponent<CanvasGroup>().alpha = StartAlpha;
        }

        /// <summary> Resets the RectTransform.anchoredPosition3D to the StartPosition value </summary>
        public virtual void ResetPosition() { RectTransform.anchoredPosition3D = StartPosition; }

        /// <summary> Resets the RectTransform.localEulerAngles to the StartRotation value </summary>
        public virtual void ResetRotation() { RectTransform.localEulerAngles = StartRotation; }

        /// <summary> Resets the RectTransform.localScale to the StartScale value </summary>
        public virtual void ResetScale() { RectTransform.localScale = StartScale; }

        /// <summary> Resets this RectTransform's value to the Start values (StartPosition, StartRotation, StartScale and StartAlpha) </summary>
        public virtual void ResetToStartValues()
        {
            if (RectTransform == null) return;
            UIAnimator.ResetCanvasGroup(RectTransform);
            ResetPosition();
            ResetRotation();
            ResetScale();
            ResetAlpha();
        }

        /// <summary> Updates the StartAlpha to the CanvasGroup.alpha value (if a CanvasGroup is attached) </summary>
        public virtual void UpdateStartAlpha() { StartAlpha = RectTransform.GetComponent<CanvasGroup>() == null ? 1 : RectTransform.GetComponent<CanvasGroup>().alpha; }

        /// <summary> Updates the StartPosition to the RectTransform.anchoredPosition3D value </summary>
        public virtual void UpdateStartPosition() { StartPosition = RectTransform.anchoredPosition3D; }

        /// <summary> Updates the StartRotation to the RectTransform.localEulerAngles value </summary>
        public virtual void UpdateStartRotation() { StartRotation = RectTransform.localEulerAngles; }

        /// <summary> Updates the StartScale to the RectTransform.localScale value </summary>
        public virtual void UpdateStartScale() { StartScale = RectTransform.localScale; }

        /// <summary> Updates the StartPosition, StartRotation, StartScale and StartAlpha for this RectTransform to the current values </summary>
        public virtual void UpdateStartValues()
        {
            if (RectTransform == null) return;
            UpdateStartPosition();
            UpdateStartRotation();
            UpdateStartScale();
            UpdateStartAlpha();
        }

        #endregion
    }
}