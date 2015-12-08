using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ShaderGUIStencilMask : ShaderGUI
	{		
		MaterialEditor 		m_MaterialEditor;	

		MaterialProperty 	m_QueueOffset 		= null;
		MaterialProperty	m_MainTex			= null;
		MaterialProperty	m_AlphaTest			= null;
		MaterialProperty	m_StencilID			= null;
		
		bool 				m_FirstTimeApply 	= true;
		
		public void FindProperties (MaterialProperty[] props)
		{
			m_MainTex 		= FindProperty ("_MainTex", props);	
			m_AlphaTest 	= FindProperty ("_AlphaTest", props);		
			m_QueueOffset 	= FindProperty ("_QueueOffset", props);		
			m_StencilID 	= FindProperty ("_StencilID", props);	
		}
		
		public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] props)
		{
			// MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
			FindProperties (props);
			
			m_MaterialEditor = materialEditor;
			Material material = materialEditor.target as Material;
			
			ShaderPropertiesGUI (material);
			
			// Make sure that needed keywords are set up if we're switching some existing material to a standard shader.
			if (m_FirstTimeApply)
			{
				SetMaterialKeywords (material);
				m_FirstTimeApply = false;
			}
		}
		
		public void ShaderPropertiesGUI (Material material)
		{
			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0f;
			
			// Detect any changes to the material
			EditorGUI.BeginChangeCheck();
			{				
				m_MaterialEditor.TextureProperty(m_MainTex, "Base (RGBA");

				m_MaterialEditor.ShaderProperty(m_AlphaTest, "AlphaTest");
				
				m_MaterialEditor.ShaderProperty(m_StencilID, "Stencil ID:");
				
				EditorGUILayout.Space();
				
				GUILayout.Label ("Custom Render Queue: " + material.renderQueue.ToString(), EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(m_QueueOffset, "Queue Offset (int)");
				
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				foreach (var obj in m_AlphaTest.targets)
					MaterialChanged((Material)obj);
			}
		}

		
		public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
		{
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
			
			if (oldShader == null ) return;

			MaterialChanged(material);
		}
		
		void SetMaterialKeywords(Material material)
		{
			// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
			// (MaterialProperty value might come from renderer material property block)
			
			// SetKeyword (material, "_MULTIPLYMODE", ( (BlendMode)material.GetFloat("_Mode") != BlendMode.AlphaBlending ) );
		}	
		
		
		void MaterialChanged(Material material)
		{
			int offsetInt = Mathf.FloorToInt( material.GetFloat("_QueueOffset") );
			material.renderQueue = 1000 + offsetInt;			

			// Debug.LogFormat("Mat: {0} Queue: {1}  Offset: {2}",material.name, material.renderQueue, offsetInt);
			
			SetMaterialKeywords(material);
		}
		
		void SetKeyword(Material m, string keyword, bool state)
		{
			if (state)
				m.EnableKeyword (keyword);
			else
				m.DisableKeyword (keyword);
		}
		
	}
	
} // namespace UnityEditor
