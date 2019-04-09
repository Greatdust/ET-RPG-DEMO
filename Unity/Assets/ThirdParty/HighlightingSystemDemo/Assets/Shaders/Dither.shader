Shader "Custom/Dither"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows addshadow vertex:vert
		#pragma multi_compile _ LOD_FADE_CROSSFADE
		
		// Use shader model 3.0 target, to get nicer looking lighting
		// Commented out since that's causing artifacts in Forward and Legacy Deferred (Light Prepass) rendering paths (only in builds)
		//#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;

			#if LOD_FADE_CROSSFADE
			float4 screenPos;
			#endif
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
		}
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			#if LOD_FADE_CROSSFADE
			float2 coord = (IN.screenPos.xy / IN.screenPos.w) * _ScreenParams.xy;
			UNITY_APPLY_DITHER_CROSSFADE(coord);
			#endif
			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}