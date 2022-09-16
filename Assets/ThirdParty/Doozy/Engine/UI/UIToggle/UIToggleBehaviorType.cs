// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI
{
    /// <summary> Describes what types of UIToggleBehaviors are available for an UIToggle </summary>
    public enum UIToggleBehaviorType
    {
        /// <summary>
        ///     UIToggle behavior when the pointer performs a click over the toggle
        /// </summary>
        OnClick,

        /// <summary>
        ///     UIToggle behavior when the pointer enters (hovers in) over the toggle's area
        /// </summary>
        OnPointerEnter,

        /// <summary>
        ///     UIToggle behavior when the pointer exits (hovers out) the toggle's area (happens only after OnPointerEnter)
        /// </summary>
        OnPointerExit,

        /// <summary>
        ///     UIToggle behavior when the toggle gets selected
        /// </summary>
        OnSelected,

        /// <summary>
        ///     UIToggle behavior when the toggle gets deselected
        /// </summary>
        OnDeselected
    }
}