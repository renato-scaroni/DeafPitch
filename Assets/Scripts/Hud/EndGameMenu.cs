using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour {
	
	public GameObject winMenu;
	public GameObject loseMenu;
	public GameObject inGameMenu;
	public GameObject titleMenu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowMenu (int score, bool won)
	{
		inGameMenu.SetActive(false);

		if (won)
			winMenu.SetActive(true);
		else
			loseMenu.SetActive(true);
	}

	public void RetryGame ()
	{
		winMenu.SetActive(false);
		loseMenu.SetActive(false);
		inGameMenu.SetActive(true);
		
		GameManager.instance.ResetGame(true);
	}

	public void BackToStart ()
	{
		winMenu.SetActive(false);
		loseMenu.SetActive(false);
		inGameMenu.SetActive(false);
		titleMenu.SetActive(false);
	}
}
