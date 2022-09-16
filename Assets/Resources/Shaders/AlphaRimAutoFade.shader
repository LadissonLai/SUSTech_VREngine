//边缘泛光基本效果 带顶点偏移
 
Shader "Fxb/Alpha Rim Auto Fade"
{
	Properties
	{
		_RimPower ("泛光强度(貌似应该是衰减)", Range(0.1, 3.0)) = 1
		_RimColor ("泛光颜色", Color) = (1.0, 1.0, 1.0, 1.0)
		_HidenDis ("最大距离，新增效果，离摄像机的距离在此距离以内的会插值进行透明，放置过近的地方遮住目标", Range(0.1, 3.0)) = 2.0
	}
	SubShader
	{
		Tags {"Queue"="Transparent" }

		// Pass
		// {
		// 	ZWrite On
		// 	ColorMask 0	
		// }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc" 
		  
 		 	float _RimPower;
 		 	fixed4 _RimColor;
			float _HidenDis;

			struct v2f
			{
				float4 pos:POSITION; 
				float3 normal:NORMAL;
				float4 vertex:TEXCOORD1;
				float3 viewPos:COLOR1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			v2f vert (in appdata_base input)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input,o);
				
 				o.pos = UnityObjectToClipPos(input.vertex);
 				o.vertex  = input.vertex;		//需要world Pos的顶点位置来与法线点乘
 				o.normal = input.normal;
				o.viewPos = UnityObjectToViewPos( input.vertex );
				 
				return o;
			}  
			 
			fixed4 frag (in v2f input):COLOR  	
			{
				UNITY_SETUP_INSTANCE_ID(input);

				float3 V = normalize(WorldSpaceViewDir(input.vertex));	//WorldSpaceViewDir 与 UnityWorldSpaceViewDir 区别
				float3 N = normalize(mul(unity_ObjectToWorld,input.normal));
				float dotVN = saturate(dot(V,N));
				float rimScale = pow(1 - dotVN,_RimPower);
				float4 rimColor = _RimColor * rimScale * 1.2f;

				//0-1
				// float diaWeight = clamp(distance( input.viewPos , float3( 0,0,0 ) ) , 0.0 , _HidenDis ) / _HidenDis;
				float disToView = distance(input.viewPos, float3(0,0,0));

				float diaWeight = saturate(pow(disToView, _HidenDis));

				fixed4 color = saturate(rimColor) * diaWeight;

				return color;
			}

			ENDCG
		}
	}
}
