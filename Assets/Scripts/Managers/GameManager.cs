using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public GameObject player; 

	static public GameManager instance
	{
		get;
		private set;
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
