using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main character master controller, extends Animation Manager.
/// </summary>
//TODO: Move MC specific code from Animation Manager to this script
public class MainCharacter : AnimationManager {
	public Animator squareCard;
	public Animator rectangleCard;
	public Animator basketController;
	public BasketAnim basket;
	public Animator rakeController;
	Material square;
	Material rectangle;
	public TextAsset optionsList;
	const int NUMBODYPARTS = 7;
	const int NUMOPTIONS = 10;
	public GameObject[] bodyParts;
	public Dictionary <int, GameObject> mirrorParts;
	public Texture2D[][] optionTextures;
	Dictionary <string, Texture2D> optionDict; //for fast lookup of a selected texture
	public int defaultPos; //standard index for the default texture for each body part
	string[] sourceLines;
	bool[] bodyPartCustomized; //tracks which options have been customized
	Highlighter partHighlighter;
	public string [] littlePayoffList;
	public string [] bigPayoffList;
	public string [] transitionList;
	bool ride;
	bool fly;
	bool skip;
	bool carry;
	float audioCounter;
	public float audioInterval;
	public Color newColor;
	public Color oldColor;
	void Awake()
	{
		Application.targetFrameRate = 15;
		audioCounter = 0;
		optionDict = new Dictionary<string, Texture2D>();
		mirrorParts = new Dictionary<int, GameObject>();
		int[] n = {2,5,6};
		string [] s = {"MainCharacter:r_antenna","MainCharacter:r_wing","MainCharacter:r_eye"};
		addMirrorParts (n, s, mirrorParts); 
		bodyPartCustomized = new bool[NUMBODYPARTS];
		optionTextures = new Texture2D[NUMBODYPARTS][];
		for(int i = 0; i < NUMBODYPARTS; i++)
		{
			bodyPartCustomized[i] = false;
			optionTextures[i] = new Texture2D[NUMOPTIONS];
		}		
		animator = GetComponent<Animator>();
		square = squareCard.GetComponent<MeshRenderer>().material;
		rectangle = rectangleCard.GetComponent<MeshRenderer>().material;
		initTextures();
		
	}
	// Use this for initialization
	void Start ()
	{
		eventHandler = GetComponent<Subject> ();	
		base.addSelfToSubjects();
		scoreTracker = GameObject.Find("Main Camera").GetComponent<ScoreTracker>();
		partHighlighter = activateNextHighlighter ();
		base.touchDetect.initP (this.onSelect);
		ride = false;
		skip = false;
		carry = false;
		fly = false;
	}
	
	/// <summary>
    /// On Awake, initialize the list of body parts with left/right mirrors
    /// </summary>
    /// <param name="numbers">array of ints</param>
    /// <param name="parts">array of strings</param>
    /// <param name="mirror">Dictionary mapping ints and GameObjects</param>

	void addMirrorParts(int[] numbers, string [] parts, Dictionary<int, GameObject> mirror)
	{
		for (int i = 0; i < numbers.Length; i++)
		{
			GameObject g = GameObject.Find (parts[i]);
			mirror.Add (numbers[i], g);
		}
	}
	/// <summary>
	/// Parses the optionsList data into the textures array.
	/// </summary>

	void parseData()
	{
		sourceLines = optionsList.text.Split('\n');
		for (int i= 1; i < sourceLines.Length; i++)
		{
			//index each texture by its body part and its position in the options hierarchy (0 is default texture)
			string[] values = sourceLines[i].Split(',');
			int bodyPart = int.Parse(values[1]);			
			int texturePosition = int.Parse(values[2]);
			Texture2D t =  Resources.Load<Texture2D>("Textures/" + values[0]);
			optionTextures[bodyPart][texturePosition] = t;
			optionDict.Add(values[0], t);

		}
	}
	/// <summary>
    /// assign appropriate audio or visual media to the main character; called in selectTarget in SpawnerScript
    /// </summary>
    /// <param name="m">serStim</param>

