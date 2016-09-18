using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour 
{
	public float speed = .03f;
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate(Vector3.right * speed);
	}
}
