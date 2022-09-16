// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressorGroup))]
    public class ProgressorGroupEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorGroupColorName; } }
        private ProgressorGroup m_target;

        private ProgressorGroup Target
        {
            get
            {
                if (m_target != null) return m_target;
                m_target = (ProgressorGroup) target;
                return m_target;
            }
        }

        private SerializedProperty
            m_progressors,
            m_onProgressChanged,
            m_onInverseProgressChanged;

        private AnimBool
            m_onProgressChangedExpanded,
            m_onInverseProgressChangedExpanded;

        private int OnProgressChangedEventCount { get { return Target.OnProgressChanged.GetPersistentEventCount(); } }
        private int OnInverseProgressChangedEventCount { get { return Target.OnInverseProgressChanged.GetPersistentEventCount(); } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_progressors = GetProperty(PropertyName.Progressors);
            m_onProgressChanged = GetProperty(PropertyName.OnProgressChanged);
            m_onInverseProgressChanged = GetProperty(PropertyName.OnInverseProgressChanged);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_onProgressChangedExpanded = GetAnimBool(m_onProgressChanged.propertyPath, OnProgressChangedEventCount > 0);
            m_onInverseProgressChangedExpanded = GetAnimBool(m_onInverseProgressChanged.propertyPath, OnInverseProgressChangedEventCount > 0);
        }

        public override bool RequiresConstantRepaint() { return true; }

        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Target.UpdateProgress();
            
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressorGroup), MenuUtils.ProgressorGroup_Manual, MenuUtils.ProgressorGroup_YouTube);
            DrawDebugMode();
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawProgressGroupProgress();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawProgressors();
            GUILayout.Space(DGUI.Properties.Space(4));
            DrawUnityEvents();
            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProgressGroupProgress()
        {
            DrawProgressInfo(Size.L);
            GUILayout.Space(DGUI.Properties.Space());
            DrawProgressBar(Target.Progress, DGUI.Properties.Space(4));
            GUILayout.Space(DGUI.Properties.Space());
        }

        private void DrawProgressBar(float progress, float barHeight = -1)
        {
            if(Math.Abs(barHeight - (-1)) < ProgressorGroup.TOLERANCE) barHeight = DGUI.Properties.Space(2);
            GUILayoutOption barHeightOption = GUILayout.Height(barHeight);
            Color backgroundColor = DGUI.Utility.IsProSkin ? Color.black : Color.white;

            GUILayout.BeginVertical(barHeightOption);
            {
                GUI.color = backgroundColor.WithAlpha(0.2f);
                GUILayout.Label(GUIContent.none, DGUI.Properties.White, barHeightOption); //draw progress bar background
                GUILayout.Space(-barHeight);
                GUI.color = InitialGUIColor;
                GUI.color = DGUI.Colors.IconColor(ComponentColorName);
                float progressBarWidth = InspectorWidth * progress;
                GUILayout.Label(GUIContent.none, DGUI.Properties.White, barHeightOption, GUILayout.Width(progressBarWidth)); //draw progress bar (dynamic width)
                GUI.color = InitialGUIColor;
                GUILayout.Space(-barHeight);
            }
            GUILayout.EndVertical();
        }

        private void DrawProgressInfo(Size textSize = Size.M)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                DGUI.Label.Draw(Mathf.Round(Target.Progress * 100) + "%",
                                textSize,
                                ComponentColorName, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

        private void DrawProgressors()
        {
            DGUI.Doozy.DrawTitleWithIcon(Styles.GetStyle(Styles.StyleName.IconProgressor),
                                         UILabels.Progressors, Size.L, DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(3),
                                         ComponentColorName);
            GUILayout.Space(DGUI.Properties.Space());

            GUILayout.BeginVertical();
            {
                float alpha = GUI.color.a;
                float backgroundHeight = DGUI.Properties.Space() + DGUI.Properties.SingleLineHeight + DGUI.Properties.Space();
                if (m_progressors.arraySize > 0)
                    for (int i = 0; i < m_progressors.arraySize; i++)
                    {
                        backgroundHeight += EditorGUI.GetPropertyHeight(m_progressors.GetArrayElementAtIndex(i));
                        backgroundHeight += DGUI.Properties.Space();
                    }

                DGUI.Background.Draw(DGUI.Colors.GetBackgroundColorName(m_progressors.arraySize > 0, ComponentColorName), backgroundHeight);
                GUILayout.Space(-backgroundHeight + DGUI.Properties.Space());

                if (m_progressors.arraySize == 0)
                {
                    GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                    {
                        GUILayout.Space(DGUI.Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                        DGUI.Label.Draw("Empty...", Size.S, DGUI.Colors.DisabledTextColorName, DGUI.Properties.SingleLineHeight);
                        GUI.color = GUI.color.WithAlpha(alpha);
                        GUILayout.FlexibleSpace();
                        if (DGUI.Button.IconButton.Plus(DGUI.Properties.SingleLineHeight)) m_progressors.InsertArrayElementAtIndex(m_progressors.arraySize);
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                    return;
                }

                for (int i = 0; i < m_progressors.arraySize; i++)
                {
                    SerializedProperty childProperty = m_progressors.GetArrayElementAtIndex(i);
                    float propertyHeight = EditorGUI.GetPropertyHeight(childProperty);
                    GUILayout.BeginHorizontal(GUILayout.Height(propertyHeight));
                    {
                        GUILayout.Space(DGUI.Properties.Space(2));
                        GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(false));
                        DGUI.Label.Draw(i.ToString(), Size.S, DGUI.Colors.DisabledTextColorName, propertyHeight);
                        GUI.color = GUI.color.WithAlpha(alpha);
                        GUILayout.Space(DGUI.Properties.Space(2));
                        if (Target.Progressors[i] != null)
                        {
                            GUILayout.Space(DGUI.Properties.Space(2));
                            DGUI.Label.Draw(Mathf.Round(Target.Progressors[i].Progress * 100) + "%", Size.S, ComponentColorName, propertyHeight);
                        }
                        DGUI.Property.Draw(childProperty);
                        if (DGUI.Button.IconButton.Minus(propertyHeight)) m_progressors.DeleteArrayElementAtIndex(i);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(DGUI.Properties.Space());
                }

                GUILayout.BeginHorizontal(GUILayout.Height(DGUI.Properties.SingleLineHeight));
                {
                    GUILayout.FlexibleSpace();
                    if (DGUI.Button.IconButton.Plus(DGUI.Properties.SingleLineHeight))
                    {
                        m_progressors.InsertArrayElementAtIndex(m_progressors.arraySize);

                        //Reset the newly created serialized property to its default value 
                        SerializedProperty p = m_progressors.GetArrayElementAtIndex(m_progressors.arraySize - 1);
                        p.objectReferenceValue = default(Object);
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }


        private void DrawUnityEvents()
        {
            DrawUnityEvent(m_onProgressChangedExpanded, m_onProgressChanged, "OnProgressChanged", OnProgressChangedEventCount);
            GUILayout.Space(DGUI.Properties.Space());
            DrawUnityEvent(m_onInverseProgressChangedExpanded, m_onInverseProgressChanged, "OnInverseProgressChanged", OnInverseProgressChangedEventCount);
        }

        private void DrawUnityEvent(AnimBool expanded, SerializedProperty property, string propertyName, int eventsCount)
        {
            DGUI.Bar.Draw(propertyName, NORMAL_BAR_SIZE, DGUI.Bar.Caret.CaretType.Caret, ComponentColorName, expanded);
            DGUI.Property.UnityEventWithFade(property, expanded, propertyName, ComponentColorName, eventsCount, true, true);
        }
    }
}