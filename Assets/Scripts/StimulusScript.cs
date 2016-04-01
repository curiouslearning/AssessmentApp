using UnityEngine;
using System.Collections;


/// <summary>
/// StimulusScript
/// Class containg data and functionality for individual stimuli initialized by the SpawnerScript using Question instances.
/// Attached to the SOO instance as child objects.
/// </summary>
public class StimulusScript : MonoBehaviour{

	public bool isTarget;
	private bool hasBeenTarget;
	private bool option;
	private bool isBeingDragged; 
	public bool isDraggable;
	//sizing modifiers
	public float charMod;
	public float textureMod;
	public float stimMod;
	private string stimType;
	private string visStim;
	private Difficulty difficulty;
	private Vector3 homePos; //snapback functionality
	private Vector3 startScale; //scaling functionality
	Selectable touchInput;
	private int optionBodyPart;
	string textureName;
	public TokenScript token;

	void Start ()
	{
		if(GetComponent<Animator>() != null)
		{
			startScale = transform.localScale * charMod;
		}
		else if (option)
		{
			startScale = transform.localScale * textureMod;
		}
		else {
			startScale = transform.localScale * stimMod;
		}
		//other initializations
		GameObject.Find("Main Camera").GetComponent<ScreenCleaner>().registerObject (this.gameObject, OnscreenObjectList.MyObjectTypes.Stimulus); //register with screen cleaner
		isBeingDragged = false;
		touchInput = GetComponent<Selectable>();
		if (touchInput != null) {
			touchInput.initP (onSelect); //attach self to input wrapper
		} else if (GetComponentInChildren<Selectable> () != null) {
			touchInput = GetComponentInChildren<Selectable> ();
			touchInput.initP (onSelect);
		}
	}

	void OnDestroy ()
	{
		if(GameObject.Find("Main Camera") == null)
			return;
		GameObject.Find("Main Camera").GetComponent<ScreenCleaner>().deRegisterObject (this.gameObject, OnscreenObjectList.MyObjectTypes.Stimulus); //deregister with screen cleaner
	}

//*******************
// Getter functions *
//*******************
	public string returnStimType (){
		return stimType;
	}
	
	public bool returnHasBeenTarget(){
		return hasBeenTarget;
	}
	public bool returnIsTarget() {
		return isTarget;
	}
	public bool returnIsBeingDragged() {
		return isBeingDragged;
	}

	public bool returnIsDraggable() {
		return isDraggable;
	}
	/// <summary>
	/// Returns the body part index for a customization option.
	/// </summary>
	/// <returns>Body part index.</returns>
	public int getBodyPart ()
	{
		return optionBodyPart;
	}

	public Vector3 returnHomePos() {
		return homePos;  
	}

	public Difficulty returnDifficulty() {
		return difficulty;
	}
		
	public string getTextureName()
	{
		return textureName;
	}

	public bool isOption ()
	{
		return option;
	}
//*******************
// Setter functions *
//*******************



	/// <summary>
	/// indicate to object that it is currently being dragged by user.
	/// </summary>
	/// <param name="input">If set to <c>true</c>, object is being dragged.</param>
	public void setIsBeingDragged(bool input) {
		isBeingDragged = input;
	}
	/// <summary>
	/// Indicate whether or not the object is draggable.
	/// </summary>
	/// <param name="input">If set to <c>true</c> input.</param>
	public void setIsDraggable(bool input) {
		isDraggable = input;
	}
	/// <summary>
	/// Update the position the object will snap back to after dragging. 
	/// </summary>
	public void setHomePos() {
		homePos = transform.position;
		if (token != null) //update the token as well
		{
			token.setPos();
		}
	}
	/// <summary>
	/// Sets the difficulty to the question's difficulty level.
	/// </summary>
	/// <param name="newDiff">difficulty level</param>
	public void setDifficulty(Difficulty newDiff) {
		difficulty = newDiff;
	}

	public void setIsTarget(bool b) {
		isTarget = b;	
	}

	public void setHasBeenTarget(bool b) {
		hasBeenTarget = b;
		if (isTarget) {
			isTarget = false;
		}
	}

	public void	toggleBoxCollider(bool b)
	{
		GetComponent<BoxCollider2D>().enabled = b;
	}

//********************
// Scaling functions *
//********************

	/// <summary>
	/// Scales the stimulus size by mod
	/// </summary>
	/// <param name="mod">scaling modifier</param>
	public void scaleToTarget (float mod)
	{
		transform.localScale = startScale*mod;
	}

	/// <summary>
	/// Resets the scale to original size.
	/// </summary>
	public void resetScale ()
	{
		transform.localScale = startScale;
	}

	

//***************************
// Initialization Functions *
//***************************
	/// <summary>
	/// Initialization function for stimuli.
	/// </summary>
	/// <param name="input">stimulus data.</param>
	public	void setStim (serStim input) {
		this.hasBeenTarget = input.hasBeenTarget;
		this.isTarget = input.isTarget;
		this.isDraggable = input.isDraggable;
		this.difficulty = input.difficulty;
		if (input.stimType == "visual") {
			this.visStim = input.sprite;
			GetComponent<AudioSource> ().clip = Resources.Load<AudioClip> ("Audio/" + input.hostStim);
		}
		else if(input.stimType == "audio")
			this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/" + input.audio);
		this.stimType = input.stimType;
		this.option = false;
	}
	public void initSprite()
	{
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/" + visStim);
	}
	/// <summary>
	/// Initialization for customization options
	/// </summary>
	/// <param name="input">customization data</param>
	public void setOptions (serCustomizationOption input)
	{
		Sprite textureSprite = input.texture;
		this.isDraggable = input.isDraggable;
		this.GetComponent<SpriteRenderer>().sprite = textureSprite;
		if(textureSprite != null){	
			this.textureName = input.texture.name;
		} else {
			this.textureName = "blank";
		}
		this.optionBodyPart = input.bodyPart;
		this.option = true;
	}
	/// <summary>
	/// Handles touch event by user
	/// </summary>
	/// <param name="t">touch instance information</param>
	public void onSelect (touchInstance t)
	{	
		AudioSource audio = GetComponent<AudioSource>();
		if (audio.clip != null && t.getType() == eType.Tap)
		{
			audio.Play();

		}
			
	}
	void tokenTalk ()
	{
		token.talk ();
	}

	
	void Update () {

	}
}
