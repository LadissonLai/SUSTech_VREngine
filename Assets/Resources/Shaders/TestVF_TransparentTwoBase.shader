﻿Shader "Test/TestVF_TransparentTwoPass"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("颜色",COLOR) = (1,1,1,1)
	}
	SubShader
	{
		LOD 100
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector" = "True" }

		Pass
		{
			ZWrite On
			ColorMask 0	
		}

		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha		//新颜色*新颜色的透明值 + 屏幕颜色 *（1-新颜色的透明值）
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= _Color;
				return col;
			}
			ENDCG
		}
	}
}
