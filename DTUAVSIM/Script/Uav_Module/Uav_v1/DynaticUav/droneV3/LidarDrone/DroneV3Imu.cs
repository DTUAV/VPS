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
using SimUnity.droneV2;

namespace SimUnity.droneV3
{

    [StructLayout(LayoutKind.Sequential)]
    public struct UavImuStruct
    {

        /// double
        public double orientation_x;

        /// double
        public double orientation_y;

        /// double
        public double orientation_z;

        /// double
        public double orientation_w;

        /// double[9]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.R8)]
        public double[] orientation_covariance;

        /// double
        public double angular_velocity_x;

        /// double
        public double angular_velocity_y;

        /// double
        public double angular_velocity_z;

        /// double[9]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.R8)]
        public double[] angular_velocity_covarience;

        /// double
        public double linear_acceleration_x;

        /// double
        public double linear_acceleration_y;

        /// double
        public double linear_acceleration_z;

        /// double[9]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.R8)]
        public double[] linear_acceleration_covarience;
    }

    public class DroneV3Imu : MonoBehaviour
    {
        public string IP = "192.168.116.128";
        public DroneV2State getUavState;
        public UavImuStruct uavImuStruct;
        public int Port = 8005;
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

        public void UavImuStructInit()
        {
            uavImuStruct.orientation_covariance = new double[9];
            uavImuStruct.angular_velocity_covarience = new double[9];
            uavImuStruct.linear_acceleration_covarience = new double[9];
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

        void getImuData()
        {
            while (!endFlag)
            {
                if (getUavState.getImuDataFlag)
                {
                    uavImuStruct.linear_acceleration_x = getUavState.linear_velocity.x - last_linear_velocity.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.linear_acceleration_y = getUavState.linear_velocity.y - last_linear_velocity.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.linear_acceleration_z = getUavState.linear_velocity.z - last_linear_velocity.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    last_linear_velocity = getUavState.linear_velocity;
                    uavImuStruct.angular_velocity_x = getUavState.angular_velocity.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.angular_velocity_y = getUavState.angular_velocity.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.angular_velocity_z = getUavState.angular_velocity.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.orientation_x = getUavState.quaternion.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.orientation_y = getUavState.quaternion.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.orientation_z = getUavState.quaternion.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    uavImuStruct.orientation_w = getUavState.quaternion.w + GaussNoisPlugin.GaussianNoiseData(0, 0.01);



                    getUavState.getImuDataFlag = false;
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
                        byte[] buffer = strb.StructToBytes(uavImuStruct);
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

            UavImuStructInit();
            connect();
            c_getData = new Thread(getImuData);
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
