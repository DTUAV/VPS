using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UavForce : MonoBehaviour
{
    // Start is called before the first frame update
    public float FRThrust;
    public float FLThrust;
    public float RRThrust;
    public float RLThrust;

    public InputCommand inputCommand;

    public float YawFactor = 0.5f;
    public float PitchFactor = 0.5f;
    public float RollFactor = 0.5f;

    public MotorForce MotorFR;
    public MotorForce MotorFL;
    public MotorForce MotorRR;
    public MotorForce MotorRL;


    void UpdateThrottle()
    {
        //油门
        if (inputCommand.Throttle >= 0f)
        {
            FRThrust = inputCommand.Throttle;
            FLThrust = inputCommand.Throttle;
            RRThrust = inputCommand.Throttle;
            RLThrust = inputCommand.Throttle;
        }
        //偏航
        if (inputCommand.Yaw > 0f)
        {
            //右转
            float yawdo = (inputCommand.Yaw * YawFactor) / 2;
            FRThrust -= yawdo;
            FLThrust += yawdo;
            RRThrust += yawdo;
            RLThrust -= yawdo;

        }
        else if (inputCommand.Yaw < 0f)
        {
            //左转
            float yawdo = (-inputCommand.Yaw * YawFactor) / 2;
            FRThrust += yawdo;
            FLThrust -= yawdo;
            RRThrust -= yawdo;
            RLThrust += yawdo;
        }

        //前后
        if (inputCommand.Pitch > 0f)
        {
            //向前
            float pitchdo = (inputCommand.Pitch * PitchFactor) / 2;
            FRThrust -= pitchdo;
            FLThrust -= pitchdo;
            RRThrust += pitchdo;
            RLThrust += pitchdo;

        }
        else if (inputCommand.Pitch < 0f)
        {
            //向后
            float pitchdo = (-inputCommand.Pitch * PitchFactor) / 2;
            FRThrust += pitchdo;
            FLThrust += pitchdo;
            RRThrust -= pitchdo;
            RLThrust -= pitchdo;
        }

        //左右
        if (inputCommand.Roll > 0f)
        {
            //向右
            float rolldo = (inputCommand.Roll * RollFactor) / 2;
            FRThrust -= rolldo;
            FLThrust += rolldo;
            RRThrust -= rolldo;
            RLThrust += rolldo;

        }
        else if (inputCommand.Roll < 0f)
        {
            //向左
            float rolldo = (-inputCommand.Roll * RollFactor) / 2;
            FRThrust += rolldo;
            FLThrust -= rolldo;
            RRThrust += rolldo;
            RLThrust -= rolldo;
        }




    }
    void UpdateMotor()
    {
        MotorFR.UpdateForce(FRThrust);
        MotorFL.UpdateForce(FLThrust);
        MotorRR.UpdateForce(RRThrust);
        MotorRL.UpdateForce(RLThrust);
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateThrottle();
    }
    void FixedUpdate()
    {
        UpdateMotor();
    }
}
