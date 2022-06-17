using lcm_iot_msgs;
using System;
using System.Collections;
using System.Collections.Generic;
using DTUAVCARS.DTNetwok.Message;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class test_code : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = Convert.ToString(56555, 2);
        Debug.Log("a:" + a);
        Debug.Log("a_len"+a.Length);
        int num = 4;
        string snum = Convert.ToString(num, 2).PadLeft(32, '0');//num.ToString().PadLeft(32, '0');
        Debug.Log("snum:"+snum);
        byte[] dataSendBuffer = System.Text.Encoding.UTF8.GetBytes(snum);
        Debug.Log("message_len: "+ dataSendBuffer.Length);
        Int32 cov_int = Convert.ToInt32(snum, 2);
        Debug.Log("cov_num:"+cov_int);


        IotMessageList listMessage = new IotMessageList();
        CurrentPoseMessage currentPose = new CurrentPoseMessage();
        currentPose.PositionX = 0;
        currentPose.PositionY = 0;
        currentPose.PositionZ = 0;
        currentPose.RotationW = 1;
        currentPose.RotationX = 1;
        currentPose.RotationY = 1;
        currentPose.RotationZ = 1;
        listMessage.MessageData= new ArrayList(1);
        listMessage.MessageData.Add(currentPose);
        listMessage.MessageID = 1;
        listMessage.SourceID = 1;
        listMessage.TargetID = 1;
        listMessage.TimeStamp = 1;
        string datajson = JsonConvert.SerializeObject(listMessage);
        Debug.Log("datajson"+datajson);
        var c = JsonConvert.DeserializeObject<IotMessageList>(datajson);
       // CurrentPoseMessage tem_cur = (CurrentPoseMessage) c.MessageData[0];
       // Debug.Log("tem_cur_positionx"+tem_cur.PositionX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
