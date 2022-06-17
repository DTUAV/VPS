using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using MavLink;

public class Subscriber {
    // Start is called before the first frame update
    private string name;

    public Subscriber(string str)
    {
        this.name = str;
    }

    public void recvMsg(string st)
    {
        Debug.Log("recvMsg"+st);
    }

    public void recvMavMsg(object sender, MavlinkPacket e)
    {
        Debug.Log("mavMsg:"+e.Message);
    }

    public void rxThreadFunc()
    {
        Debug.Log("rxThreadFunc");
    }
}
