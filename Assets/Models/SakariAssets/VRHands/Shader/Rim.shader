Shader "Sakari/VRHands"
 {
     Properties
     {
         _InnerColor("Inner Color", Color) = (1.0, 1.0, 1.0, 1.0)
         _MainTex("Particle Texture", 2D) = "white" {}
         _RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
         _RimPower("Rim Power", Range(0.5,8.0)) = 3.0
     }
         SubShader
     {
         Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
 
         Cull Back
         Blend One One
         Lighting Off
         ZWrite on
		 Pass {
		 	 ColorMask 0
		 }
 
         CGPROGRAM
 #pragma surface surf Lambert
 
     struct Input
     {
         float3 viewDir;
     };
 
     float4 _MainTex_ST;
     float4 _InnerColor;
     float4 _RimColor;
     float _RimPower;
 
     void surf(Input IN, inout SurfaceOutput o)
     {
         o.Albedo = _InnerColor.rgb;
         half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
         o.Emission = _RimColor.rgb * pow(rim, _RimPower);
     }
     ENDCG
     }
         Fallback "Diffuse"
 }