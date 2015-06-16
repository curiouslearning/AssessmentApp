using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {
/* Component instances
 * Instances to create/add:
 * Input Processor
 * Animation Handler
 * SOO handler
 * Character Customizer
 * File Wrapper
 * Scoring system
 * Spawner
 *
 */
	// Use this for initialization
	void Start () {
	
	}
//game iterating functions
	void nextQuestion()
	{
		//load the next question and the scene updating process
	}
//top down information passing functions	
	void generateCmd ()
	{
		//takes input an object and generates the proper command for that object given the scene parameters
	}

	void updateCounts ()
	{
		//Update the various gamecounts like time between touches (for idle triggering purposes)
	}

	
//Observer/Event handling functions

//Data handling functions
	void Save ()
	{
	}
	void Load()
	{
	}
	
	void Update () {
		//if scene is changing do not process input
		//otherwise generate input commands and pass them to the proper objects
		updateCounts ();	
	}
}
