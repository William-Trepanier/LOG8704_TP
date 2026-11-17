// Shader d'un overlay qui couvre tout l'écran. Ça donne l'impression d'être éblouie par la lumière.

Shader "Unlit/FlashOverlay"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0, 4)) = 0
        _FlashDir ("Flash Direction", Vector) = (0, 0, 1, 0)
        _EdgeFactor ("Edge Factor", Range(0, 1)) = 0.35
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend One One
        ZWrite Off ZTest Always Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct v2_f
            {
                float4 pos : SV_POSITION; UNITY_VERTEX_OUTPUT_STEREO
                float3 world_pos : TEXCOORD0;
            };

            float4 _Color;
            float  _Intensity;
            float4 _FlashDir;
            float _EdgeFactor;

            v2_f vert(appdata v)
            {
                v2_f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.world_pos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2_f i) : SV_Target
            {
                float3 view_dir = normalize(i.world_pos - _WorldSpaceCameraPos);
                float  facing  = saturate(dot(view_dir, normalize(_FlashDir.xyz)));   // 1 on regarde le flash et ça diminue si on tourne la tête
                float  w = lerp(_EdgeFactor, 1.0, smoothstep(0.0, 1.0, facing));
                return fixed4(_Color.rgb * _Intensity * w, 1);
            }
            ENDCG
        }
    }
}
