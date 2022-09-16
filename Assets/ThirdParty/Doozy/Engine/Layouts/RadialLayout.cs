// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine.Layouts
{
    /// <inheritdoc />
    /// <summary>
    ///     Sets child elements in a radial arrangement
    /// </summary>
    [AddComponentMenu(MenuUtils.RadialLayout_AddComponentMenu_MenuName, MenuUtils.RadialLayout_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.RADIAL_LAYOUT)]
    [RequireComponent(typeof(RectTransform))]
    public class RadialLayout : LayoutGroup
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.RadialLayout_MenuItem_ItemName, false, MenuUtils.RadialLayout_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { CreateRadialLayout(GetParent(menuCommand.context as GameObject)); }
        
        /// <summary> (EDITOR ONLY) Creates a RadialLayout and returns a reference to it </summary>
        public static RadialLayout CreateRadialLayout(GameObject parent)
        {
            var go = new GameObject(MenuUtils.RadialLayout_GameObject_Name, typeof(RectTransform), typeof(RadialLayout));
            GameObjectUtility.SetParentAndAlign(go, parent);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name); //undo option
            Selection.activeObject = go; //select the newly created gameObject
            return go.GetComponent<RadialLayout>();
        }
        
        /// <summary>
        ///     Method used when creating an UIComponent.
        ///     It looks if the selected object has a RectTransform (and returns it as the parent).
        ///     If the selected object is null or does not have a RectTransform, it returns the MasterCanvas GameObject as the parent.
        /// </summary>
        /// <param name="selectedObject"> Selected object </param>
        protected static GameObject GetParent(GameObject selectedObject)
        {
            if (selectedObject == null) return UICanvas.GetMasterCanvas().gameObject; //selected object is null -> returns the MasterCanvas GameObject
            return selectedObject.GetComponent<RectTransform>() != null ? selectedObject : UICanvas.GetMasterCanvas().gameObject;
        }

