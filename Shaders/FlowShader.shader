// Sprite Flow Shader - (c)2017 Nolan Baker. MIT License

Shader "Paraphernalia/Flow Sprite" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_FlowMap ("Flow Map", 2D) = "white" {}
    	_Flow ("Flow (X, Y, Cycle Time, Cycle Speed)", Vector) = (1, 1, 0.1, 1)
	}

	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 flowUV : TEXCOORD1;
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 flowUV : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FlowMap;
			float4 _FlowMap_ST;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);;
				OUT.flowUV = TRANSFORM_TEX(IN.flowUV, _FlowMap);;
				OUT.color = IN.color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			float4 _Flow;
			float _XOffset;
			float _YOffset;

			float4 frag(v2f IN) : SV_Target {
				float2 flowmap = -2 * tex2D(_FlowMap, IN.flowUV) + 1;
				flowmap.x *= _Flow.x;
				flowmap.y *= _Flow.y;

				float halfCycle = _Flow.z * 0.5f; 
				float phase0 = fmod(_Time.x * _Flow.w + halfCycle, _Flow.z);
				float phase1 = fmod(_Time.x * _Flow.w, _Flow.z);     
				float opacity = abs(phase0 - halfCycle) / halfCycle;

				float2 uv = IN.texcoord;
				float4 c1 = tex2D(_MainTex, uv + flowmap * phase0);
				float4 c2 = tex2D(_MainTex, uv + float2(_XOffset, 0) + flowmap * phase1);
				float4 c = lerp(c1, c2, opacity);

				c *= IN.color;
				return c;
			}
			ENDCG
		}
	}
}
