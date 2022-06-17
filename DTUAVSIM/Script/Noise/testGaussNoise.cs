using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
public class testGaussNoise : MonoBehaviour
{
    // Start is called before the first frame update
    public gaussianNoise gaussianNoise;
    void Start()
    {
        gaussianNoise = new gaussianNoise(0,0.002f);
        //   Debug.Log(gaussianNoise1.sampleData(0.01f, 0.0001f));

        gaussianNoise.randData();
    }

    // Update is called once per frame
    void Update()
    {
       Debug.Log( GaussNoisPlugin.GaussianNoiseData(0, 0.1));

    }
}
