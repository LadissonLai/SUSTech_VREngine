using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class FlakeGeneration : EditorWindow 
{
	// Saving
	private static string _textureSavePath = "/Car Paint - beffio/__Car Paint Pro/";
	private static string _normalTextureName = "GeneratedNormalFlakes";
	private static string _normalTextureExtension = ".png";
	private static string _maskTextureName = "GeneratedMaskFlakes";
	private static string _maskTextureExtension = ".png";

	// UI Text
	private static class ContentText
	{
		public static GUIContent windowTitle = new GUIContent("Flake Generation");

		public static GUIContent[] tabs = new GUIContent[]
		{
			new GUIContent("Normal Map", ""),
			new GUIContent("Mask Map", "")
		};

		public static GUIContent densityProperty = new GUIContent("Density", "The amount of flakes per area, relative to the flake size");
		public static GUIContent strengthProperty = new GUIContent("Strength", "The maximum strength of the flakes");
		public static GUIContent sizeProperty = new GUIContent("Size", "The minimum and maximum radii of the oval shaped flakes");
		public static GUIContent smoothnessProperty = new GUIContent("Smoothness", "Smoothness of the edges on the flakes");
		public static GUIContent textureSizeProperty = new GUIContent("Texture Size", "Size of the squared texture");
		public static GUIContent drawNormalMapProperty = new GUIContent("Normal Map", "");
		public static GUIContent drawMaskMapProperty = new GUIContent("Mask Map", "");


		public static GUIContent generateButton = new GUIContent("Generate", "Generate a new texture");
		public static GUIContent saveButton = new GUIContent("Save", "Save the current texture");

		public static GUIContent splitterTitle = new GUIContent("Metal flakes - Normal map");
	}

	/* VARIABLES */
	#region Variables
	private enum FlakeType
	{
		Normal,
		Mask
	}

	// Properties
	private float _density = 0.25f;
	private float _minSize = 2.0f;
	private float _maxSize = 8.0f;
	private float _strength = 0.5f;
	private float _smoothness = 3.0f;
	private int _texturePower = 9;
	public int TextureSize { get { return (int)Mathf.Pow(2, _texturePower); } }

	// Generated textures
	private Texture2D _normalTexture;
	private Texture2D _maskTexture;

	// Base colors
	private static Color _baseNormalColor = new Color(0.5f, 0.5f, 1.0f);
	private static Color _baseMaskColor = Color.black;
	private static Color _flakeMaskColor = Color.white;

	// Window settings
	private static float _minWidth = 440.0f;
	private static float _minControllerHeight = 60.0f;
	private static float _minPreviewHeight = 64.0f;

	private static float _previewHeight = 0.0f;
	private bool _splitterDragging = false;
	private Vector2 _settingsScrollPosition = Vector2.zero;
	private static FlakeType _currentFlakeType = FlakeType.Normal;

	private bool _autoUpdate = false;

	// Style data for the preview image
	private static class PreviewStyles
	{
		public static GUIStyle background = new GUIStyle(GUI.skin.box);

		private static Color previewBackgroundColor = new Color(.2f, .2f, .2f, 1.0f);

		static PreviewStyles()
		{

			Texture2D backgroundTexture = new Texture2D(1, 1);
			backgroundTexture.SetPixel(0, 0, previewBackgroundColor);
			backgroundTexture.Apply();

			background.margin = new RectOffset(0,0,0,0);
			background.border = new RectOffset(1,1,1,1);
			background.normal.background = backgroundTexture;
			background.fontStyle = FontStyle.Bold;
		}
	}
	#endregion

	[MenuItem ("Window/Beffio/Flake Generation")]
	public static void ShowWindow () 
	{
		// Setup window
		FlakeGeneration window = EditorWindow.GetWindow<FlakeGeneration>();
		window.name = ContentText.windowTitle.text;
		window.titleContent = ContentText.windowTitle;

		// Generate textures for immediate preview
		window.Generate();
	}

	private void OnEnable()
	{
		// Setup base normal color
		Vector3 normal = new Vector3(0, 0, 1);
		NormalVectorToTextureVector(ref normal);
		_baseNormalColor.r = normal.x;
		_baseNormalColor.g = normal.y;
		_baseNormalColor.b = normal.z;

		// Set preview height to half window height if not set before.
		if (_previewHeight < 1.0f)
		{
			_previewHeight = position.height / 2;
		}
	}

	private void OnInspectorUpdate() 
	{
		// Setup window minimum height
		minSize = new Vector2(_minWidth, _minControllerHeight + _minPreviewHeight + BeffioCarPaintEditorUtilities.SplitterHeight);

		Repaint();
	}

	/* EDITOR DRAWING */
	#region Editor Drawing
	private void OnGUI()
	{
		// Keep track of splitter position
		float splitterPosition = position.height - BeffioCarPaintEditorUtilities.SplitterHeight - _previewHeight;

		// Main settings kept in scroll view
		_settingsScrollPosition = EditorGUILayout.BeginScrollView(_settingsScrollPosition,
			GUILayout.Height(splitterPosition),
			GUILayout.MaxHeight(splitterPosition),
			GUILayout.MinHeight(splitterPosition),
			GUILayout.MinWidth(_minWidth),
			GUILayout.MaxWidth(position.width));
		{
			// Branding
			BeffioCarPaintEditorUtilities.DrawInspectorBranding();

			// Tabs
			_currentFlakeType = (FlakeType)GUILayout.Toolbar((int) _currentFlakeType, ContentText.tabs);

			// Settings
			DrawGUIFlakeSettings();

			// Buttons
			GUILayout.Space(10);
			DrawGUIButtons();
		}
		EditorGUILayout.EndScrollView();

		// Preview Image
		DrawGUIResizablePreview(splitterPosition);
	}

	/// <summary>
	/// Draws the GUI with flake settings.
	/// </summary>
	private void DrawGUIFlakeSettings()
	{
		// Setup auto update to auto update when toggling on
		bool isAutoUpdateAtStart = false;
		if (_autoUpdate)
		{
			isAutoUpdateAtStart = true;
			EditorGUI.BeginChangeCheck();
		}

		// Properties
		_density = EditorGUILayout.Slider(ContentText.densityProperty, _density, 0.0f, 1.0f);

		_strength = EditorGUILayout.Slider(ContentText.strengthProperty, _strength, 0.001f, 1.0f);

		EditorGUILayout.MinMaxSlider(ContentText.sizeProperty, ref _minSize, ref _maxSize, 1.0f, 20.0f);

		_smoothness = EditorGUILayout.Slider(ContentText.smoothnessProperty, _smoothness, 1.0f, 5.0f);

		// Show texture power with the texture size next to it
		EditorGUILayout.BeginHorizontal();
		{
			_texturePower = EditorGUILayout.IntSlider(ContentText.textureSizeProperty, _texturePower, 4, 12, GUILayout.ExpandWidth(true));
			int labelWidth = 50;
			EditorGUILayout.LabelField(TextureSize.ToString(), GUILayout.Width(labelWidth));
		}
		EditorGUILayout.EndHorizontal();

//		// Auto update toggle
//		_autoUpdate = EditorGUILayout.Toggle("Auto Update", _autoUpdate);

		// Regenerate textures with each change on auto update
		if (isAutoUpdateAtStart && EditorGUI.EndChangeCheck())
		{
			Generate();
		}
	}

	/// <summary>
	/// Draws the GUI with buttons.
	/// </summary>
	private void DrawGUIButtons()
	{
		// Generate button
		if (GUILayout.Button(ContentText.generateButton))
		{
			Generate();
		}

		// Save button
		if (GUILayout.Button(ContentText.saveButton))
		{
			// Save only currently selected type
			switch (_currentFlakeType)
			{
				case FlakeType.Normal:
					SaveTexture(_normalTexture, _textureSavePath, string.Format("{0}{1}", _normalTextureName, _normalTextureExtension), FlakeType.Normal);
					break;
				case FlakeType.Mask:
					SaveTexture(_maskTexture, _textureSavePath, string.Format("{0}{1}", _maskTextureName, _maskTextureExtension), FlakeType.Mask);
					break;
			}
		}

		// Show helpbox with the save path for the texture
		string savePath = "Whoops";
		switch (_currentFlakeType)
		{
			case FlakeType.Normal:
				savePath = string.Format("{0}{1}{2}", _textureSavePath, _normalTextureName, _normalTextureExtension);
				break;
			case FlakeType.Mask:
				savePath = string.Format("{0}{1}{2}", _textureSavePath, _maskTextureName, _maskTextureExtension);
				break;
		}

		EditorGUILayout.HelpBox(string.Format("Texture will be saved to {0}", savePath), MessageType.Info);
	}

	/// <summary>
	/// Draws the GUI with resizable preview.
	/// </summary>
	/// <param name="splitterPosition">Splitter position.</param>
	private void DrawGUIResizablePreview(float splitterPosition)
	{
		// Splitter
		float newPos = BeffioCarPaintEditorUtilities.DrawVerticalSplitter(
			ContentText.splitterTitle,
			splitterPosition, 
			_minControllerHeight,
			position.height - _minPreviewHeight - BeffioCarPaintEditorUtilities.SplitterHeight,
			BeffioCarPaintEditorUtilities.SplitterHeight, 
			ref _splitterDragging);

		// Check if splitter has been dragged
		if (Mathf.Abs(newPos - splitterPosition) > .01f)
		{
			splitterPosition = newPos;
			Repaint();
		}

		// Recalculate preview height
		_previewHeight = position.height - splitterPosition - BeffioCarPaintEditorUtilities.SplitterHeight;

		// Draw preview background
		float size = Mathf.Max(_previewHeight, _minPreviewHeight);
		size = Mathf.Min(position.width, size);

		GUILayout.Box("",
			PreviewStyles.background,
			GUILayout.ExpandHeight(true),
			GUILayout.ExpandWidth(true));

		// Draw preview texture
		Texture2D previewTexture = null;
		switch (_currentFlakeType)
		{
			case FlakeType.Normal:
				previewTexture = _normalTexture;
				break;
			case FlakeType.Mask:
				previewTexture = _maskTexture;
				break;
			default:
				break;
		}

		if (previewTexture != null)
		{
			EditorGUI.DrawPreviewTexture(
				new Rect(
					(position.width - size)/2, 
					position.height - size - (_previewHeight-size)/2, 
					size, 
					size), 
				previewTexture);
		}
	}
	#endregion

	/* FLAKE GENERATION */
	#region Flake Generation
	/// <summary>
	/// Generate both textures.
	/// </summary>
	private void Generate()
	{
		// Generate flakes
		// Amount of points calculated based on area, density and flake size
		int maxPoints = (int)(TextureSize * TextureSize * _density / (_maxSize * _maxSize));
		List<Flake> points = GenerateRandomFlakeList(maxPoints);

		// Generate two textures
		Generate(ref _normalTexture, points, FlakeType.Normal);
		Generate(ref _maskTexture, points, FlakeType.Mask);
	}

	/// <summary>
	/// Generate the specified texture with a list of flakes
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="flakes">List of flakes.</param>
	/// <param name="type">Texture type.</param>
	private void Generate(ref Texture2D texture, List<Flake> flakes, FlakeType type)
	{
		// If texture is incorrect size destroy it, so it can be recreated
		if (texture != null && (texture.width != TextureSize || texture.height != TextureSize))
		{
			DestroyImmediate(texture);
			texture = null;
		}

		// Create texture if necessary
		if (texture == null)
		{
			texture = new Texture2D(TextureSize, TextureSize);
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Repeat;
		}

		// Paint flakes
		if (texture != null)
		{
			PaintFlakes(ref texture, flakes, type);
		}
	}

	/// <summary>
	/// Paints a list of flakes.
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="flakes">List of flakes.</param>
	/// <param name="type">Texture type.</param>
	private void PaintFlakes(ref Texture2D texture, List<Flake> flakes, FlakeType type)
	{
		// Check which flakes to draw
		switch (type)
		{
			case FlakeType.Normal:
				PaintNormalFlakes(ref texture, flakes);
				break;
			case FlakeType.Mask:
				PaintMaskFlakes(ref texture, flakes);
				break;
			default:
				break;
		}

		// Apply changes to the texture
		texture.Apply();
	}

	/// <summary>
	/// Paints flakes on a normal texture.
	/// </summary>
	/// <param name="texture">Normal texture.</param>
	/// <param name="flakes">List of flakes.</param>
	private void PaintNormalFlakes(ref Texture2D texture, List<Flake> flakes)
	{
		// Clear texture first
		ClearTexture(ref texture, _baseNormalColor);

		// Loop over flakes
		for (int i = 0; i < flakes.Count; i++)
		{
			DrawNormalFlake(ref texture, flakes[i]);
		}
	}

	/// <summary>
	/// Paints flakes on a mask texture.
	/// </summary>
	/// <param name="texture">Mask texture.</param>
	/// <param name="flakes">List of flakes.</param>
	private void PaintMaskFlakes(ref Texture2D texture, List<Flake> flakes)
	{
		// Clear texture first
		ClearTexture(ref texture, _baseMaskColor);

		// Loop over flakes
		for (int i = 0; i < flakes.Count; i++)
		{
			DrawMaskFlake(ref texture, flakes[i]);
		}
	}

	/// <summary>
	/// Clears a texture.
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="color">Clear color.</param>
	private void ClearTexture(ref Texture2D texture, Color color)
	{
		for (int x = 0; x < texture.width; x++)
		{
			for (int y = 0; y < texture.height; y++) 
			{
				texture.SetPixel(x, y, color);
			}
		}
	}

	/// <summary>
	/// Generates a random flake list.
	/// </summary>
	/// <returns>The random flake list.</returns>
	/// <param name="maxPoints">Max amount of points.</param>
	private List<Flake> GenerateRandomFlakeList(int maxPoints)
	{
		// New seed
		#if UNITY_5_4_OR_NEWER
			Random.InitState ((int)System.DateTime.Now.ToFileTime());
		#else
			Random.seed = (int)System.DateTime.Now.ToFileTime();
		#endif

		List<Flake> output = new List<Flake>();
		for (int i = 0; i < maxPoints; i++)
		{
			output.Add(GenerateRandomFlake());
		}

		return output;
	}

	/// <summary>
	/// Generates a random flake.
	/// </summary>
	/// <returns>The random flake.</returns>
	private Flake GenerateRandomFlake()
	{
		Flake flake = new Flake();
		flake.x = Random.Range(0, TextureSize);
		flake.y = Random.Range(0, TextureSize);
		flake.rx = Random.Range(_minSize, _maxSize);
		flake.ry = Random.Range(_minSize, _maxSize);
		flake.angle = Random.Range(0, 360); // Rotation
		flake.direction = Vector3.Normalize(new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(0.0f, 1.0f)/_strength)); // Normal direction
		flake.strength = Random.Range(0.0f, _strength); // Mask strength

		return flake;
	}

	/// <summary>
	/// Normal vector to texture value conversion
	/// </summary>
	/// <param name="vector">Vector to convert.</param>
	private static void NormalVectorToTextureVector(ref Vector3 vector)
	{
		vector.x = NormalValueToTextureValue(vector.x);
		vector.y = NormalValueToTextureValue(vector.y);
		vector.z = NormalValueToTextureValue(vector.z);
	}

	/// <summary>
	/// Normal value to texture value conversion.
	/// </summary>
	/// <returns>Texture vector component.</returns>
	/// <param name="value">Normal vector component.</param>
	private static float NormalValueToTextureValue(float value)
	{
		return (value + 1) / 2;
	}

	/// <summary>
	/// Draws a flake on a normal map.
	/// </summary>
	/// <param name="texture">Normal map.</param>
	/// <param name="flake">Flake info.</param>
	private void DrawNormalFlake(ref Texture2D texture, Flake flake)
	{
		// Convert normal vector to texture value
		NormalVectorToTextureVector(ref flake.direction);
		Color normalColor = new Color(flake.direction.x, flake.direction.y, flake.direction.z);

		// Draw
		DrawAliasedImplicitEquation(ref texture, flake, normalColor, _smoothness);
	}

	/// <summary>
	/// Draws a flake on a mask texture.
	/// </summary>
	/// <param name="texture">Mask texture.</param>
	/// <param name="flake">Flake info.</param>
	private void DrawMaskFlake(ref Texture2D texture, Flake flake)
	{
		// Calculate flake color
		Color maskColor = _flakeMaskColor * flake.strength;

		// Draw
		DrawAliasedImplicitEquation(ref texture, flake, maskColor, _smoothness);
	}

	/// <summary>
	/// Draw an Anti Aliased Ellipse, based on the implicit equation.
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="flake">Flake info.</param>
	/// <param name="color">Fill color.</param>
	/// <param name="smoothness">Anti alias smoothness.</param>
	private void DrawAliasedImplicitEquation(ref Texture2D texture, Flake flake, Color color, float smoothness)
	{
		// Thickness for error (Anti Alias) calculation
		float thickness = smoothness / Mathf.Min(flake.rx, flake.ry);

		// Precalculate powers
		float radiusX2 = flake.rx * flake.rx;
		float radiusY2 = flake.ry * flake.ry;

		// Calculate covered area including anti alias border
		float maxRadius = Mathf.Max(flake.rx, flake.ry);
		float squareSize = maxRadius + smoothness;
		float area = 4 * squareSize * squareSize;

		// Loop over the whole covered area
		for (int i = 0; i < area; ++i)
		{
			float x = (int)((i / (2 * (int)squareSize)) - (int)squareSize);
			float y = (int)((i % (2 * (int)squareSize)) - (int)squareSize);

			float rotatedX = (Mathf.Cos(-flake.angle) * x + Mathf.Sin(-flake.angle) * y);
			float rotatedY = (-Mathf.Sin(-flake.angle) * x + Mathf.Cos(-flake.angle) * y);

			// Calculate anti alias error by calculating a value that should be one and comparing it to the real one
			float one = (rotatedX * rotatedX / radiusX2) + (rotatedY * rotatedY / radiusY2);
			float error = (one - 1) / thickness;

			// Ignore outside area
			if (error >= 1)
				continue;

			// Alpha is based on error
			float alpha = Mathf.Clamp(error, 0.0f, 1.0f);

			// Blend the pixel based on alpha
			BlendPixel(ref texture, flake.x, flake.y, (int)x, (int)y, flake.angle, color, alpha);
		}
	}

	/// <summary>
	/// Blends a new color on a pixel based on the alpha value. 
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="centerX">Circle Center X.</param>
	/// <param name="centerY">Circle Center Y.</param>
	/// <param name="x">X coordinate relative to Center X.</param>
	/// <param name="y">Y coordinate relative to Center Y.</param>
	/// <param name="angle">Ellipse Angle.</param>
	/// <param name="color">Fill Color.</param>
	/// <param name="alpha">Alpha value.</param>
	private void BlendPixel(ref Texture2D texture, int centerX, int centerY, int x, int y, float angle, Color color, float alpha)
	{
		int xr = (int)((centerX + x) % TextureSize);
		int xl = 2 * centerX - xr -1;
		int yt = (int)((centerY + y) % TextureSize);
		int yb = 2 * centerY - yt -1;

		Color col = Color.Lerp(color, texture.GetPixel(xl, yb), alpha);
		texture.SetPixel(xl, yb, col);
	}

	public struct Flake
	{
		public int x;
		public int y;
		public float rx;
		public float ry;
		public float angle;
		public Vector3 direction;
		public float strength;
	}
	#endregion

	/* TEXTURE SAVING */
	#region Texture Saving
	/// <summary>
	/// Saves a texture to the given path and file name.
	/// </summary>
	/// <param name="texture">Texture.</param>
	/// <param name="textureSavePath">Texture save path.</param>
	/// <param name="textureName">Texture name.</param>
	/// <param name="loadMethod">Flake type, used for the import settings.</param>
	private void SaveTexture(Texture2D texture, string textureSavePath, string textureName, FlakeType loadMethod)
	{
		// Save texture to raw png data
		byte[] textureBytes = texture.EncodeToPNG();

		// Generate name
		string texturePath = string.Format("{0}{1}", textureSavePath, textureName);

		// Check if path exists
		string directoryPath = string.Format("{0}{1}", Application.dataPath, _textureSavePath);
		// Create folder if it doesn't exist yet
		if(!AssetDatabase.IsValidFolder(directoryPath))
			Directory.CreateDirectory(directoryPath);

		// Write texture to disk
		File.WriteAllBytes(string.Format("{0}{1}", Application.dataPath, texturePath), textureBytes);

		// Import the texture asset
		string assetPath = string.Format("Assets{0}", texturePath);
		ImportAndReloadTexture(assetPath, loadMethod);
	}

	/// <summary>
	/// Import, set up import settings and reimport.
	/// </summary>
	/// <param name="assetPath">Asset path.</param>
	/// <param name="loadMethod">Flake type, used for the import settings.</param>
	private void ImportAndReloadTexture(string assetPath, FlakeType loadMethod)
	{
		// Import asset
		AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

		// Set correct import settings
		TextureImporter textureImport = TextureImporter.GetAtPath(assetPath) as TextureImporter;
		switch (loadMethod)
		{
			case FlakeType.Normal:
				textureImport.textureType = TextureImporterType.NormalMap;
				textureImport.wrapMode = TextureWrapMode.Repeat;
				textureImport.filterMode = FilterMode.Bilinear;
				textureImport.mipmapFilter = TextureImporterMipFilter.KaiserFilter;
				break;
			case FlakeType.Mask:
				textureImport.textureType = TextureImporterType.Default;
				textureImport.wrapMode = TextureWrapMode.Repeat;
				textureImport.filterMode = FilterMode.Bilinear;
				textureImport.mipmapFilter = TextureImporterMipFilter.KaiserFilter;
				break;
			default:
				break;
		}

		// Reimport
		textureImport.SaveAndReimport();
	}
	#endregion

}
