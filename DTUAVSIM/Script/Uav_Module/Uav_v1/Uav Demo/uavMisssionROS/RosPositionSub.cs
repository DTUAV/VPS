using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
public class RosPositionSub : MonoBehaviour
{
    // Start is called before the first frame update
    public  PoseStampedSubscriber ROS_To_Unity_Pose_Sub;
    public GameObject uav;
    public XYZControl xyzControl;
    public bool change = false;

    /*
    public bool isReach(GameObject obj, float erro)
    {
        Vector3 position = obj.GetComponent<Rigidbody>().position;

        if (Mathf.Abs(xyzControl.uavPosition_x - position.x) < erro && Mathf.Abs(xyzControl.uavPosition_y - position.y) < erro && Mathf.Abs(xyzControl.uavPosition_z- position.z) < erro)
        {
            change = true;
            return true;

        }
        else
            return false;
    }

    */
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xyzControl.uavPosition_x = ROS_To_Unity_Pose_Sub.position.x;
        xyzControl.uavPosition_y = ROS_To_Unity_Pose_Sub.position.y;
        xyzControl.uavPosition_z = ROS_To_Unity_Pose_Sub.position.z;
    }
}
