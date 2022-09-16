using UnityEngine;
using UnityEditor;
using System.Collections;

public static class BeffioCarPaintEditorUtilities
{
	#region Branding
	private static string _brandingLogoPath = "Assets/ThirdParty/Car Paint - beffio/__Car Paint Pro/Editor/beffio_logo.png";

	/// <summary>
	/// Draws inspector branding centered within the panel.
	/// </summary>
	public static void DrawInspectorBranding()
	{
		// Get texture and its aspect ratio
		Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(_brandingLogoPath);
		float imageAspect = tex.height/tex.width;

		// Make GUIContent from texture
		GUIContent content = new GUIContent(tex);

		// Calculate width and height clamped to the width of the inspector
		float scrollBarWidth = 40;
		float width = EditorGUIUtility.currentViewWidth - scrollBarWidth; // See for scrollbar: http://forum.unity3d.com/threads/editorguilayout-get-width-of-inspector-window-area.82068/
		float height = width * imageAspect;

		// Save background color
		Color oldCol = GUI.backgroundColor;
		// Set transparent background color
		GUI.backgroundColor = new Color(0,0,0,0);

		// Draw Header branding
		GUILayout.Box(content, GUILayout.Width(width), GUILayout.Height(height));

		// Reset old color
		GUI.backgroundColor = oldCol;
	}
	#endregion

	#region Header element
	// FROM: Unity Standard Assets - Cinematic Effects - EditorGUIHelper
	private static class HeaderStyles
	{
		public static GUIStyle header = "ShurikenModuleTitle";
		public static GUIStyle headerCheckbox = "ShurikenCheckMark";

		static HeaderStyles()
		{
			header.font = (new GUIStyle("Label")).font;
			header.border = new RectOffset(15, 7, 4, 4);
			header.fixedHeight = 22;
			header.contentOffset = new Vector2(20f, -2f);
		}
	}

	/// <summary>
	/// Draw a togleable group header with checkbox to enable.
	/// </summary>
	/// <param name="title">Header title.</param>
	/// <param name="isDisplayed">Is group displayed.</param>
	/// <param name="isEnabled">Is group enabled.</param>
	public static bool Header(GUIContent title, ref bool isDisplayed, ref bool isEnabled)
	{
		Rect headerRect = GUILayoutUtility.GetRect(16f, 22f, HeaderStyles.header);
		GUI.Box(headerRect, title, HeaderStyles.header);

		Rect toggleRect = new Rect(headerRect.x + 4f, headerRect.y + 4f, 13f, 13f);
		if (Event.current.type == EventType.Repaint)
			HeaderStyles.headerCheckbox.Draw(toggleRect, false, false, isEnabled, false);

		Event e = Event.current;
		if (e.type == EventType.MouseDown)
		{
			if (toggleRect.Contains(e.mousePosition))
			{
				isEnabled = !isEnabled;
				e.Use();
			}
			else if (headerRect.Contains(e.mousePosition))
			{
				isDisplayed = !isDisplayed;
				e.Use();
			}
		}

		return isDisplayed;
	}
	#endregion

	#region VerticalSplitter
	public static float SplitterHeight = 20.0f;
	public static class SplitterStyles
	{
		public static GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
		public static GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
		public static GUIStyle handleStyle = new GUIStyle(GUI.skin.box);

		private static Color splitterBackgroundColorTop = new Color(.4f, .4f, .4f, 1.0f);
		private static Color splitterBackgroundColorBottom = new Color(.3f, .3f, .3f, 1.0f);
		private static Color splitterBorderColor = new Color(.45f, .45f, .45f, 1.0f);
		private static int splitterBorderSize = 2;

		private static Color splitterHandleTextColor = new Color(.9f, .9f, .9f, 1.0f);

		private static Color splitterHandleColor = new Color(.75f, .75f, .75f, 1.0f);
		private static int splitterHandleDistance = 4;

