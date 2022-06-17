using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Noise
{
    public class gaussianNoise
    {
        private float Mean;//高斯噪声均值
        private float StdDev;//高斯噪声标准偏差
        public gaussianNoise(float gauss_mean, float gauss_stdDev )
        {
            this.Mean = gauss_mean;
            this.StdDev = gauss_stdDev;
           
          //  Debug.Log("this.dy" + this.StdDev);
        }
    
        public float randData()
        {
            float u1, u2, z;
            float x;
            if (this.StdDev <= 0)
            {

                return this.Mean;
            }
            u1 = Random.Range(0.0f, 1.0f);
            u2 = Random.Range(0.0f,1.0f);

          //  Debug.Log("u1" + u1);
            z = Mathf.Sqrt(-2 * Mathf.Log10(u1)) * Mathf.Sin(2 * Mathf.PI * u2);
           // Debug.Log("z"+z);
            x = this.Mean + (this.StdDev * z);
            return x;
        }

        //获取高斯噪声的均值
        public float getMean()
        {
            return this.Mean;
        }
        //获取高斯噪声的标准偏差
        public float getStdDev()
        {
            return this.StdDev;
        }

      
       
        public void SetMean(float mean)
        {
            this.Mean = mean;
        }

        
        public void SetStdDev(float stddev)
        {
            this.StdDev = stddev;
        }

   

    }
}
