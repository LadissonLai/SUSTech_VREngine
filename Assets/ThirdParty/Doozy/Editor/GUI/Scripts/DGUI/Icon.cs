// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Globalization;
using Doozy.Engine.Utils;
using UnityEngine;

namespace Doozy.Editor
{
    public static partial class DGUI
    {
        public static class Icon
        {
            public static GUIStyle Action { get { return Styles.GetStyle(Styles.StyleName.IconAction); } }
            public static GUIStyle ActionFinished { get { return Styles.GetStyle(Styles.StyleName.IconActionEnd); } }
            public static GUIStyle ActionStart { get { return Styles.GetStyle(Styles.StyleName.IconActionStart); } }
            public static GUIStyle Animations { get { return Styles.GetStyle(Styles.StyleName.IconAnimations); } }
            public static GUIStyle Animator { get { return Styles.GetStyle(Styles.StyleName.IconAnimator); } }
            public static GUIStyle ArrowDown { get { return Styles.GetStyle(Styles.StyleName.IconFaArrowAltDown); } }
            public static GUIStyle ArrowLeft { get { return Styles.GetStyle(Styles.StyleName.IconFaArrowAltLeft); } }
            public static GUIStyle ArrowRight { get { return Styles.GetStyle(Styles.StyleName.IconFaArrowAltRight); } }
            public static GUIStyle ArrowUp { get { return Styles.GetStyle(Styles.StyleName.IconFaArrowAltUp); } }
            public static GUIStyle Copy { get { return Styles.GetStyle(Styles.StyleName.IconFaCopy); } }
            public static GUIStyle Duplicate { get { return Styles.GetStyle(Styles.StyleName.IconFaClone); } }
            public static GUIStyle Edit { get { return Styles.GetStyle(Styles.StyleName.IconFaEdit); } }
            public static GUIStyle Effect { get { return Styles.GetStyle(Styles.StyleName.IconUIEffect); } }
            public static GUIStyle Fade { get { return Styles.GetStyle(Styles.StyleName.IconFade); } }
            public static GUIStyle GameEvent { get { return Styles.GetStyle(Styles.StyleName.IconGameEvent); } }
            public static GUIStyle Hide { get { return Styles.GetStyle(Styles.StyleName.IconHide); } }
            public static GUIStyle LabelIcon { get { return Styles.GetStyle(Styles.StyleName.IconLabel); } }
            public static GUIStyle Landscape { get { return Styles.GetStyle(Styles.StyleName.IconLandscape); } }
            public static GUIStyle Loop { get { return Styles.GetStyle(Styles.StyleName.IconLoop); } }
            public static GUIStyle LoopAnimation { get { return Styles.GetStyle(Styles.StyleName.IconLoop); } }
            public static GUIStyle Move { get { return Styles.GetStyle(Styles.StyleName.IconMove); } }
            public static GUIStyle New { get { return Styles.GetStyle(Styles.StyleName.IconNew); } }
            public static GUIStyle OnButtonDeselect { get { return Styles.GetStyle(Styles.StyleName.IconOnButtonDeselect); } }
            public static GUIStyle OnButtonSelect { get { return Styles.GetStyle(Styles.StyleName.IconOnButtonSelect); } }
            public static GUIStyle OnClick { get { return Styles.GetStyle(Styles.StyleName.IconButtonClick); } }
            public static GUIStyle OnDoubleClick { get { return Styles.GetStyle(Styles.StyleName.IconButtonDoubleClick); } }
            public static GUIStyle OnLongClick { get { return Styles.GetStyle(Styles.StyleName.IconButtonLongClick); } }
            public static GUIStyle OnPointerDown { get { return Styles.GetStyle(Styles.StyleName.IconOnPointerDown); } }
            public static GUIStyle OnPointerEnter { get { return Styles.GetStyle(Styles.StyleName.IconOnPointerEnter); } }
            public static GUIStyle OnPointerExit { get { return Styles.GetStyle(Styles.StyleName.IconOnPointerExit); } }
            public static GUIStyle OnPointerUp { get { return Styles.GetStyle(Styles.StyleName.IconOnPointerUp); } }
            public static GUIStyle OrientationDetector { get { return Styles.GetStyle(Styles.StyleName.IconOrientationDetector); } }
            public static GUIStyle Portrait { get { return Styles.GetStyle(Styles.StyleName.IconPortrait); } }
            public static GUIStyle PunchAnimation { get { return Styles.GetStyle(Styles.StyleName.IconPunchAnimation); } }
            public static GUIStyle Refresh { get { return Styles.GetStyle(Styles.StyleName.IconFaRedo); } }
            public static GUIStyle Reset { get { return Styles.GetStyle(Styles.StyleName.IconFaSyncAlt); } }
            public static GUIStyle Rotate { get { return Styles.GetStyle(Styles.StyleName.IconRotate); } }
            public static GUIStyle Save { get { return Styles.GetStyle(Styles.StyleName.IconSave); } }
            public static GUIStyle Scale { get { return Styles.GetStyle(Styles.StyleName.IconScale); } }
            public static GUIStyle Search { get { return Styles.GetStyle(Styles.StyleName.IconFaBinoculars); } }
            public static GUIStyle Show { get { return Styles.GetStyle(Styles.StyleName.IconShow); } }
            public static GUIStyle SortAlphaDown { get { return Styles.GetStyle(Styles.StyleName.IconFaSortAlphaDown); } }
            public static GUIStyle Sound { get { return Styles.GetStyle(Styles.StyleName.IconSound); } }
            public static GUIStyle StateAnimation { get { return Styles.GetStyle(Styles.StyleName.IconStateAnimation); } }
            public static GUIStyle ToggleOff { get { return Styles.GetStyle(Styles.StyleName.IconFaSquare); } }
            public static GUIStyle ToggleOn { get { return Styles.GetStyle(Styles.StyleName.IconFaCheckSquare); } }
            public static GUIStyle Touchy { get { return Styles.GetStyle(Styles.StyleName.IconTouchy); } }
            public static GUIStyle UnityEvent { get { return Styles.GetStyle(Styles.StyleName.IconUnityEvent); } }


