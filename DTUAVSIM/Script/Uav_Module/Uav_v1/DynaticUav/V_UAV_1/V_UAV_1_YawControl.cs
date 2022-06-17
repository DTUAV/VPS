using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V_UAV_1
{
    public class V_UAV_1_YawControl : MonoBehaviour
    {
        [Header("指定的偏航角")]
        public float refer_yaw;
        [Header("偏航角的PID控制")]
        public V_UAV_1_PidControl yaw_PID;
        [Header("偏航角PID控制输出")]
        public float yawCorrection;
        // Start is called before the first frame update
        [Header("无人机的状态")]
        public V_UAV_1_State uavState;

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