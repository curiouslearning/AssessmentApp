
// Notes:
// Multiple methods to use here
// 1. Use multiply blending and make non-visible parts of texture white instead of black. Not sure alpha is used.
// 2. Use Multiply blending and use custom shader code to lerp RGB to White where apha is black.


// Custom shader versions, requires 'WaterColorShaderGUI' Editor script for inspector.
// Enables setting of RenderQueueOffset
// Enables setting of RenderMode ( e.g. Multiply or alphaBlending)
// AlphaFactor - adjusts textures alpha for use in lerp to white function.

// Notes:
// Blend SrcAlpha OneMinusSrcAlpha 	// AlphaBlending				
// Blend DstColor Zero 				// Multiply


Shader "CL/WaterColor/Custom" 
{
	Properties 
	{
		_MainTex 		("Base (RGB) Trans (A)", 				2D) 				= "white" {}
		_AlphaFactor	("AlphaFactor",  						Range(0.0, 4.0)) 	= 1.5
		
		// Properties only available from ShaderGUI
		[HideInInspector] _QueueOffset	("QueueOffset (int)", 	Float)				= 0.0
		
			// Blending state	
		[HideInInspector] _Mode 		("__mode", 				Float) 				= 0.0
		[HideInInspector] _SrcBlend 	("__src", 				Float) 				= 1.0
		[HideInInspector] _DstBlend 	("__dst", 				Float) 				= 0.0
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		
	
		
		Pass 
		{  
			Blend [_SrcBlend] [_DstBlend]
			
			CGPROGRAM
				#pragma shader_feature _MULTIPLYMODE
				
				#pragma vertex vert
				#pragma fragment frag
								
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				sampler2D 	_MainTex;
				float4 		_MainTex_ST;				
				float		_AlphaFactor;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);					
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
					
					#if ( _MULTIPLYMODE )
					// col.rgb = lerp( col.rgb, fixed3(1,1,1), 1 - min( 1, col.a * _AlphaFactor));
					col.rgb = lerp( col.rgb, fixed3(1,1,1), 1 - (col.a * _AlphaFactor));
					#endif
					
					return col;
				}
			ENDCG
		}
	}

	CustomEditor "ShaderGUIWaterColorCustom"
}