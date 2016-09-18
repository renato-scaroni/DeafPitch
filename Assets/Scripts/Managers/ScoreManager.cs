using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour 
{

	private List<GameObject> activeGhosts;
	private List<GameObject> ghostPool;
	public GameObject ghostPrefab;
	public Camera mainCamera;
	public Image ghostBarFill;

	public int currentScore;
	public int maxScore = 12; // Should be multiple of 3 

	// Use this for initialization
	void Start () {
    	activeGhosts = new List<GameObject>();
    	ghostPool = new List<GameObject>();
		ghostBarFill.fillAmount = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public GameObject getGhost ()
	{
		GameObject newGhost;

		if (ghostPool.Count == 0) {
			newGhost = Instantiate(ghostPrefab) as GameObject;
			newGhost.transform.parent = mainCamera.transform;
		}
		else {
			newGhost = ghostPool[0];
			ghostPool.RemoveAt(0);
			newGhost.SetActive(true);
		}

		activeGhosts.Add(newGhost);

		return newGhost;
	}

	public void returnGhost (GameObject ghost, bool shouldRemoveActive)
	{
		ghost.SetActive(false);

		ghostPool.Add(ghost);

		if (shouldRemoveActive)
			activeGhosts.Remove(ghost);
	}

	public bool addScore(GameObject person)
	{
		currentScore ++;

		GameObject newGhost = getGhost();

		GhostController ghostController = newGhost.GetComponent<GhostController>();
		ghostController.startAnimation(
			mainCamera.transform.InverseTransformPoint(person.transform.position),
			new Vector3(-8.2f, 4.52f, 1)
		);

		return currentScore >= maxScore;
	}

	public void UpdateGhostBar()
	{
		ghostBarFill.fillAmount = (float)(currentScore) / (float)(maxScore);
	}

	public int GetTier()
	{
		float lifePercent = (float)(currentScore) / (float)(maxScore);

		int tierAmount = (int) (maxScore / 3);

		return (int)Mathf.Floor(currentScore / tierAmount);
	} 

	////////
	// Game Flow Control
	///////

	public void PauseScore ()
	{
		enabled = false;
		
		foreach (GameObject ghost in activeGhosts)
			ghost.GetComponent<GhostController>().PauseGhost();
	}

	public void ResumeScore ()
	{
		enabled = true;

		foreach (GameObject ghost in activeGhosts)
			ghost.GetComponent<GhostController>().ResumeGhost();
	}


	public void ResetScore()
	{
		currentScore = 0;

		foreach (GameObject ghost in activeGhosts)
			returnGhost(ghost, false);

		UpdateGhostBar();
			
    	activeGhosts = new List<GameObject>();
	}
}
