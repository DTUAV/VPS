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

[Serializable] // 指示可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
public struct Operator1
{
    public double uav_position_x;
    public double uav_position_y;
    public double uav_position_z;
    public double uav_angle_pitch;
    public double uav_angle_roll;
    public double uav_angle_yaw;

}

[Serializable] // 指示可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
public struct  SendPositionStruct
{
   public  double positionX;
   public  double positionY;
   public  double positionZ;
   public  double uav_position_x;
   public  double uav_position_y;
   public double uav_position_z;
}





public class connect_ros : MonoBehaviour
{
    protected byte[] Struct2Bytes<T>(T obj)
    {
        int size = Marshal.SizeOf(obj);
        byte[] bytes = new byte[size];
        IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
        Marshal.StructureToPtr(obj, arrPtr, true);
        return bytes;
    }

    protected T Bytes2Struct<T>(byte[] bytes)
    {
        IntPtr arrPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
        return (T)Marshal.PtrToStructure(arrPtr, typeof(T));
    }






    public object BytesToStruct(byte[] bytes, Type type)
    {
        //得到结构的大小
        int size = Marshal.SizeOf(type);
        // Log(size.ToString(), 1);
        //byte数组长度小于结构的大小
        if (size > bytes.Length)
        {
            //返回空
            return null;
        }
        //分配结构大小的内存空间
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        //将byte数组拷到分配好的内存空间
        Marshal.Copy(bytes, 0, structPtr, size);
        //将内存空间转换为目标结构
        object obj = Marshal.PtrToStructure(structPtr, type);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        //返回结构
        return obj;
    }

    /// 将结构转换为字节数组
    /// 结构对象
    /// 字节数组
    public byte[] StructToBytes(object obj)
    {
        //得到结构体的大小
        int size = Marshal.SizeOf(obj);
        //创建byte数组
        byte[] bytes = new byte[size];
        //分配结构体大小的内存空间
        IntPtr structPtr = Marshal.AllocHGlobal(size);
        //将结构体拷到分配好的内存空间
        Marshal.StructureToPtr(obj, structPtr, false);
        //从内存空间拷到byte数组
        Marshal.Copy(structPtr, bytes, 0, size);
        //释放内存空间
        Marshal.FreeHGlobal(structPtr);
        //返回byte数组
        return bytes;
    }






    public static Mutex mutex = new Mutex();//互斥锁




    Socket socketSend;
    static public bool isStop = true;
    static public bool isStart = false;
    static public bool isConnect = false;
    static public bool endReceived = false;
    static public bool endSend = false;
    //开启新的线程，启动socket
    Thread socket_thread;
    Thread c_thread;//接收信息的线程
    Thread send_thread;//发送信息的线程

    SendPositionStruct positionStruct;



    private void bt_connect_Click()
    {
        while (!isConnect)
        {
            if (!isStop && isStart)
            {
                try
                {
                    int _port = 8080;
                    string _ip = "192.168.152.128";
                    //创建客户端Socket，获得远程ip和端口号
                    socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPAddress ip = IPAddress.Parse(_ip);
                    IPEndPoint point = new IPEndPoint(ip, _port);

                    socketSend.Connect(point);
                    Debug.Log("连接成功!");
                    isConnect = true;
                    endReceived = false;
                    endSend = false;
                    c_thread.Start();
                    send_thread.Start();

                }
                catch (Exception)
                {
                    Debug.Log("IP或者端口号错误...重新连接");

                }
            }
            else
            {
                Debug.Log("按下开始键");
            }
        }

    }

