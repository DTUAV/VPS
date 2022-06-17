using RosSharp.RosBridgeClient.MessageTypes.Sensor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Sensor.Imu;
namespace RosSharp.RosBridgeClient
{
    public class ImuSensorDataPub : UnityPublisher<MessageTypes.Sensor.Imu>
    {

        public ImuSensor imuSensor;
        public string frame_id;
        private MessageTypes.Sensor.Imu message;
     
        

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
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
            message.header.frame_id = frame_id;
        }
        private void UpdateMessage()
        {
            message.angular_velocity = GetGeometryVector3(imuSensor.angularVelocity);
            message.linear_acceleration = GetGeometryVector3(imuSensor.velocity);
            message.orientation.x = imuSensor.orientation.x;
            message.orientation.y = imuSensor.orientation.y;
            message.orientation.z = imuSensor.orientation.z;
            message.orientation.w = imuSensor.orientation.w;
            Publish(message);
        }

        private static MessageTypes.Geometry.Vector3 GetGeometryVector3(Vector3 vector3)
        {
            MessageTypes.Geometry.Vector3 geometryVector3 = new MessageTypes.Geometry.Vector3();
            geometryVector3.x = vector3.x;
            geometryVector3.y = vector3.y;
            geometryVector3.z = vector3.z;
            return geometryVector3;
        }
        
    }


}
