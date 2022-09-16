// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the possible orientation targets for UI components (like the UIView) </summary>
    public enum TargetOrientation
    {
        /// <summary> Target orientation for Portrait and PortraitUpsideDown screen orientations </summary>
        Portrait = 0,

        /// <summary> Target orientation for LandscapeLeft and LandscapeRight screen orientations </summary>
        Landscape = 1,

        /// <summary> Target orientation for any screen orientation </summary>
        Any = 2
    }
}