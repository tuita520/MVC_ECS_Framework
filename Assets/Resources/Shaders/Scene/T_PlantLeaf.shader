// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "T_shaders/Scene/Transparent/PlantLeaf" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range(0,1)) = 0.5
		[HideInInspector]
		_LightMapStrength("LightMap Strength", Range(0,1)) = 0.5
	}
	SubShader 
	{
		pass
		{
			Tags
			{ 
				"LightMode" = "ForwardBase" 		
				"Queue" = "AlphaTest" 
				"IgnoreProjector" = "True" 
				"RenderType" = "TransparentCutout" 
			}
			Cull Back
			//Blend SrcAlpha OneMinusSrcAlpha
			LOD 200
			CGPROGRAM
			#pragma vertex vertFunction
			#pragma fragment fragFunction
			#pragma multi_compile_fog
			#pragma multi_compile LIGHTMAP_ON LIGHTMAP_OFF
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _Cutoff;
			fixed _contrast;
			fixed _brightness;
			float _LightMapStrength;
			struct v2f
			{
				fixed4 pos : POSITION;
                fixed2 uv : TEXCOORD0;
                #ifdef LIGHTMAP_ON
                float2 uv2 : TEXCOORD1;
                #endif
                
			};
			struct frag
			{
				fixed4 pos : POSITION;
				fixed2 uv : TEXCOORD0;
				#ifdef LIGHTMAP_ON
                float2 uv2 : TEXCOORD1;
                #endif
				UNITY_FOG_COORDS(2)
			};
			
			frag vertFunction(v2f input)
			{
				frag output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.uv = input.uv;
				UNITY_TRANSFER_FOG(output,output.pos);
				#ifdef LIGHTMAP_ON
                output.uv2.xy = input.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                #endif
				return output;
			}
			
			fixed4 fragFunction(frag input):COLOR
			{	
				fixed4 Tex = tex2D(_MainTex, input.uv)*_Color;
				clip(Tex.a - _Cutoff);
				#ifdef LIGHTMAP_ON
                Tex.rgb *= ((DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.uv2)))+_LightMapStrength);
                #endif
                UNITY_APPLY_FOG(input.fogCoord, Tex);
  				return Tex;
			}
			ENDCG
		}
		//下面这个Pass块 应用于实时光情况(像素阴影)
		//pass
		//{
		//	Tags
		//	{ 
		//		"LIGHTMODE"="SHADOWCASTER" "SHADOWSUPPORT"="true" "IGNOREPROJECTOR"="true" "RenderType"="TreeLeaf" 
		//	}
		//	CGPROGRAM
		//	#pragma vertex vertFunction
		//	#pragma fragment fragFunction
		//	#include "UnityCG.cginc"
		//	
		//	sampler2D _MainTex;
		//	fixed _Cutoff;
		//	
		//	struct v2f
		//	{
		//		fixed4 pos : POSITION;
        //        fixed2 uv : TEXCOORD0;
		//	};
		//	struct frag
		//	{
		//		fixed4 pos : POSITION;
		//		fixed2 uv : TEXCOORD0;
		//	};
		//	
		//	v2f vertFunction(v2f input)
		//	{
		//		v2f output;
		//		output.pos = mul(UNITY_MATRIX_MVP, input.pos);
		//		output.uv = input.uv;
		//		return output;
		//	}
		//	
		//	fixed4 fragFunction(frag input):COLOR
		//	{	
		//		fixed4 Tex = tex2D(_MainTex, input.uv);
		//		fixed x_2;
  		//		x_2 = (Tex.a - _Cutoff);
  		//		if ((x_2 < 0.0)) 
  		//		{
    	//			discard;
  		//		};
		//		return Tex;
		//	}
		//	ENDCG
		//}	
} 
	//顶点阴影
	//FallBack "Diffuse"
}