    /// <summary>
    /// 接收服务端返回的消息
    /// </summary>
    void Received()
    {
        while (!isStop && !endReceived)
        {
            try
            {
                byte[] buffer = new byte[1024];
                //实际接收到的有效字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break;
                }

                // Operator MyOper = new Operator();
                // MyOper = (Operator)BytesToStruct(buffer, MyOper.GetType()); // 将字节数组转换成结构

                Operator1 MyOper = Bytes2Struct<Operator1>(buffer);


                Debug.Log("position_x: " + MyOper.uav_position_x);
                Debug.Log("position_y: " + MyOper.uav_position_y);
                Debug.Log("position_z: " + MyOper.uav_position_z);
                Debug.Log("angle_pitch: " + MyOper.uav_angle_pitch);
                Debug.Log("angle_roll: " + MyOper.uav_angle_roll);
                Debug.Log("angle_yaw: " + MyOper.uav_angle_yaw);

                uav.uav_position_x = (float)MyOper.uav_position_x;
                uav.uav_position_y = (float)MyOper.uav_position_y;
                uav.uav_position_z = (float)MyOper.uav_position_z;
                uav.uav_angle_pitch = (float)MyOper.uav_angle_pitch;
                uav.uav_angle_roll = (float)MyOper.uav_angle_roll;
                uav.uav_angle_yaw = (float)MyOper.uav_angle_yaw;

                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("客户端打印：" + socketSend.RemoteEndPoint + ":" + str);
            }
            catch
            {

                if (!isStop && isStart)
                {
                    socket_thread.Start();
                    isConnect = false;
                    endReceived = true;
                   
                }
            }
        }
    }

    void SendToSocket()
    {
       // Debug.Log("fffffffffffffffffff");
      while (!endSend)
        {
            
            if(LidarLaserSensor.isSend)
            {
                /*Vector3 a = new Vector3();
                a.x = 1;
                a.y = 2;
                a.z = 3;
                Send(a);
                Debug.Log("fffffffffffffffffff");
                */
                Send(LidarLaserSensor.sendHisPosition);
                mutex.WaitOne();
                LidarLaserSensor.isSend = false;
                mutex.ReleaseMutex();
               
            }

        }


    }



    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
     void Send(Vector3 hitPosition)
    {
        try
        {
            positionStruct.positionX = hitPosition.x;
            positionStruct.positionY = hitPosition.y;
            positionStruct.positionZ = hitPosition.z;
            positionStruct.uav_position_x = uav_ancient_village.uav_position_x;
            positionStruct.uav_position_y = uav_ancient_village.uav_position_y;
            positionStruct.uav_position_z = uav_ancient_village.uav_position_z;


            Debug.Log("hitPositionX " + hitPosition.x);
            // string msg = str;
            byte[] buffer = StructToBytes(positionStruct);
           // buffer = Encoding.UTF8.GetBytes(msg);
            socketSend.Send(buffer);
        }
        catch {

            Debug.Log("发送失败");

        }
    }

    void start_socket()
    {
        bt_connect_Click();
    }


    // Start is called before the first frame update
    void Start()
    {
        socket_thread = new Thread(start_socket);
        socket_thread.IsBackground = true;

        //开启新的线程，不停的接收服务器发来的消息
        c_thread = new Thread(Received);
        c_thread.IsBackground = true;

        //开启新的线程，不停的向服务器发送消息
      
        send_thread = new Thread(SendToSocket);
        send_thread.IsBackground = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
        if (GUILayout.Button("开始"))
        {
            isStart = true;
            isStop = false;
            endReceived = false;
            endSend = false;
            if (!socket_thread.IsAlive)
            {
                socket_thread.Start();
            }
        }

        if (GUILayout.Button("停止"))
        {
            isStop = true;
            isStart = false;
            endReceived = true;
            endSend = true;
            if (c_thread.IsAlive)
            {
                c_thread.Abort();
            }
            if (socket_thread.IsAlive)
            {
                socket_thread.Abort();
            }
            if(send_thread.IsAlive)
            {
                send_thread.Abort();
            }


        }
    }

    private void OnApplicationQuit()
    {
        isStop = true;
        isStart = false;
        endReceived = true;
        endSend = true;
        if (c_thread.IsAlive)
        {
            c_thread.Abort();
        }
        if (socket_thread.IsAlive)
        {
            socket_thread.Abort();
        }
        if(send_thread.IsAlive)
        {
            send_thread.Abort();
        }
    }
}
