using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class serStim{
	public string audio;
	public string sprite;
	public bool isCorrect;
	public bool isDraggable;
}
public class Question : ScriptableObject {

	int questionNumber;
	List<serStim> stimuli;
	serStim temp;
	
	void Awake ()
	{
		stimuli = new List<serStim>();
	}

	public void init (int num, string [] sprites, string [] sounds, int correct)
	{
		questionNumber = num;
		for (int i = 0; i<4; i++)
		{
			temp = new serStim();
			temp.audio = sounds [i];
			temp.sprite = sprites[i];
			if(i == correct)
				temp.isCorrect = true;
			else
				temp.isCorrect = false;
			temp.isDraggable = true;
			stimuli.Add(temp);
		}
		 
	}
	public serStim getStim (int index) {return stimuli[index];}
	public int getNumber () {return questionNumber;}
}
