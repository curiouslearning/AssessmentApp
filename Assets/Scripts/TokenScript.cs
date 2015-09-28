using UnityEngine;
using System.Collections;

public class TokenScript : MonoBehaviour {

	bool isCorrect;
	
	void start() {
		isCorrect = false;
	}
	
	public void setIsCorrect (bool val)
	{
		isCorrect = val;
	}
	
	public bool returnIsCorrect ()
	{
		return isCorrect;
	}
}
