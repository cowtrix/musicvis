// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Legacy Shaders/Transparent/Diffuse" {
Properties {
	_Color1 ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_ColorBoost ("Color Boost", FLoat) = 1

	_Distortion1Strength ("Distortion 1 Strength", Float) = 0
	_Distortion1Freq ("Distortion 1 Freq", Float) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	Cull Off

CGPROGRAM
#pragma surface surf StandardSpecular alpha:fade vertex:vert

sampler2D _MainTex;
fixed4 _Color1;
float _ColorBoost;
float _Distortion1Strength;
float _Distortion1Freq;

struct Input {
	float2 uv_MainTex;
};

float rand(float2 co){
    return frac(sin(dot(co.xy , float2(12.9898,78.233))) * 43758.5453);
}

void vert (inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input,o);
	v.vertex.xyz += float3(sin(v.vertex.x * _Distortion1Freq), cos(v.vertex.y * _Distortion1Freq), tan(v.vertex.z * _Distortion1Freq)) * _Distortion1Strength;
}

void surf (Input IN, inout SurfaceOutputStandardSpecular o) 
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color1;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * _ColorBoost;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Legacy Shaders/Transparent/VertexLit"
}
