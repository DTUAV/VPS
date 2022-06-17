using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_angle : MonoBehaviour {

    //无人机的转动惯量
    private float Ix = 0.0f;
    private float Iy = 0.0f;
    private float Iz = 0.0f;

    //无人机旋翼中心到无人机坐标系x轴的垂直距离
    private float d = 0.0f;

    //无人机的姿态角
    static public float roll = 0.0f;// 无人机的横滚角度增量
    static public float pitch = 0.0f;//无人机的俯仰角度增量
    static public float yaw = 0.0f;//无人机的偏航角度增量

    static public float yaw_now = 0.0f;//无人机的偏航角度
    static public float roll_now = 0.0f;//无人机的横滚角度
    static public float pitch_now = 0.0f;//无人机的俯仰角度

    static public float yaw_before = 0.0f;
    static public float roll_before = 0.0f;
    static public float pitch_before = 0.0f;//无人机前一次姿态角度

    //无人机的姿态角角速度
    static public float roll_speed = 0.0f;//无人机的横滚角度的角速度
    static public float pitch_speed = 0.0f;//无人机的俯仰角的角速度
    static public float yaw_speed = 0.0f;//无人机的偏航角的角速度

    //无人机的姿态角角加速度
    static public float roll_acc = 0.0f;//无人机的横滚角度的角加速度
    static public float pitch_acc = 0.0f;//无人机的俯仰角的角加速度
    static public float yaw_acc = 0.0f;//无人机的偏航角的角加速度

    //固定更新的时间
    static public float fixed_update_time = 0.0f;//FixedUpdate函数运行的时间，以此时间求得角速度和角度

    //角加速度计算的参数
    private float k1 = 0.0f;//横滚角角加速度
    private float k2 = 0.0f;//俯仰角角加速度
    private float k3 = 0.0f;//偏航角角加速度

    //与角度计算有关的参数初始化
    void angle_para_init()
    {
        //转动惯量初始化
        Ix = 0.01777f;
        Iy = 0.01777f;
        Iz = 0.03451f;
        //无人机旋翼中心到无人机坐标系x轴的垂直距离初始化
        d = 0.2f;
        //固定更新的时间初始化
        fixed_update_time = 0.5f;

        //角加速度计算的参数//改变这个可以改变F滑块的灵敏度
        k1 = 0.1f;
        k2 = 0.1f;
        k3 = 0.1f;

    }

    //姿态角更新
    void angle_update()
    {
        if (UAV1_1_power.update_angle_flag)
       {
            if (UAV1_1_power.uav_control_model)
            {
                //手动模式飞行角度控制

                roll_acc = d * (UAV1_1_power.U2 - UAV1_1_power.U2_before - k1 * roll_speed) / Ix;
                pitch_acc = d * (UAV1_1_power.U3 - UAV1_1_power.U3_before - k2 * pitch_speed) / Iy;
                yaw_acc = d * (UAV1_1_power.U4 - UAV1_1_power.U4_before - k3 * yaw_speed) / Iz;

                UAV1_1_power.U1_before = UAV1_1_power.U1;
                UAV1_1_power.U2_before = UAV1_1_power.U2;
                UAV1_1_power.U3_before = UAV1_1_power.U3;
                UAV1_1_power.U4_before = UAV1_1_power.U4;


                //roll_speed = roll_speed + roll_acc * fixed_update_time;
                //pitch_speed = pitch_speed + pitch_acc * fixed_update_time;
                //yaw_speed = yaw_speed + yaw_acc * fixed_update_time;

                roll_speed = roll_acc * fixed_update_time;
                pitch_speed = pitch_acc * fixed_update_time;
                yaw_speed = yaw_acc * fixed_update_time;


                roll_now = roll_now + roll_speed * fixed_update_time;
                pitch_now = pitch_now + pitch_speed * fixed_update_time;
                yaw_now = yaw_now + yaw_speed * fixed_update_time;

                roll = roll_speed * fixed_update_time*8000000000000;
                pitch = pitch_speed * fixed_update_time*8000000000000;
                yaw = yaw_speed * fixed_update_time*8000000000000;
               


                roll = Mathf.Clamp(roll, -30f, 30f);
                pitch = Mathf.Clamp(pitch, -30f, 30f);
                yaw = Mathf.Clamp(yaw, -30f, 30f);

                UAV1_1_power.update_angle_flag = false;
                UAV1_1_power.update_rotate_flag = true;

            }
            else
            {
                //自动飞行角度控制
                roll = roll_now - roll_before;
                pitch = pitch_now - pitch_before;
                yaw = yaw_now - yaw_before;

                roll_before = roll_now;
                pitch_before = pitch_now;
                yaw_before = yaw_now;

                UAV1_1_power.update_rotate_flag = true;
            }

           
        }
    }

    // Use this for initialization
    void Start () {

        angle_para_init();

    }
	
	// Update is called once per frame
	void Update () {
 
    }

    void FixedUpdate()
    {
        angle_update();
    }
}
