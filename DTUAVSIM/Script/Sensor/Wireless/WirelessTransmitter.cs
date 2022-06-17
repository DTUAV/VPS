using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
namespace SimUnity.Sensor.Wireless
{


    public class WirelessTransmitter : MonoBehaviour
    {

        [Header("天线增益")] public float Gain;
        [Header("发送端能量")] public float Power;
        [Header("发送端标识")] public int Id;
        [Header("发送频率")] public float Freq = 2442;//MHz
        [Header("传感器对象")] public GameObject trm_obj;
        [Header("传输模型参数")] [Header("有障碍物模型")] public float NEmpty = 6;
        [Header("没有障碍物模型")] public float NObstacle = 12;
        [Header("高斯噪声")] [Header("高斯噪声的均值")] public float Avg = 0;
        [Header("高斯噪声的方差")] public float Dev = 6;


        private float SpeedOfLight = 299792458;

        //计算接受点的信号强度
        public float getSignalStrength(Vector3 receiver_position, float receiver_gain)
        {
            //receiver_position是接收器的位置
            //receiver_gain是接收器的增益
            Vector3 start = trm_obj.transform.position;
            Vector3 end = receiver_position;
            if (start == end)
            {
                end.z += 0.00001f;
            }

            float n = NEmpty;
            Vector3 dire = trm_obj.transform.TransformDirection(end);
            Ray ray = new Ray(start,dire);
            if (!Physics.Raycast(ray, Mathf.Infinity, 8))
            {
                n = NObstacle;
            }

            float distance = Mathf.Max(1.0f, Vector3.Distance(start, end));
            float x = (float)GaussNoisPlugin.GaussianNoiseData((double)Avg, (double)Dev);
            float wavelength = SpeedOfLight / (Freq * 1000000);
            float rxPower = Power + Gain + receiver_gain - x + 20 * Mathf.Log10(wavelength) -
                            20 * Mathf.Log10(4 * Mathf.PI) - 10 * n * Mathf.Log10(distance);
            return rxPower;
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}