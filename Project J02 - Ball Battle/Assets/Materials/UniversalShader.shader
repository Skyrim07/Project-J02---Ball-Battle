Shader "SKCell/ModelProcessing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Saturation("Saturation", Range(0,1)) = 1
        [HDR]_Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        //Pass
        //{
        //    Cull Front
        //    CGPROGRAM
        //    #pragma vertex vert
        //    #pragma fragment frag

        //    #include "UnityCG.cginc"
        //    #include "Lighting.cginc"

        //    struct appdata
        //    {
        //        float4 vertex : POSITION;
        //        float2 uv : TEXCOORD0;
        //        float3 normal : NORMAL;
        //    };

        //    struct v2f
        //    {
        //        float2 uv : TEXCOORD0;
        //        float4 vertex : SV_POSITION;
        //        float3 worldNormal : TEXCOORD1;
        //        float3 worldLight : TEXCOORD2;
        //    };

        //    sampler2D _MainTex;
        //    float4 _MainTex_ST;

        //    float _Saturation;
        //    fixed4 _Color, _RimColor;
        //    float _RimWidth;

        //    v2f vert(appdata v)
        //    {
        //        v2f o;
        //        v.vertex.xyz += _RimWidth * v.normal;
        //        o.vertex = UnityObjectToClipPos(v.vertex);
        //        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        //        return o;
        //    }

        //    fixed4 frag(v2f i) : SV_Target
        //    {
        //        return _RimColor;
        //    }
        //    ENDCG
        //}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
                float3 worldLight : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Saturation, _Brightness;
            fixed4 _Color, _RimColor;
            float _RimWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = mul(unity_ObjectToWorld, v.normal);
                o.worldLight = normalize(_WorldSpaceLightPos0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col.rgb *= _Brightness;
                float ndl = max(0, dot(i.worldNormal, i.worldLight))*0.5+0.5;
                col.rgb *= ndl * _LightColor0;

                float l = Luminance(col);
                col = lerp(col, fixed4(l, l, l, col.a), 1-_Saturation);
                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
