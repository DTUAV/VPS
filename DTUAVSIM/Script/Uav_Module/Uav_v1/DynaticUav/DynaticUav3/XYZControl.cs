using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYZControl : MonoBehaviour
{
    [Header("xPD控制器")]
    public PDControl xPd;
    [Header("xPD控制器输出的结果")]
    public float xPd_result;
    [Header("yPID控制器")]
    public PIDControl yPid;
    [Header("yPID控制器输出的结果")]
    public float yPid_result;
    [Header("zPD控制器")]
    public PDControl zPd;
    [Header("zPd控制器输出的结果")]
    public float zPd_result;

    [Header("PitchPID控制器")]
    public PIDControl pitchPID;
    [Header("PitchPID控制器输出结果")]
    public float pitchPID_result;

    [Header("RollPID控制器")]
    public PIDControl rollPID;
    [Header("RollPID控制器输出结果")]
    public float rollPID_result;

    [Header("无人机的状态")]
    public GetUavState uavState;

    [Header("指定无人机坐标")]
    public float uavPosition_x = 1;
    public float uavPosition_y = 2;
    public float uavPosition_z = 3;

    


    public InputControl inputControl;
    void UpdatePIDResult()
    {
       // yPid_result = yPid.Update(uavPosition_y,uavState.uavPosition.y,Time.deltaTime);
      //  pitchPID_result = pitchPID.Update(zPd_result, uavState.uavAngle.x, Time.deltaTime);
      //  rollPID_result = rollPID.Update(xPd_result, -uavState.uavAngle.z, Time.deltaTime);
        // pitchPID_result = Mathf.Clamp(pitchPID_result, -1, 1);
        // rollPID_result = Mathf.Clamp(rollPID_result, -1, 1);
        yPid_result = yPid.Update(uavPosition_y, uavState.uavPosition.y, Time.fixedDeltaTime);
        pitchPID_result = pitchPID.Update(zPd_result, uavState.uavAngle.x, Time.fixedDeltaTime);
        rollPID_result = rollPID.Update(xPd_result, -uavState.uavAngle.z, Time.fixedDeltaTime);


    }

    void UpdatePDResult()
    {
        

        zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.fixedDeltaTime);
        xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.fixedDeltaTime);
       // zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.deltaTime);
       // xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.deltaTime);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(inputControl.ControlModel == true)
       // {
       //     UpdatePIDResult();
       //     UpdatePDResult();

      //  }
      
    }

    void FixedUpdate()
    {
        if (inputControl.ControlModel == true)
        {
            UpdatePDResult();
            UpdatePIDResult();
           

        }
    }
}
