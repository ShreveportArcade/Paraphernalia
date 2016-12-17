Shader "Flow" {
	Properties {
		_MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Alpha ("Alpha", Range(0, 1)) = 0.75
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
		Blend One OneMinusSrcAlpha

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
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				float4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			sampler2D _MainTex;
	        float4 _MainTex_ST;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);;
				OUT.color = IN.color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _FlowMap;
			sampler2D _ScreenTex;
			float4 _Flow;
			float _XOffset;
			float _YOffset;
			float _Alpha;

			float4 frag(v2f IN) : SV_Target {
				float2 uv = IN.texcoord;
				float2 flowmap = -2 * tex2D(_FlowMap, uv) + 1;
				flowmap.x *= _Flow.x;
				flowmap.y *= _Flow.y;

				float halfCycle = _Flow.z * 0.5f; 
				float phase0 = fmod(_Time.x * _Flow.w + halfCycle, _Flow.z);
				float phase1 = fmod(_Time.x * _Flow.w, _Flow.z);     
				float opacity = abs(phase0 - halfCycle) / halfCycle;

				float4 c1 = tex2D(_MainTex, uv + flowmap * phase0);
				float4 c2 = tex2D(_MainTex, uv + float2(_XOffset, 0) + flowmap * phase1);
				float4 c = lerp(c1, c2, opacity);

				c *= IN.color;
				c.rgb *= c.a;
				c.a *= _Alpha;
				return c;
			}
			ENDCG
		}
	}
}
