// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "_SheeroChana/DayNight_Opaque"
{
	Properties
	{
		_Day_Texture("Day_Texture", 2D) = "white" {}
		_DayColor("DayColor", Color) = (1,1,1,0)
		_Night_Texture("Night_Texture", 2D) = "white" {}
		_NightColor("NightColor", Color) = (1,1,1,0)
		_DayNightAlbedo("DayNightAlbedo", Range( 0 , 1)) = 0
		_Day_EmissiveTexture("Day_EmissiveTexture", 2D) = "white" {}
		_Day_EmissiveColor("Day_EmissiveColor", Color) = (0,0,0,0)
		_Night_EmissiveTexture("Night_EmissiveTexture", 2D) = "white" {}
		_Night_EmissiveColor("Night_EmissiveColor", Color) = (0,0,0,0)
		_DayNightEmissive("DayNightEmissive", Range( 0 , 1)) = 0
		_Normal("Normal", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.2
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

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
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float2 uv_Day_Texture = i.uv_texcoord * _Day_Texture_ST.xy + _Day_Texture_ST.zw;
			float4 tex2DNode69 = tex2D( _Day_Texture, uv_Day_Texture );
			float2 uv_Night_Texture = i.uv_texcoord * _Night_Texture_ST.xy + _Night_Texture_ST.zw;
			float4 lerpResult79 = lerp( ( tex2DNode69 * _DayColor ) , ( tex2D( _Night_Texture, uv_Night_Texture ) * _NightColor ) , _DayNightAlbedo);
			o.Albedo = lerpResult79.rgb;
			float2 uv_Day_EmissiveTexture = i.uv_texcoord * _Day_EmissiveTexture_ST.xy + _Day_EmissiveTexture_ST.zw;
			float2 uv_Night_EmissiveTexture = i.uv_texcoord * _Night_EmissiveTexture_ST.xy + _Night_EmissiveTexture_ST.zw;
			float4 lerpResult80 = lerp( ( tex2D( _Day_EmissiveTexture, uv_Day_EmissiveTexture ) * _Day_EmissiveColor ) , ( tex2D( _Night_EmissiveTexture, uv_Night_EmissiveTexture ) * _Night_EmissiveColor ) , _DayNightEmissive);
			o.Emission = lerpResult80.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			o.Occlusion = tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17500
21;162;1899;829;2472.971;724.1979;1.841173;True;True
Node;AmplifyShaderEditor.CommentaryNode;65;-2425.174,174.1225;Inherit;False;639.2007;482.5995;Comment;3;76;68;66;NUIT - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;64;-2427.702,-397.9657;Inherit;False;639.2007;482.5995;Comment;3;75;73;67;JOUR - Em;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;62;-2486.321,-1103.835;Inherit;False;736.3441;511.4706;Comment;3;78;72;70;NUIT Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;63;-2497.195,-1706.228;Inherit;False;737.939;439.7025;Comment;3;77;71;69;JOUR Al;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;72;-2397.665,-815.875;Inherit;False;Property;_NightColor;NightColor;3;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;68;-2312.823,449.3096;Inherit;False;Property;_Night_EmissiveColor;Night_EmissiveColor;8;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-2372.612,223.7096;Inherit;True;Property;_Night_EmissiveTexture;Night_EmissiveTexture;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;67;-2317.915,-122.3646;Inherit;False;Property;_Day_EmissiveColor;Day_EmissiveColor;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;73;-2377.702,-346.9658;Inherit;True;Property;_Day_EmissiveTexture;Day_EmissiveTexture;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;71;-2411.156,-1444.878;Inherit;False;Property;_DayColor;DayColor;1;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;70;-2418.925,-1023.505;Inherit;True;Property;_Night_Texture;Night_Texture;2;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;69;-2460.302,-1646.51;Inherit;True;Property;_Day_Texture;Day_Texture;0;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-1937.426,-900.7969;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1570.928,-887.4219;Inherit;False;Property;_DayNightAlbedo;DayNightAlbedo;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-1485.318,326.8999;Inherit;False;Property;_DayNightEmissive;DayNightEmissive;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-1974.923,-1523.802;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-1951.864,-180.5837;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-1946.769,391.091;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-458.3618,200.1088;Inherit;False;Property;_Metallic;Metallic;12;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;59;-472.7239,387.6776;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;13;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-454.6244,-99.76092;Inherit;True;Property;_Normal;Normal;10;0;Create;True;0;0;False;0;-1;07e7dcfe4180f0649979fed77a3e9429;07e7dcfe4180f0649979fed77a3e9429;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;80;-1175.24,59.11739;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-468.0407,298.6811;Inherit;False;Property;_Smoothness;Smoothness;11;0;Create;True;0;0;False;0;0.2;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;79;-1229.175,-1110.337;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;75.16439,28.0274;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;_SheeroChana/DayNight_Opaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;True;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;31;19;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;78;0;70;0
WireConnection;78;1;72;0
WireConnection;77;0;69;0
WireConnection;77;1;71;0
WireConnection;75;0;73;0
WireConnection;75;1;67;0
WireConnection;76;0;66;0
WireConnection;76;1;68;0
WireConnection;80;0;75;0
WireConnection;80;1;76;0
WireConnection;80;2;81;0
WireConnection;79;0;77;0
WireConnection;79;1;78;0
WireConnection;79;2;74;0
WireConnection;0;0;79;0
WireConnection;0;1;6;0
WireConnection;0;2;80;0
WireConnection;0;3;99;0
WireConnection;0;4;58;0
WireConnection;0;5;59;1
WireConnection;0;10;69;4
ASEEND*/
//CHKSM=38A42F01C09BEBA0A5F643AA67464B13BF28C876