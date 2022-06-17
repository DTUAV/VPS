using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V_UAV_1
{
    public class V_UAV_1_State : MonoBehaviour
    {
        [Header("无人机对象")]
        public GameObject Uav;

        [Header("无人机当前的状态")]
        [Header("无人机当前位置")]
        public Vector3 uavPosition;
        [Header("无人机当前的姿态角")]
        public Vector3 uavAngle;
        [Header("无人机当前的速度")]
        public float uavSpeed;
        [Header("无人机当前各轴速度矢量")]
        public Vector3 uavVelocity;
        [Header("无人机当前各轴角速度矢量")]
        public Vector3 uavAngleVelocity;

        [Header("无人机刚体")]
        public Rigidbody rb;

        [Header("无人机坐标系")]
        public Vector3 uavLocalPosition;

        public Quaternion quaternion;
        public Vector3 posePosition;
        public Vector3 angular_velocity;
        public Vector3 linear_velocity;
        public V_UAV_1_ControlMode inputControl;



        public bool getImuDataFlag = false;
        public bool getPoseDataFlag = false;
        public bool getOdometryDataFlag = false;
        public bool getUavStateDataFlag = false;


        private Vector3 last_linear_velocity;
        private Vector3 last_angle_velocity;
        private Vector3 last_position;
        private Vector3 last_angle;
        private Vector3 current_position;
        private float update_time;

        // Start is called before the first frame update
        void Start()
        {
            if (Uav == null)
            {
                Debug.LogError("未指定无人机对象");
            }
            else
            {
                rb = Uav.GetComponent<Rigidbody>();//获取无人机刚体对象
                last_position = rb.position;
                last_angle = rb.rotation.eulerAngles;
                last_linear_velocity = rb.velocity;
                last_angle_velocity = rb.angularVelocity;
            }
        }

        void UpdateMsgState()
        {
            if (!getOdometryDataFlag)
            {
                posePosition = rb.position;
                linear_velocity = rb.velocity;
                quaternion = rb.rotation.normalized;
                angular_velocity = rb.angularVelocity;
                getImuDataFlag = true;
                getPoseDataFlag = true;
                getOdometryDataFlag = true;
                getUavStateDataFlag = true;
            }
        }
        void UpdateState()
        {
            uavPosition = rb.position;//获取无人机的位置  
            uavAngle = rb.rotation.eulerAngles;//获取无人机的姿态角          
                                               // uavAngle = Uav.transform.localEulerAngles;
            uavAngle.x = (uavAngle.x > 180) ? uavAngle.x - 360 : uavAngle.x;
            uavAngle.y = (uavAngle.y > 180) ? uavAngle.y - 360 : uavAngle.y;
            uavAngle.z = (uavAngle.z > 180) ? uavAngle.z - 360 : uavAngle.z;


            uavLocalPosition = new Vector3((uavPosition.x * Mathf.Cos((3.14f / 180) * uavAngle.y)) - (uavPosition.z * Mathf.Sin((3.14f / 180) * uavAngle.y)), uavPosition.y, (uavPosition.x * Mathf.Sin((3.14f / 180) * uavAngle.y)) + (uavPosition.z * Mathf.Cos((3.14f / 180) * uavAngle.y)));
            uavSpeed = rb.velocity.magnitude;//获取当前无人机的速度
            uavVelocity = rb.velocity;//获取当前无人机的速度矢量
            uavAngleVelocity = rb.angularVelocity;//获取当前无人机的角速度矢量


        }


        void FixedUpdate()
        {
            UpdateMsgState();
            if (inputControl.ControlModel == true)
            {
                UpdateState();
            }
        }
    }
}
