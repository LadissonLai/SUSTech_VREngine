struct Input 
{
	float2 uv_MainTex;
	float2 uv_EmissionMap;
	float2 uv_FlakesBumpMap;
	float2 uv_FlakesColorMap;
	float3 viewDir;
};

// Base Material
fixed4 _Color;
sampler2D _MainTex;
sampler2D _MetallicGlossMap;
half _Glossiness;
half _Metallic;
sampler2D _BumpMap;
half _BumpScale;
sampler2D _OcclusionMap;
half _OcclusionStrength;
sampler2D _EmissionMap;
half3 _EmissionColor;

// Flakes Bump
sampler2D _FlakesBumpMap;
half _FlakesBumpMapScale;
half _FlakesBumpStrength;

// Flakes Color
fixed4 _FlakeColor;
sampler2D _FlakesColorMap;
half _FlakesColorMapScale;
float _FlakesColorStrength;
float _FlakesColorCutoff;

// Fresnel
fixed4 _FresnelColor;
float _FresnelPower;

void BasePaintSurface (Input IN, inout SurfaceOutputStandard o) 
{
	// NORMALS
	#ifdef _NORMALMAP
		half3 bumpNormal = UnpackScaleNormal(tex2D (_BumpMap, IN.uv_MainTex), _BumpScale);
	#else
		half3 bumpNormal = half3(0,0,1);
	#endif

	half3 outputNormal = bumpNormal;
	#ifdef FLAKES_BUMP
		// Apply scaled flake normal map
		float2 scaledUV = IN.uv_FlakesBumpMap * _FlakesBumpMapScale;
		half3 flakeNormal = UnpackNormal(tex2D (_FlakesBumpMap, scaledUV));

		// Apply flake map strength
		half3 scaledFlakeNormal = flakeNormal;
		scaledFlakeNormal.xy *= _FlakesBumpStrength;
		scaledFlakeNormal.z = 0; // Z set to 0 for better blending with other normal map.

		// Blend regular normal map with flakes normal map
		outputNormal = normalize(outputNormal + scaledFlakeNormal);
	#endif
	o.Normal = outputNormal;

	// ALBEDO
	fixed4 textureColorValue = tex2D (_MainTex, IN.uv_MainTex);
	fixed3 finalColor = _Color.xyz * textureColorValue.xyz;

	// Apply Fresnel
	#ifdef FRESNEL
		float fresnel =  1.0 - max(dot(normalize(IN.viewDir.xyz), bumpNormal), 0.0);
		fresnel = pow(fresnel, _FresnelPower);
		finalColor = lerp(finalColor, _FresnelColor.xyz, fresnel);
	#endif

	// Apply Flake Colors
	#ifdef FLAKES_COLOR_MAP
		// Flakes from pattern map
		float2 scaledFlakeColorUV = IN.uv_FlakesColorMap * _FlakesColorMapScale;
		fixed4 flakesMap = tex2D(_FlakesColorMap, scaledFlakeColorUV);

		finalColor = lerp(finalColor, _FlakeColor.xyz, flakesMap.r * _FlakeColor.a);
	#elif FLAKES_BUMP
		// Interpret flakes from the normal map
		#ifdef FLAKES_COLOR_ALPHA
			// Blend color based on flake normal Z
			half normalColorRatio = (1-flakeNormal.z) * _FlakesColorStrength;
			finalColor = lerp(finalColor, _FlakeColor.xyz, saturate(normalColorRatio) * _FlakeColor.a);
		#elif FLAKES_COLOR_CUTOFF
			// Switch color based on flake normal Z
			if (flakeNormal.z < _FlakesColorCutoff)
			{
				finalColor = lerp(finalColor, _FlakeColor.xyz, _FlakeColor.a);
			}
		#endif
	#endif

	o.Albedo = finalColor;


	// METALLIC & SMOOTHNESS
	#ifdef _METALLICGLOSSMAP
		// Metallic (R) - Smoothness (A)
		half4 textureMetallicGlossValue = tex2D (_MetallicGlossMap, IN.uv_MainTex);
		o.Metallic = textureMetallicGlossValue.r;
		o.Smoothness = textureMetallicGlossValue.a;
	#else
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
	#endif

	// OCCLUSION
	#ifdef _OCCLUSIONMAP
		// Occlusion (G)
		o.Occlusion = lerp(1.0, tex2D(_OcclusionMap, IN.uv_MainTex).g, _OcclusionStrength); 
		// Use uv_MainTex to fix occlusion.
		// See: http://forum.unity3d.com/threads/standard-surface-shader-with-ambient-occlusion-based-on-2nd-uv-set.382094/
	#endif

	// EMISSIVE
	#ifdef _EMISSIVEMAP
		// Emissive (RGB)
		half4 emissiveValue = tex2D (_EmissionMap, IN.uv_EmissionMap);
		o.Emission = emissiveValue.rgb * _EmissionColor.rgb;
	#else
		o.Emission = _EmissionColor.rgb;
	#endif


	// ALPHA
	o.Alpha = _Color.a * textureColorValue.a;
}