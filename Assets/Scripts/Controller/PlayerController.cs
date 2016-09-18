using UnityEngine;
using System.Collections;
using Spine.Unity;

public class PlayerController : MonoBehaviour 
{
	public PlayerController instance
	{
		get;
		private set;
	}

	public float minSpeed = 0.01f;
	public float maxSpeed = 1f;
	public float minHeight = -4.5f; 
	public float maxHeight = 0;
	public float maxInputDiff = 1f;
	public bool alertState = false;
	public float alertStateTime;

	public float deacelerationDelta = 0.1f;
	public float startDeaceleration = 15;
	public float minDeaceleration = 0.1f;
	private float currentDeaceleration = 10;

	public float maxSpeedDelta = 0.0005f;
	public float currentSpeed = 0.01f;
	public float alertThreshold = 0.2f;

	private float lastMoveInput1 = 0;
	private float lastMoveInput2 = 0.1f;
	private float speedFactor = 0;	


	// Input order variables
	public bool DEBUG = false;

	private KeyCode moveKeyboardInput1 = KeyCode.Z;
	private KeyCode moveKeyboardInput2 = KeyCode.X;
	private MicHandle.AvailableInputs moveAudioInput1 = MicHandle.AvailableInputs.Swim1;
	private MicHandle.AvailableInputs moveAudioInput2 = MicHandle.AvailableInputs.Swim2;

	private KeyCode lastKeyboardInput = KeyCode.Space;
	private MicHandle.AvailableInputs lastAudioInput = MicHandle.AvailableInputs.None;

	
	// Spine Variables
	private SkeletonAnimation skeletonAnimation;
	private Spine.AnimationState spineAnimationState;
	// private Spine.Skeleton skeleton;

	private string swimAnimation = "idle";
	private string attackAnimation = "bite";

	public float maxAnimationScale = 3;
	public float minAnimationScale = 1;

	// Person Interactions
	public delegate void AtePersonHandler();
	public static event AtePersonHandler OnAtePerson;

	public delegate void AlertHandler();
	public static event AlertHandler OnAlert;

	public delegate void DesperateHandler();
	public static event DesperateHandler OnDesperate;

	enum PlayerState {bite, swimming};
	private PlayerState currentState;



	private bool checkInput(KeyCode moveKeyboardInput, MicHandle.AvailableInputs moveAudioInput)
	{
		if (DEBUG)
			return Input.GetKeyDown(moveKeyboardInput);
		else
			return moveAudioInput != lastAudioInput && MicHandle.instance.getInputDown(moveAudioInput);
	}

	private void MoveInputHandling()
	{
		bool input = false;
		bool anyInput = false;

		if (checkInput(moveKeyboardInput1, moveAudioInput1))
		{
			if (moveKeyboardInput1 != lastKeyboardInput && moveAudioInput1 != lastAudioInput)
			{
				lastMoveInput1 = Time.time;
				input = true;

				lastKeyboardInput = moveKeyboardInput1;
				lastAudioInput = moveAudioInput1;
			}

			else
				anyInput = true;
		}

		else if (checkInput(moveKeyboardInput2, moveAudioInput2))
		{
			if (moveKeyboardInput2 != lastKeyboardInput && moveAudioInput2 != lastAudioInput)
			{
				lastMoveInput2 = Time.time;
				input = true;

				lastKeyboardInput = moveKeyboardInput2;
				lastAudioInput = moveAudioInput2;
			}

			else
				anyInput = true;
				
		}
        
		float targetSpeedFactor = maxInputDiff - Mathf.Abs(lastMoveInput1 - lastMoveInput2);
		targetSpeedFactor = targetSpeedFactor < 0 ? 0 : targetSpeedFactor;
		if(input)
		{
			currentDeaceleration = startDeaceleration;
			speedFactor = targetSpeedFactor;
		}
		else
		{
			if (anyInput)
				currentDeaceleration = startDeaceleration;
			else
				currentDeaceleration = Mathf.Clamp(currentDeaceleration - deacelerationDelta, minDeaceleration, startDeaceleration);

			speedFactor = speedFactor > 0 ? speedFactor -= Time.deltaTime/currentDeaceleration : 0;
		}
	}	

