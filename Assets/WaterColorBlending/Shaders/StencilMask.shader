Shader "CL/Stencil/MaskByAlpha" 
{
	// Requires 'ShaderGUIStencilMask' Editor script for inspector functionality!
	//
	// Shader that ONLY renders to stencil buffer, nothing rendered to Color or Depth Buffer!
	// Uses alphaTest value to discard pixels or write into the stencil buffer.
	// Stencil values written are used by subsequent shaders to determine if pixels should be rendered or not i.e. masked.
	
	Properties 
	{
		_MainTex 		("Base (RGB) Trans (A)", 	2D) 				= "white" {}
		_AlphaTest		("AlphaTest",  				Range(0.0, 1.0)) 	= 0.5
		_StencilID 		("Stencil ID (int)", 		Float) 				= 0
		
		// Properties only available from ShaderGUI
		[HideInInspector] _QueueOffset	("QueueOffset (int)", 	Float)	= 0.0
	}

	SubShader 
	{		
		Tags 		{ "Queue" = "Geometry-1" }  	// Write to the stencil buffer before drawing any geometry to the screen
		
	
		Pass
		{               
			Stencil
			{
				Ref [_StencilID]	// Set stencil value to one if we pass
				Comp always			// Don't compare, we are only interesting in writing
				Pass replace		// This tells it to set the value, this should always pass because nothing is writing to the depth buffer.
            }
            
            ColorMask 	0 			// Don't write to any colour channels
			ZWrite 		Off 		// Don't write to the Depth buffer
		
		
			CGPROGRAM
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
				float		_AlphaTest;
				
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
					 if (col.a < _AlphaTest) discard;			// Not optimal but no other way to write stencil mask based on alpha.
					return col;
				}
			ENDCG
		}
	}
	
	CustomEditor "ShaderGUIStencilMask"
}