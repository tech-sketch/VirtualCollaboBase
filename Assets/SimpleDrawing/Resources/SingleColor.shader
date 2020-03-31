Shader "SimpleDrawing/SingleColor"
{
    Properties
    {
        _SingleColor ("Main Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // vertex shader
            // this time instead of using "appdata" struct, just spell inputs manually,
            // and instead of returning v2f struct, also just return a single output
            // float4 clip position
            float4 vert (float4 vertex : POSITION) : SV_POSITION
            {
                return UnityObjectToClipPos(vertex);
            }
            
            // color from the material
            fixed4 _SingleColor;

            // pixel shader, no inputs needed
            fixed4 frag () : SV_Target
            {
                return _SingleColor; // just return it
            }
            ENDCG
        }
    }
}
