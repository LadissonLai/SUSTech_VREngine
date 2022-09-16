// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    /// UIButton global message
    /// </summary>
    [Serializable]
    public class UIButtonMessage : Message
    {
        #region Public Variables
        
        /// <summary> Reference to the UIButton that sent this message </summary>
        public UIButton Button;

        /// <summary> Button name of the UIButton that sent this message (used when a UIButton reference is not available) </summary>
        public string ButtonName;

        /// <summary> UIButtonBehaviorType of the UIButtonBehavior that triggered the UIButton to send this message </summary>
        public UIButtonBehaviorType Type;

        #endregion
        
        #region Constructors
        
        /// <summary> Initializes a new instance of the class with a reference to the UIButton that sent this message </summary>
        /// <param name="button"> Reference to the UIButton that sent this message </param>
        public UIButtonMessage(UIButton button)
        {
            ButtonName = button.ButtonName;
            Button = button;
        }
        
        /// <summary> Initializes a new instance of the class with reference to the UIButton and the UIButtonBehaviorType, of the UIButtonBehavior, that triggered this message </summary>
        /// <param name="button"> Reference to the UIButton that sent this message </param>
        /// <param name="type"> UIButtonBehaviorType of the UIButtonBehavior that triggered the UIButton to send this message </param>
        public UIButtonMessage(UIButton button, UIButtonBehaviorType type)
        {
            ButtonName = button.ButtonName;
            Button = button;
            Type = type;
        }
        
        /// <summary> Initializes a new instance of the class with the button name of the UIButton that sent this message </summary>
        /// <param name="buttonName"> Button name of the UIButton that sent this message </param>
        public UIButtonMessage(string buttonName)
        {
            ButtonName = buttonName;
            Button = null;
        }

        /// <summary> Initializes a new instance of the class with the button name and the UIButtonBehaviorType, of the UIButtonBehavior, that triggered that sent this message </summary>
        /// <param name="buttonName"> Button name of the UIButton that sent this message </param>
        /// <param name="type"> UIButtonBehaviorType of the UIButtonBehavior that triggered the UIButton to send this message </param>
        public UIButtonMessage(string buttonName, UIButtonBehaviorType type)
        {
            ButtonName = buttonName;
            Button = null;
            Type = type;
        }
        
        #endregion
    }
}