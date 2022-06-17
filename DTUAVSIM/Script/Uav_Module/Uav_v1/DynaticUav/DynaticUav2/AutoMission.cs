using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMission : MonoBehaviour
{
    List<string[]> WayPointList;
    public GetTxt WayPointTxt;
    public path_uav UavPath;
    Vector3[] wayPoint;
    public Transform uavTransform;
    public float throttleFactor = 0.0001f;//油门控制因子
    static public float throttleValue;//油门量
    static public int Throttle = 0;
    static public int Yaw = 0;
    static public int Pitch = 0;
    static public int Roll = 0;
    static public float PitchAim =0;
    static public float RollAim = 0;
    static public float YawAim = 0;
    static public float height = 0;
    public void  controlByPath()
    {
        float distant = Vector3.Distance(UavPath.waypoint, uavTransform.position);
        throttleValue = 1; //distant * throttleFactor;

        Vector3 dir = UavPath.waypoint - uavTransform.position;
        Quaternion qua = Quaternion.LookRotation(dir);//要旋转的角度

        PitchAim = qua.x;
        RollAim = qua.y;
        YawAim = qua.y;
        height = UavPath.waypoint.y;
        if (UavPath.waypoint.x - uavTransform.position.x >0)
        {
            Roll = -1;
        }
        else if(UavPath.waypoint.x - uavTransform.position.x <0)
        {
            Roll = 1;
        }
        else
        {
            Roll = 0;
        }

        if (UavPath.waypoint.z - uavTransform.position.z > 0)
        {
            Pitch = -1;
        }
        else if (UavPath.waypoint.z - uavTransform.position.z < 0)
        {
            Pitch = 1;
        }
        else
        {
            Pitch = 0;
        }

        if (UavPath.waypoint.y - uavTransform.position.y > 0)
        {
            Throttle = 1;
        }
        else if (UavPath.waypoint.x - uavTransform.position.x < 0)
        {
           Throttle = -1;
        }
        else
        {
            Throttle = 0;
        }

        if(qua.y - uavTransform.eulerAngles.y>0)
        {
            Yaw = 1;
        }
        else if(qua.y - uavTransform.eulerAngles.y <0)
        {
            Yaw = -1;
        }
        else
        {
            Yaw = 0;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        WayPointList = new List<string[]>();
        WayPointTxt = new GetTxt();
        UavPath = new path_uav();
        WayPointList = WayPointTxt.GetWayPoint("waypoint");
        wayPoint = new Vector3[WayPointList.Count];
        int i = 0;
        foreach(string[] str in WayPointList)
        {
            wayPoint[i] = new Vector3(float.Parse(str[0]),float.Parse(str[1]),float.Parse( str[2]));
        }
        UavPath.InitPath(wayPoint, false, 0.2f);
        i = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UavPath.autoChangePath(uavTransform);
        controlByPath();
    }

}
