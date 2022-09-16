// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Fxb/DropAbleQuadDrawer"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		Offset 0 , 0
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			uniform sampler2D _TextureSample0;
			uniform float4 _TextureSample0_ST;
			uniform float4 _Color;
			
			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
			};

			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				o.ase_texcoord.zw = 0;
	  
				//只能用作quad mesh的处理 顶点范围-0.5,-0.5  到  0.5,0.5，中心0,0
				//quad 顶点到左下角
				float4x4 MATRIX_QUAD_VERTEX_TO_TL = float4x4
				(
					1,0,0,0.5, 
					0,1,0,0.5, 
					0,0,1,0, 
					0,0,0,1
				);
  
				//转剪裁空间 直接翻转y
				float4x4 MATRIX_QUAD_VERTEX_TL_TO_CLIP = float4x4
				(
					2,0,0,-1, 
					0,-2,0,1, 
					0,0,2,0, 
					0,0,0,1
				);

				v.vertex = float4(v.vertex.xy, 0,1);
 
				float4x4 vertexToClip = mul(MATRIX_QUAD_VERTEX_TL_TO_CLIP, unity_ObjectToWorld);
 
				vertexToClip = mul(vertexToClip, MATRIX_QUAD_VERTEX_TO_TL);

				o.vertex = mul(vertexToClip , v.vertex);

				// o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				
				fixed4 finalColor;

				float2 uv_TextureSample0 = i.ase_texcoord.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				
				// finalColor = tex2D( _TextureSample0, uv_TextureSample0 );
				 
				finalColor = _Color;
				return finalColor;
			}
			ENDCG
		}
	}
}