	void setAnimationTimeScale (float speed)
	{
		float deltaTimeScale = maxAnimationScale - minAnimationScale;
		
		skeletonAnimation.timeScale = minAnimationScale + (speed / maxSpeed * deltaTimeScale);
	}


	public float CalculateTargetSpeed()
	{
		float speed = maxSpeed * speedFactor; 
		speed = speed < minSpeed ? minSpeed : speed;

		return speed;
	}

    void OnTriggerEnter2D(Collider2D other) 
	{
        if(other.gameObject.tag == "Person")
		{
			if(OnAtePerson != null)
			{
				OnAtePerson();
				currentState = PlayerState.bite;
				spineAnimationState.SetAnimation(0, attackAnimation, false);
			}
		}
    }

	void AnimationComplete(Spine.AnimationState state, int trackIndex, int loopCount)
	{
		if(currentState == PlayerState.bite)
		{
			spineAnimationState.SetAnimation(0, swimAnimation, true);
			currentState = PlayerState.swimming;	
		}
	}

	void Start () 
	{
		instance = this;

		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		spineAnimationState.SetAnimation(0, swimAnimation, true);
		// skeleton = skeletonAnimation.skeleton;

		currentState = PlayerState.swimming;
		spineAnimationState.Complete += AnimationComplete;
	}

	public float safetyAlertWait = 1f;
	public IEnumerator AlertState()
	{
		if(OnAlert != null)
		{
			OnAlert();
		}
		yield return new WaitForSeconds(safetyAlertWait);
		alertState = true;
		yield return new WaitForSeconds(alertStateTime);
		alertState = false;
	}

	// Update is called once per frame
	void Update () 
	{
		MoveInputHandling();

		float newSpeed = CalculateTargetSpeed();

		currentSpeed = Mathf.Min(newSpeed, currentSpeed + maxSpeedDelta); 

		float currentSpeedDelta = currentSpeed - minSpeed;
		float totalSpeedDelta = maxSpeed - minSpeed;
		float currentSpeedPercentage = currentSpeedDelta / totalSpeedDelta;

		float currentThreshold = (1 - currentSpeedPercentage) * alertThreshold;

		// DO PERSON ALERT HERE
		if (currentThreshold < newSpeed - currentSpeed)
		{
			if(!alertState)
			{
				StartCoroutine(AlertState());
			}
			else
			{
				if(OnDesperate != null)
				{
					OnDesperate();
				}
			}
		}

		if(currentState == PlayerState.swimming)
			setAnimationTimeScale(currentSpeed);

		Vector3 position = transform.position;
		Vector3 targetPosition = position;
		targetPosition.x = targetPosition.x + currentSpeed;

		float targetY = currentSpeedDelta * (maxHeight - minHeight)/(maxSpeed-minSpeed) + minHeight;
		targetPosition.y = Mathf.Lerp(position.y, targetY, Time.deltaTime);

		transform.position = targetPosition;
	}


	public void ResetPlayer ()
	{
		currentState = PlayerState.swimming;
		// Set AnimationComplete

		lastKeyboardInput = KeyCode.Space;
		lastAudioInput = MicHandle.AvailableInputs.None;

		lastMoveInput1 = 0;
		lastMoveInput2 = 0.1f;
		speedFactor = 0;

		currentSpeed = minSpeed;
		currentDeaceleration = startDeaceleration;

		Vector3 startPosition = transform.position;
		startPosition.y = minHeight;
		transform.position = startPosition;

		alertState = false;
	}

	public void PausePlayer ()
	{
		enabled = false;
		skeletonAnimation.enabled = false;
	}

	public void ResumePlayer ()
	{
		enabled = true;
		skeletonAnimation.enabled = true;
	}
}
