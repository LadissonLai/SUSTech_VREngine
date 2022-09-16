Shader "Beffio/Upgrade Clear Coat/Car Paint - Opaque" 
{
	Properties 
	{
		// Base properties
		_Color ("Base Albedo", Color) = (1,1,1,1)
		_MainTex ("Base Albedo Texture", 2D) = "white" {}
		_Glossiness ("Base Smoothness", Range(0,1)) = 0.5
		[Gamma] _Metallic ("Base Metallic", Range(0,1)) = 0.0
		_MetallicGlossMap ("Base Metallic Texture", 2D) = "white" {}
		_BumpScale("Scale", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}
		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		// Flake Bump properties
		[NoScaleOffset] [Normal] _FlakesBumpMap ("Base Bump Flakes (normal)", 2D) = "bump" {}
		_FlakesBumpMapScale("Base Bump Flakes Scale", Float) = 1.0
		_FlakesBumpStrength("Base Bump Flakes Strength", Range(0.001, 8)) = 1.0

		// Flake Color properties
		_FlakeColor("Base Flakes Albedo", Color) = (1,1,1,1)
		_FlakesColorMap ("Base Flakes Albedo Texture", 2D) = "black" {}
		_FlakesColorMapScale("Base Flakes Color Scale", Float) = 1.0
		_FlakesColorStrength("Base Flakes Color Strength", Range(0,10)) = 1
		_FlakesColorCutoff("Base Flakes Color Cutoff", Range(0,.95)) = 0.5

		// Fresnel properties
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelPower ("Fresnel Power", Range(0,10)) = 1

		// Reflection properties
		_ReflectionSpecularMap("Reflection Specular Map", 2D) = "black" {}
		_ReflectionSpecular("Reflection Specular", Color) = (0.3, 0.3, 0.3)
		_ReflectionGlossiness("Reflection Smoothness", Range(0, 1)) = 1
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Geometry"
			"RenderType"="Opaque"
			"IgnoreProjector"="false"
		}
		LOD 300

		// Layer 1: Base Paint
		CGPROGRAM
			// Surface shader
			// deferred and prepass are not used, so they are excluded
			#pragma surface BasePaintSurface Standard fullforwardshadows exclude_path:deferred exclude_path:prepass
			#pragma target 3.0

			// Shader features
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _OCCLUSIONMAP
			#pragma shader_feature _EMISSIVEMAP
			#pragma shader_feature FLAKES_BUMP
			#pragma shader_feature __ FLAKES_COLOR_MAP FLAKES_COLOR_ALPHA FLAKES_COLOR_CUTOFF
			#pragma shader_feature FRESNEL

			#include "Layer - Base Paint.cginc"
		ENDCG

		// Layer 2: Reflective Coating
		CGPROGRAM
			// Surface shader
			// alpha: premul gives best transparency results
			// deferred and prepass are not used, so they are excluded
			#pragma surface ReflectiveCoatingSurface StandardSpecular nofog alpha:premul exclude_path:deferred exclude_path:prepass
			#pragma target 3.0

			// Shader features
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _OCCLUSIONMAP
			#pragma shader_feature _REFLECTION_SPECGLOSSMAP

			#include "Layer - Reflective Coating.cginc"
		ENDCG
	}

	FallBack "Diffuse"
    CustomEditor "CustomCarPaintEditor"
}
