// Basic Multiply shader - not much use, just kept for reference, shouldn't be used.

Shader "CL/WaterColor/MultiplyBasic"
 {
    Properties 
    {
        _MainTex ("Texture to blend", 2D) = "black" {}
    }
    
    SubShader 
    {
        Tags { "Queue" = "Transparent" }
        
        Pass 
        {
            // Mode:	Blend SrcFactor DstFactor, SrcFactorA DstFactorA:
            // Factors: SrcAlpha, DstAlpha, One, Zero, OneMinusSrcAlpha, OneMinusDstAlpha, SrcColor, DstColor
            
            // Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha One
           
            // Working
         //   Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
            
           	Blend DstColor Zero, SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            SetTexture [_MainTex] { combine texture }
        }
    }
}   

 
// Blend One One  // Additive
// Blend SrcAlpha One
// Blend DstAlpha SrcAlpha
// Blend SrcAlpha One, SrcAlpha One