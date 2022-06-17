using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorControl : MonoBehaviour
{
    [Header("该电机提供向上的力")]
    public float UpForce = 0.0f;

    [Header("电机转矩产生的滑动力")]
    public float SideForce = 0.0f;

    [Header("该电机的力分配贡献")]
    public float Power_Auto = 5f;
    public float Power_Man = 0.5f;

    [Header("电机旋转的方向")]
    public bool InvertDirection;

    [Header("对姿态角的影响因子")]
    public float YawFactor_Auto = 0.0f;
    public float PitchFactor_Auto = 0.0f;
    public float RollFactor_Auto = 0.0f;

    public float YawFactor_Man = 0.0f;
    public float PitchFactor_Man = 0.0f;
    public float RollFactor_Man = 0.0f;



    [Header("电机的旋转速度")]
    public float SpeedPropeller = 0;

    [Header("螺旋桨")]
    public GameObject Propeller;

    [Header("电机")]
    public GameObject motor;

    public InputControl inputControl;

    public YawControl yaw_control;

    public void UpdateForce()
    {
        float UpForceTotal;
        if (inputControl.ControlModel == true)
        {
             UpForceTotal = Mathf.Clamp(InputControl.ThrottleValueTo, 0, 1) * Power_Auto;
             UpForceTotal -= UavControl.PitchCorrectionTo * PitchFactor_Auto;
             UpForceTotal -= UavControl.RollCorrectionTo * RollFactor_Auto;
        }
        else
        {
             UpForceTotal = Mathf.Clamp(InputControl.ThrottleValueTo, 0, 1) * Power_Man;
            UpForceTotal -= UavControl.PitchCorrectionTo * PitchFactor_Man;
            UpForceTotal -= UavControl.RollCorrectionTo * RollFactor_Man;


        }


        UpForce = UpForceTotal;
        if(inputControl.ControlModel == true)
        {
            SideForce = PreNormalize(yaw_control.yawCorrection, YawFactor_Auto);
        }
        else
        {
            SideForce = PreNormalize(InputControl.YawToUav, YawFactor_Man);
        }
       
        SpeedPropeller = Mathf.Lerp(SpeedPropeller, UpForce * 1550000.0f, Time.deltaTime);
        UpdatePropeller(SpeedPropeller);
    }

    public void UpdatePropeller(float speed)
    {
        Propeller.transform.Rotate(0.0f, SpeedPropeller * 2 * Time.deltaTime, 0.0f);
    }


    float PreNormalize(float input, float factor)
    {
        float finalValue = input;

        if (InvertDirection)
            finalValue = Mathf.Clamp(finalValue, -1, 0);
        else
            finalValue = Mathf.Clamp(finalValue, 0, 1);

        if(inputControl.ControlModel == true)
        {
            return finalValue * (YawFactor_Auto);
        }
        else
        {
            return finalValue * (YawFactor_Man);
        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
