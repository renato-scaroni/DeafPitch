using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
 
public class MicHandle : MonoBehaviour {
 
  private const int FREQUENCY = 48000;    // Wavelength, I think.
  private const int SAMPLECOUNT = 1024;   // Sample Count.
  private const float REFVALUE = 0.1f;    // RMS value for 0 dB.
  private const float THRESHOLD = 0.01f;  // Minimum amplitude to extract pitch (recieve anything)
  private const float ALPHA = 0.05f;      // The alpha for the low pass filter (I don't really understand this).
 
  public int recordedLength = 50;    // How many previous frames of sound are analyzed.
  public int requiedBlowTime = 4;    // How long a blow must last to be classified as a blow (and not a sigh for instance).
  public int clamp = 160;            // Used to clamp dB (I don't really understand this either).
  public float epsilon = 30;         //
 
  private float rmsValue;            // Volume in RMS
  private float dbValue;             // Volume in DB
  private float pitchValue;          // Pitch - Hz (is this frequency?)
  private int blowingTime;           // How long each blow has lasted
 
  private float lowPassResults;      // Low Pass Filter result
  private float peakPowerForChannel; //
 
  private float[] samples;           // Samples
  private float[] spectrum;          // Spectrum
  private List<float> dbValues;      // Used to average recent volume.
  private List<float> pitchValues;   // Used to average recent pitch.
 

  ///////
  //  Input states handling
  ///////

  public bool DEBUG = true;
  public enum AvailableInputs {None, Swim1, Swim2, Attack, Jump};
  public enum InputStates {None, Pressed, JustPressed, Hold, Released};
  private Dictionary<AvailableInputs, float> targetPitchs;
  private Dictionary<AvailableInputs, InputStates> currentStates;

  private int holdCount = 0;
  public int holdThreshold = 10;



  ///////
  //  Default Methods
  ///////

  public void Start () {
    samples = new float[SAMPLECOUNT];
    spectrum = new float[SAMPLECOUNT];
    dbValues = new List<float>();
    pitchValues = new List<float>();
 
    targetPitchs = new Dictionary<AvailableInputs, float>
    {
        { AvailableInputs.Swim1, 550},
        { AvailableInputs.Swim2, 750}
    };

    currentStates = new Dictionary<AvailableInputs, InputStates>
    {
        { AvailableInputs.Swim1, InputStates.None},
        { AvailableInputs.Swim2, InputStates.None}
    };

    StartMicListener();
  }
 
  public void LateUpdate () {
    // If the audio has stopped playing, this will restart the mic play the clip.
    if (!GetComponent<AudioSource>().isPlaying) {
      StartMicListener();
    }
 
    // Gets volume and pitch values
    AnalyzeSound();
 
    // Runs a series of algorithms to decide whether a blow is occuring.
    // DeriveBlow();
 
	  // if (firstPitch == 0 && pitchValue > 0) firstPitch = pitchValue;
    updateInputStates();
  }
 


  ///////
  //  Sound Handling Methods
  ///////

  /// Starts the Mic, and plays the audio back in (near) real-time.
  private void StartMicListener() {
    GetComponent<AudioSource>().clip = Microphone.Start("Built-in Microphone", true, 999, FREQUENCY);

    // HACK - Forces the function to wait until the microphone has started, before moving onto the play function.
    while (!(Microphone.GetPosition("Built-in Microphone") > 0));
    
    GetComponent<AudioSource>().Play();

    // print("InvokeRepeating");
    // InvokeRepeating("AnalyzeSound", 0.0f, 1.0f / 15.0f); // update at 15 fps
  }
 
  /// Credits to aldonaletto for the function, http://goo.gl/VGwKt
  /// Analyzes the sound, to get volume and pitch values.
  private void AnalyzeSound() {
 
    // Get all of our samples from the mic.
    GetComponent<AudioSource>().GetOutputData(samples, 0);
 
    // Sums squared samples
    float sum = 0;
    for (int i = 0; i < SAMPLECOUNT; i++){
      sum += Mathf.Pow(samples[i], 2);
    }
 
    // RMS is the square root of the average value of the samples.
    rmsValue = Mathf.Sqrt(sum / SAMPLECOUNT);
    dbValue = 20 * Mathf.Log10(rmsValue / REFVALUE);
 
    // Clamp it to {clamp} min
    if (dbValue < -clamp) {
      dbValue = -clamp;
    }
 
    // Gets the sound spectrum.
    GetComponent<AudioSource>().GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    float maxV = 0;
    int maxN = 0;
 
    // Find the highest sample.
    for (int i = 0; i < SAMPLECOUNT; i++){
      if (spectrum[i] > maxV && spectrum[i] > THRESHOLD){
        maxV = spectrum[i];
        maxN = i; // maxN is the index of max
      }
    }
 
    // Pass the index to a float variable
    float freqN = maxN;
 
    // Interpolate index using neighbours
    if (maxN > 0 && maxN < SAMPLECOUNT - 1) {
      float dL = spectrum[maxN-1] / spectrum[maxN];
      float dR = spectrum[maxN+1] / spectrum[maxN];
      freqN += 0.5f * (dR * dR - dL * dL);
    }
 
    // Convert index to frequency
    pitchValue = freqN * 24000 / SAMPLECOUNT;


    // Print Debugs
    // if (DEBUG && pitchValue > 0) {
    //   print("Pitch Value: " + pitchValue);
    // }
  }
 
