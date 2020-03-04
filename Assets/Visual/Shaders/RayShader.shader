// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/RayShader"
{
	Properties
	{
		_AlbedoIntensity("AlbedoIntensity", Float) = 2
		_EmissiveIntensity("EmissiveIntensity", Float) = 1
		_Frequency("Frequency", Range( -40 , 40)) = -0.5
		_Texture("Texture", 2D) = "white" {}
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_ContrastIntensity("ContrastIntensity", Float) = 2.1
		_OpacityFalloff("OpacityFalloff", Float) = 4
		_Color("Color", Color) = (1,0.7803586,0,0)
		_OpacityStart("OpacityStart", Float) = 0
		_FresnelBias1("FresnelBias", Range( -2 , 2)) = 0
		_FresnelScale1("FresnelScale", Range( 0 , 2)) = 0.99
		_FresnelPower1("FresnelPower", Range( 0 , 50)) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform float _AlbedoIntensity;
		uniform float4 _Color;
		uniform sampler2D _Texture;
		uniform float _Frequency;
		uniform float _ContrastIntensity;
		uniform float _EmissiveIntensity;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _FresnelBias1;
		uniform float _FresnelScale1;
		uniform float _FresnelPower1;
		uniform float _OpacityStart;
		uniform float _OpacityFalloff;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float4 Color160 = _Color;
			float2 temp_cast_0 = (( _Time.y * _Frequency )).xx;
			float2 uv_TexCoord123 = i.uv_texcoord + temp_cast_0;
			float2 temp_cast_1 = (uv_TexCoord123.x).xx;
			float4 tex2DNode118 = tex2D( _Texture, temp_cast_1 );
			o.Albedo = CalculateContrast(_AlbedoIntensity,( Color160 * tex2DNode118 )).rgb;
			o.Emission = ( CalculateContrast(_ContrastIntensity,( tex2DNode118 * _EmissiveIntensity )) * Color160 ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV155 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode155 = ( _FresnelBias1 + _FresnelScale1 * pow( 1.0 - fresnelNdotV155, _FresnelPower1 ) );
			o.Translucency = ( fresnelNode155 * Color160 ).rgb;
			float temp_output_78_0 = ( 1.0 - i.uv_texcoord.y );
			float temp_output_145_0 = ( i.uv_texcoord.y * temp_output_78_0 * 2.0 );
			o.Alpha = (_OpacityStart + (temp_output_145_0 - 0.0) * (_OpacityFalloff - _OpacityStart) / (1.0 - 0.0));
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom alpha:fade keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
454;152;1899;805;1526.785;275.0161;1.365262;True;True
Node;AmplifyShaderEditor.CommentaryNode;132;-520.42,-499.1591;Inherit;False;1793.153;545.3281;;11;149;161;139;131;140;130;118;123;143;121;142;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-515.36,-307.0224;Inherit;False;Property;_Frequency;Frequency;3;0;Create;True;0;0;False;0;-0.5;-0.5;-40;40;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;121;-487.7527,-449.2608;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-226.5482,-378.7699;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;101;-543.4786,744.116;Inherit;False;1932.931;423.6885;Y axe, can switch gradient start with GradientSide;8;81;147;145;92;78;77;80;82;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;-53.30836,-440.5598;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;158;-667.9412,-1091.268;Inherit;False;Property;_Color;Color;15;0;Create;True;0;0;False;0;1,0.7803586,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;151;-57.91503,87.20209;Inherit;False;871.549;532.403;Translucency;7;157;156;155;154;153;152;162;FresnelEffect;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;130;300.7782,-245.1626;Inherit;False;Property;_EmissiveIntensity;EmissiveIntensity;2;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-384.589,-1081.653;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-493.4787,794.116;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;144;365.1438,-957.8981;Inherit;False;1056.598;370.8063;Comment;5;56;134;138;137;159;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;118;258.6506,-449.159;Inherit;True;Property;_Texture;Texture;4;0;Create;True;0;0;False;0;-1;9fbef4b79ca3b784ba023cb1331520d5;9fbef4b79ca3b784ba023cb1331520d5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;159;389.218,-900.0136;Inherit;False;160;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;620.5887,-435.1561;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;78;-188.6312,801.7302;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-29.10399,212.8267;Inherit;False;Property;_FresnelScale1;FresnelScale;19;0;Create;True;0;0;False;0;0.99;1.58;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;153;-26.47899,291.5198;Inherit;False;Property;_FresnelPower1;FresnelPower;20;0;Create;True;0;0;False;0;5;4.74;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-288.0338,1055.002;Inherit;False;Constant;_GradientIntensity;Gradient Intensity;14;0;Create;True;0;0;False;0;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;154;-28.68505,133.8268;Inherit;False;Property;_FresnelBias1;FresnelBias;18;0;Create;True;0;0;False;0;0;1.58;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;647.7934,-201.9567;Inherit;False;Property;_ContrastIntensity;ContrastIntensity;13;0;Create;True;0;0;False;0;2.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;155;305.3832,142.8677;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;775.6509,1054.791;Inherit;False;Property;_OpacityFalloff;OpacityFalloff;14;0;Create;True;0;0;False;0;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;391.781,408.1126;Inherit;False;160;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;15.96632,877.0018;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;139;935.0487,-386.7238;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;3.15;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;980.439,-159.7367;Inherit;False;160;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;682.6472,-876.215;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;138;907.7827,-702.0917;Inherit;False;Property;_AlbedoIntensity;AlbedoIntensity;1;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;776.1658,964.213;Inherit;False;Property;_OpacityStart;OpacityStart;16;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;80;1034.4,877.1113;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;137;1154.742,-886.3851;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;3.15;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;644.9828,328.0892;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;56;388.1438,-796.8981;Inherit;False;Property;_AlbedoColor;AlbedoColor;0;0;Create;True;0;0;False;0;1,0.7611992,0.4103774,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;149;750.7824,-126.1608;Inherit;False;Property;_EmissiveColor;EmissiveColor;12;0;Create;True;0;0;False;0;0.8867924,0.8432013,0.4726771,0;0.8867924,0.8432013,0.4726771,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;156;117.0281,405.5862;Inherit;False;Property;_FresnelColor1;FresnelColor;17;0;Create;True;0;0;False;0;1,0.937908,0.2392157,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;1330.044,-384.5566;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;92;457.162,804.2794;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1747.117,-258.7155;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/RayShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;5;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;143;0;121;0
WireConnection;143;1;142;0
WireConnection;123;1;143;0
WireConnection;160;0;158;0
WireConnection;118;1;123;1
WireConnection;131;0;118;0
WireConnection;131;1;130;0
WireConnection;78;0;77;2
WireConnection;155;1;154;0
WireConnection;155;2;152;0
WireConnection;155;3;153;0
WireConnection;145;0;77;2
WireConnection;145;1;78;0
WireConnection;145;2;147;0
WireConnection;139;1;131;0
WireConnection;139;0;140;0
WireConnection;134;0;159;0
WireConnection;134;1;118;0
WireConnection;80;0;145;0
WireConnection;80;3;81;0
WireConnection;80;4;82;0
WireConnection;137;1;134;0
WireConnection;137;0;138;0
WireConnection;157;0;155;0
WireConnection;157;1;162;0
WireConnection;150;0;139;0
WireConnection;150;1;161;0
WireConnection;92;0;78;0
WireConnection;92;1;145;0
WireConnection;0;0;137;0
WireConnection;0;2;150;0
WireConnection;0;7;157;0
WireConnection;0;9;80;0
ASEEND*/
//CHKSM=6F1B35229EABC41CF1426B9841DE8AE4C94CDB51