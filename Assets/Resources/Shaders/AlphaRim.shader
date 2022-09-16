//边缘泛光基本效果 带顶点偏移
 
Shader "Fxb/Alpha Rim"
{
	Properties
	{
		_RimPower ("泛光强度(貌似应该是衰减)", Range(0.1, 2.0)) = 1
		_RimColor ("泛光颜色", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" }

		Pass
		{
			ZWrite On
			ColorMask 0	
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off 
  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" 
		  
 		 	float _RimPower;
 		 	fixed4 _RimColor;

			struct v2f
			{
				float4 pos:POSITION; 
				float3 normal:NORMAL;
				float4 vertex:TEXCOORD1;
			};

			v2f vert (in appdata_base input)
			{
				v2f o;
				
 				o.pos = UnityObjectToClipPos(input.vertex);
 				o.vertex  = input.vertex;		 
 				o.normal = input.normal;
 				
				return o;
			}  
			 
			fixed4 frag (in v2f input):COLOR  	
			{
				float3 V = normalize(WorldSpaceViewDir(input.vertex));	
				float3 N = normalize(mul(unity_ObjectToWorld,input.normal));
				float dotVN = saturate(dot(V,N));
				float rimScale = pow(1 - dotVN,_RimPower);
				float4 rimColor = _RimColor * rimScale;
				fixed4 color = rimColor;
 
				return color;
			}

			ENDCG
		}
	}
}
