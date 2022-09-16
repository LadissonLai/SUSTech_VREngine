// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor;
using Doozy.Editor.Internal;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.UI.Animation
{
    [CustomPropertyDrawer(typeof(Scale), true)]
    public class ScaleDrawer : BaseAnimationDrawer
    {
        private static DColor DrawerColor { get { return EditorColors.Instance.Red; } }
        private static GUIStyle DrawerIconStyle { get { return Styles.GetStyle(Styles.StyleName.IconScale); } }

        private void Init(SerializedProperty property)
        {
            if (Initialized.ContainsKey(property.propertyPath) && Initialized[property.propertyPath]) return;

            Contents.Add(UILabels.PunchBy);

            Elements.Add(Properties.Add(PropertyName.AnimationType, property), Contents.Add(UILabels.AnimationType));
            Elements.Add(Properties.Add(PropertyName.Enabled, property), Contents.Add(UILabels.Enabled));
            Elements.Add(Properties.Add(PropertyName.From, property), Contents.Add(UILabels.ScaleFrom));
            Elements.Add(Properties.Add(PropertyName.To, property), Contents.Add(UILabels.ScaleTo));
            Elements.Add(Properties.Add(PropertyName.By, property), Contents.Add(UILabels.ScaleBy));
            Elements.Add(Properties.Add(PropertyName.UseCustomFromAndTo, property), Contents.Add(UILabels.UseCustomFromAndTo), 0f, DGUI.Toggle.Checkbox.Width);
            Elements.Add(Properties.Add(PropertyName.Vibrato, property), Contents.Add(UILabels.Vibrato));
            Elements.Add(Properties.Add(PropertyName.Elasticity, property), Contents.Add(UILabels.Elasticity));
            Elements.Add(Properties.Add(PropertyName.NumberOfLoops, property), Contents.Add(UILabels.NumberOfLoops));
            Elements.Add(Properties.Add(PropertyName.LoopType, property), Contents.Add(UILabels.LoopType));
            Elements.Add(Properties.Add(PropertyName.EaseType, property), Contents.Add(UILabels.EaseType));
            Elements.Add(Properties.Add(PropertyName.Ease, property), Contents.Add(UILabels.Ease));
            Elements.Add(Properties.Add(PropertyName.AnimationCurve, property), Contents.Add(UILabels.AnimationCurve));
            Elements.Add(Properties.Add(PropertyName.StartDelay, property), Contents.Add(UILabels.StartDelay));
            Elements.Add(Properties.Add(PropertyName.Duration, property), Contents.Add(UILabels.Duration));

            if (!Initialized.ContainsKey(property.propertyPath))
                Initialized.Add(property.propertyPath, true);
            else
                Initialized[property.propertyPath] = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            Init(property);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            {
                // don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                Color initialColor = GUI.color; //save the GUI color
                DrawSelector(position, property, Properties.Get(PropertyName.Enabled, property).boolValue ? DrawerColor : EditorColors.Instance.Gray);
                GUI.color = initialColor; //restore the GUI color

                // set indent back to what it was
                EditorGUI.indentLevel = indent;

                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        protected override void DrawShow(Rect position, SerializedProperty property, DColor dColor)
        {
            NumberOfLines[property.propertyPath] = 3;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], dColor); //calculate draw rect and draw background

            drawRect = InsertDrawerIcon(drawRect, property, dColor, DrawerIconStyle);
            GUI.color = dColor.Light;

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            DrawLineStartDelayDurationCustomFromAndTo(drawRect, property);

            //LINE 2
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            if (Properties.Get(PropertyName.UseCustomFromAndTo, property).boolValue)
                Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.From, property), 0.5f), Elements.GetLayout(Properties.Get(PropertyName.To, property), 0.5f));
            else
                Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.From, property), 1f));

            //LINE 3
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            DrawLineEaseTypeEaseAnimationCurve(drawRect, property);
        }

        protected override void DrawHide(Rect position, SerializedProperty property, DColor dColor)
        {
            NumberOfLines[property.propertyPath] = 3;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], dColor); //calculate draw rect and draw background

            drawRect = InsertDrawerIcon(drawRect, property, dColor, DrawerIconStyle);
            GUI.color = dColor.Light;

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            DrawLineStartDelayDurationCustomFromAndTo(drawRect, property);

            //LINE 2
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            if (Properties.Get(PropertyName.UseCustomFromAndTo, property).boolValue)
                Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.To, property), 0.5f), Elements.GetLayout(Properties.Get(PropertyName.From, property), 0.5f));
            else
                Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.To, property), 1f));

            //LINE 3
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            DrawLineEaseTypeEaseAnimationCurve(drawRect, property);
        }

        protected override void DrawState(Rect position, SerializedProperty property, DColor dColor)
        {
            NumberOfLines[property.propertyPath] = 3;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], dColor); //calculate draw rect and draw background

            drawRect = InsertDrawerIcon(drawRect, property, dColor, DrawerIconStyle);
            GUI.color = dColor.Light;

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            DrawLineStartDelayAndDuration(drawRect, property);

            //LINE 2
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.By, property), 1f));

            //LINE 3
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            DrawLineEaseTypeEaseAnimationCurve(drawRect, property);
        }

        protected override void DrawPunch(Rect position, SerializedProperty property, DColor dColor)
        {
            NumberOfLines[property.propertyPath] = 2;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], dColor); //calculate draw rect and draw background

            drawRect = InsertDrawerIcon(drawRect, property, dColor, DrawerIconStyle);
            GUI.color = dColor.Light;

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.StartDelay, property), 0.25f), Elements.GetLayout(Properties.Get(PropertyName.Duration, property), 0.25f), Elements.GetLayout(Properties.Get(PropertyName.Vibrato, property), 0.25f), Elements.GetLayout(Properties.Get(PropertyName.Elasticity, property), 0.25f));

            //LINE 2
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.By, property), DGUIElement.DrawMode.LabelAndField));
        }

        protected override void DrawLoop(Rect position, SerializedProperty property, DColor dColor)
        {
            NumberOfLines[property.propertyPath] = 3;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], dColor); //calculate draw rect and draw background

            drawRect = InsertDrawerIcon(drawRect, property, dColor, DrawerIconStyle);
            GUI.color = dColor.Light;

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, 
                              Elements.GetLayout(Properties.Get(PropertyName.StartDelay, property), 0.2f),
                              Elements.GetLayout(Properties.Get(PropertyName.Duration, property), 0.2f),
                              Elements.GetLayout(Properties.Get(PropertyName.NumberOfLoops, property), 0.2f),
                              Elements.GetLayout(Properties.Get(PropertyName.LoopType, property), 0.4f));

            //LINE 2
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, 
                              Elements.GetLayout(Properties.Get(PropertyName.From, property), 0.5f),
                              Elements.GetLayout(Properties.Get(PropertyName.To, property), 0.5f));

            //LINE 3
            drawRect.y += DGUI.Properties.SingleLineHeight + DGUI.Properties.StandardVerticalSpacing;
            DrawLineEaseTypeEaseAnimationCurve(drawRect, property);
        }

        protected override void DrawUndefined(Rect position, SerializedProperty property)
        {
            NumberOfLines[property.propertyPath] = 1;
            Rect drawRect = GetDrawRectAndDrawBackground(position, NumberOfLines[property.propertyPath], EditorColors.Instance.Red); //calculate draw rect and draw background

            //LINE 1
            drawRect.y += DGUI.Properties.StandardVerticalSpacing;
            Elements.DrawLine(drawRect, Elements.GetLayout(Properties.Get(PropertyName.AnimationType, property), DGUIElement.DrawMode.LabelAndField));
        }
    }
}