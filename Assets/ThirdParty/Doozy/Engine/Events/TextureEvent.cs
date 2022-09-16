// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Engine.Events
{
    /// <inheritdoc />
    /// <summary> UnityEvent used to send Texture references </summary>
    [Serializable]
    public class TextureEvent : UnityEvent<Texture> { }
}