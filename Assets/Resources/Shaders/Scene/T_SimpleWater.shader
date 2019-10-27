// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "T_shaders/Water/Simple Water" 
{
	Properties 
	{
		[NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
		[NoScaleOffset] _ReflectiveColor ("Reflect color", 2D) = "" {}
		[NoScaleOffset]_ReflTex ("Reflect Texture", 2D) = "bump" {}
		_WaveScale ("Wave scale", Range (0.02,0.15)) = 0.063
		_WaveSpeed ("Wave speed", Vector) = (19,9,-16,-7)
		_HorizonColor ("Simple water horizon color", Color)  = ( .172, .463, .435, 1)
		_MainColor ("MainColor", Color)  = (0.25,0.25,0.25,1.0)
	}

	// -----------------------------------------------------------
	// Fragment program cards
	Subshader 
	{
		Tags 
		{ 
			"Queue" = "Transparent-10" 
			"RenderType"="Transparent" 
		}
		Pass 
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog			
			#include "UnityCG.cginc"
			
			uniform float4 _WaveScale4;
			uniform float4 _WaveOffset;
			
			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 bumpuv0 : TEXCOORD0;
				float2 bumpuv1 : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float2 bumpuv2 : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				
				// scroll bump waves
				float4 temp;
				float4 wpos = mul (unity_ObjectToWorld, v.vertex);
				temp.xyzw = wpos.xzxz * _WaveScale4 + _WaveOffset;
				o.bumpuv0 = temp.xy;
				o.bumpuv1 = temp.wz;
				o.bumpuv2 = v.uv;
				// object space view direction (will normalize per pixel)
				o.viewDir.xzy = WorldSpaceViewDir(v.vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			sampler2D _ReflectiveColor;
			float4 _HorizonColor;
			sampler2D _BumpMap;
			sampler2D _ReflTex;
			float4 _MainColor;
			half4 frag( v2f i ) : SV_Target
			{
				i.viewDir = normalize(i.viewDir);
				
				// combine two scrolling bumpmaps into one
				half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv0 )).rgb;
				half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv1 )).rgb;
				half3 bump = (bump1 + bump2) * 0.5;
				
				//Simulated water refraction interference
				i.bumpuv2.x += (bump1.r+bump2.r)*_WaveScale4.x*0.2;
				i.bumpuv2.y += (bump1.r+bump2.r)*_WaveScale4.x*0.2;
				
				// fresnel factor
				half fresnelFac = dot( i.viewDir, bump );
				
				// final color is between refracted and reflected based on fresnel
				half4 color;
				half4 water = tex2D(_ReflTex,i.bumpuv2)*tex2D( _ReflectiveColor, float2(fresnelFac,fresnelFac) );
				color.rgb = lerp( water.rgb, _HorizonColor.rgb, water.a )+_MainColor.xyz*+_MainColor.w;
				color.a = _HorizonColor.a;
				UNITY_APPLY_FOG(i.fogCoord, color);
				return color;
			}
			ENDCG
		}
	}
}
