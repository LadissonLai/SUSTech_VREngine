// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PolyPixel/Standard"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		_EmissivePower("Emissive Power", Range( 0 , 1)) = 0
		[Toggle]_UseAlphaCut("Use Alpha Cut", Float) = 0
		_AlphaCutoff("Alpha Cutoff", Range( 0 , 1)) = 0.45
		_TintColor("Tint Color", Color) = (1,1,1,0)
		_BaseColor("Base Color", 2D) = "white" {}
		[Toggle]_UseMaskTint("Use Mask Tint", Float) = 1
		_MaskedStrength("Masked Strength", Range( 0 , 1)) = 0
		_NormalMap("Normal Map", 2D) = "white" {}
		_BumpScale("Bump Scale", Float) = 1
		_Metalic("Metalic", 2D) = "white" {}
		_MetallicAmount("Metallic Amount", Range( 0 , 1)) = 1
		_SmoothnessAmount("Smoothness Amount", Range( 0 , 1)) = 1
		_AmbientOcclusionAmount("Ambient Occlusion Amount", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _BumpScale;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _UseMaskTint;
		uniform float4 _TintColor;
		uniform sampler2D _BaseColor;
		uniform float4 _BaseColor_ST;
		uniform float _MaskedStrength;
		uniform float _EmissivePower;
		uniform float _MetallicAmount;
		uniform sampler2D _Metalic;
		uniform float4 _Metalic_ST;
		uniform float _SmoothnessAmount;
		uniform float _AmbientOcclusionAmount;
		uniform float _UseAlphaCut;
		uniform float _AlphaCutoff;
		uniform float _MaskClipValue = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ) ,_BumpScale );
			float2 uv_BaseColor = i.uv_texcoord * _BaseColor_ST.xy + _BaseColor_ST.zw;
			float4 tex2DNode3 = tex2D( _BaseColor, uv_BaseColor );
			float4 temp_output_8_0 = ( _TintColor * tex2DNode3 );
			float4 lerpResult10 = lerp( tex2DNode3 , temp_output_8_0 , ( tex2DNode3.a * _MaskedStrength ));
			o.Albedo = lerp(temp_output_8_0,lerpResult10,_UseMaskTint).rgb;
			o.Emission = ( _EmissivePower * tex2DNode3 ).rgb;
			float2 uv_Metalic = i.uv_texcoord * _Metalic_ST.xy + _Metalic_ST.zw;
			float4 tex2DNode6 = tex2D( _Metalic, uv_Metalic );
			o.Metallic = ( _MetallicAmount * tex2DNode6.r );
			o.Smoothness = ( _SmoothnessAmount * tex2DNode6.g );
			o.Occlusion = ( _AmbientOcclusionAmount * tex2DNode6.b );
			o.Alpha = 1;
			clip( lerp(1.0,( tex2DNode3.a - _AlphaCutoff ),_UseAlphaCut) - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
7;36;1266;873;1541.582;467.1853;1.865239;True;True
Node;AmplifyShaderEditor.CommentaryNode;18;-1076.084,-322.9057;Float;False;1111.437;837.8315;Albedo;7;22;10;9;8;4;2;3;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;2;-1024.971,-271.7115;Float;False;Property;_TintColor;Tint Color;4;0;1,1,1,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-1026.084,-47.19011;Float;True;Property;_BaseColor;Base Color;5;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-1021.131,178.3184;Float;False;Property;_MaskedStrength;Masked Strength;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;27;-369.0424,705.5013;Float;False;761.5932;344.9074;Alpha;4;23;24;26;25;Alpha;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-319.0424,847.3967;Float;False;Property;_AlphaCutoff;Alpha Cutoff;3;0;0.45;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-583.9768,97.7086;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;17;-1069.93,-616.0066;Float;False;564.0081;212.6628;Emissive;2;7;1;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-673.3303,-210.1768;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;20;-1063.134,1402.338;Float;False;582.5529;280;Normal;2;19;5;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1066.798,703.8403;Float;False;651.493;583.1793;PBR;7;12;13;11;6;15;16;14;PBR;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-185.73,935.4091;Float;False;Constant;_Float1;Float 1;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;19;-1013.134,1547.215;Float;False;Property;_BumpScale;Bump Scale;9;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;10;-416.3303,-54.17669;Float;False;3;0;COLOR;0.0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;1;-1019.93,-566.0065;Float;False;Property;_EmissivePower;Emissive Power;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;24;-31.708,755.5013;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;13;-971.1517,1167.245;Float;False;Property;_AmbientOcclusionAmount;Ambient Occlusion Amount;13;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;-961.4633,973.0831;Float;False;Property;_MetallicAmount;Metallic Amount;11;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-1016.798,753.8403;Float;True;Property;_Metalic;Metalic;10;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-968.5252,1070.503;Float;False;Property;_SmoothnessAmount;Smoothness Amount;12;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-584.3049,1154.02;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;26;136.5506,868.106;Float;False;Property;_UseAlphaCut;Use Alpha Cut;2;1;[Toggle];0;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ToggleSwitchNode;22;-187.9948,-65.98168;Float;False;Property;_UseMaskTint;Use Mask Tint;6;1;[Toggle];1;2;0;COLOR;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-674.9219,-536.3438;Float;False;2;2;0;FLOAT;0.0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-586.424,1032.168;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-585.3644,906.0771;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;5;-800.5812,1452.338;Float;True;Property;_NormalMap;Normal Map;8;0;None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;502.1883,-95.77801;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PolyPixel/Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;True;TransparentCutout;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;3;4
WireConnection;9;1;4;0
WireConnection;8;0;2;0
WireConnection;8;1;3;0
WireConnection;10;0;3;0
WireConnection;10;1;8;0
WireConnection;10;2;9;0
WireConnection;24;0;3;4
WireConnection;24;1;23;0
WireConnection;16;0;13;0
WireConnection;16;1;6;3
WireConnection;26;0;25;0
WireConnection;26;1;24;0
WireConnection;22;0;8;0
WireConnection;22;1;10;0
WireConnection;7;0;1;0
WireConnection;7;1;3;0
WireConnection;15;0;12;0
WireConnection;15;1;6;2
WireConnection;14;0;11;0
WireConnection;14;1;6;1
WireConnection;5;5;19;0
WireConnection;0;0;22;0
WireConnection;0;1;5;0
WireConnection;0;2;7;0
WireConnection;0;3;14;0
WireConnection;0;4;15;0
WireConnection;0;5;16;0
WireConnection;0;10;26;0
ASEEND*/
//CHKSM=961370DE797F2BAB02B292D2F2E0641AA6EB2A12