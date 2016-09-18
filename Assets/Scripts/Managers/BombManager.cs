using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombManager : MonoBehaviour {

	private List<GameObject> activeBombs;
	private List<GameObject> bombPool;
	public GameObject bombPrefab;
	public Camera mainCamera;

	private float bombCD = 0;
	private float targetCD = 0;
	public float bombSpawnMaxTime = 7;
	public float bombSpawnMinTime = 3; 

	public float minHeight = -4;
	public float maxHeight = -0.9f;


	// Use this for initialization
	void Start () {
    	activeBombs = new List<GameObject>();
    	bombPool = new List<GameObject>();

		PlayerController.OnHitBomb += (BombController bomb) =>
		{
			bomb.OnHit();
		};
	}
	
	// Update is called once per frame
	void Update () {
		bombCD += Time.deltaTime;

		if (bombCD > targetCD)
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
		if (bombCD < bombSpawnMinTime) return;
		bombCD = 0;
		targetCD = Random.Range(bombSpawnMinTime, bombSpawnMaxTime);

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
		bombCD = 0;

		foreach (GameObject bomb in activeBombs)
			returnBomb(bomb, false);

    	activeBombs = new List<GameObject>();
	}
}
