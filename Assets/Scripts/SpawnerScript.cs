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
	//variables to hold stimulus selection components
	GameObject[] newStims;
	Sprite[] newOptions;
	Dictionary<string, Difficulty> diffParser;
	Dictionary<string, Category> catParser;
	List<serStim> stimPool;
	//initialization variables
	public TextAsset stimList;
	public GameObject sooPrefab;
	public GameObject stimPrefab;
	public Texture2D[] secondaryAtlases;
	//components
	public GameObject character;
	public AnimationManager host;
	public ToggleBasket receptacle;
	public BackgroundScroller background;
	public List<Observer> observers;
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
	public Sprite noSprite;
	public Sprite visStimSprite;

    /// <summary>
    /// Used for initialization; creates stimPool, parses data
    /// </summary>

    void Awake () 
	{
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

    /// <summary>
    /// creates a dictionary for the different Categories; called in Awake()
    /// </summary>

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

    /// <summary>
    /// parses data stored in StimuliList.csv; called in Awake()
    /// </summary>

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
			try {	
				data.category = catParser[vals[5].TrimEnd('\r')];			
				
			} 
			catch (System.Exception ex) {
				Debug.Log ("KeyNotFoundException @ stimulus " +(i+1)+ ", value is " + vals[5]);
			}
			stimPool.Add(data);
		}
	}

	/// <summary>
	/// Gets the hard stim count.
	/// </summary>
	/// <returns>The hard stims.</returns>
	public int getStimsByDifficulty (Difficulty diff, string type)
	{
		int retval = 0;
		for (int i = 0; i < stimPool.Count; i++)
		{
			if( stimPool[i].difficulty == diff && stimPool[i].stimType == type)
			{
				retval++;
			}
		}
		return retval;
	}

	/// <summary>
	/// Returns a list of four semi-randomly generated serStims, one of which is
	/// tagged as the correct answer
	/// </summary>
	/// <returns>List of 4 serStims</returns>
	/// <param name="cat">current Category in game</param>
	/// <param name="diffLevel">current Difficulty in game</param>

	List<serStim> findStim (Category cat, Difficulty diffLevel) 
	{
		List<serStim> answer = new List<serStim>();
		int counter = 0;
		string type;
		
		// the type of stimulus accepted is assigned based on the current category
		type = setType(cat);
		if(type == "gameOver")
			return null;
            // return null if a complete SOO cannot be created
            int numMatch = 0;
            for (int i = 0; i < stimPool.Count; i++)
                
                if (matchesCriteria(cat, diffLevel, type, stimPool[i])) 
                    numMatch++;
                
                if (numMatch < 4)
                return null;
            
		//find and add the target stimulus
		serStim temp = selectTarget(cat, diffLevel, type);
        string hostStim = temp.hostStim;
		answer.Add (temp);
        //find and add the remaining stimuli
        while (answer.Count < 4) 
		{
            counter = (int)Random.Range(0, stimPool.Count); // RANDOM STARTING POINT
			//do revolution counting in a separate function
			if (counter == stimPool.Count)
			{
				counter = 0;	
			}
			serStim s = stimPool[counter];
            // when a list of 4 serStims is assembled, findStim returns answer
            if (matchesCriteria(cat, diffLevel, type, s) && !answer.Contains(s) && (cat != Category.RhymingWordMatching)) // non-rhyming categories
			{
				s.isTarget = false;
				answer = randomAdd (answer, s);
			}
            if (matchesCriteria(cat, diffLevel, type, s) && !answer.Contains(s) && (cat == Category.RhymingWordMatching)) // special case for rhyming
            {
                if (s.audio != hostStim)
                {
                    s.isTarget = false;
                    answer = randomAdd(answer, s);
                }
            }
                counter++;
		}
		//randomize order here
		return answer;
	}

    //********************************
    // HELPER FUNCTIONS FOR findStim *
    //********************************

    /// <summary>
    /// use the category to set the type of stimulus findStim will search for; called by findStim
    /// </summary>
    /// <param name ="cat" >current Category </param>
    /// <returns> 
    /// string with current type of stimuli needed: visual, audio, or gameOver
    /// </returns>

    string setType (Category cat)
	{
		string type;
		if(cat.Equals(Category.GameOver)){
			return "gameOver";
		}
		if (cat.Equals (Category.ReceptiveVocabulary) || cat.Equals(Category.Customization) || cat.Equals(Category.BlendingWordIdentification) || cat.Equals(Category.PseudowordMatching)) 
		{
			type = "visual";
		} else 
		{
			type = "audio";
		}	
		return type;
	}
    /// <summary>
    /// select the target stimulus for a given question, according to the given criteria
    /// </summary>
    /// <param name="cat"> current Category</param>
    /// <param name="diffLevel">current Difficulty</param>
    /// <param name="type">current type of stimuli needed</param>
    /// <returns>
    /// return a single serStim "s" which will be the target stimuli for the next question
    /// </returns>

    serStim selectTarget (Category cat, Difficulty diffLevel, string type)
	{
		serStim s = null;
        int counter = (int) Random.Range(0, stimPool.Count); //RANDOM STARTING POINT
		int resetCounter = 0;
		bool foundTarget = false;
		if(type == "gameOver")
			return null;
        // return null if a complete SOO cannot be created
        int numMatch = 0;
        for (int i = 0; i < stimPool.Count; i++)
        {
            if (matchesCriteria(cat, diffLevel, type, stimPool[i]))
                numMatch++;
        }
        if (numMatch < 1)
            return null;
        while (foundTarget == false)
		{
			if(resetCounter >= 5){
				if(diffLevel != Difficulty.Easy){
					GetComponent<ScoreTracker>().resetDifficulty(diffLevel);
				}
				else {
					return null;
				}
			}
			if (checkFreeStims (cat,diffLevel,type) == 0) //if all stimuli have been used as a target (should not happen with full complement of stims)
			{
				resetTargets();
				resetCounter++;
			}
			if (counter == stimPool.Count) 
				counter = 0;
			s = stimPool[counter]; 
			if(s.stimType.Equals(type) && s.category.Equals(cat) && s.difficulty == diffLevel)
			{
				if(s.hasBeenTarget) //pass over any previously used targets
				{
					counter++;
					continue;
				}
				// this block of code handles retrieving a
				// stimulus to be the target
				float f = Random.Range (0.0f,4.0f);
				if (f < 1.0f) {
                   // Debug.Log("Sprite: " + s.sprite);
                   // Debug.Log("Audio: " + s.audio);
                    stimPool[counter].hasBeenTarget = true;
					s.isTarget = true;
					foundTarget = true;
					host.setHostMedia(s); // pass the target's prompt to the main character
					break;
				}
			}
			counter++;
		}
        return s;
	}

    /// <summary>
    /// check for stimuli that correspond the to the given criteria, and have not been used as a target yet; called by selectTarget
    /// </summary>
    /// <param name="cat">current Category</param>
    /// <param name="diffLevel">current Difficulty</param>
    /// <param name="type">type of stimuli needed</param>
    /// <returns>
    /// returns the number of stims in stimPool that are eligible to be used as a target for the next question
    /// </returns>

    int checkFreeStims (Category cat, Difficulty diffLevel, string type) // use this to prevent crashes
	{	
		int total = 0;
		for(int i = 0; i< stimPool.Count; i++)
		{
			if (matchesCriteria(cat, diffLevel, type, stimPool[i]) && !stimPool[i].hasBeenTarget)
				total++;
		}
		return total;
	}
    /// <summary>
    /// check to make sure a stimulus matches the defined criteria
    /// </summary>
    /// <param name="cat">currnet Category</param>
    /// <param name="diff">current Difficulty</param>
    /// <param name="type">current type of stimuli needed</param>
    /// <param name="s">an individual serStim</param>
    /// <returns>
    /// returns true if s meets the criteria specified by cat, diff, and type; false otherwise
    /// </returns>

    bool matchesCriteria (Category cat, Difficulty diff, string type, serStim s)
	{
		return (s.stimType.Equals (type) && 
			s.difficulty.Equals (diff) &&
				s.category.Equals(cat));
	}

    /// <summary>
    /// randomly choose whether or not to add the selected stimulus
    /// </summary>
    /// <param name="answer">a list of serStim</param>
    /// <param name="s">an individual serStim</param>
    /// <returns>returns answer with or without s attached</returns>

    List<serStim> randomAdd(List<serStim> answer, serStim s)
	{
		float f = Random.Range (0.0f,8.0f);
		if (f < 1.0f) 
		{
			answer.Add(s);
            //Debug.Log("Sprite: " + s.sprite);
            //Debug.Log("Audio: " + s.audio);
        }
		else if (f > 1.0f && f < 2.0f)
		{
			answer.Insert(0, s); //if f is between 1 & 2, insert the stimulus at the beginning. Helps randomize position of target
            //Debug.Log("Sprite: " + s.sprite);
            //Debug.Log("Audio: " + s.audio);
        }
        return answer;
	}

    /// <summary>
    /// reset targets when all have been used (should be obsolete with full complement of stimuli)
    /// </summary>

    void resetTargets()
	{
	
		for (int i = 0; i < stimPool.Count; i++)
		{	
			stimPool[i].hasBeenTarget = false;
			stimPool[i].isTarget = false;
		}
	}
    // ********************************
    // spawnNext and spawnSOO
    // ******************************** 

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
		else 
		{
			List<serStim> temp = findStim(cat, difLevel);
			if(temp == null)
				return null;
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
		//host.registerGameObjectWithSoo(newSoo);
		//receptacle.registerGameObjectWithSoo(newSoo);
		//background.registerGameObjectWithSoo(newSoo);
		registerObservers(newSoo);
		registerChildren(newSoo);
		return newSoo;	
	}

