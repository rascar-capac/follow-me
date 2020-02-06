// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/CliffShader"
{
	Properties
	{
		_ModelNormal("ModelNormal", 2D) = "bump" {}
		_ModelAO("ModelAO", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_CliffNormalScale("CliffNormalScale", Float) = 1
		_Grass("Grass", 2D) = "white" {}
		_NormalGrass("NormalGrass", 2D) = "bump" {}
		_GrassNormalScale("GrassNormalScale", Float) = 1
		_Tilling("Tilling", Vector) = (1,1,0,0)
		_TriplanarFalloff("TriplanarFalloff", Float) = 1
		_Metallic("Metallic", Range( 0 , 1)) = 0.6485723
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.7037836
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
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

		uniform sampler2D _ModelNormal;
		uniform float4 _ModelNormal_ST;
		uniform float _CliffNormalScale;
		uniform sampler2D _Normal;
		uniform float2 _Tilling;
		uniform float _TriplanarFalloff;
		uniform float _GrassNormalScale;
		uniform sampler2D _NormalGrass;
		uniform sampler2D _Albedo;
		uniform sampler2D _Grass;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _ModelAO;
		uniform float4 _ModelAO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_ModelNormal = i.uv_texcoord * _ModelNormal_ST.xy + _ModelNormal_ST.zw;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform32 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float2 appendResult30 = (float2(transform32.x , transform32.y));
			float2 ProjectionSideXY63 = ( appendResult30 * _Tilling );
			float2 appendResult34 = (float2(transform32.y , transform32.z));
			float2 ProjectionSideYZ64 = ( appendResult34 * _Tilling );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 transform43 = mul(unity_ObjectToWorld,float4( ase_vertexNormal , 0.0 ));
			float NormalAbsX68 = pow( abs( transform43.x ) , _TriplanarFalloff );
			float3 lerpResult84 = lerp( UnpackScaleNormal( tex2D( _Normal, ProjectionSideXY63 ), _CliffNormalScale ) , UnpackNormal( tex2D( _Normal, ProjectionSideYZ64 ) ) , NormalAbsX68);
			float2 appendResult35 = (float2(transform32.x , transform32.z));
			float2 ProjectionTopXZ65 = ( appendResult35 * _Tilling );
			float NormalZ70 = (0.0 + (transform43.y - -1.0) * (1.0 - 0.0) / (1.0 - -1.0));
			float3 lerpResult85 = lerp( UnpackNormal( tex2D( _Normal, ProjectionTopXZ65 ) ) , UnpackScaleNormal( tex2D( _NormalGrass, ProjectionTopXZ65 ), _GrassNormalScale ) , NormalZ70);
			float NormalAbsY69 = pow( abs( transform43.y ) , _TriplanarFalloff );
			float3 lerpResult79 = lerp( lerpResult84 , lerpResult85 , NormalAbsY69);
			float3 normalizeResult99 = normalize( ( UnpackNormal( tex2D( _ModelNormal, uv_ModelNormal ) ) + lerpResult79 ) );
			o.Normal = normalizeResult99;
			float4 lerpResult40 = lerp( tex2D( _Albedo, ProjectionSideXY63 ) , tex2D( _Albedo, ProjectionSideYZ64 ) , NormalAbsX68);
			float4 lerpResult51 = lerp( tex2D( _Albedo, ProjectionTopXZ65 ) , tex2D( _Grass, ProjectionTopXZ65 ) , NormalZ70);
			float4 lerpResult41 = lerp( lerpResult40 , lerpResult51 , NormalAbsY69);
			o.Albedo = lerpResult41.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_ModelAO = i.uv_texcoord * _ModelAO_ST.xy + _ModelAO_ST.zw;
			o.Occlusion = tex2D( _ModelAO, uv_ModelAO ).r;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
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
38;201;1899;747;1444.935;333.9004;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;91;-4786.352,-1443.332;Inherit;False;3506.142;2527.924;Comment;14;61;60;75;74;64;65;63;70;89;90;68;69;72;40;DetailMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;61;-4493.524,-1393.332;Inherit;False;989.7302;804.5961;Projection;6;38;37;36;31;32;27;Projection;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;60;-4736.352,-376.6821;Inherit;False;1183.787;638.2091;Normal;8;42;43;45;59;44;52;58;57;World Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;31;-4443.524,-1284.833;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;32;-4228.524,-1283.833;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;38;-3909.953,-1343.332;Inherit;False;336.6;242.8;Side X Y;2;30;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;37;-3902.694,-1102.5;Inherit;False;330.1;254.5;Side Y Z;2;34;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;36;-3899.494,-841.6008;Inherit;False;345.7;271.4;Top X Z;2;35;49;;1,1,1,1;0;0
Node;AmplifyShaderEditor.NormalVertexDataNode;42;-4686.352,-150.9608;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;34;-3875.694,-1058.5;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;43;-4422.722,-142.0326;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;27;-4269.486,-749.7354;Inherit;False;Property;_Tilling;Tilling;11;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;30;-3880.953,-1285.332;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-3870.494,-791.6008;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-3715.213,-1056.452;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-4025.246,-243.0952;Inherit;False;Property;_TriplanarFalloff;TriplanarFalloff;12;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-3716.213,-1279.453;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;45;-3987.357,-118.5005;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-3708.213,-780.4524;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;57;-3743.896,-189.6821;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;52;-4023.053,58.30598;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-3425.57,-1319.706;Inherit;False;ProjectionSideXY;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-3404.69,-767.0105;Inherit;False;ProjectionTopXZ;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.AbsOpNode;44;-3993.583,-39.47337;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;75;-2838.895,71.11732;Inherit;False;1508.685;1013.476;Comment;13;88;87;86;85;84;83;82;81;80;79;78;77;76;Normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;64;-3418.646,-1051.187;Inherit;False;ProjectionSideYZ;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-2788.895,702.6484;Inherit;False;65;ProjectionTopXZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;68;-3502.272,-328.8994;Inherit;False;NormalAbsX;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3034.461,746.9781;Inherit;False;Property;_GrassNormalScale;GrassNormalScale;10;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2738.55,121.1175;Inherit;False;63;ProjectionSideXY;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-2750.025,403.3772;Inherit;False;64;ProjectionSideYZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-3045.461,394.9781;Inherit;False;Property;_CliffNormalScale;CliffNormalScale;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-3522.431,38.74886;Inherit;False;NormalZ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;58;-3755.565,-79.65501;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;80;-2462.565,158.703;Inherit;True;Property;_Normal;Normal;6;0;Create;True;0;0;False;0;-1;2e0ae9eb0cfb950488818dd979f958b7;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;88;-2164.465,887.1394;Inherit;False;70;NormalZ;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;69;-3526.431,-144.2511;Inherit;False;NormalAbsY;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;74;-3002.859,-1317.501;Inherit;False;1508.685;1013.476;Comment;13;33;50;39;3;62;67;66;51;71;73;40;41;72;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;78;-2472.494,613.7892;Inherit;True;Property;_TextureSample5;Texture Sample 5;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Instance;80;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;76;-2463.494,358.7892;Inherit;True;Property;_TextureSample4;Texture Sample 4;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Instance;80;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;86;-2182.381,433.8965;Inherit;False;68;NormalAbsX;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;77;-2499.78,854.593;Inherit;True;Property;_NormalGrass;NormalGrass;9;0;Create;True;0;0;False;0;-1;ccc652a56e4f8394b8aade5d45837f4b;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;62;-2902.514,-1267.501;Inherit;False;63;ProjectionSideXY;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-2952.859,-685.97;Inherit;False;65;ProjectionTopXZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-2898.389,-1017.74;Inherit;False;64;ProjectionSideYZ;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;85;-2001.889,615.6536;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;84;-1908.077,155.2585;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;87;-1840.208,867.4332;Inherit;False;69;NormalAbsY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;79;-1595.21,556.3146;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;33;-2627.427,-1024.542;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;3;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-2629.946,-1238.05;Inherit;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;-1;0f99824124a559a4497b267fceb66a69;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;92;-953.7856,-2.742428;Inherit;True;Property;_ModelNormal;ModelNormal;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;50;-2663.745,-534.0251;Inherit;True;Property;_Grass;Grass;8;0;Create;True;0;0;False;0;-1;99e0ab75521fdcc4985745d7b62a8f81;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;73;-2328.43,-501.4784;Inherit;False;70;NormalZ;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;71;-2346.346,-954.7215;Inherit;False;68;NormalAbsX;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;39;-2644.427,-769.5422;Inherit;True;Property;_TextureSample1;Texture Sample 1;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;3;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;72;-2029.743,-486.9442;Inherit;False;69;NormalAbsY;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;-2249.383,-1192.962;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;100;-384.6953,41.61841;Inherit;False;235;160;Normaliser l'ensemble des normals pour garder une magnitude de 1;1;99;;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;51;-2165.854,-772.9646;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-589.2538,133.4866;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;41;-1742.992,-774.3123;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-398.4815,284.7939;Inherit;False;Property;_Metallic;Metallic;13;0;Create;True;0;0;False;0;0.6485723;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-405.1042,376.9467;Inherit;False;Property;_Smoothness;Smoothness;14;0;Create;True;0;0;False;0;0.7037836;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;99;-334.6953,91.61841;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;93;-953.0416,-212.6348;Inherit;True;Property;_ModelAO;ModelAO;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;91.32677,-54.14069;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/CliffShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;31;0
WireConnection;34;0;32;2
WireConnection;34;1;32;3
WireConnection;43;0;42;0
WireConnection;30;0;32;1
WireConnection;30;1;32;2
WireConnection;35;0;32;1
WireConnection;35;1;32;3
WireConnection;48;0;34;0
WireConnection;48;1;27;0
WireConnection;47;0;30;0
WireConnection;47;1;27;0
WireConnection;45;0;43;1
WireConnection;49;0;35;0
WireConnection;49;1;27;0
WireConnection;57;0;45;0
WireConnection;57;1;59;0
WireConnection;52;0;43;2
WireConnection;63;0;47;0
WireConnection;65;0;49;0
WireConnection;44;0;43;2
WireConnection;64;0;48;0
WireConnection;68;0;57;0
WireConnection;70;0;52;0
WireConnection;58;0;44;0
WireConnection;58;1;59;0
WireConnection;80;1;81;0
WireConnection;80;5;89;0
WireConnection;69;0;58;0
WireConnection;78;1;82;0
WireConnection;76;1;83;0
WireConnection;77;1;82;0
WireConnection;77;5;90;0
WireConnection;85;0;78;0
WireConnection;85;1;77;0
WireConnection;85;2;88;0
WireConnection;84;0;80;0
WireConnection;84;1;76;0
WireConnection;84;2;86;0
WireConnection;79;0;84;0
WireConnection;79;1;85;0
WireConnection;79;2;87;0
WireConnection;33;1;66;0
WireConnection;3;1;62;0
WireConnection;50;1;67;0
WireConnection;39;1;67;0
WireConnection;40;0;3;0
WireConnection;40;1;33;0
WireConnection;40;2;71;0
WireConnection;51;0;39;0
WireConnection;51;1;50;0
WireConnection;51;2;73;0
WireConnection;96;0;92;0
WireConnection;96;1;79;0
WireConnection;41;0;40;0
WireConnection;41;1;51;0
WireConnection;41;2;72;0
WireConnection;99;0;96;0
WireConnection;2;0;41;0
WireConnection;2;1;99;0
WireConnection;2;3;8;0
WireConnection;2;4;20;0
WireConnection;2;5;93;0
ASEEND*/
//CHKSM=09177526BFD742A5CFF538A0447A4EFDAFD69B2B