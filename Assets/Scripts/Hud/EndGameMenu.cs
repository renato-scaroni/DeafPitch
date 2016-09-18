using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGameMenu : MonoBehaviour {
	
	public GameObject menu;
	public Button closeButton;

	// Use this for initialization
	void Start () {
		closeButton.onClick.AddListener(() => { HideMenu(); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ShowMenu (int score, bool won)
	{
		menu.SetActive(true);
	}

	public void HideMenu ()
	{
		menu.SetActive(false);
		GameManager.instance.ResetGame();
	}
}
