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
using LCM.LCM;
using lcm_iot_msgs;
using DTUAVCARS.DTNetwok.Message;

namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    public class SocketClient : MonoBehaviour,LCM.LCM.LCMSubscriber
    {
        public string IP = "192.168.152.12";
        public int Port = 8080;
        [Header("是否重新开启")]
        public bool isStartAgain = false;
        public string IotMessageSubName;
        public string IotMessagePubName;
        public float DataRecvHz;
        private LCM.LCM.LCM PubLcm;
        private LCM.LCM.LCM SubLcm;
        private LcmIotMessage _lcmIotMessage;
        private IotMessage _iotMessage;
        private Socket socketSend;
        private SocketClientBase clientBase;
        private Thread c_thread;
        private StructBytes strb = new StructBytes();
        private bool endFlag = false;
        private int _recvTime;
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
                c_thread = new Thread(Received);
                c_thread.IsBackground = true;
                c_thread.Start();
            }
        }

        /// <summary>
        /// 接收服务端返回的消息
        /// </summary>
        void Received()
        {
            while (!endFlag)
            {
                try
                {
                    byte[] buf = new byte[2];
                    if (socketSend.Receive(buf,0,2,SocketFlags.None) == 2)
                    {
                        if (buf[0] == '\n' && buf[1] == '\n')
                        {
                            byte[] bufDataSize = new byte[Marshal.SizeOf(typeof(DataSize))];
                            if (socketSend.Receive(bufDataSize,0, Marshal.SizeOf(typeof(DataSize)),SocketFlags.None)== Marshal.SizeOf(typeof(DataSize)))
                            {
                                DataSize dataSize = strb.Bytes2Struct<DataSize>(bufDataSize);
                                long recvDataLen = SocketDataCommon.GetDataSize(dataSize);
                                byte[] bufRecvData = new byte[recvDataLen];
                                if (socketSend.Receive(bufRecvData,0,(int)recvDataLen,SocketFlags.None) == recvDataLen)
                                {
                                    IotMessage recvMessage = JsonUtility.FromJson<IotMessage>(Encoding.ASCII.GetString(bufRecvData));
                                    _lcmIotMessage.TimeStamp = recvMessage.TimeStamp;
                                    _lcmIotMessage.MessageData = recvMessage.MessageData;
                                    _lcmIotMessage.MessageID = recvMessage.MessageID;
                                    _lcmIotMessage.SourceID = recvMessage.SourceID;
                                    _lcmIotMessage.TargetID = recvMessage.TargetID;
                                    PubLcm.Publish(IotMessagePubName, _lcmIotMessage);
                                    Debug.Log(Encoding.ASCII.GetString(bufRecvData));
                                    Debug.Log(_lcmIotMessage.TimeStamp);
                                }
                            }

                        }
                    }
                }
                catch
                {

                }
                System.Threading.Thread.Sleep(_recvTime);
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            SubLcm = new LCM.LCM.LCM();
            SubLcm.Subscribe(IotMessageSubName, this);
            PubLcm = LCM.LCM.LCM.Singleton;
            _lcmIotMessage = new LcmIotMessage();
            _iotMessage = new IotMessage();
            _recvTime = (int) (1.0 / DataRecvHz)*1000;
            connect();

        }

        // Update is called once per frame
        void Update()
        {
            if(isStartAgain)
            {
                endFlag = true;
                socketSend.Close();
                if(c_thread.IsAlive)
                {
                    c_thread.Abort();
                }
                Start();
                isStartAgain = false;
            }
        }

        void OnDestroy()
        {
            endFlag = true;
            socketSend.Close();
            if (c_thread.IsAlive)
            {
                c_thread.Abort();
            }

        }

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            if (channel == IotMessageSubName)
            {
                LcmIotMessage msg = new LcmIotMessage(ins);
                _iotMessage.TimeStamp = msg.TimeStamp;
                _iotMessage.TargetID = msg.TargetID;
                _iotMessage.SourceID = msg.SourceID;
                _iotMessage.MessageID = msg.MessageID;
                _iotMessage.MessageData = msg.MessageData;
                string iotMsgJson = JsonUtility.ToJson(_iotMessage);
                long dataLen = Encoding.UTF8.GetBytes(iotMsgJson).Length;
                DataSize dataSize = SocketDataCommon.GetDataSizeStrut(dataLen);
                byte[] dataSizeBuffer = strb.StructToBytes(dataSize);
                byte[] dataFlagBuffer = new byte[2];
                dataFlagBuffer[0] = (byte) '\n';
                dataFlagBuffer[1] = (byte) '\n';
                byte[] dataSendBuffer = Encoding.UTF8.GetBytes(iotMsgJson);
                socketSend.Send(dataFlagBuffer);
                socketSend.Send(dataSizeBuffer);
                socketSend.Send(dataSendBuffer);
               // DateTime centuryBegin = new DateTime(2001, 1, 1);
               // DateTime currentDate = DateTime.Now;
               // long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
               // TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
               // Debug.Log("recv"+elapsedSpan.TotalSeconds);
            }
        }
    }
}