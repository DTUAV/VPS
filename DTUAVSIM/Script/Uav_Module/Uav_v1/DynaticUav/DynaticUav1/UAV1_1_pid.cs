using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_pid : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
      
    }

    void FixedUpdate()
    {
        if ((!UAV1_1_power.uav_control_model)&&UAV1_1_power.pose_control_flag)
        {
            //自动飞行调整
            UAV1_1_power.PID_control();
        }
    }
}
