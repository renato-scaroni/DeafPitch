using UnityEngine;
using System.Collections;
 
public class Spectrum : MonoBehaviour
{
 
    //reduced to 512 sampling the max is a bit much ;)
    public int numOfSamples = 256; //Min: 64, Max: 8192
 
    //Removed Updated to AudioListener
    //public AudioSource aSource;

	public float inputThreshold = 0.01f;
	public float epsilon = 0.005f;

    public float[] freqData;
    public float[] band;
 
    public GameObject[] g;
 
	private float [] mappedInputs;

    void Start()
    {
        freqData = new float[numOfSamples];
 
        int n = freqData.Length;
 
        int k = freqData.Length;
        // for (int j = 0; j < freqData.Length; j++)
        // {
        //     n = n / 2;
        //     if (n <= 0) break;
        //     k++;
        // }
 
        band = new float[k + 1];
        g = new GameObject[k + 1];
 
 
        for (int i = 0; i < band.Length; i++)
        {
            band[i] = 0;
            g[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g[i].GetComponent<Renderer>().material.SetColor("_Color", Color.cyan);
            g[i].transform.position = new Vector3(i, 0, 0);
        }
 
		mappedInputs = new float [10];

		for (int i = 0; i < 10; i++)
		{
			mappedInputs[i] = 0;
		}

        InvokeRepeating("check", 0.0f, 1.0f / 15.0f); // update at 15 Hz
 
    }
 
    private void check()
    {
        //Updated to Audio Listener this removes the tie to your audio source
        //Have one Audio Source in the scene playing, this allows multiple Listeners
        //to process the same Audio Source in a scene without tying up resources for
        //Source playing multiple sounds
        AudioListener.GetSpectrumData(freqData, 0, FFTWindow.Rectangular);
 
        int k = 0;
        int crossover = 2;
		
		int maxi = 0;
        for (int i = maxi; i < freqData.Length; i++)
        {
            float d = freqData[i];
            float b = band[k];

            // find the max as the peak value in that frequency band.
			if(d > b)
			{
				band[k] = d;
				maxi = i;
			}
			// band[k] = (d > b) ? d : b;

            // if (i > (crossover - 3))
            // {
            //     k++;
            //     crossover *= 2;   // frequency crossover point for each band.
				k = i;
                Vector3 tmp = new Vector3(g[k].transform.position.x, d * 1000, g[k].transform.position.z);
                g[k].transform.position = tmp;
            //     band[k] = 0;
            // }
        }

		// print("The input was " + maxi + " " + ((float)maxi) * 44000f/512f);
		// if(maxd > inputThreshold)
		// {
		// 	print("The input was " + maxd + " the mapped input is " + mappedInputs[0] + " they are equal " + (Mathf.Abs(mappedInputs[0] - maxd) < epsilon));
		// 	if(mappedInputs[0] == 0)
		// 	{
		// 		mappedInputs[0] = maxd;
		// 	}
		// }
    }
}
 
 
 