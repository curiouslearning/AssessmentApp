using UnityEngine;
using System.Collections;

public class TestAnim : MonoBehaviour {

	Animator a;
	// Use this for initialization
	float count;
	void Start () {
		a = GameObject.Find("MainCharacter").GetComponent<Animator>();
		
			
	}


	bool isPlaying (string b)
	{
		return (a.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Idle"));
	}
	/// <summary>
	/// Begin cycling the highlight animation.
	/// </summary>
	public void runAnimations (string b, bool reset)
	{
		bool done = false;
		//this.gameObject.SetActive(true);
		while (!done)
		{
			if(isPlaying (b)) //prevent multiple calls from interrupting animation
			{
				done = true;
				if(reset)
					a.ResetTrigger(b);
				else
					a.SetTrigger(b);
			}
			else
			{
				continue;
			}
		}
		
	}	
	// Update is called once per frame
	void Update () {
		if (count  >= 5f){
			runAnimations("Fly", true);
			runAnimations("Fly", false);
			runAnimations("Skip", true);
			runAnimations("Skip", false);
			runAnimations("GrabBasket", true);
			runAnimations("GrabBasket", false);
			runAnimations("Throw", true);
			runAnimations("Throw", false);
			count = 0f;
		}
		else{
			count += Time.deltaTime;
		}
	}
}
