Shader "MS_Shader/MSShader_Blend"
{
    Properties
	{
		[HDR]_TintColor("Color", Color) = (1,1,1,1)
		_TintColorIntensity("TintColorIntensity",float) = 1
		_MaskTex ("MaskTex", 2D) = "white" {}
		_MaskTill("MaskTill",vector) = (1,1,0,0)
		_MainTex ("Texture", 2D) = "white" {}
		_MainTexSpeed("MainTexSpeed",vector) = (0,0,0,0)
		_DistortionAmount("DistortionAmount",range(0,1)) = 0
		_DistortionTex ("DistortionTex", 2D) = "white" {}
		_DistortionSpeed("DistortionSpeed",vector) = (0,0,1,1)
		_DissolveAmount("DissolveAmount",range(0,10)) = 0.5
		_DissolveTex ("DissolveTex", 2D) = "white" {}
		_DissolveSpeed("DissolveSpeed",vector) = (0,0,1,1)
		_AlphaPow("Alpha Pow", Float) = 1
		_AlphaMul("Alpha Mul", Float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
			};

			sampler2D _MainTex,_DistortionTex,_DissolveTex,_MaskTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float4 _MainTexSpeed,_DistortionSpeed,_DissolveSpeed,_MaskTill;
			float _AlphaPow;
			float _AlphaMul;
			float _TintColorIntensity;
			float _DistortionAmount,_DissolveAmount;
			
			v2f vert (appdata vertexInput)
			{
				v2f pixelInput;
				pixelInput.vertex = UnityObjectToClipPos(vertexInput.vertex);
				pixelInput.uv = TRANSFORM_TEX(vertexInput.uv, _MainTex);
				pixelInput.color = vertexInput.color;
				return pixelInput;
			}
			
			half4 frag (v2f pixelInput) : SV_Target
			{
				// sample the texture
				half4 distortionCol = tex2D(_DistortionTex, pixelInput.uv * _DistortionSpeed.zw + _Time.y * _DistortionSpeed.xy);
				half2 uv = lerp(pixelInput.uv,pixelInput.uv + distortionCol.xy,_DistortionAmount);

				half4 dissolveCol = tex2D(_DissolveTex, pixelInput.uv * _DissolveSpeed.zw + _Time.y * _DissolveSpeed.xy);
				dissolveCol = pow(dissolveCol,_DissolveAmount);

				half4 mask = tex2D(_MaskTex,  pixelInput.uv * _MaskTill.xy + _MaskTill.zw);
				
				half4 mainCol = tex2D(_MainTex, uv + _Time.y * _MainTexSpeed.xy);
				
				half4 col = mainCol * _TintColor * pixelInput.color * _TintColorIntensity * dissolveCol * mask;
				col.a = saturate(pow(col.a, _AlphaPow) * _AlphaMul);
				return col;
			}
			ENDCG
		}
	}
}
