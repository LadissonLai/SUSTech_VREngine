// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Engine.Touchy
{
    /// <summary> Structure that stores data related to a single touch instance and is sent throughout the Touchy system as a data package </summary>
    public struct TouchInfo
    {
        /// <summary> Structure describing the status of a finger touching the screen </summary>
        public Touch Touch;

        /// <summary> Swipe direction </summary>
        public Swipe Direction;

        /// <summary> The raw direction, as a Vector2, that describes this touch </summary>
        public Vector2 RawDirection;

        /// <summary> Touch position where this touch started (used to detect gestures) </summary>
        public Vector2 StartPosition;

        /// <summary> Touch position where this touch ended (used to detect gestures) </summary>
        public Vector2 EndPosition;

        /// <summary> How fast the touch moved on both X and Y axes </summary>
        public Vector2 Velocity;

        /// <summary> When did the touch start (used to detect gestures) </summary>
        public float StartTime;

        /// <summary> When did the touch end (used to detect gestures) </summary>
        public float EndTime;

        /// <summary> For how long did the touch happen (used to detect gestures) </summary>
        public float Duration;

        /// <summary> Was this touch a tap (a light touch on a touch-sensitive screen) </summary>
        public bool Tap;

        /// <summary> Was this touch a long tap (a light touch on a touch-sensitive screen, held down down for a second or two) </summary>
        public bool LongTap;

        /// <summary> Distance in the current frame (while the touch is in progress) </summary>
        public float Distance;

        /// <summary> Distance from the start position to the end position </summary>
        public float LongestDistance;

        /// <summary> Reference to the object involved in the touch event </summary>
        public GameObject GameObject;

        /// <summary> Reference to the object currently being dragged in the touch event </summary>
        public GameObject DraggedObject;

        /// <summary> Touch position in the current frame </summary>
        public Vector2 CurrentTouchPosition;

        /// <summary> Touch position in the previous frame </summary>
        public Vector2 PreviousTouchPosition;

        /// <summary> Amount of time that has passed since the last recorded change in Touch values </summary>
        public float TouchDeltaTime;

        /// <summary> Returns TRUE if a GameObject is currently being dragged (DraggedObject != null) </summary>
        public bool IsDragging { get { return DraggedObject != null; } }

        /// <summary> Returns the touch velocity between the current touch position and the previous touch position </summary>
        public Vector2 TouchVelocity { get { return CurrentTouchPosition - PreviousTouchPosition; } }

        /// <summary> Updates the struct data with the given Touch info and GameObject reference </summary>
        /// <param name="touch"> New touch info </param>
        /// <param name="gameObject"> Reference to the GameObject involved in the touch event </param>
        public void Update(Touch touch, GameObject gameObject = null)
        {
            Touch = touch;

            Direction = Swipe.None;
            StartPosition = new Vector2(touch.position.x, touch.position.y);
            EndPosition = StartPosition;
            Velocity = Vector2.zero;
            StartTime = Time.time;
            EndTime = StartTime;
            Duration = 0f;
            Tap = false;
            LongTap = false;
            Distance = 0f;
            LongestDistance = 0f;
            GameObject = gameObject;

            DraggedObject = null;
        }

        /// <summary> Returns a nicely formatted string for this TouchInfo </summary>
        public override string ToString()
        {
            if (Tap) return string.Format("[Tap: At {0}, Time {1:0.00}s]", StartPosition, Duration);
            if (LongTap) return string.Format("[LongTap: At {0}, Time {1:0.00}s]", StartPosition, Duration);
            return string.Format("[Swipe: {0}, From {1}, To {2}, Delta {3}, Time {4:0.00}s]", Direction, RawDirection, StartPosition, EndPosition, Duration);
        }
    }
}