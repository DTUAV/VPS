
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using SimUnity.Sensor.LidarSensorV5;

namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct point
    {

        /// double[1000]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.R8)]
        public double[] x;

        /// double[1000]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.R8)]
        public double[] y;

        /// double[1000]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.R8)]
        public double[] z;

        /// double[1000]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000, ArraySubType = UnmanagedType.R8)]
        public double[] values;



    }


    public class send_point_cloud2matlab : MonoBehaviour
    {
        public point sendpoint;
        public LidarSensorV5 lidarSensorV5;
        public string IP = "192.168.152.12";
        public int Port = 8002;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        //public GetUavState getUavState;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private Thread c_getData;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private bool sendFlag = false;

        public void sendpoint_init()
        {
            sendpoint.x = new double[1000];
            sendpoint.y = new double[1000];
            sendpoint.z = new double[1000];
            sendpoint.values = new double[1000];

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

        void getPointCloudData()
        {
            while (!endFlag)
            {
                if (!lidarSensorV5.IsUpdateLidar)
                {
                    int i = 0;

                    foreach (UnityEngine.Vector3 d in lidarSensorV5.hitDataPosition)
                    {

                        sendpoint.x[i] = d.z;
                        sendpoint.y[i] = -d.x;
                        sendpoint.z[i] = d.y;

                        i++;

                    }


                    i = 0;

                    foreach (float den in lidarSensorV5.Distension)
                    {
                        sendpoint.values[i] = den;
                        i++;

                    }
                    Debug.Log("ddddddddddddddddddd");
                    sendFlag = true;
                    lidarSensorV5.IsUpdateLidar = true;
                }
               // Thread.Sleep(40);
            }

        }


        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        void Send()
        {
            while (!endFlag)
            {
                Debug.Log("jjjjjjjjjjj");
                if (sendFlag)
                {
                    Debug.Log("eeeeeeeeeeee");
                    try
                    {
                        byte[] buffer = strb.StructToBytes(sendpoint);
                        socketSend.Send(buffer);
                        sendFlag = false;
                        Debug.Log("ffffffffffffffffff");
                        Debug.Log(sendpoint.values[1]);
                        //Debug.Log(buffer[1]);
                    }
                    catch
                    {
                        Debug.Log("发送错误");
                    }
                }
                //Thread.Sleep(40);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            sendpoint_init();

            connect();
            c_getData = new Thread(getPointCloudData);
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
