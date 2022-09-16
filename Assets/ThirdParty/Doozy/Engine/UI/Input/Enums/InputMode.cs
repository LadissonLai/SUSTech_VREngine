// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.UI.Input
{
    /// <summary> Describes the types of controller input modes that are enabled. By default the system reacts to mouse clicks and touches. </summary>
    public enum InputMode
    {
        /// <summary> System only reacts to mouse clicks and touches  </summary>
        None,

        /// <summary> System also reacts to a set KeyCode, besides the mouse clicks and touches </summary>
        KeyCode,

        /// <summary> System also reacts to a set virtual button name (set up in the InputManager), besides the mouse clicks and touches </summary>
        VirtualButton
    }
}