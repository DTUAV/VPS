using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.droneV2;
using SimUnity.droneV3;
public class ComputerModule : MonoBehaviour
{
    [Header("Settings")]
    [Range(0, 90)] public float PitchLimit;
    [Range(0, 90)] public float RollLimit;

    [Header("手动控制PID")]
    [Header("Parts")]
    public PID PidThrottle;
    public PID PidPitch;
    public PID PidRoll;
    public BasicGyro Gyro;


    [Header("自动控制PID")]
    [Header("Parts")]
    public PID PidThrottleAuto;
    public PID PidPitchAuto;
    public PID PidRollAuto;


    [Header("Values")]
    public float PitchCorrection;
    public float RollCorrection;
    public float HeightCorrection;


   public Transform uav_transform;
    public void UpdateComputer(float ControlPitch, float ControlRoll, float ControlHeight)
    {
        UpdateGyro();
        if (ControlModel.ComModel == 1)
        {
            //PitchCorrection = PidPitchAuto.Update(AutoMission.PitchAim, Gyro.Pitch, Time.deltaTime);
            // RollCorrection = PidRollAuto.Update(AutoMission.RollAim, Gyro.Roll, Time.deltaTime);
            // HeightCorrection = PidThrottleAuto.Update(AutoMission.height, Gyro.Altitude, Time.deltaTime);
            //  HeightCorrection = PidThrottleAuto.Update(2, Gyro.Altitude, Time.deltaTime);
            //  PitchCorrection = 0;
            //  RollCorrection = 0;
            PitchCorrection = PidPitch.Update(ControlPitch * PitchLimit, Gyro.Pitch, Time.deltaTime);
            RollCorrection = PidRoll.Update(ControlRoll * RollLimit, Gyro.Roll, Time.deltaTime);
            HeightCorrection = PidThrottle.Update(ControlHeight, Gyro.Altitude, Time.deltaTime);


        }
        else
        {
            PitchCorrection = PidPitch.Update(ControlPitch * PitchLimit, Gyro.Pitch, Time.deltaTime);
            RollCorrection = PidRoll.Update(Gyro.Roll, ControlRoll * RollLimit, Time.deltaTime);
            HeightCorrection = PidThrottle.Update(ControlHeight, Gyro.VelocityVector.y, Time.deltaTime);
           
        }
    }

    public void UpdateGyro()
    {
        Gyro.UpdateGyro(uav_transform);
    }






    /*[Header("无人对象")]
    public Transform UavTransform;

    [Header("Settings")]
    [Range(0, 90)] public float PitchLimit;//俯仰角限制
    [Range(0, 90)] public float RollLimit;//滚转角限制

    [Header("Parts")]
    public PID PidThrottle;//油门PID
    public PID PidPitch;//俯仰角PID
    public PID PidRoll;//滚转角PID
    public BasicGyro Gyro;//获取无人接的姿态角和高度和速度信息

    [Header("Values")]
    public float PitchCorrection;//当前俯仰角通过PID得到的修正值
    public float RollCorrection;//当前滚转角通过PID得到的修正值
    public float HeightCorrection;//当前高度通过PID得到的修正值

    public void UpdateComputer(float ControlPitch, float ControlRoll, float ControlHeight)
    {
        UpdateGyro();
        PitchCorrection = PidPitch.Update(ControlPitch * PitchLimit, Gyro.Pitch, Time.deltaTime);
        RollCorrection = PidRoll.Update(ControlRoll * RollLimit, Gyro.Roll, Time.deltaTime);
        HeightCorrection = PidThrottle.Update(ControlHeight, Gyro.VelocityVector.y, Time.deltaTime);
    }



    public void UpdateGyro()
    {
        Gyro.UpdateGyro(UavTransform);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
