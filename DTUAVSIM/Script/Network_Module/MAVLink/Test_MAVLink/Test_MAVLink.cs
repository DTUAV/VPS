using System.Collections;
using System.Collections.Generic;
using mavlinkWinformClient.MavClient;
using UnityEngine;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using MavLink;
using System.Net.Sockets;
using System;

public class Test_MAVLink : MonoBehaviour
{
    public string IP = "192.168.1.1";

    public int Port = 5000;

    private  Mavclient mavclient = new Mavclient();

    private Thread mavlinkRxThread;//接受线程
    private Thread mavlinkSendThread;//发送线程
    private bool IsStop = false;
    private void commStatusHandler(bool comm)
    {
        if (comm)
        {
            
        }
    }

    private void commDataHandler(float dt)
    {
        
    }

    private void connect_click()
    {
        mavclient.connect(IP,Port);
        if (mavclient.commStatus)
        {
            Debug.Log("已连接");

            mavlinkRxThread = new Thread(mavclient.parseMavlink);//这个线程解析包
            mavlinkRxThread.Start();
            Debug.Log("mavlinkRxThread: "+mavlinkRxThread.ManagedThreadId.ToString());

            mavlinkSendThread = new Thread(sendMsg);
            mavlinkSendThread.Start();
            Debug.Log("mavlinkSendThread: " + mavlinkSendThread.ManagedThreadId.ToString());
        }
    }

    private void send_imu(NetworkStream stream)
    {
        MavlinkPacket mavPack = new MavlinkPacket();
        Msg_raw_imu mht = new Msg_raw_imu();
        Mavlink mav = new Mavlink();
        try
        {
            mht.time_usec = 10000;
            mht.xacc = 1;
            mht.xgyro = 2;
            mht.xmag = 2;
            mht.yacc = 2;
            mht.ygyro = 3;
            mht.ymag = 2;
            mht.zacc = 3;
            mht.zgyro = 2;
            mht.zmag = 23;
           
            mavPack.ComponentId = 1;
            mavPack.SystemId = 1;
            mavPack.SequenceNumber = 0;
            mavPack.TimeStamp = DateTime.Now;
            mavPack.Message = mht;

            byte[] buffer = mav.Send(mavPack);

            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("Write: " + Thread.CurrentThread.ManagedThreadId.ToString());
        }
        catch (Exception ee)
        {
            Debug.Log(ee.Message);
        }
    }

    private void send_heartbeat(NetworkStream stream) 
    {
        MavlinkPacket mavPack = new MavlinkPacket();
        Msg_heartbeat mht = new Msg_heartbeat();
        Mavlink mav = new Mavlink();
        try
        {
            /* mavlink encode */
            mht.mavlink_version = 1;
            mht.custom_mode = (uint)6;
            mht.base_mode =1;

            mht.system_status = 4;

            mavPack.ComponentId = 1;
            mavPack.SystemId = 1;
            mavPack.SequenceNumber = 0;
            mavPack.TimeStamp = DateTime.Now;
            mavPack.Message = mht;

            byte[] buffer = mav.Send(mavPack);

            stream.Write(buffer, 0, buffer.Length);
            Debug.Log("Write: " + Thread.CurrentThread.ManagedThreadId.ToString());
            
        }
        catch (Exception ee)
        {
            Debug.Log(ee.Message);
        }

    }
    private void sendMsg()
    {
        while (!IsStop)
        {
          //  mavclient.sendMavMsg(5, 6, 1);
          mavclient.sendData();
        }
       
    }

    
    private void stopMavlink()
    {
        IsStop = true;
        mavclient.close();
        if (mavlinkRxThread != null)
        {
            if (mavlinkRxThread.IsAlive)
            {
                mavlinkRxThread.Abort();
            }
        }
        if (mavlinkSendThread != null)
        {
            if (mavlinkSendThread.IsAlive)
            {
                mavlinkSendThread.Abort();
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        mavclient.commStatusEvent += commStatusHandler;
        mavclient.commDataEvent += commDataHandler;
        //mavclient.sendDataEvent += send_heartbeat;
        mavclient.sendDataEvent += send_imu;
        connect_click();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        
    }

    void OnDestroy()
    {
        stopMavlink();
    }
}
