using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using SimUnity.Sensor.Imu;
using SimUnity.Noise;
namespace RosSharp.RosBridgeClient
{
    public class uavPosePub : UnityPublisher<MessageTypes.Geometry.PoseStamped>
    {
        private Thread c_getData;
        public GetUavState getUavState;
        public string FrameId = "Unity";
        private bool endFlag = false;
        private MessageTypes.Geometry.PoseStamped message;

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
            message = new MessageTypes.Geometry.PoseStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            while (!endFlag)
            {
                if (getUavState.getPoseDataFlag)
                {

                    message.header.Update();
                    //    message.pose.position = GetGeometryPoint(getUavState.posePosition.Unity2Ros());
                    //  message.pose.orientation = GetGeometryQuaternion(getUavState.quaternion.Unity2Ros());
                       message.pose.position = GetGeometryPoint(getUavState.posePosition + new Vector3( (float)GaussNoisPlugin.GaussianNoiseData(0,0.01), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.01),(float) GaussNoisPlugin.GaussianNoiseData(0, 0.01)));
                       message.pose.orientation = GetGeometryQuaternion(getUavState.quaternion);
                    Publish(message);
                    getUavState.getPoseDataFlag = false;
                }
            }
        }

        private MessageTypes.Geometry.Point GetGeometryPoint(Vector3 position)
        {
            MessageTypes.Geometry.Point geometryPoint = new MessageTypes.Geometry.Point();
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
            return geometryPoint;
        }

        private MessageTypes.Geometry.Quaternion GetGeometryQuaternion(Quaternion quaternion)
        {
            MessageTypes.Geometry.Quaternion geometryQuaternion = new MessageTypes.Geometry.Quaternion();
            geometryQuaternion.x = quaternion.x + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            geometryQuaternion.y = quaternion.y + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            geometryQuaternion.z = quaternion.z + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            geometryQuaternion.w = quaternion.w + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.01);
            return geometryQuaternion;
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