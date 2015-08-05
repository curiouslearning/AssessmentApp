using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/* [Serializable] serStim
 * serializable class to store stimulus data for question data persistance
 */
[Serializable]
public class serStim{
	public string audio;
	public string sprite;
	public bool isCorrect;
	public bool isDraggable;
	public Difficulty difficulty; 
}
public class serCustomizationOption{
	public string texture;
	public bool isDraggable;
	public int bodyPart;
}	

/* Question : Scriptable Object
 * A class to hold information about each question in the assessment
 * Contains:
 * 		serStim array of stimuli
 * 		prompt for host
 *		question number
 * initialized by fileWrapper, passed to Spawner by GameManagerScript for question initialization
 * Stored in a GameManagerScript Queue
 */
public class Question : ScriptableObject {

	int questionNumber;
	List<serStim> stimuli;
	List<serCustomizationOption> options;
	serCustomizationOption tempOption;
	serStim tempStim;
	serStim prompt;
	Category questionCat;
 
	
	void Awake ()
	{
		stimuli = new List<serStim>();
		options = new List<serCustomizationOption>();
	}

	public void init (int num, string [] sprites, string [] sounds, int correct, Category cat)
	{
		questionNumber = num;
		questionCat = cat;
		for (int i = 0; i<4; i++)
		{
			tempStim = new serStim();
			tempStim.audio = sounds [i];
			tempStim.sprite = sprites[i];
			if(i == correct) 
				tempStim.isCorrect = true;
			else
				tempStim.isCorrect = false;
			tempStim.isDraggable = true;
			stimuli.Add(tempStim);
		}
		 
	}
		public void init (int num, string [] textures, int bodyPart, Category cat)
	{
		questionNumber = num;
		questionCat = cat;
		for (int i = 0; i<4; i++)
		{
			tempOption = new serCustomizationOption();
			tempOption.texture = textures[i];
			tempOption.isDraggable = true;
			tempOption.bodyPart = bodyPart;
			options.Add(tempOption);
		}
		 
	}
	public serStim getStim (int index) {return stimuli[index];}
	public serCustomizationOption getOption (int index) {return options[index];}
	public int getNumber () {return questionNumber;}
	public Category getCategory () {return questionCat;}
}
