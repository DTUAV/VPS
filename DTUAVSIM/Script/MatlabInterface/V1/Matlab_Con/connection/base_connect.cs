
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using SimUnity.Sensor;
using System.Text;

namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct test
    {

        /// double[2]
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.R8)]
        public double[] a;

        /// double
        public double b;

        /// double
        public double c;
    }


    public class base_connect : MonoBehaviour
    {
        public string IP = "192.168.116.128";
        public int Port = 7000;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private Thread r_thread;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;

        public test data;
        
        void data_init()
        {
            data.a = new double[2];
            data.a[0] = 1;
            data.a[1] = 2;
            data.b = 3;
            data.c = 4;
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

                r_thread = new Thread(recevied);
                r_thread.IsBackground = true;
                r_thread.Start();
            }
        }



        void recevied()
        {
            while (!endFlag)
            {
                Debug.Log("ddd");
                try
                {
                    byte[] buffer = new byte[1024];
                    //实际接收到的有效字节数
                    int len = socketSend.Receive(buffer);
                    if (len <= 15)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, len);
                    Debug.Log("客户端打印：" + socketSend.RemoteEndPoint + ":" + str);
                }
                catch
                {


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
                if (true)
                {
                    try
                    {
                        byte[] buffer = strb.StructToBytes(data);
                        //byte[] buffer = data;
                        int a = buffer.Length;

                         socketSend.Send(buffer);
                       
                      // for(int i = 0;i<a;i++)
                      //  {
                          //  Debug.Log(buffer[i]);
                     //   }
                       
                        //  Debug.Log(buffer[1]);
                      
                      
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
            data_init();

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
