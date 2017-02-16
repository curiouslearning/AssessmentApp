using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InstructionAudio : Observer {
	public AudioSource source;
	AudioClip firstStatement;
	AudioClip secondStatement;
	Subject eventHandler;
	public ScoreTracker scoring;
	Category currentCategory;
	public MainCharacter character;
	public Subject charSubject;

	void Start () {
		source = GetComponent<AudioSource> ();
		eventHandler = GetComponent<Subject> ();
		eventHandler.addObserver (new Subject.GameObjectNotify (this.onNotify));
		charSubject.addObserver (new Subject.GameObjectNotify (this.onNotify));
		firstStatement = Resources.Load<AudioClip> ("Audio/customization");
		if (firstStatement == null) {
		}
	}

	public override void onNotify (EventInstance<GameObject> e){
		switch (e.type){
		case eType.CategoryChange:
			currentCategory = scoring.queryCategory ();
			updateAudio ();
			break;
		case eType.Ready:
			if (currentCategory != scoring.queryCategory ()) {
				currentCategory = scoring.queryCategory ();
				updateAudio ();
			}
			StartCoroutine (playWholeStatement ());
			break;	
		case eType.StartAudio:
			if (currentCategory == Category.Customization) {
				StartCoroutine(playWholeStatement ());
			} else {
				StartCoroutine(playHalfStatement ());
			}
			break;
		default:
			break;
		}
	}
	void playAudio(AudioClip statement){
		if (statement == null) {
			Debug.LogError ("null statement!");
		}
			if (!source.isPlaying) {
				Debug.Log ("Playing: " + statement.name);
				source.clip = statement;
				source.Play ();
			}
	}
	IEnumerator playHalfStatement()
	{
		playAudio (secondStatement);
		yield return new WaitForSeconds (secondStatement.length);
		character.playAudio ();
	}
	IEnumerator playWholeStatement(){
		playAudio (firstStatement);
		yield return new WaitForSeconds(firstStatement.length);
		if (currentCategory != Category.Customization) { //Customizations only need one recording
			character.playAudio ();
			yield return new WaitForSeconds (2);
			playAudio (secondStatement);
			yield return new WaitForSeconds(secondStatement.length);
			character.playAudio ();
		}
	}


	void updateAudio () {
		switch (currentCategory) {
		case Category.Customization:
			firstStatement = Resources.Load <AudioClip> ("Audio/customization");
			secondStatement = null;
			break;		
		case Category.ReceptiveVocabulary:
			firstStatement = Resources.Load <AudioClip>("Audio/receptive_vocabulary_1");
			secondStatement = Resources.Load <AudioClip>("Audio/receptive_vocabulary_2");
			break;
		case Category.LetterNameRecognition:
			firstStatement = Resources.Load <AudioClip>("Audio/letter_name_recognition_1");
			secondStatement = Resources.Load <AudioClip>("Audio/letter_name_recognition_2");
			break;
		case Category.LetterSoundMatching:
			firstStatement = Resources.Load<AudioClip> ("Audio/letter_sound_matching_1");
			secondStatement = Resources.Load <AudioClip>("Audio/letter_sound_matching_2");
			break;
		case Category.CVCWordIdentification:
			firstStatement = Resources.Load <AudioClip>("Audio/cvc_word_identification_1");
			secondStatement = Resources.Load<AudioClip> ("Audio/cvc_word_identification_2");
			break;
		case Category.BlendingWordIdentification:
			firstStatement = Resources.Load<AudioClip> ("Audio/blending_word_identification_1");
			secondStatement = Resources.Load<AudioClip> ("Audio/blending_word_identification_2");
			break;
		case Category.SightWordIdentification:
			firstStatement = Resources.Load<AudioClip> ("Audio/sight_word_identification_1");
			secondStatement = Resources.Load<AudioClip> ("Audio/sight_word_identification_2");
			break;
		case Category.RhymingWordMatching:
			firstStatement = Resources.Load<AudioClip> ("Audio/rhyming_word_matching_1");
			secondStatement = Resources.Load<AudioClip> ("/Audio/rhyming_word_matching_2");
			break;
		case Category.PseudowordMatching:
			firstStatement = Resources.Load<AudioClip> ("Audio/pseudoword_matching_1");
			secondStatement = Resources.Load<AudioClip> ("Audio/pseudoword_matching_2");
			break;
		default:
			break;
		}	
	}
}
