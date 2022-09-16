// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Engine.Extensions;
using Doozy.Engine.Touchy;
using UnityEngine;

namespace Doozy.Engine.UI
{
    /// <summary>
    ///     Settings for the animated arrow used to show the open and close directions for an UIDrawer
    /// </summary>
    [Serializable]
    public class UIDrawerArrow
    {
        #region Constants

        private const bool DEFAULT_ENABLED = true;
        private const bool DEFAULT_OVERRIDE_COLOR = false;
        private const float DEFAULT_SCALE = 1f;

        #endregion

        #region Static Properties

        /// <summary> Returns the default arrow color when the drawer is opened </summary>
        private static readonly Color DefaultOpenedColor = Color.white;

        /// <summary> Returns the default arrow color when the drawer is closed </summary>
        private static readonly Color DefaultClosedColor = Color.white;

        #endregion

        #region Public Variables

        /// <summary> Animation engine for the UIDrawerArrow </summary>
        public UIDrawerArrowAnimator Animator;

        /// <summary> If OverrideColor is TRUE, the UIDrawerArrow's Image color, for when the UIDrawer is closed, will get overridden by this value </summary>
        public Color ClosedColor;

        /// <summary> RectTransform container that contains all the UIDrawerArrow.Holders </summary>
        public RectTransform Container;

        /// <summary> If the UIDrawer closes downwards, then this UIDrawerArrow.Holder's root will become the UIDrawerArrow's new parent </summary>
        public Holder Down;

        /// <summary> Determines if the UIDrawerArrow is enabled or not </summary>
        public bool Enabled;

        /// <summary> If the UIDrawer closes to the left, then this UIDrawerArrow.Holder's root will become the UIDrawerArrow's new parent </summary>
        public Holder Left;

        /// <summary> If OverrideColor is TRUE, the UIDrawerArrow's Image color, for when the UIDrawer is opened, will get overridden by this value </summary>
        public Color OpenedColor;

        /// <summary> If TRUE, the UIDrawerArrow color will get interpolated between ClosedColor and OpenedColor </summary>
        public bool OverrideColor;

        /// <summary> If the UIDrawer closes to the right, then this UIDrawerArrow.Holder's root will become the UIDrawerArrow's new parent </summary>
        public Holder Right;

        /// <summary>
        ///     Scale variable that overrides the scale of the UIDrawerArrow at runtime.
        ///     <para />
        ///     This will set the localScale values of the UIDrawerArrow. (only overrides localScale.x and localScale.y)
        /// </summary>
        public float Scale;

        /// <summary> If the UIDrawer closes upwards, then this UIDrawerArrow.Holder's root will become the UIDrawerArrow's new parent </summary>
        public Holder Up;

        #endregion

        #region Constructors

        /// <summary> Initializes a new instance of the class </summary>
        public UIDrawerArrow() { Reset(); }

        #endregion

        #region Public Methods

