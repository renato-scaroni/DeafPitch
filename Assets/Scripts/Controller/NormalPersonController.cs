using UnityEngine;
using System.Collections;

public class NormalPersonController : PersonController 
{
	private Animator animator;
	public float desperateSpeed = .06f;
	public float normalSpeed = .03f;
	
	void OnAlert()
	{
		speed = desperateSpeed;
		animator.SetBool("Desperate", true);
	}

	void OnEnable ()
	{
		PlayerController.OnAlert += OnAlert;
	}

	void OnDisable ()
	{
		PlayerController.OnAlert -= OnAlert;
	}


	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();

	}
}
