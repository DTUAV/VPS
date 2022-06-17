using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V_UAV_1
{
    public class V_UAV_1_XYZControl : MonoBehaviour
    {
        [Header("xPD控制器")]
        public V_UAV_1_PdControl xPd;
        [Header("xPD控制器输出的结果")]
        public float xPd_result;
        [Header("yPID控制器")]
        public V_UAV_1_PidControl yPid;
        [Header("yPID控制器输出的结果")]
        public float yPid_result;
        [Header("zPD控制器")]
        public V_UAV_1_PdControl zPd;
        [Header("zPd控制器输出的结果")]
        public float zPd_result;

        [Header("PitchPID控制器")]
        public V_UAV_1_PidControl pitchPID;
        [Header("PitchPID控制器输出结果")]
        public float pitchPID_result;

        [Header("RollPID控制器")]
        public V_UAV_1_PidControl rollPID;
        [Header("RollPID控制器输出结果")]
        public float rollPID_result;

        [Header("无人机的状态")]
        public V_UAV_1_State uavState;

        [Header("指定无人机坐标")]
        public float uavPosition_x = 51;
        public float uavPosition_y = 1;
        public float uavPosition_z = 110;

        public float refUavLocalPosition_x;
        public float refUavLocalPosition_y;
        public float refUavLocalPosition_z;

        [Header("无人机状态")]
        public bool takeoff = false;

        public bool lane = true;
        [HideInInspector] public bool chage_ref_position = true;
        public V_UAV_1_ControlMode inputControl;
        void UpdatePIDResult()
        {
            // yPid_result = yPid.Update(uavPosition_y,uavState.uavPosition.y,Time.deltaTime);
            //  pitchPID_result = pitchPID.Update(zPd_result, uavState.uavAngle.x, Time.deltaTime);
            //  rollPID_result = rollPID.Update(xPd_result, -uavState.uavAngle.z, Time.deltaTime);
            // pitchPID_result = Mathf.Clamp(pitchPID_result, -1, 1);
            // rollPID_result = Mathf.Clamp(rollPID_result, -1, 1);
            yPid_result = yPid.Update(refUavLocalPosition_y, uavState.uavPosition.y, Time.fixedDeltaTime);
            pitchPID_result = pitchPID.Update(zPd_result, uavState.uavAngle.x, Time.fixedDeltaTime);
            rollPID_result = rollPID.Update(xPd_result, -uavState.uavAngle.z, Time.fixedDeltaTime);


        }

        void UpdatePDResult()
        {
            refUavLocalPosition_x = uavPosition_x * Mathf.Cos((3.14f / 180) * uavState.uavAngle.y) - uavPosition_z * Mathf.Sin((3.14f / 180) * uavState.uavAngle.y);
            refUavLocalPosition_z = uavPosition_x * Mathf.Sin((3.14f / 180) * uavState.uavAngle.y) + uavPosition_z * Mathf.Cos((3.14f / 180) * uavState.uavAngle.y);
            refUavLocalPosition_y = uavPosition_y;


            zPd_result = zPd.UpdatePD(refUavLocalPosition_z, uavState.uavLocalPosition.z, Time.fixedDeltaTime);
            xPd_result = xPd.UpdatePD(refUavLocalPosition_x, uavState.uavLocalPosition.x, Time.fixedDeltaTime);

            zPd_result = Mathf.Clamp(zPd_result, -3, 3);
            xPd_result = Mathf.Clamp(xPd_result, -3, 3);




            // zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.fixedDeltaTime);
            //  xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.fixedDeltaTime);
            // zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.deltaTime);
            // xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.deltaTime);
        }
        // Start is called before the first frame update

        void FixedUpdate()
        {
            if (uavState.uavPosition.y <= 0.5f)
            {
                lane = true;
            }
            else
            {
                lane = false;
            }
            if (!takeoff && lane)
            {

            }
            else
            {
                if (inputControl.ControlModel == true)
                {
                    chage_ref_position = false;
                    UpdatePDResult();
                    UpdatePIDResult();
                    chage_ref_position = true;

                }
            }
        }
    }
}
