using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour 
{
	public GameObject player; 
	public Camera mainCamera;
	public GameObject[] personPrefabs;
	public float normalProbability;

	public PlayerController playerController
	{
		get;
		private set;
	}
	
	private GameObject[] personInstances;
	private Vector3 defaultPosition;
	private float screenWidth;
	private int personTypeCount;
	private int currentPersonType;

	static public GameManager instance
	{
		get;
		private set;
	}

	public void CreateNewPerson(Vector3 position)
	{
		float uniform = Random.Range(0f,1f);

		currentPersonType = uniform > normalProbability ? 1 : 0;

		print ("CreateNewPerson personType " + currentPersonType);

		if(personInstances[currentPersonType] == null)
		{
			personInstances[currentPersonType] = Instantiate(personPrefabs[currentPersonType]) as GameObject;
		}
		else
		{
			personInstances[currentPersonType].SetActive(true);
		}

		personInstances[currentPersonType].transform.position = position;
	}


	public IEnumerator CreateNewPersonDelayed(float time)
	{
		yield return new WaitForSeconds(time);
		Vector3 position = defaultPosition;
		position.x += mainCamera.transform.position.x + playerController.currentSpeed;
		CreateNewPerson(position);
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		float aspectRatio = (float)(Screen.width) / (float)(Screen.height);
		screenWidth = mainCamera.orthographicSize * aspectRatio;
		print("[GameManager] " + screenWidth);
		defaultPosition = new Vector3(screenWidth*3/4, 0, 0);
		playerController = player.GetComponent<PlayerController>();
		personInstances = new GameObject[personPrefabs.Length];
		CreateNewPerson(defaultPosition);
		PlayerController.OnAtePerson += () =>
		{
			personInstances[currentPersonType].SetActive(false);
			StartCoroutine(CreateNewPersonDelayed(1));
		};
		personTypeCount = personPrefabs.Length;
	}
	
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
