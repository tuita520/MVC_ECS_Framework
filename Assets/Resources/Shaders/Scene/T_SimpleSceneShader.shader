Shader "T_shaders/Scene/T_SimpleShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_brightness ("亮度Brightness", Range(-1,1)) = 0.06
		_contrast ("对比度Contrast", Range(-1,1)) = 0.6
		_Emission("Ession Strength",Range(0,1)) = 0.3
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _contrast;
		fixed _brightness;
		float _Emission;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			c.rgb = (c.rgb - 0.5)*tan(0.55439865+0.54207868*_contrast) + 0.5;
  			c.rgb += _brightness;
			o.Albedo = c.rgb * (1 - 0.3);
			o.Emission = c.rgb * 0.3;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
