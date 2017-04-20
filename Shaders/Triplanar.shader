// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// added triplanar support - Nolan Baker

Shader "Terrain/Triplanar" {
	Properties {
		_Scale ("Texture Scales", Vector) = (1,1,1,1)
		_Depth ("Depth", Range(0.0, 1.0)) = 0.2

		// set by terrain engine
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _Normal3 ("Normal 3 (A)", 2D) = "bump" {}
		[HideInInspector] _Normal2 ("Normal 2 (B)", 2D) = "bump" {}
		[HideInInspector] _Normal1 ("Normal 1 (G)", 2D) = "bump" {}
		[HideInInspector] _Normal0 ("Normal 0 (R)", 2D) = "bump" {}
		[HideInInspector] [Gamma] _Metallic0 ("Metallic 0", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic1 ("Metallic 1", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic2 ("Metallic 2", Range(0.0, 1.0)) = 0.0	
		[HideInInspector] [Gamma] _Metallic3 ("Metallic 3", Range(0.0, 1.0)) = 0.0
		[HideInInspector] _Smoothness0 ("Smoothness 0", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness1 ("Smoothness 1", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness2 ("Smoothness 2", Range(0.0, 1.0)) = 1.0	
		[HideInInspector] _Smoothness3 ("Smoothness 3", Range(0.0, 1.0)) = 1.0

		// used in fallback on old cards & base map
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {
		Tags {
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma multi_compile_fog
		#pragma target 3.0
		#pragma exclude_renderers gles psp2
		#include "UnityPBSLighting.cginc"

		half _Metallic0, _Metallic1, _Metallic2, _Metallic3;
		half _Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3;
		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
		sampler2D _Normal0, _Normal1, _Normal2, _Normal3;
		float4 _Scale;
		float _Depth;

		struct Input {
		    float2 tc_Control : TEXCOORD0;
		    float3 worldPos : TEXCOORD1;
			float3 worldNormal : TEXCOORD2;
			UNITY_FOG_COORDS(5)
			INTERNAL_DATA
		};

		void vert(inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
		    o.tc_Control = TRANSFORM_TEX(v.texcoord, _Control);
		    float4 pos = UnityObjectToClipPos(v.vertex);
		    UNITY_TRANSFER_FOG(o, pos);

		    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			o.worldNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, _Smoothness2, _Smoothness3);			
			half4 splat_control = tex2D(_Control, IN.tc_Control);
		    half weight = dot(splat_control, half4(1,1,1,1));

		    splat_control /= (weight + 1e-3f);
		    o.Normal = float3(0,0,1);
			float3 n = WorldNormalVector(IN, o.Normal);
			float3 projNormal = saturate(pow(n * 1.4, 4));

		// CALCULATE X
			float2 uv = IN.worldPos.zy;
			float2 uv0 = uv * _Scale.x;
			float2 uv1 = uv * _Scale.y;
			float2 uv2 = uv * _Scale.z;
			float2 uv3 = uv * _Scale.w;
			
			float4 splat0 = tex2D(_Splat0, uv0);
			float4 splat1 = tex2D(_Splat1, uv1);
			float4 splat2 = tex2D(_Splat2, uv2);
			float4 splat3 = tex2D(_Splat3, uv3);

		    float ma = max(max(splat0.a * splat_control.r, splat1.a * splat_control.g), max(splat2.a * splat_control.b, splat3.a * splat_control.a)) - _Depth;
		    float b0 = max(splat0.a * splat_control.r - ma, 0.0005);
		    float b1 = max(splat1.a * splat_control.g - ma, 0.0005);
		    float b2 = max(splat2.a * splat_control.b - ma, 0.0005);
		    float b3 = max(splat3.a * splat_control.a - ma, 0.0005);

		    fixed3 albedoX = (splat0.rgb * b0 + splat1.rgb * b1 + splat2.rgb * b2 + splat3.rgb * b3) / (b0 + b1 + b2 + b3);
			float3 normalX = UnpackNormal((tex2D (_Normal0, uv0) * b0 + tex2D (_Normal1, uv1) * b1 + tex2D (_Normal2, uv2) * b2 + tex2D (_Normal3, uv3) * b3) / (b0 + b1 + b2 + b3)) * abs(n.x);

		// CALCULATE Y
			uv = IN.worldPos.xz;
			uv0 = uv0 = uv * _Scale.x;
			uv1 = uv1 = uv * _Scale.y;
			uv2 = uv2 = uv * _Scale.z;
			uv3 = uv3 = uv * _Scale.w;
			
			splat0 = tex2D(_Splat0, uv0);
			splat1 = tex2D(_Splat1, uv1);
			splat2 = tex2D(_Splat2, uv2);
			splat3 = tex2D(_Splat3, uv3);
			
			ma = max(max(splat0.a * splat_control.r, splat1.a * splat_control.g), max(splat2.a * splat_control.b, splat3.a * splat_control.a)) - _Depth;
		    b0 = max(splat0.a * splat_control.r - ma, 0.0005);
		    b1 = max(splat1.a * splat_control.g - ma, 0.0005);
		    b2 = max(splat2.a * splat_control.b - ma, 0.0005);
		    b3 = max(splat3.a * splat_control.a - ma, 0.0005);

		    fixed3 albedoY = (splat0.rgb * b0 + splat1.rgb * b1 + splat2.rgb * b2 + splat3.rgb * b3) / (b0 + b1 + b2 + b3);
			float3 normalY = UnpackNormal((tex2D (_Normal0, uv0) * b0 + tex2D (_Normal1, uv1) * b1 + tex2D (_Normal2, uv2) * b2 + tex2D (_Normal3, uv3) * b3) / (b0 + b1 + b2 + b3)) * abs(n.y);

		// CALCULATE Z
			uv = IN.worldPos.xy;
			uv.x *= -1;
			uv0 = uv0 = uv * _Scale.x;
			uv1 = uv1 = uv * _Scale.y;
			uv2 = uv2 = uv * _Scale.z;
			uv3 = uv3 = uv * _Scale.w;
			
			splat0 = tex2D(_Splat0, uv0);
			splat1 = tex2D(_Splat1, uv1);
			splat2 = tex2D(_Splat2, uv2);
			splat3 = tex2D(_Splat3, uv3);

			ma = max(max(splat0.a * splat_control.r, splat1.a * splat_control.g), max(splat2.a * splat_control.b, splat3.a * splat_control.a)) - _Depth;
		    b0 = max(splat0.a * splat_control.r - ma, 0.0005);
		    b1 = max(splat1.a * splat_control.g - ma, 0.0005);
		    b2 = max(splat2.a * splat_control.b - ma, 0.0005);
		    b3 = max(splat3.a * splat_control.a - ma, 0.0005);

		    fixed3 albedoZ = (splat0.rgb * b0 + splat1.rgb * b1 + splat2.rgb * b2 + splat3.rgb * b3) / (b0 + b1 + b2 + b3);
			float3 normalZ = UnpackNormal((tex2D (_Normal0, uv0) * b0 + tex2D (_Normal1, uv1) * b1 + tex2D (_Normal2, uv2) * b2 + tex2D (_Normal3, uv3) * b3) / (b0 + b1 + b2 + b3)) * abs(n.y);
			normalZ.x *= -1;

		// COMBINE
		    o.Albedo = albedoZ;
			o.Albedo = lerp(o.Albedo, albedoY, projNormal.y);
			o.Albedo = lerp(o.Albedo, albedoX, projNormal.x);

		    float3 normal = normalZ;
			normal = lerp(normal, normalY, projNormal.y);
			normal = lerp(normal, normalX, projNormal.x);
			o.Normal = normalize(normal);

			o.Alpha = weight;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, _Metallic2, _Metallic3));
		}

		ENDCG
	}

	Fallback "Nature/Terrain/Diffuse"	
}
