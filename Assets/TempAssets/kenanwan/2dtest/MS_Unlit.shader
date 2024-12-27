Shader "MS_Shader/BlinPhoneNormal"
{
      Properties
      {
            [Header(BaseColor  BaseTeture)]
            [Enum(OFF,0,ON,1)]_TexBlend("TexBlend",float)=1
            [Space(20)]
            _BaseColor("BaseColor",2D)="white"{}
            //三色混合
            [Header(Ramp Color)]
            [HDR]_UpColor("UpColor",Color)=(1,1,1,1)
            [HDR]_MidColor("MidColor",Color)=(1,1,1,1)
            [HDR]_DownColor("DownColor",Color)=(1,1,1,1)
            _UpMidInterval("UpMidInterval",range(0,1)) = 0.8
            _MidDownInterval("MidDownInterval",range(0,1)) = 0.3
            [Space(20)][Header(Normal)]
            [Normal]_NormalTex("Normal",2D)="bump"{}//注意这里是小写
            _NormalScale("NormalScale",Range(0,0.9))=0
            [Space(20)][Header(Specular)]
            _SpecularRange("SpecularRange",Range(1,200))=50
            [HDR]_SpecularColor("SpecularColor",Color)=(1,1,1,1)
            //是否接受额外的光源效果
            [KeywordEnum(ON,OFF)]_ADD_LIGHT("AddLight",int)=1
            //是否接受其他物体的阴影
            [Enum(OFF,0,ON,1)]_ShadowReceive("ShadowReceive",int)=1
            //是否开启吸收效果
            [Space(20)][KeywordEnum(ON,OFF)]_Absorb("Absorb",float)=1
            _AbsorbPoint("AbsorbPoint",vector) = (0,0,0,0)
            _AbsorbRadius("AbsorbRadius",float) = 0.5
            _AbsorbHardness("AbsorbHardness",float) = 0.5
            _AbsorbRotateScale("AbsorbRotateScale",float) = 20
            _AbsorbMaskSmoothness("AbsorbMaskSmoothness",range(0,1)) = 1
      }
      
      SubShader
      {
            Tags{
                  "RenderPipeline"="UniversalRenderPipeline" 
                  //"DisableBatching" = "True" 
            }

            HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #pragma shader_feature _ABSORB_ON _ABSORB_OFF
            
            CBUFFER_START(UnityPerMaterial)
            float4 _NormalTex_ST;
            float4 _BaseColor_ST;
            real _TexBlend;
            real _ShadowReceive;
            real _NormalScale;
            real _SpecularRange;
            real4 _SpecularColor;
            real4 _UpColor;
            real4 _MidColor;
            real4 _DownColor;
            real _UpMidInterval;
            real _MidDownInterval;
            real4 _AbsorbPoint;
            real _AbsorbRadius;
            real _AbsorbHardness;
            real _AbsorbRotateScale;
            real _AbsorbMaskSmoothness;
            CBUFFER_END
            
            TEXTURE2D(_BaseColor); SAMPLER(sampler_BaseColor);
            TEXTURE2D(_NormalTex); SAMPLER(sampler_NormalTex);
            
            struct a2v
            {
                  float3 positionOS:POSITION;
                  float2 texcoord:TEXCOORD0;
                  float3 normalOS:NORMAL;
                  float4 tangentOS:TANGENT;  
            } ;

            struct v2f
            {
                  float4 positionCS:SV_POSITION;
                  float4 texcoord:TEXCOORD0;
                  float4 tangentWS:TANGENT;
                  float4 normalWS:NORMAL;
                  float4 BtangentWS:TEXCOORD1;
            };

            //仿照ASE节点SphereMask
            real SphereMask(real3 position,real3 center,real radius,real hardness)
            {
                  real3 dir = (position - center) / radius;
                  real dis = dot(dir,dir);
                  real output = pow(saturate(dis),hardness);
                  return output;
            }
            //仿照ASE节点RotateAroundAxis
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
            
            //forward vert
            v2f forwardvert(a2v i)
            {
                  v2f o;
                  o.texcoord.xy=TRANSFORM_TEX(i.texcoord,_BaseColor);
                  o.texcoord.zw=TRANSFORM_TEX(i.texcoord,_NormalTex);
                  o.normalWS.xyz=normalize( TransformObjectToWorldNormal(i.normalOS));
                  o.tangentWS.xyz=normalize(TransformObjectToWorld(i.tangentOS));
                  o.BtangentWS.xyz=cross(o.normalWS.xyz,o.tangentWS.xyz)
                  *i.tangentOS.w*unity_WorldTransformParams.w;
                  //这里乘一个unity_WorldTransformParams.w是为判断是否使用了奇数相反的缩放
                  
                  float3 positionWS=TransformObjectToWorld(i.positionOS);
                  o.tangentWS.w=positionWS.x;
                  o.BtangentWS.w=positionWS.y;
                  o.normalWS.w=positionWS.z;

                  //吸收到原点
                  real3 vertexOffset=real3(0,0,0);
            #if _ABSORB_ON
                  float3 targetPos = _AbsorbPoint.xyz;
                  float spMask = SphereMask(positionWS,targetPos,_AbsorbRadius,_AbsorbHardness);
                  float mask = 1 - saturate(spMask);
                  float rotateAngle = sin(spMask);
                  
                  real3 pivot = TransformObjectToWorld(real3(0,0,0));
                  real3 axis = normalize(targetPos - pivot);
                  real angle = _AbsorbRotateScale * PI * rotateAngle;
                  real3 rotateAmount = RotateAroundAxis(targetPos,positionWS,axis,angle);
                  
                  real3 offset = TransformWorldToObject((targetPos - rotateAmount) * mask + rotateAmount);
                  real lerpAmount = smoothstep(0,_AbsorbMaskSmoothness,mask);
                  vertexOffset = lerp(i.positionOS,offset,lerpAmount);
            #else
                  vertexOffset=i.positionOS;
            #endif
                  
                  o.positionCS=TransformObjectToHClip(vertexOffset);
                  return   o;
            }  
            //forward frag
            real4 forwardfrag(v2f i):SV_TARGET
            {
                  float3 WSpos=float3(i.tangentWS.w,i.BtangentWS.w,i.normalWS.w);
                  float3x3 T2W={i.tangentWS.xyz,i.BtangentWS.xyz,i.normalWS.xyz};
                  real4 nortex=SAMPLE_TEXTURE2D(_NormalTex,sampler_NormalTex,i.texcoord.zw);
                  real3 normalTS=UnpackNormalScale(nortex,_NormalScale);
                  normalTS.z=pow((1-pow(normalTS.x,2)-pow(normalTS.y,2)),0.5);//规范化法线
                  //注意这里是右乘T2W的，等同于左乘T2W的逆
                  real3 norWS=mul(normalTS,T2W);
                  
                  Light mylight=GetMainLight(TransformWorldToShadowCoord(WSpos));
                  //计算半兰伯特
                  real halflambot=dot(norWS,normalize(mylight.direction))*0.5+0.5;

                  real4 diff=SAMPLE_TEXTURE2D(_BaseColor,sampler_BaseColor,i.texcoord.xy)
                  *halflambot*real4(mylight.color,1);
                              
                  _DownColor = lerp(_DownColor,_DownColor * diff, _TexBlend);
                  _MidColor = lerp(_MidColor,diff, _TexBlend);
                  _UpColor = lerp(_UpColor, _UpColor * diff, _TexBlend);
                  
                  real m1 = step(halflambot,_MidDownInterval);
                  real m2 = step(halflambot,_UpMidInterval);
                  real4 diffcol = (_DownColor - _MidColor) * m1
                   + m2 * (_MidColor - _UpColor) + _UpColor ; 
                  
                  //calcute addlight
                  real4 addcolor=real4(0,0,0,1);
            #if _ADD_LIGHT_ON
                  int addLightsCount = GetAdditionalLightsCount(); //定义在lighting库函数的方法 返回一个额外灯光的数量
                  for(int i=0;i<addLightsCount;i++)
                  {
                      Light addlight=GetAdditionalLight(i,WSpos);//定义在lightling库里的方法 返回一个灯光类型的数据
                      float3 WS_addLightDir=normalize(addlight.direction);
                      addcolor+=(halflambot*real4(addlight.color,1)*addlight.distanceAttenuation*addlight.shadowAttenuation);
                  }
            #else
                  addcolor=real4(0,0,0,1);
            #endif 

                  //计算高光  max(,0)控制点乘防止曝光
                  real spe= max(dot(normalize(normalize(mylight.direction)
                        +normalize(_WorldSpaceCameraPos-WSpos)),norWS),0);
                  spe=saturate(pow(spe,_SpecularRange));
                  
                  real4 col = spe*_SpecularColor+diffcol +addcolor;
                  real4 colReceiveShadow = col * (mylight.shadowAttenuation * 0.5 + 0.5);
                  col = lerp(col,colReceiveShadow, _ShadowReceive);
          
                  return col;
            }

            //shadow vert
            v2f vertshadow(a2v i)
            {
                  v2f o;
                  o.texcoord.xy=TRANSFORM_TEX(i.texcoord,_BaseColor);
                  float3 WSpos=TransformObjectToWorld(i.positionOS.xyz);
                  Light MainLight=GetMainLight();
                  float3 WSnor=TransformObjectToWorldNormal(i.normalOS.xyz);

                  //阴影顶点偏移开始
                  //吸收到原点
                  real3 vertexOffset=real3(0,0,0);
            #if _ABSORB_ON
                  float3 targetPos = _AbsorbPoint.xyz;
                  float spMask = SphereMask(WSpos,targetPos,_AbsorbRadius,_AbsorbHardness);
                  //float spMask = SphereMask(WSpos,targetPos,_Time.y,_AbsorbHardness);
                  float mask = 1 - saturate(spMask);
                  float rotateAngle = sin(spMask);
                  
                  real3 pivot = TransformObjectToWorld(real3(0,0,0));
                  real3 axis = normalize(targetPos - pivot);
                  real angle = _AbsorbRotateScale * PI * rotateAngle;
                  real3 rotateAmount = RotateAroundAxis(targetPos,WSpos,axis,angle);
                  
                  real3 offset = TransformWorldToObject((targetPos - rotateAmount) * mask + rotateAmount);
                  real lerpAmount = smoothstep(0,_AbsorbMaskSmoothness,mask);
                  vertexOffset = lerp(i.positionOS,offset,lerpAmount);
            #else
                   vertexOffset=i.positionOS;
             #endif
                  //阴影顶点偏移结束
                  
                  WSpos=TransformObjectToWorld(vertexOffset.xyz);
                  
                  o.positionCS=TransformWorldToHClip(ApplyShadowBias(WSpos  ,WSnor,MainLight.direction));
             #if UNITY_REVERSED_Z
                  o.positionCS.z=min(o.positionCS.z,o.positionCS.w*UNITY_NEAR_CLIP_VALUE);
             #else
                  o.positionCS.z=max(o.positionCS.z,o.positionCS.w*UNITY_NEAR_CLIP_VALUE);
             #endif
                  return o;
            }
            //shadow frag
            half4 fragshadow(v2f i):SV_TARGET
            {
                  #ifdef _CUT_ON 
                  float alpha=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.texcoord).a;
                  clip(alpha-_Cutoff);
                  #endif
                  return 0;
            } 
            
            ENDHLSL
            
            Pass {
                  NAME"MainPass"
                  Tags{
                        "LightMode"="UniversalForward"
                  }
                  Cull Off
                  HLSLPROGRAM
                  #pragma vertex forwardvert
                  #pragma fragment forwardfrag
                  #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                  #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                  #pragma shader_feature _ADD_LIGHT_ON _ADD_LIGHT_OFF
                  
                  ENDHLSL
            }
            
            Pass
            {
                  Tags{"LightMode" = "ShadowCaster"}
                  HLSLPROGRAM
                  
                  #pragma vertex vertshadow
                  #pragma fragment fragshadow
                  
                  ENDHLSL
            }
            
            //UsePass "Universal Render Pipeline/Lit/ShadowCaster"
      }
}