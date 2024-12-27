

Shader "Unlit/GrayColorShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _AttackedColor("AttackedColor",color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _DistortionMask("DistortionMask",2D) = "White"{}
        _AlphaAmount("AlphaAmount",float) = 1
        _TimeStampSpeed("TimeStampSpeed",float) = 1
        _ColorMask ("Color Mask", Float) = 15
        [HideinInspector]_TimeStamp("TimeStamp",float) = 0
    }
     
    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
     
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        
     
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"
     
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
     
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
     
            half4 _Color;
            half4 _AttackedColor;
            sampler2D _MainTex;
            sampler2D _DistortionMask;
            float _AlphaAmount;
            float _TimeStamp;
            float _TimeStampSpeed;
        
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color ;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }
     
     
            half4 frag(v2f IN) : SV_Target
            {
                half4 c = tex2D(_MainTex, IN.texcoord) * IN.color * _Color;
                half mask = tex2D(_DistortionMask,IN.texcoord).r;
                half3 originCol = IN.color.rgb ;
                half3 attackedCol = IN.color.rgb * _AttackedColor;
                // c.rgb = lerp( attackedCol, originCol ,saturate((_Time.y - _TimeStamp)*_TimeStampSpeed));
                // c.rgb = lerp( attackedCol, originCol ,(_CosTime.w+1)*0.5);
                c.rgb *= c.a;
                c.a = saturate(c.a * mask * _AlphaAmount);
                return c;
            }
        ENDCG
        }
    }
}
