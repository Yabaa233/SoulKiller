Shader "MS_Shader/HeightFog"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _FogNoiseTex ("FogNoiseTex", 2D) = "white" {}
        [HDR]_FogColor("_FogColor (default = 1,1,1,1)", color) = (1,1,1,1)
    }
    
    HLSLINCLUDE
    
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    
    CBUFFER_START(UnityPerMaterial)
    float4 _MainTex_ST;
    half4 _FogColor;
    float _FogStartHeight;
    float _FogEndHeight;
    float _FogIntensity;
    float3 _Forward;
    CBUFFER_END

    TEXTURE2D(_MainTex);
    SAMPLER(sampler_MainTex);
    TEXTURE2D(_FogNoiseTex);
    SAMPLER(sampler_FogNoiseTex);
    TEXTURE2D(_CameraDepthTexture);
    SAMPLER(sampler_CameraDepthTexture);

    struct appdata {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f {
        float4 positionCS : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 viewRayWorld : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    //vertex shader
    v2f vert(appdata v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.positionCS = TransformObjectToHClip(v.positionOS.xyz);

        float sceneRawDepth = 1;
        #if defined(UNITY_REVERSED_Z)
            sceneRawDepth = 1 - sceneRawDepth;
        #endif
        
        float3 worldPos = ComputeWorldSpacePosition(v.uv, sceneRawDepth, UNITY_MATRIX_I_VP);
        o.viewRayWorld = worldPos - _WorldSpaceCameraPos.xyz;
        o.uv = v.uv;
        return o;
    }

    //fragment shader
    float4 frag(v2f i) : SV_Target
    {
        
        // float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv);
        // float linear01Depth = Linear01Depth(sceneRawDepth, _ZBufferParams);
        // float3 worldPos = _WorldSpaceCameraPos.xyz + ( linear01Depth) * i.viewRayWorld;
        
        float rawDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, i.uv).x;
        //获得裁剪空间坐标
        float4 HClipPos = float4(i.uv * 2 - 1, rawDepth, 1);
        //判断是否DirectX平台需翻转y坐标，对于正确实现效果很重要。具体解释可查看Packages/Core RP Library/ShaderLibrary/Common.hlsl中的ComputeClipSpacePosition函数
        #if UNITY_UV_STARTS_AT_TOP
        HClipPos.y = -HClipPos.y;
        #endif
        //视角*投影矩阵的逆矩阵矩阵乘法，获得世界坐标
        float4 worldPos = mul(UNITY_MATRIX_I_VP, HClipPos);
        worldPos.xyz = worldPos.xyz / worldPos.w;
        
        float blendParam  = ((_FogEndHeight - worldPos.y) / _FogEndHeight - _FogStartHeight);
        blendParam = saturate(blendParam * _FogIntensity);

        float2 noise_offseet1 = (_Time * float2(0.05,0.1));
		float2 noise_offseet2 = (_Time * float2(-1,-0.1));
		float noise = SAMPLE_TEXTURE2D(_FogNoiseTex,sampler_FogNoiseTex,i.uv + noise_offseet1)
        + SAMPLE_TEXTURE2D(_FogNoiseTex,sampler_FogNoiseTex,i.uv + noise_offseet2);
        blendParam *= noise;
        
        half4 screenCol =  SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex ,i.uv);
        //return half4(blendParam.xxx,1.0);
        return lerp(screenCol, _FogColor, blendParam );
    }
    ENDHLSL
    
    //开始SubShader
    SubShader
    {
        //Tags {"RenderType" = "Opaque"  "RenderPipeline" = "UniversalPipeline"}
        Tags{ "RenderPipeline" = "UniversalPipeline"  "RenderType" = "Overlay" "Queue" = "Transparent-499" "DisableBatching" = "True" }
            LOD 100
            ZTest Always Cull Off ZWrite Off
            Blend one zero
        Pass
        {
             Name "HeightFog"
             //后处理效果一般都是这几个状态
             //使用上面定义的vertex和fragment shader
             HLSLPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             ENDHLSL
        }
    }
    //后处理效果一般不给fallback，如果不支持，不显示后处理即可
}