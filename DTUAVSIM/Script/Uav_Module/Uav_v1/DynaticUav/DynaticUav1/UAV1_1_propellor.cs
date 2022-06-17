using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_propellor : MonoBehaviour {

    //无人机螺旋桨对象
    [Header("无人机螺旋桨对象")]
   public  GameObject UAV_propellor_FR ;//无人机一号电机的螺旋桨
   public  GameObject UAV_propellor_BL;//无人机二号电机的螺旋桨
   public  GameObject UAV_propellor_FL ;//无人机三号电机的螺旋桨
   public  GameObject UAV_propellor_BR;//无人机四号电机的螺旋桨
                                      //public  GameObject UAV_propellor_1 ;//无人机一号电机的螺旋桨
                                      //public  GameObject UAV_propellor_2 ;//无人机二号电机的螺旋桨
                                      //public  GameObject UAV_propellor_3 ;//无人机三号电机的螺旋桨
                                      //public GameObject UAV_propellor_4;//无人机四号电机的螺旋桨

    //更新螺旋桨的转速
    void propellor_update()
    {
        UAV_propellor_FR.transform.Rotate(new Vector3(0, UAV1_1_power.motor_1_speed, 0));
        UAV_propellor_BL.transform.Rotate(new Vector3(0, UAV1_1_power.motor_2_speed, 0));
        UAV_propellor_FL.transform.Rotate(new Vector3(0, -UAV1_1_power.motor_3_speed, 0));
        UAV_propellor_BR.transform.Rotate(new Vector3(0, -UAV1_1_power.motor_4_speed, 0));
    }
  
	// Use this for initialization
	void Start () {

        

    }
	
	// Update is called once per frame
	void Update () {

        propellor_update();


    }

}
