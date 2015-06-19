using UnityEngine;
using System.Collections;

public class SOOScript : MonoBehaviour {

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
	 
//tell SOO to forget about deleted stimulus
	public void releaseStim(GameObject stim) {
		GameObject[] temp = new GameObject[stimArray.Length];
		for (int i = 0; i < stimArray.Length; i++) {
			if (stimArray[i].Equals (stim)) {
				i++;
			} else temp[i] = stimArray[i];
		} 
		stimArray = temp;
	}

//move the SOO to the next destination

	public void move(int dest)
	{
		Debug.Log("moving now true");
		moving = true;
		curDest = destArray[dest];
		distance = Vector3.Distance(transform.position, curDest);
	}
	
	//initialization function
	public void setSoo (GameObject[] array, int qNum) {
		this.stimArray = array;
		questionNumber = qNum;
	}
	void Update()
	{
		if(moving == true)
		{
			Debug.Log("yes moving!");
			transform.position = Vector3.Lerp(transform.position, curDest, Time.deltaTime * speed/distance);
		}
		if(transform.position == curDest)
		{
			Debug.Log("moving is false");
			moving = false;
		}
	}
		
			

}