	public void setHostMedia (serStim m)
	{
		
		if (m.hostStimType== "audio")
		{
			AudioClip audio = Resources.Load<AudioClip>("Audio/" + m.hostStim);	
			if(audio == null)
			{
				Debug.LogError("Audio did not load properly");
			}
			setHostMediaInternal (audio);
		}
		else{
			Debug.Log("vis stim with name: " + m.hostStim);
			Sprite sprite = Resources.Load<Sprite>("Art/" + m.hostStim);
			setHostMediaInternal (sprite);
		}
	}
	    /// <summary>
    /// Helper method for setHostMedia
    /// </summary>
    /// <param name="a">an AudioClip</param>

	void setHostMediaInternal (AudioClip a)
	{
		AudioSource host = GetComponent<AudioSource>();
		host.clip = a;
		square.mainTexture = null;
		rectangle.mainTexture = null;
	}

    /// <summary>
    /// Helper method  for setHostMedia
    /// </summary>
    /// <param name="s">a Sprite</param>

	void setHostMediaInternal (Sprite s)
	{
		Text t;
		if ((s.rect.xMax-s.rect.xMin) >= (s.rect.yMax-s.rect.yMin +50)) //if the texture is a rectangle
		{
			t = rectangleCard.gameObject.GetComponentInChildren<Text> ();
			t.text = s.name;
			rectangle.mainTexture = s.texture;
			square.mainTexture = null;
		}
		else {
			t = squareCard.gameObject.GetComponentInChildren<Text> ();
			t.text = s.name;
			square.mainTexture = s.texture;
			rectangle.mainTexture = null;
		}
		AudioSource host = GetComponent<AudioSource> ();
		//host.clip = null;
	}

	/// <summary>
	/// Creates atlases for each body part, sets the UVs to default values
	/// </summary>
    
	void initTextures ()
	{
		parseData();
		for (int i =0; i< bodyParts.Length; i++)
		{
			//pack textures for body part into an atlas, store dimensions in a Rect
			SkinnedMeshRenderer bodyPart = bodyParts[i].GetComponent<SkinnedMeshRenderer>();
			bodyPart.material.mainTexture = optionTextures[i][defaultPos];
			/*atlas = new Texture2D(atlasDimensions, atlasDimensions);
			texturePositions[i] = atlas.PackTextures(optionTextures[i], padding, atlasSize);
			atlases[i] = atlas;
			//set bodyPart texture to the default texture in the atlas
			bodyPart.material.mainTexture=atlas;
			bodyPart.material.mainTextureScale = new Vector2 (texturePositions[i][defaultPos].x, texturePositions[i][defaultPos].y);*/
			
		}		
	}

	/// <summary>
	/// Method for handling events this class is listening for
	/// </summary>
	/// <param name="e">Event Instance.</param>
    
	public override void onNotify (EventInstance<GameObject> e)
	{
		if (e.type == eType.NewQuestion)
		{
			currentCategory = scoreTracker.queryCategory();
			if (currentCategory == Category.Customization) {
				partHighlighter = activateNextHighlighter ();
			} else {
				hideAllHighlighters ();
			}

		}	
		if (e.type == eType.TimedOut) {
			GetComponentInChildren<SpriteRenderer> ().enabled = false;
			startTransition ();
			return;
		}
		if (e.type == eType.FingerUp) {
			partHighlighter.reset ();
		}
		if(e.type == eType.Selected)
		{
			GetComponentInChildren<SpriteRenderer> ().enabled = false;
			partHighlighter.reset ();
			startPayoff();
			if(e.signaler.GetComponent<StimulusScript>() != null && e.signaler.GetComponent<StimulusScript>().isOption()) //if selected object is a body part
			{
				StimulusScript s = e.signaler.GetComponent<StimulusScript>();
				changeBodyPart(s.getBodyPart(), s.getTextureName());
				Destroy (e.signaler);
			}
			return;
		}

		if (e.type == eType.Grab)
		{
			
				animator.SetTrigger("Point");
			return;
		}
		if (e.type == eType.Ready)
		{
			finishTransitions();
			if (GetComponent<AudioSource> ().clip != null) { //if the prompt is auditory
				setAudioSource ();
			}
			 else if (square.mainTexture != null || rectangle.mainTexture != null) { //if the prompt is visual
				setCards ();
			} 
			if (GetComponent<AudioSource>().clip == null){
				GetComponentInChildren<SpriteRenderer> ().enabled = false;  //remove the talk bubble if no audio
			}
			return;
		}
	}
	/// <summary>
	/// overridden method to handle events with bool subjects
	/// </summary>
	/// <param name="e">Event Instance containing the subject and the type of event thrown.</param>
	public override void onNotify (EventInstance<bool> e)
	{
		Debug.Log ("boop boop");
		if(e.type == eType.Selected)
		{
			startTransition();
			return;	
		}
	}
	//reset animators for new question
	void finishTransitions()
	{
		animator.SetTrigger("Landed");
		if (carry) { //throw the basket if it's being carried
			throwBasket ();
			carry = false;
		} else if (ride){
			endRide ();
			ride = false;
		} else if (skip){
			endSkip ();
			skip = false;
		} else if (fly){
			flyToStand();
			fly = false;
		}
	}

