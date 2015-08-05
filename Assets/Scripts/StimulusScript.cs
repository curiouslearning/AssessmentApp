using UnityEngine;
using System.Collections;


/// <summary>
/// Inidicator for question difficulty.
/// </summary>
public enum Difficulty {Easy, Medium, Hard};

/// <summary>
/// StimulusScript
/// Class containg data and functionality for individual stimuli initialized by the SpawnerScript using Question instances.
/// Attached to the SOO instance as child objects.
/// </summary>
public class StimulusScript : MonoBehaviour{

	private bool isCorrect; //bool for indicating the correct stimulus response in a question
	private bool option;
	private bool isBeingDragged; 
	private bool isDraggable;
	private Difficulty difficulty;
	private Vector3 homePos; //snapback functionality
	private Vector3 startScale; //scaling functionality
	public Selectable touchInput;
	private int optionBodyPart;
	string textureName;	

	void Start ()
	{
		startScale = transform.localScale*0.6f;
		isBeingDragged = false;
		touchInput.initP(onSelect);
	}

//*******************
// Getter functions *
//*******************

	public bool returnIsCorrect() {
		return isCorrect;
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
	/// Indicate whether or not a specific stimulus is the correct answer.
	/// </summary>
	/// <param name="input">Value to set isCorrect to.</param>
	public void setIsCorrect(bool input) {
		isCorrect = input;
	}

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
	}
	/// <summary>
	/// Sets the difficulty to the question's difficulty level.
	/// </summary>
	/// <param name="newDiff">difficulty level</param>
	public void setDifficulty(Difficulty newDiff) {
		difficulty = newDiff;
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

	/// <summary>
	/// Initialization function for stimuli.
	/// </summary>
	/// <param name="input">stimulus data.</param>
	public	void setStim (serStim input) {
		this.isCorrect = input.isCorrect;
		this.isDraggable = input.isDraggable;
		this.difficulty = input.difficulty;
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/" + input.sprite);
		this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/" + input.audio);
		this.option = false;
	}
	/// <summary>
	/// Initialization for customization options
	/// </summary>
	/// <param name="input">customization data</param>
	public void setOptions (serCustomizationOption input)
	{
		Texture2D texture = Resources.Load<Texture2D>("Textures/" + input.texture);
		Sprite textureSprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
		this.isDraggable = input.isDraggable;
		this.GetComponent<SpriteRenderer>().sprite = textureSprite;
		this.textureName = input.texture; 
		this.optionBodyPart = input.bodyPart;
		this.option = true;
	}
	/// <summary>
	/// Handles touch event by user
	/// </summary>
	/// <param name="t">touch instance information</param>
	public void onSelect (touchInstance t)
	{
		Debug.Log("successful selection!");
		Debug.Log("There was a " + t.getType() + " of length " + t.getTime());
		//Animation stuff here
		//sound stuff here
	}
	void Update () {

	}
}
