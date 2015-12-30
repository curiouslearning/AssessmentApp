// Usage Guide
//
// This script will adjust the materials renderQueue on a per material basis.
// This enables a degree of control over the render order of gameObjects based on their Material.
// If a gameObject renderer is using a sharedMaterial then it will become non-shared.
//
// RenderQueue
// Unity uses the RenderQueue value to control the overall draw order of gameObjects.
// This RenderQueue value is by default set by the shader, relating queue tags to absolute values.
// e.g. geometry = 1000, transparent = 3000 etc
//
// By setting the RenderQueue of a Material it overrides the default setting of the shader.
// Once a Materials RenderQueue has been changed, it no longer respects the shader value regardless 
// of if you change the materials shader.
//
//
// m_QueueOffset:  
// This value represents the adjustment made to the materials default renderQueue val.
// Positive values mean this material is rendered AFTER any materials that use the same default renderQueue value.
// Negative values mean this material is rendered BEFORE any materials that use the same default renderQueue value.
// Generally you'll probably want to use negative values.



using UnityEngine;
using System.Collections;

public class MaterialRenderQueueOverride : MonoBehaviour 
{
	public	int			m_QueueOffset = 0;
	private	int			m_QueueDefaultIndex;
	private	Material	m_Mat;
	
	void Awake()
	{
		m_Mat 					= GetComponent<Renderer>().material;
		m_QueueDefaultIndex 	= m_Mat.renderQueue; 
		m_Mat.renderQueue 		= m_QueueDefaultIndex + m_QueueOffset;
		Debug.LogFormat("MaterialLayerIndex: {0} - {1} - Queue is {2}  Default: {3}  Offset: {4}", gameObject.name, m_Mat.name, m_Mat.renderQueue, m_QueueDefaultIndex, m_QueueOffset );
	}

}
