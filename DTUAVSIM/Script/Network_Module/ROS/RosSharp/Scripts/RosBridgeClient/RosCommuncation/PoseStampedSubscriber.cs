/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PoseStampedSubscriber : UnitySubscriber<MessageTypes.Geometry.PoseStamped>
    {
        public Transform PublishedTransform;

        public Vector3 position;
        public Quaternion rotation;
        private bool isMessageReceived;

        public Vector3 Direction;

        public bool isRun = true;
        private Vector3 _lastPosition;
        private bool _isFirst;
        protected override void Start()
        {
			base.Start();
            _isFirst = false;
            position = Vector3.zero;
            Direction = Vector3.zero;
            _lastPosition = Vector3.zero;
        }
		
        private void Update()
        {
            if (isMessageReceived)
                ProcessMessage();
        }

        protected override void ReceiveMessage(MessageTypes.Geometry.PoseStamped message)
        {
            if (!_isFirst)
            {
                position = GetPosition(message).Ros2Unity();
                rotation = GetRotation(message).Ros2Unity();
                isMessageReceived = true;
                Direction = position - _lastPosition;
                _lastPosition = position;
            }
            else
            {
                position = GetPosition(message).Ros2Unity();
                rotation = GetRotation(message).Ros2Unity();
                _lastPosition = position;
                _isFirst = false;
            }
           
        }

        private void ProcessMessage()
        {
            if (isRun)
            {
                PublishedTransform.position = position;
                PublishedTransform.rotation = rotation;
            }
            
        }

        private Vector3 GetPosition(MessageTypes.Geometry.PoseStamped message)
        {
            return new Vector3(
                (float)message.pose.position.x,
                (float)message.pose.position.y,
                (float)message.pose.position.z);
        }

        private Quaternion GetRotation(MessageTypes.Geometry.PoseStamped message)
        {
            return new Quaternion(
                (float)message.pose.orientation.x,
                (float)message.pose.orientation.y,
                (float)message.pose.orientation.z,
                (float)message.pose.orientation.w);
        }
    }
}