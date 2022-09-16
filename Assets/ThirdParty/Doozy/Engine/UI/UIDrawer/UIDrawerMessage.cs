// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     UIDrawer global message
    /// </summary>
    [Serializable]
    public class UIDrawerMessage : Message
    {
        #region Public Variables

        /// <summary> Reference to the UIDrawer that sent this message </summary>
        public UIDrawer Drawer;

        /// <summary> UIDrawerBehaviorType of the UIDrawerBehavior that triggered the UIDrawer to send this message </summary>
        public UIDrawerBehaviorType Type;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the class with reference to the UIDrawer and the UIDrawerBehaviorType,
        ///     of the UIDrawerBehavior, that triggered that sent this message
        /// </summary>
        /// <param name="drawer"> Reference to the UIDrawer that sent this message </param>
        /// <param name="type"> UIDrawerBehaviorType of the UIDrawerBehavior that triggered the UIDrawer to send this message </param>
        public UIDrawerMessage(UIDrawer drawer, UIDrawerBehaviorType type)
        {
            Drawer = drawer;
            Type = type;
        }

        #endregion
    }
}