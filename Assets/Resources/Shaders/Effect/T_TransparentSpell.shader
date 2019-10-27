// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "T_shaders/Effect/T_Transparent" 
{
	Properties 
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_Color ("MainColor", Color) = (1.0,1.0,1.0,1.0)
	}
	
	SubShader 
	{
		tags
		{
			"Queue" = "Transparent" 
			"RenderType" = "Overlay" 	
		}
		ZWrite Off  
        ZTest On
		Pass
		{ 
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#pragma vertex vs
			#pragma fragment ps 
			
			sampler2D _MainTex;
			float4 _Color;
			struct v2f
			{
				fixed4 pos : POSITION;
				fixed4 uv : TEXCOORD0;
			};
						
			v2f vs(v2f input)
			{
				v2f output;
				output.pos = UnityObjectToClipPos(input.pos);
				output.uv = input.uv;
				return output;
			} 
			
			fixed4 ps(v2f input):COLOR
			{				
				fixed4 Tex1 = tex2D(_MainTex, input.uv)*_Color;
				return Tex1;
			} 
			ENDCG
		}
	} 
}
