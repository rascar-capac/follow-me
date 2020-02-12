// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/DayNight_GrassShader"
{
	Properties
	{
		_Day_Texture("Day_Texture", 2D) = "white" {}
		_DayColor("DayColor", Color) = (1,1,1,0)
		_Night_Texture("Night_Texture", 2D) = "white" {}
		_NightColor("NightColor", Color) = (1,1,1,0)
		_DayNightAlbedo("DayNightAlbedo", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.2
		_Day_EmissiveTexture("Day_EmissiveTexture", 2D) = "white" {}
		_Day_EmissiveColor("Day_EmissiveColor", Color) = (0,0,0,0)
		_Night_EmissiveTexture("Night_EmissiveTexture", 2D) = "white" {}
		_Night_EmissiveColor("Night_EmissiveColor", Color) = (0,0,0,0)
		_DayNightEmissive("DayNightEmissive", Range( 0 , 1)) = 0
		_NoiseScale("NoiseScale", Float) = 1
		_NoiseFrequency("NoiseFrequency", Float) = 0.1
		_YFalloff("YFalloff", Float) = 0.5
		_Day_FresnelColor("Day_FresnelColor", Color) = (0.8226063,1,0.240566,0)
		_Night_FresnelColor("Night_FresnelColor", Color) = (0.3191082,0.9391559,0.9528302,0)
		_DayNightFresnel("DayNightFresnel", Range( 0 , 1)) = 0
		_FresnelBias("FresnelBias", Range( -2 , 2)) = 0
		_Float0("Float 0", Range( -2 , 2)) = 0
		_Float1("Float 1", Range( 0 , 2)) = 1
		_FresnelScale("FresnelScale", Range( 0 , 2)) = 1
		_Float2("Float 2", Range( 0 , 50)) = 2
		_FresnelPower("FresnelPower", Range( 0 , 50)) = 2
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
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

		uniform float _NoiseFrequency;
		uniform float _NoiseScale;
		uniform float _YFalloff;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Day_Texture;
		uniform float4 _Day_Texture_ST;
		uniform float4 _DayColor;
		uniform sampler2D _Night_Texture;
		uniform float4 _Night_Texture_ST;
		uniform float4 _NightColor;
		uniform float _DayNightAlbedo;
		uniform sampler2D _Day_EmissiveTexture;
		uniform float4 _Day_EmissiveTexture_ST;
		uniform float4 _Day_EmissiveColor;
		uniform sampler2D _Night_EmissiveTexture;
		uniform float4 _Night_EmissiveTexture_ST;
		uniform float4 _Night_EmissiveColor;
		uniform float _DayNightEmissive;
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
		uniform float _Float2;
		uniform float4 _Day_FresnelColor;
		uniform float4 _Night_FresnelColor;
		uniform float _DayNightFresnel;
		uniform float _Float0;
		uniform float _Float1;
		uniform float _FresnelPower;
		uniform float _Cutoff = 0.5;


		float2 voronoihash42( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi42( float2 v, float time, inout float2 id, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash42( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime47 = _Time.y * _NoiseFrequency;
			float time42 = _Time.y;
			float2 coords42 = ase_vertex3Pos.xy * sin( ( ase_vertex3Pos.x + ase_vertex3Pos.y + ase_vertex3Pos.z + mulTime47 ) );
			float2 id42 = 0;
			float voroi42 = voronoi42( coords42, time42,id42, 0 );
			float3 ase_vertexNormal = v.normal.xyz;
			v.vertex.xyz += ( voroi42 * ( ase_vertexNormal * _NoiseScale ) * ( ase_vertex3Pos.y * _YFalloff ) );
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
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Day_Texture = i.uv_texcoord * _Day_Texture_ST.xy + _Day_Texture_ST.zw;
			float4 tex2DNode73 = tex2D( _Day_Texture, uv_Day_Texture );
			float2 uv_Night_Texture = i.uv_texcoord * _Night_Texture_ST.xy + _Night_Texture_ST.zw;
			float4 lerpResult81 = lerp( ( tex2DNode73 * _DayColor ) , ( tex2D( _Night_Texture, uv_Night_Texture ) * _NightColor ) , _DayNightAlbedo);
			o.Albedo = lerpResult81.rgb;
			float2 uv_Day_EmissiveTexture = i.uv_texcoord * _Day_EmissiveTexture_ST.xy + _Day_EmissiveTexture_ST.zw;
			float2 uv_Night_EmissiveTexture = i.uv_texcoord * _Night_EmissiveTexture_ST.xy + _Night_EmissiveTexture_ST.zw;
			float4 lerpResult82 = lerp( ( tex2D( _Day_EmissiveTexture, uv_Day_EmissiveTexture ) * _Day_EmissiveColor ) , ( tex2D( _Night_EmissiveTexture, uv_Night_EmissiveTexture ) * _Night_EmissiveColor ) , _DayNightEmissive);
			o.Emission = lerpResult82.rgb;
			o.Smoothness = _Smoothness;
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV71 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode71 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNdotV71, _Float2 ) );
			float4 lerpResult103 = lerp( _Day_FresnelColor , _Night_FresnelColor , _DayNightFresnel);
			float fresnelNdotV97 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode97 = ( _Float0 + _Float1 * pow( 1.0 - fresnelNdotV97, _FresnelPower ) );
			float4 lerpResult100 = lerp( ( fresnelNode71 * lerpResult103 ) , ( fresnelNode97 * lerpResult103 ) , _DayNightFresnel);
			o.Translucency = lerpResult100.rgb;
			o.Alpha = 1;
			clip( tex2DNode73.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				vertexDataFunc( v, customInputData );
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
579;156;1899;835;3541.089;3452.934;5.447826;True;True
Node;AmplifyShaderEditor.CommentaryNode;53;-2248.241,116.1397;Inherit;False;1801.821;1073.201;Comment;15;33;42;51;56;57;46;52;37;48;55;44;47;24;50;58;Wind;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2198.241,471.9169;Inherit;False;Property;_NoiseFrequency;NoiseFrequency;14;0;Create;True;0;0;False;0;0.1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;24;-1922.393,166.1397;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;47;-1910.669,478.5987;Inherit;False;1;0;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;93;-2275.167,-615.5226;Inherit;False;871.549;532.403;Translucency;5;99;97;96;95;94;FresnelEffectNIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-2270.598,-1231.721;Inherit;False;871.549;532.403;Translucency;5;91;90;89;72;71;FresnelEffectDAY;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;70;-3021.13,-921.9047;Inherit;False;Property;_Day_FresnelColor;Day_FresnelColor;16;0;Create;True;0;0;False;0;0.8226063,1,0.240566,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;69;-2236.932,-1850.678;Inherit;False;639.2007;482.5995;Comment;3;88;86;78;NUIT - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-1629.157,397.6233;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;68;-2263.133,-2457.247;Inherit;False;639.2007;482.5995;Comment;3;87;85;77;JOUR - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;67;-2265.556,-3689.312;Inherit;False;737.939;439.7025;Comment;3;79;74;73;JOUR Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;66;-2246.527,-3095.076;Inherit;False;736.3441;511.4706;Comment;3;84;80;75;NUIT Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-3103.452,-575.9743;Inherit;False;Property;_DayNightFresnel;DayNightFresnel;18;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-2239.162,-1027.403;Inherit;False;Property;_Float2;Float 2;23;0;Create;True;0;0;False;0;2;4.74;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2246.357,-489.8989;Inherit;False;Property;_Float1;Float 1;21;0;Create;True;0;0;False;0;1;1.58;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-2243.732,-411.2048;Inherit;False;Property;_FresnelPower;FresnelPower;24;0;Create;True;0;0;False;0;2;4.74;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-2245.938,-568.8988;Inherit;False;Property;_Float0;Float 0;20;0;Create;True;0;0;False;0;0;1.58;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-2241.787,-1106.097;Inherit;False;Property;_FresnelScale;FresnelScale;22;0;Create;True;0;0;False;0;1;1.58;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-2241.368,-1185.097;Inherit;False;Property;_FresnelBias;FresnelBias;19;0;Create;True;0;0;False;0;0;1.58;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;98;-3033.895,-346.6887;Inherit;False;Property;_Night_FresnelColor;Night_FresnelColor;17;0;Create;True;0;0;False;0;0.3191082,0.9391559,0.9528302,0;0.8226063,1,0.240566,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;84;-2179.131,-3014.745;Inherit;True;Property;_Night_Texture;Night_Texture;2;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;37;-1579.836,252.3315;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-1154.449,979.7748;Inherit;False;Property;_YFalloff;YFalloff;15;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;75;-2157.871,-2807.116;Inherit;False;Property;_NightColor;NightColor;3;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-1159.847,661.4635;Inherit;False;Property;_NoiseScale;NoiseScale;13;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;46;-1430.472,374.9945;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;85;-2213.133,-2406.248;Inherit;True;Property;_Day_EmissiveTexture;Day_EmissiveTexture;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;74;-2179.517,-3427.962;Inherit;False;Property;_DayColor;DayColor;1;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-2228.663,-3629.594;Inherit;True;Property;_Day_Texture;Day_Texture;0;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;87;-2153.344,-2181.646;Inherit;False;Property;_Day_EmissiveColor;Day_EmissiveColor;9;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;86;-2184.37,-1801.091;Inherit;True;Property;_Night_EmissiveTexture;Night_EmissiveTexture;10;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;88;-2124.581,-1575.491;Inherit;False;Property;_Night_EmissiveColor;Night_EmissiveColor;11;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;48;-1158.582,504.0136;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;103;-2741.991,-702.3668;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;55;-1264.985,808.7651;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;71;-1907.302,-1176.056;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;97;-1911.872,-559.8578;Inherit;True;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-1401.819,-2992.846;Inherit;False;Property;_DayNightAlbedo;DayNightAlbedo;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1743.286,-3506.886;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;80;-1697.632,-2892.037;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-834.4473,451.0634;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-1445.807,-1803.067;Inherit;False;Property;_DayNightEmissive;DayNightEmissive;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;42;-1198.605,222.7529;Inherit;True;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1787.294,-2239.865;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-1758.529,-1633.709;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-934.7501,847.1746;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-1566.339,-951.2182;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1572.272,-374.6355;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;-628.9574,324.5183;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;92;-344.4801,-124.7581;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;81;-1060.066,-3215.761;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;82;-1135.729,-2070.849;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;60;-328.1405,116.9251;Inherit;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;0.2;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;100;-1099.309,-672.7162;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;61;-336.1405,207.9251;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;54,10;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/DayNight_GrassShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;32;25;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;58;-1314.985,758.7651;Inherit;False;615.235;341.4095;BaseImmobile;0;BaseImmobile;1,1,1,1;0;0
WireConnection;47;0;50;0
WireConnection;44;0;24;1
WireConnection;44;1;24;2
WireConnection;44;2;24;3
WireConnection;44;3;47;0
WireConnection;46;0;44;0
WireConnection;103;0;70;0
WireConnection;103;1;98;0
WireConnection;103;2;102;0
WireConnection;71;1;89;0
WireConnection;71;2;90;0
WireConnection;71;3;91;0
WireConnection;97;1;94;0
WireConnection;97;2;96;0
WireConnection;97;3;95;0
WireConnection;79;0;73;0
WireConnection;79;1;74;0
WireConnection;80;0;84;0
WireConnection;80;1;75;0
WireConnection;51;0;48;0
WireConnection;51;1;52;0
WireConnection;42;0;24;0
WireConnection;42;1;37;0
WireConnection;42;2;46;0
WireConnection;77;0;85;0
WireConnection;77;1;87;0
WireConnection;78;0;86;0
WireConnection;78;1;88;0
WireConnection;56;0;55;2
WireConnection;56;1;57;0
WireConnection;72;0;71;0
WireConnection;72;1;103;0
WireConnection;99;0;97;0
WireConnection;99;1;103;0
WireConnection;33;0;42;0
WireConnection;33;1;51;0
WireConnection;33;2;56;0
WireConnection;81;0;79;0
WireConnection;81;1;80;0
WireConnection;81;2;76;0
WireConnection;82;0;77;0
WireConnection;82;1;78;0
WireConnection;82;2;83;0
WireConnection;100;0;72;0
WireConnection;100;1;99;0
WireConnection;100;2;102;0
WireConnection;0;0;81;0
WireConnection;0;1;92;0
WireConnection;0;2;82;0
WireConnection;0;4;60;0
WireConnection;0;5;61;1
WireConnection;0;7;100;0
WireConnection;0;10;73;4
WireConnection;0;11;33;0
ASEEND*/
//CHKSM=4FBA2C4B059039F22603B360CC3BB16B8CCC2492