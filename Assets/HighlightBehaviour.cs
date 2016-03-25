using UnityEngine;
using System.Collections;

public class HighlightBehaviour : StateMachineBehaviour {
	Mesh highlight;
	Vector3[] vertices;
	Color[] colors;
	public Color newColor;
	Color oldColor;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	/*override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		SkinnedMeshRenderer r = animator.GetComponentInChildren<SkinnedMeshRenderer> ();
		StartCoroutine (Flasher);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	IEnumerator flash (SkinnedMeshRenderer renderer){
		for (int i = 0; i < 5; i++)
			{
				renderer.material.color = newColor;
				yield return new WaitForSeconds(.1f);
				renderer.material.color = oldColor; 
				yield return new WaitForSeconds(.1f);
			}
	}*/

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
