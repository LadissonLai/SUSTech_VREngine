Shader "HighlightPlus/Geometry/Glow" {
Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Glow ("Glow", Vector) = (1, 0.025, 0.75, 0.5)
    _Glow2 ("Glow2", Vector) = (0.01, 1, 0.5, 0)
    _GlowColor ("Glow Color", Color) = (1,1,1)
    _Color ("Color", Color) = (1,1,1) // not used; dummy property to avoid inspector warning "material has no _Color property"
    _GlowDirection("GlowDir", Vector) = (1,1,0)
    _Cull ("Cull Mode", Int) = 2
    _ConstantWidth ("Constant Width", Float) = 1
	_GlowZTest ("ZTest", Int) = 4
}
    SubShader
    {
        Tags { "Queue"="Transparent+102" "RenderType"="Transparent" }
      
        // Glow passes
        Pass
        {
        	Stencil {
                Ref 2
                Comp NotEqual
                Pass keep 
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull [_Cull]
            ZTest [_GlowZTest]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
				float4 pos   : SV_POSITION;
                fixed4 color : COLOR;
				UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Glow; // x = intensity, y = width, z = magic number 1, w = magic number 2
            float3 _Glow2; // x = outline width, y = glow speed, z = dither on/off
            fixed4 _GlowColor;
            float2 _GlowDirection;
            float _ConstantWidth;

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float4 pos = UnityObjectToClipPos(v.vertex);
                float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
                float2 offset = TransformViewToProjection(normalize(norm.xy));
                offset += _GlowDirection;
                float z = lerp(UNITY_Z_0_FAR_FROM_CLIPSPACE(pos.z), 2.0, UNITY_MATRIX_P[3][3]);
                z = _ConstantWidth * (z - 2.0) + 2.0;
                float outlineWidth = _Glow2.x;
                float animatedWidth = _Glow.y * (1.0 + 0.25 * sin(_Time.w * _Glow2.y));
                offset *= z * (outlineWidth + animatedWidth);
                pos.xy += offset;
				o.pos = pos;
                o.color = _GlowColor;
                o.color.a = _Glow.x;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = i.color;
                float2 screenPos = floor( i.pos.xy * _Glow.z ) * _Glow.w;
                color.a *= saturate(_Glow2.z + frac(screenPos.x + screenPos.y));
                return color;
            }
            ENDCG
        }
 
    }
}