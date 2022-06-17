using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
namespace SimUnity.Sensor.UWB
{
    public struct UWB_Data
    {
        public int deviceID;
        public string deviceName;
        public Vector3 deviceRfPosition;
        public float distance;
    }



    public class UWB_DeviceR : MonoBehaviour
    {
        [Header("The UWB DeviceR ID")] public int DeviceRID = 0;

        [Header("The UWB DeviceR Name")] public string DeviceRName = "uab_r";

        [Header("The UWB DeviceR 3D Model")] public GameObject DeviceRObject = null;

        [Header("The Range of UWB DeviceR")] public float DeviceRRange = 40.0f;

        [Header("The UWB DeviceR Data Nois")] 
        public float GassMean = 0;

        public float GassDev = 0.3f;

        [Header("The Number of UWB DeviceS")] 
        public uint UWB_DeviceS_Num = 4;

        [Header("The UWB DeviceS")] 
        public List<UWB_DeviceS> UwbDeviceSes;

        [Header("Open UWB DeviceR")] public bool IsOpenDevR = true;

        [Header("Open UWB DeviceR Show")] public bool IsOpenRShow = true;

        [Header("The UWB DeviceR State of Current Condition")] [Header("The Light of Green " + " Run Normal")]
        public bool GreenRun = true;

        [Header("The Light of Red " + " Run Abnormal")]
        public bool RedRun = false;

        [Header("The Data of UWB DeviceR")]
        private List<UWB_Data> UwbDatas;
        public float Uwb1DataDistant;
        // Start is called before the first frame update
        void Start()
        {
            if (DeviceRObject == null)
            {
                Debug.LogError("Please Check The UWB Device 3D Model");
                GreenRun = false;
                RedRun = true;
            }
            else
            {
                if (IsOpenDevR)
                {
                    GreenRun = true;
                    RedRun = false;
                    UwbDatas = new List<UWB_Data>();
                    if (IsOpenRShow)
                    {
                        DeviceRObject.GetComponent<Light>().enabled = true;
                        DeviceRObject.GetComponent<Light>().range = DeviceRRange;
                    }
                }

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (IsOpenRShow)
            {
                DeviceRObject.GetComponent<Light>().enabled = true;
                DeviceRObject.GetComponent<Light>().range = DeviceRRange;
            }
            else
            {
                DeviceRObject.GetComponent<Light>().enabled = false;
            }
        }

        void FixedUpdate()
        {
            foreach (UWB_DeviceS uwbDeviceS in UwbDeviceSes)
            {
                UWB_Data uwbData;
                uwbData.deviceID = uwbDeviceS.DeviceSID;
                uwbData.deviceName = uwbDeviceS.DeviceSName;
                uwbData.deviceRfPosition = uwbDeviceS.DeviceSRfPos;
                uwbData.distance = Vector3.Distance(uwbDeviceS.DeviceSObject.transform.position, DeviceRObject.transform.position) + (float)GaussNoisPlugin.GaussianNoiseData(GassMean,GassDev);
                if (uwbData.distance > DeviceRRange)
                {
                    uwbData.distance = 0;
                }
                UwbDatas.Add(uwbData);
                if (uwbData.deviceID == 1)
                {
                    Uwb1DataDistant = uwbData.distance;
                }
            }

            if (UwbDatas.Count >=8)
            {
                UwbDatas.RemoveRange(0,4);
            }
        }
    }
}
