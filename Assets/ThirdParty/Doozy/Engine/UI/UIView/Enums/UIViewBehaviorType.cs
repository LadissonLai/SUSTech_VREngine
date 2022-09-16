// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes what types of UIViewBehaviors are available for an UIView </summary>
    public enum UIViewBehaviorType
    {
        /// <summary>
        ///  Unknown UIView behavior
        /// </summary>
        Unknown,
        
        /// <summary>
        ///  UIView behavior when it becomes visible
        /// </summary>
        Show,
        
        /// <summary>
        ///  UIView behavior when it goes off screen 
        /// </summary>
        Hide,
        
        /// <summary>
        ///  UIView behavior when it starts the loop animation 
        /// </summary>
        Loop
    }
}