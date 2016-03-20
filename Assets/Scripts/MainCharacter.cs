﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Main character master controller, extends Animation Manager.
/// </summary>
//TODO: Move MC specific code from Animation Manager to this script
public class MainCharacter : AnimationManager {
	public Animator squareCard;
	public Animator rectangleCard;
	public Animator basketController;
	public Animator rakeController;
	public Animator[] tokenControllers;	
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
	Animator partHighlighter;
	public Highlighter mainHighlighter;
	public string [] littlePayoffList;
	public string [] bigPayoffList;
	public string [] transitionList;
	float audioCounter;
	public float audioInterval;
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
		touchDetect.initP (this.onSelect);
		initTextures();
		
	}
	// Use this for initialization
	void Start ()
	{
		eventHandler = GetComponent<Subject> ();	
		addSelfToSubjects();
		scoreTracker = GameObject.Find("Main Camera").GetComponent<ScoreTracker>();
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
			//Debug.Log("vis stim with name: " + m.hostStim);
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
		if ((s.rect.xMax-s.rect.xMin) >= (s.rect.yMax-s.rect.yMin +50)) //if the texture is a rectangle
		{
			rectangle.mainTexture = s.texture;
			square.mainTexture = null;
		}
		else {
			square.mainTexture = s.texture;
			rectangle.mainTexture = null;
		}
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
			if(currentCategory == Category.Customization)
			{
				updateHighlighter(); 
			}
			else
			{
				hideHighlighter();
			}
		}	
		if (e.type == eType.TimedOut) {
			startTransition ();
			return;
		}
		if(e.type == eType.Selected)
		{
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
			animator.SetTrigger("Landed");
			if (basketController.GetBool ("Carry")) { //throw the basket if it's being carried
				throwBasket ();
			}
			basketController.SetBool ("Carry", false);
			basketController.ResetTrigger ("Ride");
			basketController.SetBool ("Skip", false);
			basketController.SetBool ("Fly", false);
			if(GetComponent<AudioSource>().clip != null)
			{
				animator.SetTrigger("Talk");
            }
			else if( square.mainTexture != null || rectangle.mainTexture != null)
			{
				animator.SetBool("ShowCard", true);
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

//************************************
// Animation initialization functions*
//************************************
	public void carryBasket ()
	{
		animator.SetTrigger ("GrabBasket");
		basketController.SetBool("Carry", true);
	}
	
	public void throwBasket ()
	{
		basketController.SetBool("Carry", false);
		animator.SetTrigger ("Throw");
		basketController.SetTrigger("Throw");
	}

	public void flyBasket()
	{
		animator.SetTrigger ("Fly");
		basketController.SetBool("Fly", true);
	}


	public void flyToStand()
	{
		basketController.SetBool ("Fly", false);
	}
	public void searchBasket ()
	{
		Debug.Log ("called search");
		animator.SetTrigger ("Search");
		basketController.SetTrigger("Search");
		setTokens ("Search");
	}
	public void dumpBasket()
	{
		animator.SetTrigger ("Dump");
		basketController.SetTrigger ("Dump");
		rakeController.SetTrigger ("Rake");
		setTokens ("Dump");
	}

	public void rideBasket()
	{
		animator.SetTrigger ("Ride");
		basketController.SetBool ("Ride", true);
	}

	public void endRide ()
	{
		animator.ResetTrigger("Ride");
		basketController.SetBool ("Ride", false);
	}
	public void skipBasket ()
	{
		animator.SetTrigger ("Skip");
		basketController.SetTrigger ("StartSkip"); 
		basketController.SetBool ("Skip", true);
	}

	public void tripBasket()
	{
		animator.SetTrigger ("TripBasket");
		basketController.SetTrigger ("Trip");
		setTokens ("Trip");
	}

	public void jump ()
	{
		animator.SetTrigger ("Jump");
	}

	public void jumpSpin()
	{
		animator.SetTrigger ("JumpSpin");
	}

	public void clap ()
	{
		animator.SetTrigger ("Clap");
	}

	public void rummage()
	{
		animator.SetTrigger ("Rummage");
		basketController.SetTrigger ("Rummage");
		setTokens ("Rummage");
	}

	/// <summary>
	///call the specified animation on all tokens in the basket
	/// </summary>
	/// <param name="s">S.</param>
	void setTokens (string s)
	{
		for (int i = 0; i < tokenControllers.Length; i++) {
			tokenControllers[i].SetTrigger (s);
		}
		
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
		randomAnimation (bigPayoffList);
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
		Debug.Log ("calling startAnimation with string: " + "\"" + s + "\"");
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
		default:
			base.startAnimation (s);
			break;
		}
	}
	//*************************************
//Other MainCharacter Helper Functions*
//TODO: Split into a Maincharacter    *
//extension of AnimationManager       *
//*************************************

//TODO: refactor this according to new highlighter
	void updateHighlighter()
	{
		int part = getBodyPart();
		bodyParts[part].transform.GetChild(0).gameObject.SetActive(true);
		for (int i =0; i < bodyParts.Length; i++)
		{
			if (i != part)	
			{	
				bodyParts[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}
	}
	
	void hideHighlighter()
	{
		for (int i = 0; i < bodyParts.Length; i++)
		{
			bodyParts[i].transform.GetChild(0).gameObject.SetActive(false);
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
		Debug.Log("play");
		AudioSource a = GetComponent<AudioSource>();
        if (a != null && !a.isPlaying)
        {
            //talkBubble.sprite = talkBubbleSprite;
            mainHighlighter.highlightOnce();
            a.Play();
        }
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
	public void onSelect (touchInstance t)
	{
		Debug.Log("caught tap");
		if(GetComponent<AudioSource>().clip != null)
		{
			animator.SetTrigger("TapTalk");
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
