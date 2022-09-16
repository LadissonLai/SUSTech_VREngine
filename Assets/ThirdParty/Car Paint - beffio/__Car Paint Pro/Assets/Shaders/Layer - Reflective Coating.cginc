struct Input 
{
	float2 uv_MainTex;
	float2 uv_OcclusionMap;
	float2 uv_ReflectionSpecularMap;
};

// Base Material
sampler2D _BumpMap;
half _BumpScale;
sampler2D _OcclusionMap;
half _OcclusionStrength;

// Reflection
sampler2D _ReflectionSpecularMap;
float _ReflectionGlossiness;
fixed4 _ReflectionSpecular;

void ReflectiveCoatingSurface (Input IN, inout SurfaceOutputStandardSpecular o) 
{
	// NORMALS
	#ifdef _NORMALMAP
		half3 bumpNormal = UnpackScaleNormal(tex2D (_BumpMap, IN.uv_MainTex), _BumpScale);
	#else
		half3 bumpNormal = half3(0,0,1);
	#endif
	o.Normal = bumpNormal;

	// ALBEDO & ALPHA
	o.Alpha = 0;
	o.Albedo = fixed3(0,0,0);

	// SPECULAR & SMOOTHNES
	#ifdef _REFLECTION_SPECGLOSSMAP
		// Specular (RGB) - Smoothness (A)
		half4 textureSpecGlossValue = tex2D (_ReflectionSpecularMap, IN.uv_ReflectionSpecularMap);
		o.Specular = textureSpecGlossValue.rgb;
		o.Smoothness = textureSpecGlossValue.a;
	#else
		o.Specular = _ReflectionSpecular.xyz;
		o.Smoothness = _ReflectionGlossiness;
	#endif

	// OCCLUSION
	#ifdef _OCCLUSIONMAP
		// Occlusion (G)
		half4 occlusion = tex2D(_OcclusionMap, IN.uv_OcclusionMap).g;
		o.Occlusion = lerp(1.0, occlusion.g, _OcclusionStrength);
		o.Alpha = occlusion.g * 0.01;
		// Overwrite alpha with part of occlusion map value. 
		// See: http://forum.unity3d.com/threads/standard-surface-shader-with-ambient-occlusion-based-on-2nd-uv-set.382094/
	#endif
}