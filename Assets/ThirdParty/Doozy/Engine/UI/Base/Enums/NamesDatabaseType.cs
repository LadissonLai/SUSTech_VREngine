// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Base
{
    /// <summary>
    /// Defines the names of the components that make use of the NamesDatabase class.
    /// For each component type, the NamesDatabase is configured to work in a different specialized way.
    /// </summary>
    public enum NamesDatabaseType
    {
        /// <summary>
        ///     UIButton component
        /// </summary>
        UIButton,

        /// <summary>
        ///     UICanvas component
        /// </summary>
        UICanvas,

        /// <summary>
        ///     UIView component
        /// </summary>
        UIView,

        /// <summary>
        ///     UIDrawer component
        /// </summary>
        UIDrawer
    }
}