using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class uavMission : MonoBehaviour
{
    List<string[]> WayPointList;
    public GetTxt WayPointTxt;
    public path_uav UavPath;
    public string wayPointName = "waypoint";
    Vector3[] wayPoint;
    public GameObject uav;
    public XYZControl xyzControl;
    private Vector3 Rb_position = new Vector3();
    private bool isStop = false;
    Thread c_thread;

    // Start is called before the first frame update
    void Start()
    {
        Rb_position = uav.GetComponent<Rigidbody>().position;
        
        WayPointList = new List<string[]>();
        WayPointTxt = new GetTxt();
        UavPath = new path_uav();
        WayPointList = WayPointTxt.GetWayPoint(wayPointName);
        wayPoint = new Vector3[WayPointList.Count];
        int i = 0;
        foreach (string[] str in WayPointList)
        {
            string x, y, z;
            x = str[0];
            y = str[1];
            z = str[2];
             wayPoint[i] = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
           // Debug.Log(wayPoint[i]);
            i++;
        }
        UavPath.InitPath(wayPoint, false, 0.2f);
       // UavPath.isLoop = true;
        i = 0;
        //  UavPath.NextWayPoint();
        c_thread = new Thread(check_Position);
        c_thread.IsBackground = true;
        c_thread.Start();
    }
    void changePosition()
    {
        xyzControl.uavPosition_x = UavPath.waypoint.x;
        xyzControl.uavPosition_y = UavPath.waypoint.y;
        xyzControl.uavPosition_z = UavPath.waypoint.z;
    }
    // Update is called once per frame
    void Update()
    {
       // Rb_position = uav.GetComponent<Rigidbody>().position;
       // changePosition();
    }
    void FixedUpdate()
    {

        Rb_position = uav.GetComponent<Rigidbody>().position;
        changePosition();
        UavPath.autoChangePath(uav, 0.025f);


    }
    void check_Position()
    {
        while (!isStop)
        {
          //  UavPath.autoChangePath(Rb_position, 0.05f);
           
        }
    }
    void OnDestroy()
    {
        isStop = true;
       
        if (c_thread.IsAlive)
        {
            c_thread.Abort();
        }
        

    }
}
