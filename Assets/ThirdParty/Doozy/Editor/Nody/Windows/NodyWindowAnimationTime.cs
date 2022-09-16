// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Nody.Utils;
using Doozy.Engine;
using Doozy.Engine.Nody;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Nody.Windows
{
    public partial class NodyWindow
    {
        private double m_lastAnimationTime;
        private double m_currentAnimationTime;

        /// <summary> Returns the animation time [0,1] </summary>
        private float AnimationTime
        {
            get
            {
                m_currentAnimationTime = EditorApplication.timeSinceStartup - m_lastAnimationTime;
                if (m_currentAnimationTime > 1 / CurrentDotAnimationSpeed)
                {
                    m_currentAnimationTime = 0;
                    m_lastAnimationTime = EditorApplication.timeSinceStartup;
//                    PrintDotAnimationCurveKeyframes();
                }

                return NodyWindowSettings.Instance.DotAnimationCurve.Evaluate((float) m_currentAnimationTime / (1 / CurrentDotAnimationSpeed));
            }
        }

        // ReSharper disable once UnusedMember.Local
        /// <summary> Print to the Console the current DotAnimationCurve keyframes. This is useful to get the values needed to create (from code) the custom animation curve for the dot.</summary>
        private void PrintDotAnimationCurveKeyframes()
        {
            DDebug.Log("-------------------");
            for (int i = 0; i < NodyWindowSettings.Instance.DotAnimationCurve.keys.Length; i++)
            {
                Keyframe keyframe = NodyWindowSettings.Instance.DotAnimationCurve.keys[i];
                DDebug.Log(string.Format("[{0}] time: {1} | value: {2} | inTangent: {3} | outTangent: {4}", i, keyframe.time, keyframe.value, keyframe.inTangent, keyframe.outTangent));
            }

            DDebug.Log("-------------------");
        }
    }
}