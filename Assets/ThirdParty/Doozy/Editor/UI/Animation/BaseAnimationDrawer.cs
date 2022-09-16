// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using PropertyName = Doozy.Editor.PropertyName;

namespace Doozy.Editor.UI.Animation
{
    public class BaseAnimationDrawer : BaseDrawer
    {
        private AnimationType m_animationType;

        protected void DrawSelector(Rect position, SerializedProperty property, DColor dColor)
        {
            m_animationType = (AnimationType) Properties.Get(PropertyName.AnimationType, property).enumValueIndex;
            switch (m_animationType)
            {
                case AnimationType.Show:
                    DrawShow(position, property, dColor);
                    break;
                case AnimationType.Hide:
                    DrawHide(position, property, dColor);
                    break;
                case AnimationType.Loop:
                    DrawLoop(position, property, dColor);
                    break;
                case AnimationType.Punch:
                    DrawPunch(position, property, dColor);
                    break;
                case AnimationType.State:
                    DrawState(position, property, dColor);
                    break;
                case AnimationType.Undefined:
                    DrawUndefined(position, property);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void DrawShow(Rect position, SerializedProperty property, DColor dColor) { }
        protected virtual void DrawHide(Rect position, SerializedProperty property, DColor dColor) { }
        protected virtual void DrawLoop(Rect position, SerializedProperty property, DColor dColor) { }
        protected virtual void DrawPunch(Rect position, SerializedProperty property, DColor dColor) { }
        protected virtual void DrawState(Rect position, SerializedProperty property, DColor dColor) { }
        protected virtual void DrawUndefined(Rect position, SerializedProperty property) { }

        protected void DrawLineStartDelayDurationCustomFromAndTo(Rect drawRect, SerializedProperty property)
        {
            Elements.DrawLine(drawRect,
                              Elements.GetLayout(Properties.Get(PropertyName.StartDelay, property), 0.5f),
                              Elements.GetLayout(Properties.Get(PropertyName.Duration, property), 0.5f),
                              Elements.GetLayout(Properties.Get(PropertyName.UseCustomFromAndTo, property), DGUIElement.DrawMode.FieldAndLabel));
        }

        protected void DrawLineStartDelayAndDuration(Rect drawRect, SerializedProperty property)
        {
            Elements.DrawLine(drawRect,
                              Elements.GetLayout(Properties.Get(PropertyName.StartDelay, property), 0.5f),
                              Elements.GetLayout(Properties.Get(PropertyName.Duration, property), 0.5f));
        }

        protected void DrawLineEaseTypeEaseAnimationCurve(Rect drawRect, SerializedProperty property)
        {
            if ((EaseType) Properties.Get(PropertyName.EaseType, property).enumValueIndex == EaseType.Ease)
                Elements.DrawLine(drawRect,
                                  Elements.GetLayout(Properties.Get(PropertyName.EaseType, property), 0.3f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.Ease, property), 0.7f, DGUIElement.DrawMode.Field));
            else
                Elements.DrawLine(drawRect,
                                  Elements.GetLayout(Properties.Get(PropertyName.EaseType, property), 0.3f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.AnimationCurve, property), 0.7f, DGUIElement.DrawMode.Field));
        }

        protected void DrawLineEaseTypeEaseAnimationCurveRotateMode(Rect drawRect, SerializedProperty property)
        {
            SerializedProperty easeType = Properties.Get(PropertyName.EaseType, property);

            if ((EaseType) easeType.enumValueIndex == EaseType.Ease)
                Elements.DrawLine(drawRect,
                                  Elements.GetLayout(easeType, 0.2f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.Ease, property), 0.5f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.RotateMode, property), 0.3f));
            else
                Elements.DrawLine(drawRect,
                                  Elements.GetLayout(easeType, 0.2f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.AnimationCurve, property), 0.5f, DGUIElement.DrawMode.Field),
                                  Elements.GetLayout(Properties.Get(PropertyName.RotateMode, property), 0.3f));
        }
    }
}