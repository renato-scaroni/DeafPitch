using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour {

	private MicHandle.AvailableInputs targetInput;
	private bool isRecording = false;
	private bool isOnMenu = false;

	public Button Swim1Button;
	public Button Swim2Button;
	public Button PauseButton;


	public bool DEBUG;

	// Use this for initialization
	void Start () {
		Swim1Button.onClick.AddListener(() => { ToggleRecording(MicHandle.AvailableInputs.Swim1); });
		Swim2Button.onClick.AddListener(() => { ToggleRecording(MicHandle.AvailableInputs.Swim2); });
		PauseButton.onClick.AddListener(() => {
			isOnMenu = !isOnMenu;

			Swim1Button.gameObject.SetActive(isOnMenu);
			Swim2Button.gameObject.SetActive(isOnMenu);

			if (isOnMenu)
				GameManager.instance.PauseGame();
			else
				GameManager.instance.ResumeGame();
		});
	}
	
	// Update is called once per frame
	void Update () {
		updateButtonColor (Swim1Button, MicHandle.AvailableInputs.Swim1);
		updateButtonColor (Swim2Button, MicHandle.AvailableInputs.Swim2);
	}

	void updateButtonColor (Button button, MicHandle.AvailableInputs buttonInput) {

		if (isRecording) {
			if (targetInput == buttonInput)
				button.image.color = Color.green;
			else
				button.image.color = Color.white;
		}
		else {
			if (MicHandle.instance.getInputDown(buttonInput) ||
				MicHandle.instance.getInputState(buttonInput, MicHandle.InputStates.JustPressed))
				button.image.color = Color.black;

			else if (MicHandle.instance.getInputHold(buttonInput))
				button.image.color = Color.red;
			
			else
				button.image.color = Color.white;
		}
	}


	void ToggleRecording (MicHandle.AvailableInputs buttonInput) {
		if (isRecording) {
			if (targetInput == buttonInput) {
				isRecording = false;
				float newPitch = MicHandle.instance.stopAndGetAverage();
				MicHandle.instance.setAsDefaultInputIfPossible(targetInput, newPitch);
			}
		}

		else {
			if (DEBUG) {
				print("should start recording");
			}
			isRecording = true;
			MicHandle.instance.startRecording();
			targetInput = buttonInput;
		}
	}
}
