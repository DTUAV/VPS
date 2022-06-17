using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimUnity.Sensor.SensorData;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using System.Threading;
using SimUnity.Noise;
namespace SimUnity.Sensor.LidarSensorV3
{
   
    public class LidarSensorV3 : MonoBehaviour
    {
        public string Lidarname = "LidarV3";//激光雷达传感器的名字
        public int LaserCount = 32;//激光雷达的线束
        public float MinDistance = 0.01f;//测量的最小距离
        public float MaxDistance = 50f;//测量的最大距离
        public int TimePerRotation = 12;//一圈需要旋转的次数
        public int MeasurementsPerRotation = 1200;//每圈每个线束测量的点数
        public float HorizontalAngleAll = 360;//水平旋转角度
        public float FOV = 30;//垂直角度
        public float CenterAngle = 0;//中心角度
        private List<float> VerticalAngle = new List<float>();//每个线束的垂直角度
        private List<float> HorizontalAngle = new List<float>();//每个测量点的水平角度
        public float VerticalAngleDealt;//垂直角度的增量
        public float HorizontalAngleDealt;//水平角度的增量
        public Camera LidarCamera;//激光雷达相机
        public Texture2D lastImage;//图像
        public Texture2D currentImage;
        public RawImage rawImage;
        private int maxCameraReaderWidth ;//最大渲染宽度
        private int maxCameraReaderHeight;//最大渲染高度
        private float rotationAngle;//每次旋转的角度
        public List<Texture2D> texture2Ds = new List<Texture2D>();
        public List<Mat> mats = new List<Mat>();
        Texture2D readRenderTex;
        public LidarSensorDataStruct lidarSensorDataStruct;
        public List<LidarSensorDataKeep> lidarSensorDataKeeps;
        public List<double> distanctLaser1 = new List<double>();
        public List<double> densityLaser1 = new List<double>();
        public List<double> distanctLaser2 = new List<double>();
        public List<double> densityLaser2 = new List<double>();
        public float[] distanct1;
        public float[] density1;
        private Thread datathread;
        private bool endOperation = true;
        private float lastTime;
        private float currentTime;
        public float dealtTime;
        void LidarInit()
        {
            rotationAngle = 360 / TimePerRotation;
            VerticalAngleDealt = LaserCount / FOV;
            HorizontalAngleDealt =  HorizontalAngleAll / MeasurementsPerRotation;
            for(int i = 0; i<= LaserCount;i++)
            {

                VerticalAngle.Add( CenterAngle + FOV / 2 - i * VerticalAngleDealt);
            }
            for(int j = 0; j<= MeasurementsPerRotation;j++)
            {
                HorizontalAngle.Add(j * MeasurementsPerRotation);
            }
            maxCameraReaderWidth =  (int)MeasurementsPerRotation/TimePerRotation;
            maxCameraReaderHeight = LaserCount;
            lastImage = new Texture2D(1, LaserCount, TextureFormat.RGB24,false);
            currentImage = new Texture2D(1, LaserCount, TextureFormat.RGB24, false);
            readRenderTex = new Texture2D(1, LaserCount, TextureFormat.RGBA32, false);
            LidarCamera.targetTexture = new RenderTexture(maxCameraReaderWidth, maxCameraReaderHeight, 24);
           // LidarCamera.targetTexture = new RenderTexture(500, 32, 24);
            LidarCamera.targetTexture.Create();
            LidarCamera.aspect = 1;
        }
        void LidarCameraInit()
        {
            if(LidarCamera == null)
            {
                Debug.LogError("未初始化相机");
            }
            else
            {
                LidarCamera.transform.localEulerAngles = new Vector3(0,0,0);
                LidarCamera.fieldOfView = FOV;
                LidarCamera.farClipPlane = MaxDistance;
                LidarCamera.nearClipPlane = MinDistance;
            }
        }
        void LidarSensorDataInit()
        {
            lidarSensorDataStruct.CenterAngle = CenterAngle;
            lidarSensorDataStruct.FOV = FOV;
            lidarSensorDataStruct.HorizontalAngleAll = HorizontalAngleAll;
            lidarSensorDataStruct.HorizontalAnglePer = HorizontalAngleDealt;
            lidarSensorDataStruct.LidarName = Lidarname;
            lidarSensorDataStruct.LidarNumber = LaserCount;
            lidarSensorDataStruct.MaxDistance = MaxDistance;
            lidarSensorDataStruct.MinDistance = MinDistance;
            lidarSensorDataStruct.TimePerRotation = TimePerRotation;

        }
        void ThreadInit()
        {
            datathread = new Thread(thread_data);
            datathread.Start();
            endOperation = false;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            LidarInit();
            LidarCameraInit();
            distanct1 = new float[MeasurementsPerRotation];
            density1 = new float[MeasurementsPerRotation];
            ThreadInit();
        }

      