//*****************************
//* spawnSOO HELPER FUNCTIONS *
//*****************************
	/// <summary>
    /// call registerGameObjectWithSoo on all observers; called by spawnSOO
    /// </summary>
    /// <param name="soo">a Stimulus Organizational Object</param>
	void registerObservers (GameObject soo)
	{
		for (int i = 0; i < observers.Count; i++)
		{
			observers[i].registerGameObjectWithSoo(soo);
		}
	}

    /// <summary>
    /// registers the highlighter component in the new stimuli; called by spawnSOO
    /// </summary>
    /// <param name="soo">a Stimulus Organizational Object</param>

	void registerChildren (GameObject soo)
	{
		for (int i =0; i < newStims.Length; i++)
		{
			Highlighter h = newStims[i].GetComponentInChildren<Highlighter>();
			h.registerGameObjectWithSoo(soo);
		}
	}

    /// <summary>
    /// randomly select an atlas to build the secondary character
    /// </summary>

    Texture2D selectAtlas ()
	{
		return secondaryAtlases[Random.Range(0, secondaryAtlases.Length-1)];
	}
    /// <summary>
    /// add an atlas to the character for texturing
    /// </summary>
    /// <param name="c">GameObject</param>

    void drawCharacter (GameObject c)
	{
		SkinnedMeshRenderer r = c.transform.GetChild(0).GetComponentInChildren<SkinnedMeshRenderer>();
		Material m = r.material;
		m.mainTexture = selectAtlas ();
	}
	
    /// <summary>
    /// handles proper positioning of stimuli within SOO; called by spawnSOO
    /// </summary>
    /// <param name="q">Question</param>

	void arrangeSOO(Question q)
	{
			SpriteRenderer background;
		
		//create 4 instances of stimuli as children of the SOO, and arrange them within its Box Collider
		for (int i =0; i< 4; i++)
		{
			//identify the prefab and proper spacing needed within the SOO
			if(needsCharacter(q.getCat()))
			{
				newStims[i] = Instantiate (character) as GameObject; //use the secondary character
				drawCharacter(newStims[i]);
				spacing = charStimSpacing;	
			}
			else
			{
				newStims[i] = Instantiate (stimPrefab) as GameObject; //just display the stimulus as a sprite
				background = newStims[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
				if(q.getCat() == Category.Customization)
				{
					spacing = textureSpacing;
					background.sprite  = null;
				}
				else
				{
					spacing = visStimSpacing;
					background.sprite = visStimSprite;
				}
			}
			newStims[i].transform.SetParent(newSoo.transform);
			
			//set stimulus position within SOO
			switch(i)
			{
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
	
    /// <summary>
    /// initialize new stims; called by arrangeSOO
    /// </summary>
    /// <param name="q">a Question</param>
    /// <param name="i">an int</param>

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
		else 
		{
			newStims[i].GetComponent<StimulusScript>().setStim(q.getStim(i));
			newStims[i].GetComponent<StimulusScript>().initSprite();	
		}		
	}

    /// <summary>
    /// scales children; called by spawnSOO
    /// </summary>
    /// <param name="q">a Question</param>
    /// <param name="holder">a SOOScript instance</param>
    /// <returns></returns>

	SOOScript scaleChildren (Question q, SOOScript holder)
	{
		if(q.isCustomizationEvent())
		{
			holder.transform.localScale = new Vector3 (scaleTexture,scaleTexture,scaleTexture);
			holder.setBoxColliders();
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

    /// <summary>
    /// returns true if the current category requires secondary characters, false otherwise
    /// called by spawnSOO
    /// </summary>
    /// <param name="cat">the current category</param>
    /// <returns>a bool</returns>

	bool needsCharacter (Category cat)
	{
		switch (cat)
		{
		case Category.ReceptiveVocabulary: //add categories that don't need the SecChar as cases here
			return false;
		case Category.Customization:
			return false;
		case Category.PseudowordMatching:
			return false;
        case Category.BlendingWordIdentification:
            return false;
		default:
			return true;
		}
	}			
}