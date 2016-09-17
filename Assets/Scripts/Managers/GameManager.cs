using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour 
{
	public GameObject player; 
	public Camera camera;
	public GameObject person;

	private PlayerController playerController;
	private GameObject personInstance;
	private Vector3 defaultPosition;
	private float screenWidth;

	static public GameManager instance
	{
		get;
		private set;
	}

	public void CreateNewPerson(Vector3 position)
	{
		if(personInstance == null)
		{
			personInstance = Instantiate(person) as GameObject;
		}
		else
		{
			personInstance.SetActive(true);
		}

		personInstance.transform.position = position;
	}


	public IEnumerator CreateNewPersonDelayed(float time)
	{
		yield return new WaitForSeconds(time);
		Vector3 position = defaultPosition;
		position.x += camera.transform.position.x + playerController.GetCurrentSpeed();
		CreateNewPerson(position);
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		float aspectRatio = (float)(Screen.width) / (float)(Screen.height);
		screenWidth = camera.orthographicSize * aspectRatio;
		print("[GameManager] " + screenWidth);
		defaultPosition = new Vector3(screenWidth*3/4, 0, 0);
		CreateNewPerson(defaultPosition);
		playerController = player.GetComponent<PlayerController>();
		PlayerController.OnAtePerson += () =>
		{
			personInstance.SetActive(false);
			StartCoroutine(CreateNewPersonDelayed(1));
		};
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
