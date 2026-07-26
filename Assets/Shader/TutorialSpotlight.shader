Shader "UI/TutorialSpotlight"
{
    Properties
    {
        _Color ("Overlay Color", Color) =
        (0,0,0,0.75)

        _Feather ("Feather", Float) =
        0.02

        _Radius ("Corner Radius", Range(0,0.2)) = 0.03

        _Hole1 ("Hole1", Vector) =
        (-1,-1,0,0)

        _Hole2 ("Hole2", Vector) =
        (-1,-1,0,0)

        _Hole3 ("Hole3", Vector) =
        (-1,-1,0,0)

        _Hole4 ("Hole4", Vector) =
        (-1,-1,0,0)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;

            float4 _Hole1;
            float4 _Hole2;
            float4 _Hole3;
            float4 _Hole4;

            float _Feather;
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex =
                    UnityObjectToClipPos(
                        v.vertex
                    );

                o.uv = v.uv;

                return o;
            }

            float RectMask(
                float2 uv,
                float4 hole,
                float feather
            )
            {
                if (
                    hole.z <= 0 ||
                    hole.w <= 0
                )
                {
                    return 0;
                }

                float radius = _Radius;

                float2 halfSize =
                    hole.zw * 0.5 - radius;

                float2 p =
                    abs(uv - hole.xy) -
                    halfSize;

                float dist =
                    length(max(p, 0.0)) +
                    min(max(p.x, p.y), 0.0) -
                    radius;

                return 1.0 -
                    smoothstep(
                        0,
                        feather,
                        dist
                    );
            }

            // fixed4 frag(v2f i)
            //     : SV_Target
            // {
            //     float cutout = 0;

            //     cutout =
            //         max(
            //             cutout,
            //             RectMask(
            //                 i.uv,
            //                 _Hole1,
            //                 _Feather
            //             )
            //         );

            //     cutout =
            //         max(
            //             cutout,
            //             RectMask(
            //                 i.uv,
            //                 _Hole2,
            //                 _Feather
            //             )
            //         );

            //     cutout =
            //         max(
            //             cutout,
            //             RectMask(
            //                 i.uv,
            //                 _Hole3,
            //                 _Feather
            //             )
            //         );

            //     cutout =
            //         max(
            //             cutout,
            //             RectMask(
            //                 i.uv,
            //                 _Hole4,
            //                 _Feather
            //             )
            //         );

            //     fixed4 col =
            //         _Color;

            //     col.a *=
            //         1 - cutout;

            //     return col;
            // }

            fixed4 frag(v2f i) : SV_Target
            {
                float cutout = 0;

                cutout = max(
                    cutout,
                    RectMask(
                        i.uv,
                        _Hole1,
                        _Feather
                    )
                );

                cutout = max(
                    cutout,
                    RectMask(
                        i.uv,
                        _Hole2,
                        _Feather
                    )
                );

                cutout = max(
                    cutout,
                    RectMask(
                        i.uv,
                        _Hole3,
                        _Feather
                    )
                );

                cutout = max(
                    cutout,
                    RectMask(
                        i.uv,
                        _Hole4,
                        _Feather
                    )
                );

                fixed4 col = _Color;

                col.a *= 1.0 - saturate(cutout);

                return col;
            }
            ENDCG
        }
    }
}