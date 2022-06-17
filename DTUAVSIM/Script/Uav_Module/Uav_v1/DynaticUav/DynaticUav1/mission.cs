using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mission : MonoBehaviour {

    // Use this for initialization
    
    
    static public float x_last_uav1 = 1.0f;
    static public float y_last_uav1 = 2.0f;
    static public float z_last_uav1 = 1.0f;

   

   
   
    static public bool check_uav1_position_flag = false;
   
  //  GameObject uav4 = null;
        //检查无人机的位置并设置标志位
    

   

    void check_uav1_position()//无人机1号
    {

        if (Mathf.Abs(UAV1_1_power.target_x - UAV1_1_power.uav_1_self.transform.position.x) <= 0.01f && Mathf.Abs(UAV1_1_power.target_y - UAV1_1_power.uav_1_self.transform.position.y) <= 0.01f && Mathf.Abs(UAV1_1_power.target_z - UAV1_1_power.uav_1_self.transform.position.z) <= 0.01f)
        {
            Station_Manger.mission_uav1_next_flag = true;
            x_last_uav1 = UAV1_1_power.uav_1_self.transform.position.x;
            y_last_uav1 = UAV1_1_power.uav_1_self.transform.position.y;
            z_last_uav1 = UAV1_1_power.uav_1_self.transform.position.z;
           
        }
    }

   


    void Start () {
	
    }
	
	// Update is called once per frame
	void Update () {

        if(check_uav1_position_flag)
        {
            check_uav1_position();
        }
    }


    void LateUpdate()
    {
        Station_Manger.mission_do();
    }
    void FixedUpdate()
    {
       
    }
}
