// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/PulseStone"
{
	Properties
	{
		_Base_Texture("Base_Texture", 2D) = "white" {}
		_BaseColor("BaseColor", Color) = (1,1,1,0)
		_Pulse_Texture("Pulse_Texture", 2D) = "white" {}
		_PulseColor("PulseColor", Color) = (1,1,1,0)
		_PulseState("PulseState", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0.4
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.4
		_Base_EmissiveTexture("Base_EmissiveTexture", 2D) = "white" {}
		_Base_EmissiveColor("Base_EmissiveColor", Color) = (0,0,0,0)
		_Pulse_EmissiveTexture("Pulse_EmissiveTexture", 2D) = "white" {}
		_Pulse_EmissiveColor("Pulse_EmissiveColor", Color) = (0,0,0,0)
		_Base_FresnelColor("Base_FresnelColor", Color) = (0.8226063,1,0.240566,0)
		_Pulse_FresnelColor("Pulse_FresnelColor", Color) = (0.3191082,0.9391559,0.9528302,0)
		_FresnelBias("FresnelBias", Range( -2 , 2)) = 0
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_FresnelScale("FresnelScale", Range( 0 , 2)) = 1
		_FresnelPower("FresnelPower", Range( 0 , 50)) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Base_Texture;
		uniform float4 _Base_Texture_ST;
		uniform float4 _BaseColor;
		uniform sampler2D _Pulse_Texture;
		uniform float4 _Pulse_Texture_ST;
		uniform float4 _PulseColor;
		uniform float _PulseState;
		uniform sampler2D _Base_EmissiveTexture;
		uniform float4 _Base_EmissiveTexture_ST;
		uniform float4 _Base_EmissiveColor;
		uniform sampler2D _Pulse_EmissiveTexture;
		uniform float4 _Pulse_EmissiveTexture_ST;
		uniform float4 _Pulse_EmissiveColor;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float4 _Base_FresnelColor;
		uniform float4 _Pulse_FresnelColor;

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
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Base_Texture = i.uv_texcoord * _Base_Texture_ST.xy + _Base_Texture_ST.zw;
			float4 tex2DNode69 = tex2D( _Base_Texture, uv_Base_Texture );
			float2 uv_Pulse_Texture = i.uv_texcoord * _Pulse_Texture_ST.xy + _Pulse_Texture_ST.zw;
			float4 lerpResult79 = lerp( ( tex2DNode69 * _BaseColor ) , ( tex2D( _Pulse_Texture, uv_Pulse_Texture ) * _PulseColor ) , _PulseState);
			o.Albedo = lerpResult79.rgb;
			float2 uv_Base_EmissiveTexture = i.uv_texcoord * _Base_EmissiveTexture_ST.xy + _Base_EmissiveTexture_ST.zw;
			float2 uv_Pulse_EmissiveTexture = i.uv_texcoord * _Pulse_EmissiveTexture_ST.xy + _Pulse_EmissiveTexture_ST.zw;
			float PulseState100 = _PulseState;
			float4 lerpResult80 = lerp( ( tex2D( _Base_EmissiveTexture, uv_Base_EmissiveTexture ) * _Base_EmissiveColor ) , ( tex2D( _Pulse_EmissiveTexture, uv_Pulse_EmissiveTexture ) * _Pulse_EmissiveColor ) , PulseState100);
			o.Emission = lerpResult80.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV91 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode91 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV91, _FresnelPower ) );
			float4 lerpResult98 = lerp( _Base_FresnelColor , _Pulse_FresnelColor , PulseState100);
			float4 lerpResult84 = lerp( ( fresnelNode91 * lerpResult98 ) , float4( 0,0,0,0 ) , float4( 0,0,0,0 ));
			o.Translucency = lerpResult84.rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
36;138;1899;823;3230.966;2027.413;2.253844;True;True
Node;AmplifyShaderEditor.RangedFloatNode;74;-1560.784,-2005.541;Inherit;False;Property;_PulseState;PulseState;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;100;-1232.547,-1993.368;Inherit;False;PulseState;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;83;-3158.584,-540.6662;Inherit;False;871.549;532.403;Translucency;5;93;91;90;86;85;FresnelEffectDAY;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;62;-2949.639,-2395.536;Inherit;False;736.3441;511.4706;Comment;3;78;72;70;NUIT Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3127.149,-336.3482;Inherit;False;Property;_FresnelPower;FresnelPower;24;0;Create;True;0;0;False;0;2;4.74;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;95;-3722.654,-460.3413;Inherit;False;Property;_Base_FresnelColor;Base_FresnelColor;13;0;Create;True;0;0;False;0;0.8226063,1,0.240566,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;96;-3735.42,114.8748;Inherit;False;Property;_Pulse_FresnelColor;Pulse_FresnelColor;14;0;Create;True;0;0;False;0;0.3191082,0.9391559,0.9528302,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;65;-2882.32,-1124.259;Inherit;False;639.2007;482.5995;Comment;3;76;68;66;NUIT - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-3129.354,-494.0423;Inherit;False;Property;_FresnelBias;FresnelBias;15;0;Create;True;0;0;False;0;0;1.58;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;86;-3129.774,-415.0423;Inherit;False;Property;_FresnelScale;FresnelScale;23;0;Create;True;0;0;False;0;1;1.58;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;63;-2963.923,-2970.658;Inherit;False;737.939;439.7025;Comment;3;77;71;69;JOUR Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;-3789.953,-192.6305;Inherit;False;100;PulseState;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;64;-2926.918,-1659.535;Inherit;False;639.2007;482.5995;Comment;3;75;73;67;JOUR - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;98;-3443.516,-240.8033;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;68;-2769.969,-849.0723;Inherit;False;Property;_Pulse_EmissiveColor;Pulse_EmissiveColor;12;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-2829.758,-1074.672;Inherit;True;Property;_Pulse_EmissiveTexture;Pulse_EmissiveTexture;11;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;67;-2817.13,-1383.934;Inherit;False;Property;_Base_EmissiveColor;Base_EmissiveColor;10;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-2874.829,-1608.535;Inherit;True;Property;_Base_EmissiveTexture;Base_EmissiveTexture;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;72;-2860.983,-2107.576;Inherit;False;Property;_PulseColor;PulseColor;3;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;70;-2882.243,-2315.206;Inherit;True;Property;_Pulse_Texture;Pulse_Texture;2;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;69;-2927.03,-2910.94;Inherit;True;Property;_Base_Texture;Base_Texture;0;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;91;-2795.289,-485.0013;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;71;-2877.884,-2709.308;Inherit;False;Property;_BaseColor;BaseColor;1;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-2403.916,-907.2909;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-2454.326,-260.1635;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;102;-1355.485,-1006.965;Inherit;False;100;PulseState;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-2441.65,-2788.232;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-2400.744,-2192.498;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-2451.08,-1442.153;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;84;-1987.296,18.33856;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;59;-331.9666,223.9455;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;80;-1017.095,-1202.452;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;6;-921.3245,-394.861;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;79;-1031.147,-2306.586;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-351.9666,125.9455;Inherit;False;Property;_Smoothness;Smoothness;8;0;Create;True;0;0;False;0;0.4;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-362.3599,31.07025;Inherit;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;False;0;0.4;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;54,10;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/PulseStone;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;True;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;31;16;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;100;0;74;0
WireConnection;98;0;95;0
WireConnection;98;1;96;0
WireConnection;98;2;101;0
WireConnection;91;1;85;0
WireConnection;91;2;86;0
WireConnection;91;3;90;0
WireConnection;76;0;66;0
WireConnection;76;1;68;0
WireConnection;93;0;91;0
WireConnection;93;1;98;0
WireConnection;77;0;69;0
WireConnection;77;1;71;0
WireConnection;78;0;70;0
WireConnection;78;1;72;0
WireConnection;75;0;73;0
WireConnection;75;1;67;0
WireConnection;84;0;93;0
WireConnection;80;0;75;0
WireConnection;80;1;76;0
WireConnection;80;2;102;0
WireConnection;79;0;77;0
WireConnection;79;1;78;0
WireConnection;79;2;74;0
WireConnection;0;0;79;0
WireConnection;0;1;6;0
WireConnection;0;2;80;0
WireConnection;0;3;99;0
WireConnection;0;4;58;0
WireConnection;0;5;59;1
WireConnection;0;7;84;0
WireConnection;0;10;69;4
ASEEND*/
//CHKSM=11F17218171CEAAE5005748C3BAF9CE8F7EDA882