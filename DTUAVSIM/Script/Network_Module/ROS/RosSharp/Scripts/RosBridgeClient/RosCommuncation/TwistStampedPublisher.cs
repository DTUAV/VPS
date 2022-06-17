using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class TwistStampedPublisher : UnityPublisher<RosSharp.RosBridgeClient.MessageTypes.Geometry.TwistStamped>
    {
        public string FrameId = "Unity";

        private RosSharp.RosBridgeClient.MessageTypes.Geometry.TwistStamped message;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void InitializeMessage()
        {
            message = new RosSharp.RosBridgeClient.MessageTypes.Geometry.TwistStamped
            {
                header = new RosSharp.RosBridgeClient.MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        public bool PublishTwistStampedMsg(Vector3 linear_velocity, Vector3 angle_velocity)
        {
            message.header.Update();
            message.twist.linear.x = linear_velocity.x;
            message.twist.linear.y = linear_velocity.y;
            message.twist.linear.z = linear_velocity.z;
            message.twist.angular.x = angle_velocity.x;
            message.twist.angular.y = angle_velocity.y;
            message.twist.angular.z = angle_velocity.z;
            Publish(message);
            return true;
        }

    }
}
