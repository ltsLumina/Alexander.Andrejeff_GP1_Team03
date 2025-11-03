Shader "Custom/ColorBlindlessShader"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0, 1)) = 1.0

        _R ("R Row", Color) = (1, 0, 0, 0)
        _G ("G Row", Color) = (0, 1, 0, 0)
        _B ("B Row", Color) = (0, 0, 1, 0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            sampler2D _CameraOpaqueTexture;
            
            fixed4 _Color;
            half _Opacity;
            fixed4 _R;
            fixed4 _G;
            fixed4 _B;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float4 screenPos : TEXCOORD1; 
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                o.screenPos = ComputeScreenPos(o.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                fixed4 col = tex2D(_CameraOpaqueTexture, screenUV);

                fixed r_new = dot(col.rgb, _R.rgb);
                fixed g_new = dot(col.rgb, _G.rgb);
                fixed b_new = dot(col.rgb, _B.rgb);

                fixed4 finalColor = fixed4(r_new, g_new, b_new, col.a);

                finalColor *= _Color;
                finalColor.a *= _Opacity;
                
                return finalColor;
            }
            ENDCG
        }
    }
}