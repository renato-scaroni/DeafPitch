using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour 
{

	static public GameManager instance
	{
		get;
		private set;
	}

	
	public GameObject player; 
	public Camera mainCamera;
	public ScoreManager scoreManager;
	public GameObject bloodAnimation;
	public GameObject[] personPrefabs;
	public EndGameMenu endGameMenu;

	public float normalProbability;
	public float maxDistanceToLose = 10;

	private PlayerController playerController;
	private GameObject[] personInstances;
	private Vector3 defaultPosition;
	private float screenWidth;
	private int currentPersonType = 0;



	GameObject getCurrentPerson ()
	{
		return personInstances[currentPersonType];
	}

	public void CreateNewPerson(Vector3 position)
	{
		float uniform = Random.Range(0f,1f);

		currentPersonType = uniform > normalProbability ? 1 : 0;

		if(getCurrentPerson() == null)
		{
			personInstances[currentPersonType] = Instantiate(personPrefabs[currentPersonType]) as GameObject;
		}
		else
		{
			getCurrentPerson().SetActive(true);
		}

		getCurrentPerson().transform.position = position;
	}


	public IEnumerator CreateNewPersonDelayed(float time)
	{
		yield return new WaitForSeconds(time);
		Vector3 position = defaultPosition;
		position.x += mainCamera.transform.position.x + playerController.currentSpeed;
		CreateNewPerson(position);
	}

	public IEnumerator WaitAndDeActivate(float time)
	{
		yield return new WaitForSeconds(time);
		getCurrentPerson().SetActive(false);

		if (scoreManager.addScore(getCurrentPerson()))
			EndGame(true);
		
		else {
			StartCoroutine(CreateNewPersonDelayed(4));
			bloodAnimation.transform.position = player.transform.position + -Vector3.up * 1.7f + Vector3.right * 2f;
		}

	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		float aspectRatio = (float)(Screen.width) / (float)(Screen.height);
		screenWidth = mainCamera.orthographicSize * aspectRatio;
		
		defaultPosition = new Vector3(screenWidth*3/4, 0, 0);
		playerController = player.GetComponent<PlayerController>();
		personInstances = new GameObject[personPrefabs.Length];

		StartCoroutine(CreateNewPersonDelayed(2));

		PlayerController.OnAtePerson += () =>
		{
			bloodAnimation.GetComponent<Animator>().SetTrigger("Reset");
			bloodAnimation.SetActive(true);
			StartCoroutine(WaitAndDeActivate(.25f));
		};
	}
	
	void Update () 
	{
		GameObject currentPerson = getCurrentPerson();
		if (currentPerson == null) return;

		if (currentPerson.transform.position.x - mainCamera.transform.position.x > maxDistanceToLose)
			EndGame(false);
	}



	////////
	// Game Flow Control
	///////

	public void EndGame (bool won)
	{
		endGameMenu.ShowMenu(scoreManager.currentScore, won);
		PauseGame();
	}

	public void PauseGame ()
	{
		enabled = false;
		playerController.PausePlayer();
		mainCamera.GetComponent<CameraController>().PauseCamera();
		getCurrentPerson().GetComponent<PersonController>().PausePerson();
	}

	public void ResumeGame ()
	{
		// enabled = true;
		playerController.ResumePlayer();
		mainCamera.GetComponent<CameraController>().ResumeCamera();
		getCurrentPerson().GetComponent<PersonController>().ResumePerson();
	}

	public void ResetGame ()
	{
		playerController.ResetPlayer();
		scoreManager.ResetScore();

		getCurrentPerson().SetActive(false);
		StartCoroutine(CreateNewPersonDelayed(0));

		ResumeGame();
	}
}
