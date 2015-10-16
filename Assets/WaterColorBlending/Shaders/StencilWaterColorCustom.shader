Shader "CL/Stencil/WaterColor/Custom" 
{
	// Requires 'ShaderGUIStencilWaterColorCustom' Editor script for inspector functionality!
	//
	// Customisable version of WaterColor Shader that supports
	// Setting of RenderQueueOffset
	// Setting of RenderMode ( e.g. Multiply or alphaBlending)
	// Setting of AlphaFactor - adjusts textures alpha for use in lerp to white function.
	// Setting stencil comparrision to control if rendered or not

	// Notes:
	// Blend SrcAlpha OneMinusSrcAlpha 	// AlphaBlending				
	// Blend DstColor Zero 				// Multiply

	Properties 
	{
		_MainTex 		("Base (RGB) Trans (A)", 				2D) 				= "white" {}
		_AlphaFactor	("AlphaFactor",  						Range(0.0, 4.0)) 	= 1.5
		_StencilID 		("Stencil ID", 							Float) 				= 0
		
		
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
				
		Pass 
		{
			Stencil
			{
				Ref [_StencilID]	// Set stencil value to one if we pass
				Comp equal			// Only pass pixel if the stencil equals the ref value.
			}
						
			Blend 	[_SrcBlend] [_DstBlend]
			ZWrite 	Off
			
			
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

	CustomEditor "ShaderGUIStencilWaterColorCustom"
}