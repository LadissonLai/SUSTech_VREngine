// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System;
using System.Collections.Generic;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Internal
{
    /// <inheritdoc />
    /// <summary> Base class for all Doozy Base Editors </summary>
    public class EditorBase :  UnityEditor.Editor
    {
        #region Constants

        protected const Size NORMAL_BAR_SIZE = Size.L;
        protected const Size NORMAL_TEXT_SIZE = Size.M;

        #endregion
        
        #region Static Properties

        protected static float NormalBarHeight { get { return DGUI.Bar.Height(NORMAL_BAR_SIZE); } }
        protected static UILanguagePack UILabels { get { return UILanguagePack.Instance; } }

        #endregion
        
        #region Properties
        
        protected virtual bool UseCustomRepaintInterval { get { return false; } }
        protected virtual ColorName ComponentColorName { get { return DGUI.Colors.DisabledTextColorName; } }
        protected virtual double CustomRepaintIntervalDuringPlayMode { get { return 0.4f; } }
        protected virtual double CustomRepaintIntervalWhileIdle { get { return 0.6f; } }
        
        #endregion
        
        #region Public Variables

        protected bool LeftMouseButtonIsDown;
        protected Color InitialGUIColor;
        protected double LastRepaintTime;
        protected float InspectorWidth;
        protected Vector2 CurrentMousePosition;

        #endregion
        
        #region Private Variables

        protected readonly Dictionary<string, AnimBool> AnimBools = new Dictionary<string, AnimBool>();
        protected readonly Dictionary<string, InfoMessage> InfoMessages = new Dictionary<string, InfoMessage>();
        protected readonly Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

        #endregion
        
        #region Unity Methods
        
        public override bool RequiresConstantRepaint() { return true; }
        protected virtual bool RepaintOnInspectorUpdate { get { return true; } }
        
        #endregion
        
        #region Public Methods
        
        protected void AddInfoMessage(string key, InfoMessage infoMessage)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log("You cannot add a new InfoMessage with an empty key!");
                return;
            }

            if (InfoMessages.ContainsKey(key))
            {
                Debug.Log("Another InfoMessage, with the key '" + key + "', already exists in the database!");
                return;
            }

            if (infoMessage == null)
            {
                Debug.Log("Cannot add a null InfoMessage, with the key '" + key + "', to the database!");
                return;
            }

            InfoMessages.Add(key, infoMessage);
        }
        
        /// <summary> Calls for a window Repaint when certain Event.current.type happen </summary>
        protected void ContextualRepaint()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                case EventType.MouseUp:
                case EventType.MouseMove:
                case EventType.MouseDrag:
//                case EventType.KeyDown:
//                case EventType.KeyUp:
                case EventType.ScrollWheel:
//                case EventType.DragUpdated:
//                case EventType.DragPerform:
//                case EventType.DragExited:
//                case EventType.Ignore:
                case EventType.Used:
//                case EventType.ValidateCommand:
//                case EventType.ExecuteCommand:
//                case EventType.ContextClick:
                case EventType.MouseEnterWindow:
                case EventType.MouseLeaveWindow:
                    Repaint();
                    break;
            }
        }
        
        protected AnimBool GetAnimBool(string key, bool defaultValue = false)
        {
            if (AnimBools.ContainsKey(key)) return AnimBools[key];
            AnimBools.Add(key, new AnimBool(defaultValue, Repaint));
            return AnimBools[key];
        }

        protected InfoMessage GetInfoMessage(string key)
        {
            return InfoMessages.ContainsKey(key)
                       ? InfoMessages[key]
                       : new InfoMessage(InfoMessage.MessageType.Error, "Missing Message!!!", true, Repaint);
        }

        protected SerializedProperty GetProperty(PropertyName propertyName) { return GetProperty(propertyName.ToString()); }
        protected SerializedProperty GetProperty(PropertyName propertyName, SerializedProperty parentProperty) { return GetProperty(propertyName.ToString(), parentProperty); }

        protected SerializedProperty GetProperty(string propertyName)
        {
            string key = propertyName;
            if (SerializedProperties.ContainsKey(key)) return SerializedProperties[key];
            SerializedProperty s = serializedObject.FindProperty(propertyName);
            if (s == null) return null;
            SerializedProperties.Add(key, s);
            return s;
        }

        protected SerializedProperty GetProperty(string propertyName, SerializedProperty parentProperty)
        {
            string key = parentProperty.propertyPath + "." + propertyName;
            if (SerializedProperties.ContainsKey(key)) return SerializedProperties[key];
            SerializedProperty s = parentProperty.FindPropertyRelative(propertyName);
            if (s == null) return null;
            SerializedProperties.Add(key, s);
            return s;
        }
        
    
        
        #endregion

        #region Static Methods

        protected static Vector3 AdjustToRoundValues(Vector3 v3, int maximumAllowedDecimals = 3)
        {
            return new Vector3(RoundToIntIfNeeded(v3.x, maximumAllowedDecimals),
                               RoundToIntIfNeeded(v3.y, maximumAllowedDecimals),
                               RoundToIntIfNeeded(v3.z, maximumAllowedDecimals));
        }

        protected static void AdjustPositionRotationAndScaleToRoundValues(RectTransform rectTransform)
        {
            rectTransform.anchoredPosition3D = AdjustToRoundValues(rectTransform.anchoredPosition3D);
            rectTransform.localEulerAngles = AdjustToRoundValues(rectTransform.localEulerAngles);
            rectTransform.localScale = AdjustToRoundValues(rectTransform.localScale);
        }

         protected static void DrawHeader(GUIStyle headerStyle)
        {
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Label(GUIContent.none, headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(DGUI.Properties.Space(2));
        }

        protected static void DrawHeader(GUIStyle headerStyle, string componentName)
        {
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Label(GUIContent.none, headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(-DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                DGUI.Doozy.DrawSettingsButton(componentName);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
        }

        protected static void DrawHeader(GUIStyle headerStyle, string urlManual, string urlYouTube)
        {
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Label(GUIContent.none, headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(-DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                DGUI.Doozy.DrawManualButton(urlManual);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Doozy.DrawYoutubeButton(urlYouTube);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
        }

        protected static void DrawHeader(GUIStyle headerStyle, string componentName, string urlManual, string urlYouTube)
        {
            GUILayout.Space(DGUI.Properties.Space(2));
            GUILayout.Label(GUIContent.none, headerStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(-DGUI.Properties.Space(2));
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                DGUI.Doozy.DrawSettingsButton(componentName);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Doozy.DrawManualButton(urlManual);
                GUILayout.Space(DGUI.Properties.Space());
                DGUI.Doozy.DrawYoutubeButton(urlYouTube);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(DGUI.Properties.Space(2));
        }
        
        
        private static float RoundToIntIfNeeded(float value, int maximumAllowedDecimals = 3)
        {
            int numberOfDecimals = BitConverter.GetBytes(decimal.GetBits((decimal) value)[3])[2];
            return numberOfDecimals >= maximumAllowedDecimals ? Mathf.Round(value) : value;
        }
        
        #endregion
    }

}