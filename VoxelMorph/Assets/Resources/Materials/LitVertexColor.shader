Shader "Custom/LitVertexColor"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #include "UnityCG.cginc"

        struct Input
        {
            float4 color : COLOR;
        };

        float _Glossiness;
        float _Metallic;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.color.rgb; // Цвет вершин
            o.Smoothness = _Glossiness; // Гладкость
            o.Metallic = _Metallic; // Металл
        }
        ENDCG
    }
}
