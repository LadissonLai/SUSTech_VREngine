// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes what types of UIDrawerBehaviors are available for an UIDrawer </summary>
    public enum UIDrawerBehaviorType
    {
        /// <summary>
        ///     The UIDrawer becomes visible on screen
        /// </summary>
        Open,

        /// <summary>
        ///     The UIDrawer goes off screen
        /// </summary>
        Close,

        /// <summary>
        ///     The UIDrawer is dragged
        /// </summary>
        Drag
    }
}