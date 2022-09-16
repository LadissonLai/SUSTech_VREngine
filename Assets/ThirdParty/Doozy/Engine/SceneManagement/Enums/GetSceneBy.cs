// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Engine.SceneManagement
{
    /// <summary> Describes the ways a Scene can be retrieved from the SceneManager </summary>
    public enum GetSceneBy
    {
        /// <summary> Use the Scene Name or path of the Scene to load </summary>
        Name,

        /// <summary> Use the index of the Scene in the Build Settings </summary>
        BuildIndex
    }
}