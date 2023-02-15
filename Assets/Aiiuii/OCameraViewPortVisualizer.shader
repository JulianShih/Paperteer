// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OCameraViewPortVisualizer" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _Size ("Size", Range(0.0001, 0.05)) = 0.01
        _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "gray" {}
    }

    SubShader{
        Pass {
            Tags {"LightMode" = "Always"}
            // Cull front
            CGPROGRAM
            
			#pragma vertex vert	
			#pragma fragment frag

            float4 _normals[4];
            float _Size;
            float4 _Color;
			sampler2D _MainTex;

			//頂點著色器輸入
			struct a2v
			{
				float4  position : POSITION;
				float3  normal: NORMAL;
				float2  texcoord : TEXCOORD0;	
 			};

			//頂點著色器輸出
			struct v2f
			{
				float4 position: SV_POSITION;
				float2 texcoord: TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;	
				o.position = UnityObjectToClipPos(v.position);									
				o.texcoord = v.texcoord;

				return o;
			}


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
            
			fixed4 frag(v2f IN) : SV_Target 
			{
                // float2 dist[4];

                // dist[0] = IN.texcoord - RadialCoords(_normals[0]);
                // dist[1] = IN.texcoord - RadialCoords(_normals[1]);
                // dist[2] = IN.texcoord - RadialCoords(_normals[2]);
                // dist[3] = IN.texcoord - RadialCoords(_normals[3]);
                
                // if (dist[0].x*dist[0].x + dist[0].y*dist[0].y <= _Size*_Size)
                //     return _Color;
                // else if (dist[1].x*dist[1].x + dist[1].y*dist[1].y <= _Size*_Size)
                //     return _Color;
                // else if (dist[2].x*dist[2].x + dist[2].y*dist[2].y <= _Size*_Size)
                //     return _Color;
                // else if (dist[3].x*dist[3].x + dist[3].y*dist[3].y <= _Size*_Size)
                //     return _Color;
                // else
                //     return tex2D(_MainTex, IN.texcoord);

                float2 corner[4];
                corner[0] = RadialCoords(_normals[0]);
                corner[1] = RadialCoords(_normals[1]);
                corner[2] = RadialCoords(_normals[2]);
                corner[3] = RadialCoords(_normals[3]);

                if(abs(IN.texcoord.x - corner[0].x) <= _Size)
                // if((IN.texcoord.x - corner[0].x) <= _Size)
                {
                    if(corner[0].y >= IN.texcoord.y && IN.texcoord.y >= corner[2].y)
                        return _Color;
                }

                if(abs(IN.texcoord.x - corner[1].x) <= _Size)
                {
                    if(corner[1].y >= IN.texcoord.y && IN.texcoord.y >= corner[3].y)
                        return _Color;
                }

                if(abs(IN.texcoord.y - corner[1].y) <= (_Size * 3840 / 1920))
                {
                    if(IN.texcoord.x <= corner[1].x && corner[0].x <= IN.texcoord.x)
                        return _Color;
                }

                if(abs(IN.texcoord.y - corner[3].y) <= (_Size * 3840 / 1920))
                {
                    if(corner[2].x <= IN.texcoord.x && IN.texcoord.x <= corner[3].x)
                        return _Color;
                }

                return tex2D(_MainTex, IN.texcoord);
			}            
            ENDCG
        }
    }
    FallBack "VertexLit"
}

