using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAV1_1_battery : MonoBehaviour {

    //电机转矩
    static public float M1 = 0.0f;//电机1的转矩
    static public float M2 = 0.0f;//电机2的转矩
    static public float M3 = 0.0f;//电机3的转矩
    static public float M4 = 0.0f;//电机4的转矩

    //螺旋桨的直径
    static public float Dp = 0.0f;//螺旋桨的直径

    //电机参数：电机型号：SunnySky Angela：A2212
    static public float Kt = 0.0f;
    static public float ke = 0.0f;
    static public float Kvo = 0.0f;
    static public float Im1 = 0.0f;//电机1电流
    static public float Im2 = 0.0f;//电机2电流
    static public float Im3 = 0.0f;//电机3电流
    static public float Im4 = 0.0f;//电机4电流
    static public float Im0 = 0.0f;//电机空载电流
    static public float Um1 = 0.0f;//电机1电压
    static public float Um2 = 0.0f;//电机2电压
    static public float Um3 = 0.0f;//电机3电压
    static public float Um4 = 0.0f;//电机4电压
    static public float Um0 = 0.0f;//电机空载电压
    static public float Rm = 0.0f;//电机电阻
    static public float Ue1 = 0.0f;//电调1电压
    static public float Ue2 = 0.0f;//电调2电压
    static public float Ue3 = 0.0f;//电调3电压
    static public float Ue4 = 0.0f;//电调4电压
    static public float Ie1 = 0.0f;//电调1电流
    static public float Ie2 = 0.0f;//电调2电流
    static public float Ie3 = 0.0f;//电调3电流
    static public float Ie4 = 0.0f;//电调4电流
    static public float Re = 0.0f;//电调电阻
    static public float throttle1 = 0.0f;//油门1
    static public float throttle2 = 0.0f;//油门2
    static public float throttle3 = 0.0f;//油门3
    static public float throttle4 = 0.0f;//油门4
    static public float Ubo = 0.0f;//电池电压
    static public float Ub = 0.0f;//当前电池电压
    static public float Ib = 0.0f;//电池电流
    static public float Cb = 0.0f;//电池容量
    static public float Creal = 0.0f;//剩余电池容量
    static public float Rb = 0.0f;//电池电阻
    static public float Cmin = 0.0f;//限制电池的最小容量
    static public float Tb = 0.0f;//可放电时间

    static public float Iother = 0.0f;//其他电流

    //电池参数初始化
    void battery_init()
    {
        Um0 = 20;
        Rm = 0.5f;
        Im0 = 0.12f;
        Kvo = 980;
        ke = (Um0 - Rm * Im0) / (Kvo * Um0);
        Kt = 9.55f * ke;
        Re = 0.008f;
        Rb = 0.016f;
        Ubo = 12;
        Cb = 4000;
        Cmin = 200;
        Dp = 0.233f;

        Creal = Cb;
        Iother = 0;////////////////////////////////////改。。。。。。。。。。。。、、、、、、】】】】】】】】】】】

    }
    //电池状态的更新
    void update_battery()
    {
        M1 = UAV1_1_power.F1 * Dp;
        M2 = UAV1_1_power.F2 * Dp;
        M3 = UAV1_1_power.F3 * Dp;
        M4 = UAV1_1_power.F4 * Dp;

        Im1 = M1 / Kt + Im0;
        Im2 = M2 / Kt + Im0;
        Im3 = M3 / Kt + Im0;
        Im4 = M4 / Kt + Im0;

        Um1 = ke * UAV1_1_power.motor_1_speed + Rm * Im1;
        Um2 = ke * UAV1_1_power.motor_2_speed + Rm * Im2;
        Um3 = ke * UAV1_1_power.motor_3_speed + Rm * Im3;
        Um4 = ke * UAV1_1_power.motor_4_speed + Rm * Im4;

        Ue1 = Um1 + Im1 * Re;
        Ue2 = Um2 + Im2 * Re;
        Ue3 = Um3 + Im3 * Re;
        Ue4 = Um4 + Im4 * Re;

        throttle1 = Ue1 / Ubo;
        throttle2 = Ue2 / Ubo;
        throttle3 = Ue3 / Ubo;
        throttle4 = Ue4 / Ubo;

        Ie1 = throttle1 * Im1;
        Ie2 = throttle2 * Im2;
        Ie3 = throttle3 * Im3;
        Ie4 = throttle4 * Im4;

        Ub = Ubo - Ie1 * Rb - Ie2 * Rb - Ie3 * Rb - Ie4 * Rb;
        Ib = Ie1 + Ie2 + Ie2 + Ie4 + Iother;

        Creal = Creal - Ib*0.000001f;
       

      //  Tb = ((Creal - Cmin) / 0.00001f*Ib) * 0.06f;
      //  Debug.Log("Tb" + Tb);
    }

    // Use this for initialization
    void Start () {

        battery_init();


    }
	
	// Update is called once per frame
	void Update () {
        
       

    }
    void FixedUpdate()
    {
        update_battery();
    }
}
