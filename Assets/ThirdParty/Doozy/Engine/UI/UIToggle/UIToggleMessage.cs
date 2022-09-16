// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

// ReSharper disable NotAccessedField.Global

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     UIToggle global message
    /// </summary>
    [Serializable]
    public class UIToggleMessage : Message
    {
        #region Public Variables

        /// <summary> Reference to the UIToggle that sent this message </summary>
        public UIToggle Toggle;

        /// <summary> The toggle state, the UIToggle was in, when the message was sent </summary>
        public UIToggleState ToggleState;

        /// <summary> UIToggleBehaviorType of the UIToggleBehavior that triggered the UIToggle to send this message </summary>
        public UIToggleBehaviorType Type;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class with the toggle state and behavior type of the UIToggle that sent this message </summary>
        /// <param name="toggleState"> The toggle state the UIToggle was in when the message was sent </param>
        /// <param name="type"></param>
        public UIToggleMessage(UIToggleState toggleState, UIToggleBehaviorType type)
        {
            Toggle = null;
            ToggleState = toggleState;
            Type = type;
        }

        /// <summary> Initializes a new instance of the class with reference to the UIToggle, its toggle state and UIButtonBehaviorType, of the UIButtonBehavior, that triggered this message </summary>
        /// <param name="toggle"> Reference to the UIToggle that sent this message </param>
        /// <param name="toggleState"> The toggle state the UIToggle was in when the message was sent </param>
        /// <param name="type"> UIToggleBehaviorType of the UIToggleBehavior that triggered the UIToggle to send this message </param>
        public UIToggleMessage(UIToggle toggle, UIToggleState toggleState, UIToggleBehaviorType type)
        {
            Toggle = toggle;
            ToggleState = toggleState;
            Type = type;
        }

        #endregion
    }
}