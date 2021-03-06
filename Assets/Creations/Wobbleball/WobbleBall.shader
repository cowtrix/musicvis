﻿// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/WobbleBall" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		[Bump] _Bump ("Normal Map", 2D) = "bump" {}
		_OffsetMinMax("Offset Min/Max", Vector) = (1, 4, 0, 0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		//Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
		}; 

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _Wavelength;
		sampler2D _Bump;
		sampler2D _Delta;
		fixed4 _OffsetMinMax;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float4 tex = tex2Dlod (_Delta, float4(v.texcoord.xy, 0, 0));
			tex *= tex * tex * tex;
			tex /= _OffsetMinMax.y;			
			v.vertex.xyz *= _OffsetMinMax.x + tex.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 w = tex2D (_Wavelength, IN.uv_MainTex);
			o.Albedo = c.rgb * w.rgb;
			o.Smoothness = _Glossiness;
			o.Specular = _Metallic;
			o.Alpha = c.a * IN.color.a;
			o.Emission = w * w * 1.2;
			o.Normal = UnpackNormal(tex2D(_Bump, IN.uv_MainTex));
			//o.Occlusion = 1 - w.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
