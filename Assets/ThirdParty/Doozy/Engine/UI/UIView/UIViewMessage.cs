// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     UIView global message
    /// </summary>
    [Serializable]
    public class UIViewMessage : Message
    {
        #region Public Variables

        /// <summary> Reference to the UIView that sent this message </summary>
        public UIView View;

        /// <summary> UIViewBehaviorType of the UIViewBehavior that triggered the UIView to send this message </summary>
        public UIViewBehaviorType Type;
        
        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class with a reference to the UIView that sent this message </summary>
        /// <param name="view"> Reference to the UIView that sent this message </param>
        public UIViewMessage(UIView view)
        {
            View = view;
            Type = UIViewBehaviorType.Unknown;
        }

        /// <summary> Initializes a new instance of the class with reference to the UIView and the UIViewBehaviorType, of the UIViewBehavior, that triggered this message </summary>
        /// <param name="view"> Reference to the UIView that sent this message </param>
        /// <param name="type"> UIViewBehaviorType of the UIViewBehavior that triggered the UIView to send this message </param>
        public UIViewMessage(UIView view, UIViewBehaviorType type)
        {
            View = view;
            Type = type;
        }

        #endregion
    }
}