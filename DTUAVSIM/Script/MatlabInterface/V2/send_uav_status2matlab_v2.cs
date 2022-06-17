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
using SimUnity.droneV2;

namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct UavState2_v2
    {
        public double position_x;
        public double position_y;
        public double position_z;

        public double roll;
        public double yaw;
        public double pitch;

        public double velocity;
    }



    public class send_uav_status2matlab_v2 : MonoBehaviour
    {
        public UavState2_v2 uavState;
        public string IP = "192.168.116.128";
        public int Port = 8009;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        [Header("是否使用动态初始化IP")]
        public bool isInitIpD = false;
        public Rigidbody getUavState;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private bool sendFlag = false;


        private void connect()
        {
            if (isInitIpD == true)
            {
                IP = PlayerPrefs.GetString("IP");
            }

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
                        byte[] buffer = strb.StructToBytes(uavState);
                        socketSend.Send(buffer);
                        //Debug.Log("position_y"+uavState.position_y);
                        //Debug.Log("buff[1]"+buffer[1]);
                        sendFlag = false;
                        // Debug.Log(sendpoint.values[1]);
                        //Debug.Log(buffer[1]);
                    }
                    catch
                    {
                        Debug.Log("发送错误");
                    }
                }
                Thread.Sleep(400);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            connect();

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
                Start();
                isStartAgain = false;
            }

            uavState.position_x = getUavState.position.x;
            uavState.position_y = getUavState.position.y;
            uavState.position_z = getUavState.position.z;
            //Debug.Log(uavState.position_z);
            uavState.roll = getUavState.rotation.eulerAngles.x;
            uavState.pitch = getUavState.rotation.eulerAngles.z;
            uavState.yaw = getUavState.rotation.eulerAngles.y;
            uavState.velocity = getUavState.velocity.magnitude;
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

        }
    }
}
