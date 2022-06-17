using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.SensorData
{
   public struct LidarSensorDataStruct
    {
        public string LidarName;//激光雷达传感器的名字      
        public int LidarNumber;//当前激光雷达的线束
        public int TimePerRotation;//激光雷达传感器一圈旋转的次数
        public float MinDistance;//激光雷达传感器测量的最小距离
        public float MaxDistance;//激光雷达传感器测量的最大距离
        public int MeasurementsPerRotation;//激光雷达传感器每圈每个线束测量的点数
        public float HorizontalAngleAll;//水平旋转总角度
        public float FOV;//垂直方向的总角度
        public float CenterAngle;//激光雷达传感器中心角度
        public float CurrentVerticalAngle;//当前线束的垂直角度
        public float CurrentHorizontalAngle;//当前线束的水平角度
        public float HorizontalAnglePer;//水平方向旋转步进角度
        

    };
    public struct LidarSensorDataKeep
    {
        public int LidarID;//当前激光雷达的线束编号
        public float VerticalAngel;//垂直方向角度
        public List<float> HorizontalAngle;//水平方向角度
        public List<double> distant;//距离
        public List<double> density;//密度
    }
    
}