	void setAudioSource()
	{
			GetComponentInChildren<SpriteRenderer> ().enabled = true;
			animator.SetTrigger ("Talk");
	}
	//set the flashcards
	void setCards()
	{
		GetComponentInChildren<SpriteRenderer> ().enabled = false;
		animator.SetTrigger ("ShowCard");
		animator.SetBool ("CardUp", true);
	}
	IEnumerator Flash (Renderer r){
		Debug.Log ("using: " + r);
		r.enabled = false;
		yield return new WaitForSeconds(1f);
		r.enabled = true;
		yield return new WaitForSeconds(1f);
	}
//************************************
// Animation initialization functions*
//************************************

//Transitions
//------------

	/// <summary>
	/// Starts the walk with basket transition animation
	/// </summary>
	public void carryBasket ()
	{
		basketController.SetTrigger ("StartCarry");
		animator.SetTrigger ("GrabBasket");
		carry = true;
	}
	/// <summary>
	/// Throws the basket.
	/// </summary>
	public void throwBasket ()
	{
		animator.ResetTrigger ("GrabBasket");
		basketController.ResetTrigger ("StartCarry");
		animator.SetTrigger ("Throw");
		basketController.SetTrigger("Throw");
	}
	/// <summary>
	/// Starts the fly with basket transition animation
	/// </summary>
	public void flyBasket()
	{
		basketController.SetTrigger ("StartFly");
		animator.SetTrigger ("Fly");
		fly = true;
	}

	/// <summary>
	/// End the flying transition animation
	/// </summary>
	public void flyToStand()
	{
		animator.ResetTrigger ("Fly");
		animator.SetTrigger ("FinishFly");
		basketController.SetTrigger ("FinishFly");
	}

	/// <summary>
	/// Start the skip with basket transition animation
	/// </summary>
	public void skipBasket ()
	{
		basketController.SetTrigger ("StartSkip"); 
		animator.SetTrigger ("Skip");
		skip = true;
	}
	/// <summary>
	/// Ends the skip.
	/// </summary>
	public void endSkip ()
	{
		animator.SetTrigger("FinishSkip");
		basketController.SetTrigger ("FinishSkip");
	}
	/// <summary>
	/// Start the ride in basket transition animation
	/// </summary>
	public void rideBasket()
	{
		basketController.SetTrigger ("StartRide");
		animator.SetTrigger ("Ride");
		ride = true;
	}
	/// <summary>
	/// Ends the ride transition animation
	/// </summary>
	public void endRide ()
	{
		animator.ResetTrigger("Ride");
		basketController.ResetTrigger ("StartRide");
		animator.SetTrigger ("FinishRide");
		basketController.SetTrigger ("FinishRide");
	}

//Big Payoff Animations
//-----------------------

	/// <summary>
	/// Starts the search through basket payoff animation
	/// </summary>
	public void searchBasket ()
	{
		Debug.Log ("called search");
		basketController.SetTrigger("Search");
		animator.SetTrigger ("Search");
		basket.setTokens ("Search");
	}

	/// <summary>
	/// Start the dump and rake basket payoff animation
	/// </summary>
	public void dumpBasket()
	{
		basketController.SetTrigger ("Dump");
		rakeController.SetTrigger ("Rake");
		animator.SetTrigger ("Dump");
		basket.setTokens ("Dump");
	}

