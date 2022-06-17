using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_power : MonoBehaviour {

 static public  GUIStyle label_style1 ;
 static public  GUIStyle label_style2 ;//GUI的style

    static public bool keep_laser_data_flag = false;//保存激光的数据标志

    //起飞的高度
    static public float height_takeoff = 2.0f;
    //起飞的PID
    static public float kp = 0.6f;
    static public float ki = 0.14f;
    static public float kd = 0.02f;

    static public float err = 0.0f;
    static public float err_last = 0.0f;
    static public float intergral = 0.0f;

    //位置y轴的控制PID参数
    static public float kp_y = 0.6f;
    static public float ki_y = 0.04f;
    static public float kd_y = 0.02f;

    static public float err_y = 0.0f;
    static public float err_last_y = 0.0f;
    static public float intergral_y = 0.0f;

    //位置x轴的控制PID参数
    static public float kp_x = 0.2f;
    static public float ki_x = 0.04f;
    static public float kd_x = 0.02f;

    static public float err_x = 0.0f;
    static public float err_last_x = 0.0f;
    static public float intergral_x = 0.0f;

    //位置z轴的控制PID参数
    static public float kp_z = 0.2f;
    static public float ki_z = 0.04f;
    static public float kd_z = 0.02f;

    static public float err_z = 0.0f;
    static public float err_last_z = 0.0f;
    static public float intergral_z = 0.0f;

    //偏航角yaw的控制的PID参数
    static public float kp_yaw = 0.6f;
    static public float ki_yaw = 0.14f;
    static public float kd_yaw = 0.02f;

    static public float err_yaw = 0.0f;
    static public float err_last_yaw = 0.0f;
    static public float intergral_yaw = 0.0f;


    //输入期望的位置和偏航角
    static public float target_x = 2.0f;
    static public float target_y = 2.0f;
    static public float target_z = 2.0f;
    static public float target_yaw = 0.0f;

    static public  string yaw_input = "0";//从界面输入的期望偏航角
    static public  string x_input = "0";//从界面输入的期望位置x
    static public  string y_input = "0";//从界面输入的期望位置y
    static public  string z_input = "0";//从界面输入的期望位置z



    //四个电机的拉力F=p*ct*motor_speed*motor_speed*0.5=k*motor_speed*motor_speed
    static public float F1 = 0.0f;//无人机电机1的拉力
    static public float F2 = 0.0f;//无人机电机2的拉力
    static public float F3 = 0.0f;//无人机电机3的拉力
    static public float F4 = 0.0f;//无人机电机4的拉力

    //当前的拉力
    static public  float current_F1 = 0.0f;
    static public  float current_F2 = 0.0f;
    static public  float current_F3 = 0.0f;
    static public  float current_F4 = 0.0f;

    //更新标志
    static public bool update_xyz_flag = false;//更新xyz的加速度、速度、位移的标志
    static public bool update_angle_flag = false;//更新角度的加速度、速度、角度的标志
    static public bool update_move_flag = false;//更新无人机的位置，在更新xyz后设置标志位
    static public bool update_rotate_flag = false;//更新无人机的姿态角，在更新角度后设置标志位
    static public bool pose_control_flag = false;//定点控制飞行的标志

    //四个电机的转速
    static public float motor_1_speed = 0.0f;//无人机电机1的转速
    static public float motor_2_speed = 0.0f;//无人机电机2的转速
    static public float motor_3_speed = 0.0f;//无人机电机3的转速
    static public float motor_4_speed = 0.0f;//无人机电机4的转速

    //电机的拉力计算的参数
    private float p = 0.0f;//空气密度
    private float ct = 0.0f;//电机升力系数
    private float k1 = 0.0f;//无人机电机1的拉力计算系数
    private float k2 = 0.0f;//无人机电机2的拉力计算系数
    private float k3 = 0.0f;//无人机电机3的拉力计算系数
    private float k4 = 0.0f;//无人机电机4的拉力计算系数

    //电机属性差异（可利用此对电机对无人机的影响)
    private float motor_1_para = 0.0f;//无人机电机1的属性
    private float motor_2_para = 0.0f;//无人机电机2的属性
    private float motor_3_para = 0.0f;//无人机电机3的属性
    private float motor_4_para = 0.0f;//无人机电机4的属性


    //无人机各方向，姿态的控制量
    static public float U1 = 0.0f;//无人机垂直升降控制量
    static public float U2 = 0.0f;//无人机横滚控制量
    static public float U3 = 0.0f;//无人机俯仰控制量
    static public float U4 = 0.0f;//无人机偏航控制量

    static public float U1_before = 0.0f;
    static public float U2_before = 0.0f;
    static public float U3_before = 0.0f;
    static public float U4_before = 0.0f;

    //无人机1对象
  static public   GameObject uav_1_self ;

    //无人机的控制模式
    static public bool uav_control_model = false;//默认自动模式
    static public string control_model = "自动模式";//自动模式||手动模式

    //电机提供的最大力
    static public  float min_motor1_power = 0.0f;//无人机电机1最小的拉力
    static public  float max_motor1_power = 380.0f;//无人机电机1最大的拉力
    static public  float min_motor2_power = 0.0f;//无人机电机2最小的拉力
    static public  float max_motor2_power = 380.0f;//无人机电机2的最大拉力
    static public  float min_motor3_power = 0.0f;//无人机电机3的最小的拉力
    static public  float max_motor3_power = 380.0f;//无人机电机3的最大拉力
    static public  float min_motor4_power = 0.0f;//无人机电机4的最小拉力
    static public  float max_motor4_power = 380.0f;//无人机电机4的最大拉力

    //定点飞行控制

  static public void PID_control()
    {
        //err = height_takeoff - uav_4_self.transform.position.y;
        //intergral += err;
        //U1 = kp * err + ki * intergral + kd * (err - err_last);
        //err_last = err;
        //F1 = F2 = F3 = F4 = U1 / 4;

        //方法1

        //// y轴位置调整
        // err_y = target_y - uav_4_self.transform.position.y;
        // intergral_y += err_y;
        // U1 = kp_y * err_y + ki_y * intergral_y + kd_y * (err_y - err_last_y);
        // err_last_y = err_y;

        // //x轴位置调整
        // err_x = target_x - uav_4_self.transform.position.x;
        // intergral_x += err_x;
        // U3 = kp_x * err_x + ki_x * intergral_x + kd_x * (err_x - err_last_x);
        // err_last_x = err_x;

        // //z轴位置调整
        // err_z = target_z - uav_4_self.transform.position.z;
        // intergral_z += err_z;
        // U2 = kp_z * err_z + ki_z * intergral_z + kd_z * (err_z - err_last_z);
        // err_last_z = err_z;

        // //yaw偏航角调整
        // err_yaw = target_yaw - UAV_4_angle.yaw_now;
        // intergral_yaw += err_yaw;
        // U4 = kp_yaw * err_yaw + ki_yaw * intergral_yaw + kd_yaw * (err_yaw - err_last_yaw);
        // err_last_yaw = err_yaw;

        // F1 = (U1 + U2 + U3 - U4) / 4;
        // F2 = (U1 - U2 - U3 + 3 * U4) / 4;
        // F3 = (U1 - U2 + U3 - U4) / 4;
        // F4 = (U1 + U2 - U3 - U4) / 4;


        // F1 = Mathf.Clamp(F1, 1, 50);
        //  F2 = Mathf.Clamp(F2, 1, 50);
        //  F3 = Mathf.Clamp(F3, 1, 50);
        //  F4 = Mathf.Clamp(F4, 1, 50);

        //方法2

        // y轴位置调整
        err_y = target_y - uav_1_self.transform.position.y;
        intergral_y += err_y;
        U1 = kp_y * err_y + ki_y* intergral_y + kd_y * (err_y - err_last_y);
        err_last_y = err_y;

        //x轴位置调整
        err_x = target_x - uav_1_self.transform.position.x;
        intergral_x += err_x;
        UAV1_1_angle.pitch_now = kp_x * err_x;
        err_last_x = err_x;


        //z轴位置调整
        err_z = target_z - uav_1_self.transform.position.z;
        intergral_z += err_z;
        UAV1_1_angle.roll_now = kp_z * err_z;
        err_last_z = err_z;

        //yaw偏航角调整
        err_yaw = target_yaw - UAV1_1_angle.yaw_now;
        intergral_yaw += err_yaw;
        U4 = kp_yaw * err_yaw + ki_yaw * intergral_yaw + kd_yaw * (err_yaw - err_last_yaw);
        err_last_yaw = err_yaw;


        F1 = F2 = F3 = F4 = U1 / 4;

        if(UAV1_1_angle.pitch_now >0)
        {
            F1 = F1 + UAV1_1_angle.pitch_now / 2;
            F4 = F4 - UAV1_1_angle.pitch_now / 2;

        }

        if (UAV1_1_angle.pitch_now < 0)
        {
            F1 = F1 - UAV1_1_angle.pitch_now / 2;
            F4 = F4 + UAV1_1_angle.pitch_now / 2;

        }

        if(UAV1_1_angle.roll_now>0)
        {
            F1 = F1 + UAV1_1_angle.roll_now / 2;
            F3 = F3 - UAV1_1_angle.roll_now / 2;

        }

        if (UAV1_1_angle.roll_now < 0)
        {
            F1 = F1 - UAV1_1_angle.roll_now / 2;
            F3 = F3 + UAV1_1_angle.roll_now / 2;

        }



    }


    //检查F1、F2、F3、F4是否改变并设置标志位
    void check_update()
    {
        if(F1 != current_F1 || F2 != current_F2 || F3 != current_F3 || F4 != current_F4)
        {
            update_xyz_flag = true;
            update_angle_flag = true;

            current_F1 = F1;
            current_F2 = F2;
            current_F3 = F3;
            current_F4 = F4;

        }
    }

    //无人机拉力参数初始化
    void F_para_init()
    {
        p = 1.0f;
        ct = 0.0000008898f;
        motor_1_para = 1;
        motor_2_para = 1;
        motor_3_para = 1;
        motor_4_para = 1;
        k1 = 0.5f * p * ct * motor_1_para;
        k2 = 0.5f * p * ct * motor_2_para;
        k3 = 0.5f * p * ct * motor_3_para;
        k4 = 0.5f * p * ct * motor_4_para;
    }

    //无人机电机的转速
    void motor_speed()
    {
        motor_1_speed = Mathf.Sqrt(Mathf.Abs( F1) / k1);//无人机电机1的转速
        motor_2_speed = Mathf.Sqrt(Mathf.Abs( F2) / k2);//无人机电机2的转速
        motor_3_speed = Mathf.Sqrt(Mathf.Abs( F3) / k3);//无人机电机3的转速
        motor_4_speed = Mathf.Sqrt(Mathf.Abs( F4) / k4);//无人机电机4的转速

    }
    //无人机各方向的控制量
    void control()
    {
        U1 = F1 + F2 + F3 + F4;//垂直升降控制量
        U2 = 2 * (F1 - F3);//横滚控制量
        U3 = 2 * (F1 - F4);//俯仰控制量
        U4 = F1 - F3 + F2 - F4;//偏航控制量

       
    }
    //检查无人机的控制模式
    void check_control_model()
    {
        if(uav_control_model)
        {
            control_model = "手动模式";
        }
        else
        {
            control_model = "自动模式";
            update_xyz_flag = true;
            update_angle_flag = true;

        }
    }

    //利用按键改变F1、F2、F3、F4的大小
    //F1：A 为加1， S 为减1      F3：G 为加1， H 为减1
    //F2：D 为加1， F 为减1      F4：J 为加1， K 为减1
    void key_scam_control()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UAV1_1_power.F1 = UAV1_1_power.F1 + 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UAV1_1_power.F1 = UAV1_1_power.F1 - 1;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UAV1_1_power.F2 = UAV1_1_power.F2 + 1;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            UAV1_1_power.F2 = UAV1_1_power.F2 - 1;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            UAV1_1_power.F3 = UAV1_1_power.F3 + 1;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            UAV1_1_power.F3 = UAV1_1_power.F3 - 1;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            UAV1_1_power.F4 = UAV1_1_power.F4 + 1;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            UAV1_1_power.F4 = UAV1_1_power.F4 - 1;
        }
    }
    
    //起飞2米
    void take_off()
    {

    }



    // Use this for initialization
    void Start () {

        uav_1_self = GameObject.FindGameObjectWithTag("uav");//无人机1初始化
        
         label_style1 = new GUIStyle();
         label_style2 = new GUIStyle();//GUI的style


        F_para_init();//电机拉力参数初始化

	}
	
	// Update is called once per frame
	void Update () {

        check_control_model();//检查控制模式
        check_update();//检查F1、F2、F3、F4是否改变并设置标志位
        motor_speed();//更新电机速度
        control();//更新各方向的控制量
        key_scam_control();

    }
   static public  void gui_uav4()
    {
       // if (change_control.uav_1_active_flag)
        {
            label_style1.normal.textColor = Color.red;
            label_style2.normal.textColor = Color.black;
            GUILayout.Label("无人机当前位置" + uav_1_self.transform.position, label_style1);//显示无人机的当前位置
            GUILayout.Label("无人机期望位置" + new Vector3(target_x,target_y,target_z), label_style1);//显示无人机的当前位置
            GUILayout.Label("当前横滚角加速度：" + UAV1_1_angle.roll_acc + "  横滚角速度：" + UAV1_1_angle.roll_speed + "  横滚角：" + UAV1_1_angle.roll,label_style1);
            GUILayout.Label("当前俯仰角加速度：" + UAV1_1_angle.pitch_acc + "  俯仰角速度：" + UAV1_1_angle.pitch_speed + "  俯仰角：" + UAV1_1_angle.pitch,label_style1);
            GUILayout.Label("当前偏航角加速度：" + UAV1_1_angle.yaw_acc + "  偏航角速度：" + UAV1_1_angle.yaw_speed + "  偏航角：" + UAV1_1_angle.yaw,label_style1);
            GUILayout.Label("X轴当前的加速度：" + UAV1_1_linear_motion.x_acc + "  X轴的速度" + UAV1_1_linear_motion.x_speed + "  X轴的位移增量" + UAV1_1_linear_motion.x,label_style1);
            GUILayout.Label("Z轴当前的加速度：" + UAV1_1_linear_motion.z_acc + "  Z轴的速度" + UAV1_1_linear_motion.z_speed + "  Z轴的位移增量" + UAV1_1_linear_motion.z,label_style1);
            GUILayout.Label("Y轴当前的加速度：" + UAV1_1_linear_motion.y_acc + "  Y轴的速度" + UAV1_1_linear_motion.y_speed + "  Y轴的位移增量" + UAV1_1_linear_motion.y,label_style1);
            GUILayout.Label("电机1的速度：" + motor_1_speed+"   电机2的速度：" + motor_2_speed + "   电机3的速度：" + motor_3_speed + "   电机4的速度：" + motor_4_speed , label_style1);
            GUILayout.Label("电池的总电量："+ UAV1_1_battery.Cb + "   电池的剩余电量：" + UAV1_1_battery.Creal + "可用电量：" + (UAV1_1_battery.Creal - UAV1_1_battery.Cmin),label_style1);
            GUILayout.Label("当前的F1：" + F1 + "  当前的F2：" + F2 + "  当前的F3：" + F3 + "   当前的F4：" + F4,label_style1);
            GUILayout.Label("当前无人机的控制模式" + control_model,label_style1);//显示无人机的控制模式
            if (GUILayout.Button("自动模式"))
            {
                uav_control_model = false;
            }
            if (GUILayout.Button("手动模式"))
            {
                uav_control_model = true;
                pose_control_flag = false;
            }
            if(GUILayout.Button("定点控制与起飞"))
            {
                if(uav_control_model)
                {
                    
                }
                else
                {
                    pose_control_flag = true;
                }
            }
            if(GUILayout.Button("矩形飞行打开"))
            {
                mission.check_uav1_position_flag = true;
                Station_Manger.mission_uav1_flag = true;
            }
            if(GUILayout.Button("矩行飞行关闭"))
            {
                mission.check_uav1_position_flag = false;
                Station_Manger.mission_uav1_flag = false;
            }

            if(GUILayout.Button("保存激光的数据"))
            {
                keep_laser_data_flag = true;
            }

            if (!uav_control_model)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("期望的偏航角：", label_style2);
                yaw_input = GUILayout.TextField(yaw_input, 10);
                if (GUILayout.Button("确定"))
                {
                    float.TryParse(yaw_input, out target_yaw);//parse 将string类型转换为float类型
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("期望的位置x：", label_style2);
                x_input = GUILayout.TextField(x_input, 10);

                if (GUILayout.Button("确定"))
                {
                    float.TryParse(x_input, out target_x);//parse 将string类型转换为float类型
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("期望的位置y：", label_style2);
                y_input = GUILayout.TextField(y_input, 10);

                if (GUILayout.Button("确定"))
                {
                    float.TryParse(y_input, out target_y);//parse 将string类型转换为float类型
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                GUILayout.Label("期望的位置z：", label_style2);
                z_input = GUILayout.TextField(z_input, 10);

                if (GUILayout.Button("确定"))
                {
                    float.TryParse(z_input, out target_z);//parse 将string类型转换为float类型
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            if (uav_control_model)
            {
                F1 = GUILayout.HorizontalSlider(F1, min_motor1_power, max_motor1_power);//通过滑块改变F1的大小
                GUILayout.Label("当前F1的大小为" + F1,label_style1);//显示F1的当前值
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("F1加1"))
                {
                    F1 = F1 + 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("F1减1"))
                {
                    F1 = F1 - 1;
                }
                GUILayout.EndHorizontal();

                F2 = GUILayout.HorizontalSlider(F2, min_motor2_power, max_motor2_power);//通过滑块改变F2的大小
                GUILayout.Label("当前F2的大小为" + F2,label_style1);//显示F2的当前值
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("F2加1"))
                {
                    F2 = F2 + 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("F2减1"))
                {
                    F2 = F2 - 1;
                }
                GUILayout.EndHorizontal();

                F3 = GUILayout.HorizontalSlider(F3, min_motor3_power, max_motor3_power);//通过滑块改变F3的大小
                GUILayout.Label("当前F3的大小为" + F3,label_style1);//显示F3的当前值

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("F3加1"))
                {
                    F3 = F3 + 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("F3减1"))
                {
                    F3 = F3 - 1;
                }
                GUILayout.EndHorizontal();

                F4 = GUILayout.HorizontalSlider(F4, min_motor4_power, max_motor4_power);//通过滑块改变F4的大小
                GUILayout.Label("当前F4的大小为" + F4,label_style1);//显示F4的当前值

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("F4加1"))
                {
                    F4 = F4 + 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("F4减1"))
                {
                    F4 = F4 - 1;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("整体加1"))
                {
                    F1 = F1 + 1;
                    F2 = F2 + 1;
                    F3 = F3 + 1;
                    F4 = F4 + 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("整体减1"))
                {
                    F1 = F1 - 1;
                    F2 = F2 - 1;
                    F3 = F3 - 1;
                    F4 = F4 - 1;
                }
                GUILayout.Space(10);
                if (GUILayout.Button("起飞"))
                {
                    F1 = 4;
                    F2 = 4;
                    F3 = 4;
                    F4 = 4;

                }
                GUILayout.EndHorizontal();
            }
          
        }
    }
}
