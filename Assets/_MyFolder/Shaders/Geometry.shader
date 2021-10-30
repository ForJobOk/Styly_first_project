Shader "Custom/Geometry"
{
    Properties
    {
        [HDR]_MainColor("MainColor",Color) =(0,0,0,0)
        _MoveFactor ("Move Factor", Range(0,1.0)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        
        //両面描画
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _MoveFactor;
            float4 _MainColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 localPos : TEXCOORD0;
            };

            //頂点シェーダー
            appdata vert(appdata v)
            {
                appdata o;
                o.localPos = v.vertex.xyz; //ジオメトリーシェーダーで頂点を動かす前に"描画しようとしているピクセル"のローカル座標を保持しておく
                return v;
            }

            struct g2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            //回転させる
            //pは回転させたい座標　angleは回転させる角度　axisはどの軸を元に回転させるか　
            float3 rotate(float3 p, float angle, float3 axis)
            {
                float3 a = normalize(axis);
                float s = sin(angle);
                float c = cos(angle);
                float r = 1.0 - c;
                float3x3 m = float3x3(
                    a.x * a.x * r + c, a.y * a.x * r + a.z * s, a.z * a.x * r - a.y * s,
                    a.x * a.y * r - a.z * s, a.y * a.y * r + c, a.z * a.y * r + a.x * s,
                    a.x * a.z * r + a.y * s, a.y * a.z * r - a.x * s, a.z * a.z * r + c
                );

                return mul(m, p);
            }

            //ランダムな値を返す
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            // ジオメトリシェーダー
            [maxvertexcount(3)]
            void geom(triangle appdata input[3], uint pid : SV_PrimitiveID,inout LineStream<g2f> stream)
            {
                // 法線を計算
                float3 vec1 = input[1].vertex - input[0].vertex;
                float3 vec2 = input[2].vertex - input[0].vertex;
                float3 normal = normalize(cross(vec1, vec2));

                //1枚のポリゴンの中心
                float3 center = (input[0].vertex + input[1].vertex + input[2].vertex) / 3;
                float random = 2.0 * rand(center.xy) - 0.5;
                float3 r3 = random.xxx;
              
                [unroll]
                for (int i = 0; i < 3; i++)
                {
                    appdata v = input[i];
                    g2f o;

                    //こっちの方がすっきり　順番を変えただけ
                    v.vertex.xyz = center + (v.vertex.xyz - center) * _MoveFactor;
                    v.vertex.xyz += normal * (1.0-_MoveFactor) * abs(r3);
                    
                    
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    
                    o.color = fixed4(_MainColor.rgb,1);
                    stream.Append(o);
                }
            }

            //フラグメントシェーダー
            fixed4 frag(g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}