	/// <summary>
	/// Start the trip over basket payoff animation
	/// </summary>
	public void tripBasket()
	{
		basketController.SetTrigger ("Trip");
		animator.SetTrigger ("TripBasket");
		basket.setTokens ("Trip");
	}
	/// <summary>
	/// Starts the rummage payoff animation
	/// </summary>
	public void rummage()
	{
		animator.SetTrigger ("Rummage");
		basketController.SetTrigger ("Rummage");
		basket.setTokens ("Rummage");
	}

//Quick Payoff Animations
//-----------------------

	/// <summary>
	/// Start the jump payoff animation
	/// </summary>
	public void jump ()
	{
		animator.SetTrigger ("Jump");
	}
	/// <summary>
	/// Start the Jump and spin payoff animation
	/// </summary>
	public void jumpSpin()
	{
		animator.SetTrigger ("JumpSpin");
	}
	/// <summary>
	/// Starts the clap payoff animation
	/// </summary>
	public void clap ()
	{
		animator.SetTrigger ("Clap");
	}


	/// <summary>
	/// starts the dance payoff animation
	/// </summary>
	public void dance ()
	{
		animator.SetTrigger ("Dance");
	}



	/// <summary>
	/// Starts the payoff animation sequence.
	/// </summary>
	public void startPayoff()
	{
		animator.SetBool ("ShowCard", false);
		hideVisibleCards ();
		animator.ResetTrigger ("Landed");
		animator.ResetTrigger ("Throw");
		animator.ResetTrigger ("Point");
		if(currentCategory == Category.Customization && scoreTracker.questionNumber > 1)
		{
			randomAnimation (bigPayoffList);
		}
		else
		{
			randomAnimation (littlePayoffList);
		}
		GetComponent<AudioSource>().clip = null;
		clearCards();
	}	

	/// <summary>
	/// Starts the question transition sequence.
	/// </summary>
	public void startTransition ()
	{
		eventHandler.sendEvent (eType.Transition);
		animator.SetBool("ShowCard", false);
		hideVisibleCards();
		animator.ResetTrigger("Landed");
		animator.ResetTrigger("Throw");
		animator.ResetTrigger("Point");
		randomAnimation(transitionList);
		GetComponent<AudioSource>().clip = null;
		clearCards();
	}
	/// <summary>
	/// Reveal the MainCharacter's card. Show square if both are null. 
	/// </summary>
	public void startCards()
	{
		if(rectangle.mainTexture != null)
		{
			rectangleCard.SetTrigger("ShowCard");
		}
		else
		{
			squareCard.SetTrigger("ShowCard");
			
		}	
		
	}

	void clearCards ()
	{
		square.mainTexture = null;
		rectangle.mainTexture = null;
	}

	/// <summary>
	/// Hides the visible card.
	/// </summary>
	public void hideVisibleCards ()
	{
		squareCard.SetTrigger("RemoveCard");
		rectangleCard.SetTrigger("RemoveCard");
	}

	/// <summary>
	/// Starts the given animation.
	/// </summary>
	/// <param name="s">S.</param>
	protected override void startAnimation(string s)
	{
		switch (s) {
		case "Dump":
			dumpBasket ();
			break;
		case "Throw":
			throwBasket ();
			break;
		case "Fly":
			flyBasket ();
			break;
		case "Ride":
			rideBasket (); 
			break;
		case "Search":
			searchBasket ();
			break;
		case "Trip":
			tripBasket ();
			break;
		case "Skip": 
			skipBasket (); 
			break;
		case "GrabBasket":
			carryBasket ();
			break;
		case "Jump":
			jump ();
			break;
		case "JumpSpin":
			jumpSpin ();
			break;
		case "Rummage":
			rummage ();
			break;
		case "Clap":
			clap ();
			break;
		case "Dance":
			dance ();
			break;
		default:
			base.startAnimation (s);
			break;
		}
	}
//*************************************
//Other MainCharacter Helper Functions*
//*************************************

//TODO: refactor this according to new highlighter
	Highlighter activateNextHighlighter()
	{
		hideAllHighlighters (); //reset all body parts
		int part = getBodyPart();
		Highlighter h = bodyParts [part].GetComponent<Highlighter> ();
		h.toggleActive (true);
		return h;
	}

	
	void hideAllHighlighters()
	{
		for (int i = 0; i < bodyParts.Length; i++)
		{
			bodyParts [i].GetComponent<Highlighter> ().toggleActive (false);
		}
	}