            public static void Draw(Rect rect, GUIStyle icon, DColor dColor) { Draw(rect, icon, Colors.IconColor(dColor)); }
            public static void Draw(Rect rect, GUIStyle icon, ColorName colorName) { Draw(rect, icon, Colors.IconColor(colorName)); }

            public static void Draw(Rect rect, GUIStyle icon, Color color)
            {
                Color initialColor = GUI.color;
                GUI.color = color;
                Draw(rect, icon);
                GUI.color = initialColor;
            }

            public static void Draw(Rect rect, GUIStyle icon)
            {
                float textureWidth = icon.normal.background.width;
                float textureHeight = icon.normal.background.height;
                float iconHeight = rect.height;
                float iconWidth = rect.height * textureWidth / textureHeight;
                var adjustedRect = new Rect(rect.x + (rect.width - iconWidth) / 2, rect.y + (rect.height - iconHeight) / 2, iconWidth, iconHeight);
                GUI.Label(adjustedRect, GUIContent.none, icon);
            }


            public static void Draw(GUIStyle icon, float iconSize, float rowHeight)
            {
                float textureWidth = icon.normal.background.width;
                float textureHeight = icon.normal.background.height;

                float iconHeight = iconSize;
                float iconWidth = iconSize * textureWidth / textureHeight;

                GUILayout.BeginVertical(GUILayout.Width(iconSize), GUILayout.Height(rowHeight));
                {
                    GUILayout.Space((rowHeight - iconHeight) / 2);
                    GUILayout.BeginHorizontal(GUILayout.Width(iconSize));
                    {
                        GUILayout.Space((iconHeight - iconWidth) / 2);
                        GUILayout.Label(GUIContent.none, icon, GUILayout.Width(iconWidth), GUILayout.Height(iconHeight));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            public static void Draw(GUIStyle icon, float iconSize, float rowHeight, DColor dColor) { Draw(icon, iconSize, rowHeight, Colors.IconColor(dColor)); }
            public static void Draw(GUIStyle icon, float iconSize, float rowHeight, ColorName colorName) { Draw(icon, iconSize, rowHeight, Colors.IconColor(colorName)); }

            public static void Draw(GUIStyle icon, float iconSize, float rowHeight, Color color)
            {
                Color initialColor = GUI.color;
                GUI.color = color;
                Draw(icon, iconSize, rowHeight);
                GUI.color = initialColor;
            }

            public static void Draw(GUIStyle icon, float iconSize, DColor dColor) { Draw(icon, iconSize, Colors.IconColor(dColor)); }
            public static void Draw(GUIStyle icon, float iconSize, ColorName colorName) { Draw(icon, iconSize, Colors.IconColor(colorName)); }

            public static void Draw(GUIStyle icon, float iconSize, Color color)
            {
                Color initialColor = GUI.color;
                GUI.color = color;
                Draw(icon, iconSize);
                GUI.color = initialColor;
            }

            public static void Draw(GUIStyle icon, float iconSize) { GUILayout.Label(GUIContent.none, icon, GUILayout.Width(iconSize), GUILayout.Height(iconSize)); }

            public static void DrawIconSizeViewTest(GUIStyle iconStyle, float startSize = 8f, float maxSize = 32f, float sizeStep = 2f, float iconSpacing = 8f, float beforeAndAfterLineSpacing = 16f)
            {
                GUILayout.Space(beforeAndAfterLineSpacing);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                float size = startSize;
                while (size <= maxSize)
                {
                    GUILayout.BeginVertical(GUILayout.Width(size));
                    GUILayout.Label(size.ToString(CultureInfo.InvariantCulture), Label.Style(Size.S, TextAlign.Center), GUILayout.Width(size));
                    GUILayout.Space(2);
                    Draw(iconStyle, size);
                    GUILayout.EndVertical();
                    GUILayout.Space(iconSpacing);
                    size += sizeStep;
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.Space(beforeAndAfterLineSpacing);
            }
        }
    }
}