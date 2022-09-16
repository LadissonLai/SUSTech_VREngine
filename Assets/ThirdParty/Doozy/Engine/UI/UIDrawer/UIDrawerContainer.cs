// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Base;
using UnityEngine;

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    /// Is the actual component, of an UIDrawer, that gets animated and holds all of the UIDrawer's content.
    /// </summary>
    [Serializable]
    public class UIDrawerContainer : UIContainer
    {
        #region Constants
        
        private const UIDrawerContainerSize DEFAULT_CONTAINER_SIZE = UIDrawerContainerSize.PercentageOfScreen;
        private const bool DEFAULT_FADE_OUT = true;
        private const float DEFAULT_FIXED_SIZE = 128f;
        private const float DEFAULT_MINIMUM_SIZE = 128f;
        private const float DEFAULT_PERCENTAGE_OF_SCREEN_SIZE = 0.5f;

        #endregion

        #region Properties
        
        /// <summary> Returns the container's current velocity = CurrentPosition - PreviousPosition </summary>
        public Vector2 Velocity { get { return CurrentPosition - PreviousPosition; } }
        
        #endregion

        #region Public Variables

        /// <summary> The UIDrawerContainer calculated size </summary>
        public Vector2 CalculatedSize;

        /// <summary> RectTransform.anchoredPosition3D when the drawer is closed </summary>
        public Vector3 ClosedPosition;
        
        /// <summary> RectTransform.anchoredPosition3D in the current frame </summary>
        public Vector3 CurrentPosition;
        
        /// <summary> If TRUE, when the UIDrawer closes, the container will fade out and, when the UIDrawer opens, the container will fade in </summary>
        public bool FadeOut;
        
        /// <summary> When UIDrawerContainerSize is set to FixedSize, the container will have a set fixed size </summary>
        public float FixedSize;
        
        /// <summary> When UIDrawerContainerSize is set to PercentageOfScreen, the container will have a set minimum size </summary>
        public float MinimumSize;
        
        /// <summary> RectTransform.anchoredPosition3D when the drawer is open </summary>
        public Vector3 OpenedPosition;

        /// <summary> When UIDrawerContainerSize is set to PercentageOfScreen, the container's width and height will be calculated as percentage of the screen's size </summary>
        public float PercentageOfScreen;

        /// <summary> RectTransform.anchoredPosition3D in the previous frame </summary>
        public Vector3 PreviousPosition;

        /// <summary> Determines the container's size (FullScreen, PercentageOfScreen or FixedSize) </summary>
        public UIDrawerContainerSize Size;

        #endregion
        
        #region Constructors

        // ReSharper disable once VirtualMemberCallInConstructor
        /// <inheritdoc />
        /// <summary> Initializes a new instance of the class </summary>
        public UIDrawerContainer() { Reset(); }

        #endregion
        
        #region Public Methods
        
        /// <inheritdoc />
        /// <summary> Resets this instance to the default values </summary>
        public override void Reset()
        {
            base.Reset();

            Size = DEFAULT_CONTAINER_SIZE;
            PercentageOfScreen = DEFAULT_PERCENTAGE_OF_SCREEN_SIZE;
            MinimumSize = DEFAULT_MINIMUM_SIZE;
            FixedSize = DEFAULT_FIXED_SIZE;
            FadeOut = DEFAULT_FADE_OUT;
        }

//        public void UpdatePosition(Vector3 anchoredPosition3D) { RectTransform.anchoredPosition3D = anchoredPosition3D; }
        
        #endregion
    }
}