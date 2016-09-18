using UnityEngine;
using System.Collections;

public abstract class PersonController : MonoBehaviour 
{
	public float speed = .03f;
	public Behaviour animationController;
	public GameObject swimAnim;
	public GameObject deathAnim; 

	public abstract void SetDeathAnimation ();
	public abstract void SetSwimAnimation ();

	// Update is called once per frame
	void Update () 
	{
		transform.Translate(Vector3.right * speed);
	}

	public void PausePerson ()
	{
		enabled = false;
		animationController.enabled = false;
	}

	public void ResumePerson ()
	{
		enabled = true;
		animationController.enabled = true;
	}
}
