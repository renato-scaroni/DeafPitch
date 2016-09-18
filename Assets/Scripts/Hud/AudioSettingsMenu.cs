using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour {

	private MicHandle.AvailableInputs targetInput;
	private bool isRecording = false;

	public Button Swim1Button;
	public Button Swim2Button;

	public Sprite normal1;
	public Sprite normal2;
	public Sprite recording;
	public Sprite tryAgain;

	public float failed1 = 0;
	public float failed2 = 0;


	public bool DEBUG;

	// Use this for initialization
	void Start () {
		Swim1Button.onClick.AddListener(() => { ToggleRecording(MicHandle.AvailableInputs.Swim1, out failed1); });
		Swim2Button.onClick.AddListener(() => { ToggleRecording(MicHandle.AvailableInputs.Swim2, out failed2); });
	}
	
	// Update is called once per frame
	void Update () {
		failed1 -= Time.deltaTime;
		failed2 -= Time.deltaTime;

		updateButtonColor (
			Swim1Button,
			MicHandle.AvailableInputs.Swim1,
			failed1 < 0 ? normal1 : tryAgain
		);

		updateButtonColor (
			Swim2Button,
			MicHandle.AvailableInputs.Swim2,
			failed2 < 0 ? normal2 : tryAgain
		);
	}

	void updateButtonColor (Button button, MicHandle.AvailableInputs buttonInput, Sprite normalSprite) {

		if (isRecording) {
			if (targetInput == buttonInput)
				button.image.sprite = recording;
			else
				button.image.sprite = normalSprite;
		}
		else {
			if (MicHandle.instance.getInputDown(buttonInput) ||
				MicHandle.instance.getInputState(buttonInput, MicHandle.InputStates.JustPressed)) {
				button.image.sprite = normalSprite;
				button.image.color = Color.red;
			}

			else if (MicHandle.instance.getInputHold(buttonInput)) {
				button.image.sprite = normalSprite;
				button.image.color = Color.red;
			}
			
			else {
				button.image.sprite = normalSprite;
				button.image.color = Color.white;
			}
		}
	}


	void ToggleRecording (MicHandle.AvailableInputs buttonInput, out float failTime) {
		if (isRecording) {
			if (targetInput == buttonInput) {
				isRecording = false;
				float newPitch = MicHandle.instance.stopAndGetAverage();

				if (!MicHandle.instance.setAsDefaultInputIfPossible(targetInput, newPitch))
					failTime = 5;
				else
					failTime = 0;
			}

			else
				failTime = 0;
		}

		else {
			if (DEBUG) {
				print("should start recording");
			}
			isRecording = true;
			MicHandle.instance.startRecording();
			targetInput = buttonInput;

			failTime = 0;
		}
	}
}
