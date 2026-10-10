Shader "Unlit/OutlineShaderBakeNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("Outline Width", Range(0, 20)) = 1
    }
    SubShader
    {
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Unlit" }

HLSLINCLUDE
        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent:TANGENT;
            float2 uv : TEXCOORD0;
            float3 bakedNormal:TEXCOORD7;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        float3 OctahedronToUnitVector(float2 Oct)
        {
            float3 N = float3(Oct, 1 - dot(1, abs(Oct)));
            if (N.z < 0)
            {
                N.xy = (1 - abs(N.yx)) * (N.xy >= 0 ? float2(1, 1) : float2(-1, -1));
            }
            return normalize(N);
        }
        float3 TransformTBN(float2 bakedNormal, float3x3 tbn)
        {
            float3 normal = OctahedronToUnitVector(bakedNormal);
            return  (mul(normal, tbn));
        }
ENDHLSL

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            Cull Front
            ZTest On

            HLSLPROGRAM
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _OUTLINE_USE_NORMAL _OUTLINE_USE_BAKENORMAL

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;

                float3 objPos = v.vertex;
                float4 clipPos = TransformObjectToHClip(objPos);
                float outlineWidth = _OutlineWidth * 0.001 * clipPos.w;

                #if defined(_OUTLINE_USE_NORMAL)
                    objPos += normalize(v.normal) * outlineWidth;
                #endif

                #if defined(_OUTLINE_USE_BAKENORMAL)
                    float3 normalOS = normalize(v.normal);
                    float3 tangentOS = normalize(v.tangent);
                    float3 bitangentOS = normalize(cross(normalOS, tangentOS) * v.tangent.w);
                    float3x3 tbn = float3x3(tangentOS, bitangentOS, normalOS);
                    float3 bakedNormalDir = (TransformTBN(v.bakedNormal, tbn));
                    objPos += (bakedNormalDir * outlineWidth);
                #endif

                o.vertex = TransformObjectToHClip(objPos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return half4(0,0,0,1);
            }

            ENDHLSL
        }

        Pass
        {
            Name "Default"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                return col;
            }

            ENDHLSL
        }

    }
}
