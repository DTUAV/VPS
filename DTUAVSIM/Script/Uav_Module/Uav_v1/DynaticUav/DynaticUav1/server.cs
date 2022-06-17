using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class server : MonoBehaviour {

    int port_net = 0;//网络端口
    string ip_net = "";//网络的IP
    Socket socketSend_net;//发送
    void send_Received_net(string  str)//接收到消息回复
    {
        byte[] buffer = Encoding.UTF8.GetBytes(str);
        socketSend_net.Send(buffer);
    }

    //等待客户端的连接 并且创建与之通信的Socket

    void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                socketSend_net = socketWatch.Accept();//等待接收客户端连接
                Debug.Log(socketSend_net.RemoteEndPoint.ToString() + ":" + "连接成功!");
                //开启一个新线程，执行接收消息方法
                Thread r_thread = new Thread(Received_net);
                r_thread.IsBackground = true;
                r_thread.Start(socketSend_net);
            }
        }
        catch { }
    }


    void Received_net(object o)//服务器不停接收客户端发来的消息
    {
        try
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                //客户端连接服务器成功后，服务器接收客户端发送的消息
                byte[] buffer = new byte[1024 * 1024 * 3];
                //实际接收到的有效字节数
                int len = socketSend.Receive(buffer);
                if (len == 0)
                {
                    break;
                }
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log("服务器打印：" + socketSend.RemoteEndPoint + ":" + str);
                send_Received_net("我收到了");
            }
        }
        catch { }
    }
   
    void startServer()
    {
        try
        {
            
            //点击开始监听时 在服务端创建一个负责监听IP和端口号的Socket
            Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ip_net);
            //创建对象端口
            IPEndPoint point = new IPEndPoint(ip, port_net);

            socketWatch.Bind(point);//绑定端口号
            Debug.Log("监听成功!");
            socketWatch.Listen(10);//设置监听，最大同时连接10台

            //创建监听线程
            Thread thread = new Thread(Listen);
            thread.IsBackground = true;
            thread.Start(socketWatch);
        }
        catch { }

    }

    // Use this for initialization
    void Start () {

        port_net = 8000;
        ip_net = "192.168.0.1";
        startServer();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
