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
using DTUAVCARS.DTNetWork.SocketNetwork;

namespace SimUnity.droneV3
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct UavState
    {
        public double position_x;
        public double position_y;
        public double position_z;
        public double rotation_x;
        public double rotation_y;
        public double rotation_z;
        public double rotation_w;
    }



    public class DroneV3UavState : MonoBehaviour
    {
        public UavState uavState;
        public string IP = "192.168.116.128";
        public int Port = 8009;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        public DroneV2State getUavState;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private Thread c_getData;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private bool sendFlag = false;


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

        void getState()
        {
            while (!endFlag)
            {
                if (getUavState.getUavStateDataFlag)
                {
                    uavState.position_x = getUavState.posePosition.x;
                    uavState.position_y = getUavState.posePosition.y;
                    uavState.position_z = getUavState.posePosition.z;

                    uavState.rotation_x = getUavState.quaternion.x;
                    uavState.rotation_y = getUavState.quaternion.y;
                    uavState.rotation_z = getUavState.quaternion.z;
                    uavState.rotation_w = getUavState.quaternion.w;
                    getUavState.getUavStateDataFlag = false;
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
                        byte[] buffer = strb.StructToBytes(uavState);
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


            connect();
            c_getData = new Thread(getState);
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
