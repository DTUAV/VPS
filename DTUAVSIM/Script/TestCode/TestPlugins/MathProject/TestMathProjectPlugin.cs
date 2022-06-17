/*
 * 2021 Yuanlin Yang, Guandong University of Technology, Guanzhou,China 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Math;
public class TestMathProjectPlugin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int min_value = 0;
        int max_value = 1;
        Debug.Log("RandomDoubleWithDefaultRandomEngine: "+SimMath.RandomDoubleWithDefaultRandomEngine(min_value,6));
        Debug.Log("NormalDistribution: "+SimMath.NormalDistribution(min_value,max_value));
        Debug.Log("RandomDoubleWithMt19937_64: "+SimMath.RandomDoubleWithMt19937_64(min_value,max_value));
        Debug.Log("RandomIntWithDefaultRandomEngine: "+SimMath.RandomIntWithDefaultRandomEngine(min_value,max_value));
        Debug.Log("RandomIntWithMt19937_64: "+SimMath.RandomIntWithMt19937_64(min_value,max_value));
        Debug.Log("RandomLongWithDefaultRandomEngine: "+SimMath.RandomLongWithDefaultRandomEngine(min_value,max_value));
        Debug.Log("RandomLongWithMt19937_64: "+SimMath.RandomLongWithMt19937_64(min_value,max_value));
        Debug.Log("UniformIntDistribution: "+SimMath.UniformIntDistribution(min_value,max_value));
        
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
