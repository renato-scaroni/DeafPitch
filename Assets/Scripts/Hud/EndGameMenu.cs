using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour {
	
	public GameObject winMenu;
	public GameObject loseMenu;
	public Button pauseButton;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowMenu (int score, bool won)
	{
		pauseButton.gameObject.SetActive(false);

		if (won)
			winMenu.SetActive(true);
		else
			loseMenu.SetActive(true);
	}

	public void RetryGame ()
	{
		winMenu.SetActive(false);
		loseMenu.SetActive(false);
		pauseButton.gameObject.SetActive(true);
		
		GameManager.instance.ResetGame();
	}

	public void BackToStart ()
	{
		winMenu.SetActive(false);
		loseMenu.SetActive(false);
		pauseButton.gameObject.SetActive(true);

		GameManager.instance.ResetGame();
	}
}
