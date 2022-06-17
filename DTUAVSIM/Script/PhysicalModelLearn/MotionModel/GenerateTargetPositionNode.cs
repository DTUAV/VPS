/*
© Guandong Univesity of Technology , 2022
Author: Yuanlin Yang (yongwang0808@163.com)

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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DTUAVCARS.DTPlanning;
using RosSharp;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace DTUAV.ModelLearn.MotionModel
{
    public class GenerateTargetPositionNode : MonoBehaviour
    {
        [Header("The Upper Limit of Target Position.")]
        public Vector3 UpperPositionLimit;
        [Header("The Lower Limit of Target Position.")]
        public Vector3 LowerPositionLimit;
        [Header("The Instance of Velocity Command Node.")]
        public VelocityCommand VelocityCommandNode;

        [Header("The Ros Subscriber of Virtual UAV.")]
        public PoseStampedSubscriber VirtualUavPose;

        [Header("The Ros Subscriber of Physical UAV.")]
        public PoseStampedSubscriber PhysicalUavPose;

        [Header("The Running Frequency of This Node.")]
        public float RunningHz;

        [Header("The Error Value of Target Position Switching")]
        public float ErrorValue;

        private GenerateTargetPosition _generateTargetPositionNode;

        private Thread _runThread;

        private int _sleepTime;

        private bool _isRun;

        private Vector3 _targetPosition;

        private Vector3 _virtualUavPosition;

        private Vector3 _physicalUavPosition;
        // Start is called before the first frame update
        void Start()
        {
            _generateTargetPositionNode = new GenerateTargetPosition(UpperPositionLimit,LowerPositionLimit);
            _sleepTime = (int)((1.0 / RunningHz) * 1000);
            _isRun = true;
            _targetPosition = new Vector3(0,1,0);
            _runThread = new Thread(ThreadFun);
            _runThread.IsBackground = true;
            _runThread.Start();
        }

        void ThreadFun()
        {
            while (_isRun)
            {
                _virtualUavPosition = VirtualUavPose.position;
                _physicalUavPosition = PhysicalUavPose.position;
                if (Vector3.Distance(_targetPosition, _virtualUavPosition) <= ErrorValue &&
                    Vector3.Distance(_targetPosition, _physicalUavPosition) <= ErrorValue)
                {
                    _targetPosition = _generateTargetPositionNode.GetTargetPosition();
                }

                VelocityCommandNode.TargetPositionRos = _targetPosition.Unity2Ros();
                
                System.Threading.Thread.Sleep(_sleepTime);
            }

        }

        void OnDestroy()
        {
            _isRun = false;
        }

    }
}
