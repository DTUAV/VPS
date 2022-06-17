using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;

namespace SimUnity.droneV2
{
    public class DroneV2XYZControl : MonoBehaviour
    {
        [Header("xPD控制器")] public DroneV2PdControl xPd;
        [Header("xPD控制器输出的结果")] public float xPd_result;
        [Header("yPID控制器")] public DroneV2PidControl yPid;
        [Header("yPID控制器输出的结果")] public float yPid_result;
        [Header("zPD控制器")] public DroneV2PdControl zPd;
        [Header("zPd控制器输出的结果")] public float zPd_result;

        [Header("PitchPID控制器")] public DroneV2PidControl pitchPID;
        [Header("PitchPID控制器输出结果")] public float pitchPID_result;

        [Header("RollPID控制器")] public DroneV2PidControl rollPID;
        [Header("RollPID控制器输出结果")] public float rollPID_result;

        [Header("无人机的状态")] public DroneV2State uavState;

        [Header("指定无人机坐标")] 
        public float uavPosition_x = 51;
        public float uavPosition_y = 1;
        public float uavPosition_z = 110;

        public float refUavLocalPosition_x;//无人机坐标系的坐标，用于PID调节
        public float refUavLocalPosition_y;
        public float refUavLocalPosition_z;

        public Vector3 uav_init_position;//记录无人机初始状态
        [Header("无人机状态")] 
        public bool takeoff = false;//无人机是否起飞控制
        public bool is_lane = false;//无人机是否降落控制
        public bool lane = true;//判断是否着陆
        public bool IsCollisionCheck = false;//是否需要进行避障
        [Header("如果IsCollisionCkeck为true,需要选择SafeRunning")]
        public safe_uav_running SafeRunning;//避障脚本类
     
        [HideInInspector] public bool chage_ref_position = true;
        public DroneV2ControlMode inputControl;
        private float ref_position_x;
        private float ref_position_y;
        private float ref_position_z;
        private Vector3 refPositionLocal;
        private float uav_init_heigh;
        void Start()
        {
            uav_init_position = uavState.rb.position;
            uav_init_heigh = uavState.rb.position.y;
        }
        //姿态PID控制器
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
        //世界坐标系转无人机机身坐标系
        Vector3 ToLocalPosition(Vector3 worldPosition,float currentYaw)
        {
            Vector3 localPositon = Vector3.zero;
            localPositon.x = worldPosition.x * Mathf.Cos((3.14f / 180) * currentYaw) -
                                    worldPosition.z * Mathf.Sin((3.14f / 180) * currentYaw);
            localPositon.z = worldPosition.x * Mathf.Sin((3.14f / 180) * currentYaw) +
                                    worldPosition.z * Mathf.Cos((3.14f / 180) * currentYaw);
            localPositon.y = worldPosition.y;
            return localPositon;

        }
        //无人机机身坐标系转世界坐标系
        Vector3 ToWorldPosition(Vector3 localPosition, float currentYaw)
        {
            Vector3 worldPosition = Vector3.zero;
            worldPosition.x = localPosition.x * Mathf.Cos((3.14f / 180) * currentYaw) +
                              localPosition.z * Mathf.Sin((3.14f / 180) * currentYaw);
            worldPosition.z = -localPosition.x * Mathf.Sin((3.14f / 180) * currentYaw) +
                              localPosition.z * Mathf.Cos((3.14f / 180) * currentYaw);
            worldPosition.y = localPosition.y;
            return worldPosition;
        }
        //位置PID控制器
        void UpdatePDResult(float ref_position_x,float ref_position_z,float ref_position_y)
        {
         //   refUavLocalPosition_x = ref_position_x * Mathf.Cos((3.14f / 180) * uavState.uavAngle.y) -
        //                            ref_position_z * Mathf.Sin((3.14f / 180) * uavState.uavAngle.y);
         //   refUavLocalPosition_z = ref_position_x * Mathf.Sin((3.14f / 180) * uavState.uavAngle.y) +
         //                          ref_position_z * Mathf.Cos((3.14f / 180) * uavState.uavAngle.y);
         refUavLocalPosition_x = ref_position_x;
         refUavLocalPosition_z = ref_position_z;
            if (is_lane)
            {
                refUavLocalPosition_y = 0;
            }
            else
            {
                refUavLocalPosition_y = ref_position_y;
            }



            zPd_result = zPd.UpdatePD(refUavLocalPosition_z, uavState.uavLocalPosition.z, Time.fixedDeltaTime);
            xPd_result = xPd.UpdatePD(refUavLocalPosition_x, uavState.uavLocalPosition.x, Time.fixedDeltaTime);

            zPd_result = Mathf.Clamp(zPd_result, -5, 5);
            xPd_result = Mathf.Clamp(xPd_result, -3, 3);




            // zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.fixedDeltaTime);
            //  xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.fixedDeltaTime);
            // zPd_result = zPd.UpdatePD(uavPosition_z, uavState.uavPosition.z, Time.deltaTime);
            // xPd_result = xPd.UpdatePD(uavPosition_x, uavState.uavPosition.x, Time.deltaTime);
        } 
        // Start is called before the first frame update

