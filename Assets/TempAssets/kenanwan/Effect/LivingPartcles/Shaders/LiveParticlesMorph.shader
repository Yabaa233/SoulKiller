Shader "MS_Shader/LiveParticlesMorph"
{
	Properties
	{
		[Toggle(_FLIPMORPHMASK_ON)] _FlipMorphMask("Flip Morph Mask", Float) = 0
		[Toggle(_FLIPEMISSIONMASK_ON)] _FlipEmissionMask("Flip Emission Mask", Float) = 0
		[Toggle(_FLIPOFFSETMASK_ON)] _FlipOffsetMask("Flip Offset Mask", Float) = 0
		_MorphMain("Morph Main", 2D) = "white" {}
		_MorphNormal("Morph Normal", 2D) = "white" {}
		[Space(20)][Header(AlbedoColor)]
		_ColorTint("Color Tint", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		[Space(20)][Header(EmissionColor)]
		_Emission("Emission", 2D) = "white" {}
		_FinalColorOne("Final Color One", Color) = (1,0,0,1)
		_FinalColorTwo("Final Color Two", Color) = (0,0,0,0)
		_FinalPower("Final Power", Range( 0 , 20)) = 6
		_FinalMaskMultiply("Final Mask Multiply", Range( 0 , 10)) = 2
		[Space(20)][Header(Distance)]
		_Distance("Distance", Float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		_DistanceRemap("Distance Remap", Range( 1 , 4)) = 1
		[Space(20)][Header(Offset)]
		_OffsetPower("Offset Power", Float) = 0
		_CenterMaskMultiply("Center Mask Multiply", Float) = 4
		_CenterMaskSubtract("Center Mask Subtract", Float) = 0.75
		[Space(20)][Header(Position)]
		_RampColor("RampColor", 2D) = "white" {}
		_PositionRange("PositionRange",vector) = (0,0,0,0)
	}

	SubShader
	{
		LOD 0
		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE
		#pragma target 2.0

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		
		real3 RotateAroundAxis( real3 center, real3 original, real3 u, real angle )
		{
			original -= center;
			real C = cos( angle );
			real S = sin( angle );
			real t = 1 - C;
			real m00 = t * u.x * u.x + C;
			real m01 = t * u.x * u.y - S * u.z;
			real m02 = t * u.x * u.z + S * u.y;
			real m10 = t * u.x * u.y + S * u.z;
			real m11 = t * u.y * u.y + C;
			real m12 = t * u.y * u.z - S * u.x;
			real m20 = t * u.x * u.z - S * u.y;
			real m21 = t * u.y * u.z + S * u.x;
			real m22 = t * u.z * u.z + C;
			real3x3 finalMatrix = real3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}

		//t1,t2为旧的区间，s1,s2为新的区间，x为当前位置
		real Remap(real x, real t1, real t2, real s1, real s2) 
		{
            return (x - t1) / (t2 - t1) * (s2 - s1) + s1;
        }
			
		CBUFFER_START(UnityPerMaterial)
			real4 _ColorTint;
			real4 _Albedo_ST;
			real4 _FinalColorTwo;
			real4 _FinalColorOne;
			real4 _Emission_ST;
			real4 _RampColor_ST;
			real _Distance;
			real _DistanceRemap;
			real _DistancePower;
			real _CenterMaskSubtract;
			real _CenterMaskMultiply;
			real _OffsetPower;
			real _FinalMaskMultiply;
			real _FinalPower;
		CBUFFER_END
			real4 _PositionRange;
			real4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;
			sampler2D _Albedo;
			sampler2D _Ramp;
			sampler2D _Emission;
			sampler2D _RampColor;

		ENDHLSL

		Pass
		{
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
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
			
			#pragma shader_feature _FLIPOFFSETMASK_ON
			#pragma shader_feature _FLIPMORPHMASK_ON
			#pragma shader_feature _FLIPEMISSIONMASK_ON

			struct VertexInput
			{
				real4 vertex : POSITION;
				real3 normal : NORMAL;
				real4 ase_texcoord1 : TEXCOORD1;
				real4 ase_texcoord2 : TEXCOORD2;
				real4 ase_texcoord3 : TEXCOORD3;
				real4 ase_texcoord : TEXCOORD0;
				real4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				real4 clipPos : SV_POSITION;
				real3 worldPos : TEXCOORD0;
				real4 shadowCoord : TEXCOORD1;
				real4 ase_texcoord3 : TEXCOORD3;
				real4 ase_texcoord4 : TEXCOORD4;
				real4 ase_texcoord5 : TEXCOORD5;
				real4 ase_color : COLOR;
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
				real4 particleCenter = (real4(v.ase_texcoord1.w , v.ase_texcoord2.x , v.ase_texcoord2.y , 0.0));
				real distanceMask = ( 1.0 - distance( particleCenter , _Affector ) );
				real clampResult = clamp( 0.0 + (( distanceMask + ( _Distance - 1.0 ) ) - 0.0)
					* (_DistanceRemap - 0.0) / (_Distance - 0.0) , 0.0 , 1.0 );
				real resultMask = pow( clampResult , _DistancePower );

				//反转均一化mask
				#ifdef _FLIPOFFSETMASK_ON
				real flipMask = ( 1.0 - resultMask );
				#else
				real flipMask = resultMask;
				#endif
				
				real flipClampMask = clamp( ( flipMask - _CenterMaskSubtract ) , 0.0 , 1.0 );
				real offsetFromCenter = ( flipMask - ( flipClampMask * _CenterMaskMultiply ) );
				//得到粒子和物体之间的方向向量
				real4 centerVector = normalize( ( particleCenter - _Affector ) );
				real3 rotation3D = -(real3(v.ase_texcoord2.z , v.ase_texcoord2.w , v.ase_texcoord3.x));
				
				#ifdef _FLIPMORPHMASK_ON
				real flipMorphMask = ( 1.0 - resultMask );
				#else
				real flipMorphMask = resultMask;
				#endif
				
				//读取morph图，一个神奇的贴图
				real2 uvMorph = real2(v.ase_texcoord1.x ,v.ase_texcoord1.y + (-0.5 + flipMorphMask));
				real4 colorMorphMain = tex2Dlod( _MorphMain, real4( uvMorph, 0, 0.0) );
				colorMorphMain *= half4(-1,-1,1,1);

				//旋转morph
				real4 rotate =  colorMorphMain * v.ase_texcoord1.z ;
				real3 rotateZ = RotateAroundAxis( real3( 0,0,0 ), rotate.xyz, real3( 0,0,-1 ), rotation3D.z );
				real3 rotateZX = RotateAroundAxis( real3( 0,0,0 ), rotateZ, real3( -1,0,0 ), rotation3D.x );
				real3 rotateZXY = RotateAroundAxis( real3( 0,0,0 ), rotateZX, real3( 0,-1,0 ), rotation3D.y );
				real4 offsetFinal =  offsetFromCenter * centerVector * _OffsetPower * real4( 1,0,1,0 )  + real4( rotateZXY , 0.0 ) ;
				//旋转morph Normal
				real4 morphNormal = tex2Dlod( _MorphNormal, real4( uvMorph, 0, 0.0) );
				real4 morphNormalOffset = real4( 1,1,-1,-1 ) + morphNormal * real4( -2,-2,2,2 );
				real3 rotateNormalZ = RotateAroundAxis( real3( 0,0,0 ), morphNormalOffset.xyz, real3( 0,0,-1 ), rotation3D.z );
				real3 rotateNormalZX = RotateAroundAxis( real3( 0,0,0 ), rotateNormalZ, real3( -1,0,0 ), rotation3D.x );
				real3 rotateNormalZXY = RotateAroundAxis( real3( 0,0,0 ), rotateNormalZX, real3( 0,-1,0 ), rotation3D.y );
				
				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				o.ase_texcoord4 = v.ase_texcoord1;
				o.ase_texcoord5 = v.ase_texcoord2;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;

				//施加顶点偏移
				real3 vertexValue = offsetFinal.xyz;
				v.vertex.xyz += vertexValue;
				v.normal = rotateNormalZXY;
				real3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				real4 positionCS = TransformWorldToHClip( positionWS );
				o.worldPos = positionWS;
				
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				
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
				
				real3 WorldPosition = IN.worldPos;
				real4 ShadowCoords = real4( 0, 0, 0, 0 );
				ShadowCoords = IN.shadowCoord;
				
				//得到粒子和物体之间的距离均一化mask
				real4 particleCenter = (real4(IN.ase_texcoord4.w , IN.ase_texcoord5.x , IN.ase_texcoord5.y , 0.0));
				real distanceMask = ( 1.0 - distance( particleCenter , _Affector ) );
				real clampResult = clamp( (0.0 + (( distanceMask + ( _Distance - 1.0 ) ) - 0.0) *
					(_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				real resultMask = pow( clampResult , _DistancePower );
				
				//反转自发光效果
				#ifdef _FLIPEMISSIONMASK_ON
				real flipEmissionMask = ( 1.0 - resultMask );
				#else
				real flipEmissionMask = resultMask;
				#endif

				//自发光颜色调整
				real2 positionMaskUV = real2(saturate(Remap(WorldPosition.x,_PositionRange.x,_PositionRange.z,0,1))
					,saturate(Remap(WorldPosition.z,_PositionRange.y,_PositionRange.w,0,1)));
				real4 positionMask = tex2D(_RampColor,positionMaskUV);
				real clampFlipEmissionMask = clamp(  flipEmissionMask * _FinalMaskMultiply  , 0.0 , 1.0 );
				real4 lerpColor = lerp( _FinalColorTwo , positionMask , clampFlipEmissionMask);
				
				//读取自发光贴图
				real2 uv_Emission = IN.ase_texcoord3.xy * _Emission_ST.xy + _Emission_ST.zw;
				half4 colorEmission = ( lerpColor * IN.ase_color * _FinalPower * tex2D( _Emission, uv_Emission ).r
					* IN.ase_color.a );
				real2 uv_Albedo = IN.ase_texcoord3.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				half4 colorAlbedo = _ColorTint * tex2D( _Albedo, uv_Albedo );
				real4 colorFinal = lerp( colorAlbedo , colorEmission , colorEmission.a);
				
				real3 Color = colorFinal.rgb;
				return real4( Color, 1.0 );
			}

			ENDHLSL
		}

	}
	Fallback "Hidden/InternalErrorShader"
}