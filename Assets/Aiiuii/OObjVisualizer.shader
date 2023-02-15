// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OObjVisualizer" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Size ("Size", Range(0.0, 0.1)) = 0.02
        _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "gray" {}
    }

    SubShader{
        Pass {
            Tags {"LightMode" = "Always"}
            // Cull front
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma glsl
                #pragma target 3.0

                #include "UnityCG.cginc"

                struct appdata {
                   float4 vertex : POSITION;
                   float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4    pos : SV_POSITION;
                    float3    normal : TEXCOORD0;
                };

                v2f vert (appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.normal = v.normal;
                    return o;
                }

                float4 _normals[4];
                sampler2D _MainTex;
                float4 _scaleAndOffset; 
                float _Size;
                float4 _Color;

                #define PI 3.141592653589793

                inline float2 RadialCoords(float3 a_coords)
                {
                    float3 a_coords_n = normalize(a_coords);
                    float lon = atan2(a_coords_n.z, a_coords_n.x);
                    float lat = acos(a_coords_n.y);
                    float2 sphereCoords = float2(lon, lat) * (1.0 / PI);
                    // return float2(sphereCoords.x * 0.5 + 0.5, 1 - sphereCoords.y);
                    // return float2(sphereCoords.x * 0.5 + 0.5, sphereCoords.y);
                    return float2(1 - (sphereCoords.x * 0.5 + 0.5), 1 - sphereCoords.y);
                }

                float4 frag(v2f IN) : COLOR
                {
                    float2 equiUV = RadialCoords(IN.normal);
                    float2 dist[4];

                    dist[0] = equiUV - RadialCoords(_normals[0]);
                    dist[1] = equiUV - RadialCoords(_normals[1]);
                    dist[2] = equiUV - RadialCoords(_normals[2]);
                    dist[3] = equiUV - RadialCoords(_normals[3]);
                    
                    if (dist[0].x*dist[0].x + dist[0].y*dist[0].y <= _Size*_Size)
                        return _Color;
                    else if (dist[1].x*dist[1].x + dist[1].y*dist[1].y <= _Size*_Size)
                        return _Color;
                    else if (dist[2].x*dist[2].x + dist[2].y*dist[2].y <= _Size*_Size)
                        return _Color;
                    else if (dist[3].x*dist[3].x + dist[3].y*dist[3].y <= _Size*_Size)
                        return _Color;
                    else
                        return tex2D(_MainTex, equiUV);
                }
            ENDCG
        }
    }
    FallBack "VertexLit"
}