Shader "MS_Shader/SkyboxColors"
{
	Properties {
		_TopColor ("Top Color", Color) = (1, 0.3, 0.3, 0)
		_MiddleColor ("MiddleColor", Color) = (1.0, 1.0, 0.8)
		_BottomColor ("Bottom Up Color", Color) = (0.3, 0.3, 1, 0)
		_BottomDownColor ("Bottom Down Color", Color) = (0.3, 0.3, 1, 0)
		_Up ("Up", Vector) = (0, 1, 0)
		_InterValUp("InterValUp",range(0,1)) = 0.6
		_InterValDown("InterValDown",range(0,1)) = 0.2
	}
	SubShader {
		Tags {
			"RenderType" = "Background"
			"Queue" = "Background"
			"PreviewType" = "Skybox"
		}
		Pass {
			ZWrite Off
			Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed3 _TopColor, _BottomColor, _MiddleColor,_BottomDownColor;
			float3 _Up;
			float _Exp,_InterValUp,_InterValDown;

			struct appdata {
				float4 vertex : POSITION;
				float3 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float3 texcoord : TEXCOORD0;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			//t1,t2为旧的区间，s1,s2为新的区间，x为当前位置
			float Remap(float x, float t1, float t2, float s1, float s2) 
			{
	            return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
	        }
			
			fixed4 frag (v2f i) : SV_TARGET {
				float3 texcoord = normalize(i.texcoord);
				float3 up = normalize(_Up);
				float d = dot(texcoord, up) * 0.5 + 0.5;
				half3 col;
				float intervalDown = Remap(d,0,_InterValDown,0,1);
				float intervalMid = Remap(d,_InterValDown,_InterValUp,0,1);
				float intervalUp = Remap(d,_InterValUp,1,0,1);
				if (step(d,_InterValDown))
				{
					col = lerp(_BottomDownColor,_BottomColor,intervalDown);
				}else if (step(d,_InterValUp))
				{
					col = lerp(_BottomColor,_MiddleColor,intervalMid);
				}else
				{
					col = lerp(_MiddleColor,_TopColor,intervalUp);
				}
				// half3 col = lerp(_MiddleColor,
				// 	s < 0.0 ? _BottomColor : _TopColor,
				// 	pow(abs(d),_Exp)
				// );
				return half4(col,1.0);
			}

			ENDCG
		}
	}
}