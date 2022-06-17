using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCommand : MonoBehaviour
{
    [Header("控制量")]
    [Header("油门控制")]
    public float Throttle = 0.0f;//油门控制
    public bool InvertThrottle = true;
    private string ThrottleCommand = "Throttle";

    [Header("偏航角控制")]
    public float Yaw = 0.0f;//偏航角控制
    public bool InvertYaw = false;
    private string YawCommand = "Yaw";

    [Header("俯仰角控制")]
    public float Pitch = 0.0f;//偏航角控制
    public bool InvertPitch = true;
    private string PitchCommand = "Pitch";

    [Header("滚转角控制")]
    public float Roll = 0.0f;//滚转角控制
    public bool InvertRoll = true;
    private string RollCommand = "Roll";

    void UpdateInput()
    {
        Throttle = Input.GetAxisRaw(ThrottleCommand) * (InvertThrottle ? -1 : 1);
        Yaw = Input.GetAxisRaw(YawCommand) * (InvertYaw ? -1 : 1);
        Pitch = Input.GetAxisRaw(PitchCommand) * (InvertPitch ? -1 : 1);
        Roll = Input.GetAxisRaw(RollCommand) * (InvertRoll ? -1 : 1);

       
       
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }
}
