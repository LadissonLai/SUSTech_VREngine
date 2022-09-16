// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Engine.Extensions;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Internal
{
    public class BaseDrawer : PropertyDrawer
    {
        #region Static Properties

        /// <summary> Direct reference to the active language pack </summary>
        protected static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion

        #region Public Variables

        protected readonly Dictionary<string, bool> Initialized = new Dictionary<string, bool>();
        protected Dictionary<string, AnimBool> AnimBools = new Dictionary<string, AnimBool>();
        protected readonly Dictionary<string, int> NumberOfLines = new Dictionary<string, int>();

        #endregion

        #region Properties

        protected DPropertyRelative Properties { get; private set; }
        protected DGUIElements Elements { get; private set; }

        protected DGUIContent Contents { get; private set; }


        protected virtual ColorName DrawerColorName { get { return DGUI.Colors.DisabledTextColorName; } }

        #endregion

        #region Private Variables

        private bool m_databasesInitialized;

        #endregion

        #region Unity Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) { InitDatabases(); }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return PropertyHeight(NumberOfLines.ContainsKey(property.propertyPath) ? NumberOfLines[property.propertyPath] : 1); }

        #endregion

        #region Public Methods

        protected Rect InsertDrawerIcon(Rect drawRect, SerializedProperty property, ColorName drawerColorName, GUIStyle iconStyle) { return InsertDrawerIcon(drawRect, property, DGUI.Colors.GetDColor(drawerColorName), iconStyle); }

        protected Rect InsertDrawerIcon(Rect drawRect, SerializedProperty property, DColor drawerColor, GUIStyle iconStyle)
        {
            var verticalHeaderRect = new Rect(drawRect.x, drawRect.y, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), PropertyHeight(NumberOfLines[property.propertyPath]));
            DGUI.Background.Draw(verticalHeaderRect, drawerColor);

            var iconRect = new Rect(verticalHeaderRect.x + DGUI.Properties.Space() * 1.5f,
                                    verticalHeaderRect.center.y - DGUI.Properties.SingleLineHeight / 2,
                                    DGUI.Properties.SingleLineHeight - DGUI.Properties.Space(),
                                    DGUI.Properties.SingleLineHeight - DGUI.Properties.Space());
            Color color = GUI.color;
            GUI.color = (EditorGUIUtility.isProSkin ? drawerColor.Normal : drawerColor.Normal.Darker().Darker().Darker().Darker().Darker()).WithAlpha(GUI.color.a * 0.6f);
            GUI.Label(iconRect, GUIContent.none, iconStyle);
            GUI.color = color;

            drawRect.x += verticalHeaderRect.width + DGUI.Properties.Space();
            drawRect.width -= verticalHeaderRect.width;

            return drawRect;
        }

        protected AnimBool GetAnimBool(string key, bool defaultValue = false)
        {
            if (AnimBools.ContainsKey(key)) return AnimBools[key];
            AnimBools.Add(key, new AnimBool(defaultValue));
            return AnimBools[key];
        }

        #endregion

        #region Private Methods

        private void InitDatabases()
        {
            if (m_databasesInitialized) return;
            AnimBools = new Dictionary<string, AnimBool>();
            Properties = new DPropertyRelative();
            Elements = new DGUIElements();
            Contents = new DGUIContent();
            m_databasesInitialized = true;
        }

        #endregion

        #region Static Methods

        protected static float PropertyHeight(int numberOfLines) { return DGUI.Properties.SingleLineHeight * numberOfLines + DGUI.Properties.StandardVerticalSpacing * (numberOfLines + 1); }

        protected static Rect GetDrawRectAndDrawBackground(Rect position, int numberOfLines, ColorName drawerColorName) { return GetDrawRectAndDrawBackground(position, numberOfLines, DGUI.Colors.GetDColor(drawerColorName)); }

        protected static Rect GetDrawRectAndDrawBackground(Rect position, int numberOfLines, DColor drawerColor)
        {
            //calculate rects
            var rect = new Rect(position.x, position.y, position.width, PropertyHeight(numberOfLines));
            var drawRect = new Rect(position.x, position.y, position.width, DGUI.Properties.SingleLineHeight);

            //draw background
            DGUI.Background.Draw(rect, drawerColor);

            return drawRect;
        }

        protected static Rect GetDrawRect(Rect position)
        {
            //calculate rects
            return new Rect(position.x, position.y, position.width, DGUI.Properties.SingleLineHeight);
        }

        #endregion
    }
}