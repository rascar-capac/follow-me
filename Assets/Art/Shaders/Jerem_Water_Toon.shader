// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Shader_Water_Toon"
{
	Properties
	{
		[NoScaleOffset][Normal]_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalTiling("NormalTiling", Float) = 1
		_NormalScale("Normal Scale", Float) = 1
		_NormalsSpeed("NormalsSpeed", Vector) = (-0.03,0,0.04,0.04)
		[NoScaleOffset]_HeightMap("HeightMap", 2D) = "gray" {}
		_HeightTiling("HeightTiling", Float) = 1
		_HeightsSpeed("HeightsSpeed", Vector) = (-0.03,0,0.04,0.04)
		_HeightsIntensity("HeightsIntensity", Float) = 1
		_DeepColor("Deep Color", Color) = (0,0,0,0)
		_ShalowColor("Shalow Color", Color) = (1,1,1,0)
		_WaterDepth("Water Depth", Float) = 0.99
		_WaterFalloff("Water Falloff", Float) = -3
		[NoScaleOffset]_FoamMap("FoamMap", 2D) = "white" {}
		_FoamTiling("FoamTiling", Float) = 1
		_FoamDepth("Foam Depth", Float) = 0.94
		_FoamFalloff("Foam Falloff", Float) = -30
		_FoamSharpness("FoamSharpness", Range( 0 , 0.4999)) = 0
		_FoamThickness("FoamThickness", Range( -0.5 , 0.499)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform sampler2D _HeightMap;
		uniform float4 _HeightsSpeed;
		uniform float _HeightTiling;
		uniform float _HeightsIntensity;
		uniform sampler2D _NormalMap;
		uniform float _NormalScale;
		uniform float4 _NormalsSpeed;
		uniform float _NormalTiling;
		uniform float4 _DeepColor;
		uniform float4 _ShalowColor;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _WaterDepth;
		uniform float _WaterFalloff;
		uniform float _FoamSharpness;
		uniform float _FoamDepth;
		uniform float _FoamFalloff;
		uniform float _FoamThickness;
		uniform sampler2D _FoamMap;
		uniform float _FoamTiling;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 appendResult87 = (float4(_HeightsSpeed.x , _HeightsSpeed.y , 0.0 , 0.0));
			float2 temp_cast_1 = (_HeightTiling).xx;
			float2 uv_TexCoord39 = v.texcoord.xy * temp_cast_1;
			float2 panner41 = ( 1.0 * _Time.y * appendResult87.xy + uv_TexCoord39);
			float4 appendResult88 = (float4(_HeightsSpeed.z , _HeightsSpeed.w , 0.0 , 0.0));
			float2 panner40 = ( 1.0 * _Time.y * appendResult88.xy + uv_TexCoord39);
			float4 appendResult56 = (float4(0.0 , _HeightsIntensity , 0.0 , 0.0));
			v.vertex.xyz += ( saturate( ( tex2Dlod( _HeightMap, float4( panner41, 0, 0.0) ) * tex2Dlod( _HeightMap, float4( panner40, 0, 0.0) ) ) ) * appendResult56 ).rgb;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult70 = (float4(_NormalsSpeed.x , _NormalsSpeed.y , 0.0 , 0.0));
			float2 temp_cast_1 = (_NormalTiling).xx;
			float2 uv_TexCoord6 = i.uv_texcoord * temp_cast_1;
			float2 panner8 = ( 1.0 * _Time.y * appendResult70.xy + uv_TexCoord6);
			float4 appendResult71 = (float4(_NormalsSpeed.z , _NormalsSpeed.w , 0.0 , 0.0));
			float2 panner7 = ( 1.0 * _Time.y * appendResult71.xy + uv_TexCoord6);
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, panner8 ), _NormalScale ) , UnpackScaleNormal( tex2D( _NormalMap, panner7 ), _NormalScale ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth3 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float temp_output_5_0 = abs( ( eyeDepth3 - ase_screenPos.w ) );
			float4 lerpResult21 = lerp( _DeepColor , _ShalowColor , saturate( pow( ( temp_output_5_0 + _WaterDepth ) , _WaterFalloff ) ));
			float temp_output_35_0 = saturate( pow( ( temp_output_5_0 + _FoamDepth ) , _FoamFalloff ) );
			float smoothstepResult77 = smoothstep( _FoamSharpness , ( 1.0 - _FoamSharpness ) , ( temp_output_35_0 + _FoamThickness ));
			float2 temp_cast_3 = (_FoamTiling).xx;
			float2 uv_TexCoord30 = i.uv_texcoord * temp_cast_3;
			float2 panner32 = ( 1.0 * _Time.y * float2( -0.01,0.01 ) + uv_TexCoord30);
			float4 lerpResult38 = lerp( lerpResult21 , float4(1,1,1,0) , ( smoothstepResult77 + ( temp_output_35_0 * tex2D( _FoamMap, panner32 ).r ) ));
			o.Albedo = lerpResult38.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
140;430;1347;532;2460.398;-1827.872;1.247562;True;False
Node;AmplifyShaderEditor.CommentaryNode;24;-2838.234,60.66348;Inherit;False;821.7407;445.6608;Comment;5;5;4;3;1;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;1;-2788.234,110.6635;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScreenDepthNode;3;-2559.091,131.8242;Inherit;False;0;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-2784.09,294.3242;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;27;-2542.16,1394.344;Inherit;False;1695.321;588.8805;Foam controls and texture;15;64;37;34;74;35;32;33;30;29;31;28;77;81;73;82;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;4;-2351.69,177.1245;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;5;-2166.493,174.9405;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-2524.767,1512.285;Float;False;Property;_FoamDepth;Foam Depth;14;0;Create;True;0;0;False;0;0.94;0.94;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;46;-2428.681,2053.235;Inherit;False;1334.603;525.8979;Comment;7;39;40;41;44;45;84;85;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;25;-1930.609,-46.59945;Inherit;False;1106.201;504.3004;Comment;10;14;15;16;17;18;19;20;21;23;13;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-2240.259,1444.344;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-2772.104,1710.436;Inherit;False;Property;_FoamTiling;FoamTiling;13;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-2681.114,2155.739;Inherit;False;Property;_HeightTiling;HeightTiling;5;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;86;-2868.918,2360.023;Inherit;False;Property;_HeightsSpeed;HeightsSpeed;6;0;Create;True;0;0;False;0;-0.03,0,0.04,0.04;-0.03,0,0.04,0.04;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-2229.66,1580.145;Float;False;Property;_FoamFalloff;Foam Falloff;15;0;Create;True;0;0;False;0;-30;-60;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;88;-2553.488,2480.065;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;87;-2543.27,2303.833;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-2492.16,1691.745;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;39;-2409.958,2155.54;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;33;-2055.46,1453.245;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1880.609,289.8451;Float;False;Property;_WaterDepth;Water Depth;10;0;Create;True;0;0;False;0;0.99;0.99;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;32;-2239.672,1709.958;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.01,0.01;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-2886.883,751.098;Inherit;False;Property;_NormalTiling;NormalTiling;1;0;Create;True;0;0;False;0;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1703.109,341.701;Float;False;Property;_WaterFalloff;Water Falloff;11;0;Create;True;0;0;False;0;-3;-3.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;69;-3081.569,957.7964;Inherit;False;Property;_NormalsSpeed;NormalsSpeed;3;0;Create;True;0;0;False;0;-0.03,0,0.04,0.04;-0.03,0,0.04,0.04;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;26;-2625.267,620.6346;Inherit;False;1334.603;498.1993;Comment;7;6;7;8;9;10;11;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-1853.332,1452.642;Inherit;False;Property;_FoamThickness;FoamThickness;17;0;Create;True;0;0;False;0;0;0;-0.5;0.499;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1678.652,1586.565;Inherit;False;Property;_FoamSharpness;FoamSharpness;16;0;Create;True;0;0;False;0;0;0;0;0.4999;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;40;-2086.375,2313.225;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;15;-1698.914,216.3185;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;35;-1882.483,1543.377;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;41;-2105.981,2103.235;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;17;-1764.409,3.400575;Float;False;Property;_DeepColor;Deep Color;8;0;Create;True;0;0;False;0;0,0,0,0;0,0.04310164,0.2499982,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;45;-1760.602,2344.133;Inherit;True;Property;_HeightMap;HeightMap;4;1;[NoScaleOffset];Create;True;0;0;False;0;-1;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;81;-1442.448,1487.064;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;-2018.659,1668.545;Inherit;True;Property;_FoamMap;FoamMap;12;1;[NoScaleOffset];Create;True;0;0;False;0;-1;None;d01457b88b1c5174ea4235d140b5fab8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;74;-1417.767,1685.983;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;44;-1759.415,2104.131;Inherit;True;Property;_Height2;Height2;4;0;Create;True;0;0;False;0;-1;None;None;True;0;True;gray;Auto;False;Instance;45;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;23;-1522.009,92.60134;Float;False;Property;_ShalowColor;Shalow Color;9;0;Create;True;0;0;False;0;1,1,1,0;0,0.8088232,0.8088235,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;16;-1522.715,302.718;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;71;-2795.194,1059.763;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-2775.668,840.6433;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-2575.267,697.9337;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-2286.669,956.1765;Float;False;Property;_NormalScale;Normal Scale;2;0;Create;True;0;0;False;0;1;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;19;-1217.916,66.31792;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;7;-2288.267,803.1337;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.04,0.04;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;77;-1237.694,1529.717;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.16;False;2;FLOAT;0.36;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1392.651,2227.21;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;20;-1216.016,158.9179;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;8;-2285.567,668.6347;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.03,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;18;-1316.413,323.9173;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1679.058,1746.054;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1209.767,2458.297;Inherit;False;Property;_HeightsIntensity;HeightsIntensity;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-1006.408,200.2014;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;64;-989.5469,1746.025;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-1952,880;Inherit;True;Property;_NormalMap;NormalMap;0;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;-1;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;85;-1212.588,2246.366;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;11;-1952,672;Inherit;True;Property;_Normal2;Normal2;0;0;Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Instance;10;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;56;-976.6179,2428.752;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;36;-1083.382,997.6052;Float;False;Constant;_Color0;Color 0;-1;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendNormalsNode;12;-1518.664,823.4337;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;38;-517.0131,835.7632;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-766.8021,2103.435;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-150.1387,962.211;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Shader_Water_Toon;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;1;0
WireConnection;4;0;3;0
WireConnection;4;1;2;4
WireConnection;5;0;4;0
WireConnection;29;0;5;0
WireConnection;29;1;28;0
WireConnection;88;0;86;3
WireConnection;88;1;86;4
WireConnection;87;0;86;1
WireConnection;87;1;86;2
WireConnection;30;0;53;0
WireConnection;39;0;54;0
WireConnection;33;0;29;0
WireConnection;33;1;31;0
WireConnection;32;0;30;0
WireConnection;40;0;39;0
WireConnection;40;2;88;0
WireConnection;15;0;5;0
WireConnection;15;1;13;0
WireConnection;35;0;33;0
WireConnection;41;0;39;0
WireConnection;41;2;87;0
WireConnection;45;1;40;0
WireConnection;81;0;35;0
WireConnection;81;1;82;0
WireConnection;34;1;32;0
WireConnection;74;0;73;0
WireConnection;44;1;41;0
WireConnection;16;0;15;0
WireConnection;16;1;14;0
WireConnection;71;0;69;3
WireConnection;71;1;69;4
WireConnection;70;0;69;1
WireConnection;70;1;69;2
WireConnection;6;0;48;0
WireConnection;19;0;17;0
WireConnection;7;0;6;0
WireConnection;7;2;71;0
WireConnection;77;0;81;0
WireConnection;77;1;73;0
WireConnection;77;2;74;0
WireConnection;84;0;44;0
WireConnection;84;1;45;0
WireConnection;20;0;23;0
WireConnection;8;0;6;0
WireConnection;8;2;70;0
WireConnection;18;0;16;0
WireConnection;37;0;35;0
WireConnection;37;1;34;1
WireConnection;21;0;19;0
WireConnection;21;1;20;0
WireConnection;21;2;18;0
WireConnection;64;0;77;0
WireConnection;64;1;37;0
WireConnection;10;1;7;0
WireConnection;10;5;9;0
WireConnection;85;0;84;0
WireConnection;11;1;8;0
WireConnection;11;5;9;0
WireConnection;56;1;50;0
WireConnection;12;0;11;0
WireConnection;12;1;10;0
WireConnection;38;0;21;0
WireConnection;38;1;36;0
WireConnection;38;2;64;0
WireConnection;49;0;85;0
WireConnection;49;1;56;0
WireConnection;0;0;38;0
WireConnection;0;1;12;0
WireConnection;0;11;49;0
ASEEND*/
//CHKSM=32F0C5A8BCEDD75DBE1F0C809C1518BDFA753BD1