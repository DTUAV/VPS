using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using DTUAVCARS.DTNetWork.SocketNetwork;
using SimUnity.Network.msgs.sensor_msgs;
using SimUnity.Noise;
using SimUnity.Data_bridge;
using SimUnity.droneV2;

namespace SimUnity.droneV3
{

    [StructLayout(LayoutKind.Sequential)]
    public struct UavOdometryStruct
    {

        /// double
        public double position_x;

        /// double
        public double position_y;

        /// double
        public double position_z;

        /// double
        public double orientation_x;

        /// double
        public double orientation_y;

        /// double
        public double orientation_z;

        /// double
        public double orientation_w;

        /// double[36]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = UnmanagedType.R8)]
        public double[] pose_covariance;

        /// double
        public double linear_x;

        /// double
        public double linear_y;

        /// double
        public double linear_z;

        /// double
        public double angular_x;

        /// double
        public double angular_y;

        /// double
        public double angular_z;

        /// double[36]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36, ArraySubType = UnmanagedType.R8)]
        public double[] twist_covariance;
    }




    public class DroneV3Odomentry : MonoBehaviour
    {
        public string IP = "192.168.116.128";
        public DroneV2State getUavState;
        public UavOdometryStruct uavOdometryStruct;
        public int Port = 8006;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private Thread c_getData;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private bool sendFlag = false;
        private Vector3 last_linear_velocity;

        public void UavOdometryStructInit()
        {
            uavOdometryStruct.pose_covariance = new double[36];
            uavOdometryStruct.twist_covariance = new double[36];
        }
        private void connect()
        {
            clientBase = new SocketClientBase(IP, Port);
            socketSend = clientBase.ConnectServer();
            if (socketSend == null)
            {
                Debug.Log("连接失败");
            }
            else
            {
                Debug.Log("连接成功!");
                endFlag = false;
                //开启新的线程，不停的接收服务器发来的消息
                c_thread = new Thread(Send);
                c_thread.IsBackground = true;
                c_thread.Start();
            }
        }

        void getOdometryData()
        {
            while (!endFlag)
            {
                if (getUavState.getOdometryDataFlag)
                {
                    //未转换
                    /*
                    uavOdometryStruct.position_x = getUavState.posePosition.x  + GaussNoisPlugin.GaussianNoiseData(0, 0.1);
                    uavOdometryStruct.position_y = getUavState.posePosition.y  + GaussNoisPlugin.GaussianNoiseData(0, 0.1);
                    uavOdometryStruct.position_z = getUavState.posePosition.z  + GaussNoisPlugin.GaussianNoiseData(0, 0.1);
                    uavOdometryStruct.orientation_x = getUavState.quaternion.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.orientation_y = getUavState.quaternion.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.orientation_z = getUavState.quaternion.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.orientation_w = getUavState.quaternion.w + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.linear_x = getUavState.linear_velocity.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.linear_y = getUavState.linear_velocity.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.linear_z = getUavState.linear_velocity.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.angular_x = getUavState.angular_velocity.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.angular_y = getUavState.angular_velocity.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavOdometryStruct.angular_z = getUavState.angular_velocity.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    */
                    //转换到ros
                    Vector3 ros_position = UnityToRos.Unity2Ros(getUavState.posePosition);
                    Quaternion ros_rotation = UnityToRos.Unity2Ros(getUavState.quaternion);
                    Vector3 ros_linear = UnityToRos.Unity2Ros(getUavState.linear_velocity);
                    Vector3 ros_angular = UnityToRos.Unity2Ros(getUavState.angular_velocity);
                    uavOdometryStruct.position_x = ros_position.x;
                    uavOdometryStruct.position_y = ros_position.y;
                    uavOdometryStruct.position_z = ros_position.z;
                    uavOdometryStruct.orientation_x = 0;
                    uavOdometryStruct.orientation_y = 0;
                    uavOdometryStruct.orientation_z = ros_rotation.z;
                    uavOdometryStruct.orientation_w = ros_rotation.w;
                    uavOdometryStruct.linear_x = ros_linear.x;
                    uavOdometryStruct.linear_y = ros_linear.y;
                    uavOdometryStruct.linear_z = ros_linear.z;
                    uavOdometryStruct.angular_x = ros_angular.x;
                    uavOdometryStruct.angular_y = ros_angular.y;
                    uavOdometryStruct.angular_z = ros_angular.z;







                    getUavState.getOdometryDataFlag = false;
                    sendFlag = true;
                }

            }



        }




        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        void Send()
        {
            while (!endFlag)
            {
                if (sendFlag)
                {
                    try
                    {
                        byte[] buffer = strb.StructToBytes(uavOdometryStruct);
                        socketSend.Send(buffer);
                        sendFlag = false;
                        // Debug.Log(sendpoint.values[1]);
                        //Debug.Log(buffer[1]);
                    }
                    catch
                    {
                        Debug.Log("发送错误");
                    }
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

            UavOdometryStructInit();
            connect();
            c_getData = new Thread(getOdometryData);
            c_getData.IsBackground = true;
            c_getData.Start();

        }

        // Update is called once per frame
        void Update()
        {
            if (isStartAgain)
            {
                endFlag = true;
                socketSend.Close();
                if (c_thread.IsAlive)
                {
                    c_thread.Abort();
                }
                if (c_getData.IsAlive)
                {
                    c_getData.Abort();
                }
                Start();
                isStartAgain = false;
            }
        }

        void OnGUI()
        {

        }

        void OnDestroy()
        {
            endFlag = true;
            if (socketSend.IsBound)
            {
                socketSend.Close();
            }
            if (c_thread.IsAlive)
            {
                c_thread.Abort();
            }
            if (c_getData.IsAlive)
            {
                c_getData.Abort();
            }

        }

    }
}
