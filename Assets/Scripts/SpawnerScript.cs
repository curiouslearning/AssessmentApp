using UnityEngine;
using System.Collections;


/// <summary>
/// SpawnerScript
/// Class used to initialize Stimulus Organization Object (SOO) and stimuli for each new question.
/// </summary>
// Gets question information from Question Queue.
// Gets SOO and stimulus positioning info from Editor.
public class SpawnerScript : MonoBehaviour {
	GameObject newSoo;
	GameObject[] newStims;
	Sprite[] newOptions;
	public GameObject sooPrefab;
	public GameObject stimPrefab;
	//placement modifiers for stimulus positions
	public float left;
	public float right;
	public float up;
	public float down;
	//variables for the SOO to hold onto
	public Vector3[] destinations;
	Vector3[] positions;
	public AnimationManager host;
	// Use this for initialization
	void Awake () {
		newStims = new GameObject[4];
		positions = new Vector3[4]; 
	}

	/// <summary>
	/// Randomly create a question or customization event based on the category and current difficulty level
	///</summary>	
	///<returns>Reference to Initialized SOO</returns>
	///<param name="cat">Category of Question</param>
	///<param name="difLevel">Difficulty level of stimuli </param>
	public GameObject spawnNext (Category cat, Difficulty difLevel, int questionNumber)
	{
		Question q =  Question.CreateInstance<Question>();
		//if Category.Customization, call findCustomizationOptions
		//if(cat == Category.Customization)
		//{
			q.init(questionNumber, host.getOptions(), host.getBodyPart());
			return spawnNext(q);
		//}
		//else {
		//	q.init (questionNumber, findSprites(), findSounds(), target, cat
		//}
		//else call findStimuli
		//pass resulting question into spawnSoo and return the result
	}

	
	/// <summary>
	/// Create a new SOO and stimuli with Question Data
	/// </summary>
	/// <returns>Reference to initialized SOO</returns>
	/// <param name="q">Question data </param>

	//IMPORTANT: RENAME WHEN spawnNext ABOVE IS COMPLETED
	public GameObject spawnNext (Question q)
	{
		newSoo = Instantiate(sooPrefab) as GameObject;
		SOOScript holder = newSoo.GetComponent<SOOScript>();
		newSoo.transform.position = transform.position;
		//create 4 instances of stimuli as children of the SOO, and arrange them within its Box Collider
		for (int i =0; i< 4; i++)
		{
			newStims[i] = Instantiate (stimPrefab) as GameObject;
			newStims[i].transform.SetParent(newSoo.transform);
			
			//set stimulus position within SOO
			switch(i){
				case 0: 
					newStims[i].transform.position = newSoo.transform.position + new Vector3 (left, up, 0);
					break;
				case 1: 
					newStims[i].transform.position = newSoo.transform.position + new Vector3 (left, down, 0);
					break;
				case 2: 
					newStims[i].transform.position = newSoo.transform.position + new Vector3 (right, down, 0);
					break;
				case 3: 
					newStims[i].transform.position = newSoo.transform.position + new Vector3 (right, up, 0);
					break;
			};
			positions[i] = transform.position;
			if(q.isCustomizationEvent())
			{
				Debug.Log("pulling from options");
				newStims[i].GetComponent<StimulusScript>().setOptions(q.getOption(i));  
			}
			else
			{
				Debug.Log("pulling from stimuli");
				newStims[i].GetComponent<StimulusScript>().setStim(q.getStim(i));  
			}
		}
		//add stims, stim positions, and SOO destinations to SOO instance
		holder.setSoo(newStims, q.getNumber()); 
		holder.setPosArray(positions);
		holder.setDestArray(destinations);
		//scale size to screen
		holder.transform.localScale = new Vector3 (0.6f,0.6f,0.6f);
		return newSoo;
		
	}		
}