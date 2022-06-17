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
namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    public class SocketClientBase
    {
        private string IP;
        private int Port;
        public SocketClientBase(string ip,int port)
        {
            this.IP = ip;
            this.Port = port;
        }

        public Socket ConnectServer()
        {
            Socket socketSend;
            try
            {
                int _port = Port;
                string _ip = IP;
                //创建客户端Socket，获得远程ip和端口号
                socketSend = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               // socketSend.Blocking = false;//非阻塞
                IPAddress ip = IPAddress.Parse(_ip);
                IPEndPoint point = new IPEndPoint(ip, _port);
                socketSend.Connect(point);
                Debug.Log("连接成功!");
                return socketSend;
            }
            catch (Exception)
            {
                Debug.Log("IP或者端口号错误...重新连接");
                return null;
            }
        }
    }
}
