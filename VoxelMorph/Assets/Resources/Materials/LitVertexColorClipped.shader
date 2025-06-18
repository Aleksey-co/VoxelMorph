Shader "Custom/LitVertexColorClipped"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MaskMin ("Mask Min", Vector) = (0,0,0,0)
        _MaskMax ("Mask Max", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+10" }

        Stencil
        {
            Ref 1
            Comp Equal
            Pass Keep
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert
        #include "UnityCG.cginc"

        float _Glossiness;
        float _Metallic;
        float4 _MaskMin;
        float4 _MaskMax;

        struct Input
        {
            float4 color : COLOR;
            float3 worldPos;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.color = v.color;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            if (any(IN.worldPos < _MaskMin.xyz) || any(IN.worldPos > _MaskMax.xyz))
                clip(-1); // обрезаем пиксель

            o.Albedo = IN.color.rgb;
            o.Smoothness = _Glossiness;
            o.Metallic = _Metallic;
        }
        ENDCG
    }
}
