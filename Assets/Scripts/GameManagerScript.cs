using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
// Category is an enum that holds all possible kinds of
// questions that the app may ask a student.  Customization
// is included as a category so the character customization
// at the beginning of gameplay will be recognized as its
// own kind of activity;
public enum Category {Customization,
	                  ReceptiveVocabulary, 
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
		currentCategory = Category.Customization;
		initQList ();
		startQuestion(); 
		currentDifficulty = Difficulty.Easy;
		Debug.Log ("Current category/difficulty is " + currentCategory + "/" + currentDifficulty); 
	}

//Gets queue of Questions from FileWrapper
	void initQList ()
	{ // SUPER HACK, YOU MUST GO  BACK TO THIS
		string [] textures = new string[] {"AngryFace1", "HappyFace1", "NeutralFace1", "HappyFace2"};
		string [] sprites1 = new string [] {"sprite0", "sprite1", "sprite2", "sprite3"};
		string [] sprites2 = new string [] {"sprite4", "sprite5", "sprite6", "sprite7"};
		string [] sounds = new string[] {"","","",""};
		qList = new Queue<Question>();
		Question temp = Question.CreateInstance<Question>();
		temp.init(0,textures, 2);
		qList.Enqueue(temp);
		currentCategory = Category.ReceptiveVocabulary;
		temp = Question.CreateInstance<Question>();
		temp.init(1,sprites1, sounds, 1, currentCategory);
		qList.Enqueue(temp);
		temp = Question.CreateInstance<Question>();
		temp.init (2, sprites2, sounds, 2, currentCategory);
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
		// this onNotify handles event notifications sent from ScoreTracker and changes 
		// the currentDifficulty and currentCategory vasriables accordingly
		Debug.Log ("this is call " + eTester++);
		if (e.type == eType.ChangeDifficulty) {
			// ChangeDifficulty events are sent when the student has answered three
			// questions correctly in a row.  If the current difficulty is "hard",
			// the game moves on to a new category of question and the difficulty is
			// reset to "easy" in the new category
			Debug.Log ("got a ChangeDifficulty event from " + e.signaler.name); // debugger
			if (currentDifficulty == Difficulty.Hard) {
				currentCategory++;
				currentDifficulty = Difficulty.Easy;
				return;
			} else{ 
				currentDifficulty++;
			    Debug.Log ("current Category is " + currentCategory + " and current difficulty is " + currentDifficulty); // debugger
			    return;
			}
		} else if (e.type == eType.ChangeCategory) {
			// ChangeCategory events are sent when the student gets four consecutive
			// questions wrong or maxes out the number of questions per category (20).
			// When the category is incremented, difficulty is reset to easy.
			Debug.Log ("got a ChangeCategory event from " + e.signaler.name); // debugger
			currentCategory++;
			currentDifficulty = Difficulty.Easy;
			Debug.Log ("current Category is " + currentCategory + " and current difficulty is " + currentDifficulty); // debugger
			return;
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
		Debug.Log ("current Category is " + currentCategory + " and current difficulty is " + currentDifficulty);
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

	
	public Category getCurrentCategory ()
	{
		return currentCategory;
	}
	public Difficulty getCurrentDifficulty ()
	{
		return currentDifficulty;
	}

//Data handling functions
	void Save ()
	{
	}
	void Load()
	{
	}
	
	void Update () {
		// questionTime keeps track of the elapsed time since the
		// start of the current question.  It must be updated
		// frequently, which is why it is placed in Update().
		questionTime = Time.time - startTime;
		// if questionTime goes over 15 seconds it sends a
		// TimeOut (which will be picked up in ScoreTracker)
		// event and moves on to the next question, calling
		// move(1) on sooHolder
		if (questionTime >= 15.0f) {
			startTime = Time.time;
			EventInstance<GameManagerScript> e;
			e = new EventInstance<GameManagerScript>();
			e.setEvent (eType.Timeout, this);
			eventHandler.notify(e);
			Debug.Log ("Sent Timeout notification to ScoreTracker");
			sooHolder.move (1);
		}
		//if scene is changing do not process input
		//otherwise generate input commands and pass them to the proper objects
	}
}
