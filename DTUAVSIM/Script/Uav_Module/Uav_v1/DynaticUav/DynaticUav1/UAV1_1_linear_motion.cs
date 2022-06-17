using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_linear_motion : MonoBehaviour {

    //空气阻力参数
    private float Kx = 0.0f;//X轴方向的空气阻力参数
    private float Ky = 0.0f;//Y轴方向的空气阻力参数
    private float Kz = 0.0f;//Z轴方向的空气阻力参数

    //空气阻力
    static public float fx = 0.0f;//X轴方向的空气阻力
    static public float fy = 0.0f;//Y轴方向的空气阻力
    static public float fz = 0.0f;//Z轴方向的空气阻力

    //线运动的速度
    static public float x_speed = 0.0f;//X轴方向的速度
    static public float y_speed = 0.0f;//Y轴方向的速度
    static public float z_speed = 0.0f;//Z轴方向的速度

    //线运动的加速度
    static public float x_acc = 0.0f;//X轴方向的加速度
    static public float y_acc = 0.0f;//Y轴方向的加速度
    static public float z_acc = 0.0f;//Z轴方向的加速度

    //线运动的位移
    static public float x = 0.0f;//X轴方向的位移
    static public float y = 0.0f;//Y轴方向的位移
    static public float z = 0.0f;//Z轴方向的位移

    //无人机的质量
    private float mass_uav = 0.0f;//无人机的质量

    //重力加速度
    private float g = 0.0f;//重力加速度

    //固定更新的时间，此时间用于计算速度和位移
    private float fixed_update_time = 0.0f;

    //起飞标志
    static public bool take_off_uav_flag = false;

    //无人机对象
    [Header("无人机对象")]
   public GameObject UAV_1_self ;
    

    //检查起飞和设置标志位
    void check_takeoff()
    {




        if (y_acc < 0 && UAV_1_self.transform.position.y <= 0.08f)
        {
            take_off_uav_flag = false;
            y_speed = 0.0f;
            y = 0;
        }
        else
        { take_off_uav_flag = true; }
    }

    //线运动参数初始化
    void linear_motion_init()
    {
        //更新空气阻力的参数
        Kx = 0.05292f;
        Ky = 0.05292f;
        Kz = 0.5292f;

        //无人机质量
        mass_uav = 1.4f;

        //重力加速度
        g = 9.8f;

        //固定更新的时间
        // fixed_update_time = 1f;
        fixed_update_time = 0.8f;
    }

    //更新空气阻力
    void fx_fy_fz_update()
    {
        fx = Kx * x_speed * x_speed;
        fy = Ky * y_speed * y_speed;
        fz = Kz * z_speed * z_speed;
    }

   //更新线运动
   void linear_motion_update()
    {
        x_acc = (UAV1_1_power.U1 * (Mathf.Cos(UAV1_1_angle.yaw_now * Mathf.Deg2Rad) * Mathf.Sin(UAV1_1_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Cos(UAV1_1_angle.roll_now * Mathf.Deg2Rad) + Mathf.Sin(UAV1_1_angle.yaw_now * Mathf.Deg2Rad) * Mathf.Sin(UAV1_1_angle.roll_now * Mathf.Deg2Rad))) / mass_uav;
        z_acc = (UAV1_1_power.U1 * (Mathf.Sin(UAV1_1_angle.yaw_now * Mathf.Deg2Rad) * Mathf.Sin(UAV1_1_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Cos(UAV1_1_angle.roll_now * Mathf.Deg2Rad) + Mathf.Sin(UAV1_1_angle.roll_now * Mathf.Deg2Rad) * Mathf.Cos(UAV1_1_angle.yaw_now * Mathf.Deg2Rad))) / mass_uav;
        y_acc = (UAV1_1_power.U1 * (Mathf.Cos(UAV1_1_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Cos(UAV1_1_angle.roll_now * Mathf.Deg2Rad)) - mass_uav * g ) / mass_uav;




  // y_acc = ((UAV_4_power.U1 * (Mathf.Cos(UAV_4_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Sin(UAV_4_angle.roll_now * Mathf.Deg2Rad) * Mathf.Cos(UAV_4_angle.yaw_now * Mathf.Deg2Rad) + Mathf.Sin(UAV_4_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Sin(UAV_4_angle.yaw_now * Mathf.Deg2Rad)))-fx) / mass_uav;
       
       // x_acc = (((UAV_4_power.F1 + UAV_4_power.F2 + UAV_4_power.F3 + UAV_4_power.F4) * (Mathf.Cos(UAV_4_angle.pitch) * Mathf.Sin(UAV_4_angle.roll) * Mathf.Cos(UAV_4_angle.yaw) + Mathf.Sin(UAV_4_angle.pitch) * Mathf.Sin(UAV_4_angle.yaw))) ) / mass_uav;
        // x_acc = (UAV_4_power.U3 - UAV_4_power.U3_before - fx) / mass_uav;

 // x_acc = ((UAV_4_power.U1 * (Mathf.Sin(UAV_4_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Cos(UAV_4_angle.roll_now * Mathf.Deg2Rad) * Mathf.Cos(UAV_4_angle.yaw_now * Mathf.Deg2Rad) + Mathf.Cos(UAV_4_angle.pitch_now * Mathf.Deg2Rad) * Mathf.Sin(UAV_4_angle.yaw_now * Mathf.Deg2Rad)))-fy) / mass_uav;

      

        //   y_acc = (((UAV_4_power.F1 + UAV_4_power.F2 + UAV_4_power.F3 + UAV_4_power.F4) * (Mathf.Sin(UAV_4_angle.pitch) * Mathf.Sin(UAV_4_angle.roll) * Mathf.Cos(UAV_4_angle.yaw) + Mathf.Cos(UAV_4_angle.pitch) * Mathf.Sin(UAV_4_angle.yaw)))) / mass_uav;
        //  y_acc = (UAV_4_power.U2 - UAV_4_power.U2_before - fy) / mass_uav;

 // z_acc = ((UAV_4_power.U1 * (Mathf.Cos(UAV_4_angle.roll_now * Mathf.Deg2Rad) * Mathf.Cos(UAV_4_angle.pitch_now * Mathf.Deg2Rad)) - mass_uav * g )-fz) / mass_uav;

        //  z_acc = (((UAV_4_power.F1 + UAV_4_power.F2 + UAV_4_power.F3 + UAV_4_power.F4) * (Mathf.Cos(UAV_4_angle.roll) * Mathf.Cos(UAV_4_angle.yaw))) - mass_uav * g-fz ) / mass_uav;

        //  z_acc = (((UAV_4_power.F1 + UAV_4_power.F2 + UAV_4_power.F3 + UAV_4_power.F4) ) - mass_uav * g) / mass_uav;
        // z_acc = (UAV_4_power.U1 - mass_uav* g- fy) / mass_uav;
        

            x_acc = Mathf.Clamp(x_acc, -10,10);
            y_acc = Mathf.Clamp(y_acc, -10, 10);
            z_acc = Mathf.Clamp(z_acc, -10, 10);

        if (take_off_uav_flag)
        {

            //x_speed = x_speed + x_acc * fixed_update_time;
            //y_speed = y_speed + y_acc * fixed_update_time;
            //z_speed = z_speed + z_acc * fixed_update_time;

            x_speed =  x_acc * fixed_update_time;
            y_speed =  y_acc * fixed_update_time;
            z_speed =  z_acc * fixed_update_time;

            x_speed = Mathf.Clamp(x_speed, -5, 5);
            y_speed = Mathf.Clamp(y_speed, -5, 5);
            z_speed = Mathf.Clamp(z_speed, -5, 5);


            x = x_speed * fixed_update_time;
            y = y_speed * fixed_update_time;
            z = z_speed * fixed_update_time;
        }



        //x = x + x_speed * fixed_update_time;
        //y = y + y_speed * fixed_update_time;
        //z = z + z_speed * fixed_update_time;

      

       // z = Mathf.Clamp(z, 0, 500);
        
    }
	// Use this for initialization
	void Start () {
        
        linear_motion_init();
        
    }
	
	// Update is called once per frame
	void Update () {

        fx_fy_fz_update();
        check_takeoff();
      

    }

    void FixedUpdate()
    {
        linear_motion_update();
    }
}