        void FixedUpdate()
        {
            if (IsCollisionCheck)//是否进行避障
            {
                if (SafeRunning.IsWillCollision)//无人机是否将要与障碍物碰撞
                {
                    refPositionLocal = ToLocalPosition(new Vector3(ref_position_x, ref_position_y, ref_position_z),
                        uavState.uavAngle.y);
                    ref_position_x = SafeRunning.UavCurrentPosition.x;//检测到无人机碰撞的无人机位置
                    ref_position_z = SafeRunning.UavCurrentPosition.z;
                    ref_position_y = SafeRunning.UavCurrentPosition.y;
                    float currentUavPositionX = uavPosition_x;
                    float currentUavPositionY = uavPosition_y;
                    float currentUavPositionZ = uavPosition_z;
                    // uavPosition_z = SafeRunning.UavCurrentPosition.z;
                    // uavPosition_x = SafeRunning.UavCurrentPosition.x;
                    // uavPosition_y = SafeRunning.UavCurrentPosition.y;
                    Vector3 currentUavPositionLocal =
                        ToLocalPosition(new Vector3(currentUavPositionX, currentUavPositionY, currentUavPositionZ),
                            uavState.uavAngle.y);
                    //如果无人机目标点是偏离障碍物方向，则赋予无人机目标点，否则无人机保持原位置
                    if (!SafeRunning.isBackCollision && refPositionLocal.z - currentUavPositionLocal.z >= 0)
                    {
                        refPositionLocal.z = currentUavPositionLocal.z;
                    }

                    if (!SafeRunning.isForwardCollision && refPositionLocal.z - currentUavPositionLocal.z <= 0)
                    {
                        refPositionLocal.z = currentUavPositionLocal.z;
                    }

                    if (!SafeRunning.isRightCollision && refPositionLocal.x - currentUavPositionLocal.x <= 0)
                    {
                        refPositionLocal.x = currentUavPositionLocal.x;
                    }

                    if (!SafeRunning.isLeftCollision && refPositionLocal.x - currentUavPositionLocal.x >= 0)
                    {
                        refPositionLocal.x = currentUavPositionLocal.x;
                    }

                    if (!SafeRunning.isUpCollision && refPositionLocal.y - currentUavPositionLocal.y >= 0)
                    {
                        refPositionLocal.y = currentUavPositionLocal.y;
                    }

                    if (!SafeRunning.isDownCollision && refPositionLocal.y - currentUavPositionLocal.y <= 0)
                    {
                        refPositionLocal.y = currentUavPositionLocal.y;
                    }

                    Vector3 refWorldPosition = ToWorldPosition(refPositionLocal, uavState.uavAngle.y);

                    uavPosition_z = refWorldPosition.z;
                    uavPosition_x = refWorldPosition.x;
                    uavPosition_y = refWorldPosition.y;
                }
                else
                {
                    ref_position_x = uavPosition_x;
                    ref_position_z = uavPosition_z;
                    ref_position_y = uavPosition_y;
                    refPositionLocal = ToLocalPosition(new Vector3(ref_position_x, ref_position_y, ref_position_z),
                        uavState.uavAngle.y);
                }
            }

            else
            {
                ref_position_x = uavPosition_x;
                ref_position_z = uavPosition_z;
                ref_position_y = uavPosition_y;
                refPositionLocal = ToLocalPosition(new Vector3(ref_position_x, ref_position_y, ref_position_z),
                    uavState.uavAngle.y);
            }

            if (uavState.uavPosition.y <= uav_init_heigh+0.2)
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
                        UpdatePDResult(refPositionLocal.x, refPositionLocal.z, refPositionLocal.y);
                        UpdatePIDResult();
                        chage_ref_position = true;

                    }
                }
            
        }
    }
}