// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the states of visibility for certain UI components (like the UIView, UIDrawer, UIPopup) </summary>
    public enum VisibilityState
    {
        /// <summary> Is visible </summary>
        Visible = 0,

        /// <summary> Is NOT visible </summary>
        NotVisible = 1,

        /// <summary> Is playing the HIDE animation (in transition exit view) </summary>
        Hiding = 2,

        /// <summary> Is playing the SHOW animation (in transition enter view) </summary>
        Showing = 3
    }
}