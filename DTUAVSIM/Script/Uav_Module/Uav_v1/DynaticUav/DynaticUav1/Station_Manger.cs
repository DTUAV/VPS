using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Station_Manger : MonoBehaviour {

    //无人机的任务更新标志
    

    
    static public bool mission_uav1_flag = false;
    static public bool mission_uav1_next_flag = false;

   

    //无人机的任务数的序号
    
    static public int mission_uav1_num = 1;
   


    //任务数组
  
    static public Vector3[] uav1_mission = new Vector3[4];
    
    
    //初始化任务
    void mission_init()
    {
        

        uav1_mission[0] = new Vector3(2, 2, 2);
        uav1_mission[1] = new Vector3(2, 2, 3);
        uav1_mission[2] = new Vector3(3, 2, 3);
        uav1_mission[3] = new Vector3(3, 2, 2);

       


    }

    //任务调度
    static public  void mission_do()
    {
     
        if (mission_uav1_next_flag)
        {
            switch (mission_uav1_num)
            {
                case 1:
                    {
                        UAV1_1_power.target_x = uav1_mission[0].x;
                        UAV1_1_power.target_y = uav1_mission[0].y;
                        UAV1_1_power.target_z = uav1_mission[0].z;

                        mission_uav1_next_flag = false;
                        mission_uav1_num++;
                        break;
                    }

                case 2:
                    {
                        UAV1_1_power.target_x = uav1_mission[1].x;
                        UAV1_1_power.target_y = uav1_mission[1].y;
                        UAV1_1_power.target_z = uav1_mission[1].z;
                        mission_uav1_next_flag = false;
                        mission_uav1_num++;
                        break;
                    }

                case 3:
                    {
                        UAV1_1_power.target_x = uav1_mission[2].x;
                        UAV1_1_power.target_y = uav1_mission[2].y;
                        UAV1_1_power.target_z = uav1_mission[2].z;
                        mission_uav1_next_flag = false;
                        mission_uav1_num++;
                        break;
                    }

                case 4:
                    {
                        UAV1_1_power.target_x = uav1_mission[3].x;
                        UAV1_1_power.target_y = uav1_mission[3].y;
                        UAV1_1_power.target_z = uav1_mission[3].z;
                        mission_uav1_next_flag = false;
                        mission_uav1_num = 1;
                        break;
                    }
            }
        }

        
            
        
    }

	// Use this for initialization
	void Start () {
        mission_init();
    }
	
	// Update is called once per frame
	void Update () {

     
    }

    void FixedUpdate()
    {
      
    }
}
