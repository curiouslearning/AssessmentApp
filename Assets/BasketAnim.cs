using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasketAnim : AnimationManager{
	Highlighter h;
	public GameObject[] tokens;
	Animator[] tokenControllers;
	// Use this for initialization
	void Start () {
		base.addSelfToSubjects ();
		h = GetComponentInChildren<Highlighter> ();
		if (h == null) {
			Debug.Log ("null highlighter");
		}
		grabControllers ();
	}
	void grabControllers ()
	{
		tokenControllers = new Animator[tokens.Length];
		for (int i= 0; i < tokens.Length; i++)
		{
			tokenControllers[i] = tokens[i].GetComponent<Animator>();
		}
		deactivateTokens ();
	}
	public override void onNotify(EventInstance<GameObject> e)
	{
		if (e.type == eType.NewQuestion) {
			currentCategory = scoreTracker.queryCategory ();
			if (currentCategory == Category.Customization) {
				h.toggleActive (false);
			} else {
				h.toggleActive (true);
			}
		} else if (e.type == eType.Transition) {
			deactivateTokens ();
		}else if (e.type == eType.Selected) {
			//initNextToken ();
		}
	}

	//activate a given token
	void initToken(int i)
	{
		if (i < tokens.Length) {
			tokens [i].SetActive (true);
		}
	}
	//activate all 4 tokens
	void initTokens ()
	{
		for (int i = 0; i < tokens.Length; i++) {
			tokens [i].SetActive (true);
		}
	}
	/// <summary>
	/// Activates the next token in the basket.
	/// </summary>
	public void initNextToken ()
	{
		if (tokens [tokens.Length - 1].activeSelf) {
			return; //don't execute on a fully activated basket
		}
		int i = 0;
		while (tokens[i].activeSelf && i < tokens.Length)
		{
			i++;
		}
		tokens [i].SetActive (true);
	}
	//deactivate all 4 tokens
	void deactivateTokens ()
	{
		for (int i = 0; i < tokens.Length; i++) {
			tokens [i].SetActive (false);
		}
	}
	/// <summary>
	///call the specified animation on all tokens in the basket
	/// </summary>
	/// <param name="s">S.</param>
	public void setTokens (string s)
	{
		initTokens (); //make sure all tokens are active
		for (int i = 0; i < tokenControllers.Length; i++) {
			tokenControllers[i].SetTrigger (s);
		}

	}
	// Update is called once per frame
	void Update () {
	
	}
}
