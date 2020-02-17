// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/EmissiveTile"
{
	Properties
	{
		_Mesh_Albedo("Mesh_Albedo", 2D) = "white" {}
		_Mesh_Normal("Mesh_Normal", 2D) = "white" {}
		_Mesh_Emissive("Mesh_Emissive", 2D) = "white" {}
		_EmissiveColor("EmissiveColor", Color) = (0,0,0,0)
		_Mesh_AO("Mesh_AO", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0.6485723
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.7037836
		_Tilling1("Tilling", Vector) = (1,1,0,0)
		_Detail_Albedo("Detail_Albedo", 2D) = "white" {}
		_Detail_Normal("Detail_Normal", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mesh_Normal;
		uniform float4 _Mesh_Normal_ST;
		uniform sampler2D _Detail_Normal;
		uniform float2 _Tilling1;
		uniform sampler2D _Mesh_Albedo;
		uniform float4 _Mesh_Albedo_ST;
		uniform sampler2D _Detail_Albedo;
		uniform sampler2D _Mesh_Emissive;
		uniform float4 _Mesh_Emissive_ST;
		uniform float4 _EmissiveColor;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _Mesh_AO;
		uniform float4 _Mesh_AO_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Mesh_Normal = i.uv_texcoord * _Mesh_Normal_ST.xy + _Mesh_Normal_ST.zw;
			float4 normalizeResult114 = normalize( ( tex2D( _Mesh_Normal, uv_Mesh_Normal ) + tex2D( _Detail_Normal, _Tilling1 ) ) );
			o.Normal = normalizeResult114.rgb;
			float2 uv_Mesh_Albedo = i.uv_texcoord * _Mesh_Albedo_ST.xy + _Mesh_Albedo_ST.zw;
			o.Albedo = ( tex2D( _Mesh_Albedo, uv_Mesh_Albedo ) * tex2D( _Detail_Albedo, _Tilling1 ) ).rgb;
			float2 uv_Mesh_Emissive = i.uv_texcoord * _Mesh_Emissive_ST.xy + _Mesh_Emissive_ST.zw;
			o.Emission = ( tex2D( _Mesh_Emissive, uv_Mesh_Emissive ) * _EmissiveColor ).rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_Mesh_AO = i.uv_texcoord * _Mesh_AO_ST.xy + _Mesh_AO_ST.zw;
			o.Occlusion = tex2D( _Mesh_AO, uv_Mesh_AO ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
14;-1097;1899;799;2225.429;1256.988;1.630856;True;True
Node;AmplifyShaderEditor.CommentaryNode;118;-1871.247,-289.0402;Inherit;False;238;211;Comment;1;104;GENERAL TILLING;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;115;-1359.586,-316.4207;Inherit;False;812.7202;512.4452;Comment;4;111;109;113;114;NORMAL;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;104;-1821.247,-239.0402;Inherit;False;Property;_Tilling1;Tilling;7;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;117;-501.738,-584.99;Inherit;False;653.9999;452.486;Comment;3;102;101;103;EMISSIVE;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;111;-1300.456,-266.4207;Inherit;True;Property;_Mesh_Normal;Mesh_Normal;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;116;-1343.804,-1007.616;Inherit;False;661.1537;512.4448;Comment;3;106;105;107;ALBEDO;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;109;-1309.586,-33.9755;Inherit;True;Property;_Detail_Normal;Detail_Normal;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;101;-398.069,-339.504;Inherit;False;Property;_EmissiveColor;EmissiveColor;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;102;-451.7379,-534.99;Inherit;True;Property;_Mesh_Emissive;Mesh_Emissive;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;105;-1293.804,-725.171;Inherit;True;Property;_Detail_Albedo;Detail_Albedo;8;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;106;-1284.674,-957.6158;Inherit;True;Property;_Mesh_Albedo;Mesh_Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;113;-909.4203,-107.6984;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;93;-416.8143,141.3237;Inherit;True;Property;_Mesh_AO;Mesh_AO;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-412.6743,53.5941;Inherit;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;False;0;0.7037836;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-419.1508,-33.72728;Inherit;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0.6485723;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;103;-16.738,-363.9899;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;114;-731.866,-112.6649;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-851.6506,-811.0132;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;2;91.32677,-54.14069;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/EmissiveTile;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;109;1;104;0
WireConnection;105;1;104;0
WireConnection;113;0;111;0
WireConnection;113;1;109;0
WireConnection;103;0;102;0
WireConnection;103;1;101;0
WireConnection;114;0;113;0
WireConnection;107;0;106;0
WireConnection;107;1;105;0
WireConnection;2;0;107;0
WireConnection;2;1;114;0
WireConnection;2;2;103;0
WireConnection;2;3;8;0
WireConnection;2;4;20;0
WireConnection;2;5;93;0
ASEEND*/
//CHKSM=EF7A70B3D842F13E030ABDBCD1F689F13D0605E1