Shader "Custom/URP/ChromaKeyVideo"
{
    Properties
    {
        [MainTexture] _BaseMap("Video Texture / RenderTexture", 2D) = "white" {}
        [MainColor] _BaseColor("Color Tint", Color) = (1,1,1,1)

        _KeyColor("Key Color", Color) = (0,1,0,1)
        _Tolerance("Tolerance", Range(0,1)) = 0.35
        _Softness("Softness", Range(0,1)) = 0.08
        _Despill("Green Despill", Range(0,1)) = 0.35
        _AlphaMultiplier("Alpha Multiplier", Range(0,2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Name "ChromaKeyVideo"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float4 _KeyColor;
                float _Tolerance;
                float _Softness;
                float _Despill;
                float _AlphaMultiplier;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 videoColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                videoColor *= _BaseColor;

                float3 rgb = videoColor.rgb;
                float3 key = _KeyColor.rgb;

                // Distance from green key color.
                float colorDistance = distance(rgb, key);

                // Alpha becomes 0 near green, 1 away from green.
                float alpha = smoothstep(_Tolerance, _Tolerance + _Softness, colorDistance);

                alpha *= _AlphaMultiplier;
                alpha = saturate(alpha);

                // Reduce green spill around edges.
                float greenAmount = rgb.g - max(rgb.r, rgb.b);
                if (greenAmount > 0)
                {
                    rgb.g -= greenAmount * _Despill * (1.0 - alpha);
                }

                return half4(rgb, alpha * videoColor.a);
            }

            ENDHLSL
        }
    }
}