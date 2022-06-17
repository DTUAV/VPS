using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using SimUnity.Sensor.Imu;
using SimUnity.Noise;
namespace RosSharp.RosBridgeClient
{
    public class stateOdometryPub : UnityPublisher<MessageTypes.Nav.Odometry>
    {
        private Thread c_getData;
        public GetUavState getUavState;
        public string FrameId = "Unity";
        private bool endFlag = false;
        private MessageTypes.Nav.Odometry message;
        private Vector3 last_linear_velocity;
        private Vector3 last_angle_velocity;
        private Vector3 last_position;
        private Vector3 last_angle;
        private Vector3 current_angle;
        private float update_time;




        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            c_getData = new Thread(UpdateMsg);
            c_getData.IsBackground = true;
            c_getData.Start();
            last_position = new Vector3(2, 0.1f, 2);
           // last_angle = Vector3.zero;
          //  last_linear_velocity = Vector3.zero;
           // last_angle_velocity = Vector3.zero;
            update_time = Time.fixedDeltaTime;
        }

        private void UpdateMsg()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Nav.Odometry();
            message.child_frame_id = FrameId;
            message.pose = new MessageTypes.Geometry.PoseWithCovariance();
            message.twist = new MessageTypes.Geometry.TwistWithCovariance();

        }
        private void UpdateMessage()
        {
            while (!endFlag)
            {
                if (getUavState.getOdometryDataFlag)
                {

                    message.header.Update();

                    message.pose.pose.position.x = last_position.x + 0.5f * (getUavState.uavVelocity.x + last_linear_velocity.x)* update_time;
                    message.pose.pose.position.y = last_position.y + 0.5f * (getUavState.uavVelocity.y + last_linear_velocity.y) * update_time;
                    message.pose.pose.position.z = last_position.z + 0.5f * (getUavState.uavVelocity.z + last_linear_velocity.z) * update_time;

                    last_position.x =(float)message.pose.pose.position.x;
                    last_position.y = (float)message.pose.pose.position.y;
                    last_position.z = (float)message.pose.pose.position.z;

                    Vector3 current_angular_velocity = getUavState.angular_velocity;
                   current_angle.x = last_angle.x + 0.5f * (current_angular_velocity.x + last_angle_velocity.x) * update_time;
                   current_angle.y = last_angle.y + 0.5f * (current_angular_velocity.y + last_angle_velocity.y) * update_time;
                   current_angle.z = last_angle.z + 0.5f * (current_angular_velocity.z + last_angle_velocity.z) * update_time;
                  // Debug.Log(current_angle*100);

                   Quaternion qua = Quaternion.Euler(current_angle * 100);
                    message.pose.pose.orientation.x = qua.x;
                    message.pose.pose.orientation.y = qua.y;
                    message.pose.pose.orientation.z = qua.z;
                    message.pose.pose.orientation.w = qua.w;

                    last_angle = current_angle;


                    last_angle_velocity = current_angular_velocity;

                    message.twist.twist.angular.x = getUavState.angular_velocity.x;
                    message.twist.twist.angular.y = getUavState.angular_velocity.y;
                    message.twist.twist.angular.z = getUavState.angular_velocity.z;

                    message.twist.twist.linear.x = getUavState.linear_velocity.x;
                    message.twist.twist.linear.y = getUavState.linear_velocity.y;
                    message.twist.twist.linear.z = getUavState.linear_velocity.z;

                    last_linear_velocity = new Vector3(getUavState.uavVelocity.x, getUavState.uavVelocity.y, getUavState.uavVelocity.z);

                    Publish(message);
                    getUavState.getOdometryDataFlag = false;
                }
            }




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