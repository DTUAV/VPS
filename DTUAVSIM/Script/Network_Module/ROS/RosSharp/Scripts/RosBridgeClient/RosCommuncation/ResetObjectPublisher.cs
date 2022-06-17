/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
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
    public class ResetObjectPublisher : UnityPublisher<MessageTypes.Std.Float64MultiArray>
    {
        public Transform V_UAV_1_Transform;
        [Header("True-->reset object only one")]
        private MessageTypes.Std.Float64MultiArray _message;
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            InitializeMessage();
        }
        private void InitializeMessage()
        {
            _message = new MessageTypes.Std.Float64MultiArray()
            {
                data = new double[3]
            };
        }
        // Update is called once per frame
        public void PublishReset()
        {
            _message.data[0] = V_UAV_1_Transform.position.Unity2Ros().x;
            _message.data[1] = V_UAV_1_Transform.position.Unity2Ros().y;
            _message.data[2] = V_UAV_1_Transform.position.Unity2Ros().z;
            Publish(_message);
        }

        public void PublishReset(Vector3 pose)
        {
            _message.data[0] = pose.Unity2Ros().x;
            _message.data[1] = pose.Unity2Ros().y;
            _message.data[2] = pose.Unity2Ros().z;
            Publish(_message);
        }
    }
}
