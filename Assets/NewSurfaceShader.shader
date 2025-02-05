Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM

            // 頂点の操作
            #pragma vertex vert

            // 画素の色塗り
            #pragma fragment frag

            #include "UnityCG.cginc"

            // vert関数で使用する構造体
            struct appdata
            {
                float4 vertex : POSITION;
            };

            // vert関数、frag関数で使用する構造体
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            // Propertiesで定義した変数をCGPROGRAM内で定義
            fixed4 Color;

            // 頂点の操作する関数
            v2f vert(appdata v)
            {
                v2f o;

                // 拡大率を決める
                float amp = 1.0 + 15.0 * sin(_Time.x * 20.0);

                // 拡大率を座標に掛ける
                o.vertex = UnityObjectToClipPos(v.vertex * amp);

                return o;
            }

            // フラグメントシェーダー
            fixed4 frag(v2f i) : SV_TARGET
            {
                return Color;
            }

            ENDCG
        }
    }
}