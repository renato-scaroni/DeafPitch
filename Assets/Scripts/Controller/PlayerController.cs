using UnityEngine;
using System.Collections;

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

	public KeyCode lastKeyboardInput = KeyCode.Space;
	public MicHandle.AvailableInputs lastAudioInput = MicHandle.AvailableInputs.None;

	private bool checkInput(KeyCode moveKeyboardInput, MicHandle.AvailableInputs moveAudioInput)
	{
		if (DEBUG)
			return moveKeyboardInput != lastKeyboardInput && Input.GetKeyDown(moveKeyboardInput);
		else
			return moveAudioInput != lastAudioInput && MicHandle.instance.getInputDown(moveAudioInput);
	}

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

	public float GetCurrentSpeed()
	{
		float speed = maxSpeed * speedFactor; 

		return speed < minSpeed ? minSpeed : speed;
	}

	void Start () 
	{

	}

	// Update is called once per frame
	void Update () 
	{
		MoveInputHandling();
		float currentSpeed = GetCurrentSpeed();
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
