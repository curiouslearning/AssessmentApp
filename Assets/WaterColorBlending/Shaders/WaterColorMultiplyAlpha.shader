
// WaterColor - MultiplyAlpha
// This shader will require the script 'MaterialRenderQueueOverride' applied to the gameobject to control RenderQueue.
// Does not include stencil options, those could be added if necessary

Shader "CL/WaterColor/MultiplyAlpha" 
{
	Properties 
	{
		_MainTex 		("Base (RGB) Trans (A)", 	2D) 				= "white" {}
		_AlphaFactor	("AlphaFactor",  			Range(0.0, 4.0)) 	= 1.5
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		// Blend SrcAlpha OneMinusSrcAlpha 	// Alpha blending
		// Blend SrcAlpha One
		Blend DstColor Zero 				// Multiply
		
		Pass {  
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
					col.rgb = lerp( col.rgb, fixed3(1,1,1), 1 - col.a * _AlphaFactor);
					return col;
				}
			ENDCG
		}
	}
}