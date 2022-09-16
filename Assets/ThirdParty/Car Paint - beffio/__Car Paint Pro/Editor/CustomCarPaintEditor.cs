using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomCarPaintEditor : MaterialEditor 
{
	// UI Text
	private static class ContentText
	{
		public static GUIContent baseAlbedoProperty = new GUIContent("Base Albedo", "Albedo (RGB) and Transparency (A)");
		public static GUIContent baseMetallicMapProperty = new GUIContent("Base Metallic", "Metallic (R) and Smoothness (A)");
		public static GUIContent baseSmoothnessProperty = new GUIContent("Smoothness");
		public static GUIContent normalMapProperty = new GUIContent("Normal Map", "Normal Map");
		public static GUIContent occlusionMapProperty = new GUIContent("Occlusion", "Occlusion (G)");
		public static GUIContent emissionMapProperty = new GUIContent("Emission", "Emission (RGB)");

		public static GUIContent reflectionSpecularMapProperty = new GUIContent("Reflection Specular", "Specular (RGB) and Smoothness (A)");
		public static GUIContent reflectionSmoothnessProperty = new GUIContent("Reflection Smoothness");

		public static GUIContent flakeBumpMapProperty = new GUIContent("Flake Normal Map", "Normal Map");
		public static GUIContent flakeBumpScaleProperty = new GUIContent("Flake Scale");
		public static GUIContent flakeBumpStrengthProperty = new GUIContent("Flake Strength");

		public static GUIContent flakeColorMapProperty = new GUIContent("Flake Color", "Flake Pattern (R) colorized");
		public static GUIContent flakeColorScaleProperty = new GUIContent("Flake Pattern Scale");
		public static GUIContent flakeColorAlphaStrenthProperty = new GUIContent("Flake Alpha Strength");
		public static GUIContent flakeColorCutoffProperty = new GUIContent("Flake Cutoff");

		public static GUIContent fresnelColorProperty = new GUIContent("Fresnel Color");
		public static GUIContent fresnelPowerProperty = new GUIContent("Fresnel Power");

		public static GUIContent overlayAlbedoProperty = new GUIContent("Overlay Albedo", "Albedo (RGB) and Transparency (A)");
		public static GUIContent overlaySpecularMapProperty = new GUIContent("Overlay Specular", "Specular (RGB) and Smoothness (A)");
		public static GUIContent overlaySmoothnessProperty = new GUIContent("Overlay Smoothness");

		public static GUIContent basePaintTitle = new GUIContent("Base Paint", "Bese metallic car paint");
		public static GUIContent reflectiveCoatTitle = new GUIContent("Reflective Coating", "Add a reflective coating to the material");
		public static GUIContent overlayTitle = new GUIContent("Overlay Layer", "Add an overlay layer, commonly used for decals or dirt");
		public static GUIContent flakesBumpHeader = new GUIContent("Flakes Bumped", "Add bumped flakes to the base paint layer");
		public static GUIContent flakesColorHeader = new GUIContent("Flakes Color", "Add colored flakes to the base paint layer");
		public static GUIContent fresnelHeader = new GUIContent("Fresnel", "Add a colored fresnel to the base paint layer");
		public static GUIContent settingsHeader = new GUIContent("Settings", "");
	}

	/* VARIABLES */
	#region Variables
	private enum FlakeColorMode
	{
		None,
		Map,
		NormalAlpha,
		NormalCutoff
	}

	// Material
	private Material _targetMat;

	// Material Properties
	private MaterialProperty _albedoMapProperty = null;
	private MaterialProperty _albedoColorProperty = null;
	private MaterialProperty _metallicMapProperty = null;
	private MaterialProperty _metallicProperty = null;
	private MaterialProperty _smoothnessProperty = null;
	private MaterialProperty _normalMapProperty = null;
	private MaterialProperty _normalScaleProperty = null;
	private MaterialProperty _occlusionMapProperty = null;
	private MaterialProperty _occlusionStrengthProperty = null;
	private MaterialProperty _emissionMapProperty = null;
	private MaterialProperty _emissionColorProperty = null;

	private MaterialProperty _reflectionSpecularMapProperty = null;
	private MaterialProperty _reflectionSpecularProperty = null;
	private MaterialProperty _reflectionSmoothnessProperty = null;

	private MaterialProperty _flakeBumpMapProperty = null;
	private MaterialProperty _flakeBumpScaleProperty = null;
	private MaterialProperty _flakeBumpStrengthProperty = null;

	private MaterialProperty _flakeColorProperty = null;
	private MaterialProperty _flakeColorMapProperty = null;
	private MaterialProperty _flakeColorScaleProperty = null;
	private MaterialProperty _flakeColorStrengthProperty = null;
	private MaterialProperty _flakeColorCutoffProperty = null;

	private MaterialProperty _fresnelColorProperty = null;
	private MaterialProperty _fresnelPowerProperty = null;

	private MaterialProperty _overlayAlbedoMapProperty = null;
	private MaterialProperty _overlayColorProperty = null;
	private MaterialProperty _overlaySpecularMapProperty = null;
	private MaterialProperty _overlaySpecularProperty = null;
	private MaterialProperty _overlaySmoothnessProperty = null;

	// Emission
	private ColorPickerHDRConfig _colorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 1/99f, 3f);

	// Flakes Bump
	private bool _flakesBumpEnabled = true;
	private static bool _flakesBumpDisplayed = false;

	// Flakes Color
	private bool _flakesColorEnabled = false;
	private FlakeColorMode _selectedFlakeColorMode = FlakeColorMode.NormalAlpha;
	private static bool _flakesColorDisplayed = false;

	// Fresnel
	private bool _fresnelEnabled = true;
	private static bool _fresnelDisplayed = false;

	// TEST
