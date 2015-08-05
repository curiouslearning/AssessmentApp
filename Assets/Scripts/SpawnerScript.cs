using UnityEngine;
using System.Collections;

/* SpawnerScript
 * Class used to initialize Stimulus Organization Object (SOO) and stimuli
 * for each new question
 * Gets question information from Question Queue
 * Gets SOO and stimulus positioning info from Editor
 */
public class SpawnerScript : MonoBehaviour {
	GameObject newSoo;
	GameObject[] newStims;
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
	// Use this for initialization
	void Awake () {
		newStims = new GameObject[4];
		positions = new Vector3[4]; 
	}
	//to do: split SOO and stimuli into layers
	public GameObject spawnNext (Question q)
	{
		Debug.Log("question cat is: " + q.getCategory());
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
			};
			positions[i] = transform.position;
			if(q.getCategory() == Category.Customization)
			{
				Debug.Log("pulling from options");
				newStims[i].GetComponent<StimulusScript>().setOptions(q.getOption(i));  
			}
			else
			{
				Debug.Log("pulling from stimuli");
				newStims[i].GetComponent<StimulusScript>().setStim(q.getStim(i));  
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