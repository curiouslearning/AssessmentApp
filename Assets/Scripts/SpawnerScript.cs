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
	Dictionary<string, Category> catParser;
	List<serStim> stimPool;
	public TextAsset stimList;
	public GameObject sooPrefab;
	public GameObject stimPrefab;
	public GameObject character;
	//placement modifiers for stimulus positions
	public Vector4 visStimSpacing;
	public Vector4 charStimSpacing;
	public Vector4 textureSpacing;
	public Vector4 spacing;
	public float scaleStim;
	public float scaleCharacter;
	public float scaleTexture; //factor by which to scale down textures to fit on screen
	//variables for the SOO to hold onto
	public Vector3[] destinations;
	Vector3[] positions;
	public AnimationManager host;
	public ToggleBasket receptacle;
	public BackgroundScroller background;
	// Use this for initialization
	void Awake () {
		diffParser = new Dictionary<string, Difficulty>();
		diffParser.Add ("Easy", Difficulty.Easy);
		diffParser.Add ("Medium", Difficulty.Medium);
		diffParser.Add ("Hard", Difficulty.Hard);
		initCatParser();
		newStims = new GameObject[4];
		positions = new Vector3[4]; 
		stimPool = new List<serStim>();
		parseData();
	}

	void initCatParser()	
	{
		catParser = new Dictionary<string, Category>();
		catParser.Add("ReceptiveVocabulary",Category.ReceptiveVocabulary);
		catParser.Add("BlendingWordIdentification",Category.BlendingWordIdentification);
		catParser.Add("Customization",Category.Customization);
		catParser.Add("CVCWordIdentification",Category.CVCWordIdentification);
		catParser.Add("LetterNameID",Category.LetterNameRecognition);
		catParser.Add("LetterSoundMatching",Category.LetterSoundMatching);
		catParser.Add("PseudoWordMatching",Category.PseudowordMatching);
		catParser.Add("RhymingWordMatching",Category.RhymingWordMatching);
		catParser.Add("SightWordIdentification",Category.SightWordIdentification);
	}
	void parseData()
	{ 
		string[] sourceLines = stimList.text.Split('\n');
		for(int i = 1; i < sourceLines.Length; i++)
		{
			serStim data = new serStim();
			string [] vals = sourceLines[i].Split(',');
			data.hasBeenTarget = false;
			if(vals[1] == "audio")
			{
				data.audio = vals[0];
			}
			else
			{
				data.sprite = vals[0];
			}
			data.stimType = vals[1];
			data.difficulty = diffParser[vals[2].TrimEnd('\r')];	
			data.hostStimType = vals[3];
			data.hostStim = vals[4];  //what the host shows/says when this stimulus is the target
			data.category = catParser[vals[5].TrimEnd('\r')];
			stimPool.Add(data);
		}
	}


	string setType (Category cat)
	{
		string type;
		if (cat.Equals (Category.ReceptiveVocabulary) || cat.Equals(Category.Customization)) {
			type = "visual";
		} else {
			type = "audio";
		}	
		return type;
	}

	/*serStim selectTarget (Category cat, Difficulty diffLevel, string type)
	{
		//TODO: MOVE THE FIND TARGET WHILE LOOP HERE
	}*/

	/// <summary>
	/// Returns a list of four semi-randomly generated serStims, one of which is
	/// tagged as the correct answer
	/// </summary>
	/// <returns>List of 4 serStims</returns>
	/// <param name="cat">current Category in game</param>
	/// <param name="diffLevel">current Difficulty in game</param>

	//TODO: REFACTOR THIS BEHEMOTH
	List<serStim> findStim (Category cat, Difficulty diffLevel) {
		List<serStim> answer = new List<serStim>();
		int counter = 0;
		int targetCount = 0;
		int revolutionCount =0;
		string type;
		
		// the type of stimulus accepted is assigned based on the current category
		type = setType(cat);
		while (answer.Count == 0)
		{
			if (counter == stimPool.Count) //remember to declare this in new fxn
				counter = 0;
			serStim s = stimPool[counter]; //does this need to be global?
			if(s.stimType.Equals(type) && s.category.Equals(cat))
			{
				if(s.hasBeenTarget)
				{
					targetCount++; //make an independent variable for this
					if(targetCount == stimPool.Count)
					{
						break;
					}
					else{
						counter++;
						continue;
					}
				}
				
				// this block of code handles retrieving a
				// stimulus to be the target
				float f = Random.Range (0.0f,4.0f);
				if (f < 1.0f) {
					stimPool[counter].hasBeenTarget = true;
					s.isCorrect = true;
					answer.Add(s); //pass this, return this
					host.setHostMedia(s); //global
					break;
				}
			}
			counter++;
		}
		counter = 0;
		while (answer.Count < 4) {
			if (counter == stimPool.Count)
			{
				counter = 0;
				revolutionCount++;
				if(revolutionCount > 5)
				{
					resetTargets();
					revolutionCount = 0;
				}
			}
			serStim s = stimPool[counter];
			if(s.hasBeenTarget)
			{
				targetCount++;
				if(targetCount == stimPool.Count)
				{
					Debug.Log("ran out of " + diffLevel + " stims");
					ScoreTracker inst = GameObject.Find ("Main Camera").GetComponent<ScoreTracker>();
					diffLevel = inst.resetDifficulty(diffLevel); //if we run out of one difficulty don't freak out
					resetTargets();
					counter = 0;
					revolutionCount++;
					continue;
				}
				else{
					counter++;
					continue;
				}
			}
			// when a list of 4 serStims is assembled, findStim returns answer
			 if (s.stimType.Equals (type) && 
			    s.difficulty.Equals (diffLevel) &&
			    !answer.Contains(s) && s.category.Equals(cat)) { 
				// this block of code handles generating non-target
				// stimuli
			    float f = Random.Range (0.0f,4.0f);
				if (f < 1.0f) {
					answer.Add(s);
				}
				 
			}
			counter++;
		}
		return answer;
	}

	void resetTargets()
	{
		for (int i = 0; i < stimPool.Count; i++)
		{	
			stimPool[i].hasBeenTarget = false;
			stimPool[i].isCorrect = false;
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
			List<serStim> temp = findStim(cat, difLevel);
			q.init (questionNumber, temp, cat);
			return spawnSOO(q);
		}
	}

	
	/// <summary>
	/// Create a new SOO and stimuli with Question Data
	/// </summary>
	/// <returns>Reference to initialized SOO</returns>
	/// <param name="q">Question data </param>
	GameObject spawnSOO (Question q)
	{
		newSoo = Instantiate(sooPrefab) as GameObject;
		SOOScript holder = newSoo.GetComponent<SOOScript>();
		newSoo.transform.position = transform.position;
		arrangeSOO(q); //arrange the objects inside the SOO
		//add stims, stim positions, and SOO destinations to SOO instance
		holder.setSoo(newStims, q.getNumber()); 
		holder.setPosArray(positions);
		holder.setDestArray(destinations);
		holder = scaleChildren(q, holder);  //scale size to screen	
		host.registerGameObjectWithSoo(newSoo);
		receptacle.registerGameObjectWithSoo(newSoo);
		background.registerGameObjectWithSoo(newSoo);
		return newSoo;	
	}

