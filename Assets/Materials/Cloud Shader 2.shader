Shader "Custom/Displacement/Cloud Shader 2"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Amplitude("Amplitude", Vector) = (1,1,1,1)
		_Frequency("Frequency", Vector) = (1,1,1,1)
		_Speed("Speed", Vector) = (1,1,1,1)
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "true" }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct input {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord  : TEXCOORD0;
			};
			float4 _Color;
			float4 _Amplitude;
			float4 _Frequency;
			float4 _Speed;
			v2f vert(input v) {
				v2f o;
				const float PI = 3.14159265359;
				float3 w = mul(unity_ObjectToWorld, v.vertex).xyz;
				v.vertex.zx += _Amplitude.x * float2(cos(_Frequency.x*PI*w.x + _Time.y*_Speed.x),sin(_Frequency.x*PI*w.z + _Time.y*_Speed.x))
					+ _Amplitude.y * float2(sin(_Frequency.y*PI*w.x + _Time.y*_Speed.y), cos(_Frequency.y*PI*w.z + _Time.y*_Speed.y))
					+ _Amplitude.z * float2(cos(_Frequency.z*PI*w.x + _Time.y*_Speed.z), sin(_Frequency.z*PI*w.z + _Time.y*_Speed.z))
					+ _Amplitude.w * float2(sin(_Frequency.w*PI*w.x + _Time.y*_Speed.w), cos(_Frequency.w*PI*w.z + _Time.y*_Speed.w))
					;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f o) : SV_Target
			{
				half4 col = tex2D(_MainTex, o.texcoord) * _Color;
				return col;
			}
		ENDCG
		}
	}
}