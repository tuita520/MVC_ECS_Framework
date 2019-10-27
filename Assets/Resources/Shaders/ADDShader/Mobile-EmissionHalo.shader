// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "D-Z/EmssionHalo" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_LightHalo("Light Color",Color) = (1,1,1,1)  
		_EmissionMask("Emission Mask",2D) = "white"{}
		_Emission("Emission Strength",Range(0,1)) = 0
		_Reflecttion("Ref Strength",Range(0,1)) = 0
		//_RimPower ("Rim Power", Range(0.6,9.0)) = 1.0  
		//_Length("Normal Size",float) = 0
	}
	
	SubShader 
	{
	
		Tags 
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}
		
		LOD 150
	
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd
	   
		sampler2D _MainTex;
		sampler2D _EmissionMask;
		float _Emission;
		float _Reflecttion;
	
		struct Input 
		{
			float2 uv_MainTex;
		};
	
		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			float4 Em = tex2D(_EmissionMask,IN.uv_MainTex);
			o.Albedo = c.rgb * _Reflecttion;
			o.Emission = c.rgb * (_Emission * Em.r)+c.rgb * (_Emission * Em.b *0.5);
			o.Alpha = 1;
		}
		ENDCG
	
		//Blend SrcAlpha One
		//Lighting Off 
		//ZWrite Off 
		//Fog { Color (1,1,1,1) }
		//Pass 
		//	{
		//		CGPROGRAM
		//		#pragma vertex vert
		//		#pragma fragment frag
		//		#include "UnityCG.cginc"
		//	
		//		struct appdata_t 
		//		{
		//			float4 vertex : POSITION;
		//			float4 normal :NORMAL;
		//			fixed2 uv:TEXCOORD0;
		//			
		//		};
		//
		//		struct v2f 
		//		{
		//			float4 vertex : POSITION;
		//			fixed2 uv:TEXCOORD0;
		//			float3 normal:TEXCOORD1;
		//			float3 ViewDir:TEXCOORD2;
		//
		//		};
		//		
		//		sampler2D _EmissionMask;
		//		float _Length;
		//		float _RimPower;
		//		float4 _LightHalo;
		//
		//
		//		v2f vert (appdata_t v)
		//		{
		//			v2f o;
		//
		//			float3 worldNorm = normalize(_World2Object[0].xyz * v.normal.x + _World2Object[1].xyz * v.normal.y + _World2Object[2].xyz * v.normal.z);
		//			float4 worldVex = mul(_Object2World,v.vertex);
		//			worldVex.xyz += worldNorm * (_Length);
		//			v.vertex = 	mul(_World2Object,worldVex);
		//			
		//			
		//			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		//			o.uv = v.uv;
		//			o.ViewDir = ObjSpaceViewDir(v.vertex);
		//			o.normal = v.normal;
		//			
		//			return o;
		//		}
		//
		//		fixed4 frag( v2f i ) : COLOR
		//		{
		//
		//			i.vertex.w = 10;
		//			half rim = saturate(dot (normalize(i.ViewDir), i.normal)); 
		//		    float4 C = tex2D(_EmissionMask,i.uv);
		//			_LightHalo.a =  (pow (rim, _RimPower) * C.r);  
		//			return  _LightHalo;
		//		}
		//		ENDCG
		//
		//}	
	}
//Fallback "Mobile/VertexLit"
}
