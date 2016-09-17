using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float minSpeed = 0.01f;
	public float maxSpeed = 1f;
	public KeyCode moveInput1 = KeyCode.Z;
	public KeyCode moveInput2 = KeyCode.X;
	public float minHeight = -4.5f; 
	public float maxHeight = 0;
	public float maxInputDiff = .7f;

	private float lastMoveInput1 = 0;
	private float lastMoveInput2 = 0.1f;
	private float speedFactor = 0;
	private KeyCode lastInput = KeyCode.Space;	

	private void MoveInputHandling()
	{
		bool input = false;
		if(Input.GetKeyDown(moveInput1) && moveInput1 != lastInput)
		{
			lastMoveInput1 = Time.time;
			input = true;
		}
		if(Input.GetKeyDown(moveInput2) && moveInput2 != lastInput)
		{
			lastMoveInput2 = Time.time;
			input = true;
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
		
		print(speedFactor);
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