  private void DeriveBlow() {
 
    UpdateRecords(dbValue, dbValues);
    UpdateRecords(pitchValue, pitchValues);
 
    // Find the average pitch in our records (used to decipher against whistles, clicks, etc).
    float sumPitch = 0;
    foreach (float num in pitchValues) {
      sumPitch += num;
    }
    sumPitch /= pitchValues.Count;
 
    // Run our low pass filter.
    lowPassResults = LowPassFilter(dbValue);
 
    // Decides whether this instance of the result could be a blow or not.
    if (lowPassResults > -30 || sumPitch == 0) {
      blowingTime += 1;
    } else {
      blowingTime = 0;
    }
  }
 
  // Updates a record, by removing the oldest entry and adding the newest value (val).
  private void UpdateRecords(float val, List<float> record) {
    if (record.Count > recordedLength) {
      record.RemoveAt(0);
    }
    record.Add(val);
  }
 
  /// Gives a result (I don't really understand this yet) based on the peak volume of the record
  /// and the previous low pass results.
  private float LowPassFilter(float peakVolume) {
    return ALPHA * peakVolume + (1.0f - ALPHA) * lowPassResults;
  }



  ///////
  //  Input State Methods
  ///////

  // public enum AvailableInputs {Swim1, Swim2, Attack, Jump};
  // public enum InputStates {None, Pressed, JustPressed, Hold, Released};
  // private Dictionary<AvailableInputs, float> targetPitchs;
  // private Dictionary<AvailableInputs, InputStates> currentStates;

  // private int holdCount = 0;
  // public int holdThreshold = 10;


  private AvailableInputs checkLatestInput() {
    AvailableInputs currentInput = AvailableInputs.None;
    float minDifference = epsilon;

    foreach(KeyValuePair<AvailableInputs, float> entry in targetPitchs)
    {
      float difference = Mathf.Abs(pitchValue - entry.Value);
      if (difference < minDifference) {
        minDifference = difference;

        currentInput = entry.Key;
      }
    }

    return currentInput;
  }

  private void updateInputStates() {
    AvailableInputs lastInput = checkLatestInput();

    List<AvailableInputs> keys = new List<AvailableInputs> (currentStates.Keys);
    foreach (AvailableInputs key in keys) {
      InputStates currentValue = currentStates[key];

      if (key == lastInput) {

        // Set Pressed
        if (currentValue == InputStates.None || currentValue == InputStates.Released) {
          currentStates[key] = InputStates.Pressed;
          holdCount = 0;
        }

        // Check for hold
        else if (currentValue == InputStates.Pressed || currentValue == InputStates.JustPressed) {
          holdCount++;
          if (holdCount > holdThreshold)
            currentStates[key] = InputStates.Hold;
          
          else
            currentStates[key] = InputStates.JustPressed;
        }

        // Maintain hold
        else
          holdCount++;
      }

      else {
        // Set Released
        if (currentValue == InputStates.None || currentValue == InputStates.Released)
          currentStates[key] = InputStates.None;

        // Set no input
        else
          currentStates[key] = InputStates.Released;
      }
    }





    // UpdateRecords(dbValue, dbValues);
    // UpdateRecords(pitchValue, pitchValues);

    if (DEBUG) {
      InputStates swim1;
      InputStates swim2;
      currentStates.TryGetValue(AvailableInputs.Swim1, out swim1);
      currentStates.TryGetValue(AvailableInputs.Swim2, out swim2);

      print("Swim 1: " + InputStates.GetName(typeof(InputStates), swim1) + "    Swim 2: " + InputStates.GetName(typeof(InputStates), swim2));
    }
  }


  public bool getInputDown(AvailableInputs name) {
    InputStates currentState;

    currentStates.TryGetValue(name, out currentState);

    return currentState == InputStates.Pressed;
  }

  public bool getInputUp(AvailableInputs name) {
    InputStates currentState;

    currentStates.TryGetValue(name, out currentState);

    return currentState == InputStates.Released;
  }
  
  public bool getInputHold(AvailableInputs name) {
    InputStates currentState;

    currentStates.TryGetValue(name, out currentState);

    return currentState == InputStates.Hold;
  }
}