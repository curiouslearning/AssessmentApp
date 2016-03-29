using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SetID : MonoBehaviour {

	private string id;
	GameObject g;

	void Start ()
	{
		Debug.Log ("start!");
		DontDestroyOnLoad (this);
	}

	public void setID (Text s)
	{
		id = s.ToString();
	}
	public void nextLevel() //change the level after setting the id
	{
		SceneManager.LoadScene ("Main");	
	}
	void OnLevelWasLoaded ()
	{
		if (Scene.Equals (SceneManager.GetActiveScene (), SceneManager.GetSceneByName ("Main"))) {
			g = GameObject.Find ("Main Camera");
			g.GetComponent<ScoreTracker> ().setUser (id);
			Debug.Log ("Successful Pass!");
			Destroy (this.gameObject);
		}
	}
}
