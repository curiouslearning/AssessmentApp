using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Category {ReceptiveVocabulary, 
				      LetterNameRecognition, 
				      LetterSoundMatching, 
					  CVCWordIdentification, 
					  SightWordIdentification, 
	                  RhymingWordMatching,
	                  BlendingWordIdentification, 
	                  PseudowordMatching};   

/* GameManager Class
 * Centralized data storage and functionality for Question iteration
 * Contains a queue of Questions, pointers to the spawner, Garbage Collector
 * and current Stimulus Organizational Object
 */
public class GameManagerScript : Observer {
/* Component instances
 * Instances to create/add:
 * Animation Handler
 * Character Customizer
 * File Wrapper
 * Scoring system
 */
//timekeeping variables
public int questionNumber;
public float questionTime;
float startTime;

//component variables
public GameObject spawner;
SpawnerScript spawnHolder;
public GameObject receptacle;
public GameObject gCollector;
public ScoreTracker scoreHolder;
GameObject stimOrgOb;
SOOScript sooHolder;
Difficulty currentDifficulty; 
Category currentCategory;

Queue<Question> qList;
//Event variables
public Subject eventHandler;
int eTester; //debugger
	// Use this for initialization
	void Start () {
		eTester = 0; //debugger
		CollisionNotification trashHolder;
		spawnHolder = spawner.GetComponent<SpawnerScript>();
		trashHolder = receptacle.GetComponent<CollisionNotification>();
		trashHolder.sub.addObserver(new Subject.GameObjectNotify(this.onNotify));
		trashHolder = gCollector.GetComponent<CollisionNotification>();	
		trashHolder.sub.addObserver(new Subject.GameObjectNotify(this.onNotify));
		scoreHolder.GetComponent<Subject>().addObserver(new Subject.scoreTrackerNotify (this.onNotify));
		questionNumber = 0;
		questionTime = 0f;
		startTime = Time.time;
		initQList ();
		startQuestion(); 
		currentDifficulty = Difficulty.Easy;
		currentCategory = Category.ReceptiveVocabulary;
	}

//Gets queue of Questions from FileWrapper
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
			
			Debug.Log("going through the next question"); //debugger
			changeQuestion();
			startQuestion();
			return; //prevent repeated action on same event
		}
		else if (e.type == eType.Selected)
		{
			Debug.Log("got event from: " + e.signaler.name); //debugger
			e.signaler.gameObject.SetActive(false);
			sooHolder.move(1);
			return;
		}
	}

    public override void onNotify (EventInstance<ScoreTracker> e) {
		Debug.Log ("this is call " + eTester++);
		if (e.type == eType.ChangeDifficulty) {
			Debug.Log ("got a ChangeDifficulty event from " + e.signaler.name); // debugger
			if (currentDifficulty == Difficulty.Hard) {
				currentCategory++;
			} else 
				currentDifficulty++;
			    return;
		} else if (e.type == eType.ChangeCategory) {
			Debug.Log ("got a ChangeCategory event from " + e.signaler.name); // debugger
			currentCategory++;
		}
	}

//Dequeue the next question and initialize the question period
	void startQuestion ()
	{
		if(qList.Count == 0){ 
				endGame();
				return;
		}
	stimOrgOb = spawnHolder.spawnNext(qList.Dequeue());
	Debug.Log("got new SOO"); //debugger
	sooHolder = stimOrgOb.GetComponent<SOOScript>();
	sooHolder.move(0);
	}


	void changeQuestion ()
	{
		Debug.Log("we're in changeQuestion!");
		questionTime = 0;
		startTime = Time.time;
		questionNumber++;
		EventInstance<GameManagerScript> e;
		e = new EventInstance<GameManagerScript>();
		e.setEvent(eType.NewQuestion, this);
		eventHandler.notify(e);
		//Update the various gamecounts like time between touches (for idle triggering purposes)
	}
	void endGame ()
	{
		EventInstance<GameManagerScript> e;
		e = new EventInstance<GameManagerScript>();
		e.setEvent (eType.EndGame, this);
		eventHandler.notify(e);
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
		questionTime = Time.time - startTime;
		//if scene is changing do not process input
		//otherwise generate input commands and pass them to the proper objects
	}
}
