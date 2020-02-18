// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader_Blending"
{
	Properties
	{
		_PrimaryAlbedoWhite("Primary Albedo (White)", 2D) = "gray" {}
		_Primary_MS("Primary_MS", 2D) = "black" {}
		[Normal]_PrimaryNormalMap("Primary NormalMap", 2D) = "bump" {}
		_R_Albedo("R_Albedo", 2D) = "gray" {}
		_R_MS("R_MS", 2D) = "black" {}
		[Normal]_R_NormalMap("R_NormalMap", 2D) = "bump" {}
		_B_Albedo("B_Albedo", 2D) = "gray" {}
		_B_MS("B_MS", 2D) = "black" {}
		[Normal]_B_NormalMap("B_NormalMap", 2D) = "bump" {}
		_G_Albedo("G_Albedo", 2D) = "gray" {}
		_G_MS("G_MS", 2D) = "black" {}
		[Normal]_G_NormalMap("G_NormalMap", 2D) = "bump" {}
		_A_Albedo("A_ Albedo", 2D) = "gray" {}
		_A_MS("A_MS", 2D) = "black" {}
		[Normal]_A_NormalMap("A_NormalMap", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _PrimaryNormalMap;
		uniform float4 _PrimaryNormalMap_ST;
		uniform sampler2D _R_NormalMap;
		uniform float4 _R_NormalMap_ST;
		uniform sampler2D _B_NormalMap;
		uniform float4 _B_NormalMap_ST;
		uniform sampler2D _G_NormalMap;
		uniform float4 _G_NormalMap_ST;
		uniform sampler2D _A_NormalMap;
		uniform float4 _A_NormalMap_ST;
		uniform sampler2D _PrimaryAlbedoWhite;
		uniform float4 _PrimaryAlbedoWhite_ST;
		uniform sampler2D _R_Albedo;
		uniform float4 _R_Albedo_ST;
		uniform sampler2D _B_Albedo;
		uniform float4 _B_Albedo_ST;
		uniform sampler2D _G_Albedo;
		uniform float4 _G_Albedo_ST;
		uniform sampler2D _A_Albedo;
		uniform float4 _A_Albedo_ST;
		uniform sampler2D _Primary_MS;
		uniform float4 _Primary_MS_ST;
		uniform sampler2D _R_MS;
		uniform float4 _R_MS_ST;
		uniform sampler2D _B_MS;
		uniform float4 _B_MS_ST;
		uniform sampler2D _G_MS;
		uniform float4 _G_MS_ST;
		uniform sampler2D _A_MS;
		uniform float4 _A_MS_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_PrimaryNormalMap = i.uv_texcoord * _PrimaryNormalMap_ST.xy + _PrimaryNormalMap_ST.zw;
			float2 uv_R_NormalMap = i.uv_texcoord * _R_NormalMap_ST.xy + _R_NormalMap_ST.zw;
			float temp_output_2_0 = ( 1.0 - i.vertexColor.r );
			float3 lerpResult5 = lerp( UnpackNormal( tex2D( _PrimaryNormalMap, uv_PrimaryNormalMap ) ) , UnpackNormal( tex2D( _R_NormalMap, uv_R_NormalMap ) ) , temp_output_2_0);
			float2 uv_B_NormalMap = i.uv_texcoord * _B_NormalMap_ST.xy + _B_NormalMap_ST.zw;
			float temp_output_7_0 = ( 1.0 - i.vertexColor.g );
			float3 lerpResult40 = lerp( lerpResult5 , UnpackNormal( tex2D( _B_NormalMap, uv_B_NormalMap ) ) , temp_output_7_0);
			float2 uv_G_NormalMap = i.uv_texcoord * _G_NormalMap_ST.xy + _G_NormalMap_ST.zw;
			float temp_output_10_0 = ( 1.0 - i.vertexColor.b );
			float3 lerpResult41 = lerp( lerpResult40 , UnpackNormal( tex2D( _G_NormalMap, uv_G_NormalMap ) ) , temp_output_10_0);
			float2 uv_A_NormalMap = i.uv_texcoord * _A_NormalMap_ST.xy + _A_NormalMap_ST.zw;
			float temp_output_13_0 = ( 1.0 - i.vertexColor.a );
			float3 lerpResult42 = lerp( lerpResult41 , UnpackNormal( tex2D( _A_NormalMap, uv_A_NormalMap ) ) , temp_output_13_0);
			float3 normalizeResult12 = normalize( lerpResult42 );
			o.Normal = normalizeResult12;
			float2 uv_PrimaryAlbedoWhite = i.uv_texcoord * _PrimaryAlbedoWhite_ST.xy + _PrimaryAlbedoWhite_ST.zw;
			float2 uv_R_Albedo = i.uv_texcoord * _R_Albedo_ST.xy + _R_Albedo_ST.zw;
			float4 lerpResult11 = lerp( tex2D( _PrimaryAlbedoWhite, uv_PrimaryAlbedoWhite ) , tex2D( _R_Albedo, uv_R_Albedo ) , temp_output_2_0);
			float2 uv_B_Albedo = i.uv_texcoord * _B_Albedo_ST.xy + _B_Albedo_ST.zw;
			float4 lerpResult36 = lerp( lerpResult11 , tex2D( _B_Albedo, uv_B_Albedo ) , temp_output_7_0);
			float2 uv_G_Albedo = i.uv_texcoord * _G_Albedo_ST.xy + _G_Albedo_ST.zw;
			float4 lerpResult37 = lerp( lerpResult36 , tex2D( _G_Albedo, uv_G_Albedo ) , temp_output_10_0);
			float2 uv_A_Albedo = i.uv_texcoord * _A_Albedo_ST.xy + _A_Albedo_ST.zw;
			float4 lerpResult38 = lerp( lerpResult37 , tex2D( _A_Albedo, uv_A_Albedo ) , temp_output_13_0);
			o.Albedo = lerpResult38.rgb;
			float2 uv_Primary_MS = i.uv_texcoord * _Primary_MS_ST.xy + _Primary_MS_ST.zw;
			float2 uv_R_MS = i.uv_texcoord * _R_MS_ST.xy + _R_MS_ST.zw;
			float4 lerpResult8 = lerp( tex2D( _Primary_MS, uv_Primary_MS ) , tex2D( _R_MS, uv_R_MS ) , temp_output_2_0);
			float2 uv_B_MS = i.uv_texcoord * _B_MS_ST.xy + _B_MS_ST.zw;
			float4 lerpResult45 = lerp( lerpResult8 , tex2D( _B_MS, uv_B_MS ) , temp_output_7_0);
			float2 uv_G_MS = i.uv_texcoord * _G_MS_ST.xy + _G_MS_ST.zw;
			float4 lerpResult46 = lerp( lerpResult45 , tex2D( _G_MS, uv_G_MS ) , temp_output_10_0);
			float2 uv_A_MS = i.uv_texcoord * _A_MS_ST.xy + _A_MS_ST.zw;
			float4 lerpResult47 = lerp( lerpResult46 , tex2D( _A_MS, uv_A_MS ) , temp_output_13_0);
			o.Metallic = (lerpResult47).r;
			o.Smoothness = (lerpResult47).a;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
99;330;1347;532;5524.376;-465.434;5.172072;True;False
Node;AmplifyShaderEditor.CommentaryNode;44;-3854.919,610.2107;Inherit;False;496.394;381.0217;Masques;5;1;2;7;10;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;50;-2633.762,1616.015;Inherit;False;1411.893;1300.038;MetallicSmoothness;9;46;45;8;17;25;19;22;34;47;;1,1,1,1;0;0
Node;AmplifyShaderEditor.VertexColorNode;1;-3804.919,681.1279;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;43;-2641.403,139.8703;Inherit;False;1416.606;1294.749;Normal;9;18;15;5;40;24;33;21;41;42;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;39;-2640.156,-1236.046;Inherit;False;1400.474;1272.187;Albedo;9;38;37;36;11;26;32;23;20;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;15;-2588.37,189.8704;Inherit;True;Property;_PrimaryNormalMap;Primary NormalMap;2;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;2;-3539.881,660.2106;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;18;-2591.206,432.2177;Inherit;True;Property;_R_NormalMap;R_NormalMap;5;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;17;-2579.516,1666.015;Inherit;True;Property;_Primary_MS;Primary_MS;1;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-2583.762,1923.12;Inherit;True;Property;_R_MS;R_MS;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;8;-2176.313,1902.067;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;22;-2579.948,2182.464;Inherit;True;Property;_B_MS;B_MS;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;7;-3539.964,731.5491;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;16;-2590.156,-1186.046;Inherit;True;Property;_PrimaryAlbedoWhite;Primary Albedo (White);0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;21;-2591.403,690.0886;Inherit;True;Property;_B_NormalMap;B_NormalMap;8;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;5;-2182.855,421.2664;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;20;-2588.408,-941.0856;Inherit;True;Property;_R_Albedo;R_Albedo;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;11;-2173.886,-964.1276;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;23;-2589.047,-682.2889;Inherit;True;Property;_B_Albedo;B_Albedo;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;40;-1927.024,672.1761;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;45;-1917.31,2162.489;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;33;-2590.911,950.0844;Inherit;True;Property;_G_NormalMap;G_NormalMap;11;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;-2582.179,2434.324;Inherit;True;Property;_G_MS;G_MS;10;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;10;-3537.525,802.2347;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;13;-3538.746,880.2324;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;24;-2591.326,1204.62;Inherit;True;Property;_A_NormalMap;A_NormalMap;14;1;[Normal];Create;True;0;0;False;0;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;41;-1667.864,923.7144;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;36;-1919.174,-715.6741;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;25;-2582.526,2686.053;Inherit;True;Property;_A_MS;A_MS;13;0;Create;True;0;0;False;0;-1;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;46;-1658.025,2409.079;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;32;-2589.965,-426.0384;Inherit;True;Property;_G_Albedo;G_Albedo;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;37;-1659.563,-452.9619;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;42;-1406.797,1179.064;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;26;-2588.595,-175.356;Inherit;True;Property;_A_Albedo;A_ Albedo;12;0;Create;True;0;0;False;0;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;47;-1403.869,2660.944;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;38;-1406.644,-205.1987;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;48;-1001.311,707.3087;Inherit;False;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;49;-996.5276,822.0699;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;12;-990.7545,615.0967;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-759.6597,564.8177;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Shader_Blending;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;1
WireConnection;8;0;17;0
WireConnection;8;1;19;0
WireConnection;8;2;2;0
WireConnection;7;0;1;2
WireConnection;5;0;15;0
WireConnection;5;1;18;0
WireConnection;5;2;2;0
WireConnection;11;0;16;0
WireConnection;11;1;20;0
WireConnection;11;2;2;0
WireConnection;40;0;5;0
WireConnection;40;1;21;0
WireConnection;40;2;7;0
WireConnection;45;0;8;0
WireConnection;45;1;22;0
WireConnection;45;2;7;0
WireConnection;10;0;1;3
WireConnection;13;0;1;4
WireConnection;41;0;40;0
WireConnection;41;1;33;0
WireConnection;41;2;10;0
WireConnection;36;0;11;0
WireConnection;36;1;23;0
WireConnection;36;2;7;0
WireConnection;46;0;45;0
WireConnection;46;1;34;0
WireConnection;46;2;10;0
WireConnection;37;0;36;0
WireConnection;37;1;32;0
WireConnection;37;2;10;0
WireConnection;42;0;41;0
WireConnection;42;1;24;0
WireConnection;42;2;13;0
WireConnection;47;0;46;0
WireConnection;47;1;25;0
WireConnection;47;2;13;0
WireConnection;38;0;37;0
WireConnection;38;1;26;0
WireConnection;38;2;13;0
WireConnection;48;0;47;0
WireConnection;49;0;47;0
WireConnection;12;0;42;0
WireConnection;0;0;38;0
WireConnection;0;1;12;0
WireConnection;0;3;48;0
WireConnection;0;4;49;0
ASEEND*/
//CHKSM=AFBAA381FF498750A9563F30A59AA2B6FE93F3AC