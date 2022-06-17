using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class uav_ancient_village : MonoBehaviour
{

    //无人机螺旋桨对象
    GameObject UAV_propellor_1 = null;//无人机一号电机的螺旋桨
    GameObject UAV_propellor_2 = null;//无人机二号电机的螺旋桨
    GameObject UAV_propellor_3 = null;//无人机三号电机的螺旋桨
    GameObject UAV_propellor_4 = null;//无人机四号电机的螺旋桨

    public static float uav_position_x = 50;
    public static float uav_position_y = 2;
    public static float uav_position_z = 66;

    public static float uav_angle_pitch;
    public static float uav_angle_roll;
    public static float uav_angle_yaw;

    public static double current_position_x;
    public static double current_position_y;
    public static double current_position_z;

    private static float propellor_speech;

    


    //无人机的自身
    GameObject uav_self = null;


    //显示因子
    private float kvis = 0.02f;


    //开启新的线程，启动socket
    Thread propellor_thread;

    private bool isStop = false;//线程停止



    void getUAVPosition()
    {
        current_position_x = uav_self.transform.position.x;
        current_position_y = uav_self.transform.position.y;
        current_position_z = uav_self.transform.position.z;
    }



    void uav_move()
    {
        uav_self.transform.Rotate(new Vector3((kvis * (uav_angle_roll - uav_self.transform.eulerAngles.x)), (kvis * (uav_angle_yaw - uav_self.transform.eulerAngles.y)), (kvis * (uav_angle_pitch - uav_self.transform.eulerAngles.z))));
        uav_self.transform.Translate(new Vector3((kvis * (uav_position_x - uav_self.transform.position.x)), (kvis * (uav_position_y - uav_self.transform.position.y)), (kvis * (uav_position_z - uav_self.transform.position.z))), Space.World);
    }



    //更新螺旋桨的转速
    void propellor_update()
    {
        // while (!isStop)
        /// {
        UAV_propellor_1.transform.Rotate(new Vector3(0, propellor_speech, 0));
        UAV_propellor_2.transform.Rotate(new Vector3(0, propellor_speech, 0));
        UAV_propellor_3.transform.Rotate(new Vector3(0, -propellor_speech, 0));
        UAV_propellor_4.transform.Rotate(new Vector3(0, -propellor_speech, 0));
        //  }
    }



    // Start is called before the first frame update
    void Start()
    {
        //获取无人机的对象
        UAV_propellor_1 = GameObject.Find("uav2_1/motor/motor1/propellor");//无人机1的一号电机的螺旋桨
        UAV_propellor_2 = GameObject.Find("uav2_1/motor/motor2/propellor");//无人机1的二号电机的螺旋桨
        UAV_propellor_3 = GameObject.Find("uav2_1/motor/motor3/propellor");//无人机1的三号电机的螺旋桨
        UAV_propellor_4 = GameObject.Find("uav2_1/motor/motor4/propellor");//无人机1的四号电机的螺旋桨

        //获取无人机的对象
        uav_self = GameObject.Find("uav2_1");

        propellor_thread = new Thread(propellor_update);
        propellor_thread.IsBackground = true;
        // propellor_thread.Start();


    }

    // Update is called once per frame
    void Update()
    {
        // propellor_update();
        uav_move();
        propellor_speech = uav_self.transform.position.y * 1024;
        // propellor_update();
    }

    private void FixedUpdate()
    {
        propellor_update();
    }
    private void OnApplicationQuit()
    {
        isStop = true;
        propellor_thread.Abort();

    }

}
