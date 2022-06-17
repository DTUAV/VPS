using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV4
{
    public class LidarSensorV4 : MonoBehaviour
    {
        public string Lidarname = "LidarV4";//激光雷达传感器的名字
        public int LaserCount = 2;//激光雷达的线束
        public float MinDistance = 0.01f;//测量的最小距离
        public float MaxDistance = 5f;//测量的最大距离
        public int MeasurementsPerRotation = 500;//每圈每个线束测量的点数
        public float HorizontalAngleAll = 360;//水平旋转角度
        public float FOV = 10;//垂直角度
        public float CenterAngle = 0;//中心角度       
        public float VerticalAngleDealt;//垂直角度的增量
        public float HorizontalAngleDealt;//水平角度的增量
        public GameObject LidarLaserObject;
        public List<LidarSensorALLData> hits = new List<LidarSensorALLData>();
        public List<Vector3> hitDataPosition = new List<Vector3>();//射线终点位置
        public List<float> hitDensity = new List<float>();//射线终点的密度
        public List<double> hitDistant = new List<double>();//射线终点的距离
        private List<float> VerticalAngle = new List<float>();//每个线束的垂直角度
        private List<float> HorizontalAngle = new List<float>();//每个测量点的水平角度
        private List<LidarLaser> lasers = new List<LidarLaser>();
        public bool sendOK = false;
        public bool updateOK = true;
        // public List<Vector3> HitPosition
        void LidarInit()
        {
            VerticalAngleDealt = FOV/LaserCount-1;
            HorizontalAngleDealt = HorizontalAngleAll / MeasurementsPerRotation;
            for (int i = 0; i < LaserCount; i++)
            {
                lasers.Add(new LidarLaser(LidarLaserObject, CenterAngle + FOV / 2 - i * VerticalAngleDealt, MaxDistance, i));

                VerticalAngle.Add(CenterAngle + FOV / 2 - i * VerticalAngleDealt);
            }
            for (int j = 0; j <= MeasurementsPerRotation; j++)
            {
                HorizontalAngle.Add(j * HorizontalAngleDealt);
            }
         
        }



        // Start is called before the first frame update
        void Start()
        {
            LidarInit();
        }

        // Update is called once per frame
        void Update()
        {
            
        /*    foreach (LidarLaser laser in lasers)
            {
                laser.DrawRay();
            }
          */  
            
        }

        void FixedUpdate()
        {
            if (updateOK)
            {

                hitDataPosition.Clear();
                hitDensity.Clear();
                hitDistant.Clear();
                hits.Clear();
                hits = new List<LidarSensorALLData>();
                hitDataPosition = new List<Vector3>();
                hitDensity = new List<float>();
                hitDistant = new List<double>();
                float horizontalAngle = 0;

                for (int i = 0; i < MeasurementsPerRotation; i++)
                {
                    
                        LidarLaserObject.transform.Rotate(0, HorizontalAngleDealt, 0, Space.Self);
                   // Debug.Log(LidarLaserObject.transform.rotation.eulerAngles);
                        horizontalAngle = horizontalAngle + MeasurementsPerRotation;

                        foreach (LidarLaser laser in lasers)
                        {
                            laser.laser_color = Color.green;
                            RaycastHit hit = laser.ShootRay();
                            float distance = hit.distance;
                            if (distance != 0) // Didn't hit anything, don't add to list.
                            {
                                float verticalAngle = laser.GetVerticalAngle();
                                hits.Add(new LidarSensorALLData(LidarLaserObject.transform.position, distance, verticalAngle, horizontalAngle, hit.point, laser.GetLaserId()));
                                Vector3 b = hit.point;
                                hitDistant.Add(distance);
                                // lock (hitDataPosition)
                                {
                                    hitDataPosition.Add(b);
                                }
                                float a = (hit.distance * 255) / MaxDistance;
                                //Debug.Log("a" + a);
                                // lock (hitDensity)
                                {
                                    hitDensity.Add((int)a);
                                }
                            }
                            else
                            {
                                hitDistant.Add(MaxDistance);
                                float dx = laser.ray.direction.x / (Mathf.Sqrt(laser.ray.direction.x * laser.ray.direction.x + laser.ray.direction.y * laser.ray.direction.y + laser.ray.direction.z * laser.ray.direction.z));
                                float dy = laser.ray.direction.y / (Mathf.Sqrt(laser.ray.direction.x * laser.ray.direction.x + laser.ray.direction.y * laser.ray.direction.y + laser.ray.direction.z * laser.ray.direction.z));
                                float dz = laser.ray.direction.z / (Mathf.Sqrt(laser.ray.direction.x * laser.ray.direction.x + laser.ray.direction.y * laser.ray.direction.y + laser.ray.direction.z * laser.ray.direction.z));
                                // lock (hitDataPosition)
                                {
                                    hitDataPosition.Add(new Vector3(laser.ray.origin.x + laser.rayDistance * dx, laser.ray.origin.y + laser.rayDistance * dy, laser.ray.origin.z + laser.rayDistance * dz));
                                }
                                float a = (laser.rayDistance * 255) / MaxDistance;
                                //Debug.Log(a);
                                //  lock (hitDensity)
                                {
                                    hitDensity.Add((int)a);
                                }
                                //Debug.Log(laser.ray.direction.x*dx);
                                // Debug.Log(new Vector3(laser.ray.origin.x + laser.rayDistance * dx, laser.ray.origin.y + laser.rayDistance * dy, laser.ray.origin.z + laser.rayDistance * dz));
                            }
                        }



                    
                }
                    sendOK = true;
                    updateOK = false;
                }
            

        }
    }
}