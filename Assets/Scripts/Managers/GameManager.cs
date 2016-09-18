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
	public BombManager bombManager;
	public GameObject bloodAnimation;
	public GameObject[] personPrefabs;
	public EndGameMenu endGameMenu;

	public float normalProbability;
	public float maxDistanceToLose = 10;

	private PlayerController playerController;
	private GameObject[] personInstances;
	private Vector3 defaultPosition;
	public float screenWidth;
	private int currentPersonType = -1;



	GameObject getCurrentPerson ()
	{
		return currentPersonType == -1 ? null : personInstances[currentPersonType];
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
		getCurrentPerson().GetComponent<PersonController>().ResumePerson();
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
		GameObject currentPerson = getCurrentPerson();

		currentPerson.SetActive(false);
		bombManager.spawnBomb(currentPerson.transform.position.y);

		if (scoreManager.addScore(currentPerson))
			EndGame(true);
		
		else {
			// currentPersonType = -1;
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
		
		defaultPosition = new Vector3(screenWidth + 1, 0.8f, 0);
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

		GameObject currentPerson = getCurrentPerson();
		if (currentPerson != null) currentPerson.GetComponent<PersonController>().PausePerson();

		scoreManager.PauseScore();
		bombManager.PauseBombs();
	}

	public void ResumeGame ()
	{
		enabled = true;
		playerController.ResumePlayer();
		mainCamera.GetComponent<CameraController>().ResumeCamera();

		GameObject currentPerson = getCurrentPerson();
		if (currentPerson != null) currentPerson.GetComponent<PersonController>().ResumePerson();

		scoreManager.ResumeScore();
		bombManager.ResumeBombs();
	}

	public void ResetGame ()
	{
		playerController.ResetPlayer();
		scoreManager.ResetScore();
		bombManager.ResetBombs();

		GameObject currentPerson = getCurrentPerson();
		if (currentPerson != null) currentPerson.SetActive(false);

		currentPersonType = -1;

		StartCoroutine(CreateNewPersonDelayed(6));

		ResumeGame();
	}
}
