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
using DTUAVCARS.TF;
using LCM.LCM;
using UnityEngine;
using System.Threading;
using DTUAVSIM.Time;
using geometry_msgs;

namespace DTUAV.UAV_Module.Quadrotor
{
    public class MotionWithVelocityNode : MonoBehaviour, LCMSubscriber
    {
        public Rigidbody objectRigidbody;
        public UnityEngine.Vector3 targetVelocity;
        public float maxAngle;
        public float runningHz;
        public float positionPubHz;
        public string targetVelocitySubTopicName;
        public string localPositionPubTopicName;
        public string globalPositionPubTopicName;
        private LCM.LCM.LCM _localPositionPub;
        private LCM.LCM.LCM _globalPositionPub;
        private LCM.LCM.LCM _targetVelocitySub;
        private MotionWithVelocity _motionWithVelocity;
        private UnityEngine.Vector3 _homePosition;
        private Thread _positionPubThread;
        private UnityEngine.Vector3 _globalPosition;
        private UnityEngine.Vector3 _localPosition;
        private bool _isStop;
        private PoseStamp _localPoseStamp;
        private PoseStamp _globalPoseStamp;
        private UnityEngine.Quaternion _rotation;

        private int _sleepTime;

        // Start is called before the first frame update
        void Start()
        {
            _isStop = false;
            _sleepTime = (int) ((1.0 / positionPubHz) * 1000);
            _localPoseStamp = new PoseStamp();
            _localPoseStamp.orientation = new geometry_msgs.Quaternion();
            _localPoseStamp.position = new Point();
            _globalPoseStamp = new PoseStamp();
            _globalPoseStamp.orientation = new geometry_msgs.Quaternion();
            _globalPoseStamp.position = new Point();
            _motionWithVelocity = new MotionWithVelocity(objectRigidbody, targetVelocity, maxAngle, runningHz);
            _targetVelocitySub = new LCM.LCM.LCM();
            _targetVelocitySub.Subscribe(targetVelocitySubTopicName, this);
            _localPositionPub = LCM.LCM.LCM.Singleton;
            _globalPositionPub = LCM.LCM.LCM.Singleton;
            _homePosition = TF.Unity2Ros(objectRigidbody.position);
            _localPosition = UnityEngine.Vector3.zero;
            _globalPosition = TF.Unity2Ros(objectRigidbody.position);
            _rotation = TF.Unity2Ros(objectRigidbody.rotation);
            _positionPubThread = new Thread(PositionPub);
            _positionPubThread.IsBackground = true;
            _positionPubThread.Start();
        }

        void PositionPub()
        {
            while (!_isStop)
            {
                _localPoseStamp.position.x = _localPosition.x;
                _localPoseStamp.position.y = _localPosition.y;
                _localPoseStamp.position.z = _localPosition.z;

                _localPoseStamp.orientation.x = _rotation.x;
                _localPoseStamp.orientation.y = _rotation.y;
                _localPoseStamp.orientation.z = _rotation.z;
                _localPoseStamp.orientation.w = _rotation.w;

                _localPoseStamp.timestamp = SimTime.GetSystemTimeStampMs();

                _globalPoseStamp.position.x = _globalPosition.x;
                _globalPoseStamp.position.y = _globalPosition.y;
                _globalPoseStamp.position.z = _globalPosition.z;

                _globalPoseStamp.orientation.x = _rotation.x;
                _globalPoseStamp.orientation.y = _rotation.y;
                _globalPoseStamp.orientation.z = _rotation.z;
                _globalPoseStamp.orientation.w = _rotation.w;

                _globalPoseStamp.timestamp = SimTime.GetSystemTimeStampMs();

                _localPositionPub.Publish(localPositionPubTopicName, _localPoseStamp);
                _globalPositionPub.Publish(globalPositionPubTopicName, _globalPoseStamp);
                System.Threading.Thread.Sleep(_sleepTime);
            }
        }

        // Update is called once per frame
        void Update()
        {
            _motionWithVelocity.UpdateVelocity(targetVelocity);
            _globalPosition = TF.Unity2Ros(objectRigidbody.position);
            _localPosition = TF.Unity2Ros(objectRigidbody.position) - _homePosition;
            _rotation = TF.Unity2Ros(objectRigidbody.rotation);

        }

        void OnDestroy()
        {
            _motionWithVelocity.SetIsRun(false);
            _isStop = true;
        }

        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            if (channel == targetVelocitySubTopicName)
            {
                TwistStamp targetTwistStamp = new TwistStamp(ins);
                targetVelocity.x = (float) targetTwistStamp.linear.x;
                targetVelocity.y = (float) targetTwistStamp.linear.y;
                targetVelocity.z = (float) targetTwistStamp.linear.z;
            }
        }
    }
}
