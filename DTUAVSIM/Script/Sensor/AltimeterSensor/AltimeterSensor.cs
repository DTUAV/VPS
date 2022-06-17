using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
namespace SimUnity.Sensor
{
   
    public class AltimeterSensor: MonoBehaviour
    {
        // Start is called before the first frame update
        private Rigidbody rb;
        private bool isNoise;
        private NoiseType noiseType;
        private double vertical_position;
        private double vertical_velocity;
        private double Mean = 0f;//高斯噪声均值
        private double StdDev = 0.01f;//高斯噪声标准偏差
        private Transform ObTransform;
        private double lastPosition = 0;
        public AltimeterSensor(Rigidbody rb)
        {
            this.rb = rb;

        }
        public void AltimeterSensorInit(Rigidbody rb1, bool isnoise,NoiseType noiseType1)
        {
            rb = rb1;
            isNoise = isnoise;
            noiseType = noiseType1;
           // Debug.Log("ddddffffffffffffffffdddddddddd");
            
            

        }
        public void AltimeterSensorInit(Rigidbody rb1, bool isnoise, NoiseType noiseType1, double mean, double stdDev)
        {
           rb = rb1;
            isNoise = isnoise;
           noiseType = noiseType1;
            if(isnoise)
            {
                if(noiseType == NoiseType.Gaussian)
                {
                   
                    Mean = mean;
                    StdDev = stdDev;
                    
                }
            }
            
        }

       

        public AltimeterSensor(Transform tf)
        {
            ObTransform = tf;

        }
        public void AltimeterSensorInit(Transform tf,bool isnoise, NoiseType noiseType1)
        {
            ObTransform = tf;
            isNoise = isnoise;
            noiseType = noiseType1;
            
            


        }
        public void AltimeterSensorInit(Transform tf, bool isnoise, NoiseType noiseType1, double mean, double stdDev)
        {
            ObTransform = tf;
            isNoise = isnoise;
            noiseType = noiseType1;
            if (isnoise)
            {
                if (noiseType == NoiseType.Gaussian)
                {
                 
                   Mean = mean;
                   StdDev = stdDev;
                   
                }
            }

        }

        //获取位置
        public void GetAltimeterData()
        {
         
            if (rb != null)
            {
                double obPosition = rb.position.y;
                double obVelocity = rb.velocity.y;
                if (isNoise)
                {
                    switch (noiseType)
                    {
                        case NoiseType.None:
                            {
                                vertical_position = obPosition;
                                vertical_velocity = obVelocity;
                                break;
                            }
                        case NoiseType.Gaussian:
                            {
                               vertical_position =obPosition + GaussNoisPlugin.GaussianNoiseData(Mean,StdDev);
                                vertical_velocity =obVelocity + GaussNoisPlugin.GaussianNoiseData(Mean,StdDev);
                                break;
                            }
                        case NoiseType.Other:
                            {
                               vertical_position = obPosition;
                                vertical_velocity = obVelocity;
                                break;
                            }


                    }
                }
                else
                {
                   vertical_position = obPosition;
                   vertical_velocity = obVelocity;
                }
            }
            else
            {
               
                double obPosition = ObTransform.position.y;
                
                double obVelocity = (obPosition-lastPosition)/Time.deltaTime;
                lastPosition = obPosition;
                if (isNoise)
                {
                    switch (noiseType)
                    {
                        case NoiseType.None:
                            {
                                vertical_position = obPosition;
                                vertical_velocity = obVelocity;
                                break;
                            }
                        case NoiseType.Gaussian:
                            {
                               
                             
                              vertical_position =obPosition + GaussNoisPlugin.GaussianNoiseData(Mean,StdDev);
                               
                               vertical_velocity =obVelocity + GaussNoisPlugin.GaussianNoiseData(Mean,StdDev);
                                break;
                            }
                        case NoiseType.Other:
                            {
                               vertical_position = obPosition;
                               vertical_velocity = obVelocity;
                                break;
                            }


                    }
                }
                else
                {
                    vertical_position = obPosition;
                   vertical_velocity = obVelocity;
                }
               
            }
        }

        void Start()
        {
            Mean = 0f;//高斯噪声均值
           StdDev = 0.1f;//高斯噪声标准偏差
    }
        void Update()
        {
            GetAltimeterData();
           // Debug.Log(GaussNoisPlugin.GaussianNoiseData(0, 0.1));
        }

        public Rigidbody getRigidbody()
        {
            return rb;
        }
        public void setRigidbody(Rigidbody setRb)
        {
           rb = setRb;
        }
        public bool getIsNoise()
        {
            return isNoise;
        }
        public void setIsNoise(bool setIsnoise)
        {
           isNoise = setIsnoise;
        }

        public NoiseType getNoiseType()
        {
            return noiseType;
        }

        public void setNoiseType(NoiseType setType)
        {
            noiseType = setType;
        }

        public double getVertical_position()
        {
            return vertical_position;
        }
        public double getVertical_velocity()
        {
            return vertical_velocity;
        }
        public Transform getTransform()
        {
            return ObTransform;
        }
        public void setTransform(Transform tf)
        {
            ObTransform = tf;
        }
    }
}
