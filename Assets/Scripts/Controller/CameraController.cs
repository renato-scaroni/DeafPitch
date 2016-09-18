using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject Waves;
	public float playerDistance = -7;

	private GameManager gameManager;
	// private Transform playerTransfom;
	private PlayerController player;
	public float currentSpeed
	{
		get;
		private set;
	}

	private void MoveCamera()
	{

	}

	// Use this for initialization
	void Start () 
	{
		gameManager = GameManager.instance;
		// playerTransfom = gameManager.player.transform;
		player = gameManager.player.GetComponent<PlayerController>();
		currentSpeed = player.currentSpeed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentSpeed = Mathf.Lerp(currentSpeed, player.currentSpeed, Time.deltaTime);
		transform.Translate(Vector3.right * currentSpeed);	
	}

	public void PauseCamera ()
	{
		enabled = false;
		Waves.GetComponent<Animator>().enabled = false;

		foreach (Transform child in Waves.transform)
			child.gameObject.GetComponent<RepeatSpriteBoundary>().enabled = false;
	}

	public void ResumeCamera ()
	{
		enabled = true;
		Waves.GetComponent<Animator>().enabled = true;

		foreach (Transform child in Waves.transform)
			child.gameObject.GetComponent<RepeatSpriteBoundary>().enabled = true;
	}
}
