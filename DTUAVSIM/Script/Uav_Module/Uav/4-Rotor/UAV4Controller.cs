using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.Controller;
namespace DTUAVCARS.UAV
{
    public class UAV4Controller : MonoBehaviour
    {
        public float SampleTime;

        public float X_PD_P_Position;
        public float X_PD_D_Position;
        public float X_PD_Result_Position;

        public float Pitch_PD_P_Position;
        public float Pitch_PD_D_Position;
        public float Pitch_PD_Result_Position;


        public float Y_PD_P_Position;
        public float Y_PD_D_Position;
        public float Y_PD_Result_Position;

        public float Roll_PD_P_Position;
        public float Roll_PD_D_Position;
        public float Roll_PD_Result_Position;


        public float Z_PID_P_Position;
        public float Z_PID_I_Position;
        public float Z_PID_D_Position;
        public float Z_PID_Result_Position;

        public float Yaw_PID_P_Position;
        public float Yaw_PID_I_Position;
        public float Yaw_PID_D_Position;
        public float Yaw_PID_Result_Position;

        public Vector3 TargetPositionLocalROS;
        public float TargetYaw;
        public UAV4Implement Uav4ImplementInstance;

        private PD X_PD_Position;
        private PD Y_PD_Position;
        private PID Z_PID_Position;

        private PD Pitch_PD_Position;
        private PD Roll_PD_Position;
        private PID Yaw_PID_Position;

        private Coroutine _runControlCoroutine;
        private bool _isRun;
        private Vector3 taregetLocalPosition;
        private void ControllerInit()
        {
            X_PD_Position = new PD(X_PD_P_Position,X_PD_D_Position);
            Y_PD_Position = new PD(Y_PD_P_Position,Y_PD_D_Position);
            Z_PID_Position = new PID(Z_PID_P_Position,Z_PID_I_Position,Z_PID_D_Position);

            Pitch_PD_Position = new PD(Pitch_PD_P_Position,Pitch_PD_D_Position);
            Roll_PD_Position = new PD(Roll_PD_P_Position,Roll_PD_D_Position);
            Yaw_PID_Position = new PID(Yaw_PID_P_Position,Yaw_PID_I_Position,Yaw_PID_D_Position);
            _isRun = true;
           // _runControlCoroutine = StartCoroutine(RunControl());
        }

        private void RunXControl(float timeUpdate)
        {
            //   X_PD_Result_Position = X_PD_Position.UpdatePD(TargetPositionLocalROS.x, Uav4ImplementInstance.LocalPositionRos.x, timeUpdate);
            //   Pitch_PD_Result_Position =
            //       Pitch_PD_Position.UpdatePD(X_PD_Result_Position, Uav4ImplementInstance.CurrentPitch, timeUpdate);
               X_PD_Result_Position = X_PD_Position.UpdatePD(taregetLocalPosition.z, Uav4ImplementInstance.LocalPosition.z, timeUpdate);
               Pitch_PD_Result_Position =
                  Pitch_PD_Position.UpdatePD(X_PD_Result_Position, Uav4ImplementInstance.CurrentPitch, timeUpdate);
            Pitch_PD_Result_Position = Mathf.Clamp(Pitch_PD_Result_Position, -15, 15);
        }

        private void RunYControl(float timeUpdate)
        {
            // Y_PD_Result_Position = Y_PD_Position.UpdatePD(TargetPositionLocalROS.y,
            //     Uav4ImplementInstance.LocalPositionRos.y, timeUpdate);
            //  Roll_PD_Result_Position =
            //      Roll_PD_Position.UpdatePD(Y_PD_Result_Position, Uav4ImplementInstance.CurrentRoll, timeUpdate);
             Y_PD_Result_Position = Y_PD_Position.UpdatePD(taregetLocalPosition.x,
                 Uav4ImplementInstance.LocalPosition.x, timeUpdate);
              Roll_PD_Result_Position =
                  Roll_PD_Position.UpdatePD(Y_PD_Result_Position, -Uav4ImplementInstance.CurrentRoll, timeUpdate);
            Roll_PD_Result_Position = Mathf.Clamp(Roll_PD_Result_Position, -15, 15);
        }

        private void RunZControl(float timeUpdate)
        {
            Z_PID_Result_Position = Z_PID_Position.UpdatePID(taregetLocalPosition.y,
                Uav4ImplementInstance.LocalPosition.y, timeUpdate);
        }

        private void RunYawControl(float timeUpdate)
        {
            Yaw_PID_Result_Position =
                Yaw_PID_Position.UpdatePID(TargetYaw, Uav4ImplementInstance.CurrentYaw, timeUpdate);
        }

        private IEnumerator RunControl()
        {
            float t = 0;
            while (_isRun)
            {
                t += Time.deltaTime;
                if (t >= SampleTime)
                {
                    taregetLocalPosition = TF.TF.Ros2Unity(TargetPositionLocalROS);
                    taregetLocalPosition = TF.TF.Global2Local(taregetLocalPosition, TargetYaw*Mathf.Deg2Rad);
                    RunXControl(t);
                    RunYControl(t);
                    RunZControl(t);
                    RunYawControl(t);
                    Uav4ImplementInstance.RefHeight = Z_PID_Result_Position;
                    Uav4ImplementInstance.RefPitch = Pitch_PD_Result_Position;
                    Uav4ImplementInstance.RefRoll = Roll_PD_Result_Position;
                    Uav4ImplementInstance.RefYaw = Yaw_PID_Result_Position;
                    t = 0;
                }
                yield return null;
            }
        }

        void Start()
        {
            ControllerInit();
        }

        void FixedUpdate()
        {
            float t = 0.02f;
            taregetLocalPosition = TF.TF.Ros2Unity(TargetPositionLocalROS);
            taregetLocalPosition = TF.TF.Global2Local(taregetLocalPosition, TargetYaw * Mathf.Deg2Rad);
            RunXControl(t);
            RunYControl(t);
            RunZControl(t);
            RunYawControl(t);
            Uav4ImplementInstance.RefHeight = Z_PID_Result_Position;
            Uav4ImplementInstance.RefPitch = Pitch_PD_Result_Position;
            Uav4ImplementInstance.RefRoll = Roll_PD_Result_Position;
            Uav4ImplementInstance.RefYaw = Yaw_PID_Result_Position;
        }
    }
}
