using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour {

	public Image Swim1Image;
	public Image Swim2Image;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		updateButtonColor (
			Swim1Image,
			MicHandle.AvailableInputs.Swim1
		);

		updateButtonColor (
			Swim2Image,
			MicHandle.AvailableInputs.Swim2
		);
	}

	void updateButtonColor (Image image, MicHandle.AvailableInputs imageInput) {
		if (MicHandle.instance.getInputDown(imageInput) ||
			MicHandle.instance.getInputState(imageInput, MicHandle.InputStates.JustPressed)) {
			image.color = Color.red;
		}

		else if (MicHandle.instance.getInputHold(imageInput)) {
			image.color = Color.red;
		}
		
		else {
			image.color = Color.white;
		}
	}
}
