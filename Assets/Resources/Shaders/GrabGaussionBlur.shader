//存在相同shader的Ui重叠之后显示异常的问题   ----  GrabPass{} _GrabTexture 默认调用可解决 
//存在非OverlayCanvas渲染无效的问题 ---  缩小CanvasScale 可解决 （比例异常)                                 
Shader "Fxb/GrabGaussionBlur"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_Color("Tint",Color) = (1,1,1,1)

			//Mask处理
			_StencilComp("Stencil Comparison", Float) = 8
			_Stencil("Stencil ID", Float) = 0
			_StencilOp("Stencil Operation", Float) = 0
			_StencilWriteMask("Stencil Write Mask", Float) = 255
			_StencilReadMask("Stencil Read Mask", Float) = 255

			_ColorMask("Color Mask", Float) = 15

			_BlurSpread("Blur",Range(0.2,2)) = 1
	}
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        //Mask处理
        Stencil
        {
        	Ref[_Stencil]
        	Comp[_StencilComp]
        	Pass[_StencilOp]
        	ReadMask[_StencilReadMask]
        	WriteMask[_StencilWriteMask]
        }
        
        ZTest[unity_GUIZTestMode] ZWrite Off Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]
        
        GrabPass
        {
        	//"_GrabTexture"
        }
        
        CGINCLUDE
        
        #include "UnityCG.cginc"
        #include "UnityUI.cginc"
        
        struct appdata
        {
        	float4 vertex : POSITION;
        	float2 uv : TEXCOORD0;
        	float4 color:COLOR;
        };
        
        struct v2f
        {
        	float4 pos : SV_POSITION;
        	//float4 uv[5] : TEXCOORD0;
        	float4 uv[9] : TEXCOORD0;
        
        	//float4 worldPos : TEXCOORD5;
        	float4 worldPos : TEXCOORD9;
        	fixed4 color : COLOR0;
        };
        
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        
        half _BlurSpread;
        
        //alph8格式的处理
        fixed4 _TextureSampleAdd;
        
        fixed4 _Color;
        
        //RectMask2D
        bool _UseClipRect;
        float4 _ClipRect;
        
        fixed4 frag(v2f i) : SV_Target
        {
        	//核
        	//const float weight[7] = { 0.1719, 0.4566, 0.8204, 1.0, 0.8204, 0.4566, 0.1719 };
        	 float weight[9] = { 0.05, 0.09, 0.12, 0.15, 0.18, 0.15, 0.12,0.09,0.05 };
            //float weight[5] = { 0.0545,0.2442,0.4026,0.2442,0.0545 };
            
            fixed4 sum = fixed4(0, 0, 0, 0);
            
            for (int j = 0; j < 9; j++)
            sum += (tex2Dproj(_GrabTexture, i.uv[j]) + _TextureSampleAdd) * weight[j];
            
            //Image	
            //Color的处理
            sum = fixed4(i.color * i.color.a + sum * (1 - i.color.a));
            
            //Source Image
            //sum *= i.color;
            
            if (_UseClipRect)
            	sum *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
            
            return sum;
        }
    
        ENDCG
    
        Pass
        {
        	Name "GAUSSIONBLURVERTICAL"
        
        	CGPROGRAM
        	#pragma vertex verticalVert
        	#pragma fragment frag
        
        	v2f verticalVert(appdata v)
        	{
        		v2f o;
        
        		o.worldPos = v.vertex;
        		o.color = v.color * _Color;
        
        		o.pos = UnityObjectToClipPos(v.vertex);
        
        		float4 uv = ComputeGrabScreenPos(o.pos);
        
        		//5核
        		//o.uv[0] = float4(uv.xy + half2(0, -2)* _GrabTexture_TexelSize.xy*_BlurSpread, uv.zw);
        		//o.uv[1] = float4(uv.xy + half2(0, -1)* _GrabTexture_TexelSize.xy *_BlurSpread, uv.zw);
        		//o.uv[2] = uv;
        		//o.uv[3] = float4(uv.xy + half2(0, 1)* _GrabTexture_TexelSize.xy *_BlurSpread, uv.zw);
        		//o.uv[4] = float4(uv.xy + half2(0, 2)* _GrabTexture_TexelSize.xy *_BlurSpread, uv.zw);
        
        		o.uv[0] = float4(uv.xy + half2(0, -4) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[1] = float4(uv.xy + half2(0, -3) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[2] = float4(uv.xy + half2(0, -2) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[3] = float4(uv.xy + half2(0, -1) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[4] = uv;
        		o.uv[5] = float4(uv.xy + half2(0, 1) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[6] = float4(uv.xy + half2(0, 2) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[7] = float4(uv.xy + half2(0, 3) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[8] = float4(uv.xy + half2(0, 4) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        
        		//o.uv[0] = float4(uv.xy + half2(0, -2)*(_ScreenParams.zw-1)*_BlurSpread, uv.zw);
        
        		return o;
        	}
        
        	ENDCG
        }
    
        Pass
        {
        	Name "GAUSSIONBLURHORIZONTAL"
        	CGPROGRAM
        	#pragma vertex horizontalVert
        	#pragma fragment frag
        
        
        	v2f horizontalVert(appdata v)
        	{
        		v2f o;
        
        		o.worldPos = v.vertex;
        		o.color = v.color * _Color;
        
        		o.pos = UnityObjectToClipPos(v.vertex);
        		float4 uv = ComputeGrabScreenPos(o.pos);
        
                //5核
        		//o.uv[0] = float4(uv.xy + half2(-2,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		//o.uv[1] = float4(uv.xy + half2(-1,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		//o.uv[2] = uv;
        		//o.uv[3] = float4(uv.xy + half2(1,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		//o.uv[4] = float4(uv.xy + half2(2,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        
        		o.uv[0] = float4(uv.xy + half2(-4,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[1] = float4(uv.xy + half2(-3,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[2] = float4(uv.xy + half2(-2,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[3] = float4(uv.xy + half2(-1,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[4] = uv;
        		o.uv[5] = float4(uv.xy + half2(1,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[6] = float4(uv.xy + half2(2,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[7] = float4(uv.xy + half2(3,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        		o.uv[8] = float4(uv.xy + half2(4,0) * _GrabTexture_TexelSize.xy * _BlurSpread, uv.zw);
        
        		return o;
        	}
        
        	ENDCG
        }
    }
}
