using SimUnity.Sensor;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    public struct DataSizePNG
    {
        public double datasize;
    }
    public class send_camera_png2matlab : MonoBehaviour
    {
       
        public string IP = "192.168.116.128";
        public int Port = 8007;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        [Header("是否使用动态初始化IP")]
        public bool isInitIpD = false;
        public cameraDataRGB camera_data;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private DataSizePNG datasizepng;

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
                if (camera_data.sendImageDataFlag)
                {
                    try
                    {
                        byte[] buffer = camera_data.imageDataMatla;
                        datasizepng.datasize = buffer.Length;
                        byte[] databuff = strb.StructToBytes(datasizepng);
                        socketSend.Send(databuff);
                        //byte[] buffer = camera_data.cameraData.GetRawTextureData();
                        // byte[] buffer = camera_data.cameraData.EncodeToJPG();
                        //byte[] buffer = camera_data.imageData;
                        // buffer = camera_data.cameraData.EncodeToJPG();
                        //  Debug.Log(buffer[1]);
                        //Debug.Log("buffer " + buffer[1]);
                        socketSend.Send(buffer);
                        camera_data.getImageDataFlag = true;
                        camera_data.sendImageDataFlag = false;
                    }
                    catch
                    {
                        Debug.Log("发送错误");
                    }
                    Thread.Sleep(50);
                }
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
