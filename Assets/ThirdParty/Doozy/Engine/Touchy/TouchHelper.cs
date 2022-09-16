// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using UnityEngine;

namespace Doozy.Engine.Touchy
{
    /// <summary> Helper class used to getting the current Input.touches and with an extra SimulatedTouch (by converting a mouse event into a touch event) included </summary>
    public static class TouchHelper
    {
        private static SimulatedTouch s_lastSimulatedTouch;
        private static List<Touch> s_touches;

        /// <summary> Returns a list of current detected touches. This also adds a simulated touch to the returned list, to be able to use the mouse as a finger in the Editor </summary>
        public static List<Touch> GetTouches()
        {
            if (s_touches == null)
                s_touches = new List<Touch>();
            else
                s_touches.Clear();

//            s_touches.AddRange(Input.touches); // check the Input.touches allocates temporary variables

            // fix by Randhall
            int touchCount = Input.touchCount;
            for (int i = 0; i < touchCount; i++)
                s_touches.Add(Input.GetTouch(i));


            // Uncomment if you want it only to allow mouse swipes in the Unity Editor
            //#if UNITY_EDITOR
            if (s_lastSimulatedTouch == null) s_lastSimulatedTouch = new SimulatedTouch();

            if (Input.GetMouseButtonDown(0))
            {
                s_lastSimulatedTouch.WasModified = true;
                s_lastSimulatedTouch.Phase = TouchPhase.Began;
                s_lastSimulatedTouch.DeltaPosition = new Vector2(0, 0);
                s_lastSimulatedTouch.Position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                s_lastSimulatedTouch.FingerId = 0;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                s_lastSimulatedTouch.WasModified = true;
                s_lastSimulatedTouch.Phase = TouchPhase.Ended;
                var newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                s_lastSimulatedTouch.DeltaPosition = newPosition - s_lastSimulatedTouch.Position;
                s_lastSimulatedTouch.Position = newPosition;
                s_lastSimulatedTouch.FingerId = 0;
            }
            else if (Input.GetMouseButton(0))
            {
                s_lastSimulatedTouch.WasModified = true;
                s_lastSimulatedTouch.Phase = TouchPhase.Moved;
                var newPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                s_lastSimulatedTouch.DeltaPosition = newPosition - s_lastSimulatedTouch.Position;
                s_lastSimulatedTouch.Position = newPosition;
                s_lastSimulatedTouch.FingerId = 0;
            }
            else
            {
                s_lastSimulatedTouch = null;
            }

            if (s_lastSimulatedTouch != null && s_lastSimulatedTouch.WasModified)
            {
                s_touches.Add(s_lastSimulatedTouch.Get());
                s_lastSimulatedTouch.WasModified = false;
            }
            // Uncomment if you want it only to allow mouse swipes in the Unity Editor
            //#endif

            return s_touches;
        }
    }
}