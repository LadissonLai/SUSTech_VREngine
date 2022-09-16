// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes what types of UIButtonBehaviors are available for an UIButton </summary>
    public enum UIButtonBehaviorType
    {
        /// <summary>
        ///     UIButton behavior when the pointer performs a left mouse button click over the button
        /// </summary>
        OnClick,

        /// <summary>
        ///     UIButton behavior when the pointer performs a left mouse button double click over the button
        /// </summary>
        OnDoubleClick,

        /// <summary>
        ///     UIButton behavior when the pointer performs a left mouse button long click over the button
        /// </summary>
        OnLongClick,
     
        /// <summary>
        ///     UIButton behavior when the pointer enters (hovers in) over the button's area
        /// </summary>
        OnPointerEnter,

        /// <summary>
        ///     UIButton behavior when the pointer exits (hovers out) the button's area (happens only after OnPointerEnter)
        /// </summary>
        OnPointerExit,

        /// <summary>
        ///     UIButton behavior when the pointer is down over the button
        /// </summary>
        OnPointerDown,

        /// <summary>
        ///     UIButton behavior when the pointer is up over the button (happens only after OnPointerDown)
        /// </summary>
        OnPointerUp,

        /// <summary>
        ///     UIButton behavior when the button gets selected
        /// </summary>
        OnSelected,

        /// <summary>
        ///     UIButton behavior when the button gets deselected
        /// </summary>
        OnDeselected,
           
        /// <summary>
        ///     UIButton behavior when the pointer performs a right mouse button click over the button
        /// </summary>
        OnRightClick
    }
}