using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MavLink;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;
using System.Net;

public class MavlinkClientUDP: MonoBehaviour
{
    public string ServerIP;
    public int ServerPort;

    public string ClientIP;
    public int ClientPort;
    private Socket _client;
    private Thread _sendThread;
    private Thread _recvThread;

    private bool _isStop;

    private NetworkStream _stream;
    private Mavlink _mav;
    private MavlinkPacket _mavPack;
    private Msg_heartbeat _mht;

    public delegate void sendDataDelegate();
    public event sendDataDelegate sendDataEvent;//用于发送消息

    public event commStatusDelegate commStatusEvent;
    public delegate void commStatusDelegate(bool comm);

    public event commDataDelegate commDataEvent;
    public delegate void commDataDelegate(float dt);
    public void Start()
    {
        _isStop = false;
        _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _client.Bind(new IPEndPoint(IPAddress.Parse(ClientIP), ClientPort));
        _recvThread = new Thread(RecvMessage);
        _mav = new Mavlink();
        _mavPack = new MavlinkPacket();
        _mht = new Msg_heartbeat();
        _mav.PacketReceived += recvMavMsg;

    }

    private void SendMessage(byte[] data)
    {
        EndPoint point = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
        _client.SendTo(data, point);
    }

    private void RecvMessage()
    {
        try
        {
            while (!_isStop)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0); //用来保存发送方的ip和端口号
                byte[] buffer = new byte[1024];
                int length = _client.ReceiveFrom(buffer, ref point); //接收数据报
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Debug.Log("recv_message: " + message);
                if (_stream.CanRead) //新增yyl
                {
                    if (length > 0)
                    {
                        //parse mavlink
                        _mav.ParseBytes(buffer);
                        Debug.Log("ParseBytes: " + Thread.CurrentThread.ManagedThreadId.ToString());
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }


    public void recvMavMsg(object sender, MavlinkPacket e)
    {
        Debug.Log("recvMavMsg: " + Thread.CurrentThread.ManagedThreadId.ToString());


        string str = e.Message.ToString();

        if (str == "MavLink.Msg_heartbeat")
        {
            /* convert */
            Msg_heartbeat ht = (Msg_heartbeat) e.Message;
            //  if (MsgHeartBeatEvent != null)
            //  {
            //      MsgHeartBeatEvent(ht);
            //  }
            Debug.Log("Msg_heartbeat_heart" + ht.system_status);
            /* save the incoming msg */
            //   mode = ht.custom_mode;

            /* publish */
            //    commDataEvent(mode);

        }
        else if (str == "MavLink.Msg_sys_status")
        {
            Msg_sys_status syst = (Msg_sys_status) e.Message;
            //   if (MsgSysStatusEvent != null)
            ///   {
            //       MsgSysStatusEvent(syst);
            //  }
            Debug.Log("Msg_sys_status_current_battery" + syst.current_battery);
        }
    }
    public void sendData()
    {
        if (sendDataEvent != null)
        {
            sendDataEvent();
        }

    }
    public void Update()
    {
        MavlinkPacket mavPack = new MavlinkPacket();
        
        Msg_heartbeat mht = new Msg_heartbeat();
        Mavlink mav = new Mavlink();
       // try
        {
            /* mavlink encode */
            mht.mavlink_version = 1;
            mht.custom_mode = (uint)6;
            mht.base_mode = 1;

            mht.system_status = 4;

            mavPack.ComponentId = 1;
            mavPack.SystemId = 1;
            mavPack.SequenceNumber = 0;
            mavPack.TimeStamp = DateTime.Now;
            mavPack.Message = mht;

            byte[] buffer = mav.Send(mavPack);

            SendMessage(buffer);
            Debug.Log(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            Debug.Log("Write: " + Thread.CurrentThread.ManagedThreadId.ToString());

        }
      //  catch (Exception ee)
       // {
      //      Debug.Log(ee.Message);
     //   }
    }
    public void sendMavMsg(byte cmd, byte addr, float pos)
    {
        try
        {
            /* mavlink encode */
            _mht.mavlink_version = cmd;
            _mht.custom_mode = (uint)pos;
            _mht.base_mode = addr;

            _mht.system_status = 4;

            _mavPack.ComponentId = 1;
            _mavPack.SystemId = 1;
            _mavPack.SequenceNumber = 0;
            _mavPack.TimeStamp = DateTime.Now;
            _mavPack.Message = _mht;

            byte[] buffer = _mav.Send(_mavPack);
            SendMessage(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            Debug.Log("Write: " + Thread.CurrentThread.ManagedThreadId.ToString());
        }
        catch (Exception ee)
        {
            Debug.Log(ee.Message);
        }
    }


}
