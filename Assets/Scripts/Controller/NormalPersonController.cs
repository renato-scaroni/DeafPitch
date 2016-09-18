using UnityEngine;
using System.Collections;

public class NormalPersonController : PersonController 
{
	private Animator animator;
	public float desperateSpeed = .06f;
	public float normalSpeed = .03f;
	public GameObject alert;
	public float totalAlertTime = 2f;

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
	}

	void OnDisable ()
	{
		PlayerController.OnDesperate -= OnDesperate;
		PlayerController.OnAlert -= OnAlert;
	}


	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator>();

	}
}
