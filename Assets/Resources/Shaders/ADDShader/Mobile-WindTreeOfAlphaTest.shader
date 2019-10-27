// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "D-Z/Mobile-WindTreeOfAlphaTest"
{
	Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		_SecondaryFactor ("Factor for up and fown bending", float) = 2.5
		_WaveRatio(" W R", Range(0,1)) = 0.1
		_windParams("windParams",vector) = (0,0,0,0)
	}

	SubShader 
	{
		LOD 100
		
		AlphaTest Greater [_Cutoff]
		
		CGINCLUDE
		#pragma multi_compile_fog
		#include "UnityCG.cginc"
		#include "TerrainEngine.cginc"
		
		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed _Cutoff;
		float _WaveRatio;

		float _SecondaryFactor;
		
		float4 _windParams;
		
		struct v2f 
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			#ifndef LIGHTMAP_OFF
			float2 lmap : TEXCOORD1;
			#endif
			UNITY_FOG_COORDS(2)
			fixed4 color : COLOR;
		};

		inline float4 AnimateVertex2(float4 pos, float3 normal, float4 animParams, float SecondaryFactor)
		{	

			float4 wPos = mul(unity_ObjectToWorld,pos);
			float3 wNomal = normalize(mul((float3x3)unity_ObjectToWorld,normal));
			
			float fDetailAmp = 0.1f;
			float fBranchAmp = 0.3f;
			

			float fObjPhase = dot(unity_ObjectToWorld[3].xyz, 1);
			
			float fBranchPhase = fObjPhase;
			float fVtxPhase = dot(wPos.xyz, animParams.y + fBranchPhase);

			float2 vWavesIn = _Time.yy + wPos.xz *.3 + float2(fVtxPhase, fBranchPhase );
			

			float4 vWaves = (frac( vWavesIn.xxyy * float4(1.975, 0.793, 0.375, 0.193) ) * 2.0 - 1.0);
			vWaves = SmoothTriangleWave( vWaves );
			float2 vWavesSum = vWaves.xz + vWaves.yw;


			float3 bend = animParams.y * fDetailAmp * wNomal.xyz * sign(wNomal.xyz);
			
			bend.y = animParams.z * fBranchAmp * SecondaryFactor; 
			wPos.xyz += ((vWavesSum.xyx * bend) + (_Wind.xyz * vWavesSum.y * animParams.w)) * _Wind.w * _WaveRatio; 


			wPos.xyz += animParams.w * _Wind.xyz * _Wind.w * (wPos.y ); 
			wPos = mul(unity_WorldToObject,wPos);
			return wPos;
		}


		
		v2f vert (appdata_full v)
		{
			v2f o;
			float4	windParams	= _windParams;		

			float4 mdlPos = AnimateVertex2(v.vertex, v.normal, windParams, _SecondaryFactor);
			o.pos = UnityObjectToClipPos(mdlPos);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			#ifndef LIGHTMAP_OFF
			o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
			#endif
			
			o.color.rgba = v.color.rgba;
			UNITY_TRANSFER_FOG(o,o.pos);
			return o;
		}
		
		fixed4 frag (v2f i) : COLOR
	    {
				fixed4 tex = tex2D (_MainTex, i.uv);
				clip(tex.a - _Cutoff);
				fixed4 c;
				c.rgb = tex.rgb * i.color.a;
				c.a = tex.a;
				
				#ifndef LIGHTMAP_OFF
				//fixed3 lm = DecodeLightmap (UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap));
				//c.rgb *= lm;
				#endif
				UNITY_APPLY_FOG(i.fogCoord, c);
				return c;
	   }
	   ENDCG

		Pass 
		{
			Tags 
			{"Queue"="AlphaTest"
			 "RenderType"="TransparentCutout"
			  "LightMode"="ForwardBase"
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest		

			ENDCG 
		}
		
		pass
		{
			Tags
			{ 
				"LIGHTMODE"="SHADOWCASTER" 
				"SHADOWSUPPORT"="true" 
				"IGNOREPROJECTOR"="true" 
				"RenderType"="TreeLeaf" 
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			ENDCG 
	 	}
	}
}
