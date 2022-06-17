using System.Text;
using DTUAVCARS.DTNetwok.Message;
using DTUAVCARS.DTNetWork.SocketNetwork;
using UnityEngine;
using UnityWebSocket;
using LCM.LCM;
using lcm_iot_msgs;
public class websocket_client : MonoBehaviour, LCM.LCM.LCMSubscriber
{
        public string address = "ws://127.0.0.1";
        public bool logMessage = true;
        private IWebSocket socket;
        public string IotMessageSubName;
        public string IotMessagePubName;
        private LCM.LCM.LCM PubLcm;
        private LCM.LCM.LCM SubLcm;
        private LcmIotMessage _lcmIotMessage;
        private IotMessage _iotMessage;
    void Start()
        {
            SubLcm = new LCM.LCM.LCM();
            SubLcm.Subscribe(IotMessageSubName, this);
            PubLcm = LCM.LCM.LCM.Singleton;
            _lcmIotMessage = new LcmIotMessage();
            _iotMessage = new IotMessage();
            socket = new WebSocket(address);
            socket.OnOpen += Socket_OnOpen;
            socket.OnMessage += Socket_OnMessage;
            socket.OnClose += Socket_OnClose;
            socket.OnError += Socket_OnError;
            socket.ConnectAsync();
    }

    bool IsJsonData(string strData)
    {
        if (strData[0] != '{')
            return false;

        int num = 1;
        for (int i = 1; i < strData.Length; ++i)
        {
            if (strData[i] == '{')
            {
                ++num;
            }
            else if (strData[i] == '}')
            {
                --num;
            }

            if (num == 0)
            {
                return true;
            }
        }

        return false;
    }
    bool SendMessage(string msg)
        {
            bool is_send = false;
            socket.SendAsync(msg);
            //var bytes = System.Text.Encoding.UTF8.GetBytes(sendText);
            //socket.SendAsync(bytes);
             return is_send;
        }

        void CloseWebsocket()
        {
            socket.CloseAsync();
    }
        private string log = "";
        private int sendCount;
        private int receiveCount;
        private Vector2 scrollPos;

        private void AddLog(string str)
        {
            log += str;
            // max log
            if (log.Length > 32 * 1024)
            {
                log = log.Substring(16 * 1024);
            }
        }

        private void Socket_OnOpen(object sender, OpenEventArgs e)
        {
            AddLog(string.Format("Connected: {0}\n", address));
        }

        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsBinary)
            {
                if (logMessage)
                    AddLog(string.Format("Receive Bytes ({1}): {0}\n", e.Data, e.RawData.Length));
            }
            else if (e.IsText)
            {
                if (IsJsonData(e.Data))
                { 
                    IotMessage recvMessage = JsonUtility.FromJson<IotMessage>(e.Data);
                    _lcmIotMessage.TimeStamp = recvMessage.TimeStamp;
                    _lcmIotMessage.MessageData = recvMessage.MessageData;
                    _lcmIotMessage.MessageID = recvMessage.MessageID;
                    _lcmIotMessage.SourceID = recvMessage.SourceID;
                    _lcmIotMessage.TargetID = recvMessage.TargetID;
                    PubLcm.Publish(IotMessagePubName, _lcmIotMessage);
                    Debug.Log(e.Data);
                    Debug.Log(_lcmIotMessage.TimeStamp);
                }

                if (logMessage)
                    AddLog(string.Format("Receive: {0}\n", e.Data));
            }
            receiveCount += 1;
        }

        private void Socket_OnClose(object sender, CloseEventArgs e)
        {
            AddLog(string.Format("Closed: StatusCode: {0}, Reason: {1}\n", e.StatusCode, e.Reason));
        }

        private void Socket_OnError(object sender, ErrorEventArgs e)
        {
            AddLog(string.Format("Error: {0}\n", e.Message));
        }

        private void OnApplicationQuit()
        {
            if (socket != null && socket.ReadyState != WebSocketState.Closed)
            {
                socket.CloseAsync();
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
                SendMessage(iotMsgJson);
            // DateTime centuryBegin = new DateTime(2001, 1, 1);
            // DateTime currentDate = DateTime.Now;
            // long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            // TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            // Debug.Log("recv"+elapsedSpan.TotalSeconds);
            Debug.Log("ddddddd");
            }
        }
}
