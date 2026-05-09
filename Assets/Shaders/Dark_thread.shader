Shader "Unlit/EtherealDarkEnergy3D"
{
    Properties
    {
        [HDR] _EnergyColor("Energy Color", Color) = (0, 1, 0, 1) // Kolor szumu/energii (HDR)
        _MainTex("Noise Texture", 2D) = "white" {}  // Tekstura szumu (chmury)
        _FlowSpeed("Flow Speed", Float) = 0.5       // Prêdkoœæ p³yniêcia
        _Softness("Edge Softness (3D)", Float) = 1.5// Im wiêcej, tym bardziej przezroczyste krawêdzie
        _CoreDarkness("Core Darkness", Float) = 1.0 // Jak bardzo czarny jest œrodek
        _Alpha("Overall Alpha", Range(0,1)) = 1.0   // Ogólna przezroczystoœæ (zmieni³em domyœlne na 1, ¿eby by³o widaæ)
        _UVScale("UV Scale (X, Y)", Vector) = (4, 4, 0, 0) // Gêstoœæ wzoru (na obie osie)
    }
        SubShader
        {
            // POPRAWKA: Typ renderowania na "Transparent" zamiast "Opaque"
            Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
            LOD 100

            Pass
            {
                Cull Off // Wy³¹czony Culling - widaæ wnêtrze modelu (fajne dla eterycznych rzeczy)
                //Blend SrcAlpha OneMinusSrcAlpha
                ZWrite On
                ZTest LEqual

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL; // NOWE: Pobieramy "Normalne" (kierunek, w którym patrzy wierzcho³ek)
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float3 worldNormal : NORMAL;     // NOWE: Normalne w przestrzeni œwiata
                    float3 worldViewDir : TEXCOORD1; // NOWE: Kierunek patrzenia kamery
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _EnergyColor;
                float _FlowSpeed, _Softness, _CoreDarkness, _Alpha;
                float4 _UVScale;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                    // Obliczamy dane potrzebne do 3D
                    o.worldNormal = UnityObjectToWorldNormal(v.normal);
                    o.worldViewDir = WorldSpaceViewDir(v.vertex);

                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // NOWE: Normalizujemy wektory dla poprawnych obliczeñ œwiat³a/kamery
                    float3 normal = normalize(i.worldNormal);
                    float3 viewDir = normalize(i.worldViewDir);

                    // 1. P³yn¹cy Szum (zmodyfikowany pod X i Y dla 3D)
                    float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                    float noiseVal = tex2D(_MainTex, flowUV).r;

                    // 2. Miêkka maska kszta³tu dla modelu 3D
                    // dot() zwraca 1.0, gdy patrzymy prosto na powierzchniê, i 0.0 na zagiêciach bocznych (sylwetce).
                    // U¿ywamy abs(), aby wewnêtrzna strona siatki (widoczna dziêki Cull Off) te¿ dzia³a³a.
                    float facing = abs(dot(normal, viewDir));

                    // Maska jest mocna na œrodku, miêkka na krawêdziach bocznych
                    float softMask = pow(facing, _Softness);

                    // 3. Budowanie Koloru (Czarny Core + Kolorowy Szum)
                    fixed4 finalColor = fixed4(0,0,0,0);
                    fixed3 blackCore = fixed3(0, 0, 0);
                    fixed3 magicalEnergy = _EnergyColor.rgb * noiseVal;

                    // Mieszamy kolory w oparciu o szum
                    finalColor.rgb = lerp(blackCore, magicalEnergy, noiseVal);

                    float alphaVal = softMask * (noiseVal + 0.1) * _CoreDarkness * _Alpha;

                    clip(alphaVal - 0.5);

                    // 4. Przezroczystoœæ (Alpha)
                    //finalColor.a = softMask * (noiseVal + 0.1) * _CoreDarkness * _Alpha;

                    finalColor.a = 1.0; 

                    return finalColor;
                }
                ENDCG
            }

            Pass
            {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            ColorMask 0 // Nie rysujemy kolorów, tylko sam kszta³t g³êbi
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : NORMAL;
                float3 worldViewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Softness, _CoreDarkness, _Alpha, _FlowSpeed;
            float4 _UVScale;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldViewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                // Musimy powtórzyæ logikê odcinania szumu, ¿eby woda zna³a dok³adny kszta³t rêki
                float3 normal = normalize(i.worldNormal);
                float3 viewDir = normalize(i.worldViewDir);
                float2 flowUV = i.uv * _UVScale.xy + float2(0, _Time.y * _FlowSpeed);
                float noiseVal = tex2D(_MainTex, flowUV).r;
                float facing = abs(dot(normal, viewDir));
                float softMask = pow(facing, _Softness);

                float alphaVal = softMask * (noiseVal + 0.1) * _CoreDarkness * _Alpha;

                // Odcinamy to, co niewidoczne w g³ównym Passie
                clip(alphaVal - 0.5);

                return 0; // Kolor nie ma znaczenia, liczy siê g³êbia
            }
            ENDCG
        }
        }
}