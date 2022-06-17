using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;

namespace SimUnity.Sensor.UWB
{
    public class UWB_DeviceS : MonoBehaviour
    {
        // Start is called before the first frame update
        [Header("The UWB DeviceS ID")] public int DeviceSID = 0;

        [Header("The UWB DeviceS Name")] public string DeviceSName = "uab_s";

        [Header("The UWB DeviceS 3D Model")] public GameObject DeviceSObject = null;

        [Header("The Reference Position of UWB DeviceS")]
        public Vector3 DeviceSRfPos = Vector3.zero;

        [Header("The Range of UWB DeviceS")] public float DeviceSRange = 40.0f;

        [Header("The UWB DeviceS Data Nois")] public float GassMean = 0;

        public float GassDev = 0.3f;

        [Header("Open UWB DeviceS")] public bool IsOpenDevS = true;

        [Header("Open UWB DeviceS Show")] public bool IsOpenSShow = true;

        [Header("The UWB DeviceS State of Current Condition")] [Header("The Light of Green " + " Run Normal")]
        public bool GreenRun = true;

        [Header("The Light of Red " + " Run Abnormal")]
        public bool RedRun = false;

        public Vector3 GetUWBDevSPosition()
        {
            if (GreenRun == true)
            {
                Vector3 position = Vector3.zero;

                position.x = DeviceSRfPos.x + (float) GaussNoisPlugin.GaussianNoiseData(GassMean, GassDev);
                position.y = DeviceSRfPos.y + (float) GaussNoisPlugin.GaussianNoiseData(GassMean, GassDev);
                position.z = DeviceSRfPos.z + (float) GaussNoisPlugin.GaussianNoiseData(GassMean, GassDev);
                return position;
            }
            else
            {
                Debug.LogError("NOT Open UWB DeviceS" + DeviceSID);
                return Vector3.zero;
            }
        }

        void Start()
        {
            if (DeviceSObject == null)
            {
                Debug.LogError("Please Check The UWB Device 3D Model");
                GreenRun = false;
                RedRun = true;
            }
            else
            {
                if (IsOpenDevS)
                {
                    GreenRun = true;
                    RedRun = false;
                    if (IsOpenSShow)
                    {
                        DeviceSObject.GetComponent<Light>().enabled = true;
                        DeviceSObject.GetComponent<Light>().range = DeviceSRange;
                    }
                }

            }


        }

        // Update is called once per frame
        void Update()
        {

            if (IsOpenSShow)
            {
                DeviceSObject.GetComponent<Light>().enabled = true;
                DeviceSObject.GetComponent<Light>().range = DeviceSRange;
            }
            else
            {
                DeviceSObject.GetComponent<Light>().enabled = false;
            }
        }
    }
}
