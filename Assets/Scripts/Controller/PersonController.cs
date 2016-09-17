using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour 
{
	public float speed = .03f;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Translate(Vector3.right * speed);
	}
}
