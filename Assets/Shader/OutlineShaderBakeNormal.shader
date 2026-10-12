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
            float2 bakedNormal2 : TEXCOORD6;
            float3 bakedNormal3 : TEXCOORD7;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };

        // 八面体编码压缩（Octahedron Encoding）
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
            Tags { "LightMode" = "OutlineLightMode" }
            Cull Front
            ZTest On

            HLSLPROGRAM
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _OUTLINE_NORMAL _OUTLINE_BAKE2 _OUTLINE_BAKE3

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;

                float3 objPos = v.vertex;
                float4 clipPos = TransformObjectToHClip(objPos);

                // 防止光栅化透视除法(/w)导致的outline宽度变化，逆向乘以w以抵消透视除法的影响
                float outlineWidth = _OutlineWidth * 0.001 * clipPos.w;

                #if defined(_OUTLINE_NORMAL)
                    objPos += normalize(v.normal) * outlineWidth;
                #endif

                #if defined(_OUTLINE_BAKE2)
                    // 8面体压缩法线(2D)
                    float3 normalOS = normalize(v.normal);
                    float3 tangentOS = normalize(v.tangent);
                    float3 bitangentOS = normalize(cross(normalOS, tangentOS) * v.tangent.w);
                    float3x3 tbn = float3x3(tangentOS, bitangentOS, normalOS);
                    // v.bakedNormal 是基于切线空间，需要转换回模型空间
                    float3 bakedNormalDir = (TransformTBN(v.bakedNormal2, tbn));
                    objPos += (bakedNormalDir * outlineWidth);
                #endif

                #if defined(_OUTLINE_BAKE3)
                    // 直接用3D法线
                    objPos += (v.bakedNormal3 * outlineWidth);
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
