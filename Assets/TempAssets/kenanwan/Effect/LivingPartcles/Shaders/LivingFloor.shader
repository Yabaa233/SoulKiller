Shader "MS_Shader/LivingFloor"
{
	Properties
	{
		[Header(Base)]
		[Space(10)]
		_MainTex("Main Tex", 2D) = "white" {}
		
		_Distance("Distance" , float) = 1.0
		_DistancePower("Distance Power" , float) = 1.0
		
		_BaseOffset("Base Offset" , Range(0,1)) = 0
		_FinalMaskMultiply("Final Mask Multiply" , Range(0.0,10.0)) = 1.0
		_OffsetScale("Offset Scale" , float) = 1.0

		[Header(Color)]
		[Space(10)]
		_ColorNear("Near Color" , Color) = (1.0,1.0,1.0,1.0)
		_ColorFar("Far Color" , Color) = (0,0,0,0)
		_FinalColorExp("FinalColor Exp" , Range(0.2,4)) = 1.0

		[Toggle(_USERAMPTEX_ON)] _UseRampTex("Use Ramp Tex", Float) = 0
		_RampTex("Ramp Tex", 2D) = "white" {}

		[Header(Noise)]
		[Space(10)]
		 _NoiseTex("Noise Tex", 2D) = "white" {}
		 _NoiseScale("Noise Scale" , float) = 1.0

		 _NoiseTiling("Noise Tiling" , float) = 1.0

		[Header(Setting)]
		[Space(10)]
		_YAxisLerp("Y Axis Lerp" , Range(0.0,1.0)) = 0.0

		_Affector("Affector" , vector) = (0,0,0,0)
		
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

		Pass
		{

			HLSLPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#pragma target 3.0
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma shader_feature _USERAMPTEX_ON


			struct appdata
			{
				float4 posOS : POSITION;
				float4 texcoord0 : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
					float2 uv : TEXCOORD0;
					float4 posCS : SV_POSITION;
					float vertexOffset : TEXCOORD1;
			};

			CBUFFER_START(UnityPerMaterial)

			real4 _ColorNear;
			real4 _ColorFar;
			real _Distance;
			real _DistancePower;
			real _OffsetScale;
			real _BaseOffset;

			real _FinalColorExp;

			real _FinalMaskMultiply;

			real _YAxisLerp;

			real _NoiseScale;
			real _NoiseTiling;

			real4 _NoiseTex_ST;

			real4 _Affector;
			CBUFFER_END

			TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

			TEXTURE2D(_RampTex); SAMPLER(sampler_RampTex);

			TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

			//----------------------------------------------方法-----------------------------------------//

            real Remap(real x, real t1, real t2, real s1, real s2) 
			{
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }

			//---------------------------------------------着色器------------------------------------------//

			v2f vert(appdata i)
			{
				float3 posWorld = float3(i.texcoord0.zw, i.texcoord1.x);
				_Affector.y *= _YAxisLerp;
				posWorld.y *= _YAxisLerp;

                float2 uvWorld = posWorld.xz * _NoiseTiling + frac(_Time.x );//posWorld.xz;
                float noise = _NoiseTex.SampleLevel(sampler_NoiseTex,uvWorld,0 ).r;
				noise = (noise - 0.5) * 2;

				float distanceMask = 1.0 - distance(posWorld , _Affector.xyz);
				
				distanceMask +=  _Distance - 1.0;
				distanceMask = Remap(distanceMask , 0.0, _Distance, 0.0, 1.0 ) ;
				distanceMask = saturate(distanceMask);
				distanceMask = pow(distanceMask , _DistancePower);
				distanceMask = distanceMask * _FinalMaskMultiply + _NoiseScale * noise + _BaseOffset;
				distanceMask = saturate(distanceMask);

				i.posOS.y -= _OffsetScale * (1 - distanceMask);

				v2f o;
				o.vertexOffset = distanceMask;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(i.posOS.xyz);
				o.posCS = vertexInput.positionCS;
				o.uv = i.texcoord0.xy;
				//float3 worldPos = vertexInput.positionWS;

				return o;
			}

			real4 frag(v2f i) : SV_Target
			{
				#ifdef _USERAMPTEX_ON
				real4 finalColor = SAMPLE_TEXTURE2D(_RampTex, sampler_RampTex, half2(i.vertexOffset, 0));   

				#else
				real finalLerp = pow(i.vertexOffset , _FinalColorExp);
				real4 finalColor = lerp(_ColorNear,_ColorFar, finalLerp);
				#endif
				
				real4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);   
				finalColor *= baseColor;

				//finalColor *= i.color

				return finalColor;
			}
			ENDHLSL
		}
	}
}