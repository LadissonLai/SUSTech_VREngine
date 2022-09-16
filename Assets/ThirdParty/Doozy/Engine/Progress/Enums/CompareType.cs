// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.Progress
{
    /// <summary>
    /// Describes the types of number comparisons
    /// </summary>
    public enum CompareType
    {
        /// <summary>
        /// Inside a range defined by a min value and a max value
        /// </summary>
        Between,
        
        /// <summary>
        /// Outside of a range defined by a min value and a max value
        /// </summary>
        NotBetween,
        
        /// <summary>
        /// Equal to a set value 
        /// </summary>
        EqualTo,
        
        /// <summary>
        /// Not equal to a set value
        /// </summary>
        NotEqualTo,
        
        /// <summary>
        /// Greater than a set value
        /// </summary>
        GreaterThan,
        
        /// <summary>
        /// Less than a set value
        /// </summary>
        LessThan,
        
        /// <summary>
        /// Greater than or equal to a set value
        /// </summary>
        GreaterThanOrEqualTo,
        
        /// <summary>
        /// Less than or equal to a set value
        /// </summary>
        LessThanOrEqualTo
    }
}