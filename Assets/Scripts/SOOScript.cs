using UnityEngine;
using System.Collections;

public class SOOScript : Observer {

	private GameObject[] stimArray = new GameObject[4];
	private Vector3 [] posArray = new Vector3[4];
	private Vector3[] destArray = new Vector3[4];
	private int questionNumber;
	bool isDraggable;
	Vector3 curDest;
	public float speed;
	float distance;
	bool moving;
	void Start () {
		isDraggable = false;
	}

	// Methods for accessing variables
	public bool getDrag ()
	{
		return isDraggable;
	}
	
	public int getQNumber ()
	{
		return questionNumber;
	}

	public GameObject[] returnStimArray() {
		return stimArray;
	}

	public Vector3[] returnPosArray() {
		return posArray;
	}

	public Vector3[] returnDestArray() {
		return destArray;
	}

	// Methods for setting variables
	
	public void setQNumber (int num)
	{
		questionNumber = num;
	}

	public void setStimArray(GameObject[] input) {
		stimArray = input;
	}

	public void setPosArray(Vector3[] input) {
		posArray = input;
	}

	public void setDestArray(Vector3[] input) {
		destArray = input;
	}
	

	public void setPos (Vector3 pos)
	{
		transform.position = pos;
	}
	 
	public override void onNotify (EventInstance<GameObject> e)
	{
		Debug.Log("boo"); //debugger
		//releaseStim(e.signaler);
	}
	
//move the SOO to the next destination

	public void move(int dest)
	{
		moving = true;
		curDest = destArray[dest];
		distance = Vector3.Distance(transform.position, curDest);
	}

	void updatePos()
	{
		for (int i = 0; i < stimArray.Length; i++)
		{
			if (stimArray[i] != null)
				stimArray[i].GetComponent<StimulusScript>().setHomePos();
		}
	}
	
	//initialization function
	public void setSoo (GameObject[] array, int qNum) {
		stimArray = array;
		questionNumber = qNum;
	}
	void Update()
	{
		if(moving == true)
		{
			transform.position = Vector3.Lerp(transform.position, curDest, Time.deltaTime * speed);
			updatePos();
		}
		if(transform.position == curDest)
		{
			moving = false;
		}
	}
		
			

}
