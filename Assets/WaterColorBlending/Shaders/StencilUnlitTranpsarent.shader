Shader "CL/Stencil/Unlit/Transparent" 
{
	// A version of Unity's Unlit Transparent that supports stencil comparrision.
	// Fog support has been disabled, can be re-enabled by removing comments, though unsure what the effect will be.

	// Unlit alpha-blended shader.
	// no lighting, no lightmap support, no per-material color

	Properties 
	{
		_MainTex 		("Base (RGB) Trans (A)", 	2D) 	= "white" {}		
		_StencilID 		("Stencil ID", 				Float) 	= 0
	}

	SubShader
	 {
		Tags 	{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 	100
		     
		Pass 
		{  			
			Stencil
			{			
				Ref [_StencilID]	// Compare with the stencil value.
				Comp equal			// Only pass pixel if the stencil equals the ref value.
			}
			
			Blend 	SrcAlpha OneMinusSrcAlpha
			ZWrite 	Off	
			
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			//	#pragma multi_compile_fog
				
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
					//UNITY_FOG_COORDS(1)
				};

				sampler2D 	_MainTex;
				float4 		_MainTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				//	UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.texcoord);
				//	UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
			ENDCG
		}
	}

}
