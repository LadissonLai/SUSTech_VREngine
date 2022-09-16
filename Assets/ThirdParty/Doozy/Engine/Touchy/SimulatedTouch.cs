// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Touchy
{
    /// <summary> Creates a simulated Touch struct that describes the status of a finger touching the screen </summary>
    public class SimulatedTouch
    {
        public bool WasModified { get; set; }

        private const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly Dictionary<string, FieldInfo> Fields;
        private readonly object m_touch;

        /// <summary> Creates a new instance of this class </summary>
        public SimulatedTouch()
        {
            m_touch = new Touch();
            WasModified = false;
        }

        static SimulatedTouch()
        {
            Fields = new Dictionary<string, FieldInfo>();
            foreach (FieldInfo f in typeof(Touch).GetFields(FLAGS))
                Fields.Add(f.Name, f);
        }

        #region Properties
        
        /// <summary> The unique index for the touch </summary>
        public int FingerId { get { return ((Touch) m_touch).fingerId; } set { Fields["m_FingerId"].SetValue(m_touch, value); } }

        /// <summary> The position of the touch in pixel coordinates </summary>
        public Vector2 Position { get { return ((Touch) m_touch).position; } set { Fields["m_Position"].SetValue(m_touch, value); } }

        /// <summary> The raw position used for the touch </summary>
        public Vector2 RawPosition { get { return ((Touch) m_touch).rawPosition; } set { Fields["m_RawPosition"].SetValue(m_touch, value); } }

        /// <summary> The position delta since last change </summary>
        public Vector2 DeltaPosition { get { return ((Touch) m_touch).deltaPosition; } set { Fields["m_PositionDelta"].SetValue(m_touch, value); } }

        /// <summary> Amount of time that has passed since the last recorded change in Touch values </summary>
        public float DeltaTime { get { return ((Touch) m_touch).deltaTime; } set { Fields["m_TimeDelta"].SetValue(m_touch, value); } }

        /// <summary> Number of taps </summary>
        public int TapCount { get { return ((Touch) m_touch).tapCount; } set { Fields["m_TapCount"].SetValue(m_touch, value); } }

        /// <summary> Describes the phase of the touch </summary>
        public TouchPhase Phase { get { return ((Touch) m_touch).phase; } set { Fields["m_Phase"].SetValue(m_touch, value); } }

        /// <summary> The current amount of pressure being applied to a touch. 1.0f is considered to be the pressure of an average touch. If Input.touchPressureSupported returns false, the value of this property will always be 1.0f </summary>
        public float Pressure { get { return ((Touch) m_touch).pressure; } set { Fields["m_Pressure"].SetValue(m_touch, value); } }

        /// <summary> The maximum possible pressure value for a platform. If Input.touchPressureSupported returns false, the value of this property will always be 1.0f </summary>
        public float MaximumPossiblePressure { get { return ((Touch) m_touch).maximumPossiblePressure; } set { Fields["m_maximumPossiblePressure"].SetValue(m_touch, value); } }

        /// <summary> A value that indicates whether a touch was of Direct, Indirect (or remote), or Stylus type </summary>
        public TouchType Type { get { return ((Touch) m_touch).type; } set { Fields["m_Type"].SetValue(m_touch, value); } }

        /// <summary> Value of 0 radians indicates that the stylus is parallel to the surface, pi/2 indicates that it is perpendicular </summary>
        public float AltitudeAngle { get { return ((Touch) m_touch).altitudeAngle; } set { Fields["m_AltitudeAngle"].SetValue(m_touch, value); } }

        /// <summary> Value of 0 radians indicates that the stylus is pointed along the x-axis of the device </summary>
        public float AzimuthAngle { get { return ((Touch) m_touch).azimuthAngle; } set { Fields["m_AzimuthAngle"].SetValue(m_touch, value); } }

        /// <summary> An estimated value of the radius of a touch. Add radiusVariance to get the maximum touch size, subtract it to get the minimum touch size </summary>
        public float Radius { get { return ((Touch) m_touch).radius; } set { Fields["m_Radius"].SetValue(m_touch, value); } }

        /// <summary> The amount that the radius varies by for a touch </summary>
        public float RadiusVariance { get { return ((Touch) m_touch).radiusVariance; } set { Fields["m_RadiusVariance"].SetValue(m_touch, value); } }

        #endregion
        
        #region Public Methods

        /// <summary> Returns the simulated Touch struct </summary>
        public Touch Get() { return (Touch) m_touch; }
        
        #endregion
    }
}