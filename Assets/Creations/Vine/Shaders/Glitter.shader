// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Glitter" {
	Properties {
		_Color1 ("Color", Color) = (1,1,1,1)
		_Color2 ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "black" {}
		_SpecDirection("Spec Direction", 2D) = "bump" {}
		_Specular("Specular", Range(0,1)) = 0.0
		_Roughness("Roughness", Range(0,1)) = 0.0
		_EmissionPower("EmissionPower", Float) = 2
		_EmissionStrength("EmissionStrength", Float) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf StandardSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _SpecDirection;

		struct Input {
			float2 uv_MainTex;
			float2 uv_SpecDirection;
			float3 viewDir;
			float3 worldNormal;
			float4 color;
		};

		float rand(float2 co) {
			return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
		}

		fixed4 _Color1;
		fixed4 _Color2;

		fixed _Specular;
		fixed _Roughness;
		fixed _EmissionPower;
		fixed _EmissionStrength;

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
		{
			float n = UnpackNormal(tex2D(_SpecDirection, IN.uv_SpecDirection));
			float dotP = dot(IN.viewDir, normalize(IN.worldNormal + n));
			float v = saturate(rand(IN.uv_MainTex * (1 + dotP)));

			fixed4 c = lerp(_Color1, _Color2, IN.uv_MainTex.y);
			o.Emission = _EmissionStrength * pow(v, _EmissionPower);
			o.Albedo = c.rgb * IN.color.r;
			o.Smoothness = _Roughness;
			o.Specular = _Specular;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
