using UnityEngine;
using System.Collections;

/* StimulusScript
 * Class containg data and functionality for individual stimuli 
 * initialized by the SpawnerScript using Question instances
 * attached to the SOO instance as child objects
 */

public enum Difficulty {Easy, Medium, Hard};

public class StimulusScript : MonoBehaviour{

	private bool isCorrect; //bool for indicating the correct stimulus response in a question
	private bool isBeingDragged; 
	private bool isDraggable;
	private Difficulty difficulty;
	private Vector3 homePos; //snapback functionality
	private Vector3 startScale; //scaling functionality
	public Selectable touchInput;

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

	public Vector3 returnHomePos() {
		return homePos;  
	}

	public Difficulty returnDifficulty() {
		return difficulty;
	}


//*******************
// Setter functions *
//*******************

	public void setIsCorrect(bool input) {
		isCorrect = input;
	}

	public void setIsBeingDragged(bool input) {
		isBeingDragged = input;
	}

	public void setIsDraggable(bool input) {
		isDraggable = input;
	}

	public void setHomePos() {
		homePos = transform.position;
	}

	public void setDifficulty(Difficulty newDiff) {
		difficulty = newDiff;
	}

//********************
// Scaling functions *
//********************

	public void scaleToTarget (float mod)
	{
		transform.localScale = startScale*mod;
	}

	public void resetScale ()
	{
		transform.localScale = startScale;
	}

//initialization function
	public	void setStim (serStim input) {
		this.isCorrect = input.isCorrect;
		this.isDraggable = input.isDraggable;
		this.difficulty = input.difficulty;
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Art/" + input.sprite);
		this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/" + input.audio);
	}
	
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
