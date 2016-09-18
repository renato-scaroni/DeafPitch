using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour {

	private Camera mainCamera;
	private float screenWidth;
	
	public Behaviour animationController;



	// Use this for initialization
	void Start () {
		mainCamera = GameManager.instance.mainCamera;
		screenWidth = GameManager.instance.screenWidth;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < mainCamera.transform.position.x - screenWidth - 1)
			GameManager.instance.bombManager.returnBomb(this.gameObject, true);
	}

	public void startBomb () {
		enabled = true;
	}

	public void EndGame() {
		GameManager.instance.EndGame(false);
	}

	public void OnHit () {
		// Start animation;

		EndGame();
	}


	public void PauseBomb ()
	{
		enabled = false;
		animationController.enabled = false;
	}

	public void ResumeBomb ()
	{
		enabled = true;
		animationController.enabled = true;
	}
}
