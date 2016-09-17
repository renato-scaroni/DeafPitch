using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float minSpeed = 0.01f;
	public float maxSpeed = 1f;
	public KeyCode moveInput1 = KeyCode.Z;
	public KeyCode moveInput2 = KeyCode.X;

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
			
		}
		print(Mathf.Abs(lastMoveInput1 - lastMoveInput2));
		float targetSpeedFactor = .7f - Mathf.Abs(lastMoveInput1 - lastMoveInput2);
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
		transform.Translate(Vector3.right * currentSpeed);
	}
}
