// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes the UIEffect automated sorting behaviors for its target ParticleSystem </summary>
    public enum DynamicSorting
    {
        /// <summary> Automated sorting disabled </summary>
        Disabled,

        /// <summary> Automatically updates source's SortingLayer and order in layer to appear in front of a specified target </summary>
        InFront,

        /// <summary> Automatically updates source's SortingLayer and order in layer to appear behind a specified target </summary>
        Behind,

        /// <summary> Uses custom SortingLayer and sorting order values </summary>
        Custom
    }
}