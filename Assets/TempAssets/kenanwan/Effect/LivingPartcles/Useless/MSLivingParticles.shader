Shader "MS_Shader/LivingParticles/LivingParticleMaskedMorphPbrURP"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][Toggle(_FLIPMORPHMASK_ON)] _FlipMorphMask("Flip Morph Mask", Float) = 0
		[Toggle(_FLIPEMISSIONMASK_ON)] _FlipEmissionMask("Flip Emission Mask", Float) = 0
		[Toggle(_FLIPOFFSETMASK_ON)] _FlipOffsetMask("Flip Offset Mask", Float) = 0
		_MorphMain("Morph Main", 2D) = "white" {}
		_MorphNormal("Morph Normal", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_ColorTint("Color Tint", Color) = (1,1,1,1)
		_MetallicSmoothness("MetallicSmoothness", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_Emission("Emission", 2D) = "white" {}
		_FinalColorOne("Final Color One", Color) = (1,0,0,1)
		_FinalColorTwo("Final Color Two", Color) = (0,0,0,0)
		_FinalPower("Final Power", Range( 0 , 20)) = 6
		_FinalMaskMultiply("Final Mask Multiply", Range( 0 , 10)) = 2
		[Toggle(_RAMPENABLED_ON)] _RampEnabled("Ramp Enabled", Float) = 0
		_Ramp("Ramp", 2D) = "white" {}
		_Distance("Distance", Float) = 1
		_DistancePower("Distance Power", Range( 0.2 , 4)) = 1
		_DistanceRemap("Distance Remap", Range( 1 , 4)) = 1
		[Toggle(_OFFSETYLOCK_ON)] _OffsetYLock("Offset Y Lock", Float) = 0
		_OffsetPower("Offset Power", Float) = 0
		[Toggle(_CENTERMASKENABLED_ON)] _CenterMaskEnabled("Center Mask Enabled", Float) = 0
		_CenterMaskMultiply("Center Mask Multiply", Float) = 4
		_CenterMaskSubtract("Center Mask Subtract", Float) = 0.75
		[Toggle(_VERTEXDISTORTIONENABLED_ON)] _VertexDistortionEnabled("Vertex Distortion Enabled", Float) = 0
		_VertexOffsetTexture("Vertex Offset Texture", 2D) = "white" {}
		_VertexDistortionPower("Vertex Distortion Power", Float) = 0.1
		_VertexDistortionTiling("Vertex Distortion Tiling", Float) = 1
		[ASEEnd][Toggle(_USEGAMMARENDERING_ON)] _UseGammaRendering("Use Gamma Rendering", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		LOD 0
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Back
		AlphaToMask Off
		
		HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		
		#pragma target 3.0

		#pragma shader_feature _VERTEXDISTORTIONENABLED_ON
		#pragma shader_feature _CENTERMASKENABLED_ON
		#pragma shader_feature _FLIPOFFSETMASK_ON
		#pragma shader_feature _OFFSETYLOCK_ON
		#pragma shader_feature _USEGAMMARENDERING_ON
		#pragma shader_feature _FLIPMORPHMASK_ON
		#pragma shader_feature _RAMPENABLED_ON
		#pragma shader_feature _FLIPEMISSIONMASK_ON

		CBUFFER_START(UnityPerMaterial)
			float4 _Albedo_ST;
			float4 _Emission_ST;
			float4 _FinalColorOne;
			float4 _FinalColorTwo;
			float4 _Normal_ST;
			float4 _ColorTint;
			float4 _MetallicSmoothness_ST;
			float _Metallic;
			float _FinalPower;
			float _FinalMaskMultiply;
			float _VertexDistortionPower;
			float _CenterMaskMultiply;
			float _CenterMaskSubtract;
			float _DistancePower;
			float _DistanceRemap;
			float _Distance;
			float _VertexDistortionTiling;
			float _OffsetPower;
			float _Smoothness;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
		CBUFFER_END

		inline float4 TriplanarSampling111( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
			{
				float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
				projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
				float3 nsign = sign( worldNormal );
				half4 xNorm; half4 yNorm; half4 zNorm;
				xNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.zy * float2(  nsign.x, 1.0 ), 0, 0) );
				yNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.xz * float2(  nsign.y, 1.0 ), 0, 0) );
				zNorm = tex2Dlod( topTexMap, float4(tiling * worldPos.xy * float2( -nsign.z, 1.0 ), 0, 0) );
				return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
			}
			
		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
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
			
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS _ADDITIONAL_OFF
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD
			
			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				
				float4 ase_texcoord7 : TEXCOORD7;
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_texcoord9 : TEXCOORD9;
				float4 ase_color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			sampler2D _VertexOffsetTexture;
			float4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;
			sampler2D _Albedo;
			sampler2D _Normal;
			sampler2D _Ramp;
			sampler2D _Emission;
			sampler2D _MetallicSmoothness;

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float4 triplanar111 = TriplanarSampling111( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling, 1.0, 0 );
				float4 break114 = triplanar111;
				float3 appendResult115 = (float3(break114.x , break114.y , break114.z));
				#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
				#else
				float3 staticSwitch120 = temp_cast_0;
				#endif
				float4 appendResult17 = (float4(v.texcoord1.xyzw.w , v.ase_texcoord2.x , v.ase_texcoord2.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPOFFSETMASK_ON
				float staticSwitch225 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch225 = ResultMask53;
				#endif
				float clampResult105 = clamp( ( staticSwitch225 - _CenterMaskSubtract ) , 0.0 , 1.0 );
				#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( staticSwitch225 - ( clampResult105 * _CenterMaskMultiply ) );
				#else
				float staticSwitch109 = staticSwitch225;
				#endif
				float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
				float4 CenterVector44 = normalizeResult41;
				float3 temp_cast_2 = (1.0).xxx;
				#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
				#else
				float3 staticSwitch49 = temp_cast_2;
				#endif
				float3 appendResult218 = (float3(v.ase_texcoord2.z , v.ase_texcoord2.w , v.ase_texcoord3.x));
				float3 RotationInRadian216 = -appendResult218;
				float3 break207 = RotationInRadian216;
				#ifdef _FLIPMORPHMASK_ON
				float staticSwitch202 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch202 = ResultMask53;
				#endif
				float2 appendResult177 = (float2(v.texcoord1.xyzw.x , ( v.texcoord1.xyzw.y + (-0.5 + (staticSwitch202 - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) )));
				float4 tex2DNode132 = tex2Dlod( _MorphMain, float4( appendResult177, 0, 0.0) );
				float3 gammaToLinear230 = FastSRGBToLinear( tex2DNode132.rgb );
				#ifdef _USEGAMMARENDERING_ON
				float4 staticSwitch229 = float4( gammaToLinear230 , 0.0 );
				#else
				float4 staticSwitch229 = tex2DNode132;
				#endif
				float4 break179 = staticSwitch229;
				float4 appendResult184 = (float4(( break179.r * -1.0 ) , ( break179.g * -1.0 ) , ( break179.b * 1.0 ) , ( break179.a * 1.0 )));
				float4 Morph186 = ( appendResult184 * v.texcoord1.xyzw.z );
				float3 rotatedValue208 = RotateAroundAxis( float3( 0,0,0 ), Morph186.xyz, float3( 0,0,-1 ), break207.z );
				float3 rotatedValue210 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue208, float3( -1,0,0 ), break207.x );
				float3 rotatedValue213 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue210, float3( 0,-1,0 ), break207.y );
				float4 OffsetFinal154 = ( ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ) + float4( rotatedValue213 , 0.0 ) );
				
				float3 break206 = RotationInRadian216;
				float4 break191 = tex2Dlod( _MorphNormal, float4( appendResult177, 0, 0.0) );
				float4 appendResult196 = (float4(( break191.r * 1.0 ) , ( break191.g * 1.0 ) , ( break191.b * 1.0 ) , ( break191.a * 1.0 )));
				float4 MorphNormals152 = (float4( 1,1,-1,-1 ) + (appendResult196 - float4( 0,0,0,0 )) * (float4( -1,-1,1,1 ) - float4( 1,1,-1,-1 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
				float3 rotatedValue209 = RotateAroundAxis( float3( 0,0,0 ), MorphNormals152.xyz, float3( 0,0,-1 ), break206.z );
				float3 rotatedValue211 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue209, float3( -1,0,0 ), break206.x );
				float3 rotatedValue212 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue211, float3( 0,-1,0 ), break206.y );
				float3 VertexNormalsFinal222 = rotatedValue212;
				
				o.ase_texcoord7.xy = v.texcoord.xy;
				o.ase_texcoord8 = v.texcoord1.xyzw;
				o.ase_texcoord9 = v.ase_texcoord2;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = OffsetFinal154.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = VertexNormalsFinal222;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_Albedo = IN.ase_texcoord7.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				
				float2 uv_Normal = IN.ase_texcoord7.xy * _Normal_ST.xy + _Normal_ST.zw;
				
				float4 appendResult17 = (float4(IN.ase_texcoord8.w , IN.ase_texcoord9.x , IN.ase_texcoord9.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPEMISSIONMASK_ON
				float staticSwitch223 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch223 = ResultMask53;
				#endif
				float clampResult88 = clamp( ( staticSwitch223 * _FinalMaskMultiply ) , 0.0 , 1.0 );
				float4 lerpResult37 = lerp( _FinalColorTwo , _FinalColorOne , clampResult88);
				float2 appendResult83 = (float2(clampResult88 , 0.0));
				#ifdef _RAMPENABLED_ON
				float4 staticSwitch81 = tex2D( _Ramp, appendResult83 );
				#else
				float4 staticSwitch81 = lerpResult37;
				#endif
				float2 uv_Emission = IN.ase_texcoord7.xy * _Emission_ST.xy + _Emission_ST.zw;
				float4 Emission250 = ( staticSwitch81 * IN.ase_color * _FinalPower * tex2D( _Emission, uv_Emission ).r * IN.ase_color.a );
				
				float2 uv_MetallicSmoothness = IN.ase_texcoord7.xy * _MetallicSmoothness_ST.xy + _MetallicSmoothness_ST.zw;
				float4 tex2DNode90 = tex2D( _MetallicSmoothness, uv_MetallicSmoothness );
				float Metallic253 = ( _Metallic * tex2DNode90.r );
				
				float Smoothness254 = ( tex2DNode90.a * _Smoothness );
				
				float3 Albedo = ( ( _ColorTint * tex2D( _Albedo, uv_Albedo ) ) + float4( 0,0,0,0 ) ).rgb;
				float3 Normal = ( UnpackNormalScale( tex2D( _Normal, uv_Normal ), 1.0f ) + float3( 0,0,0 ) );
				float3 Emission = Emission250.rgb;
				float3 Specular = 0.5;
				float Metallic = Metallic253;
				float Smoothness = Smoothness254;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				
				inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.clipPos);
				inputData.shadowMask = SAMPLE_SHADOWMASK(IN.lightmapUVOrVertexSH.xy);

				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, WorldNormal ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif

				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}
		
		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_SHADOWCASTER
			
			#define ASE_NEEDS_VERT_NORMAL

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _VertexOffsetTexture;
			float4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;

			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 temp_cast_0 = (0.0).xxx;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float4 triplanar111 = TriplanarSampling111( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling, 1.0, 0 );
				float4 break114 = triplanar111;
				float3 appendResult115 = (float3(break114.x , break114.y , break114.z));
				#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
				#else
				float3 staticSwitch120 = temp_cast_0;
				#endif
				float4 appendResult17 = (float4(v.ase_texcoord1.w , v.ase_texcoord2.x , v.ase_texcoord2.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPOFFSETMASK_ON
				float staticSwitch225 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch225 = ResultMask53;
				#endif
				float clampResult105 = clamp( ( staticSwitch225 - _CenterMaskSubtract ) , 0.0 , 1.0 );
				#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( staticSwitch225 - ( clampResult105 * _CenterMaskMultiply ) );
				#else
				float staticSwitch109 = staticSwitch225;
				#endif
				float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
				float4 CenterVector44 = normalizeResult41;
				float3 temp_cast_2 = (1.0).xxx;
				#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
				#else
				float3 staticSwitch49 = temp_cast_2;
				#endif
				float3 appendResult218 = (float3(v.ase_texcoord2.z , v.ase_texcoord2.w , v.ase_texcoord3.x));
				float3 RotationInRadian216 = -appendResult218;
				float3 break207 = RotationInRadian216;
				#ifdef _FLIPMORPHMASK_ON
				float staticSwitch202 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch202 = ResultMask53;
				#endif
				float2 appendResult177 = (float2(v.ase_texcoord1.x , ( v.ase_texcoord1.y + (-0.5 + (staticSwitch202 - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) )));
				float4 tex2DNode132 = tex2Dlod( _MorphMain, float4( appendResult177, 0, 0.0) );
				float3 gammaToLinear230 = FastSRGBToLinear( tex2DNode132.rgb );
				#ifdef _USEGAMMARENDERING_ON
				float4 staticSwitch229 = float4( gammaToLinear230 , 0.0 );
				#else
				float4 staticSwitch229 = tex2DNode132;
				#endif
				float4 break179 = staticSwitch229;
				float4 appendResult184 = (float4(( break179.r * -1.0 ) , ( break179.g * -1.0 ) , ( break179.b * 1.0 ) , ( break179.a * 1.0 )));
				float4 Morph186 = ( appendResult184 * v.ase_texcoord1.z );
				float3 rotatedValue208 = RotateAroundAxis( float3( 0,0,0 ), Morph186.xyz, float3( 0,0,-1 ), break207.z );
				float3 rotatedValue210 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue208, float3( -1,0,0 ), break207.x );
				float3 rotatedValue213 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue210, float3( 0,-1,0 ), break207.y );
				float4 OffsetFinal154 = ( ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ) + float4( rotatedValue213 , 0.0 ) );
				
				float3 break206 = RotationInRadian216;
				float4 break191 = tex2Dlod( _MorphNormal, float4( appendResult177, 0, 0.0) );
				float4 appendResult196 = (float4(( break191.r * 1.0 ) , ( break191.g * 1.0 ) , ( break191.b * 1.0 ) , ( break191.a * 1.0 )));
				float4 MorphNormals152 = (float4( 1,1,-1,-1 ) + (appendResult196 - float4( 0,0,0,0 )) * (float4( -1,-1,1,1 ) - float4( 1,1,-1,-1 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
				float3 rotatedValue209 = RotateAroundAxis( float3( 0,0,0 ), MorphNormals152.xyz, float3( 0,0,-1 ), break206.z );
				float3 rotatedValue211 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue209, float3( -1,0,0 ), break206.x );
				float3 rotatedValue212 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue211, float3( 0,-1,0 ), break206.y );
				float3 VertexNormalsFinal222 = rotatedValue212;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = OffsetFinal154.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = VertexNormalsFinal222;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}
			
			ENDHLSL
		}
		
		Pass
		{
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY
			
			#define ASE_NEEDS_VERT_NORMAL
			
			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _VertexOffsetTexture;
			float4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float4 triplanar111 = TriplanarSampling111( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling, 1.0, 0 );
				float4 break114 = triplanar111;
				float3 appendResult115 = (float3(break114.x , break114.y , break114.z));
				#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
				#else
				float3 staticSwitch120 = temp_cast_0;
				#endif
				float4 appendResult17 = (float4(v.ase_texcoord1.w , v.ase_texcoord2.x , v.ase_texcoord2.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPOFFSETMASK_ON
				float staticSwitch225 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch225 = ResultMask53;
				#endif
				float clampResult105 = clamp( ( staticSwitch225 - _CenterMaskSubtract ) , 0.0 , 1.0 );
				#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( staticSwitch225 - ( clampResult105 * _CenterMaskMultiply ) );
				#else
				float staticSwitch109 = staticSwitch225;
				#endif
				float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
				float4 CenterVector44 = normalizeResult41;
				float3 temp_cast_2 = (1.0).xxx;
				#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
				#else
				float3 staticSwitch49 = temp_cast_2;
				#endif
				float3 appendResult218 = (float3(v.ase_texcoord2.z , v.ase_texcoord2.w , v.ase_texcoord3.x));
				float3 RotationInRadian216 = -appendResult218;
				float3 break207 = RotationInRadian216;
				#ifdef _FLIPMORPHMASK_ON
				float staticSwitch202 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch202 = ResultMask53;
				#endif
				float2 appendResult177 = (float2(v.ase_texcoord1.x , ( v.ase_texcoord1.y + (-0.5 + (staticSwitch202 - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) )));
				float4 tex2DNode132 = tex2Dlod( _MorphMain, float4( appendResult177, 0, 0.0) );
				float3 gammaToLinear230 = FastSRGBToLinear( tex2DNode132.rgb );
				#ifdef _USEGAMMARENDERING_ON
				float4 staticSwitch229 = float4( gammaToLinear230 , 0.0 );
				#else
				float4 staticSwitch229 = tex2DNode132;
				#endif
				float4 break179 = staticSwitch229;
				float4 appendResult184 = (float4(( break179.r * -1.0 ) , ( break179.g * -1.0 ) , ( break179.b * 1.0 ) , ( break179.a * 1.0 )));
				float4 Morph186 = ( appendResult184 * v.ase_texcoord1.z );
				float3 rotatedValue208 = RotateAroundAxis( float3( 0,0,0 ), Morph186.xyz, float3( 0,0,-1 ), break207.z );
				float3 rotatedValue210 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue208, float3( -1,0,0 ), break207.x );
				float3 rotatedValue213 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue210, float3( 0,-1,0 ), break207.y );
				float4 OffsetFinal154 = ( ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ) + float4( rotatedValue213 , 0.0 ) );
				
				float3 break206 = RotationInRadian216;
				float4 break191 = tex2Dlod( _MorphNormal, float4( appendResult177, 0, 0.0) );
				float4 appendResult196 = (float4(( break191.r * 1.0 ) , ( break191.g * 1.0 ) , ( break191.b * 1.0 ) , ( break191.a * 1.0 )));
				float4 MorphNormals152 = (float4( 1,1,-1,-1 ) + (appendResult196 - float4( 0,0,0,0 )) * (float4( -1,-1,1,1 ) - float4( 1,1,-1,-1 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
				float3 rotatedValue209 = RotateAroundAxis( float3( 0,0,0 ), MorphNormals152.xyz, float3( 0,0,-1 ), break206.z );
				float3 rotatedValue211 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue209, float3( -1,0,0 ), break206.x );
				float3 rotatedValue212 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue211, float3( 0,-1,0 ), break206.y );
				float3 VertexNormalsFinal222 = rotatedValue212;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = OffsetFinal154.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = VertexNormalsFinal222;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}
			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif

				return 0;
			}
			ENDHLSL
		}
		
		Pass
		{
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META
			
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_COLOR

			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			sampler2D _VertexOffsetTexture;
			float4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;
			sampler2D _Albedo;
			sampler2D _Ramp;
			sampler2D _Emission;

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float4 triplanar111 = TriplanarSampling111( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling, 1.0, 0 );
				float4 break114 = triplanar111;
				float3 appendResult115 = (float3(break114.x , break114.y , break114.z));
				#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
				#else
				float3 staticSwitch120 = temp_cast_0;
				#endif
				float4 appendResult17 = (float4(v.texcoord1.w , v.texcoord2.x , v.texcoord2.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPOFFSETMASK_ON
				float staticSwitch225 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch225 = ResultMask53;
				#endif
				float clampResult105 = clamp( ( staticSwitch225 - _CenterMaskSubtract ) , 0.0 , 1.0 );
				#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( staticSwitch225 - ( clampResult105 * _CenterMaskMultiply ) );
				#else
				float staticSwitch109 = staticSwitch225;
				#endif
				float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
				float4 CenterVector44 = normalizeResult41;
				float3 temp_cast_2 = (1.0).xxx;
				#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
				#else
				float3 staticSwitch49 = temp_cast_2;
				#endif
				float3 appendResult218 = (float3(v.texcoord2.z , v.texcoord2.w , v.ase_texcoord3.x));
				float3 RotationInRadian216 = -appendResult218;
				float3 break207 = RotationInRadian216;
				#ifdef _FLIPMORPHMASK_ON
				float staticSwitch202 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch202 = ResultMask53;
				#endif
				float2 appendResult177 = (float2(v.texcoord1.x , ( v.texcoord1.y + (-0.5 + (staticSwitch202 - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) )));
				float4 tex2DNode132 = tex2Dlod( _MorphMain, float4( appendResult177, 0, 0.0) );
				float3 gammaToLinear230 = FastSRGBToLinear( tex2DNode132.rgb );
				#ifdef _USEGAMMARENDERING_ON
				float4 staticSwitch229 = float4( gammaToLinear230 , 0.0 );
				#else
				float4 staticSwitch229 = tex2DNode132;
				#endif
				float4 break179 = staticSwitch229;
				float4 appendResult184 = (float4(( break179.r * -1.0 ) , ( break179.g * -1.0 ) , ( break179.b * 1.0 ) , ( break179.a * 1.0 )));
				float4 Morph186 = ( appendResult184 * v.texcoord1.z );
				float3 rotatedValue208 = RotateAroundAxis( float3( 0,0,0 ), Morph186.xyz, float3( 0,0,-1 ), break207.z );
				float3 rotatedValue210 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue208, float3( -1,0,0 ), break207.x );
				float3 rotatedValue213 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue210, float3( 0,-1,0 ), break207.y );
				float4 OffsetFinal154 = ( ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ) + float4( rotatedValue213 , 0.0 ) );
				
				float3 break206 = RotationInRadian216;
				float4 break191 = tex2Dlod( _MorphNormal, float4( appendResult177, 0, 0.0) );
				float4 appendResult196 = (float4(( break191.r * 1.0 ) , ( break191.g * 1.0 ) , ( break191.b * 1.0 ) , ( break191.a * 1.0 )));
				float4 MorphNormals152 = (float4( 1,1,-1,-1 ) + (appendResult196 - float4( 0,0,0,0 )) * (float4( -1,-1,1,1 ) - float4( 1,1,-1,-1 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
				float3 rotatedValue209 = RotateAroundAxis( float3( 0,0,0 ), MorphNormals152.xyz, float3( 0,0,-1 ), break206.z );
				float3 rotatedValue211 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue209, float3( -1,0,0 ), break206.x );
				float3 rotatedValue212 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue211, float3( 0,-1,0 ), break206.y );
				float3 VertexNormalsFinal222 = rotatedValue212;
				
				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				o.ase_texcoord3 = v.texcoord1;
				o.ase_texcoord4 = v.texcoord2;
				o.ase_color = v.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = OffsetFinal154.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = VertexNormalsFinal222;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}
			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_Albedo = IN.ase_texcoord2.xy * _Albedo_ST.xy + _Albedo_ST.zw;
				
				float4 appendResult17 = (float4(IN.ase_texcoord3.w , IN.ase_texcoord4.x , IN.ase_texcoord4.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPEMISSIONMASK_ON
				float staticSwitch223 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch223 = ResultMask53;
				#endif
				float clampResult88 = clamp( ( staticSwitch223 * _FinalMaskMultiply ) , 0.0 , 1.0 );
				float4 lerpResult37 = lerp( _FinalColorTwo , _FinalColorOne , clampResult88);
				float2 appendResult83 = (float2(clampResult88 , 0.0));
				#ifdef _RAMPENABLED_ON
				float4 staticSwitch81 = tex2D( _Ramp, appendResult83 );
				#else
				float4 staticSwitch81 = lerpResult37;
				#endif
				float2 uv_Emission = IN.ase_texcoord2.xy * _Emission_ST.xy + _Emission_ST.zw;
				float4 Emission250 = ( staticSwitch81 * IN.ase_color * _FinalPower * tex2D( _Emission, uv_Emission ).r * IN.ase_color.a );
				
				
				float3 Albedo = ( ( _ColorTint * tex2D( _Albedo, uv_Albedo ) ) + float4( 0,0,0,0 ) ).rgb;
				float3 Emission = Emission250.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
            ZTest LEqual
            ZWrite On

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_SRP_VERSION 999999

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHNORMALSONLY
			
			#define ASE_NEEDS_VERT_NORMAL

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float3 worldNormal : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _VertexOffsetTexture;
			float4 _Affector;
			sampler2D _MorphMain;
			sampler2D _MorphNormal;
 
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float3 temp_cast_0 = (0.0).xxx;
				float3 ase_worldPos = mul(GetObjectToWorldMatrix(), v.vertex).xyz;
				float3 ase_worldNormal = TransformObjectToWorldNormal(v.ase_normal);
				float4 triplanar111 = TriplanarSampling111( _VertexOffsetTexture, ase_worldPos, ase_worldNormal, 1.0, _VertexDistortionTiling, 1.0, 0 );
				float4 break114 = triplanar111;
				float3 appendResult115 = (float3(break114.x , break114.y , break114.z));
				#ifdef _VERTEXDISTORTIONENABLED_ON
				float3 staticSwitch120 = ( _VertexDistortionPower * (float3( -1,-1,-1 ) + (appendResult115 - float3( 0,0,0 )) * (float3( 1,1,1 ) - float3( -1,-1,-1 )) / (float3( 1,1,1 ) - float3( 0,0,0 ))) );
				#else
				float3 staticSwitch120 = temp_cast_0;
				#endif
				float4 appendResult17 = (float4(v.ase_texcoord1.w , v.ase_texcoord2.x , v.ase_texcoord2.y , 0.0));
				float DistanceMask45 = ( 1.0 - distance( appendResult17 , _Affector ) );
				float clampResult23 = clamp( (0.0 + (( DistanceMask45 + ( _Distance - 1.0 ) ) - 0.0) * (_DistanceRemap - 0.0) / (_Distance - 0.0)) , 0.0 , 1.0 );
				float ResultMask53 = pow( clampResult23 , _DistancePower );
				#ifdef _FLIPOFFSETMASK_ON
				float staticSwitch225 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch225 = ResultMask53;
				#endif
				float clampResult105 = clamp( ( staticSwitch225 - _CenterMaskSubtract ) , 0.0 , 1.0 );
				#ifdef _CENTERMASKENABLED_ON
				float staticSwitch109 = ( staticSwitch225 - ( clampResult105 * _CenterMaskMultiply ) );
				#else
				float staticSwitch109 = staticSwitch225;
				#endif
				float4 normalizeResult41 = normalize( ( appendResult17 - _Affector ) );
				float4 CenterVector44 = normalizeResult41;
				float3 temp_cast_2 = (1.0).xxx;
				#ifdef _OFFSETYLOCK_ON
				float3 staticSwitch49 = float3(1,0,1);
				#else
				float3 staticSwitch49 = temp_cast_2;
				#endif
				float3 appendResult218 = (float3(v.ase_texcoord2.z , v.ase_texcoord2.w , v.ase_texcoord3.x));
				float3 RotationInRadian216 = -appendResult218;
				float3 break207 = RotationInRadian216;
				#ifdef _FLIPMORPHMASK_ON
				float staticSwitch202 = ( 1.0 - ResultMask53 );
				#else
				float staticSwitch202 = ResultMask53;
				#endif
				float2 appendResult177 = (float2(v.ase_texcoord1.x , ( v.ase_texcoord1.y + (-0.5 + (staticSwitch202 - 0.0) * (0.5 - -0.5) / (1.0 - 0.0)) )));
				float4 tex2DNode132 = tex2Dlod( _MorphMain, float4( appendResult177, 0, 0.0) );
				float3 gammaToLinear230 = FastSRGBToLinear( tex2DNode132.rgb );
				#ifdef _USEGAMMARENDERING_ON
				float4 staticSwitch229 = float4( gammaToLinear230 , 0.0 );
				#else
				float4 staticSwitch229 = tex2DNode132;
				#endif
				float4 break179 = staticSwitch229;
				float4 appendResult184 = (float4(( break179.r * -1.0 ) , ( break179.g * -1.0 ) , ( break179.b * 1.0 ) , ( break179.a * 1.0 )));
				float4 Morph186 = ( appendResult184 * v.ase_texcoord1.z );
				float3 rotatedValue208 = RotateAroundAxis( float3( 0,0,0 ), Morph186.xyz, float3( 0,0,-1 ), break207.z );
				float3 rotatedValue210 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue208, float3( -1,0,0 ), break207.x );
				float3 rotatedValue213 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue210, float3( 0,-1,0 ), break207.y );
				float4 OffsetFinal154 = ( ( float4( staticSwitch120 , 0.0 ) + ( staticSwitch109 * CenterVector44 * _OffsetPower * float4( staticSwitch49 , 0.0 ) ) ) + float4( rotatedValue213 , 0.0 ) );
				
				float3 break206 = RotationInRadian216;
				float4 break191 = tex2Dlod( _MorphNormal, float4( appendResult177, 0, 0.0) );
				float4 appendResult196 = (float4(( break191.r * 1.0 ) , ( break191.g * 1.0 ) , ( break191.b * 1.0 ) , ( break191.a * 1.0 )));
				float4 MorphNormals152 = (float4( 1,1,-1,-1 ) + (appendResult196 - float4( 0,0,0,0 )) * (float4( -1,-1,1,1 ) - float4( 1,1,-1,-1 )) / (float4( 1,1,1,1 ) - float4( 0,0,0,0 )));
				float3 rotatedValue209 = RotateAroundAxis( float3( 0,0,0 ), MorphNormals152.xyz, float3( 0,0,-1 ), break206.z );
				float3 rotatedValue211 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue209, float3( -1,0,0 ), break206.x );
				float3 rotatedValue212 = RotateAroundAxis( float3( 0,0,0 ), rotatedValue211, float3( 0,-1,0 ), break206.y );
				float3 VertexNormalsFinal222 = rotatedValue212;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = OffsetFinal154.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = VertexNormalsFinal222;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal( v.ase_normal );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.worldNormal = normalWS;

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}
			
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
		
			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				
				return float4(PackNormalOctRectEncode(TransformWorldToViewDir(IN.worldNormal, true)), 0.0, 0.0);
			}
			ENDHLSL
		}
	}
	/*ase_lod*/
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
}
