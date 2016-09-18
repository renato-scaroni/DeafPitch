using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour {
	
	public GameObject winMenu;
	public GameObject loseMenu;
	public GameObject inGameMenu;
	public GameObject titleMenu;

	public SpriteRenderer starWin;
	public SpriteRenderer starLose;

	public Sprite[] starSprites;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowMenu (int tier, bool won)
	{
		inGameMenu.SetActive(false);

		if (won) {
			winMenu.SetActive(true);
			starWin.sprite = starSprites[tier];
		}
		else {
			loseMenu.SetActive(true);
			starLose.sprite = starSprites[tier];
		}
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
