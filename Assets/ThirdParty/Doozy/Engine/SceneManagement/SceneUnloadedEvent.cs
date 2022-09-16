// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Doozy.Engine.SceneManagement
{
    /// <inheritdoc />
    /// <summary>
    /// UnityEvent used by the SceneDirector to notify when a Scene was unloaded.
    /// <para/> Includes a reference to the unloaded Scene.
    /// </summary>
    [Serializable]
    public class SceneUnloadedEvent : UnityEvent<Scene> { }
}