#endif

        #endregion

        #region Constants

        public const bool AUTO_REBUILD_DEFAULT_VALUE = true;
        public const bool CLOCKWISE_DEFAULT_VALUE = false;
        public const bool CONTROL_CHILD_HEIGHT_DEFAULT_VALUE = false;
        public const bool CONTROL_CHILD_WIDTH_DEFAULT_VALUE = false;
        public const bool RADIUS_CONTROLS_HEIGHT_DEFAULT_VALUE = false;
        public const bool RADIUS_CONTROLS_WIDTH_DEFAULT_VALUE = false;
        public const bool ROTATE_CHILDREN_DEFAULT_VALUE = false;
        public const float CHILD_HEIGHT_DEFAULT_VALUE = RADIUS_DEFAULT_VALUE;
        public const float CHILD_ROTATION_DEFAULT_VALUE = 0f;
        public const float CHILD_WIDTH_DEFAULT_VALUE = RADIUS_DEFAULT_VALUE;
        public const float MAX_ANGLE = 360f;
        public const float MAX_ANGLE_DEFAULT_VALUE = 360f;
        public const float MAX_RADIUS_DEFAULT_VALUE = 1000f;
        public const float MIN_ANGLE = 0f;
        public const float MIN_ANGLE_DEFAULT_VALUE = 0f;
        public const float RADIUS_DEFAULT_VALUE = 100f;
        public const float RADIUS_HEIGHT_FACTOR_DEFAULT_VALUE = 1f;
        public const float RADIUS_WIDTH_FACTOR_DEFAULT_VALUE = 1f;
        public const float SPACING_DEFAULT_VALUE = 0f;
        public const float START_ANGLE_DEFAULT_VALUE = 0f;

        #endregion

        #region Properties

        /// <summary> Automatically rebuild the layout when a parameter has changed and update the layout </summary>
        public bool AutoRebuild
        {
            get { return m_AutoRebuild; }
            set
            {
                if (m_AutoRebuild == value) return;
                m_AutoRebuild = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements height when control child height is enabled </summary>
        public float ChildHeight
        {
            get { return m_ChildHeight; }
            set
            {
                if (m_ChildHeight == value) return;
                m_ChildHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements custom rotation </summary>
        public float ChildRotation
        {
            get { return m_ChildRotation; }
            set
            {
                if (m_ChildRotation == value) return;
                m_ChildRotation = value;
                OnValueChanged();
            }
        }

        /// <summary> Child elements width when control child width is enabled </summary>
        public float ChildWidth
        {
            get { return m_ChildWidth; }
            set
            {
                if (m_ChildWidth == value) return;
                m_ChildWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Order the child elements clockwise and update the layout </summary>
        public bool Clockwise
        {
            get { return m_Clockwise; }
            set
            {
                if (m_Clockwise == value) return;
                m_Clockwise = value;
                OnValueChanged();
            }
        }

        /// <summary> Override the child elements height and update the layout </summary>
        public bool ControlChildHeight
        {
            get { return m_ControlChildHeight; }
            set
            {
                m_ControlChildHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Override the child elements width and update the layout </summary>
        public bool ControlChildWidth
        {
            get { return m_ControlChildWidth; }
            set
            {
                m_ControlChildWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Maximum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        public float MaxAngle
        {
            get { return m_MaxAngle; }
            set
            {
                if (m_MaxAngle == value) return;
                m_MaxAngle = value;
                OnValueChanged();
            }
        }

        /// <summary> Minimum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        public float MinAngle
        {
            get { return m_MinAngle; }
            set
            {
                if (m_MinAngle == value) return;
                m_MinAngle = value;
                OnValueChanged();
            }
        }

        /// <summary> Layout radius that determines the size of the circle </summary>
        public float Radius
        {
            get { return m_Radius; }
            set
            {
                if (m_Radius == value) return;
                m_Radius = value;
                OnValueChanged();
            }
        }

        /// <summary> Set the child elements height to be influenced by the layout radius and update the layout </summary>
        public bool RadiusControlsHeight
        {
            get { return m_RadiusControlsHeight; }
            set
            {
                m_RadiusControlsHeight = value;
                OnValueChanged();
            }
        }

        /// <summary> Set the child elements width to be influenced by the layout radius and update the layout </summary>
        public bool RadiusControlsWidth
        {
            get { return m_RadiusControlsWidth; }
            set
            {
                m_RadiusControlsWidth = value;
                OnValueChanged();
            }
        }

        /// <summary> Factor by which the radius influences the child elements height, if radius controls height is enabled </summary>
        public float RadiusHeightFactor
        {
            get { return m_RadiusHeightFactor; }
            set
            {
                if (m_RadiusHeightFactor == value) return;
                m_RadiusHeightFactor = value;
                OnValueChanged();
            }
        }

        /// <summary> Factor by which the radius influences the child elements width, if the radius controls width is enabled </summary>
        public float RadiusWidthFactor
        {
            get { return m_RadiusWidthFactor; }
            set
            {
                if (m_RadiusWidthFactor == value) return;
                m_RadiusWidthFactor = value;
                OnValueChanged();
            }
        }

        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform RectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        /// <summary> Automatically rotate child elements with the layout, when the start angle changes and update the layout </summary>
        public bool RotateChildren
        {
            get { return m_RotateChildren; }
            set
            {
                m_RotateChildren = value;
                OnValueChanged();
            }
        }

        /// <summary> Extra spacing between child elements </summary>
        public float Spacing
        {
            get { return m_Spacing; }
            set
            {
                if (m_Spacing == value) return;
                m_Spacing = value;
                OnValueChanged();
            }
        }

        /// <summary> Start angle for the first child element of the layout. This places all the child elements around the layout radius </summary>
        public float StartAngle
        {
            get { return m_StartAngle; }
            set
            {
                if (m_StartAngle == value) return;
                m_StartAngle = value;
                OnValueChanged();
            }
        }

        #endregion

        #region Private Variables

        /// <summary> Automatically rebuild the layout when a parameter has changed </summary>
        [SerializeField] protected bool m_AutoRebuild = AUTO_REBUILD_DEFAULT_VALUE;

        /// <summary> Child elements height when control child height is enabled </summary>
        [SerializeField] protected float m_ChildHeight = CHILD_HEIGHT_DEFAULT_VALUE;

        /// <summary> Child elements custom rotation </summary>
        [SerializeField] protected float m_ChildRotation = CHILD_ROTATION_DEFAULT_VALUE;

        /// <summary> Child elements width when control child width is enabled </summary>
        [SerializeField] protected float m_ChildWidth = CHILD_WIDTH_DEFAULT_VALUE;

        /// <summary> Order the child elements clockwise </summary>
        [SerializeField] protected bool m_Clockwise = CLOCKWISE_DEFAULT_VALUE;

        /// <summary> Override the child elements height </summary>
        [SerializeField] protected bool m_ControlChildHeight = CONTROL_CHILD_HEIGHT_DEFAULT_VALUE;

        /// <summary> Override the child elements width </summary>
        [SerializeField] protected bool m_ControlChildWidth = CONTROL_CHILD_WIDTH_DEFAULT_VALUE;

        /// <summary> Maximum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        [Range(MIN_ANGLE, MAX_ANGLE)]
        [SerializeField]
        protected float m_MaxAngle = MAX_ANGLE_DEFAULT_VALUE;

        /// <summary> Internal variable used to set a maximum radius value for the Inspector. This does nothing to affect the radius at runtime </summary>
        [SerializeField] protected float m_MaxRadius = MAX_RADIUS_DEFAULT_VALUE;

        /// <summary> Minimum angle a child element can have inside the layout. Used to make the radial layout look as an arc </summary>
        [Range(MIN_ANGLE, MAX_ANGLE)]
        [SerializeField]
        protected float m_MinAngle = MIN_ANGLE_DEFAULT_VALUE;

        /// <summary> Layout radius that determines the size of the circle </summary>
        [SerializeField] protected float m_Radius = RADIUS_DEFAULT_VALUE;

        /// <summary> Set the child elements height to be influenced by the layout radius </summary>
        [SerializeField] protected bool m_RadiusControlsHeight = RADIUS_CONTROLS_HEIGHT_DEFAULT_VALUE;

        /// <summary> Set the child elements width to be influenced by the layout radius </summary>
        [SerializeField] protected bool m_RadiusControlsWidth = RADIUS_CONTROLS_WIDTH_DEFAULT_VALUE;

        /// <summary> Factor by which the radius influences the child elements height, if radius controls height is enabled </summary>
        [SerializeField] protected float m_RadiusHeightFactor = RADIUS_HEIGHT_FACTOR_DEFAULT_VALUE;

        /// <summary> Factor by which the radius influences the child elements width, if the radius controls width is enabled </summary>
        [SerializeField] protected float m_RadiusWidthFactor = RADIUS_WIDTH_FACTOR_DEFAULT_VALUE;

        /// <summary> Automatically rotate child elements with the layout, when the start angle changes </summary>
        [SerializeField] protected bool m_RotateChildren = ROTATE_CHILDREN_DEFAULT_VALUE;

        /// <summary> Extra spacing between child elements </summary>
        [SerializeField] protected float m_Spacing = SPACING_DEFAULT_VALUE;

        /// <summary> Start angle for the first child element of the layout. This places all the child elements around the layout radius </summary>
        [Range(MIN_ANGLE, MAX_ANGLE)]
        [SerializeField]
        protected float m_StartAngle = START_ANGLE_DEFAULT_VALUE;

        /// <summary> Internal list used to count the number of child elements this layout has. It's main purpose is to improve layout performance by reducing GC </summary>
        private List<RectTransform> m_childList = new List<RectTransform>();

        /// <summary> Internal variable that holds a reference to the RectTransform component </summary>
        private RectTransform m_rectTransform;

        #endregion

        #region Unity Methods

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            CalculateRadial();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateRadial();
        }

        public override void SetLayoutHorizontal() { }

        public override void SetLayoutVertical() { }

        public override void CalculateLayoutInputVertical() { CalculateRadial(); }

        public override void CalculateLayoutInputHorizontal() { CalculateRadial(); }

//#if UNITY_EDITOR
//
//        protected override void OnValidate()
//        {
//            base.OnValidate();
//            CalculateRadial();
//        }
//
//#endif

        #endregion

        #region Public Methods

        /// <summary> Rebuild the layout </summary>
        public void CalculateRadial()
        {
            if (m_childList == null) m_childList = new List<RectTransform>();
            m_childList.Clear();
            int activeChildCount = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                if (child == null) continue;

                var childButton = child.GetComponent<UIButton>();
                if (childButton != null) childButton.UpdateStartValues();

                var childToggle = child.GetComponent<UIToggle>();
                if (childToggle != null) childToggle.UpdateStartValues();

                var childLayout = child.GetComponent<LayoutElement>();
                if (child == null || !child.gameObject.activeSelf || (childLayout != null && childLayout.ignoreLayout)) continue;
                m_childList.Add(child);
                activeChildCount++;
            }

            m_Tracker.Clear();
            if (activeChildCount == 0) return;

            RectTransform.sizeDelta = new Vector2(m_Radius, m_Radius) * 2f;

            float sAngle = 360f / activeChildCount * (activeChildCount - 1f);
            float angleOffset = m_MinAngle;
            if (angleOffset > sAngle) angleOffset = sAngle;
            float maxAngle = 360f - m_MaxAngle;
            if (maxAngle > sAngle) maxAngle = sAngle;
            if (angleOffset > sAngle) angleOffset = sAngle;
            float buff = sAngle - angleOffset;
            float fOffsetAngle = ((buff - maxAngle)) / (activeChildCount - 1f) + m_Spacing;
            float fAngle = m_StartAngle + angleOffset;
            bool controlChildrenSize = m_ControlChildWidth | m_ControlChildHeight;

            DrivenTransformProperties drivenTransformProperties = DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot;
            if (m_ControlChildWidth) drivenTransformProperties |= DrivenTransformProperties.SizeDeltaX;
            if (m_ControlChildHeight) drivenTransformProperties |= DrivenTransformProperties.SizeDeltaY;
            if (m_RotateChildren) drivenTransformProperties |= DrivenTransformProperties.Rotation;

            if (m_Clockwise) fOffsetAngle *= -1f;

            foreach (RectTransform child in m_childList)
            {
                if (child == null || !child.gameObject.activeSelf) continue;                                     //if child is null or not active -> continue
                m_Tracker.Add(this, child, drivenTransformProperties);                                           //add elements to the tracker to stop the user from modifying their positions via the editor
                var vPos = new Vector3(Mathf.Cos(fAngle * Mathf.Deg2Rad), Mathf.Sin(fAngle * Mathf.Deg2Rad), 0); //calculate the child position
                child.localPosition = vPos * m_Radius;                                                           //set the child position
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);                       //force children to be center aligned, to keep all of the objects with the same anchor points

                float elementAngle = m_ChildRotation;
                if (m_RotateChildren) elementAngle += fAngle;
                child.localEulerAngles = new Vector3(0f, 0f, elementAngle);

                if (controlChildrenSize)
                {
                    Vector2 childSizeDelta = child.sizeDelta;

                    if (ControlChildWidth)
                        childSizeDelta.x = m_RadiusControlsWidth
                                               ? m_ChildWidth * m_Radius * m_RadiusWidthFactor / 100
                                               : m_ChildWidth;

                    if (ControlChildHeight)
                        childSizeDelta.y = m_RadiusControlsHeight
                                               ? m_ChildHeight * m_Radius * m_RadiusHeightFactor / 100
                                               : m_ChildHeight;

                    child.sizeDelta = childSizeDelta;
                }

                fAngle += fOffsetAngle;
            }
        }

        #endregion

        #region Private Methods

        private void OnValueChanged()
        {
            if (!m_AutoRebuild) return;
            CalculateRadial();
        }

        #endregion
    }
}