	/// <summary>
	/// Swaps the texture on the selected body part.
	/// </summary>
	/// <param name="part">Index of the bone to be changed in bodyParts.</param>
	/// <param name="newTexture"> Replacement Texture.</param>
	void changeBodyPart (int part, string newTexture)
	{    
		if (mirrorParts.ContainsKey(part))
		{
			GameObject mirror = mirrorParts[part];
			mirror.GetComponent<SkinnedMeshRenderer>().material.mainTexture = optionDict[newTexture];
		}
		GameObject temp = bodyParts[part];	
		temp.GetComponent<SkinnedMeshRenderer>().material.mainTexture = optionDict[newTexture];
	}
	
	

	/// <summary>
	/// Gets the options for the first not-yet-customized body part.
	/// </summary>
	/// <returns>texture options converted into sprites.</returns>
	public List<Sprite> getOptions ()
	{
		List<Sprite> options;
		options = new List<Sprite>();
		int curBodyPart = getNextBodyPart();
		Texture2D[] textures = optionTextures[curBodyPart];
		for(int j = 1; j < textures.Length; j++)  //convert and package options
		{
			if(textures[j] != null)
			{
				Sprite s = Sprite.Create(textures[j], new Rect(0,0, textures[j].width, textures[j].height), new Vector2 (0.5f, 0.5f));
				s.name = textures[j].name;
				options.Add(s); 
			}
		}	
		return options;
	}

	
	/// <summary>
	/// Returns the index of the first not-yet-customized body part.
	/// </summary>
	/// <returns>Body part index.</returns>
	public int getBodyPart ()
	{
		int i =0;
		while (i < bodyPartCustomized.Length && bodyPartCustomized[i] == true ) {
			i++;
		}	
		if( i > 0)	
			return i-1;
		return i;
	}
	/// <summary>
	/// Gets the next body part in the customization line, and marks it as customized.
	/// </summary>
	/// <returns>The next body part.</returns>
	int getNextBodyPart()
	{
		int i =0;
		while (i < bodyPartCustomized.Length && bodyPartCustomized[i] == true ) {i++;}
		if (i >= bodyPartCustomized.Length) //reset body part list to maintain customizability
		{
			for (int j = 1; j < bodyPartCustomized.Length; j++)
			{
				bodyPartCustomized[j] = false;
			}
			i = 0;
		}
		bodyPartCustomized[i] = true;
		return i;
	}
	/// <summary>
	/// Plays the audio, if present.
	/// </summary>
	public void playAudio ()
	{
		AudioSource a = GetComponent<AudioSource>();
        if (a != null && !a.isPlaying)
        {
            //talkBubble.sprite = talkBubbleSprite;
            a.Play();
        }

		eventHandler.sendEvent (eType.AudioFinish, this.gameObject);
	}
	/// <summary>
	/// Resets the talk animation.
	/// </summary>
	public void resetTalk ()
	{
		animator.ResetTrigger ("Talk");
	}
	/// <summary>
	/// catches tap events and plays audio (if present)
	/// </summary>
	/// <param name="t">T.</param>
	override public void onSelect (touchInstance t)
	{
		Debug.Log("caught tap");
		if(GetComponent<AudioSource>().clip != null)
		{
			Debug.Log ("notNull!");
			animator.SetTrigger("TapTalk");
			playAudio ();
			audioCounter = 0f;
		}
		
	}
	//controls the automatic talking timer
	void Update () 
	{
		if(GetComponent<AudioSource>().clip != null)
		{
			audioCounter += Time.deltaTime;
			if(audioCounter >= audioInterval && !GetComponent<AudioSource>().isPlaying)
			{
				eventHandler.sendEvent (eType.StartAudio, this.gameObject);
				animator.SetTrigger("Talk");
				audioCounter = 0f;
			}
		}
		else {
			/*if(!(talkBubble.sprite == null)){
				talkBubble.sprite = null;
			}*/
			audioCounter = 0f;
		}
	}
}
