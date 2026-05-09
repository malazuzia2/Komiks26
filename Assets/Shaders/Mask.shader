Shader "Custom/URP_DepthMask"
{
    SubShader
    {
        // Rysujemy maskê PO ³ódce (2000), ale PRZED wod¹ (3000)
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+500" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "DepthMask"
            Tags { "LightMode" = "UniversalForward" }

            ColorMask 0 // B¹dŸ w 100% niewidzialny
            ZWrite On   // Ale blokuj to, co jest z ty³u

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(0,0,0,0);
            }
            ENDHLSL
        }
    }
}