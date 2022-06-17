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
using DTUAVCARS.Controller;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class VirtualGuideRosNode3 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        public string FrameId = "Unity";

        private MessageTypes.Geometry.TwistStamped message;

        [Header("The Config of ROS Subscriber")]
        public PoseStampedSubscriber RUavPositionSub;
        public PredictorXSub RUavPositionXPredictorSub;
        public PredictorYSub RUavPositionYPredictorSub;
        public PredictorZSub RUavPositionZPredictorSub;
        public GuideTypePub RUavGuideTypePub;
        public PoseStampedSubscriber VUavPositionSub;

        [Header("The Config of Perception Sensor")]
        public LayerMask Mask;                              //The Environment Layer of Perception Sensor
        public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
        public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
        private environment_perception _environmentPerception;

        [Header("The Config of Virtual Guider")]
        public int PredictedSteps;
        public List<float> Omiga;
        public float SafeRange;
        public List<Vector3> PredictedPosition;
        public int PredictedWindow;//this equal to predictor predictedWindowX or predictedWindowY or predictedWindowZ
        public float LeftTheta = 45.0f;//theta_1
        public float RightTheta = 45.0f;//theta_2
        public float AdjustGuideRange = 0.8f;

        private VirtualGuide _virtualGuide;

        private Vector3 _currentPosition;

        private Vector3 _predictPositionTemp;

        private Vector3 _safeGuideOutput;

        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _environmentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc, SafeRange);
            _virtualGuide = new VirtualGuide(PredictedSteps, Omiga, SafeRange, _environmentPerception, LeftTheta, RightTheta, AdjustGuideRange);
            PredictedPosition = new List<Vector3>();
            _currentPosition = new Vector3();
            _predictPositionTemp = new Vector3();
            _safeGuideOutput = new Vector3();
            for (int i = 0; i < PredictedWindow; i++)
            {
                PredictedPosition.Add(Vector3.zero);
            }
            InitializeMessage();
            
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
        private void UpdateMessage()
        {
            message.header.Update();
          //  _currentPosition = new Vector3(0,1,0);
            _currentPosition = RUavPositionSub.position;
          //  Debug.Log("CurrentPosition: " + _currentPosition);
          
            for (int i = 0; i < PredictedWindow; i++)
            {
                _predictPositionTemp.x = (float)RUavPositionXPredictorSub.data[i];
                _predictPositionTemp.y = (float)RUavPositionYPredictorSub.data[i];
                _predictPositionTemp.z = (float)RUavPositionZPredictorSub.data[i];
                PredictedPosition[i] = _predictPositionTemp.Ros2Unity();
            }
          
            _predictPositionTemp.x = (float)RUavPositionXPredictorSub.data[0];
            _predictPositionTemp.y = (float)RUavPositionYPredictorSub.data[0];
            _predictPositionTemp.z = (float)RUavPositionZPredictorSub.data[0];

            _virtualGuide.UpdateController3(PredictedPosition, _currentPosition);
            _safeGuideOutput = _virtualGuide.GetControllerOutput();
          //  _safeGuideOutput.y = 0;
            //Debug.Log("SafeCuideOutput: " + _safeGuideOutput);

            if (Mathf.Abs(_safeGuideOutput.x)<10e-6&& Mathf.Abs(_safeGuideOutput.y) < 10e-6&& Mathf.Abs(_safeGuideOutput.z) < 10e-6)
            {
                RUavGuideTypePub.PublishMsg(0);
                message.twist.linear.x = 0;
                message.twist.linear.y = 0;
                message.twist.linear.z = 0;
                Publish(message);
            }
            else
            {
                RUavGuideTypePub.PublishMsg(1);//all guide
                message.twist.linear.x = _safeGuideOutput.Unity2Ros().x;
                message.twist.linear.y = _safeGuideOutput.Unity2Ros().y;
                message.twist.linear.z = _safeGuideOutput.Unity2Ros().z;
                Publish(message);
                Debug.Log("SafeCuideOutput: " + _safeGuideOutput);
            }



        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }
    }
}
