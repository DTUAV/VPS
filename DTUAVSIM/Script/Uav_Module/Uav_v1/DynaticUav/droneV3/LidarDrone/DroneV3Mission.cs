using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using SimUnity.droneV2;
namespace SimUnity.droneV3
{
    public class DroneV3Mission : MonoBehaviour
    {
        List<string[]> WayPointList;
        public GetTxt WayPointTxt;
        public DroneV3Path UavPath;
        public string wayPointName = "waypoint";
        Vector4[] wayPoint;
        public GameObject droneV3;
        public DroneV2XYZControl xyzControl;
        public DroneV2YawControl yawControl;
        private Vector3 Rb_position = new Vector3();
        private bool isStop = false;
        public float mission_point_err;
        Thread c_thread;

        // Start is called before the first frame update
        void Start()
        {
            Rb_position = droneV3.GetComponent<Rigidbody>().position;

            WayPointList = new List<string[]>();
            WayPointTxt = new GetTxt();
            UavPath = new DroneV3Path();
            WayPointList = WayPointTxt.GetWayPoint(wayPointName);
            wayPoint = new Vector4[WayPointList.Count];
            int i = 0;
            foreach (string[] str in WayPointList)
            {
                string x, y, z,yaw;
                x = str[0];
                y = str[1];
                z = str[2];
                yaw = str[3];
                wayPoint[i] = new Vector4(float.Parse(x), float.Parse(y), float.Parse(z),float.Parse(yaw));
                
                i++;
            }
            Debug.Log(wayPoint[1]);
            UavPath.InitPath(wayPoint, false, 0.1f);
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
            yawControl.refer_yaw = UavPath.waypoint.w;
        }
        // Update is called once per frame
        void Update()
        {
            // Rb_position = uav.GetComponent<Rigidbody>().position;
            // changePosition();
        }
        void FixedUpdate()
        {

            Rb_position = droneV3.GetComponent<Rigidbody>().position;
            changePosition();
            UavPath.autoChangePath(droneV3, mission_point_err);


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
}
