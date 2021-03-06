void GetLightingInformation_float(float3 WPos, out float3 Direction, out float3 Color,out float Attenuation)//, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = float3(-0.5,0.5,-0.5);
        Color = float3(1,1,1);
        Attenuation = 0.4;
        //ShadowAtten = 1;
    #else
        float4 shadowCoord = TransformWorldToShadowCoord(WPos);

        Light light = GetMainLight(shadowCoord);
        Direction = light.direction;
        Attenuation = light.distanceAttenuation;
        Color = light.color;
        //#if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
        //    ShadowAtten = 1.0h;
        //#else
        //    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
        //    float shadowStrength = GetMainShadowStrength();
        //    ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
        //    sampler_MainLightShadowmapTexture),
        //    shadowSamplingData, shadowStrength, false);
        //#endif
    #endif
}