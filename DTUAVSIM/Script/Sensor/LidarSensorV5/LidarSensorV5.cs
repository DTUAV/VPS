using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV5
{

    public class LidarSensorV5 : MonoBehaviour
    {
        [Header("线束数量")] public uint LaserLineNumber = 1;

        [Header("激光点数")] public long LaserPointNumber = 1200;

        [Header("一周视野角度")] public float LaserAngle = 360;

        [Header("FOV")] public float LaserFOV = 0;

        [Header("水平方向角度增量：angle_inc")] public float LaserAngleInc;

        [Header("可探测最大距离")] public float RangMax = 10;

        [Header("可探测最小距离")] public float RangMin = 0.01f;

        [Header("起始角度")] public float AngleMin = 0;

        [Header("结束角度")] public float AngleMax = 360;

        [Header("探测到的距离")]
        public List<float> Distance = new List<float>();
        public List<int> Distension = new List<int>();
        public List<Vector3> hitDataPosition = new List<Vector3>();//射线终点位置
        [Header("是否显示激光雷达")] public bool ShowLidar = false;

        [Header("激光雷达对象")] public GameObject LidarObject;

        [Header("激光雷达当前位置")] public Vector3 LidarPosition;

        private List<Ray> Rays = new List<Ray>();
        
        // Start is called before the first frame update
        private float AngleIncA;
        public bool IsUpdateLidar = true;
        void LaserInit()
        {
            AngleIncA = LaserAngle / LaserPointNumber;//计算每个激光线束的角度（单位：度）
            //Debug.Log(AngleIncA);
            LidarPosition = LidarObject.transform.position;//获取激光雷达对象的位置
            LaserAngleInc = AngleIncA * (Mathf.PI / 180);//计算每个激光线束的角度（单位：弧度）
        }
        void LaserUpdate()
        {
            LaserInit();
            Distance.Clear();//每一次更新将内存清空，避免内存泄露
            Distension.Clear();
            hitDataPosition.Clear();
            Rays.Clear();
            Distension = new List<int>();
            Distance = new List<float>();
            Rays = new List<Ray>();//释放之前的内存，开辟新内存

            float yaw = LidarObject.transform.eulerAngles.y;//获取激光雷达当前朝向
            float FovAngleInc;//多线束激光雷达垂直方向的角度
            if (LaserLineNumber > 1)//判断线束数量
            {
                FovAngleInc = LaserFOV / (LaserLineNumber - 1);
            }
            else
            {
                FovAngleInc = 0;
            }

            for (int j = 0; j < LaserLineNumber; j++)//更新每个线束
            {
                float verticalAngle;
                if (LaserLineNumber == 1)
                {
                    verticalAngle = 0;
                }
                else
                {
                    verticalAngle = LaserFOV / 2 - j * FovAngleInc;
                }
                 
                verticalAngle = verticalAngle * (Mathf.PI / 180);
                for (int i = 0; i < LaserPointNumber; i++)
                {
                    float angle = AngleIncA * i + yaw;
                    if (angle > 360)
                    {
                        angle = angle - 360;
                    }

                    angle = angle * (Mathf.PI / 180);
                    Vector3 dir = new Vector3(Mathf.Cos(angle), verticalAngle, Mathf.Sin(angle)).normalized;
                    Rays.Add(new Ray(LidarPosition, dir));//每个线束一周的所有激光点
                }

                foreach (Ray ray in Rays)
                {
                    RaycastHit hit = new RaycastHit();
                    int mask = LayerMask.GetMask("Obstacle");
                    bool ishit = Physics.Raycast(ray, out hit, RangMax, mask);
                    if (ishit)
                    {
                        if (hit.distance < RangMin)
                        {
                            Distance.Add(RangMin);
                            //Distension.Add((int)(255 * RangMin / (RangMax - RangMin)));
                            Distension.Add(0);
                        }
                        else
                        {
                            Distance.Add(hit.distance);
                            Distension.Add((int)(255 * hit.distance / (RangMax - RangMin)));
                        }


                        if (ShowLidar)
                        {
                            Debug.DrawLine(LidarPosition, hit.point, Color.red);
                          //  Debug.Log("dddeeeffff");
                        }

                        hitDataPosition.Add(hit.point);
                        
                        
                    }
                    else
                    {
                        if (ShowLidar)
                        {
                            Debug.DrawLine(LidarPosition, new Vector3(LidarPosition.x + RangMax * ray.direction.x, LidarPosition.y + RangMax * ray.direction.y, LidarPosition.z + RangMax * ray.direction.z), Color.blue);
                        }
                        Distance.Add(RangMax);
                        //Distension.Add((int) (255 * RangMax / (RangMax - RangMin)));
                        Distension.Add(255);
                        float dx = ray.direction.x / (Mathf.Sqrt(ray.direction.x * ray.direction.x + ray.direction.y * ray.direction.y + ray.direction.z * ray.direction.z));
                        float dy = ray.direction.y / (Mathf.Sqrt(ray.direction.x * ray.direction.x + ray.direction.y * ray.direction.y + ray.direction.z * ray.direction.z));
                        float dz = ray.direction.z / (Mathf.Sqrt(ray.direction.x * ray.direction.x + ray.direction.y * ray.direction.y + ray.direction.z * ray.direction.z));
                        hitDataPosition.Add(new Vector3(ray.origin.x + RangMax * dx, ray.origin.y + RangMax * dy, ray.origin.z + RangMax * dz));

                    }
                }
            }
        }

        void Start()
        {
            LaserUpdate();
        }

        void FixedUpdate()
        {
            if (IsUpdateLidar)
            {
                LaserUpdate();
                IsUpdateLidar = false;
            }
           
        }
    }
}
