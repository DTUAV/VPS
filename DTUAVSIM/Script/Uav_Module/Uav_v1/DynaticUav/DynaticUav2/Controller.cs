using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.droneV3;
public class Controller : MonoBehaviour
{

    public float Throttle = 0.0f;
    public float Yaw = 0.0f;
    public float Pitch = 0.0f;
    public float Roll = 0.0f;

    public enum ThrottleMode { None, LockHeight };

    [Header("Throttle command")]
    public string ThrottleCommand = "Throttle";
    public bool InvertThrottle = true;

    [Header("Yaw Command")]
    public string YawCommand = "Yaw";
    public bool InvertYaw = false;

    [Header("Pitch Command")]
    public string PitchCommand = "Pitch";
    public bool InvertPitch = true;

    [Header("Roll Command")]
    public string RollCommand = "Roll";
    public bool InvertRoll = true;

    void Update()
    {

        if (ControlModel.ComModel == 0)
        {
            Throttle = Input.GetAxisRaw(ThrottleCommand) * (InvertThrottle ? -1 : 1);
            Yaw = Input.GetAxisRaw(YawCommand) * (InvertYaw ? -1 : 1);
            Pitch = Input.GetAxisRaw(PitchCommand) * (InvertPitch ? -1 : 1);
            Roll = Input.GetAxisRaw(RollCommand) * (InvertRoll ? -1 : 1);
        }
        else if(ControlModel.ComModel == 1)
        {
            Throttle = AutoMission.Throttle;
            Yaw = AutoMission.Yaw;
            Pitch = AutoMission.Pitch;
            Roll = AutoMission.Roll;
            
        }

    }










    //public float Throttle = 0.0f;//电机的油门
    //public float Yaw = 0.0f;//姿态角之偏航角
    //public float Pitch = 0.0f;//姿态角之俯仰角
    //public float Roll = 0.0f;//姿态角之滚转角

    //public enum ThrottleMode { None,LockHeight};//油门的模式：无/定高

    //[Header("Throttle command")]
    //public string ThrottleCommand = "Throttle";//用来定义按键
    //public bool InvertThrottle = true;//控制翻转

    //[Header("Yaw Command")]
    //public string YawCommand = "Yaw";
    //public bool InvertYaw = false;

    //[Header("pitch Command")]
    //public string PitchCommand = "Pitch";
    //public bool InvertPitch = true;

    //[Header("Roll Command")]
    //public string RollCommand = "Roll";
    //public bool InvertRoll = true;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    //读取控制输入
    //    //油门控制：键盘的上下箭头，偏航角控制：键盘的左右箭头
    //    //俯仰角控制：键盘的k和i键，滚转角控制：键盘的j和l键
    //    Throttle = Input.GetAxisRaw(ThrottleCommand) * (InvertThrottle ? -1:1);
    //    Yaw = Input.GetAxisRaw(YawCommand) * (InvertYaw ? -1 : 1);
    //    Pitch = Input.GetAxisRaw(PitchCommand) * (InvertPitch ? -1 : 1);
    //    Roll = Input.GetAxisRaw(RollCommand) * (InvertRoll ? -1 : 1);

    //}
}
