// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.UI.Animation;

// ReSharper disable NotAccessedField.Global

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     UIPopup global message
    /// </summary>
    [Serializable]
    public class UIPopupMessage : Message
    {
        #region Public Variables

        /// <summary> Reference to the UIPopup that sent this message </summary>
        public UIPopup Popup;

        /// <summary> AnimationType of the UIPopupBehavior that triggered the UIPopup to send this message </summary>
        public AnimationType AnimationType;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the class with a reference to the UIPopup that triggered that sent this message
        ///     (AnimationType = AnimationType.Undefined)
        /// </summary>
        /// <param name="popup"> Reference to the UIPopup that sent this message </param>
        public UIPopupMessage(UIPopup popup)
        {
            Popup = popup;
            AnimationType = AnimationType.Undefined;
        }

        /// <summary>
        ///     Initializes a new instance of the class with a reference to the UIPopup and AnimationType,
        ///     of the UIPopupBehavior, that triggered that sent this message
        /// </summary>
        /// <param name="popup"> Reference to the UIPopup that sent this message </param>
        /// <param name="animationType"> AnimationType of the UIPopupBehavior that triggered the UIPopup to send this message </param>
        public UIPopupMessage(UIPopup popup, AnimationType animationType)
        {
            Popup = popup;
            AnimationType = animationType;
        }

        #endregion
    }
}