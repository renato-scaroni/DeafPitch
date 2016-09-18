using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour {

	private List<GameObject> activeGhosts;
	private List<GameObject> ghostPool;
	public GameObject ghostPrefab;
	public Camera mainCamera;

	public int currentScore;

	// Use this for initialization
	void Start () {
    	activeGhosts = new List<GameObject>();
    	ghostPool = new List<GameObject>();
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

	public void returnGhost (GameObject ghost)
	{
		ghost.SetActive(false);

		ghostPool.Add(ghost);
		activeGhosts.Remove(ghost);
	}



	public void addScore(GameObject person)
	{
		currentScore ++;

		GameObject newGhost = getGhost();

		GhostController ghostController = newGhost.GetComponent<GhostController>();
		ghostController.startAnimation(
			mainCamera.transform.InverseTransformPoint(person.transform.position),
			new Vector3(-8.2f + (activeGhosts.Count - 1) * 0.97f, 4.52f, 1)
		);
	} 
}
