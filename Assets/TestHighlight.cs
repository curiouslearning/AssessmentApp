using UnityEngine;
using System.Collections;

public class TestHighlight : MonoBehaviour {

	public Highlighter highlighter;
	float count;	
	// Use this for initialization
	void Start () {
		count = 0;
		highlighter.highlight();	
	}
	
	// Update is called once per frame
	void Update () {
		if (count >= 15f){
			highlighter.reset();
			return;
		}
		else {
			count += Time.deltaTime;
		}
	
	}
}
