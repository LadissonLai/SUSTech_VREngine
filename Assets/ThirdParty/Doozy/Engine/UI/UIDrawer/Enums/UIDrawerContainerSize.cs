// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the sizes an UIDrawer UIDrawerContainer can have </summary>
    public enum UIDrawerContainerSize
    {
        /// <summary> UIDrawerContainer covers the entire screen view area </summary>
        FullScreen,

        /// <summary> UIDrawerContainer covers a set percentage of the screen view area </summary>
        PercentageOfScreen,

        /// <summary> UIDrawerContainer has a set size, regardless of the screen view area </summary>
        FixedSize
    }
}