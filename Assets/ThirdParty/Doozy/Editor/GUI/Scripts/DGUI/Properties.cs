// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor
{
	public static partial class DGUI
	{
		public static class Properties
		{
			public const float INDENT_WIDTH = 12f;
			public const float WINDOW_WINDOW_TAB_HEIGHT = 20f;
			public const float SPACE = 2f;
			public const float ANIM_BOOL_SPEED = 4f;

			public const string UNITY_EVENT_PERSISTENT_CALLS = "m_PersistentCalls.m_Calls";

			private static GUIStyle s_white;

			public static GUIStyle White
			{
				get { return s_white ?? (s_white = new GUIStyle {normal = {background = WhiteTexture}}); }
			}

			public static float CurrentViewWidth
			{
				get { return EditorGUIUtility.currentViewWidth; }
			}

			public static Texture2D WhiteTexture
			{
				get { return EditorGUIUtility.whiteTexture; }
			}

			public static float StandardWindowTabHeight
			{
				get { return WINDOW_WINDOW_TAB_HEIGHT; }
			}

			public static float SingleLineHeight
			{
				get
				{
#if UNITY_2019_3_OR_NEWER
					return 18f;
#else
                    return EditorGUIUtility.singleLineHeight;
#endif
				}
			}

			public static float StandardVerticalSpacing
			{
				get
				{
#if UNITY_2019_3_OR_NEWER
					return 2f;
#else
					return EditorGUIUtility.standardVerticalSpacing;
#endif
				}
			}

			public static float StandardHorizontalSpacing
			{
				get { return StandardVerticalSpacing * 2; }
			}

			public static float DefaultFieldWidth
			{
				get { return 40; }
			}

			public static int HotControlId
			{
				get { return GUIUtility.hotControl; }
			}

			public static int KeyboardControlId
			{
				get { return GUIUtility.keyboardControl; }
			}

			public static void ExitGUI()
			{
				GUIUtility.ExitGUI();
			}

			public static float Space(float multiplier = 1)
			{
				return StandardVerticalSpacing * multiplier;
			}

			public static void SetNextControlName(string name)
			{
				GUI.SetNextControlName(name);
			}

			public static string GetNameOfFocusedControl()
			{
				return GUI.GetNameOfFocusedControl();
			}

			public static void FocusControl(string name)
			{
				GUI.FocusControl(name);
			}

			public static void FocusTextInControl(string name)
			{
				EditorGUI.FocusTextInControl(name);
			}

			public static void ResetKeyboardFocus()
			{
				GUIUtility.keyboardControl = 0;
			}

			public static float TextIconAlphaValue(bool enabled, bool ignoreAlpha = false)
			{
				return (enabled ? 1f : 0.7f) * (ignoreAlpha ? 1f : GUI.color.a);
			}

			public static UILanguagePack Labels
			{
				get { return UILanguagePack.Instance; }
			}

			public static void InspectorWidth(float currentInspectorWidth, out float width)
			{
				width = currentInspectorWidth;
				GUILayout.Label(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(0));
				if (Event.current.type != EventType.Repaint) return;
				Rect lastRect = GUILayoutUtility.GetLastRect();
				if (lastRect.width > 1) width = lastRect.width;
			}
		}
	}
}