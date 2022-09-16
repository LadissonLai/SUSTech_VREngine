// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine
{
    /// <summary>
    ///     Struct that contains a minimum and a maximum values that define a float interval
    /// </summary>
    [Serializable]
    public struct RangedFloat
    {
        /// <summary>
        ///     Minimum value for the float interval
        /// </summary>
        public float MinValue;

        /// <summary>
        ///     Maximum value for the float interval
        /// </summary>
        public float MaxValue;
    }
}