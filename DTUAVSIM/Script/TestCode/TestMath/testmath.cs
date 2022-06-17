using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class testmath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        double[] data =  NormalDistribution();
        Debug.Log("data[0]: "+data[0] + "  data[1]: "+data[1]);

    }
    public static double[] NormalDistribution()
    {
        Random rand = new Random();
        double[] y;
        double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0, z2 = 0;
        while (s > 1 || s == 0)
        {
            u1 = rand.NextDouble();
            u2 = rand.NextDouble();
            v1 = 2 * u1 - 1;
            v2 = 2 * u2 - 1;
            s = v1 * v1 + v2 * v2;
        }
        z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
        z2 = Math.Sqrt(-2 * Math.Log(s) / s) * v2;
        y = new double[] { z1, z2 };
        return y; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
    }
}
