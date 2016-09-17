using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public GameObject player; 
	public GameObject mainCamera; 

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



	////////
	// Game Flow Control
	///////

	public void PauseGame ()
	{
		player.GetComponent<PlayerController>().PausePlayer();
		mainCamera.GetComponent<CameraController>().enabled = false;
	}

	public void ResumeGame ()
	{
		player.GetComponent<PlayerController>().ResumePlayer();
		mainCamera.GetComponent<CameraController>().enabled = true;
	}
}
