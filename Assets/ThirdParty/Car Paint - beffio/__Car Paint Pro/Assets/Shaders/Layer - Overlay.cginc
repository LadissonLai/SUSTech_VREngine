struct Input 
{
	float2 uv_OverlayMainTex;
	float2 uv_OverlaySpecGlossMap;
};

// Base Material
sampler2D _OcclusionMap;
half _OcclusionStrength;

// Overlay
fixed4 _OverlayColor;
sampler2D _OverlayMainTex;
half _OverlayGlossiness;
half3 _OverlaySpecular;
sampler2D _OverlaySpecGlossMap;

void OverlaySurface (Input IN, inout SurfaceOutputStandardSpecular o) 
{
	// ALBEDO & ALPHA
	fixed4 colorizedTextureColor = tex2D (_OverlayMainTex, IN.uv_OverlayMainTex) * _OverlayColor;
	o.Albedo = colorizedTextureColor.rgb;
	o.Alpha = colorizedTextureColor.a;

	// SPECULAR & SMOOTHNESS
	#ifdef _OVERLAY_SPECGLOSSMAP
		// Specular (RGB) - Smoothness (A)
		half4 textureSpecGlossValue = tex2D (_OverlaySpecGlossMap, IN.uv_OverlaySpecGlossMap);
		o.Specular = textureSpecGlossValue.rgb;
		o.Smoothness = textureSpecGlossValue.a;
	#else
		o.Specular = _OverlaySpecular * colorizedTextureColor.a;
		o.Smoothness = _OverlayGlossiness * colorizedTextureColor.a;
	#endif

	// OCCLUSION
	#ifdef _OCCLUSIONMAP
		// Occlusion (G)
		half4 occlusion = tex2D(_OcclusionMap, IN.uv_OverlayMainTex).g;
		o.Occlusion = lerp(1.0, occlusion.g, _OcclusionStrength);
		// Use uv_OverlayMainTex to fix occlusion.
		// See: http://forum.unity3d.com/threads/standard-surface-shader-with-ambient-occlusion-based-on-2nd-uv-set.382094/
	#endif
}