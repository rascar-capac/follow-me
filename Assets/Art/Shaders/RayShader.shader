// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/RayShader"
{
	Properties
	{
		_FresnelColor("FresnelColor", Color) = (0.968945,1,0.4575472,0)
		_FresnelBias("FresnelBias", Range( -2 , 2)) = 0
		_FresnelScale("FresnelScale", Range( 0 , 2)) = 0.99
		_FresnelPower("FresnelPower", Range( 0 , 50)) = 5
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_ContrastIntensity("ContrastIntensity", Float) = 2.1
		_EmissiveIntensity("EmissiveIntensity", Float) = 1
		_AlbedoIntensity("AlbedoIntensity", Float) = 2
		_GradientSide("GradientSide", Range( 0 , 1)) = 0
		_Texture("Texture", 2D) = "white" {}
		_OpacityFalloff("OpacityFalloff", Float) = 1
		_OpacityStart("OpacityStart", Float) = 0
		_Color("Color", Color) = (1,0.7611992,0.4103774,0)
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
		uniform float _ContrastIntensity;
		uniform float _EmissiveIntensity;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float4 _FresnelColor;
		uniform float _GradientSide;
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
			float2 temp_cast_0 = (( _Time.y * -0.5 )).xx;
			float2 uv_TexCoord123 = i.uv_texcoord + temp_cast_0;
			float2 temp_cast_1 = (uv_TexCoord123.x).xx;
			float4 tex2DNode118 = tex2D( _Texture, temp_cast_1 );
			o.Albedo = CalculateContrast(_AlbedoIntensity,( _Color * tex2DNode118 )).rgb;
			float4 temp_cast_3 = (( tex2DNode118 * _EmissiveIntensity )).xxxx;
			o.Emission = CalculateContrast(_ContrastIntensity,temp_cast_3).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV15 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode15 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV15, _FresnelPower ) );
			o.Translucency = ( fresnelNode15 * _FresnelColor ).rgb;
			float lerpResult92 = lerp( ( 1.0 - i.uv_texcoord.y ) , i.uv_texcoord.y , _GradientSide);
			o.Alpha = (_OpacityStart + (lerpResult92 - 0.0) * (_OpacityFalloff - _OpacityStart) / (1.0 - 0.0));
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
23;118;1899;843;1444.763;885.6655;2.217696;True;True
Node;AmplifyShaderEditor.CommentaryNode;132;-415.8296,-571.5679;Inherit;False;1729.153;413.3281;;9;139;140;130;131;118;123;121;142;143;Emissive;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;121;-367.8491,-487.9808;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-395.4565,-358.135;Inherit;False;Constant;_Frequency;Frequency;15;0;Create;True;0;0;False;0;-0.5;0;-40;40;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;101;-60.06569,474.5388;Inherit;False;1396.347;419.4635;Y axe, can switch gradient start with GradientSide;7;80;81;82;92;93;77;78;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-181.4565,-419.135;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;144;365.1438,-957.8981;Inherit;False;1056.598;370.8063;Comment;4;56;134;138;137;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;13;439.7368,-117.3871;Inherit;False;871.549;532.403;Translucency;6;14;16;21;20;22;15;FresnelEffect;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;123;51.2823,-512.9687;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-10.06561,524.5388;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;471.1728,86.93061;Inherit;False;Property;_FresnelPower;FresnelPower;3;0;Create;True;0;0;False;0;5;4.74;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;176.2752,779.0023;Float;False;Property;_GradientSide;GradientSide;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;78;283.7819,536.1531;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;118;363.2411,-521.5679;Inherit;True;Property;_Texture;Texture;15;0;Create;True;0;0;False;0;-1;9fbef4b79ca3b784ba023cb1331520d5;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;468.9667,-70.76231;Inherit;False;Property;_FresnelBias;FresnelBias;1;0;Create;True;0;0;False;0;0;1.58;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;130;405.3687,-317.5714;Inherit;False;Property;_EmissiveIntensity;EmissiveIntensity;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;56;415.1438,-907.8981;Inherit;False;Property;_Color;Color;18;0;Create;True;0;0;False;0;1,0.7611992,0.4103774,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;468.5478,8.237547;Inherit;False;Property;_FresnelScale;FresnelScale;2;0;Create;True;0;0;False;0;0.99;1.58;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;752.3837,-274.3656;Inherit;False;Property;_ContrastIntensity;ContrastIntensity;11;0;Create;True;0;0;False;0;2.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;138;907.7827,-702.0917;Inherit;False;Property;_AlbedoIntensity;AlbedoIntensity;13;0;Create;True;0;0;False;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;793.5785,689.636;Inherit;False;Property;_OpacityStart;OpacityStart;17;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;92;538.5751,551.7022;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;15;803.0352,-61.72138;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;131;725.179,-507.5649;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;134;682.6472,-876.215;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;82;789.467,765.3392;Inherit;False;Property;_OpacityFalloff;OpacityFalloff;16;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;825.6801,199.9972;Inherit;False;Property;_FresnelColor;FresnelColor;0;0;Create;True;0;0;False;0;0.968945,1,0.4575472,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;139;1039.639,-459.1327;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;3.15;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;80;1042.281,627.4526;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;1142.634,123.5001;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;137;1154.742,-886.3851;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;3.15;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1582.117,-188.7155;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/RayShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;True;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;2;4;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;143;0;121;0
WireConnection;143;1;142;0
WireConnection;123;1;143;0
WireConnection;78;0;77;2
WireConnection;118;1;123;1
WireConnection;92;0;78;0
WireConnection;92;1;77;2
WireConnection;92;2;93;0
WireConnection;15;1;22;0
WireConnection;15;2;21;0
WireConnection;15;3;20;0
WireConnection;131;0;118;0
WireConnection;131;1;130;0
WireConnection;134;0;56;0
WireConnection;134;1;118;0
WireConnection;139;1;131;0
WireConnection;139;0;140;0
WireConnection;80;0;92;0
WireConnection;80;3;81;0
WireConnection;80;4;82;0
WireConnection;16;0;15;0
WireConnection;16;1;14;0
WireConnection;137;1;134;0
WireConnection;137;0;138;0
WireConnection;0;0;137;0
WireConnection;0;2;139;0
WireConnection;0;7;16;0
WireConnection;0;9;80;0
ASEEND*/
//CHKSM=2A2AF9D28EBE89CFD5EE4F5D3C8C0BB5C8EB5AF5