		static SplitterStyles()
		{
			// Setup background style
			Texture2D backgroundTexture = new Texture2D(16, (int)SplitterHeight);
			DrawSplitterBackgroundTexture(
				ref backgroundTexture, 
				SplitterHeight, splitterBorderSize, 
				splitterBorderColor, splitterBackgroundColorBottom, splitterBackgroundColorTop);

			backgroundStyle = new GUIStyle(GUI.skin.box);
			backgroundStyle.margin = new RectOffset(0,0,0,0);
			backgroundStyle.padding = new RectOffset(0,0,0,0);
			backgroundStyle.border = new RectOffset(6,6,2,2);
			backgroundStyle.normal.background = backgroundTexture;
			backgroundStyle.normal.textColor = splitterHandleColor;
			backgroundStyle.fixedHeight = SplitterHeight;

			// Setup title style
			titleStyle.normal.textColor = splitterHandleTextColor;
			titleStyle.fontStyle = FontStyle.Bold;
			titleStyle.stretchWidth = true;

			// Setup handle style
			Texture2D handleTexture = new Texture2D(16, (int)SplitterHeight);
			DrawSplitterHandleTexture(ref handleTexture, SplitterHeight, splitterHandleDistance, splitterHandleColor);

			handleStyle.margin = new RectOffset(0, 4, 0, 0);
			handleStyle.padding = new RectOffset(0, 0, 0, 0);
			handleStyle.normal.background = handleTexture;
		}

		private static void DrawSplitterBackgroundTexture(
			ref Texture2D texture, 
			float height, int borderSize, 
			Color borderColor, Color backgroundColorBottom, Color backgroundColorTop)
		{
			Color color = splitterBackgroundColorTop;
			for (int i = 0; i < texture.width * texture.height; ++i)
			{
				int x = i / texture.height;
				int y = i % texture.height;
				if (x <= borderSize || y >= texture.height - borderSize)
				{
					// Draw border
					color = borderColor;
				}
				else
				{
					// Draw fill gradient
					color = Color.Lerp(backgroundColorBottom, backgroundColorTop, y / height);
				}

				texture.SetPixel(x, y, color);
			}
			texture.Apply();
		}

		private static void DrawSplitterHandleTexture(ref Texture2D texture, float height, int handleDistance, Color handleColor)
		{
			Color color = new Color(0, 0, 0, 0);
			texture = new Texture2D(1, (int)height);
			float topHandleHeight = (height - handleDistance) / 2 - 1;
			float bottomHandleHeight = (height + handleDistance) / 2 - 1;
			for (int y = 0; y < height; ++y)
			{
				if (y == topHandleHeight || y == bottomHandleHeight)
				{
					// Draw handle
					color = handleColor;
				}
				else
				{
					// Draw transparent
					color = new Color(0, 0, 0, 0);
				}
				texture.SetPixel(0, y, color);
			}
			texture.Apply();
		}
	}

	/// <summary>
	/// Draws a vertical dragable splitter.
	/// </summary>
	/// <returns>The vertical splitter position.</returns>
	/// <param name="title">Title.</param>
	/// <param name="inputPosition">Input vertical position.</param>
	/// <param name="minPosition">Minimum position.</param>
	/// <param name="maxPosition">Max position.</param>
	/// <param name="height">Splitter height.</param>
	/// <param name="isDraggin">Is splitter being dragged.</param>
	public static float DrawVerticalSplitter(GUIContent title, float inputPosition, float minPosition, float maxPosition, float height, ref bool isDraggin)
	{

		EditorGUILayout.BeginHorizontal(
			SplitterStyles.backgroundStyle,
			GUILayout.Height(height), 
			GUILayout.MaxHeight(height), 
			GUILayout.MinHeight(height),
			GUILayout.ExpandWidth(true));
		{
			// Draw label
			Rect labelRect = GUILayoutUtility.GetRect(title, SplitterStyles.titleStyle, GUILayout.ExpandWidth(false));

			GUI.Label(labelRect,
				title, 
				SplitterStyles.titleStyle);

			// Draw Handle
			GUILayout.Box("", SplitterStyles.handleStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		}
		EditorGUILayout.EndHorizontal();

		Rect splitterRect = GUILayoutUtility.GetLastRect();

		// Splitter events
		if (Event.current != null) 
		{
			switch (Event.current.type) 
			{
				case EventType.MouseDown:
					if (splitterRect.Contains (Event.current.mousePosition))
					{
						isDraggin = true;
					}
					break;
				case EventType.MouseDrag:
					if (isDraggin)
					{
						inputPosition += Event.current.delta.y;
					}
					break;
				case EventType.MouseUp:
					if (isDraggin)
					{
						isDraggin = false;
					}
					break;
			}
		}

		// Clamp and return vertical position
		inputPosition = Mathf.Clamp(inputPosition, minPosition, maxPosition);
		return inputPosition;
	}
	#endregion
}
