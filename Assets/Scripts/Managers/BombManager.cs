using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombManager : MonoBehaviour {

	private List<GameObject> activeBombs;
	private List<GameObject> bombPool;
	public GameObject bombPrefab;
	public Camera mainCamera;

	private float startPosX = 0;
	private float currentDeltaX = 0;
	private float targetDeltaPosX = 0;
	public float bombSpawnMaxDeltaX = 7;
	public float bombSpawnMinDeltaX = 3; 

	public float minHeight = -4;
	public float maxHeight = -0.9f;

	public bool HARDCORE = false;


	// Use this for initialization
	void Start () {
    	activeBombs = new List<GameObject>();
    	bombPool = new List<GameObject>();

		PlayerController.OnHitBomb += (BombController bomb) =>
		{
			bomb.OnHit();
		};

		targetDeltaPosX = Random.Range(bombSpawnMinDeltaX, bombSpawnMaxDeltaX);
	}
	
	// Update is called once per frame
	void Update () {
		currentDeltaX = mainCamera.transform.position.x - startPosX;

		if (currentDeltaX > targetDeltaPosX)
			spawnRandomBomb();
	}

	public GameObject getBomb ()
	{
		GameObject newBomb;

		if (bombPool.Count == 0) {
			newBomb = Instantiate(bombPrefab) as GameObject;
		}
		else {
			newBomb = bombPool[0];
			bombPool.RemoveAt(0);
			newBomb.SetActive(true);
		}

		activeBombs.Add(newBomb);

		return newBomb;
	}

	public void returnBomb (GameObject bomb, bool shouldRemoveActive)
	{
		bomb.SetActive(false);

		bombPool.Add(bomb);

		if (shouldRemoveActive)
			activeBombs.Remove(bomb);
	}

	public void spawnBomb(float targetY)
	{
		if (!HARDCORE && currentDeltaX < bombSpawnMinDeltaX) return;

		currentDeltaX = 0;
		startPosX = mainCamera.transform.position.x;
		targetDeltaPosX = Random.Range(bombSpawnMinDeltaX, bombSpawnMaxDeltaX);

		GameObject newBomb = getBomb();

		newBomb.transform.position = new Vector3 (
			mainCamera.transform.position.x + GameManager.instance.screenWidth + 1,
			targetY,
			1
		);

		BombController bombController = newBomb.GetComponent<BombController>();
		bombController.startBomb();
	}

	public void spawnRandomBomb()
	{
		spawnBomb(Random.Range(minHeight, maxHeight));
	}



	////////
	// Game Flow Control
	///////

	public void PauseBombs ()
	{
		enabled = false;
		
		foreach (GameObject bomb in activeBombs)
			bomb.GetComponent<BombController>().PauseBomb();
	}

	public void ResumeBombs ()
	{
		enabled = true;

		foreach (GameObject bomb in activeBombs)
			bomb.GetComponent<BombController>().ResumeBomb();
	}

	public void ResetBombs()
	{
		currentDeltaX = 0;

		foreach (GameObject bomb in activeBombs)
			returnBomb(bomb, false);

    	activeBombs = new List<GameObject>();
	}
}
