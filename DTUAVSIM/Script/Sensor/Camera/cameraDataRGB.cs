using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor
{
    public class cameraDataRGB : MonoBehaviour
    {
        // Start is called before the first frame update
        public Camera cameraSensor;
        public int width = 120;
        public int height = 120;
        public Texture2D cameraData;
        private Rect rect;
        RenderTexture rt;
        public byte[] imageData;
        public byte[] imageDataMatla;
        public bool getImageDataFlag = true;
        public bool sendImageDataFlag = false;
        void Start()
        {
            imageData = new byte[width * height * 3];
            cameraData = new Texture2D(width, height, TextureFormat.RGB24, false);
            rect = new Rect(0, 0, width, height);
            cameraSensor.targetTexture = new RenderTexture(width, height, 24);
            rt = new RenderTexture(width, height, 24);
            cameraSensor.targetTexture = rt;
        }
       
        // Update is called once per frame
        void Update()
        {
            if(getImageDataFlag == true)
            {
                RenderTexture.active = rt;
                cameraData.ReadPixels(rect, 0, 0);
                cameraData.Apply();
                RenderTexture.active = null;
                getImageDataFlag = false;
                sendImageDataFlag = true;
               
                imageData = cameraData.GetRawTextureData();
                imageDataMatla = cameraData.EncodeToPNG();

                Debug.Log("picture size "+imageDataMatla.Length);
                
            }
        }
    }
}
