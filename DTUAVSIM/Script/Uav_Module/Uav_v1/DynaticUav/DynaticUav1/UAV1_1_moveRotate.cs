using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_moveRotate : MonoBehaviour {

    //无人机的自身
    [Header("无人机对象")]
  public  GameObject uav_1_self ;

   //无人机姿态角
   void uav_Rotate()
    {
        if(UAV1_1_power.update_rotate_flag)
        {
            uav_1_self.transform.Rotate(-UAV1_1_angle.roll, -UAV1_1_angle.yaw, UAV1_1_angle.pitch);
           
        if (UAV1_1_power.uav_control_model)
        {
            UAV1_1_power.update_rotate_flag = false;
        }
            
        }
    }

    //无人机位置

        void uav_positiom()
    {
        if ((uav_1_self.transform.position.x + UAV1_1_linear_motion.x >= 0 && uav_1_self.transform.position.x + UAV1_1_linear_motion.x <= 50 + 52) && (uav_1_self.transform.position.y + UAV1_1_linear_motion.y >= 0 && uav_1_self.transform.position.y + UAV1_1_linear_motion.y <= 50) && (uav_1_self.transform.position.z + UAV1_1_linear_motion.z >= 0 && uav_1_self.transform.position.z + UAV1_1_linear_motion.z <= 50+66))
        {
            uav_1_self.transform.Translate(new Vector3(UAV1_1_linear_motion.x, UAV1_1_linear_motion.y, UAV1_1_linear_motion.z), Space.World);
        }
    }

	// Use this for initialization
	void Start () {

        
       


	}
	
	// Update is called once per frame
	void Update () {

         uav_Rotate();

        

    }
    void FixedUpdate()
    {
        uav_positiom();
    }
}
