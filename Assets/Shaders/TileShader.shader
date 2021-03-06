Shader "Lexdev/CaseStudies/CivilizationMap"
{
    Properties
    {

    }
    SubShader
    {
        CGPROGRAM

        #pragma surface surf Standard

        struct Input
        {
            float worldPos; //will let us get the world position of the vertex
        };

    float _MapSize;
    sampler2D _Mask;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = tex2D(_Mask, IN.worldPos.xz / _MapSize).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
