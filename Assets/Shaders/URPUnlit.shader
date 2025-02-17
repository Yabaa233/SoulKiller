Shader "MS_Shader/URPUnlit"
{
    Properties
    {
        _MainTex("MainTex",2D)="White"{}
        _BaseColor("BaseColor",Color)=(1,1,1,1)
    }

    SubShader
    {
        Tags{ 
            "RenderPipeline"="UniversalRenderPipeline"  
            "RenderType"="Opaque"
            }
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_ST;
        half4 _BaseColor;
        CBUFFER_END

        TEXTURE2D (_MainTex);
        SAMPLER(sampler_MainTex);
             struct a2v
             {
                 float4 positionOS:POSITION;
                 float4 normalOS:NORMAL;
                 float2 texcoord:TEXCOORD;
             };
             struct v2f
             {
                 float4 positionCS:SV_POSITION;
                 float2 uv:TEXCOORD;
             };

        ENDHLSL
        pass

        {
            HLSLPROGRAM
            #pragma vertex VERT
            #pragma fragment FRAG
            v2f VERT(a2v i)
            {
                v2f o;
                o.positionCS=TransformObjectToHClip(i.positionOS.xyz);
                o.uv=TRANSFORM_TEX(i.texcoord,_MainTex);
                return o;
            }

            half4 FRAG(v2f i):SV_TARGET
            {
                half4 tex=SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv)*_BaseColor;
                return tex;
            }
            ENDHLSL
        }
    }
}