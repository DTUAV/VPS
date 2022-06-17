using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YawControl : MonoBehaviour
{
    [Header("指定的偏航角")]
    public float refer_yaw;
    [Header("偏航角的PID控制")]
    public PIDControl yaw_PID;
    [Header("偏航角PID控制输出")]
    public float yawCorrection;
    // Start is called before the first frame update
    [Header("无人机的状态")]
    public GetUavState uavState;

    void UpdateYaw()
    {
        yawCorrection = yaw_PID.Update(refer_yaw, uavState.uavAngle.y, Time.fixedDeltaTime);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // UpdateYaw();
    }

    void FixedUpdate()
    {
        UpdateYaw();
    }
}
