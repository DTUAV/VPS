/*
© CentraleSupelec, 2017
Author: Dr. Jeremy Fix (jeremy.fix@centralesupelec.fr)

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

// Adjustments to new Publication Timing and Execution Framework
// © Siemens AG, 2018, Dr. Martin Bischoff (martin.bischoff@siemens.com)

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class ControllerOutputSubscriber : UnitySubscriber<MessageTypes.Geometry.TwistStamped>
    {
        public Vector3 LinearVelocity;
        public Vector3 AngularVelocity;

        protected override void Start()
        {
            base.Start();
            LinearVelocity = new Vector3();
            AngularVelocity = new Vector3();
        }
        protected override void ReceiveMessage(MessageTypes.Geometry.TwistStamped message)
        {
            LinearVelocity = ToVector3(message.twist.linear).Ros2Unity();
            AngularVelocity = -ToVector3(message.twist.angular).Ros2Unity();
        }

        private static Vector3 ToVector3(MessageTypes.Geometry.Vector3 geometryVector3)
        {
            return new Vector3((float)geometryVector3.x, (float)geometryVector3.y, (float)geometryVector3.z);
        }
    }
}