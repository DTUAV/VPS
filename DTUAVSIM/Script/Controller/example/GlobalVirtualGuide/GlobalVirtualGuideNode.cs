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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DTUAV.Algorithm_Module.Path_Planning.Control;
using DTUAVCARS.Controller;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class GlobalVirtualGuideNode : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        public string FrameId = "Unity";

        private MessageTypes.Geometry.TwistStamped message;

        [Header("The Config of ROS Subscriber")]
        public PoseStampedSubscriber RUavPositionSub;
        public GuideTypePub RUavGuideTypePub;
        public ControllerOutputSubscriber ControllerOutputSub;


        [Header("The Config of Perception Sensor")]
        public LayerMask Mask;                              //The Environment Layer of Perception Sensor
        public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
        public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
        private environment_perception _environmentPerception;

        [Header("The Config of Global Virtual Guide")]
        public float Omiga;
        public float SafeDistance;
        public float RunningTime;//The running time of physical object, for one control signal input. 
        public int MaxRevisedTime;
        private GlobalVirtualGuide _globalVirtualGuide;
        public float GuideRunningHz;
        public float _minVelocity = -2.0f;
        public float _maxVelocity = 2.0f;

        [Header("The Config of Planning Control Interface")]
        public PlanningControl PlanningControlNode;

        [Header("Data Collection")]
        public bool IsCollection = false;
        public TwistStampedPublisher MpcGuidanceDataPublisherNode;

        private bool _stopThread;

        private int _sleepTime;

        private Vector3 _physicalObjPosition;
        private Vector3 _lastPhysicalObjPosition;
        private Vector3 _physicalObjDirection;
        private Vector3 _controllerOutput;

        private bool _isFirst;
        // Start is called before the first frame update
        void Start()
        {
            base.Start();
            _stopThread = false;
            _isFirst = true;
            _sleepTime = (int)((1.0f / GuideRunningHz) * 1000);
            _environmentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc, SafeDistance);
            _globalVirtualGuide = new GlobalVirtualGuide(RunningTime, _environmentPerception, SafeDistance, Omiga,MaxRevisedTime);
            InitializeMessage();
            _physicalObjPosition = new Vector3();
            _lastPhysicalObjPosition = new Vector3();
            _physicalObjDirection = new Vector3();
            _controllerOutput = new Vector3();
            Loom.RunAsync(
                () =>
                {
                    Thread thread = new Thread(RunGuide);
                    thread.Start();
                }
            );

        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.TwistStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }
        void RunGuide()
        {
            while (!_stopThread)
            {
                Loom.QueueOnMainThread(() =>
                    {
                        if (_isFirst)
                        {
                            _lastPhysicalObjPosition = RUavPositionSub.position;
                            _isFirst = false;
                        }
                        else
                        {
                            message.header.Update();
                            _physicalObjPosition = RUavPositionSub.position;
                            _physicalObjDirection = _physicalObjPosition - _lastPhysicalObjPosition;
                            _lastPhysicalObjPosition = _physicalObjPosition;
                            _controllerOutput = ControllerOutputSub.LinearVelocity;
                            Vector3 output = _globalVirtualGuide.UpdateGuideOutput(_physicalObjPosition, _physicalObjDirection,
                                        _controllerOutput).Unity2Ros();
                            output.x = Mathf.Clamp(output.x, _minVelocity, _maxVelocity);
                            output.y = Mathf.Clamp(output.y, _minVelocity, _maxVelocity);
                            output.z = Mathf.Clamp(output.z, _minVelocity, _maxVelocity);
                            message.twist.linear.x = output.x;
                            message.twist.linear.y = output.y;
                            message.twist.linear.z = output.z;
                            Publish(message);
                            RUavGuideTypePub.PublishMsg(2);
                            if (IsCollection)
                            {
                                MpcGuidanceDataPublisherNode.PublishTwistStampedMsg(output, _controllerOutput.Unity2Ros());
                            }
                            

                        }
                    });
                System.Threading.Thread.Sleep(_sleepTime);
            }
        }
    }
}