// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.Progress
{
    /// <summary>
    /// Describes the reset options for the current Value of a Progressor
    /// </summary>
    [Serializable]
    public enum ResetValue
    {
        /// <summary>
        /// Value will not get reset
        /// </summary>
        Disabled,

        /// <summary>
        /// Value will get reset to the minimum value
        /// </summary>
        ToMinValue,

        /// <summary>
        /// Value will get reset to the maximum value
        /// </summary>
        ToMaxValue,

        /// <summary>
        /// Value will get reset to a custom value
        /// </summary>
        ToCustomValue
    }
}