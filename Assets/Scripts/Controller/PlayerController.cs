using UnityEngine;
using System.Collections;
using Spine.Unity;

public class PlayerController : MonoBehaviour 
{
	public bool DEBUG = false;

	public float minSpeed = 0.01f;
	public float maxSpeed = 1f;
	public float minHeight = -4.5f; 
	public float maxHeight = 0;
	public float maxInputDiff = .7f;

	private float lastMoveInput1 = 0;
	private float lastMoveInput2 = 0.1f;
	private float speedFactor = 0;	

	private KeyCode moveKeyboardInput1 = KeyCode.Z;
	private KeyCode moveKeyboardInput2 = KeyCode.X;
	private MicHandle.AvailableInputs moveAudioInput1 = MicHandle.AvailableInputs.Swim1;
	private MicHandle.AvailableInputs moveAudioInput2 = MicHandle.AvailableInputs.Swim2;

	private KeyCode lastKeyboardInput = KeyCode.Space;
	private MicHandle.AvailableInputs lastAudioInput = MicHandle.AvailableInputs.None;

	
	// Spine Variables
	private SkeletonAnimation skeletonAnimation;
	private Spine.AnimationState spineAnimationState;
	private Spine.Skeleton skeleton;

	private string swimAnimation = "idle";
	private string attackAnimation = "bite";

	public float maxAnimationScale = 3;
	public float minAnimationScale = 1;

		
	private bool checkInput(KeyCode moveKeyboardInput, MicHandle.AvailableInputs moveAudioInput)
	{
		if (DEBUG)
			return moveKeyboardInput != lastKeyboardInput && Input.GetKeyDown(moveKeyboardInput);
		else
			return moveAudioInput != lastAudioInput && MicHandle.instance.getInputDown(moveAudioInput);
	}

	public delegate void AtePersonHandler();
	public static event AtePersonHandler OnAtePerson;

	private void MoveInputHandling()
	{
		bool input = false;

		if (checkInput(moveKeyboardInput1, moveAudioInput1))
		{
			lastMoveInput1 = Time.time;
			input = true;

			lastKeyboardInput = moveKeyboardInput1;
			lastAudioInput = moveAudioInput1;
		}

		else if (checkInput(moveKeyboardInput2, moveAudioInput2))
		{
			lastMoveInput2 = Time.time;
			input = true;

			lastKeyboardInput = moveKeyboardInput2;
			lastAudioInput = moveAudioInput2;
		}
        
		// print(Mathf.Abs(lastMoveInput1 - lastMoveInput2));
		float targetSpeedFactor = maxInputDiff - Mathf.Abs(lastMoveInput1 - lastMoveInput2);
		targetSpeedFactor = targetSpeedFactor < 0 ? 0 : targetSpeedFactor;
		if(input)
		{
			speedFactor = targetSpeedFactor;
		}
		else
		{
			speedFactor = speedFactor > 0 ? speedFactor -= Time.deltaTime/10 : 0;
		}
		
		// print(speedFactor);
	}	

	void setAnimationTimeScale (float speed)
	{
		float deltaTimeScale = maxAnimationScale - minAnimationScale;
		
		skeletonAnimation.timeScale = minAnimationScale + (speed / maxSpeed * deltaTimeScale);
	}


	public float GetCurrentSpeed()
	{
		float speed = maxSpeed * speedFactor; 
		speed = speed < minSpeed ? minSpeed : speed;

		return speed;
	}

    void OnTriggerEnter2D(Collider2D other) 
	{
		print(other.gameObject);
        if(other.gameObject.tag == "Person")
		{
		print("TRIGGER!!!!!");
			if(OnAtePerson != null)
			{
				OnAtePerson();
			}
		}
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

	void Start () 
	{
		skeletonAnimation = GetComponent<SkeletonAnimation>();
		spineAnimationState = skeletonAnimation.state;
		spineAnimationState.SetAnimation(0, swimAnimation, true);
		skeleton = skeletonAnimation.skeleton;
	}

	// Update is called once per frame
	void Update () 
	{
		MoveInputHandling();
		float currentSpeed = GetCurrentSpeed();

		setAnimationTimeScale(currentSpeed);

		// transform.Translate(Vector3.right * currentSpeed);
		Vector3 position = transform.position;
		Vector3 targetPosition = position;
		targetPosition.x = targetPosition.x + currentSpeed;

		float targetY = (currentSpeed - minSpeed) * (maxHeight - minHeight)/(maxSpeed-minSpeed) + minHeight;
		targetPosition.y = Mathf.Lerp(position.y, targetY, Time.deltaTime);

		// print (targetPosition);
		// print (currentSpeed);
		// print((currentSpeed - minSpeed) * (maxHeight - minHeight)/(maxSpeed-minSpeed) + minHeight);
		transform.position = targetPosition;
	}
}
