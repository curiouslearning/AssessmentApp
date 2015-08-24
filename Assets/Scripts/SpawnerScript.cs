using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	Dictionary<string, Difficulty> diffParser;
	List<serStim> stimPool;
	public TextAsset stimList;
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
		diffParser = new Dictionary<string, Difficulty>();
		diffParser.Add ("Easy", Difficulty.Easy);
		diffParser.Add ("Medium", Difficulty.Medium);
		diffParser.Add ("Hard", Difficulty.Hard);
		newStims = new GameObject[4];
		positions = new Vector3[4]; 
		stimPool = new List<serStim>();
		parseData();
	}
	

	/// <summary>
	/// Returns a list of four semi-randomly generated serStims, one of which is
	/// tagged as the correct answer
	/// </summary>
	/// <returns>List of 4 serStims</returns>
	/// <param name="cat">current Category in game</param>
	/// <param name="diffLevel">current Difficulty in game</param>

	List<serStim> findStim (Category cat, Difficulty diffLevel) {
		List<serStim> answer = new List<serStim>();
		int counter = 0;
		string type;
		// the variable type, which is used to determine whether findStim
		// is looking for audio or visual stimulus, is assigned based
		// on the current category

		/*if (cat.Equals (Category.ReceptiveVocabulary)) { //this is needed only with mixed stimuli
			type = "visual";
		} else 
			type = "audio";*/

		type = "visual"; //workaround HACK
		Debug.Log ("type: " + type);
		while (answer.Count < 4) {
			if (counter == stimPool.Count)
				counter = 0;
			serStim s = stimPool[counter];
			s.stimType = type;
			// when a list of 4 serStims is assembled, findStim returns answer
			 if (stimPool[counter].stimType.Equals (type) && 
			    stimPool[counter].difficulty.Equals (diffLevel) &&
			    !answer.Contains(stimPool[counter])) { 
				// this block of code handles generating non-target
				// stimuli
			    if (stimPool[counter].hasBeenTarget || counter > 0) {
				    float f = Random.Range (0.0f,4.0f);
					if (f < 1.0f) {
						answer.Add(s);
					}
				} else  {
					// this block of code handles retrieving a
					// stimulus to be the target
					float f = Random.Range (0.0f,4.0f);
					if (f < 1.0f) {
						stimPool[counter].hasBeenTarget = true;
						s.isCorrect = true;
						answer.Add(s);
					}
				}
			}
			counter++;
		}
		Debug.Log ("findStim completed, answer length: " + answer.Count);
		return answer;
	}

	void parseData()
	{ 
		string[] sourceLines = stimList.text.Split('\n');
		for(int i = 1; i < sourceLines.Length; i++)
		{
			serStim data = new serStim();
			string [] vals = sourceLines[i].Split(',');
			data.sprite = vals[1]; 
			data.stimType = vals[2];
			data.difficulty = diffParser[vals[3].TrimEnd('\r')];
			data.hasBeenTarget = false;
			stimPool.Add(data);
		}
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
		if (cat == Category.Customization)
		{
			q.init(questionNumber, host.getOptions(), host.getBodyPart());
			return spawnSOO(q);
		}
		else {
			int target = 0; 
			q.init (questionNumber, findStim(cat,difLevel), target, cat);
		}
		//else call findStimuli
		//pass resulting question into spawnSoo and return the result
	    GameObject g = spawnSOO (q);
		return g;
	}

	
	/// <summary>
	/// Create a new SOO and stimuli with Question Data
	/// </summary>
	/// <returns>Reference to initialized SOO</returns>
	/// <param name="q">Question data </param>

	//IMPORTANT: RENAME WHEN spawnNext ABOVE IS COMPLETED
	public GameObject spawnSOO (Question q)
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
			}
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
				newStims[i].GetComponent<StimulusScript>().initSprite();
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