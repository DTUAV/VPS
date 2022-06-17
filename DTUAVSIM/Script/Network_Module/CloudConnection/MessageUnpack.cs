using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using LCM.LCM;
using lcm_iot_msgs;
using DTUAVCARS.DTNetwok.Message;

namespace DTUAVCARS.DTNetWork.SocketNetwork
{

    public class MessageUnpack : MonoBehaviour, LCM.LCM.LCMSubscriber
    {
        public string IotMessageSubName;

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {

            if (channel == IotMessageSubName)
            {
                LcmIotMessage msg = new LcmIotMessage(ins);
                if (msg.MessageID == 1)
                {
                    CurrentPoseMessage position_msg = JsonUtility.FromJson<CurrentPoseMessage>(msg.MessageData);

                }
            }

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }
    }
}
