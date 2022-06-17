using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V_UAV_1
{
    
        public class V_UAV_1_BaseControl : MonoBehaviour
        {
            [Header("姿态角限制")]
            public float PitchLimit = 30;
            public float RollLimit = 30;

            [Header("PID控制器")]
            [Header("油门PID控制器")]
            public V_UAV_1_PidControl PidThrottle;
            [Header("控制器输出的油门矫正量")]
            public float HeightCorrection;

            [Header("俯仰角PID控制器")]
            public V_UAV_1_PidControl PidPitch;
            [Header("控制器输出的俯仰角矫正量")]
            public float PitchCorrection;

            [Header("滚转角PID控制器")]
            public V_UAV_1_PidControl PidRoll;
            [Header("控制器输出的滚转角矫正量")]
            public float RollCorrection;

            [Header("电机")]
            public V_UAV_1_Motor[] Motors;


            [Header("无人机对象")]
            public GameObject uav;

            [Header("无人机当前姿态角")]
            public float Pitch;
            public float Roll;
            public float Yaw;
            [Header("无人机当前高度和速度向量")]
            public float Altitude;
            public float VelocityScalar;
            public Vector3 VelocityVector;

            public V_UAV_1_XYZControl xyzControl;
            public float ref_yaw = 0;

            static public float PitchCorrectionTo;
            static public float RollCorrectionTo;
            static public float HeightCorrectionTo;


            float rotateSpeed = 2f;
            Quaternion targetAngels;

            [Header("控制器")]
            public V_UAV_1_ControlMode inputControl;



            public void UpdateAngle()
            {
                Pitch = uav.transform.eulerAngles.x;
                Pitch = (Pitch > 180) ? Pitch - 360 : Pitch;

                Roll = uav.transform.eulerAngles.z;
                Roll = (Roll > 180) ? Roll - 360 : Roll;

                Yaw = uav.transform.eulerAngles.y;
                Yaw = (Yaw > 180) ? Yaw - 360 : Yaw;

                Altitude = uav.transform.position.y;
                VelocityVector = uav.GetComponent<Rigidbody>().velocity;
                VelocityScalar = VelocityVector.magnitude;
            }

            void UpdatePID()
            {
                if (inputControl.ControlModel == false)
                {
                    PitchCorrection = PidPitch.Update(V_UAV_1_ControlMode.PitchToUav * PitchLimit, Pitch, Time.deltaTime);
                    RollCorrection = PidRoll.Update(Roll, V_UAV_1_ControlMode.RollToUav * RollLimit, Time.deltaTime);
                    HeightCorrection = PidThrottle.Update(V_UAV_1_ControlMode.ThrottleToUav * V_UAV_1_ControlMode.IncreaseToUav, VelocityVector.y, Time.deltaTime);

                    PitchCorrectionTo = PitchCorrection;
                    RollCorrectionTo = RollCorrection;
                    HeightCorrectionTo = HeightCorrection;
                }
                else
                {
                    PitchCorrectionTo = xyzControl.pitchPID_result;
                    RollCorrectionTo = xyzControl.rollPID_result;
                    HeightCorrectionTo = xyzControl.yPid_result;
                }



            }
            void ComputeMotors()
            {
                float yaw = 0.0f;
                Rigidbody rb = uav.GetComponent<Rigidbody>();

                int i = 0;
                foreach (V_UAV_1_Motor motor in Motors)
                {
                    motor.UpdateForce();
                    yaw += motor.SideForce;
                    i++;

                    //Transform t = motor.GetComponent<Transform>();
                    Transform t = motor.motor.GetComponent<Transform>();
                    if (inputControl.ControlModel == false)
                    {
                        //手动控制
                        rb.AddForceAtPosition(transform.up * motor.UpForce, t.position, ForceMode.Impulse);
                    }
                    else
                    {
                        //自动控制
                        rb.AddForceAtPosition(transform.up * motor.UpForce, t.position, ForceMode.Force);
                    }

                }
                rb.AddTorque(Vector3.up * yaw, ForceMode.Force);//改变偏航角，暂时不用
                                                                //  targetAngels = Quaternion.Euler(0, ref_yaw, 0);
                                                                //  uav.transform.rotation = Quaternion.Slerp(transform.rotation, targetAngels, rotateSpeed * Time.deltaTime);
                                                                //  if (Quaternion.Angle(targetAngels, transform.rotation) < 1)
                                                                //  {
                                                                //       uav.transform.rotation = targetAngels;
                                                                //   }
            }
            void ComputeMotorSpeeds()
            {
                foreach (V_UAV_1_Motor motor in Motors)
                {
                    if (Altitude < 0.1)
                        motor.UpdatePropeller(0.0f);
                    else
                        motor.UpdatePropeller(5200.0f);
                }
            }


            void FixedUpdate()
            {
                if (!xyzControl.takeoff && xyzControl.lane)
                {

                }
                else
                {
                    UpdateAngle();
                    UpdatePID();
                    ComputeMotors();
                    //  ComputeMotorSpeeds();
                }
            }
        }
    
}