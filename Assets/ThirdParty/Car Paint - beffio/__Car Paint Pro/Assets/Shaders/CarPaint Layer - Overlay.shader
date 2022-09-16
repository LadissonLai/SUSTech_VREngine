Shader "Beffio/Upgrade Clear Coat/Layer - Overlay" 
{
	Properties 
	{
		// Base properties
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		// Overlay properties
		_OverlayColor ("Overlay Color", Color) = (1,1,1,1)
		_OverlayMainTex ("Overlay Albedo (RGB)", 2D) = "white" {}
		_OverlayGlossiness ("Overlay Smoothness", Range(0,1)) = 0.5
		_OverlaySpecular ("Overlay Specular", Color) = (0.3,0.3,0.3)
		_OverlaySpecGlossMap ("Overlay Specular Texture", 2D) = "black" {}
	}
	SubShader 
	{
		ZWrite Off
		Cull Back

		Tags 
		{
			"Queue"="Transparent"
			"RenderType"="Transparent"
			"IgnoreProjector"="false"
		}
		LOD 300
		
		CGPROGRAM
			// Surface shader
			// alpha: premul gives best transparency results
			// deferred and prepass are not used, so they are excluded
			#pragma surface OverlaySurface StandardSpecular fullforwardshadows alpha:premul exclude_path:deferred exclude_path:prepass
			#pragma target 3.0

			// Shader features
			#pragma shader_feature _OCCLUSIONMAP
			#pragma shader_feature _OVERLAY_SPECGLOSSMAP

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
		ENDCG
	}
	FallBack "Diffuse"
}
