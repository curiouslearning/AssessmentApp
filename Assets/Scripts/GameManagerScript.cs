using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManagerScript : Observer {
/* Component instances
 * Instances to create/add:
 * Input Processor
 * Animation Handler
 * SOO handler
 * Character Customizer
 * File Wrapper
 * Scoring system
 */
public GameObject spawner;
public GameObject receptacle;
public GameObject gCollector;
SpawnerScript spawnHolder;
GameObject stimOrgOb;
SOOScript sooHolder;
Queue<Question> qList;
int eTester; //debugger
	// Use this for initialization
	void Start () {
		eTester = 0; //debugger
		CollisionNotification trashHolder;
		spawnHolder = spawner.GetComponent<SpawnerScript>();
		trashHolder = receptacle.GetComponent<CollisionNotification>();
		trashHolder.sub.addObserver(this);
		trashHolder = gCollector.GetComponent<CollisionNotification>();	
		trashHolder.sub.addObserver(this);
		initQList ();
		startQuestion();
	}
	void initQList ()
	{ // SUPER HACK, YOU MUST GO  BACK TO THIS
		string [] sprites1 = new string [] {"sprite0", "sprite1", "sprite2", "sprite3"};
		string [] sprites2 = new string [] {"sprite4", "sprite5", "sprite6", "sprite7"};
		string [] sounds = new string[] {"","","",""};
		qList = new Queue<Question>();
		Question temp = Question.CreateInstance<Question>();
		temp.init(0,sprites1, sounds, 1);
		qList.Enqueue(temp);
		temp = Question.CreateInstance<Question>();
		temp.init (1, sprites2, sounds, 2);
		qList.Enqueue(temp);
	}
	public override void onNotify (EventInstance<GameObject> e)
	{
		Debug.Log("this is call " + eTester++); //debugger
		if (e.type == eType.Trashed)
		{
			Destroy(e.signaler);
			//don't end the world
			if(qList.Count == 0)
				return;
			Debug.Log("going through the next question"); //debugger
			startQuestion();
			return; //prevent repeated action on same event
		}
		else if (e.type == eType.Selected);
		{
			Debug.Log("got event from: " + e.signaler.name); //debugger
			Destroy(e.signaler);
			sooHolder.move(1);
			return;
		}
	}
	void startQuestion ()
	{
	stimOrgOb = spawnHolder.spawnNext(qList.Dequeue());
	Debug.Log("got new SOO"); //debugger
	sooHolder = stimOrgOb.GetComponent<SOOScript>();
	sooHolder.move(0);
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
