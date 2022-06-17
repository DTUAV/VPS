using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.Wireless
{
    


    public class WirelessReceiver : MonoBehaviour
    {
        [Header("接受端的频率范围")]
        [Header("频率下限")]
        public float minFreq = 2412;//MHz
        [Header("频率上限")]
        public float maxFreq = 2484;

        [Header("天线的灵敏度")] public float sensitivity = -90;//dBm
        [Header("接收端的增益")] public float gain = 2.5f;//dBi
        [Header("接收器对象")] public GameObject recv_obj;
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