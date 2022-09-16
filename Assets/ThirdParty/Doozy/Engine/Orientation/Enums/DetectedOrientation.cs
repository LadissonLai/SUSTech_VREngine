// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Orientation
{
    /// <summary> Describes the physical orientation of the device as determined by the OS. This is a simplified version of the UnityEngine.DeviceOrientation enum  </summary>
    public enum DetectedOrientation
    {
        /// <summary> The orientation of the device cannot be determined </summary>
        Unknown = 0,

        /// <summary>
        ///     The device is in portrait mode, with the device held upright and the home button at the bottom (Portrait) - OR - The device is in portrait mode but upside down, with the device held upright and the home button at the top (PortraitUpsideDown)
        /// </summary>
        Portrait = 1,

        /// <summary>
        ///     The device is in landscape mode, with the device held upright and the home button on the right side (LandscapeLeft) - OR -  The device is in landscape mode, with the device held upright and the home button on the left side (LandscapeRight)
        /// </summary>
        Landscape = 2
    }
}