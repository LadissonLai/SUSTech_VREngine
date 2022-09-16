// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine
{
    /// <summary> Contains DoozyUI special game events, also known as system events, that trigger predefined actions </summary>
    public enum SystemGameEvent
    {
        /// <summary> Activates all the Scenes that have been loaded by SceneLoaders and are ready to be activated </summary>
        ActivateLoadedScenes,

        /// <summary> Exits play mode (if in editor) or quits the application if in build mode </summary>
        ApplicationQuit,

        /// <summary> Triggers the 'Back' button system function </summary>
        Back,
    }
}