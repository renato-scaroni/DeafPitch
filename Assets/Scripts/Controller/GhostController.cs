using UnityEngine;
using System.Collections;

public class GhostController : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 targetPosition;
	private float currentAnimationTime;
	public float animationTime = 3;
	private int animationPeriods = 2;
	public float maxOscilation = 0.7f;
	public float startScale = 2;

	public Behaviour animationController;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currentAnimationTime += Time.deltaTime;
		float completion = currentAnimationTime / animationTime;
		float period = completion * animationPeriods * Mathf.PI;

		float delta = Mathf.Sin(period) * maxOscilation;

		transform.localPosition = new Vector3 (
			Mathf.Lerp(startPosition.x, targetPosition.x, completion) + delta,
			Mathf.Lerp(startPosition.y, targetPosition.y, completion) + delta,
			1
		);

		transform.localScale = new Vector3(
			Mathf.Lerp(startScale, 1, completion),
			Mathf.Lerp(startScale, 1, completion),
			1
		);
		
		if (completion > 1) enabled = false;
	}

	public void startAnimation (Vector3 start, Vector3 target)
	{
		targetPosition = target;
		currentAnimationTime = 0;
		startPosition = start;
		transform.localPosition = start;
		enabled = true;
		animationPeriods = Random.Range(2, 4);
		transform.localScale = new Vector3(startScale, startScale, 1);
	}


	public void PauseGhost ()
	{
		enabled = false;
		animationController.enabled = false;
	}

	public void ResumeGhost ()
	{
		enabled = true;
		animationController.enabled = true;
	}
}
