using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Serializable class to store stimulus data for question data persistance
/// </summary>
[Serializable]
public class serStim{
	public string audio;
	public string sprite;
	public bool isCorrect;
	public bool isDraggable;
	public Difficulty difficulty; 
}
/// <summary>
/// Serializable class to store customization event data for question data persistance
/// </summary>
public class serCustomizationOption{
	public string texture;
	public bool isDraggable;
	public int bodyPart;
}	


/// <summary>
/// A class to hold information about each question in the assessment.
/// Extends Scriptable Object.
/// </summary>

/*
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

/// <summary>
/// Init the question for a standard question event.
/// </summary>
/// <param name="num">Question Number</param>
/// <param name="sprites">Visual stimuli</param>
/// <param name="sounds">Auditory stimuli</param>
/// <param name="correct">Target stimulus</param>
/// <param name="cat">Question category</param>
	public void init (int qNum, string [] sprites, string [] sounds, int target, Category cat)
	{
		questionNumber = qNum;
		questionCat = cat;
		for (int i = 0; i<4; i++)
		{
			tempStim = new serStim();
			tempStim.audio = sounds [i];
			tempStim.sprite = sprites[i];
			if(i == target) 
				tempStim.isCorrect = true;
			else
				tempStim.isCorrect = false;
			tempStim.isDraggable = true;
			stimuli.Add(tempStim);
		}
		 
	}
	/// <summary>
	/// Init the question for a customization event
	/// </summary>
	/// <param name="num"> Question Number.</param>
	/// <param name="textures">Option Textures.</param>
	/// <param name="bodyPart">Index of body part being customized.</param>
	public void init (int qNum, string [] textures, int bodyPart)
	{
		questionNumber = qNum;
		questionCat = Category.Customization;
		for (int i = 0; i<4; i++)
		{
			tempOption = new serCustomizationOption();
			tempOption.texture = textures[i];
			tempOption.isDraggable = true;
			tempOption.bodyPart = bodyPart;
			options.Add(tempOption);
		}
		 
	}
	/// <summary>
	/// Gets the indicated stimulus
	/// </summary>
	/// <returns>indicated stimulus</returns>
	/// <param name="index">Index of desired stimulus</param>
	public serStim getStim (int index) {return stimuli[index];}
	/// <summary>
	/// Gets the indicated option.
	/// </summary>
	/// <returns>Indicated option.</returns>
	/// <param name="index">Index of desired option.</param>
	public serCustomizationOption getOption (int index) {return options[index];}
	/// <summary>
	/// Gets the question number.
	/// </summary>
	/// <returns>Question number.</returns>
	public int getNumber () {return questionNumber;}
	/// <summary>
	/// Gets the question category.
	/// </summary>
	/// <returns>Category enum.</returns>
	public Category getCategory () {return questionCat;}
}