        // Update is called once per frame
        
        void OnPostRender()
        {
            currentTime = Time.time;
            dealtTime = currentTime - lastTime;
            lastTime = currentTime;
            RenderTexture.active = LidarCamera.targetTexture;
            for (int i = 0; i < MeasurementsPerRotation/TimePerRotation; i++)
            {
               
                Texture2D texture2D = new Texture2D(1, 32, TextureFormat.RGBA32, false);
                texture2D.ReadPixels(new UnityEngine.Rect(maxCameraReaderWidth - 1 - i, 0, 1, LaserCount), 0, 0);
                // readRenderTex.ReadPixels(new UnityEngine.Rect(maxCameraReaderWidth -1 - i, 0, 1, LaserCount), 0, 0);
                //readRenderTex.Apply();
                //rawImage.texture = readRenderTex; 
                Mat dst = new Mat(texture2D.height, texture2D.width, CvType.CV_8UC4);
                Utils.texture2DToMat(texture2D, dst);
                rawImage.texture = texture2D;
                texture2Ds.Add(texture2D);
                lock (mats)
                {
                    mats.Add(dst);
                }
                // texture2Ds.Add(readRenderTex);        
            }
            RenderTexture.active = null;
            LidarCamera.transform.Rotate(0, rotationAngle, 0);
            //Debug.Log(texture2Ds.Count);
        }

        void thread_data()
        {
            while (!endOperation)
            {
                if (mats.Count >= MeasurementsPerRotation)
                {
                    lock (mats)
                    {
                        int i = 0;
                        foreach (Mat mat in mats)
                        {
                            if (mat == null)
                            {
                                Debug.Log("ddd");
                            }
                            else
                            {
                                //  for(int i = 0;i<dst.rows(); i++)
                                // {

                                // }
                                // distanctLaser1.Add(Deapthdistant(dst.get(0, 0)[0]));
                                //densityLaser1.Add(dst.get(0, 0)[0]);
                                // distanctLaser1.Add(Deapthdistant(dst.get(1, 0)[0]));
                                // densityLaser1.Add(dst.get(1, 0)[0]);
                                // Debug.Log("cols" + dst.cols());
                                // Debug.Log("rows" + dst.rows());
                               //  Debug.Log("XXXX" + mat.get(30, 0)[0] + mat.get(25, 0)[1] + mat.get(15, 0)[2] + mat.get(15, 0)[3]);
                                distanct1[i] = (float)Deapthdistant(mat.get(25, 0)[0])+ (float)GaussNoisPlugin.GaussianNoiseData(0,0.02);
                                density1[i] = (float)mat.get(25, 0)[0];
                                i++;
                            }

                        }
                        mats.RemoveRange(0, MeasurementsPerRotation);
                    }
                }
            }
        }
        public double Deapthdistant(double DeapthData)
        {
            return DeapthData/255 * (MaxDistance - MinDistance) + MinDistance;
        }
        void getLidarData()
        {
            if(texture2Ds.Count>= MeasurementsPerRotation)
            {  //旋转一周读取一次数据

                int i = 0;
                foreach (Texture2D texture2D in texture2Ds.GetRange(0,MeasurementsPerRotation) )
                {

                    Mat dst = new Mat(texture2D.height, texture2D.width, CvType.CV_8UC4);
                    Utils.texture2DToMat(texture2D, dst);
                    if (dst == null)
                    {
                        Debug.Log("ddd");
                    }
                    else
                    {
                        //  for(int i = 0;i<dst.rows(); i++)
                        // {

                        // }
                        // distanctLaser1.Add(Deapthdistant(dst.get(0, 0)[0]));
                        //densityLaser1.Add(dst.get(0, 0)[0]);
                        // distanctLaser1.Add(Deapthdistant(dst.get(1, 0)[0]));
                        // densityLaser1.Add(dst.get(1, 0)[0]);
                        // Debug.Log("cols" + dst.cols());
                        // Debug.Log("rows" + dst.rows());
                        // Debug.Log("XXXX" + dst.get(30, 0)[0] + dst.get(25, 0)[1] + dst.get(15, 0)[2] + dst.get(15, 0)[3]);
                        distanct1[i] = (float)Deapthdistant(dst.get(25, 0)[0]);
                        density1[i] = (float)dst.get(25, 0)[0];
                        
                        i++;
                    }
                    dst.release();
                    
                }

                texture2Ds.RemoveRange(0, MeasurementsPerRotation);
            }
        }
        void FixedUpdate()
        {
            //getLidarData();
        }
        void OnDestroy()
        {
           
            endOperation = true;
            if(lastImage != null)
            {
                   
            }
        }



    }
}
