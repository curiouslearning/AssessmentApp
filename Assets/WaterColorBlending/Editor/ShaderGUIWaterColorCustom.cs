using System;
using UnityEngine;

namespace UnityEditor
{
	internal class ShaderGUIWaterColorCustom : ShaderGUI
	{	
		public enum BlendMode
		{
			AlphaBlending,
			Multiply,
			Randon 
		}


		private static class Styles
		{
			public static GUIContent 		guiMainTex 			= new GUIContent("Base (RGBA", 		"Base (RGBA)");
			public static GUIContent 		guiAlphaFactor 		= new GUIContent("AlphaFactor", 	"Scales Alpha value in shader");
			public static GUIContent 		guiQueueOffset 		= new GUIContent("Queue Offset (int)", 	"Queue Offset");
			public static string 			guiSubHeader		= "Custom Render Queue: ";
			public static string 			guiRenderingMode 	= "Rendering Mode";
			public static readonly string[] guiBlendNames 		= Enum.GetNames (typeof (BlendMode));
		}

		MaterialEditor 		m_MaterialEditor;	

		MaterialProperty 	m_BlendMode 		= null;
		MaterialProperty 	m_QueueOffset 		= null;
		MaterialProperty	m_MainTex			= null;
		MaterialProperty	m_AlphaFactor		= null;

		bool 				m_FirstTimeApply 	= true;
		
		public void FindProperties (MaterialProperty[] props)
		{
			m_BlendMode 	= FindProperty ("_Mode", props);
			m_MainTex 		= FindProperty ("_MainTex", props);	
			m_AlphaFactor 	= FindProperty ("_AlphaFactor", props);		
			m_QueueOffset 	= FindProperty ("_QueueOffset", props);	
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
				BlendModePopup();

				EditorGUILayout.Space();

				m_MaterialEditor.TextureProperty(m_MainTex, Styles.guiMainTex.text);

				if (((BlendMode)material.GetFloat("_Mode") != BlendMode.AlphaBlending))
					m_MaterialEditor.ShaderProperty(m_AlphaFactor, Styles.guiAlphaFactor.text);

				EditorGUILayout.Space();

				GUILayout.Label (Styles.guiSubHeader + material.renderQueue.ToString(), EditorStyles.boldLabel);
				m_MaterialEditor.ShaderProperty(m_QueueOffset, Styles.guiQueueOffset.text);

			}

			if (EditorGUI.EndChangeCheck())
			{
				foreach (var obj in m_BlendMode.targets)
					MaterialChanged((Material)obj);
			}
		}

		void BlendModePopup()
		{
			EditorGUI.showMixedValue = m_BlendMode.hasMixedValue;
			var mode = (BlendMode)m_BlendMode.floatValue;
			
			EditorGUI.BeginChangeCheck();
			mode = (BlendMode)EditorGUILayout.Popup(Styles.guiRenderingMode, (int)mode, Styles.guiBlendNames);

			if (EditorGUI.EndChangeCheck())
			{
				m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
				m_BlendMode.floatValue = (float)mode;
			}
			
			EditorGUI.showMixedValue = false;
		}


		public override void AssignNewShaderToMaterial (Material material, Shader oldShader, Shader newShader)
		{
			base.AssignNewShaderToMaterial(material, oldShader, newShader);
			
			if (oldShader == null ) return;

			BlendMode m_BlendMode = BlendMode.Multiply;

			material.SetFloat("_Mode", (float)m_BlendMode);
			MaterialChanged(material);
		}
			
		void SetMaterialKeywords(Material material)
		{
			// Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
			// (MaterialProperty value might come from renderer material property block)

			SetKeyword (material, "_MULTIPLYMODE", ( (BlendMode)material.GetFloat("_Mode") != BlendMode.AlphaBlending ) );
		}	


		void MaterialChanged(Material material)
		{
			int offsetInt = Mathf.FloorToInt( material.GetFloat("_QueueOffset") );
			material.renderQueue = 3000 + offsetInt;  // To show before After 'cutout' shaders use 2450

			switch ( (BlendMode)material.GetFloat("_Mode") )
			{
			case BlendMode.AlphaBlending:
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				break;
			case BlendMode.Multiply:
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				break;
			case BlendMode.Randon:
				material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
				break;
			}
			// Debug.LogFormat("Mat: {0} Queue: {1}  Offset: {2}  BlendMode: {3}",material.name, material.renderQueue, offsetInt,  (BlendMode)material.GetFloat("_Mode"));

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
