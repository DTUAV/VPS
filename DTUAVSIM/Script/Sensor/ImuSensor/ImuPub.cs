using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using SimUnity.Sensor.Imu;
using SimUnity.Noise;
namespace RosSharp.RosBridgeClient
{
    public class ImuPub : UnityPublisher<MessageTypes.Sensor.Imu>
    {

        private Thread c_getData;
        public GetUavState getUavState;
        public string FrameId = "Unity";
        private bool endFlag = false;
        private Vector3 last_liear_velocity;
        private MessageTypes.Sensor.Imu message;



        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            c_getData = new Thread(UpdateMsg);
            c_getData.IsBackground = true;
            c_getData.Start();
        }

        private void UpdateMsg()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Sensor.Imu();
            message.angular_velocity = new MessageTypes.Geometry.Vector3();
            message.linear_acceleration = new MessageTypes.Geometry.Vector3();
            message.orientation = new MessageTypes.Geometry.Quaternion();
            message.angular_velocity_covariance = new double[9];
            message.linear_acceleration_covariance = new double[9];
            message.orientation_covariance = new double[9];

        }
        private void UpdateMessage()
        {
            while (!endFlag)
            {
                if (getUavState.getImuDataFlag)
                {

                    message.header.Update();
                    message.angular_velocity = GetGeometryVector3(getUavState.angular_velocity);
                    message.linear_acceleration = GetGeometryVector3(getUavState.linear_velocity -last_liear_velocity);
                    last_liear_velocity = getUavState.linear_velocity;
                   message.orientation.x = getUavState.quaternion.x + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                   message.orientation.y = getUavState.quaternion.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                   message.orientation.z = getUavState.quaternion.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                   message.orientation.w = getUavState.quaternion.w + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
                    Publish(message);
                    getUavState.getImuDataFlag = false;
                }
            }



            
        }

        private static MessageTypes.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            MessageTypes.Geometry.Vector3 geometryVector3 = new MessageTypes.Geometry.Vector3();
            geometryVector3.x = vector3.x + GaussNoisPlugin.GaussianNoiseData(0,0.01);
            geometryVector3.y = vector3.y + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            geometryVector3.z = vector3.z + GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            return geometryVector3;
        }
        void OnDestroy()
        {
            endFlag = true;
            if (c_getData.IsAlive)
            {
                c_getData.Abort();
            }

        }

    }
}