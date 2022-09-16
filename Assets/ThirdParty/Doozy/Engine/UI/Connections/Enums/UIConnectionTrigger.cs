// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Connections
{
    /// <summary>
    /// Describes the types of triggers available for an UIConnection.
    /// What triggers a UIConnection input node to become active.
    /// </summary>
    public enum UIConnectionTrigger
    {
        /// <summary>
        /// A UIButton OnClick will trigger the UIConnection to change the active node to the connection's input node
        /// </summary>
        ButtonClick,
        
        /// <summary>
        /// A UIButton OnDoubleClick will trigger the UIConnection to change the active node to the connection's input node
        /// </summary>
        ButtonDoubleClick,
        
        /// <summary>
        /// A UIButton OnLongClick will trigger the UIConnection to change the active node to the connection's input node
        /// </summary>
        ButtonLongClick,
        
        /// <summary>
        /// A specific game event string will trigger the UIConnection to change the active node to the connection's input node
        /// </summary>
        GameEvent,
        
        /// <summary>
        /// A timer will trigger the UIConnection to change the active node to the connection's input node
        /// </summary>
        TimeDelay
    }
}