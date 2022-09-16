// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes UIPopup display target options. Where should the UIPopup be shown (target UICanvas). </summary>
    public enum PopupDisplayOn
    {
        /// <summary>
        ///     Before showing the UIPopup, the system tries to get the PopupCanvas.
        ///     If the PopupCanvas does not exist, it will create a new Canvas and set its Render Mode to Screen Space - Overlay,
        ///     then returns a reference to it.
        /// </summary>
        PopupCanvas,

        /// <summary>
        ///     Before showing the UIPopup, the system looks for the specified target canvas by searching for a given canvasName.
        ///     If it does not find it, it returns a reference to the MasterCanvas UICanvas.
        /// </summary>
        TargetCanvas
    }
}