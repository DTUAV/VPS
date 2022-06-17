using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.droneV2
{
    public class DroneV2YawControl : MonoBehaviour
    {
        [Header("指定的偏航角")]
        public float refer_yaw;
        [Header("偏航角的PID控制")]
        public DroneV2PidControl yaw_PID;
        [Header("偏航角PID控制输出")]
        public float yawCorrection;
        // Start is called before the first frame update
        [Header("无人机的状态")]
        public DroneV2State uavState;

        [HideInInspector] public bool chage_yaw = false;
        void UpdateYaw()
        {
            yawCorrection = yaw_PID.Update(refer_yaw, uavState.uavAngle.y, Time.fixedDeltaTime);
        }
      
        void FixedUpdate()
        {
            chage_yaw = false;
            UpdateYaw();
            chage_yaw = true;
        }
    }
}