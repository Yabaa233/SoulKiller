Shader "MS_Shader/LIvingFlows"
{
	Properties
	{
		_AlphaClipThreshold("Alpha Clip Threshold", float) = 0.5
		[Header(Albedo)]
		_Albedo("Albedo", 2D) = "white" {}
		_ColorTint("Color Tint", Color) = (1,1,1,1)
		[Space(20)][Header(Emission)]
		_Emission("Emission", 2D) = "white" {}
		_BrightColor("Bright Color", Color) = (1,0,0,1)
		_DarkColor("Dark Color", Color) = (0,0,0,0)
		_BrightPower("Bright Power", Range( 0 , 10)) = 6
		_BrightMaskMultiply("Bright Mask Multiply", Range( 0 , 10)) = 2
		[Space(20)][Header(Distance)]
		_Distance("Distance", float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		_OffsetPower("Offset Power", float) = 0
	}

	SubShader
	{
		LOD 0
		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE
		
		#pragma target 2.0

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		CBUFFER_START(UnityPerMaterial)
			real4 _ColorTint;
			real4 _Albedo_ST;
			real4 _BrightColor;
			real4 _DarkColor;
			real4 _Emission_ST;
			real _Distance;
			real _DistancePower;
			real _OffsetPower;
			real _BrightMaskMultiply;
			real _BrightPower;
			real _AlphaClipThreshold;
			CBUFFER_END
			real4 _Affector;
			sampler2D _Albedo;
			sampler2D _Emission;

			real Remap(real x, real t1, real t2, real s1, real s2) 
			{
                return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            }
		
			struct VertexInput
			{
				float4 vertex : POSITION;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_texcoord3 : TEXCOORD1;
				float4 ase_texcoord4 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
						
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				
				//得到粒子和物体之间的距离均一化mask
				real4 particleCenter = (real4(v.ase_texcoord.z , v.ase_texcoord.w , v.ase_texcoord1.x , 0.0));
				real distanceMask = ( 1.0 - distance( particleCenter , _Affector ) );
				real clampResult = clamp( (0.0 + (( distanceMask + ( _Distance - 1.0 ) ) - 0.0) * (1.0 - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				real resultMask = pow( clampResult , _DistancePower );
				//得到粒子和物体之间的方向向量
				real4 centerVector = normalize( ( particleCenter - _Affector ) );
				real4 vertexOffsetg =  resultMask * centerVector * _OffsetPower * real4( 1,1,1,0 ) ;
				
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord1;
				o.ase_color = v.ase_color;
				
				//施加顶点偏移
				real3 vertexValue = vertexOffsetg.xyz;
				v.vertex.xyz += vertexValue;
				
				real3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				real4 positionCS = TransformWorldToHClip( positionWS );
				o.clipPos = positionCS;
				
				return o;
			}
			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				real2 uv_Albedo = IN.ase_texcoord3.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				real4 albedoColor = tex2D( _Albedo, uv_Albedo );
				real4 baseColor =  _ColorTint * albedoColor ;
				
				//得到粒子和物体之间的距离均一化mask
				real4 particleCenter = real4(IN.ase_texcoord3.z , IN.ase_texcoord3.w , IN.ase_texcoord4.x , 0.0);
				real distanceMask = ( 1.0 - distance( particleCenter , _Affector ) );
				real clampResult =  saturate(0.0 + (( distanceMask + ( _Distance - 1.0 ) ) - 0.0)
					* (1.0 - 0.0) / (_Distance - 0.0));
				real resultMask = pow( clampResult , _DistancePower );

				//根据距离mask进行混合高亮和暗部
				real lerpAmount = saturate( resultMask * _BrightMaskMultiply );
				real4 lerpColor = lerp( _DarkColor , _BrightColor , lerpAmount);
				
				real2 uv_Emission = IN.ase_texcoord3.xy * _Emission_ST.xy + _Emission_ST.zw;
				real4 emissionColor = lerpColor * IN.ase_color * _BrightPower * IN.ase_color.a
					* tex2D( _Emission, uv_Emission ).r;
			
				real3 Color = ( baseColor + emissionColor ).rgb;
				real Alpha = albedoColor.a;
				clip( Alpha - _AlphaClipThreshold );

				return half4( Color, Alpha );
			}
		
		ENDHLSL
		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma vertex vert
			#pragma fragment frag

			ENDHLSL
		}
	}
	Fallback "Hidden/InternalErrorShader"
	
}