//	private bool _showDefault = false;
	#endregion

	public override void OnEnable()
	{
		base.OnEnable();

		// Get reference of material
		_targetMat = target as Material;

		FindProperties();

		// Check material for keywords.
		_flakesBumpEnabled = _targetMat.IsKeywordEnabled("FLAKES_BUMP");
		_fresnelEnabled = _targetMat.IsKeywordEnabled("FRESNEL");

		if (_targetMat.IsKeywordEnabled("FLAKES_COLOR_MAP"))
		{
			_flakesColorEnabled = true;
		}
		else if (_targetMat.IsKeywordEnabled("FLAKES_COLOR_ALPHA"))
		{
			_flakesColorEnabled = true;
			_selectedFlakeColorMode = FlakeColorMode.NormalAlpha;
		}
		else if (_targetMat.IsKeywordEnabled("FLAKES_COLOR_CUTOFF"))
		{
			_flakesColorEnabled = true;
			_selectedFlakeColorMode = FlakeColorMode.NormalCutoff;
		}
		else
		{
			_flakesColorEnabled = false;
		}
	}

	/// <summary>
	/// Find the shader properties.
	/// </summary>
	private void FindProperties()
	{
		_albedoColorProperty = GetMaterialProperty(targets, "_Color");
		_albedoMapProperty = GetMaterialProperty(targets, "_MainTex");
		_metallicProperty = GetMaterialProperty(targets, "_Metallic");
		_smoothnessProperty = GetMaterialProperty(targets, "_Glossiness");
		_metallicMapProperty = GetMaterialProperty(targets, "_MetallicGlossMap");
		_normalMapProperty = GetMaterialProperty(targets, "_BumpMap");
		_normalScaleProperty = GetMaterialProperty(targets, "_BumpScale");
		_occlusionMapProperty = GetMaterialProperty(targets, "_OcclusionMap");
		_occlusionStrengthProperty = GetMaterialProperty(targets, "_OcclusionStrength");
		_emissionMapProperty = GetMaterialProperty(targets, "_EmissionMap");
		_emissionColorProperty = GetMaterialProperty(targets, "_EmissionColor");

		_reflectionSpecularMapProperty = GetMaterialProperty(targets, "_ReflectionSpecularMap");
		_reflectionSpecularProperty = GetMaterialProperty(targets, "_ReflectionSpecular");
		_reflectionSmoothnessProperty = GetMaterialProperty(targets, "_ReflectionGlossiness");

		_flakeBumpMapProperty = GetMaterialProperty(targets, "_FlakesBumpMap");
		_flakeBumpScaleProperty = GetMaterialProperty(targets, "_FlakesBumpMapScale");
		_flakeBumpStrengthProperty = GetMaterialProperty(targets, "_FlakesBumpStrength");

		_flakeColorProperty = GetMaterialProperty(targets, "_FlakeColor");
		_flakeColorMapProperty = GetMaterialProperty(targets, "_FlakesColorMap");
		_flakeColorScaleProperty = GetMaterialProperty(targets, "_FlakesColorMapScale");
		_flakeColorStrengthProperty = GetMaterialProperty(targets, "_FlakesColorStrength");
		_flakeColorCutoffProperty = GetMaterialProperty(targets, "_FlakesColorCutoff");

		_fresnelColorProperty = GetMaterialProperty(targets, "_FresnelColor");
		_fresnelPowerProperty = GetMaterialProperty(targets, "_FresnelPower");

		_overlayColorProperty = GetMaterialProperty(targets, "_OverlayColor");
		_overlayAlbedoMapProperty = GetMaterialProperty(targets, "_OverlayMainTex");
		_overlaySpecularProperty = GetMaterialProperty(targets, "_OverlaySpecular");
		_overlaySmoothnessProperty = GetMaterialProperty(targets, "_OverlayGlossiness");
		_overlaySpecularMapProperty = GetMaterialProperty(targets, "_OverlaySpecGlossMap");
	}

	#region Inspector Drawing
	public override void OnInspectorGUI()
	{
		// Don't show if material inspector is folded in
		if (!isVisible)
		{
			return;
		}

		// Branding
		BeffioCarPaintEditorUtilities.DrawInspectorBranding();

		// Search for properties every tick, in case they change.
		FindProperties();

		// Draw base paint settings
		DrawInspectorBaseMaterial();

		// Draw reflective coating settings
		GUILayout.Space(5);
		DrawInspectorReflectiveCoat();

		// Draw overlay layer settings,
		// if the material selected has an overlay texture
		if (_overlayAlbedoMapProperty.type == MaterialProperty.PropType.Texture)
		{
			GUILayout.Space(5);
			DrawInspectorOverlay();
		}

		// Draw flakes bump foldout group
		GUILayout.Space(5);
		DrawInspectorFlakesBump();

		// Draw flakes color foldout group
		GUILayout.Space(5);
		DrawInspectorFlakesColor();

		// Draw fresnel foldout group
		GUILayout.Space(5);
		DrawInspectorFresnel();

		// TEST: Show default inspector in foldout menu
//		GUILayout.Space(30);
//		_showDefault = EditorGUILayout.Foldout(_showDefault, "Default Inspector");
//		if (_showDefault)
//		{
//			base.OnInspectorGUI();
//		}
	}

	private void DrawInspectorBaseMaterial()
	{
		// Title
		EditorGUILayout.LabelField(ContentText.basePaintTitle, EditorStyles.boldLabel);

		// Albedo map
		TexturePropertySingleLine(ContentText.baseAlbedoProperty, _albedoMapProperty, _albedoColorProperty);

		// Metallic and smoothness
		EditorGUI.BeginChangeCheck();
		{
			if (_metallicMapProperty.textureValue == null)
			{
				// Show sliders if no texture is specified
				TexturePropertyTwoLines(
					ContentText.baseMetallicMapProperty, 
					_metallicMapProperty, 
					_metallicProperty, 
					ContentText.baseSmoothnessProperty, 
					_smoothnessProperty);
			}
			else
			{
				// Only show texture if texture is specified
				TexturePropertySingleLine(ContentText.baseMetallicMapProperty, _metallicMapProperty);
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_METALLICGLOSSMAP", _metallicMapProperty.textureValue != null);
		}

		// Normals
		EditorGUI.BeginChangeCheck();
		{
			TexturePropertySingleLine(ContentText.normalMapProperty, _normalMapProperty, _normalMapProperty.textureValue != null ? _normalScaleProperty : null);
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_NORMALMAP", _normalMapProperty.textureValue != null);
		}

		// Occlusion
		EditorGUI.BeginChangeCheck();
		{
			TexturePropertySingleLine(ContentText.occlusionMapProperty, _occlusionMapProperty, _occlusionMapProperty.textureValue != null ? _occlusionStrengthProperty : null);
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_OCCLUSIONMAP", _occlusionMapProperty.textureValue != null);
		}

		// Emission
		EditorGUI.BeginChangeCheck();
		{
			float brightness = _emissionColorProperty.colorValue.maxColorComponent;
			bool showEmissionColorAndGIControls = brightness > 0.0f;
			bool hadEmissionTexture = (_emissionMapProperty.textureValue != null);

			// Texture and HDR color controls
			TexturePropertyWithHDRColor(ContentText.emissionMapProperty, _emissionMapProperty, _emissionColorProperty, _colorPickerHDRConfig, false);

			// If texture was assigned and color was black set color to white
			if (_emissionMapProperty.textureValue != null && !hadEmissionTexture && brightness <= 0f)
			{
				_emissionColorProperty.colorValue = Color.white;
			}
		
			// Dynamic Lightmapping mode
			if (showEmissionColorAndGIControls)
			{
				bool shouldEmissionBeEnabled = ShouldEmissionBeEnabled(_emissionColorProperty.colorValue);
				EditorGUI.BeginDisabledGroup(!shouldEmissionBeEnabled);

				LightmapEmissionProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel + 1);

				EditorGUI.EndDisabledGroup();
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_EMISSIVEMAP", _emissionMapProperty.textureValue != null);
		}

		// TILING
		EditorGUI.BeginChangeCheck();
		TextureScaleOffsetProperty(_albedoMapProperty);
		if (EditorGUI.EndChangeCheck())
			_emissionMapProperty.textureScaleAndOffset = _albedoMapProperty.textureScaleAndOffset; // Apply the main texture scale and offset to the emission texture as well, for Enlighten's sake
		

	}

	static bool ShouldEmissionBeEnabled (Color color)
	{
		return color.maxColorComponent > (0.1f / 255.0f);
	}

	private void DrawInspectorReflectiveCoat()
	{
		// Title
		EditorGUILayout.LabelField(ContentText.reflectiveCoatTitle, EditorStyles.boldLabel);

		// Specular and smoothness
		EditorGUI.BeginChangeCheck();
		{
			if (_reflectionSpecularMapProperty.textureValue == null)
			{
				// Show sliders if no texture is specified
				TexturePropertyTwoLines(
					ContentText.reflectionSpecularMapProperty, 
					_reflectionSpecularMapProperty, 
					_reflectionSpecularProperty, 
					ContentText.reflectionSmoothnessProperty, 
					_reflectionSmoothnessProperty);
			}
			else
			{
				// Only show texture if texture is specified
				TexturePropertySingleLine(ContentText.reflectionSpecularMapProperty, _reflectionSpecularMapProperty);
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_REFLECTION_SPECGLOSSMAP", _reflectionSpecularMapProperty.textureValue != null);
		}
	}

	private void DrawInspectorFlakesBump()
	{
		// Header
		bool areFlakesEnabled = _flakesBumpEnabled;
		if(BeffioCarPaintEditorUtilities.Header(ContentText.flakesBumpHeader, ref _flakesBumpDisplayed, ref areFlakesEnabled))
		{
			// Flakes bump map
			TexturePropertySingleLine(ContentText.flakeBumpMapProperty, _flakeBumpMapProperty);

			// Flakes bump map scale and strength
			ShaderProperty(_flakeBumpScaleProperty, ContentText.flakeBumpScaleProperty.text, 2);
			ShaderProperty(_flakeBumpStrengthProperty, ContentText.flakeBumpStrengthProperty.text, 2);
		}
		if (_flakesBumpEnabled != areFlakesEnabled)
		{
			_flakesBumpEnabled = areFlakesEnabled;
			// Setup keyword
			EnableKeyword("FLAKES_BUMP", _flakesBumpEnabled);
		}
	}

	private void DrawInspectorFlakesColor()
	{
		// Header
		if(BeffioCarPaintEditorUtilities.Header(ContentText.flakesColorHeader, ref _flakesColorDisplayed, ref _flakesColorEnabled))
		{
			// Flake color map
			TexturePropertySingleLine(ContentText.flakeColorMapProperty, _flakeColorMapProperty, _flakeColorProperty);

			// Flake Color options
			if (_flakeColorMapProperty.textureValue != null)
			{
				// Flake color map scale if a texture is provided
				ShaderProperty(_flakeColorScaleProperty, ContentText.flakeColorScaleProperty.text, 2);
			}
			else
			{
				// If no flake color map is provided
				GUIContent[] options = new GUIContent[]{ ContentText.flakeColorAlphaStrenthProperty, ContentText.flakeColorCutoffProperty };

				// Option shown in one line
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel = 2;

					// Selection popup for normal map processing method
					_selectedFlakeColorMode = EditorGUILayout.Popup(
						_selectedFlakeColorMode - FlakeColorMode.NormalAlpha, 
						options,
						GUILayout.Width(EditorGUIUtility.labelWidth)) + FlakeColorMode.NormalAlpha;
					
					EditorGUI.indentLevel = 0;

					// Show correct option for the normal processing method
					if (_selectedFlakeColorMode == FlakeColorMode.NormalAlpha)
					{
						ShaderProperty(_flakeColorStrengthProperty, "");
					}
					else
					{
						ShaderProperty(_flakeColorCutoffProperty, "");
					}
				}
				GUILayout.EndHorizontal();
			}
		}

		// Setup keywords
		EnableKeyword("FLAKES_COLOR_MAP", 		_flakesColorEnabled && _flakeColorMapProperty.textureValue != null);
		EnableKeyword("FLAKES_COLOR_ALPHA", 	_flakesColorEnabled && _flakeColorMapProperty.textureValue == null && _selectedFlakeColorMode == FlakeColorMode.NormalAlpha);
		EnableKeyword("FLAKES_COLOR_CUTOFF", 	_flakesColorEnabled && _flakeColorMapProperty.textureValue == null && _selectedFlakeColorMode == FlakeColorMode.NormalCutoff);
	}

	private void DrawInspectorFresnel()
	{
		// Header
		bool isFresnelEnabled = _fresnelEnabled;
		if(BeffioCarPaintEditorUtilities.Header(ContentText.fresnelHeader, ref _fresnelDisplayed, ref isFresnelEnabled))
		{
			// Fresnel options
			ShaderProperty(_fresnelColorProperty, ContentText.fresnelColorProperty.text, 2);
			ShaderProperty(_fresnelPowerProperty, ContentText.fresnelPowerProperty.text, 2);
		}
		if (_fresnelEnabled != isFresnelEnabled)
		{
			_fresnelEnabled = isFresnelEnabled;
			// Setup keyword
			EnableKeyword("FRESNEL", _fresnelEnabled);
		}
	}

	private void DrawInspectorOverlay()
	{
		// Title
		EditorGUILayout.LabelField(ContentText.overlayTitle, EditorStyles.boldLabel);

		// Albedo
		TexturePropertySingleLine(ContentText.overlayAlbedoProperty, _overlayAlbedoMapProperty, _overlayColorProperty);

		// Specular and smoothnes
		EditorGUI.BeginChangeCheck();
		{
			if (_overlaySpecularMapProperty.textureValue == null)
			{
				// Show sliders if no texture is specified
				TexturePropertyTwoLines(
					ContentText.overlaySpecularMapProperty, 
					_overlaySpecularMapProperty, 
					_overlaySpecularProperty, 
					ContentText.overlaySmoothnessProperty, 
					_overlaySmoothnessProperty);
			}
			else
			{
				// Only show texture if texture is specified
				TexturePropertySingleLine(ContentText.overlaySpecularMapProperty, _overlaySpecularMapProperty);
			}
		}
		if (EditorGUI.EndChangeCheck())
		{
			// Setup keyword
			EnableKeyword("_OVERLAY_SPECGLOSSMAP", _overlaySpecularMapProperty.textureValue != null);
		}
	}
	#endregion

	/// <summary>
	/// Enables/Disable a shader keyword.
	/// </summary>
	/// <param name="keyword">Keyword.</param>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	private void EnableKeyword(string keyword, bool enabled)
	{
		if (enabled)
		{
			_targetMat.EnableKeyword(keyword);
		}
		else
		{
			_targetMat.DisableKeyword(keyword);
		}
	}
}
