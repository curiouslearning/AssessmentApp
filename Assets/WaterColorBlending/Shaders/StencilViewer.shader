Shader "CL/Stencil/ViewByID" 
{	
	// Shader that will render/higlight a specific stencil buffer value using the supplied color.
	// Use this to debug/view stencil buffer values.
	
	Properties 
	{
		_Color 		("Main Color", 	Color) 	= (1,1,1,1)
		_StencilID 	("Stencil ID", 	Float) 	= 0
		
	}

	SubShader 
	{		
		Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent" }		
	
		Pass
		{               
			Stencil
			{
				Ref [_StencilID]	// Set stencil value to one if we pass
				Comp equal			// Only pass pixel if the stencil equals the ref value.
            }
                        
			ZWrite 		Off 		// Don't write to the Depth buffer
		
		
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
								
				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
				};

				float4 		_Color;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);					
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					return _Color;
				}
			ENDCG
		}
	}
}