        /// <summary> Returns the UIDrawerArrow.Holder of the UIDrawerArrow's opened and closed positions for the target direction </summary>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public Holder GetHolder(SimpleSwipe closeDirection)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:  return Left;
                case SimpleSwipe.Right: return Right;
                case SimpleSwipe.Up:    return Up;
                case SimpleSwipe.Down:  return Down;
                default:                return null;
            }
        }

        /// <summary> Resets this instance to the default values </summary>
        private void Reset()
        {
            Enabled = DEFAULT_ENABLED;
            Scale = DEFAULT_SCALE;
            OverrideColor = DEFAULT_OVERRIDE_COLOR;
            OpenedColor = DefaultOpenedColor;
            ClosedColor = DefaultClosedColor;

            Left = new Holder(Container);
            Right = new Holder(Container);
            Up = new Holder(Container);
            Down = new Holder(Container);
        }

        /// <summary> Resets the closed UIDrawerArrow position to its default value </summary>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public void ResetArrowClosedPosition(SimpleSwipe closeDirection)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    ResetArrowClosedPosition(Left.Closed, closeDirection);
                    break;
                case SimpleSwipe.Right:
                    ResetArrowClosedPosition(Right.Closed, closeDirection);
                    break;
                case SimpleSwipe.Up:
                    ResetArrowClosedPosition(Up.Closed, closeDirection);
                    break;
                case SimpleSwipe.Down:
                    ResetArrowClosedPosition(Down.Closed, closeDirection);
                    break;
            }
        }

        /// <summary> Resets the closed UIDrawerArrow position to its default value </summary>
        /// <param name="closed"> The closed arrow position </param>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public static void ResetArrowClosedPosition(RectTransform closed, SimpleSwipe closeDirection)
        {
            closed.Center(false);
            closed.localPosition = Vector3.zero;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    closed.anchoredPosition = new Vector2(50, 0);
                    break;
                case SimpleSwipe.Right:
                    closed.anchoredPosition = new Vector2(-50, 0);
                    break;
                case SimpleSwipe.Up:
                    closed.anchoredPosition = new Vector2(0, -50);
                    break;
                case SimpleSwipe.Down:
                    closed.anchoredPosition = new Vector2(0, 50);
                    break;
            }
        }

        /// <summary> Resets the opened UIDrawerArrow position to its default value </summary>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public void ResetArrowOpenedPosition(SimpleSwipe closeDirection)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    ResetArrowOpenedPosition(Left.Opened, closeDirection);
                    break;
                case SimpleSwipe.Right:
                    ResetArrowOpenedPosition(Right.Opened, closeDirection);
                    break;
                case SimpleSwipe.Up:
                    ResetArrowOpenedPosition(Up.Opened, closeDirection);
                    break;
                case SimpleSwipe.Down:
                    ResetArrowOpenedPosition(Down.Opened, closeDirection);
                    break;
            }
        }

        /// <summary> Resets the opened UIDrawerArrow position to its default value </summary>
        /// <param name="opened"> The opened arrow position </param>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public static void ResetArrowOpenedPosition(RectTransform opened, SimpleSwipe closeDirection)
        {
            opened.Center(false);
            opened.localPosition = Vector3.zero;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    opened.anchoredPosition = new Vector2(-50, 0);
                    break;
                case SimpleSwipe.Right:
                    opened.anchoredPosition = new Vector2(50, 0);
                    break;
                case SimpleSwipe.Up:
                    opened.anchoredPosition = new Vector2(0, 50);
                    break;
                case SimpleSwipe.Down:
                    opened.anchoredPosition = new Vector2(0, -50);
                    break;
            }
        }

        /// <summary> Resets the UIDrawerArrow root to the default position </summary>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public void ResetArrowRootPosition(SimpleSwipe closeDirection)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    ResetArrowRootPosition(Left.Root, closeDirection);
                    break;
                case SimpleSwipe.Right:
                    ResetArrowRootPosition(Right.Root, closeDirection);
                    break;
                case SimpleSwipe.Up:
                    ResetArrowRootPosition(Up.Root, closeDirection);
                    break;
                case SimpleSwipe.Down:
                    ResetArrowRootPosition(Down.Root, closeDirection);
                    break;
            }
        }

        /// <summary> Resets the UIDrawerArrow root to the default position </summary>
        /// <param name="root"> The arrow holder </param>
        /// <param name="closeDirection"> The UIDrawer close direction </param>
        public static void ResetArrowRootPosition(RectTransform root, SimpleSwipe closeDirection)
        {
            root.localScale = Vector3.one;
            root.transform.localPosition = Vector3.zero;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (closeDirection)
            {
                case SimpleSwipe.Left:
                    root.anchorMin = new Vector2(1f, 0.5f);
                    root.anchorMax = new Vector2(1f, 0.5f);
                    break;
                case SimpleSwipe.Right:
                    root.anchorMin = new Vector2(0f, 0.5f);
                    root.anchorMax = new Vector2(0f, 0.5f);
                    break;
                case SimpleSwipe.Up:
                    root.anchorMin = new Vector2(0.5f, 0f);
                    root.anchorMax = new Vector2(0.5f, 0f);
                    break;
                case SimpleSwipe.Down:
                    root.anchorMin = new Vector2(0.5f, 1f);
                    root.anchorMax = new Vector2(0.5f, 1f);
                    break;
            }

            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = Vector2.zero;
            root.anchoredPosition = Vector2.zero;
        }

        #endregion

        #region Classes

        /// <summary>
        ///     Helper class used to manage the UIDrawerArrow positions around the UIDrawerContainer
        /// </summary>
        [Serializable]
        public class Holder
        {
            #region Properties

            /// <summary> Returns the Closed localPosition </summary>
            public Vector3 ClosedLocalPosition { get { return Closed.localPosition; } }

            /// <summary> Returns the Opened localPosition </summary>
            public Vector3 OpenedLocalPosition { get { return Opened.localPosition; } }

            #endregion

            #region Public Variables

            /// <summary> The position where the arrow should move to if the drawer is set to close and this holder is active </summary>
            public RectTransform Closed;

            /// <summary> The position where the arrow should move to if the drawer is set to open and this holder is active </summary>
            public RectTransform Opened;

            /// <summary> The RectTransform that is used as the arrow's parent if this holder is active </summary>
            public RectTransform Root;

            #endregion

            #region Constructors

            /// <summary> Resets this instance to the default values </summary>
            /// <param name="parent"> Target parent </param>
            public Holder(RectTransform parent) { Reset(parent); }

            #endregion

            #region Public Methods

            // ReSharper disable once MemberCanBePrivate.Global
            /// <summary> Resets this instance to the default values </summary>
            /// <param name="parent"> Target parent </param>
            public void Reset(RectTransform parent)
            {
                if (Root == null) return;
                if (parent != null) Root.SetParent(parent);
                if (Opened != null) Opened.SetParent(Root);
                if (Closed != null) Closed.SetParent(Root);
            }

            #endregion
        }

        #endregion
    }
}