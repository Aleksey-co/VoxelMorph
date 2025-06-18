Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-100" } // Рисуем РАНЬШЕ остальных
        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            ColorMask 0 // Не выводим пиксели вообще
            ZWrite Off // Можно выключить, если не важно
        }
    }
}
