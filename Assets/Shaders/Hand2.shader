Shader "Custom/URP_EtherealDarkEnergy3D"
{
    Properties
    {
        [HDR] _EnergyColor("Energy Color", Color) = (1, 0, 0, 1)
        _BaseColor("Core Color", Color) = (0.2, 0.2, 0.2, 1)

        _MainTex("Noise Texture", 2D) = "white" {}
        _FlowSpeed("Flow Speed", Float) = 0.5
        _Softness("Fresnel Glow (G³adkoœæ)", Float) = 2.0 // Steruje gruboœci¹ œwiec¹cej obwódki
        _CoreDarkness("Noise Intensity", Float) = 1.0
        _Alpha("Overall Alpha", Range(0,1)) = 1.0
        _UVScale("UV Scale (X, Y)", Vector) = (4, 4, 0, 0)
        _Cutoff("Alpha Cutoff (Dziury w modelu)", Range(0.0, 1.0)) = 0.3
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "Queue" = "AlphaTest"
                "RenderPipeline" = "UniversalPipeline"
            }
            LOD 100

            // ==========================================
            // PASS 1: KOLOR I ŒWIAT£O
            // ==========================================
            Pass
            {
                Name "UniversalForward"
                Tags { "LightMode" = "UniversalForward" }

                Cull Off
                ZWrite On
                ZTest LEqual

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes {
                    float4 positionOS   : POSITION;
                    float2 uv           : TEXCOORD0;
                    float3 normalOS     : NORMAL;
                };

                struct Varyings {
                    float4 positionHCS  : SV_POSITION;
                    float2 uv           : TEXCOORD0;
                    float3 normalWS     : TEXCOORD1;
                    float3 viewDirWS    : TEXCOORD2;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                CBUFFER_START(UnityPerMaterial)
                    float4 _EnergyColor;
                    float4 _BaseColor;
                    float4 _MainTex_ST;
                    float _FlowSpeed;
                    float _Softness;
                    float _CoreDarkness;
                    float _Alpha;
                    float4 _UVScale;
                    float _Cutoff;
                CBUFFER_END

                Varyings vert(Attributes v) {
                    Varyings o;
                    o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                    o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                    o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                    float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                    o.viewDirWS = GetWorldSpaceNormalizeViewDir(posWS);
                    return o;
                }

                half4 frag(Varyings i) : SV_Target {
                    float3 normal = normalize(i.normalWS);
                    float3 viewDir = normalize(i.viewDirWS);

                    // 1. Ruchomy Szum
                    float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                    float noiseVal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, flowUV).r;

                    // 2. K¹t Kamery (Odwrotny Fresnel - 1 na krawêdziach, 0 na œrodku)
                    float fresnel = 1.0 - abs(dot(normal, viewDir));
                    fresnel = pow(fresnel, _Softness);

                    // 3. Budowanie koloru: Baza + Szum + Œwiec¹ce krawêdzie z Fresnela
                    float colorMix = saturate((noiseVal * _CoreDarkness) + fresnel);
                    half3 finalRGB = lerp(_BaseColor.rgb, _EnergyColor.rgb, colorMix);

                    // 4. Wycinanie (tylko na podstawie szumu - brak b³êdów przy obracaniu kamery!)
                    float alphaVal = noiseVal * _Alpha;
                    clip(alphaVal - _Cutoff);

                    return half4(finalRGB, 1.0);
                }
                ENDHLSL
            }

            // ==========================================
            // PASS 2: ZAPIS G£ÊBI DLA WODY (Zoptymalizowany)
            // ==========================================
            Pass
            {
                Name "DepthOnly"
                Tags { "LightMode" = "DepthOnly" }

                Cull Off
                ZWrite On
                ColorMask 0

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes {
                    float4 positionOS   : POSITION;
                    float2 uv           : TEXCOORD0;
                };

                struct Varyings {
                    float4 positionHCS  : SV_POSITION;
                    float2 uv           : TEXCOORD0;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                CBUFFER_START(UnityPerMaterial)
                    float4 _EnergyColor;
                    float4 _BaseColor;
                    float4 _MainTex_ST;
                    float _FlowSpeed;
                    float _Softness;
                    float _CoreDarkness;
                    float _Alpha;
                    float4 _UVScale;
                    float _Cutoff;
                CBUFFER_END

                Varyings vert(Attributes v) {
                    Varyings o;
                    o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                    o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                    return o;
                }

                half frag(Varyings i) : SV_TARGET {
                    // W g³êbi obchodzi nas TYLKO wyciêcie dziur z szumu (¿eby woda mia³a dobry kszta³t)
                    float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                    float noiseVal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, flowUV).r;

                    float alphaVal = noiseVal * _Alpha;
                    clip(alphaVal - _Cutoff);

                    return 0;
                }
                ENDHLSL
            }
        }
}