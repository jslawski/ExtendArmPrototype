Shader "Custom/worldSpace"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("Normalmap",2D) = "Normal" {}
        _OrmMap("OrmMap",2D) = "OrmMap" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _UVs("UV Scale", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _OrmMap;
        float _UVs;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float2 uv_OrmMap;
            float3 worldPos;
            float3 worldNormal;
            INTERNAL_DATA
        };

        //half _Glossiness;
        half _Metallic;
        //half _Occlusion;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float3 worldSpaceColorBlend(float3 worldPos,float3 worldNormal,sampler2D textureOrg)
        {
            float3 Pos = worldPos;

            float3 tex1 = tex2D(textureOrg, Pos.yz).rgb;
            float3 tex2 = tex2D(textureOrg, Pos.xz).rgb;
            float3 tex3 = tex2D(textureOrg, Pos.xy).rgb;

            float alpha21 = abs(worldNormal.x);
            float alpha23 = abs(worldNormal.z);
        
            float3 tex21 = lerp(tex2, tex1, alpha21).rgb;
            float3 tex23 = lerp(tex21, tex3, alpha23).rgb;

            return tex23;
        }

        float3 worldSpaceNormalBlend(float3 worldPos,float3 worldNormal,sampler2D textureOrg)
        {
            float3 Pos = worldPos;

            float3 N00 = UnpackNormal(tex2D(textureOrg,Pos / 10));
            N00.y = -N00.y;

            float3 N1 = UnpackNormal(tex2D(textureOrg,Pos.yz));
            float3 N2 = UnpackNormal(tex2D(textureOrg,Pos.xz));
            float3 N3 = UnpackNormal(tex2D(textureOrg,Pos.xy));

            float alpha21 = abs(worldNormal.x);
            float alpha23 = abs(worldNormal.z);
        
            float3 N21 = lerp(N2, N1, alpha21).rgb;
            float3 N23 = lerp(N21, N3, alpha23).rgb;

            return N23;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float3 worldNormal = WorldNormalVector(IN,o.Normal);
            
            float3 Pos = IN.worldPos / (-1.0 * abs(_UVs));

            float3 c23 = worldSpaceColorBlend(Pos,worldNormal,_MainTex);
            //---- Base Color Adjustment -----

            float3 N23 = worldSpaceNormalBlend(Pos,worldNormal,_NormalMap);
            //---- Normal Adjustment -----

            float3 Orm23 =  worldSpaceColorBlend(Pos, worldNormal, _OrmMap);
            

            fixed3 c = c23 * _Color;
            o.Albedo = c23;
            o.Normal = N23;
            o.Metallic = _Metallic;
            o.Smoothness = Orm23.b;
            o.Occlusion = Orm23.r;
          
        }
        ENDCG
    }
    FallBack "Diffuse"
}
