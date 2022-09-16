// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary> Base class for all types of targets that a Progressor can have. Used by a Progressor to update a target's value </summary>
    [Serializable]
    public abstract class ProgressTarget : MonoBehaviour
    {
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

        /// <summary> Method used by a Progressor to when the current Value has changed </summary>
        /// <param name="progressor"> The Progressor that triggered this update.
        /// <para/> It allows access to all the relevant variables (Value, Progress, ReversedProgress, MinValue, MaxValue...)
        /// </param>
        public virtual void UpdateTarget(Progressor progressor) { }
    }
}