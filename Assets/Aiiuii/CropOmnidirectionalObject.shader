Shader "Custom/CropOmnidirectionalObject"
{
    Properties {
        // _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "gray" {}
    }

    SubShader{
        Pass {
            Tags {"LightMode" = "Always"}

            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma glsl
                #pragma target 3.0

                #include "UnityCG.cginc"

                struct appdata {
                   float4 vertex : POSITION;
                };

                struct v2f
                {
                    float4    pos : SV_POSITION;
                    float3    normal : TEXCOORD0;
                    float4    vertex : TEXCOORD1;
                };

                v2f vert (appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.vertex = v.vertex;
                    return o;
                }

                float4 _normals[4];
                sampler2D _MainTex;
                float4 _scaleAndOffset; 
                
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
                    float2 uv = float2(IN.vertex.x, 1 - IN.vertex.y);
                    // float2 uv = IN.vertex.xy;
                    // uv = uv * _scaleAndOffset.xy + _scaleAndOffset.zw;
                    // float3 normal = _normals[0] + uv.x * (_normals[1] - _normals[0]) + uv.y * (_normals[2] - _normals[0]);
                    float3 normal = (1.0 - uv.x) * (1.0 - uv.y) * _normals[0] + 
                                    uv.x * (1.0 - uv.y) * _normals[1] + 
                                    (1.0 - uv.x) * uv.y * _normals[2] + 
                                    uv.x * uv.y * _normals[3];
                    float2 equiUV = RadialCoords(normal);
                    float2 scaledUV = equiUV * _scaleAndOffset.xy + _scaleAndOffset.zw;
                    // return tex2D(_MainTex, scaledUV);
                    return tex2D(_MainTex, equiUV);
                }
            ENDCG
        }
    }
}
