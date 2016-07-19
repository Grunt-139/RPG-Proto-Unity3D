Shader "Custom/BulkShader" 
{
  Properties 
  {
    _MainTex ("Main Texture", 2D) = "white" {}
    _DecalTex("Decal Texture",2D) = "black" {}
    _BumpMap ("Bumpmap", 2D) = "bump" {}
    _Amount ("Extrusion Amount", Range(-0.2,0.2)) = 0
    _Colour ("Main Color", Color) = (0.5,0.5,0.5)
  }

  SubShader 
  {  
    Tags { "RenderType" = "Opaque" }
    LOD 200
    CGPROGRAM
    #pragma surface surf Lambert vertex:vert
    
    struct Input 
    {
      float2 uv_MainTex;
      float2 uv_DecalTex;
      float2 uv_BumpMap;
    };
    
    float _Amount;
    void vert (inout appdata_full v) 
    {
      v.vertex.xyz += v.normal * _Amount;
    }
      
    sampler2D _MainTex;
    sampler2D _BumpMap;
    sampler2D _DecalTex;
    float3 _Colour;
    void surf (Input IN, inout SurfaceOutput o) 
    {
      o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
      o.Albedo *= tex2D (_DecalTex, IN.uv_DecalTex).rgb *2;
      o.Albedo *= _Colour.rgb;
      
      o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
    }
    ENDCG
  } 
  Fallback "Diffuse"
}
