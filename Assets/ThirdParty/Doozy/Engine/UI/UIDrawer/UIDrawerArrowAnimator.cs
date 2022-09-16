// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Touchy;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.UI
{
    /// <inheritdoc />
    /// <summary>
    ///     Animates the UIDrawerArrow to follow the UIDrawer
    /// </summary>
    public class UIDrawerArrowAnimator : MonoBehaviour
    {
        #region Constants

        private const float CLOSED_DRAWER_VELOCITY = 0.75f;
        private const float MAX_BAR_ROTATION = 45f;
        private const float ROTATION_SPEED = 10f;

        #endregion

        #region Properties

        /// <summary> The UIDrawer that that this animator belongs to </summary>
        public UIDrawer Drawer { get; private set; }

        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();

                return m_rectTransform;
            }
        }

        /// <summary> Rotator RectTransform width </summary>
        public float Width { get; private set; }

        /// <summary> Rotator RectTransform height </summary>
        public float Height { get; private set; }

        // ReSharper disable once UnusedMember.Global
        /// <summary> Rotator RectTransform size </summary>
        public Vector2 Size { get { return new Vector2(Width, Height); } }

        #endregion

        #region Public Variables

        /// <summary> Reference to the parent of the LeftBar and RightBar RectTransforms (the arrow component that actually rotates) </summary>
        public RectTransform Rotator;

        /// <summary> Reference to the RectTransform for the left bar </summary>
        public RectTransform LeftBar;

        /// <summary> Reference to the RectTransform for the right bar </summary>
        public RectTransform RightBar;

        #endregion

        #region Private Variables

        /// <summary> Internal variable that holds a reference to the RectTransform component </summary>
        private RectTransform m_rectTransform;

        /// <summary> Internal variable that holds a reference to the left bar Image component </summary>
        private Image m_leftBarImage;

        /// <summary> Internal variable that holds a reference to the right bar Image component </summary>
        private Image m_rightBarImage;

        /// <summary> Internal variable that keeps track of the UIDrawer velocity </summary>
        private float m_velocity;

        private Vector3[] m_rotatorCorners = new Vector3[4];
        private Vector3[] m_drawerCorners = new Vector3[4];
        private float m_rotatorDisableThreshold = 0.6f;
        private Vector3[] m_tempCorners = new Vector3[4]; //avoid GC
        private Rect m_rotatorRect;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (LeftBar != null) m_leftBarImage = LeftBar.GetComponent<Image>();

            if (RightBar != null) m_rightBarImage = RightBar.GetComponent<Image>();

            UpdateSize();
        }

        #endregion

        #region Public Methods

        /// <summary> Adjusts the target RectTransform localEulerAngles according to the corners array </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="corners"> Corners array </param>
        public Vector3[] AdjustCornersToIdentityRotation(RectTransform target, Vector3[] corners)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (target.localEulerAngles.z == 0) //target is not rotated
                return corners;

            if (target.localEulerAngles.z > 0 && target.localEulerAngles.z <= 90)
            {
                // rotated view of the corners that need to be 'adjusted' to the base view - the numbers represent the vector index
                // 2 - 3
                // |   |
                // 1 - 0
                m_tempCorners[0] = corners[1];
                m_tempCorners[1] = corners[2];
                m_tempCorners[2] = corners[3];
                m_tempCorners[3] = corners[0];
                return m_tempCorners;
            }

            if (target.localEulerAngles.z > 90 && target.localEulerAngles.z <= 180)
            {
                // rotated view of the corners that need to be 'adjusted' to the base view - the numbers represent the vector index
                // 3 - 0
                // |   |
                // 2 - 1
                m_tempCorners[0] = corners[2];
                m_tempCorners[1] = corners[3];
                m_tempCorners[2] = corners[0];
                m_tempCorners[3] = corners[1];
                return m_tempCorners;
            }

            if (target.localEulerAngles.z > 180 && target.localEulerAngles.z <= 270)
            {
                // rotated view of the corners that need to be 'adjusted' to the base view - the numbers represent the vector index
                // 0 - 1
                // |   |
                // 3 - 2
                m_tempCorners[0] = corners[3];
                m_tempCorners[1] = corners[0];
                m_tempCorners[2] = corners[1];
                m_tempCorners[3] = corners[2];
                return m_tempCorners;
            }

            return corners;
        }

        /// <summary> Sets the target UIDrawer that this animator belongs to </summary>
        /// <param name="drawer"> Target UIDrawer</param>
        public void SetTargetDrawer(UIDrawer drawer)
        {
            Drawer = drawer;
            RotateAndMoveArrowToMatchDrawerDirection(Drawer);
        }

        public void UpdateArrow()
        {
//            if (Drawer == null) return;

            if (!Drawer.IsDragged && !Drawer.IsClosing && !Drawer.IsOpening) //if the target UIDrawer is not dragged, is not closing or is not opening -> set the arrow to point in the open or close direction (to be used as a visual guide)
            {
                if (Drawer.CloseDirection == SimpleSwipe.Left || Drawer.CloseDirection == SimpleSwipe.Up)
                    m_velocity = Drawer.Closed ? -CLOSED_DRAWER_VELOCITY : CLOSED_DRAWER_VELOCITY;
                else if (Drawer.CloseDirection == SimpleSwipe.Right || Drawer.CloseDirection == SimpleSwipe.Down) m_velocity = Drawer.Closed ? CLOSED_DRAWER_VELOCITY : -CLOSED_DRAWER_VELOCITY;
            }
            else //this drawer is being dragged -> make the arrow read to the drawer's movements
            {
                if (Drawer.CloseDirection == SimpleSwipe.Left || Drawer.CloseDirection == SimpleSwipe.Right)
                    m_velocity = -Drawer.Container.Velocity.x;
                else if (Drawer.CloseDirection == SimpleSwipe.Up || Drawer.CloseDirection == SimpleSwipe.Down) m_velocity = Drawer.Container.Velocity.y;

                m_velocity /= Time.unscaledDeltaTime;
                m_velocity /= 1000;

                m_velocity = m_velocity > 0 && m_velocity < 0.05f ? 0 : m_velocity;
                m_velocity = m_velocity < 0 && m_velocity > -0.05f ? 0 : m_velocity;
                m_velocity = m_velocity > 0 && m_velocity > 1f ? 1 : m_velocity;
                m_velocity = m_velocity < 0 && m_velocity < -1f ? -1 : m_velocity;
            }

            Vector3 leftBarLocalEulerAngles = LeftBar.localEulerAngles;
            leftBarLocalEulerAngles = new Vector3(leftBarLocalEulerAngles.x,
                                                  leftBarLocalEulerAngles.y,
                                                  Mathf.LerpAngle(leftBarLocalEulerAngles.z,
                                                                  Mathf.Clamp(MAX_BAR_ROTATION * m_velocity, -MAX_BAR_ROTATION, MAX_BAR_ROTATION),
                                                                  ROTATION_SPEED * Time.unscaledDeltaTime));

            LeftBar.localEulerAngles = leftBarLocalEulerAngles;
//            LeftBar.localRotation = Quaternion.Euler(leftBarLocalEulerAngles);

            Vector3 rightBarLocalEulerAngles = RightBar.localEulerAngles;
            rightBarLocalEulerAngles = new Vector3(rightBarLocalEulerAngles.x,
                                                   rightBarLocalEulerAngles.y,
                                                   Mathf.LerpAngle(rightBarLocalEulerAngles.z,
                                                                   Mathf.Clamp(-MAX_BAR_ROTATION * m_velocity, -MAX_BAR_ROTATION, MAX_BAR_ROTATION),
                                                                   ROTATION_SPEED * Time.unscaledDeltaTime));

            RightBar.localEulerAngles = rightBarLocalEulerAngles;
//            RightBar.localRotation = Quaternion.Euler(rightBarLocalEulerAngles);
        }

        /// <summary> Updates the UIDrawerArrow color according to the UIDrawer's settings </summary>
        /// <param name="drawer"> UIDrawer that controls this animator </param>
        public void UpdateArrowColor(UIDrawer drawer)
        {
            if (!drawer.Arrow.OverrideColor) return;

            if (LeftBar == null || RightBar == null) return;

            Color color = Color.white;
            if (drawer.IsDragged)
                color = Color.Lerp(drawer.Arrow.ClosedColor, drawer.Arrow.OpenedColor, drawer.VisibilityProgress);
            else
                switch (drawer.Visibility)
                {
                    case VisibilityState.Visible:
                        color = drawer.Arrow.OpenedColor;
                        break;
                    case VisibilityState.Showing:
                        color = Color.Lerp(drawer.Arrow.ClosedColor, drawer.Arrow.OpenedColor, drawer.VisibilityProgress);
                        break;
                    case VisibilityState.Hiding:
                        color = Color.Lerp(drawer.Arrow.OpenedColor, drawer.Arrow.ClosedColor, 1 - drawer.VisibilityProgress);
                        break;
                    case VisibilityState.NotVisible:
                        color = drawer.Arrow.ClosedColor;
                        break;
                }

            if (m_leftBarImage != null) m_leftBarImage.color = color;
            if (m_rightBarImage != null) m_rightBarImage.color = color;
        }

        // ReSharper disable once UnusedMember.Global
        /// <summary> Updates the RectTransform localScale to the given value </summary>
        /// <param name="scale"> New localScale value </param>
        public void UpdateLocalScale(Vector3 scale)
        {
            RectTransform.localScale = new Vector3(scale.x, scale.y, 1);
            UpdateSize();
        }

        /// <summary> Updates the RectTransform localScale to the given value </summary>
        /// <param name="scale"> New localScale value </param>
        public void UpdateLocalScale(float scale)
        {
            RectTransform.localScale = new Vector3(scale, scale, 1);
            UpdateSize();
        }

        /// <summary> Depending on the UIDrawer visibility, the Rotator RectTransform will get updated accordingly </summary>
        /// <param name="visibility"> UIDrawer visibility </param>
        public void UpdateRotatorPosition(float visibility)
        {
            //this fixes the flickering made when changing the rotator positions when out of view (issue found on Android build)
            //what we did was to disable the rotator when it was moved 40% out of view (thus the rotatorDisableThreshold value below)
            Rotator.GetWorldCorners(m_rotatorCorners);
            m_rotatorCorners = AdjustCornersToIdentityRotation(Rotator, m_rotatorCorners);
            Drawer.RectTransform.GetWorldCorners(m_drawerCorners);

            // base view of the corners - the numbers represent the vector index
            // 1 - 2
            // |   |
            // 0 - 3

            switch (Drawer.CloseDirection)
            {
                case SimpleSwipe.Left:
                    Rotator.gameObject.SetActive(!(m_rotatorCorners[2].x <= m_drawerCorners[1].x + Vector3.Distance(m_rotatorCorners[2], m_rotatorCorners[1]) * m_rotatorDisableThreshold)); //disable the rotator gameObject if it moved too much to the side of the canvas
                    Rotator.localPosition = Vector3.Lerp(Drawer.Arrow.Left.ClosedLocalPosition, Drawer.Arrow.Left.OpenedLocalPosition, 1 - visibility);                                      //move the rotator accordingly
                    break;
                case SimpleSwipe.Right:
                    Rotator.gameObject.SetActive(!(m_rotatorCorners[1].x >= m_drawerCorners[2].x - Vector3.Distance(m_rotatorCorners[1], m_rotatorCorners[2]) * m_rotatorDisableThreshold)); //disable the rotator gameObject if it moved too much to the side of the canvas
                    Rotator.localPosition = Vector3.Lerp(Drawer.Arrow.Right.ClosedLocalPosition, Drawer.Arrow.Right.OpenedLocalPosition, 1 - visibility);                                    //move the rotator accordingly
                    break;
                case SimpleSwipe.Up:
                    Rotator.gameObject.SetActive(!(m_rotatorCorners[0].y >= m_drawerCorners[1].y - Vector3.Distance(m_rotatorCorners[0], m_rotatorCorners[1]) * m_rotatorDisableThreshold)); //disable the rotator gameObject if it moved too much to the side of the canvas
                    Rotator.localPosition = Vector3.Lerp(Drawer.Arrow.Up.ClosedLocalPosition, Drawer.Arrow.Up.OpenedLocalPosition, 1 - visibility);                                          //move the rotator accordingly
                    break;
                case SimpleSwipe.Down:
                    Rotator.gameObject.SetActive(!(m_rotatorCorners[1].y <= m_drawerCorners[0].y + Vector3.Distance(m_rotatorCorners[1], m_rotatorCorners[0]) * m_rotatorDisableThreshold)); //disable the rotator gameObject if it moved too much to the side of the canvas
                    Rotator.localPosition = Vector3.Lerp(Drawer.Arrow.Down.ClosedLocalPosition, Drawer.Arrow.Down.OpenedLocalPosition, 1 - visibility);                                      //move the rotator accordingly
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary> Rotates the Rotator according to the UIDrawer's settings </summary>
        /// <param name="drawer"> UIDrawer that controls this animator </param>
        private void RotateAndMoveArrowToMatchDrawerDirection(UIDrawer drawer)
        {
            Rotator.localRotation = Quaternion.identity;
            switch (drawer.CloseDirection)
            {
                case SimpleSwipe.Left:
                    RectTransform.SetParent(drawer.Arrow.Left.Root);
                    Rotator.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case SimpleSwipe.Right:
                    RectTransform.SetParent(drawer.Arrow.Right.Root);
                    Rotator.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case SimpleSwipe.Up:
                    RectTransform.SetParent(drawer.Arrow.Up.Root);
                    Rotator.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case SimpleSwipe.Down:
                    RectTransform.SetParent(drawer.Arrow.Down.Root);
                    Rotator.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
            }

            RectTransform.anchoredPosition = Vector2.zero;
            UpdateLocalScale(drawer.Arrow.Scale);

            UpdateArrowColor(drawer);
        }

        /// <summary>
        ///     Updates the Width and Height according to the Rotator.rect
        /// </summary>
        private void UpdateSize()
        {
            m_rotatorRect = Rotator.rect;
            Width = m_rotatorRect.width;
            Height = m_rotatorRect.height;
        }

        #endregion
    }
}