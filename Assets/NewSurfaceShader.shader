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

            // ���_�̑���
            #pragma vertex vert

            // ��f�̐F�h��
            #pragma fragment frag

            #include "UnityCG.cginc"

            // vert�֐��Ŏg�p����\����
            struct appdata
            {
                float4 vertex : POSITION;
            };

            // vert�֐��Afrag�֐��Ŏg�p����\����
            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            // Properties�Œ�`�����ϐ���CGPROGRAM���Œ�`
            fixed4 Color;

            // ���_�̑��삷��֐�
            v2f vert(appdata v)
            {
                v2f o;

                // �g�嗦�����߂�
                float amp = 1.0 + 15.0 * sin(_Time.x * 20.0);

                // �g�嗦�����W�Ɋ|����
                o.vertex = UnityObjectToClipPos(v.vertex * amp);

                return o;
            }

            // �t���O�����g�V�F�[�_�[
            fixed4 frag(v2f i) : SV_TARGET
            {
                return Color;
            }

            ENDCG
        }
    }
}