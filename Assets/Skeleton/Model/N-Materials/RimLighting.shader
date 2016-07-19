Shader "RimLighting"
{
	Properties 
	{
_Maintex("_Maintex", 2D) = "black" {}
_Normal("_Normal", 2D) = "black" {}
_SpecularColor("_SpecularColor", Color) = (0.3006994,1,0,1)
_Spec("_Spec", 2D) = "black" {}
_Glossiness("_Glossiness", Range(0.1,1) ) = 0.4300518
_Alpha("_Alpha", Range(0,1) ) = 0.5
_Emission("_Emission", 2D) = "black" {}
_EmissionColor("_EmissionColor", Color) = (0,0.1188812,1,1)
_EmissionPower("_EmissionPower", Range(0.1,3) ) = 1.707772

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Transparent"
"IgnoreProjector"="False"
"RenderType"="Transparent"

		}

		
Cull Off
ZWrite On
ZTest LEqual
ColorMask RGBA
Blend SrcAlpha OneMinusSrcAlpha
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 3.0


sampler2D _Maintex;
sampler2D _Normal;
float4 _SpecularColor;
sampler2D _Spec;
float _Glossiness;
float _Alpha;
sampler2D _Emission;
float4 _EmissionColor;
float _EmissionPower;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_Maintex;
float2 uv_Normal;
float2 uv_Emission;
float2 uv_Spec;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Tex2D0=tex2D(_Maintex,(IN.uv_Maintex.xyxy).xy);
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_Normal,(IN.uv_Normal.xyxy).xy)).xyz, 1.0 );
float4 Tex2D1=tex2D(_Emission,(IN.uv_Emission.xyxy).xy);
float4 Pow0=pow(Tex2D1,_EmissionPower.xxxx);
float4 Multiply0=_EmissionColor * Pow0;
float4 Tex2D2=tex2D(_Spec,(IN.uv_Spec.xyxy).xy);
float4 Multiply2=_SpecularColor * Tex2D2;
float4 Multiply1=Tex2D0.aaaa * _Alpha.xxxx;
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Tex2D0;
o.Normal = Tex2DNormal0;
o.Emission = Multiply0;
o.Specular = _Glossiness.xxxx;
o.Gloss = Multiply2;
o.Alpha = Multiply1;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}