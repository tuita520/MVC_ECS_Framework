// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "D-Z/Mobile-WindGrassOfAlphaTest"
{
	Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_WindParams("WindParams",vector) = (0,0,0,0)
	}

	SubShader 
	{
		Tags {
			  	"Queue"="Transparent" 
			  	"IgnoreProjector"="True" 
			  	"RenderType"="Transparent" 
			  	"LightMode"="ForwardBase"
			  }
		LOD 100
		Cull Off
		
		AlphaTest Greater [_Cutoff]
		
		
		CGINCLUDE
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		half4 _MainTex_ST;
		float4 _GrassWind; 
		fixed _Cutoff;
		float4 _WindParams;
		
		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			UNITY_FOG_COORDS(1)
		};

		inline float4 AnimateGrass(float4 pos, float3 normal, float animParams)
		{	
			//pos = mul(_Object2World,pos);
			pos.xyz += animParams * _GrassWind.xyz * _GrassWind.w * pos.y;
			//pos = mul(_World2Object,pos);
			return pos;
		}


		
		v2f vert (appdata_full v)
		{
			v2f o;
			float4	windParams	= _WindParams;		
			float4 mdlPos = AnimateGrass(v.vertex, v.normal, windParams);
			o.pos = UnityObjectToClipPos(mdlPos);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;
		}
		ENDCG

		Pass {
			CGPROGRAM
			#pragma debug
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 tex = tex2D (_MainTex, i.uv);
				clip(tex.a - _Cutoff);
				fixed4 c;
				c.rgb = tex.rgb ;
				c.a = tex.a;
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
			}
			ENDCG 
		}
					
	}
}
