using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.DTMission;
using DTUAVCARS.DTNetWork;
using DTUAVCARS.DTNetWork.Lcm.Publisher;
using DTUAVCARS.DTNetWork.SocketNetwork;
using DTUAVCARS.DTPlanning;
public class test_V_UAV_Prefabs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject hp_bar = (GameObject)Resources.Load("Prefabs/V_UAV/V_UAV");
        hp_bar.name = hp_bar.name +"_"+ 1;
        Vector3 a = new Vector3(2,5,7);
        Quaternion b = new Quaternion(0, 0, 0, 0);
        hp_bar.GetComponent<velocity_command>().TargetPositionRos = a;
        hp_bar.GetComponent<triangle_track_uav>().TrackCenter = a;
        hp_bar.GetComponent<triangle_track_uav>().enabled = true;
        Instantiate(hp_bar, a, b);

        GameObject hp_bar1 = (GameObject)Resources.Load("Prefabs/V_UAV/V_UAV");
        hp_bar1.name = hp_bar1.name + "_" + 2;
        Vector3 a1 = new Vector3(2, 5, 8);
        Quaternion b1 = new Quaternion(0, 0, 0, 0);
        hp_bar1.GetComponent<triangle_track_uav>().TrackCenter = a1;
        hp_bar1.GetComponent<velocity_command>().TargetPositionRos = a1;
        hp_bar1.GetComponentInChildren<UavPositionPub>().SourceID = 1;
        hp_bar1.GetComponentInChildren<UavPositionPub>().TargetID = 101;

        hp_bar1.GetComponent<triangle_track_uav>().enabled = true;

        Instantiate(hp_bar1,a1,b1);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
