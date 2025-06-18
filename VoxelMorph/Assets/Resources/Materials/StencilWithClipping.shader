Shader "Custom/StencilWithClipping"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MaskMin ("Mask Min", Vector) = (0, 0, 0, 0)
        _MaskMax ("Mask Max", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+10" }
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _MaskMin;
            float4 _MaskMax;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (any(i.worldPos < _MaskMin.xyz) || any(i.worldPos > _MaskMax.xyz))
                    clip(-1); // отбрасываем пиксель

                return _Color;
            }
            ENDCG
        }
    }
}
