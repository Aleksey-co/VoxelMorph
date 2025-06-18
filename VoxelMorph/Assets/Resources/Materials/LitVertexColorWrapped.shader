Shader "Custom/LitVertexColorWrapped"
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
        #pragma surface surf Standard fullforwardshadows vertex:vert
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

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float3 size = _MaskMax.xyz - _MaskMin.xyz;
            float3 pos = IN.worldPos;

            bool outsideX = (pos.x < _MaskMin.x || pos.x > _MaskMax.x);
            bool outsideY = (pos.y < _MaskMin.y || pos.y > _MaskMax.y);
            bool outsideZ = (pos.z < _MaskMin.z || pos.z > _MaskMax.z);

            int outsideCount = (int)outsideX + (int)outsideY + (int)outsideZ;

            // Обрезаем, если объект выходит сразу по двум или более осям
            if (outsideCount > 1)
                clip(-1);

            // Выполняем wrap по одной оси
            float3 wrapped = pos;

            if (outsideX)
                wrapped.x = (pos.x < _MaskMin.x) ? pos.x + size.x : pos.x - size.x;
            else if (outsideY)
                wrapped.y = (pos.y < _MaskMin.y) ? pos.y + size.y : pos.y - size.y;
            else if (outsideZ)
                wrapped.z = (pos.z < _MaskMin.z) ? pos.z + size.z : pos.z - size.z;

            // Отбрасываем пиксель, если он всё ещё за пределами (защита от ошибок)
            if (any(wrapped < _MaskMin.xyz) || any(wrapped > _MaskMax.xyz))
                clip(-1);

            o.Albedo = IN.color.rgb;
            o.Smoothness = _Glossiness;
            o.Metallic = _Metallic;
        }
        ENDCG
    }
}
