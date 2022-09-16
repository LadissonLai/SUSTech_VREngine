// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Progress
{
    /// <summary>
    /// Describes the types of progress values a Progressor has
    /// </summary>
    public enum TargetProgress
    {
        /// <summary>
        /// Progress value (float between 0 and 1)
        /// </summary>
        Progress,

        /// <summary>
        /// InverseProgress value (float between 1 and 0)
        /// </summary>
        InverseProgress
    }
}