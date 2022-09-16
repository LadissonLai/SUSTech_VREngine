// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Contains popup queue data used by the UIPopupManager, for its PopupQueue, in order to keep track of the UIPopups that need to be shown in a sequential manner
    /// </summary>
    [Serializable]
    public class UIPopupQueueData
    {
        #region Public Variables

        /// <summary> Reference to an UIPopup in the scene (that is hidden and awaits to be shown) </summary>
        public UIPopup Popup;

        /// <summary> Designated popup name </summary>
        public string PopupName;

        /// <summary> Should the show animation happen instantly? (zero seconds) </summary>
        public bool InstantAction;

        #endregion

        #region Constructors 

        /// <summary> Initializes a new instance of the class with the given settings </summary>
        /// <param name="popup"> Reference to an UIPopup in the scene (that is hidden and awaits to be shown)  </param>
        /// <param name="instantAction"> Should the show animation happen instantly? (zero seconds) </param>
        public UIPopupQueueData(UIPopup popup, bool instantAction = false)
        {
            PopupName = popup.PopupName;
            Popup = popup;
            InstantAction = instantAction;
        }

        /// <summary> Initializes a new instance of the class with the given settings </summary>
        /// <param name="popupName"> Designated popup name </param>
        /// <param name="popup"> Reference to an UIPopup in the scene (that is hidden and awaits to be shown) </param>
        /// <param name="instantAction"> Should the show animation happen instantly? (zero seconds) </param>
        public UIPopupQueueData(string popupName, UIPopup popup, bool instantAction = false)
        {
            PopupName = popupName;
            Popup = popup;
            InstantAction = instantAction;
        }

        #endregion

        #region Public Methods

        /// <summary> Shows the referenced UIPopup with its preset settings </summary>
        public UIPopup Show()
        {
            if (Popup == null) return null;
            Popup.Show(InstantAction);
            return Popup;
        }

        #endregion
    }
}