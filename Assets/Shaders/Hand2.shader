Shader "Custom/URP_EtherealHand_BurningEdge"
{
    Properties
    {
        [HDR] _EnergyColor("Energy Color", Color) = (1, 0, 0, 1)
        _BaseColor("Core Color", Color) = (0.2, 0.2, 0.2, 1)

        [Header(Light Interaction)]
        [HDR] _EmissionColor("Burn Edge Color", Color) = (1, 1, 0, 1) // Kolor krawêdzi (np. ¿ó³ty/pomarañczowy)
        _RepelAmount("Dissolve Amount", Range(0, 1)) = 0.0
        _BurnWidth("Burn Edge Width", Range(0.01, 0.5)) = 0.1 // NOWE: Szerokoœæ œwiec¹cej krawêdzi
        _EmissionIntensity("Flash Intensity", Float) = 2.0

        [Header(Base Settings)]
        _MainTex("Noise Texture", 2D) = "white" {}
        _FlowSpeed("Flow Speed", Float) = 0.5
        _Softness("Fresnel Glow (G³adkoœæ)", Float) = 2.0
        _CoreDarkness("Noise Intensity", Float) = 1.0
        _Alpha("Overall Alpha", Range(0,1)) = 1.0
        _UVScale("UV Scale (X, Y)", Vector) = (4, 4, 0, 0)
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.3
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "AlphaTest" "RenderPipeline" = "UniversalPipeline" }
            LOD 100

            Pass
            {
                Name "UniversalForward"
                Tags { "LightMode" = "UniversalForward" }
                Cull Off
                ZWrite On

                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes {
                    float4 positionOS : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalOS : NORMAL;
                };

                struct Varyings {
                    float4 positionHCS : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;
                    float3 viewDirWS : TEXCOORD2;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                CBUFFER_START(UnityPerMaterial)
                    float4 _EnergyColor;
                    float4 _BaseColor;
                    float4 _EmissionColor;
                    float4 _MainTex_ST;
                    float _RepelAmount;
                    float _BurnWidth; // NOWE
                    float _EmissionIntensity;
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

                    float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                    float noiseVal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, flowUV).r;

                    float fresnel = 1.0 - abs(dot(normal, viewDir));
                    fresnel = pow(fresnel, _Softness);

                    float colorMix = saturate((noiseVal * _CoreDarkness) + fresnel);
                    half3 finalRGB = lerp(_BaseColor.rgb, _EnergyColor.rgb, colorMix);

                    // --- NOWA LOGIKA WYPALANIA KRAWÊDZI ---
                    float currentAlpha = (noiseVal * _Alpha) - (_RepelAmount * 1.1);

                    // Obliczamy maskê dla krawêdzi (tylko tam, gdzie model zaraz zostanie odciêty)
                    // U¿ywamy smoothstep, aby uzyskaæ p³ynne przejœcie koloru na brzegu dziury
                    float edgeMask = smoothstep(_Cutoff, _Cutoff + _BurnWidth, currentAlpha);
                    float burnEdge = 1.0 - edgeMask;

                    // Œwiecenie pojawia siê tylko na krawêdziach dziur
                    half3 burnGlow = _EmissionColor.rgb * burnEdge * _EmissionIntensity * (_RepelAmount > 0 ? 1 : 0);
                    finalRGB += burnGlow;

                    // Odcinamy model
                    clip(currentAlpha - _Cutoff);

                    return half4(finalRGB, 1.0);
                }
                ENDHLSL
            }

            // Pass DepthOnly musi byæ identyczny pod k¹tem clip()
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

                struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
                struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

                TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

                CBUFFER_START(UnityPerMaterial)
                    float4 _EnergyColor; float4 _BaseColor; float4 _EmissionColor; float4 _MainTex_ST;
                    float _RepelAmount; float _BurnWidth; float _EmissionIntensity;
                    float _FlowSpeed; float _Softness; float _CoreDarkness; float _Alpha;
                    float4 _UVScale; float _Cutoff;
                CBUFFER_END

                Varyings vert(Attributes v) {
                    Varyings o;
                    o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                    o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                    return o;
                }

                half frag(Varyings i) : SV_TARGET {
                    float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                    float noiseVal = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, flowUV).r;
                    float currentAlpha = (noiseVal * _Alpha) - (_RepelAmount * 1.1);
                    clip(currentAlpha - _Cutoff);
                    return 0;
                }
                ENDHLSL
            }
        }
}