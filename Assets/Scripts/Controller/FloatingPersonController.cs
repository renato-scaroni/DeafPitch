using UnityEngine;
using System.Collections;
using Spine.Unity;

public class FloatingPersonController : PersonController 
{
	private readonly string deathAnimation = "dead";

	private SkeletonAnimation skeletonAnimation;
	private Spine.AnimationState spineAnimationState;
	private bool firstPlay = true;
	 private enum AnimationStates {death, swim, none};
	
	private AnimationStates currentState;

	void AnimationComplete(Spine.AnimationState state, int trackIndex, int loopCount)
	{
		if(currentState == AnimationStates.death)
		{
			deathAnim.SetActive(false);
			currentState = AnimationStates.none;	
			gameObject.SetActive(false);
		}
	}

	public override void SetDeathAnimation ()
	{
		swimAnim.SetActive(false);
		deathAnim.SetActive(true);
		currentState = AnimationStates.death;
		spineAnimationState.SetAnimation(0, deathAnimation, true);
	}
	
	public override void SetSwimAnimation ()
	{
		swimAnim.SetActive(true);
		deathAnim.SetActive(false);
		currentState = AnimationStates.swim;
	}

	void OnEnable ()
	{
		if(!firstPlay)
			SetSwimAnimation();
	}

	void Start ()
	{
		skeletonAnimation = deathAnim.GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		SetSwimAnimation();
		currentState = AnimationStates.none;
		spineAnimationState.Complete += AnimationComplete;
		firstPlay = false;
	}
}