//*****************************
//* spawnSOO HELPER FUNCTIONS *
//*****************************
	void arrangeSOO(Question q)
	{
		//create 4 instances of stimuli as children of the SOO, and arrange them within its Box Collider
		for (int i =0; i< 4; i++)
		{
			//identify the prefab and proper spacing needed within the SOO
			if(needsCharacter(q.getCat()))
			{
				newStims[i] = Instantiate (character) as GameObject; //use the secondary character
				spacing = charStimSpacing;	
			}
			else{
				newStims[i] = Instantiate (stimPrefab) as GameObject; //just display the stimulus as a sprite
				if(q.getCat() == Category.Customization)
				{
					spacing = textureSpacing;
				}
				else{
					spacing = visStimSpacing;
				}
			}
			newStims[i].transform.SetParent(newSoo.transform);
			
			//set stimulus position within SOO
			switch(i){
			case 0: 
				newStims[i].transform.position = newSoo.transform.position + new Vector3 (spacing.x, spacing.y, 0); //upper left
				break;
			case 1: 
				newStims[i].transform.position = newSoo.transform.position + new Vector3 (spacing.z, spacing.y, 0); //upper right
				break;
			case 2: 
				newStims[i].transform.position = newSoo.transform.position + new Vector3 (spacing.z, spacing.w, 0); //lower right
				break;
			case 3: 
				newStims[i].transform.position = newSoo.transform.position + new Vector3 (spacing.x, spacing.w, 0); //lower left
				break;
			}
			positions[i] = transform.position;
			initChild(q, i);	
		}
	}
	
	void initChild (Question q, int i)
	{
		if(q.isCustomizationEvent())
		{
			newStims[i].GetComponent<StimulusScript>().setOptions(q.getOption(i)); 
			
		}
		else if (needsCharacter(q.getCat()))
		{
			newStims[i].GetComponent<StimulusScript>().setStim(q.getStim(i));
		}
		else {
			newStims[i].GetComponent<StimulusScript>().setStim(q.getStim(i));
			newStims[i].GetComponent<StimulusScript>().initSprite();	
		}		
	}

	SOOScript scaleChildren (Question q, SOOScript holder)
	{
		if(q.isCustomizationEvent())
		{
			holder.transform.localScale = new Vector3 (scaleTexture,scaleTexture,scaleTexture);
		}
		else if (needsCharacter(q.getCat()))
		{
			holder.transform.localScale = new Vector3 (scaleCharacter,scaleCharacter,scaleCharacter);
		}
		else
		{
			holder.transform.localScale = new Vector3 (scaleStim,scaleStim,scaleStim);
		}
		return holder;
	}

	bool needsCharacter (Category cat)
	{
		switch (cat){
		case Category.ReceptiveVocabulary: //add categories that don't need the SecChar as cases here
			return false;
		case Category.Customization:
			return false;
		default:
			return true;
		}
	}			
}