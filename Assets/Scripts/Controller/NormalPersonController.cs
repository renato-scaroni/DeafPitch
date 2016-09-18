using UnityEngine;
using System.Collections;
using Spine.Unity;

public class NormalPersonController : PersonController 
{
	private readonly string deathAnimation = "dead";
	private Animator animator;
	public float desperateSpeed = .06f;
	public float normalSpeed = .03f;
	public GameObject alert;
	public float totalAlertTime = 2f;
 	
	 private enum AnimationStates {death, swim, none};
	
	private AnimationStates currentState;

	private SkeletonAnimation skeletonAnimation;
	private Spine.AnimationState spineAnimationState;
	private bool firstPlay = true;

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
		alert.SetActive(false);
		currentState = AnimationStates.swim;
	}
	

	public IEnumerator AlertState()
	{
		yield return new WaitForSeconds(totalAlertTime);
		alert.SetActive(false);
	}

	void OnAlert()
	{
		alert.SetActive(true);
	}

	void OnDesperate()
	{
		speed = desperateSpeed;
		animator.SetBool("Desperate", true);
	}

	void OnEnable ()
	{
		alert.SetActive(false);
		speed = normalSpeed;
		PlayerController.OnDesperate += OnDesperate;
		PlayerController.OnAlert += OnAlert;
		if(!firstPlay)
			SetSwimAnimation();
	}

	void AnimationComplete(Spine.AnimationState state, int trackIndex, int loopCount)
	{
		if(currentState == AnimationStates.death)
		{
			deathAnim.SetActive(false);
			currentState = AnimationStates.none;	
			gameObject.SetActive(false);
			alert.SetActive(false);
		}
	}

	void Start ()
	{
		animator = swimAnim.GetComponent<Animator>();
		skeletonAnimation = deathAnim.GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		SetSwimAnimation();
		currentState = AnimationStates.none;
		spineAnimationState.Complete += AnimationComplete;
		firstPlay = false;
	}

	void OnDisable ()
	{
		PlayerController.OnDesperate -= OnDesperate;
		PlayerController.OnAlert -= OnAlert;
	}
}
