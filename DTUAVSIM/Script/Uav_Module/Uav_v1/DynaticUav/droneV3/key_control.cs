using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.droneV2;
public class key_control : MonoBehaviour
{
    // Start is called before the first frame update
    public DroneV2XYZControl DroneV2XYZ;
    public DroneV2XYZControl DroneV2XYZ_1;
    public List<DroneV2XYZControl> all_uav_control;
    public bool v_uav_reach = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.F12))
        {
            DroneV2XYZ.takeoff = true;
            DroneV2XYZ_1.takeoff = true;
        }
        if(Input.GetKeyDown(KeyCode.F1))
        {
            DroneV2XYZ.takeoff = true;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            DroneV2XYZ_1.takeoff = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            v_uav_reach = true;
        }
        */

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (all_uav_control.Capacity!=0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.takeoff = true;
                    uav.is_lane = false;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (all_uav_control.Capacity != 0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.is_lane = true;
                    uav.takeoff = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (all_uav_control.Capacity != 0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.uavPosition_x = uav.uavPosition_x - 1;
                    uav.uavPosition_y = uav.uavPosition_y + 0;
                    uav.uavPosition_z = uav.uavPosition_z + 0;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (all_uav_control.Capacity != 0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.uavPosition_x = uav.uavPosition_x + 1;
                    uav.uavPosition_y = uav.uavPosition_y + 0;
                    uav.uavPosition_z = uav.uavPosition_z + 0;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (all_uav_control.Capacity != 0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.uavPosition_x = uav.uavPosition_x + 0;
                    uav.uavPosition_y = uav.uavPosition_y + 0;
                    uav.uavPosition_z = uav.uavPosition_z + 1;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (all_uav_control.Capacity != 0)
            {
                foreach (DroneV2XYZControl uav in all_uav_control)
                {
                    uav.uavPosition_x = uav.uavPosition_x + 0;
                    uav.uavPosition_y = uav.uavPosition_y + 0;
                    uav.uavPosition_z = uav.uavPosition_z - 1;
                }
            }
        }
    }
}
